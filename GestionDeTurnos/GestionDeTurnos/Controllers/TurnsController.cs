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
    public class TurnsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string ModuleDescription = "Menu Principal";
        public string WindowDescription = "Busquedas";
        // GET: Turns
        public ActionResult Index()
        {
            if (!PermissionViewModel.TienePermisoAcesso(WindowHelper.GetWindowId(ModuleDescription, WindowDescription)))
                return View("~/Views/Shared/AccessDenied.cshtml");
            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59
            int[] statusOrdenPendiente = { 2, 3};
            var timeSpanList = new List<TimeSpan>();
            var timeSpanListDemora = new List<TimeSpan>();
            var totalTicks = 0L;
            var totalTicksDemora = 0L;
            List<CallCenterTurn> centerTurns = db.CallCenterTurns.Where(x => x.FechaTurno >= startDateTime && x.FechaTurno <= endDateTime).ToList();
            List<Turn> turns = db.Turns.Where(x =>  x.FechaTurno >= startDateTime && x.Enable == true && x.FechaTurno <= endDateTime).OrderBy(x => new {x.FechaIngreso }).ToList();

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
                ViewBag.MinPromedioAtencion = (int)avgTimeSpan.TotalMinutes;
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
                ViewBag.MinPromedioDemora= (int)avgTimeSpan.TotalMinutes;
            }
            else
            {
                ViewBag.MinPromedioDemora = 0;
            }


            ViewBag.TurnosProgramados = centerTurns.Count();


            //Pendientes: Pendientes de ser atendidos por algun box
            //El cliente cuando ingresa se agredita y genera un primer regristro.
                ViewBag.TurnosPendientes = trackings.Where(x => turns.Contains(x.Turn) && x.Status.Orden ==1 && x.Enable == true && x.FechaIngreso == null ).Count();


            //En proceso: Los que empezaron el ciclo (han sido llamados por algun box) y no finalizaron
            ViewBag.TurnosEnProceso = trackings.Where(x => turns.Contains(x.Turn) && x.Enable == true && statusOrdenPendiente.Contains(x.Status.Orden)).Count();
            //Completados: Los que han finalizado todo el ciclo
            ViewBag.TurnosCompletados = turns.Where(x => x.FechaSalida != null && x.Enable == true).Count();


            return View(turns.ToList());


        }


        public ActionResult Search()
        {
            if (!PermissionViewModel.TienePermisoAcesso(WindowHelper.GetWindowId(ModuleDescription, WindowDescription)))
                return View("~/Views/Shared/AccessDenied.cshtml");
            List<TypesLicense> lTypesLicense = new List<TypesLicense>();
            lTypesLicense = db.TypesLicenses.OrderBy(x => x.Descripcion).ToList();
            ViewBag.listaLicencias = lTypesLicense;

            ViewBag.Editar = PermissionViewModel.TienePermisoAcesso(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Ver = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Baja = PermissionViewModel.TienePermisoBaja(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            return View();
        }

        public ActionResult SearchByTurn(string NroTurno)
        {

            List<TypesLicense> lTypesLicense = new List<TypesLicense>();
            lTypesLicense = db.TypesLicenses.OrderBy(x => x.Descripcion).ToList();
            ViewBag.listaLicencias = lTypesLicense;

            ViewBag.Editar = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Ver = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Baja = PermissionViewModel.TienePermisoBaja(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));


            ViewBag.NroTurno = NroTurno;
            return View("Search");

        }

        [HttpPost]
        public JsonResult GetTurnsSearch()
        {
            try
            {

                List<TurnsSeachViewModel> turnsSeachViews = new List<TurnsSeachViewModel>();                
                List<Turn> turns = db.Turns.Take(1000).Where(x=>x.Enable==true).ToList();

                foreach (var item in turns)
                {
                    string strEsIncompletoNosePresento = EsIncompleto(item.FechaIngreso, item.Id) == true ? "SI" : "NO";
                    TurnsSeachViewModel viewModel = new TurnsSeachViewModel
                    {
                        Apellido = item.Person.Apellido,
                        DNI = item.Person.Dni,
                        FechaTurno = item.FechaTurno.ToString("dd/MM/yyyy HH:mm:ss"),
                        Id = item.Id,
                        Nombre = item.Person.Nombre,
                        Ingreso = item.FechaIngreso.ToString("dd/MM/yyyy HH:mm:ss"),
                        Salida = item.FechaIngreso.ToString("dd/MM/yyyy HH:mm:ss"),
                        Tipo = item.TypesLicense.Descripcion,
                        Turno = item.Turno,
                        IncompletoNoSePresento = strEsIncompletoNosePresento
                    };

                    turnsSeachViews.Add(viewModel);
                }

                return Json(turnsSeachViews.OrderByDescending(x=>x.Id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                throw;
            }
        }


        [HttpPost]
        public JsonResult GetTurnsSearchByTurn(string NroTurno)
        {
            try
            {

                List<TurnsSeachViewModel> turnsSeachViews = new List<TurnsSeachViewModel>();
                List<Turn> turns = db.Turns.Where(x => x.Enable == true && x.Turno== NroTurno).ToList();

                foreach (var item in turns)
                {
                    string strEsIncompletoNosePresento = EsIncompleto(item.FechaIngreso, item.Id) == true ? "SI" : "NO";
                    TurnsSeachViewModel viewModel = new TurnsSeachViewModel
                    {
                        Apellido = item.Person.Apellido,
                        DNI = item.Person.Dni,
                        FechaTurno = item.FechaTurno.ToString("dd/MM/yyyy HH:mm:ss"),
                        Id = item.Id,
                        Nombre = item.Person.Nombre,
                        Ingreso = item.FechaIngreso.ToString("dd/MM/yyyy HH:mm:ss"),
                        Salida = item.FechaIngreso.ToString("dd/MM/yyyy HH:mm:ss"),
                        Tipo = item.TypesLicense.Descripcion,
                        Turno = item.Turno,
                        IncompletoNoSePresento = strEsIncompletoNosePresento

                    };

                    turnsSeachViews.Add(viewModel);
                }

                return Json(turnsSeachViews.OrderByDescending(x => x.Id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                throw;
            }
        }

        //Verifico si el turno es del dia y si esta incompleto o no se presento
        private bool EsIncompleto(DateTime FechaIngreso, int TurnId)
        {
            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59            
            

            if (FechaIngreso>=startDateTime && FechaIngreso <= endDateTime)
            {
                //Me fijo si ese turno tiene un estadio incompleto o no se presento
                if (getTrackingIncompletoNosePresento(TurnId) != null)
                    return true;
                else
                    return false;


            }
            else
            {
                return false;
            }
        }

        private Tracking getTrackingIncompletoNosePresento(int TurnId)
        {
            int[] statusOrden = { (int)StatusOrden.INCOMPLETO, (int)StatusOrden.NO_SE_PRESENTO };
            Tracking tracking = db.Trackings.Where(x => x.TurnID == TurnId && x.Enable == true && statusOrden.Contains(x.Status.Orden)).FirstOrDefault();
            return tracking;
        }


        [HttpPost]
        public JsonResult GetTrackingSearch(int id)
        {
            try
            {

                List<Tracking> trackings = db.Trackings.Where(x => x.TurnID == id && x.Enable == true).ToList();

                List<TrackingViewModel> trackingsViewModel = new List<TrackingViewModel>();


                foreach (var item in trackings)
                {
                    TrackingViewModel viewModel = new TrackingViewModel
                    {
                        Id = item.Id,
                        CantidadDeLlamados = item.CantidadDeLlamados.ToString(),
                        FechaCreacion = item.FechaCreacion.ToString("dd/mm/yyyy"),
                        FechaIngreso = item.FechaIngreso?.ToString("dd/MM/yyyy HH:mm:ss"),
                        FechaSalida = item.FechaSalida?.ToString("dd/MM/yyyy HH:mm:ss"),
                        Sector = item.Sector.Descripcion,
                        Tiempo = string.Format("{0:hh\\:mm\\:ss}", item.Tiempo),
                        Status = item.Status.Descripcion,
                        Terminal = item.Terminal?.Descripcion,
                        Turn = item.Turn.Turno,
                        Usuario = item.Usuario?.Nombreusuario                        

                    };
                    trackingsViewModel.Add(viewModel);
                }

                return Json(trackingsViewModel.ToList().OrderByDescending(x=>x.Id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                throw;
            }
        }


        [HttpPost]
        public JsonResult GetTurns()
        {
            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59
            try
            {
                List<Tracking> trackings = db.Trackings.Where(x => x.Turn.FechaTurno >= startDateTime && x.Enable == true && x.Turn.FechaTurno <= endDateTime).ToList();
                List<TurnIndexViewModel> turnsvm = new List<TurnIndexViewModel>();
                List<Turn> turns = db.Turns.Where(x => x.FechaTurno >= startDateTime && x.FechaTurno <= endDateTime && x.Enable == true).OrderBy(x => new { x.FechaIngreso }).Take(1000).ToList();
                //Obtengo el primer Sector del Workflow para el tipo de tramite
                List<Workflow> workflows = db.Workflows.ToList();

                foreach (var item in turns)
                {
                    Workflow workflow = workflows.Where(x => x.TypesLicenseID == item.TypesLicenseID).FirstOrDefault();
                    int intSectorActual = trackings.Where(x => x.TurnID == item.Id).Select(x => x.SectorID).LastOrDefault();
                    int intOrdenSectorActual = trackings.Where(x => x.TurnID == item.Id).Select(x => x.Orden).LastOrDefault();

                    TurnIndexViewModel turn = new TurnIndexViewModel {
                        Id = item.Id,
                        Turno = item.Turno,
                        Tipo = item.TypesLicense.Descripcion,
                        Apellido = item.Person.Apellido,
                        Nombre = item.Person.Nombre,
                        Ingreso = item.FechaIngreso.ToShortDateString(),
                        Salida = item.FechaSalida?.ToShortDateString(),
                        Estado = trackings.Where(x => x.TurnID == item.Id && x.Enable == true).Select(x => x.Status.Descripcion).LastOrDefault(),
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




        public JsonResult SearchTurn(string NroTurno, string DNI, string Apellido, string Nombre, string FechaTurnoDesde, string FechaTurnoHasta, string Tipo)
        {
            

            List<TurnsSeachViewModel> turns = new List<TurnsSeachViewModel>();
            turns = ArmarConsulta(NroTurno, DNI, Apellido, Nombre, FechaTurnoDesde, FechaTurnoHasta, Tipo);

            ViewBag.Editar = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Ver = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Baja = PermissionViewModel.TienePermisoBaja(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));

            return Json(turns, JsonRequestBehavior.AllowGet);

        }






        private List<TurnsSeachViewModel> ArmarConsulta(string NroTurno, string DNI, string Apellido, string Nombre, string FechaTurnoDesde, string FechaTurnoHasta, string Tipo)
        {
            int TipoId = 0;

            if (!String.IsNullOrEmpty(Tipo))
                TipoId = Convert.ToInt32(Tipo);


            List<TurnsSeachViewModel> turns = new List<TurnsSeachViewModel>();
            DateTime? dtFechaDesde = null;
            DateTime? dtFechaHasta = null;

            if (!String.IsNullOrEmpty(FechaTurnoDesde))
                dtFechaDesde = Convert.ToDateTime(FechaTurnoDesde);

            if (!String.IsNullOrEmpty(FechaTurnoHasta))
                dtFechaHasta = Convert.ToDateTime(FechaTurnoHasta).AddDays(1).AddTicks(-1);


            try
            {


                var lista = db.Turns
                .Where(x => x.Enable == true)
                .Where(x => !string.IsNullOrEmpty(NroTurno) ? (x.Turno == NroTurno && x.Turno != null) : true)
                .Where(x => !string.IsNullOrEmpty(DNI) ? (x.Person.Dni == DNI && x.Person.Dni != null) : true)
                .Where(x => !string.IsNullOrEmpty(Apellido) ? (x.Person.Apellido == Apellido && x.Person.Apellido != null) : true)
                .Where(x => !string.IsNullOrEmpty(Nombre) ? (x.Person.Nombre == Nombre && x.Person.Nombre != null) : true)
                .Where(x => !string.IsNullOrEmpty(Tipo) ? (x.TypesLicense.Id == TipoId && x.TypesLicense.Descripcion != null) : true)
                .Where(x => !string.IsNullOrEmpty(FechaTurnoDesde) ? (x.FechaTurno >= dtFechaDesde && x.FechaTurno != null) : true)
                .Where(x => !string.IsNullOrEmpty(FechaTurnoHasta) ? (x.FechaTurno <= dtFechaHasta && x.FechaTurno != null) : true)
                .Select(c => new { c.Id, c.Turno, c.FechaTurno, c.FechaIngreso, c.FechaSalida, c.TypesLicense, c.Person }).OrderByDescending(x => x.FechaTurno).Take(1000);
                ;

                foreach (var item in lista)
                {
                    string strEsIncompletoNosePresento = EsIncompleto(item.FechaIngreso, item.Id) == true ? "SI" : "NO";
                    TurnsSeachViewModel turn = new TurnsSeachViewModel
                    {
                        Apellido = item.Person.Apellido,
                        DNI = item.Person.Dni,
                        FechaTurno = item.FechaTurno.ToString("dd/MM/yyyy HH:mm:ss"),
                        Id = item.Id,
                        Nombre = item.Person.Nombre,
                        Ingreso = item.FechaIngreso.ToString("dd/MM/yyyy HH:mm:ss"),
                        Salida = item.FechaIngreso.ToString("dd/MM/yyyy HH:mm:ss"),
                        Tipo = item.TypesLicense.Descripcion,
                        Turno = item.Turno,
                        IncompletoNoSePresento = strEsIncompletoNosePresento
                    };

                    turns.Add(turn);

                }

            }
            catch (Exception e)
            {

                throw;
            }

            return turns;
        }

        [HttpPost]
        public JsonResult GetTurnsPendientes()
        
        {
            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59
            List<TurnPendientesViewModel> turnsvm = new List<TurnPendientesViewModel>();
            try
            {
                List<Tracking> trackings = db.Trackings.Where(x => x.FechaCreacion>= startDateTime && x.Enable == true && x.FechaCreacion<= endDateTime && x.Status.Orden == 1 && x.FechaIngreso == null).ToList();


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
                List<CallCenterTurn> centerTurns = db.CallCenterTurns.Where(x => x.FechaTurno >= startDateTime  && x.FechaTurno <= endDateTime).ToList();

                foreach (var item in centerTurns)
                {

                    TurnsProgramadosViewModel turn = new TurnsProgramadosViewModel
                    {
                        Id = item.Id,
                        Apellido = item.Apellido,
                        Nombre = item.Nombre,
                        Dni = item.DNI,
                        Horario = item.FechaTurno.ToString("HH:mm:ss"),
                        Tipo = item.TipoTramite

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
                List<Tracking> trackings = db.Trackings.Where(x => x.Turn.FechaTurno >= startDateTime && x.Turn.FechaTurno <= endDateTime && x.FechaSalida != null && x.Enable == true).ToList();

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
                List<Tracking> trackings = db.Trackings.Where(x => x.Turn.FechaTurno >= startDateTime && x.Turn.FechaTurno <= endDateTime && x.FechaIngreso == null && x.Enable == true).ToList();

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
                List<Tracking> trackings = db.Trackings.Where(x => x.FechaCreacion >= startDateTime && x.FechaCreacion <= endDateTime && x.Enable == true && statusOrdenPendiente.Contains(x.Status.Orden)).ToList();


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
                List<Turn> turns   = db.Turns.Where(x => x.FechaIngreso >= startDateTime && x.FechaIngreso <= endDateTime && x.FechaSalida != null && x.Enable == true).ToList();


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


        [HttpPost]
        public JsonResult GetTurnosProgramadorPorTipoTramite()
        {
            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59

            var pl = from r in db.Turns
                     where r.FechaTurno >= startDateTime && r.FechaTurno<=endDateTime && r.Enable == true
                     group r by r.TypesLicense.Descripcion into grp
                     select new { key = grp.Key, cnt = grp.Count() };
            try
            {               

                return Json(pl, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;

            }
        }


        [HttpPost]
        public JsonResult GetTurnosProgramadorPorEstado()
        {
            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59
            List<TurnChartViewModel> list = new List<TurnChartViewModel>();
            List<Tracking> trackings = db.Trackings.Where(x => x.Turn.FechaTurno >= startDateTime && x.Turn.FechaTurno <= endDateTime && x.Enable == true).ToList();
            List<Turn> turns = db.Turns.Where(x => x.FechaTurno >= startDateTime && x.FechaTurno <= endDateTime && x.Enable == true).ToList();
            List<CallCenterTurn> centerTurns = db.CallCenterTurns.Where(x => x.FechaTurno >= startDateTime && x.FechaTurno <= endDateTime).ToList();
            TurnChartViewModel turn;

            int[] statusOrdenPendiente = { 2, 3 };




            turn = new TurnChartViewModel
                {
                    key = "Sin registrarse",
                    cnt = (from c in centerTurns
                          where !(from t in turns
                                  select t.Person.Dni)
                                 .Contains(c.DNI)
                          select c).Count().ToString()
        };
                list.Add(turn);

                turn = new TurnChartViewModel
                {
                    key = "Pendientes",
                    cnt = trackings.Where(x => x.Status.Orden == 1 && x.FechaIngreso == null && x.Enable == true).Count().ToString()
                };
                list.Add(turn);


            turn = new TurnChartViewModel
            {
                key = "En Proceso",
                cnt = trackings.Where(x => x.Status.Orden == 1 && x.Enable == true && statusOrdenPendiente.Contains(x.Status.Orden)).Count().ToString()
            };
            list.Add(turn);


            turn = new TurnChartViewModel
            {
                key = "Finalizados",
                cnt = turns.Where(x => x.FechaSalida != null && x.Enable == true).Count().ToString()
            };
            list.Add(turn);


            return Json(list, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public JsonResult GetTurnosPromedioPorSector()
        {
            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59
            List<Tracking> trackings = db.Trackings.Where(x => x.Turn.FechaTurno >= startDateTime && x.Turn.FechaTurno <= endDateTime && x.FechaSalida!=null && x.Enable == true).ToList();

            var _groupedItems = trackings.GroupBy(i => i.Sector.Descripcion)
                .Select(g => new {
                    key = g.Key,
                    cnt = (int) g.Select(x => x.Tiempo.Value.TotalMinutes).Average()
//                    cnt = new TimeSpan(Convert.ToInt64(g.Select(x => x.Tiempo.Value.Seconds).Average())).Seconds
                });

            try
            {

                return Json(_groupedItems, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;

            }
        }

        [HttpPost]
        public JsonResult GetTurnosPromedioPorTerminal()
        {
            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59
            List<Tracking> trackings = db.Trackings.Where(x => x.Turn.FechaTurno >= startDateTime && x.Turn.FechaTurno <= endDateTime && x.FechaSalida != null && x.Enable == true).ToList();

            var _groupedItems = trackings.GroupBy(i => i.Terminal.Descripcion)
                .Select(g => new {
                    key = g.Key,
                    cnt = (int)g.Select(x => x.Tiempo.Value.TotalMinutes).Average()
                    //                    cnt = new TimeSpan(Convert.ToInt64(g.Select(x => x.Tiempo.Value.Seconds).Average())).Seconds
                });

            try
            {

                return Json(_groupedItems, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;

            }
        }

        [HttpPost]
        public JsonResult GetTurnosDemoraPorSector()
        {
            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59
            List<Tracking> trackings = db.Trackings.Where(x => x.Turn.FechaTurno >= startDateTime && x.Turn.FechaTurno <= endDateTime && x.FechaIngreso == null && x.Enable == true).ToList();

            var _groupedItems = trackings.GroupBy(i => i.Sector.Descripcion)
                .Select(g => new {
                    key = g.Key,
                    cnt = (int)g.Select(x => (DateTime.Now - x.FechaCreacion).TotalMinutes).Average()
                    //                    cnt = new TimeSpan(Convert.ToInt64(g.Select(x => x.Tiempo.Value.Seconds).Average())).Seconds
                });

            try
            {

                return Json(_groupedItems, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;

            }
        }

        [HttpPost]
        public JsonResult GetTurnosPendientesPorSector()
        {
            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59

            var pl = from r in db.Trackings
                     where r.Turn.FechaTurno >= startDateTime && r.Turn.FechaTurno <= endDateTime && r.Status.Orden == 1 && r.FechaIngreso ==null && r.Enable == true && r.Turn.Enable == true
                     group r by r.Sector.Descripcion into grp
                     select new { key = grp.Key, cnt = grp.Count() };
            try
            {

                return Json(pl, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;

            }
        }


        [HttpPost]
        public JsonResult GetTurnosEnProcesoPorSector()
        {
            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59
            int[] statusOrdenPendiente = { 2, 3 };

            var pl = from r in db.Trackings 
                     where r.Turn.FechaTurno >= startDateTime && r.Turn.FechaTurno <= endDateTime && statusOrdenPendiente.Contains(r.Status.Orden) && r.Enable == true && r.Turn.Enable == true
                     group r by r.Sector.Descripcion into grp
                     select new { key = grp.Key, cnt = grp.Count() };
            try
            {

                return Json(pl, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;

            }
        }


        [HttpPost]
        public JsonResult GetTurnosFinalizadosPorTipoTramite()
        {
            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59
            int[] statusOrdenPendiente = { 2, 3 };

            var pl = from r in db.Turns
                     where r.FechaTurno >= startDateTime && r.FechaTurno <= endDateTime && r.FechaSalida!=null && r.Enable == true
                     group r by r.TypesLicense.Descripcion into grp
                     select new { key = grp.Key, cnt = grp.Count() };
            try
            {

                return Json(pl, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;

            }
        }


        //public ActionResult Search()
        //{
        //    var turns = db.Turns.Include(t => t.Person).Include(t => t.TypesLicense);
        //    return View(turns.ToList());
        //}


        public ActionResult Licenses()
        {
            var turns = db.Turns.Include(t => t.Person).Include(t => t.TypesLicense);
            return View(turns.ToList());
        }



        // GET: Turns/Create
        public ActionResult Create()
        {
            if (!PermissionViewModel.TienePermisoAcesso(WindowHelper.GetWindowId(ModuleDescription, WindowDescription)))
                return View("~/Views/Shared/AccessDenied.cshtml");

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
                turn.Enable = true;
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
            if (!PermissionViewModel.TienePermisoAcesso(WindowHelper.GetWindowId(ModuleDescription, WindowDescription)))
                return View("~/Views/Shared/AccessDenied.cshtml");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Turn turn = db.Turns.Find(id);
            if (turn == null)
            {
                return HttpNotFound();
            }
            ViewBag.TurnId = turn.Id;
            ViewBag.Editar = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Ver = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Baja = PermissionViewModel.TienePermisoBaja(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.listaTiposLicencia = new List<TypesLicense>(db.TypesLicenses.ToList());
            return View(turn);
        }

        // POST: Turns/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TypesLicenseID,FechaTurno ")] Turn turn)
        {
            if (ModelState.IsValid)
            {
                Turn turno = db.Turns.Find(turn.Id);                
                turno.FechaTurno = turn.FechaTurno;

                //Si cambiar de tipo de turno, se Finaliza el turno siguiente y se genera uno nuevo

                if (turno.TypesLicenseID != turn.TypesLicenseID)
                {
                    Status status = db.Status.Where(x => x.Orden == 7).FirstOrDefault();
                    Tracking tracking = db.Trackings.Where(x => x.TurnID == turn.Id && x.FechaSalida == null).FirstOrDefault();
                    tracking.FechaIngreso = DateTime.Now;
                    tracking.FechaSalida = DateTime.Now;
                    tracking.Status = status;
                    db.Entry(tracking).State = EntityState.Modified;

                    //Genera el tramite inicial del nuevo tipo de tramite
                    CreateNextTrancking(turn);
                }

                turno.TypesLicenseID = turn.TypesLicenseID;


                db.Entry(turno).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Search");
            }
            
            ViewBag.PersonID = new SelectList(db.People, "Id", "Nombre", turn.PersonID);
            ViewBag.TypesLicenseID = new SelectList(db.TypesLicenses, "Id", "Descripcion", turn.TypesLicenseID);
            return RedirectToAction("Search");
        }


        private void CreateNextTrancking(Turn turn)
        {
            int EstadoInicial = db.Status.Where(x => x.Orden == 1).Select(x => x.Id).FirstOrDefault();
            //Obtengo el primer Sector del Workflow para el tipo de tramite
            List<Workflow> workflows = db.Workflows.Where(x => x.TypesLicenseID == turn.TypesLicenseID).ToList();

            SectorWorkflow sectorWorkflow = db.SectorWorkflows.Where(x => x.Workflow.TypesLicenseID == turn.TypesLicenseID && x.Orden == 1).FirstOrDefault();


            Tracking tracking = new Tracking
            {
                SectorID = sectorWorkflow.SectorID,
                TurnID = turn.Id,
                FechaCreacion = DateTime.Now,
                StatusID = EstadoInicial
            };

            db.Trackings.Add(tracking);
            


        }


        // GET: Turns/Edit/5
        public ActionResult Details(int id)
        {
            if (!PermissionViewModel.TienePermisoAcesso(WindowHelper.GetWindowId(ModuleDescription, WindowDescription)))
                return View("~/Views/Shared/AccessDenied.cshtml");

            Turn turn = db.Turns.Find(id);
            if (turn == null)
            {
                return HttpNotFound();
            }
            ViewBag.TurnId = turn.Id;
            ViewBag.Editar = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Ver = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Baja = PermissionViewModel.TienePermisoBaja(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.listaTiposLicencia = new List<TypesLicense>(db.TypesLicenses.ToList());
            return View(turn);
        }

        public JsonResult DeleteTurn(int id)
        {
            if (id == 0)
            {
                return Json(new { responseCode = "-10" });
            }

            Turn turn = db.Turns.Find(id);
            turn.Enable = false;
            db.Entry(turn).State = EntityState.Modified;
            db.SaveChanges();

            if (turn.CallCenterTurnId!=null)
            {
                CallCenterTurn callCenterTurn = db.CallCenterTurns.Find(turn.CallCenterTurnId);
                callCenterTurn.Asignado = false;
                db.Entry(callCenterTurn).State = EntityState.Modified;
                db.SaveChanges();
            }
            



            AuditHelper.Auditar("Baja", "Id -" + turn.Id.ToString() + " / Turno -" + turn.Turno, "Turns", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);


        }


        public JsonResult RellamarTurn(int id)
        {
            if (id == 0)
            {
                return Json(new { responseCode = "-10" });
            }

            Tracking tracking = getTrackingIncompletoNosePresento(id);
            tracking.StatusID = (int) StatusOrden.EN_ESPERA;
            tracking.TerminalID = null;
            tracking.UsuarioID = null;
            tracking.FechaIngreso = null;
            tracking.FechaSalida = null;
            tracking.Tiempo = null;
            tracking.CantidadDeLlamados = 0;
            tracking.FechaCreacion = DateTime.Now;

            db.Entry(tracking).State = EntityState.Modified;
            db.SaveChanges();



            AuditHelper.Auditar("Modificacion", "Id -" + id.ToString() + " / Turno -" + tracking.Turn.Turno, "Trackings", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);


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
