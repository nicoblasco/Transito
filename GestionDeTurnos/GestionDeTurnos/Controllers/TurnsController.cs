using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GestionDeTurnos.Helpers;
using GestionDeTurnos.Models;
using GestionDeTurnos.ViewModel;

namespace GestionDeTurnos.Controllers
{
    public class TurnsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Turns
        public ActionResult Index()
        {
            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59
            int[] statusOrdenPendiente = { 2, 3};
            var timeSpanList = new List<TimeSpan>();
            var timeSpanListDemora = new List<TimeSpan>();
            var totalTicks = 0L;
            var totalTicksDemora = 0L;

            List<Turn> turns = db.Turns.Where(x =>  x.FechaTurno >= startDateTime && x.FechaTurno <= endDateTime).OrderBy(x => new {x.FechaIngreso }).ToList();

            List<Tracking> trackings = db.Trackings.Where(x => x.Turn.FechaTurno >= startDateTime && x.Turn.FechaTurno <= endDateTime).ToList();

            foreach (var item in trackings.Where(x => x.FechaSalida != null).ToList())
            {
                timeSpanList.Add(item.Tiempo.Value);                
            }

            foreach (var ts in timeSpanList)
            {
                totalTicks += ts.Ticks;
            }

            if (timeSpanList.Count > 0)
            {
                var avgTicks = totalTicks / timeSpanList.Count;
                var avgTimeSpan = new TimeSpan(avgTicks);
                ViewBag.MinPromedioAtencion = avgTimeSpan.Minutes;
            }
            else
            {
                ViewBag.MinPromedioAtencion = 0;
            }

            //Demora
            foreach (var item in trackings.Where(x => x.FechaSalida == null).ToList())
            {
                TimeSpan timeSpanDemora;
                timeSpanDemora = DateTime.Now - item.FechaCreacion;

                timeSpanListDemora.Add(timeSpanDemora);
            }

            foreach (var ts in timeSpanListDemora)
            {
                totalTicksDemora += ts.Ticks;
            }

            if (timeSpanListDemora.Count > 0)
            {
                var avgTicks = totalTicksDemora / timeSpanListDemora.Count;
                var avgTimeSpan = new TimeSpan(avgTicks);
                ViewBag.MinPromedioDemora= avgTimeSpan.Minutes;
            }
            else
            {
                ViewBag.MinPromedioDemora = 0;
            }


            //TurnosProgramados
            ViewBag.TurnosProgramados = trackings.Where(x => turns.Contains(x.Turn)).Count();

            //Pendientes: Pendientes de ser atendidos por algun box
            //El cliente cuando ingresa se agredita y genera un primer regristro.
            ViewBag.TurnosPendientes = trackings.Where(x => turns.Contains(x.Turn) && x.Status.Orden ==1 && x.FechaIngreso == null ).Count();


            //En proceso: Los que empezaron el ciclo (han sido llamados por algun box) y no finalizaron
            ViewBag.TurnosEnProceso = trackings.Where(x => turns.Contains(x.Turn) && statusOrdenPendiente.Contains(x.Status.Orden)).Count();
            //Completados: Los que han finalizado todo el ciclo
            ViewBag.TurnosCompletados = turns.Where(x => x.FechaSalida != null ).Count();


            return View(turns.ToList());


        }

        [HttpPost]
        public JsonResult GetTurns()
        {
            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59
            try
            {
                List<Tracking> trackings = db.Trackings.Where(x => x.Turn.FechaTurno >= startDateTime && x.Turn.FechaTurno <= endDateTime).ToList();
                List<TurnIndexViewModel> turnsvm = new List<TurnIndexViewModel>();
                List<Turn> turns = db.Turns.Where(x => x.FechaTurno >= startDateTime && x.FechaTurno <= endDateTime).OrderBy(x => new { x.FechaIngreso }).Take(1000).ToList();
                //Obtengo el primer Sector del Workflow para el tipo de tramite
                List<Workflow> workflows = db.Workflows.ToList();

                foreach (var item in turns)
                {
                    Workflow workflow = workflows.Where(x => x.TypesLicenseID == item.TypesLicenseID).FirstOrDefault();
                    int intSectorActual = trackings.Where(x => x.TurnID == item.Id).Select(x => x.SectorID).LastOrDefault();
                    int intOrdenSectorActual = workflow.SectorWorkflows.Where(x => x.SectorID == intSectorActual).Select(x => x.Orden).FirstOrDefault();

                    TurnIndexViewModel turn = new TurnIndexViewModel {
                        Id = item.Id,
                        Turno = item.Turno,
                        Tipo = item.TypesLicense.Descripcion,
                        Apellido = item.Person.Apellido,
                        Nombre = item.Person.Nombre,
                        Ingreso = item.FechaIngreso.ToShortDateString(),
                        Salida = item.FechaSalida?.ToShortDateString(),
                        Estado = trackings.Where(x => x.TurnID == item.Id).Select(x => x.Status.Descripcion).LastOrDefault(),
                      //  SectorAnterior = workflow.SectorWorkflows.Where(x => x.SectorID != intSectorActual && x.Orden == intOrdenSectorActual -1).Select(x => x.Sector.Descripcion).FirstOrDefault(),
                        SectorActual = trackings.Where(x => x.TurnID == item.Id).Select(x => x.Sector.Descripcion).LastOrDefault(),
                        SectorProximo = workflow.SectorWorkflows.Where(x => x.SectorID != intSectorActual && x.Orden == intOrdenSectorActual +1).Select(x => x.Sector.Descripcion).FirstOrDefault()


                    };

                    turnsvm.Add(turn);
                }



                return Json(turnsvm, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpPost]
        public JsonResult GetTurnsPendientes()
        
        {
            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59
            List<TurnPendientesViewModel> turnsvm = new List<TurnPendientesViewModel>();
            try
            {
                List<Tracking> trackings = db.Trackings.Where(x => x.FechaCreacion>= startDateTime && x.FechaCreacion<= endDateTime && x.Status.Orden == 1 && x.FechaIngreso == null).ToList();


                foreach (var item in trackings)
                {
                    
                    TurnPendientesViewModel turn = new TurnPendientesViewModel
                    {
                        Id = item.Id,
                        Turno = item.Turn.Turno,
                        Ingreso = item.FechaCreacion.ToString("dd/MM/yyyy HH:mm:ss"),
                        Sector = item.Sector.Descripcion,
                        Demora = string.Format("{0:hh\\:mm\\:ss}",  DateTime.Now - item.FechaCreacion )

                    };

                    turnsvm.Add(turn);
                }



                return Json(turnsvm, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public JsonResult GetTurnsProgramados()
        {
            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59
            List<TurnsProgramadosViewModel> turnsvm = new List<TurnsProgramadosViewModel>();
            try
            {
                List<Turn> turns = db.Turns.Where(x => x.FechaTurno >= startDateTime && x.FechaTurno <= endDateTime).ToList();

                foreach (var item in turns)
                {

                    TurnsProgramadosViewModel turn = new TurnsProgramadosViewModel
                    {
                        Id = item.Id,
                        Turno = item.Turno,
                        Apellido = item.Person.Apellido,
                        Nombre = item.Person.Nombre,
                        Dni = item.Person.Dni,
                        Horario = item.FechaTurno.ToString("HH:mm:ss"),
                        Tipo = item.TypesLicense.Descripcion

                    };

                    turnsvm.Add(turn);
                }



                return Json(turnsvm, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpPost]
        public JsonResult GetTurnsPromedio()
        {
            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59
            List<TurnPromedioViewModel> turnsvm = new List<TurnPromedioViewModel>();
            try
            {
                List<Tracking> trackings = db.Trackings.Where(x => x.Turn.FechaTurno >= startDateTime && x.Turn.FechaTurno <= endDateTime && x.FechaSalida != null).ToList();

                foreach (var item in trackings)
                {

                    TurnPromedioViewModel turn = new TurnPromedioViewModel
                    {
                        Id = item.Id,
                        Turno = item.Turn.Turno,
                        Ingreso = item.FechaIngreso?.ToString("dd/MM/yyyy HH:mm:ss"),
                        Salida = item.FechaSalida?.ToString("dd/MM/yyyy HH:mm:ss"),
                        Sector = item.Sector.Descripcion,
                        Tiempo = string.Format("{0:hh\\:mm\\:ss}", item.Tiempo),
                        Usuario = item.Usuario.Nombreusuario

                    };

                    turnsvm.Add(turn);
                }



                return Json(turnsvm, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpPost]
        public JsonResult GetTurnsPromedioDemora()
        {
            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59
            List<TurnDemoraViewModel> turnsvm = new List<TurnDemoraViewModel>();
            try
            {
                List<Tracking> trackings = db.Trackings.Where(x => x.Turn.FechaTurno >= startDateTime && x.Turn.FechaTurno <= endDateTime && x.FechaIngreso == null).ToList();

                foreach (var item in trackings)
                {

                    TurnDemoraViewModel turn = new TurnDemoraViewModel
                    {
                        Id = item.Id,
                        Desde = item.FechaCreacion.ToLongDateString(),
                        Sector = item.Sector.Descripcion,
                        Tiempo = string.Format("{0:hh\\:mm\\:ss}", DateTime.Now - item.FechaCreacion),
                        Turno = item.Turn.Turno
                    };

                    turnsvm.Add(turn);
                }



                return Json(turnsvm, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public JsonResult GetTurnsEnProceso()

        {
            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59
            List<TurnEnProcesoViewModel> turnsvm = new List<TurnEnProcesoViewModel>();
            int[] statusOrdenPendiente = { 2, 3 };
            try
            {
                List<Tracking> trackings = db.Trackings.Where(x => x.FechaCreacion >= startDateTime && x.FechaCreacion <= endDateTime && statusOrdenPendiente.Contains(x.Status.Orden)).ToList();


                foreach (var item in trackings)
                {

                    TurnEnProcesoViewModel turn = new TurnEnProcesoViewModel
                    {
                        Id = item.Id,
                        Turno = item.Turn.Turno,
                        Ingreso = item.FechaCreacion.ToString("dd/MM/yyyy HH:mm:ss"),
                        Sector = item.Sector.Descripcion,
                        Tiempo = string.Format("{0:hh\\:mm\\:ss}", DateTime.Now - item.FechaIngreso),
                        Usuario = item.Usuario.Nombreusuario

                    };

                    turnsvm.Add(turn);
                }



                return Json(turnsvm, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public JsonResult GetTurnsFinzalizados()

        {
            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59
            List<TurnFinalizadoViewModel> turnsvm = new List<TurnFinalizadoViewModel>();
            int[] statusOrdenPendiente = { 2, 3 };
            try
            {
                List<Turn> turns   = db.Turns.Where(x => x.FechaIngreso >= startDateTime && x.FechaIngreso <= endDateTime && x.FechaSalida != null ).ToList();


                foreach (var item in turns)
                {

                    TurnFinalizadoViewModel turn = new TurnFinalizadoViewModel
                    {
                        Id = item.Id,
                        Turno = item.Turno,
                        Ingreso = item.FechaIngreso.ToString("dd/MM/yyyy HH:mm:ss"),
                        Tiempo = string.Format("{0:hh\\:mm\\:ss}", item.Tiempo),
                        Finalizado = item.FechaSalida?.ToString("dd/MM/yyyy HH:mm:ss"),
                        FechaTurno = item.FechaTurno.ToString("dd/MM/yyyy HH:mm:ss")



                    };

                    turnsvm.Add(turn);
                }



                return Json(turnsvm, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public ActionResult Search()
        {
            var turns = db.Turns.Include(t => t.Person).Include(t => t.TypesLicense);
            return View(turns.ToList());
        }


        public ActionResult Licenses()
        {
            var turns = db.Turns.Include(t => t.Person).Include(t => t.TypesLicense);
            return View(turns.ToList());
        }


        // GET: Turns/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Turn turn = db.Turns.Find(id);
            if (turn == null)
            {
                return HttpNotFound();
            }
            return View(turn);
        }

        // GET: Turns/Create
        public ActionResult Create()
        {
            ViewBag.PersonID = new SelectList(db.People, "Id", "Nombre");
            ViewBag.TypesLicenseID = new SelectList(db.TypesLicenses, "Id", "Descripcion");
            return View();
        }

        // POST: Turns/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Turno,TypesLicenseID,PersonID")] Turn turn)
        {
            if (ModelState.IsValid)
            {
                db.Turns.Add(turn);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PersonID = new SelectList(db.People, "Id", "Nombre", turn.PersonID);
            ViewBag.TypesLicenseID = new SelectList(db.TypesLicenses, "Id", "Descripcion", turn.TypesLicenseID);
            return View(turn);
        }

        // GET: Turns/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Turn turn = db.Turns.Find(id);
            if (turn == null)
            {
                return HttpNotFound();
            }
            ViewBag.PersonID = new SelectList(db.People, "Id", "Nombre", turn.PersonID);
            ViewBag.TypesLicenseID = new SelectList(db.TypesLicenses, "Id", "Descripcion", turn.TypesLicenseID);
            return View(turn);
        }

        // POST: Turns/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Turno,TypesLicenseID,PersonID")] Turn turn)
        {
            if (ModelState.IsValid)
            {
                db.Entry(turn).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PersonID = new SelectList(db.People, "Id", "Nombre", turn.PersonID);
            ViewBag.TypesLicenseID = new SelectList(db.TypesLicenses, "Id", "Descripcion", turn.TypesLicenseID);
            return View(turn);
        }

        // GET: Turns/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Turn turn = db.Turns.Find(id);
            if (turn == null)
            {
                return HttpNotFound();
            }
            return View(turn);
        }

        // POST: Turns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Turn turn = db.Turns.Find(id);
            db.Turns.Remove(turn);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
