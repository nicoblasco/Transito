using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GestionDeTurnos.Enumerations;
using GestionDeTurnos.Helpers;
using GestionDeTurnos.Models;
using GestionDeTurnos.Tags;
using GestionDeTurnos.ViewModel;

namespace GestionDeTurnos.Controllers
{
    [AutenticadoAttribute]
    public class TrackingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string ModuleDescription = "Menu Principal";
        public string WindowDescription = "Atencion al cliente";


        // GET: Trackings
        public ActionResult Index()
        {
            if (!PermissionViewModel.TienePermisoAcesso(WindowHelper.GetWindowId(ModuleDescription, WindowDescription)))
                return View("~/Views/Shared/AccessDenied.cshtml");
            //Obtengo el numero de sector de esta maquina
            //string IP = Request.UserHostName;
            //string terminalName = CompNameHelper.DetermineCompName(IP);
            string terminalName = Request.UserHostName;
            List<Setting> setting = db.Settings.ToList();
            int[] statusOrden = { 2,3 };
            int? CantidadDeLlamadosPosibles = setting.Where(x => x.Clave == "CANTIDAD_DE_LLAMADOS").FirstOrDefault().Numero1;
            Terminal terminal = db.Terminals.Where(x => x.IP == terminalName && x.Enable==true).FirstOrDefault();
            ViewBag.HabilitaLlamarNuevamente = true;
            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59

            if (terminal != null)
            {
                ViewBag.Terminal = terminal.Descripcion;
                // var trackings = db.Trackings.Include(t => t.Sector).Include(t => t.Terminal).Include(t => t.Turn).Include(t => t.Usuario).Where(x => x.FechaIngreso == null  && x.TerminalID==terminal.Id);
                //List<Tracking> trackings =db.Trackings.Where(x => x.Status.Orden == 1 && x.SectorID == terminal.SectorID).OrderBy(x => x.FechaCreacion).Take(20).ToList();

                //Me fijo si todavia tiene un turno asignado
                //Esto por si cerro la ventana o algo por el estilo

                    //Lo filtro por turnos del dia tambien???
                Tracking tracking = db.Trackings.Where(x => statusOrden.Contains(x.Status.Orden) && x.Enable==true  && x.SectorID == terminal.SectorID && x.TerminalID==terminal.Id && x.FechaCreacion>=startDateTime && x.FechaCreacion<=endDateTime ).FirstOrDefault();

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


        
        public ActionResult Turnero()
        {
            //Obtengo el numero de sector de esta maquina
            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59
            List<Setting> setting = db.Settings.ToList();
            ViewBag.Video = db.Settings.Where(x => x.Clave == "VIDEO").Select(x => x.Texto1).FirstOrDefault();
            //  int? CantidadDeLlamadosPosibles = setting.Where(x => x.Clave == "CANTIDAD_DE_LLAMADOS").FirstOrDefault().Numero1;
            int[] statusOrden = { 2, 3,4,5,6 };

                List< Tracking> trackings = db.Trackings.Where(x => statusOrden.Contains(x.Status.Orden) && x.Terminal!=null && x.Enable == true && x.Turn.FechaIngreso >= startDateTime && x.Turn.FechaIngreso <= endDateTime).OrderBy(x => new { x.Status.Orden, x.FechaIngreso }).Take(10).ToList();




                return View(trackings);


        }


        [HttpPost]
        public JsonResult GetTurnosPendientes()
        {
            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59
            List<Tracking> list = new List<Tracking>();
            //string IP = Request.UserHostName;
            //string terminalName = CompNameHelper.DetermineCompName(IP);
            string terminalName = Request.UserHostName;
            Terminal terminal = db.Terminals.Where(x => x.IP == terminalName && x.Enable==true ).FirstOrDefault();
            try
            {
                list = db.Trackings.Where(x => x.Status.Orden == 1 && x.Enable == true && x.SectorID == terminal.SectorID && x.Turn.FechaIngreso >= startDateTime && x.Turn.FechaIngreso <= endDateTime ).OrderBy(x => x.FechaCreacion).Take(20).ToList();

                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpPost]
        public JsonResult GetTurnosTurnero()
        {
            //Obtengo el numero de sector de esta maquina
            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59
            List<Setting> setting = db.Settings.ToList();
            //  int? CantidadDeLlamadosPosibles = setting.Where(x => x.Clave == "CANTIDAD_DE_LLAMADOS").FirstOrDefault().Numero1;
            int[] statusOrden = { 2, 3, 4, 5, 6 };
            
            try
            {
                List<Tracking> trackings = db.Trackings.Where(x => statusOrden.Contains(x.Status.Orden) && x.Enable == true && x.Turn.FechaIngreso >= startDateTime && x.Turn.FechaIngreso <= endDateTime).OrderByDescending(x => x.FechaCreacion).Take(10).ToList();

                return Json(trackings, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public JsonResult UpdateAlerta(int id)
        {
            Tracking tracking = new Tracking();

            try
            {
                tracking = db.Trackings.Find(id);


                    //Actualizo 
                    tracking.Alerta = false;
                    db.Entry(tracking).State = EntityState.Modified;
                    db.SaveChanges();




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


        public JsonResult Llamar()
        {
            //string IP = Request.UserHostName;
            //string terminalName = CompNameHelper.DetermineCompName(IP);
            string terminalName = Request.UserHostName;
            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59
            Tracking tracking = new Tracking();
            Status status= db.Status.Where(x => x.Orden == 2).FirstOrDefault();

            Terminal terminal = db.Terminals.Where(x => x.IP == terminalName && x.Enable == true).FirstOrDefault();
            

            if (terminal == null)
            {
                return Json(new { responseCode = "-10" });
            }

            try
            {
                tracking = db.Trackings.Where(x => x.Status.Orden == 1 && x.Enable == true && x.SectorID == terminal.SectorID && x.Turn.FechaIngreso >= startDateTime && x.Turn.FechaIngreso <= endDateTime).OrderBy(x => x.FechaCreacion).FirstOrDefault();

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
                    tracking.Alerta = true;
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
                    tracking.Alerta = true;
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
                    tracking.Alerta = false;
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
            string estadoLicencia = db.Settings.Where(x => x.Clave == "ESTADOS_LICENCIAS" && x.Numero1 == 1).FirstOrDefault().Texto1;
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
                    tracking.Alerta = false;
                    db.Entry(tracking).State = EntityState.Modified;
                    db.SaveChanges();

                    //Genero el nuevo tracking. Va al siguiente estadio


                    //Obtengo el id del workflow del tipo de tramite
                    Workflow workflow = db.Workflows.Where(x => x.TypesLicenseID == tracking.Turn.TypesLicense.Id).FirstOrDefault();

                    //Obtengo la lista de sectores para el tipo de tramite
                    List<SectorWorkflow> sectorWorkflows = db.SectorWorkflows.Where(x => x.Workflow.Id == workflow.Id).ToList();

                    //obtengo el orden del sector actual
                    OrdenDeSectorActual = tracking.Orden; //sectorWorkflows.Where(x => x.SectorID == tracking.SectorID).Select(x => x.Orden).FirstOrDefault() ;

                    //obtengo el proximo sector
                    SectorProximo = sectorWorkflows.Where(x => x.Orden == OrdenDeSectorActual +1 ).Select(x => x.SectorID).FirstOrDefault();

                    //Si tiene proximo Sector

                    if (SectorProximo != 0)
                    {
                        Tracking newtracking = new Tracking
                        {
                            SectorID = SectorProximo.Value,
                            TurnID = tracking.Turn.Id,
                            FechaCreacion = DateTime.Now,
                            Alerta = false,
                            Enable = true,
                            StatusID = EstadoInicial,
                            Orden = OrdenDeSectorActual+1
                        };

                        db.Trackings.Add(newtracking);
                        db.SaveChanges();
                    }
                    else
                    {
                        //Finaliza la atencion
                        Turn turn = db.Turns.Find(tracking.TurnID);
                        turn.FechaSalida = DateTime.Now;
                        turn.Tiempo = turn.FechaSalida - turn.FechaIngreso;
                        db.Entry(turn).State = EntityState.Modified;
                        db.SaveChanges();

                        //Creo la licencia y la dejo en espera
                        License license = new License
                        {
                            PersonId = turn.PersonID,
                            TurnId = turn.Id,
                            TypesLicenseId = turn.TypesLicenseID,
                            Estado = estadoLicencia
                        };

                        db.Licenses.Add(license);
                        db.SaveChanges();
                    }

                }

                var responseObject = new
                {
                    responseCode = 0
                };

                return Json(responseObject);
            }
            catch (Exception e)
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
                    tracking.Alerta = false;
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
                    tracking.Alerta = false;
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

        public ActionResult Details(int id)
        {
            string ModuleDescription = "Menu Principal";
            string WindowDescription = "Busquedas";
            if (!PermissionViewModel.TienePermisoAcesso(WindowHelper.GetWindowId(ModuleDescription, WindowDescription)))
                return View("~/Views/Shared/AccessDenied.cshtml");

            Tracking tracking = db.Trackings.Find(id);
            if (tracking == null)
            {
                return HttpNotFound();
            }

            ViewBag.listaTiposLicencia = new List<TypesLicense>(db.TypesLicenses.ToList());
            return View(tracking);
        }


        public ActionResult Edit(int id)
        {
            string ModuleDescription = "Menu Principal";
            string WindowDescription = "Busquedas";
            if (!PermissionViewModel.TienePermisoAcesso(WindowHelper.GetWindowId(ModuleDescription, WindowDescription)))
                return View("~/Views/Shared/AccessDenied.cshtml");

            Tracking tracking = db.Trackings.Find(id);
            if (tracking == null)
            {
                return HttpNotFound();
            }

            ViewBag.listaTiposLicencia = new List<TypesLicense>(db.TypesLicenses.ToList());
            ViewBag.listaEstados = new List<Status>(db.Status.ToList());
            return View(tracking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,StatusID,CantidadDeLlamados ")] Tracking vmtracking)
        {
            if (ModelState.IsValid)
            {
                Tracking tracking = db.Trackings.Find(vmtracking.Id);                
                tracking.StatusID = vmtracking.StatusID;
                tracking.CantidadDeLlamados = vmtracking.CantidadDeLlamados;

                db.Entry(tracking).State = EntityState.Modified;
                db.SaveChanges();

                int OrdenDeSectorActual;
                int? SectorProximo;

                if (tracking.Status.Orden == (int)StatusOrden.FINALIZADO)
                {
                    string estadoLicencia = db.Settings.Where(x => x.Clave == "ESTADOS_LICENCIAS" && x.Numero1 == 1).FirstOrDefault().Texto1;
                    int EstadoInicial = db.Status.Where(x => x.Orden == 1).Select(x => x.Id).FirstOrDefault();
                    //Obtengo el id del workflow del tipo de tramite
                    Workflow workflow = db.Workflows.Where(x => x.TypesLicenseID == tracking.Turn.TypesLicense.Id).FirstOrDefault();

                    //Obtengo la lista de sectores para el tipo de tramite
                    List<SectorWorkflow> sectorWorkflows = db.SectorWorkflows.Where(x => x.Workflow.Id == workflow.Id).ToList();
                    //obtengo el orden del sector actual
                    OrdenDeSectorActual = tracking.Orden; //sectorWorkflows.Where(x => x.SectorID == tracking.SectorID).Select(x => x.Orden).FirstOrDefault() ;

                    //obtengo el proximo sector
                    SectorProximo = sectorWorkflows.Where(x => x.Orden == OrdenDeSectorActual + 1).Select(x => x.SectorID).FirstOrDefault();

                    //Si tiene proximo Sector

                    if (SectorProximo != 0)
                    {
                        Tracking newtracking = new Tracking
                        {
                            SectorID = SectorProximo.Value,
                            TurnID = tracking.Turn.Id,
                            FechaCreacion = DateTime.Now,
                            Alerta = false,
                            Enable = true,
                            StatusID = EstadoInicial,
                            Orden = OrdenDeSectorActual + 1
                        };

                        db.Trackings.Add(newtracking);
                        db.SaveChanges();
                    }
                    else
                    {
                        //Finaliza la atencion
                        Turn turn = db.Turns.Find(tracking.TurnID);
                        turn.FechaSalida = DateTime.Now;
                        turn.Tiempo = turn.FechaSalida - turn.FechaIngreso;
                        db.Entry(turn).State = EntityState.Modified;
                        db.SaveChanges();

                        //Creo la licencia y la dejo en espera
                        License license = new License
                        {
                            PersonId = turn.PersonID,
                            TurnId = turn.Id,
                            TypesLicenseId = turn.TypesLicenseID,
                            Estado = estadoLicencia
                        };
                        db.Licenses.Add(license);
                        db.SaveChanges();
                    }
                }


                return RedirectToAction("Search","Turns");
            }

            ViewBag.listaTiposLicencia = new List<TypesLicense>(db.TypesLicenses.ToList());
            ViewBag.listaEstados = new List<Status>(db.Status.ToList());
            return RedirectToAction("Search");
        }

        public JsonResult DeleteTracking(int id)
        {
            if (id == 0)
            {
                return Json(new { responseCode = "-10" });
            }

            Tracking tracking = db.Trackings.Find(id);
            tracking.Enable = false;
            db.Entry(tracking).State = EntityState.Modified;
            db.SaveChanges();

            AuditHelper.Auditar("Baja", "Id -" + tracking.Id.ToString() + " / Turno -" + tracking.Turn.Turno, "Trackings", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);


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
