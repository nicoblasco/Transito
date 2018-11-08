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

namespace GestionDeTurnos.Controllers
{
    public class TrackingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string ModuleDescription = "MenuPrincipal";
        public string WindowDescription = "Atencion";


        // GET: Trackings
        public ActionResult Index()
        {
            //Obtengo el numero de sector de esta maquina
            string IP = Request.UserHostName;
            string terminalName = CompNameHelper.DetermineCompName(IP);
            List<Setting> setting = db.Settings.ToList();
            int[] statusOrden = { 2,3 };
            int? CantidadDeLlamadosPosibles = setting.Where(x => x.Clave == "CANTIDAD_DE_LLAMADOS").FirstOrDefault().Numero1;
            Terminal terminal = db.Terminals.Where(x => x.IP == terminalName).FirstOrDefault();
            ViewBag.HabilitaLlamarNuevamente = true;

            if (terminal != null)
            {
                ViewBag.Terminal = terminal.Descripcion;
                // var trackings = db.Trackings.Include(t => t.Sector).Include(t => t.Terminal).Include(t => t.Turn).Include(t => t.Usuario).Where(x => x.FechaIngreso == null  && x.TerminalID==terminal.Id);
                //List<Tracking> trackings =db.Trackings.Where(x => x.Status.Orden == 1 && x.SectorID == terminal.SectorID).OrderBy(x => x.FechaCreacion).Take(20).ToList();

                //Me fijo si todavia tiene un turno asignado
                //Esto por si cerro la ventana o algo por el estilo

                Tracking tracking = db.Trackings.Where(x => statusOrden.Contains(x.Status.Orden)  && x.SectorID == terminal.SectorID && x.TerminalID==terminal.Id).FirstOrDefault();

                if (tracking!=null)
                {
                    if (tracking.CantidadDeLlamados>=CantidadDeLlamadosPosibles)
                        ViewBag.HabilitaLlamarNuevamente = false;
                }
                
                

                return View(tracking);
                // return View(trackings.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Turns");
            }

        }




        [HttpPost]
        public JsonResult GetTurnosPendientes()
        {
            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59
            List<Tracking> list = new List<Tracking>();
            string IP = Request.UserHostName;
            string terminalName = CompNameHelper.DetermineCompName(IP);
            Terminal terminal = db.Terminals.Where(x => x.IP == terminalName).FirstOrDefault();
            try
            {
                list = db.Trackings.Where(x => x.Status.Orden == 1 && x.SectorID == terminal.SectorID && x.Turn.FechaTurno >= startDateTime && x.Turn.FechaTurno<= endDateTime ).OrderBy(x => x.FechaCreacion).Take(20).ToList();

                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public JsonResult Llamar()
        {
            string IP = Request.UserHostName;
            string terminalName = CompNameHelper.DetermineCompName(IP);

            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59
            Tracking tracking = new Tracking();
            Status status= db.Status.Where(x => x.Orden == 2).FirstOrDefault();

            Terminal terminal = db.Terminals.Where(x => x.IP == terminalName).FirstOrDefault();
            

            if (terminal == null)
            {
                return Json(new { responseCode = "-10" });
            }

            try
            {
                tracking = db.Trackings.Where(x => x.Status.Orden == 1 && x.SectorID == terminal.SectorID && x.Turn.FechaTurno >= startDateTime && x.Turn.FechaTurno <= endDateTime).OrderBy(x => x.FechaCreacion).FirstOrDefault();

                if (tracking==null)
                {
                    //No hay ningun turno pendiente
                    return Json(new { responseCode = "2" });
                }
                else
                {
                    //Actualizo 
                    tracking.StatusID = status.Id;
                    tracking.Status = status;
                    tracking.TerminalID = terminal.Id;
                    tracking.UsuarioID = SessionHelper.GetUser();
                    tracking.CantidadDeLlamados = tracking.CantidadDeLlamados + 1;
                    db.Entry(tracking).State = EntityState.Modified;
                    db.SaveChanges();


                }

                var responseObject = new
                {
                    responseCode = 0,
                    tracking
                };

                return Json(responseObject);
            }
            catch (Exception)
            {

                return Json(new { responseCode = "-10" });
            };

        }


        public JsonResult LlamarNuevamente(int id)
        {
            Tracking tracking = new Tracking();
            List<Setting> setting = db.Settings.ToList();
            int? CantidadDeLlamadosPosibles = setting.Where(x => x.Clave == "CANTIDAD_DE_LLAMADOS").FirstOrDefault().Numero1;
            bool PuedeSeguirLlamando = true;

            try
            {
                tracking = db.Trackings.Find(id);

                if (tracking == null)
                {
                    //No hay ningun turno pendiente
                    return Json(new { responseCode = "2" });
                }
                else
                {
                    //Actualizo 
                    tracking.CantidadDeLlamados = tracking.CantidadDeLlamados + 1;
                    db.Entry(tracking).State = EntityState.Modified;
                    db.SaveChanges();
                    if (tracking.CantidadDeLlamados >= CantidadDeLlamadosPosibles.Value)
                        PuedeSeguirLlamando = false;
                    else
                        PuedeSeguirLlamando = true;
                }

                var responseObject = new
                {
                    responseCode = 0,
                    PuedeSeguirLlamando


                };

                return Json(responseObject);
            }
            catch (Exception)
            {

                return Json(new { responseCode = "-10" });
            };

        }



        public JsonResult IniciarAtencion(int id)
        {

            Tracking tracking = new Tracking();
            Status status = db.Status.Where(x => x.Orden == 3).FirstOrDefault();

            try
            {
                tracking = db.Trackings.Find(id);

                if (tracking == null)
                {
                    //No hay ningun turno pendiente
                    return Json(new { responseCode = "2" });
                }
                else
                {
                    //Actualizo 
                    tracking.StatusID = status.Id;
                    tracking.Status = status;
                    tracking.UsuarioID = SessionHelper.GetUser();
                    tracking.FechaIngreso = DateTime.Now;
                    db.Entry(tracking).State = EntityState.Modified;
                    db.SaveChanges();


                }

                var responseObject = new
                {
                    responseCode = 0,
                    tracking
                };

                return Json(responseObject);
            }
            catch (Exception)
            {

                return Json(new { responseCode = "-10" });
            };

        }



        public JsonResult FinalizarAtencion(int id)
        {

            Tracking tracking = new Tracking();
            Status status = db.Status.Where(x => x.Orden == 4).FirstOrDefault();
            int EstadoInicial = db.Status.Where(x => x.Orden == 1).Select(x => x.Id).FirstOrDefault();
            int OrdenDeSectorActual;
            int? SectorProximo;

            try
            {
                tracking = db.Trackings.Find(id);

                if (tracking == null)
                {
                    //No hay ningun turno pendiente
                    return Json(new { responseCode = "2" });
                }
                else
                {
                    //Actualizo 
                    tracking.StatusID = status.Id;
                    tracking.Status = status;
                    tracking.FechaSalida = DateTime.Now;
                    tracking.Tiempo = tracking.FechaSalida - tracking.FechaIngreso;
                    db.Entry(tracking).State = EntityState.Modified;
                    db.SaveChanges();

                    //Genero el nuevo tracking. Va al siguiente estadio


                    //Obtengo el id del workflow del tipo de tramite
                    Workflow workflow = db.Workflows.Where(x => x.TypesLicenseID == tracking.Turn.TypesLicense.Id).FirstOrDefault();

                    //Obtengo la lista de sectores para el tipo de tramite
                    List<SectorWorkflow> sectorWorkflows = db.SectorWorkflows.Where(x => x.Workflow.Id == workflow.Id).ToList();

                    //obtengo el orden del sector actual
                    OrdenDeSectorActual =sectorWorkflows.Where(x => x.SectorID == tracking.SectorID).Select(x => x.Orden).FirstOrDefault() ;

                    //obtengo el proximo sector
                    SectorProximo = sectorWorkflows.Where(x => x.Orden == OrdenDeSectorActual +1 ).Select(x => x.SectorID).FirstOrDefault();

                    //Si tiene proximo Sector

                    if (SectorProximo != null)
                    {
                        Tracking newtracking = new Tracking
                        {
                            SectorID = SectorProximo.Value,
                            TurnID = tracking.Turn.Id,
                            FechaCreacion = DateTime.Now,
                            StatusID = EstadoInicial
                        };

                        db.Trackings.Add(newtracking);
                        db.SaveChanges();
                    }

                }

                var responseObject = new
                {
                    responseCode = 0
                };

                return Json(responseObject);
            }
            catch (Exception)
            {

                return Json(new { responseCode = "-10" });
            };

        }


        public JsonResult NoSePresento(int id)
        {

            Tracking tracking = new Tracking();
            Status status = db.Status.Where(x => x.Orden == 5).FirstOrDefault();

            try
            {
                tracking = db.Trackings.Find(id);

                if (tracking == null)
                {
                    //No hay ningun turno pendiente
                    return Json(new { responseCode = "2" });
                }
                else
                {
                    //Actualizo 
                    tracking.StatusID = status.Id;
                    tracking.Status = status;
                    tracking.FechaIngreso = DateTime.Now;
                    tracking.FechaSalida = DateTime.Now;
                    tracking.Tiempo = tracking.FechaSalida - tracking.FechaIngreso;
                    db.Entry(tracking).State = EntityState.Modified;
                    db.SaveChanges();

                }

                var responseObject = new
                {
                    responseCode = 0
                };

                return Json(responseObject);
            }
            catch (Exception)
            {

                return Json(new { responseCode = "-10" });
            };

        }


        public JsonResult Incompleto(int id)
        {

            Tracking tracking = new Tracking();
            Status status = db.Status.Where(x => x.Orden == 6).FirstOrDefault();

            try
            {
                tracking = db.Trackings.Find(id);

                if (tracking == null)
                {
                    //No hay ningun turno pendiente
                    return Json(new { responseCode = "2" });
                }
                else
                {
                    //Actualizo 
                    tracking.StatusID = status.Id;
                    tracking.Status = status;
                    tracking.FechaSalida = DateTime.Now;
                    tracking.Tiempo = tracking.FechaSalida - tracking.FechaIngreso;
                    db.Entry(tracking).State = EntityState.Modified;
                    db.SaveChanges();

                }

                var responseObject = new
                {
                    responseCode = 0
                };

                return Json(responseObject);
            }
            catch (Exception)
            {

                return Json(new { responseCode = "-10" });
            };

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
