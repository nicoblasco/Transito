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
using GestionDeTurnos.Tags;
using GestionDeTurnos.ViewModel;

namespace GestionDeTurnos.Controllers
{
    [AutenticadoAttribute]
    public class CallCenterTurnsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string ModuleDescription = "Menu Principal";
        public string WindowDescription = "Call Center";
        // GET: CallCenterTurns
        public ActionResult Index()
        {
            //Verifico los Permisos
            if (!PermissionViewModel.TienePermisoAcesso(WindowHelper.GetWindowId(ModuleDescription, WindowDescription)))
                return View("~/Views/Shared/AccessDenied.cshtml");

            List<TypesLicense> lTypesLicense = new List<TypesLicense>();
            lTypesLicense = db.TypesLicenses.OrderBy(x => x.Descripcion).ToList();
            ViewBag.listaLicencias = lTypesLicense;

            ViewBag.Editar = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription,WindowDescription));
            ViewBag.Ver = PermissionViewModel.TienePermisoAcesso(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Baja = PermissionViewModel.TienePermisoBaja(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            return View();
        }


        [HttpPost]
        public JsonResult GetTurnsSearch()
        {

            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59

            try
            {

                List<CallCenterIndexViewModel> turnsSeachViews = new List<CallCenterIndexViewModel>();
                List<CallCenterTurn> turns = db.CallCenterTurns.Where(x=> x.FechaTurno>= startDateTime && x.FechaTurno <= endDateTime ).ToList();

                foreach (var item in turns)
                {
                    CallCenterIndexViewModel viewModel = new CallCenterIndexViewModel
                    {

                        Id = item.Id,
                        DNI = item.DNI,
                        Apellido = item.Apellido,
                        Nombre = item.Nombre,
                        FechaTurno = item.FechaTurno.ToString("dd/MM/yyyy HH:mm:ss"),
                        Asignado = ConvertSiNo(item.Asignado), 
                        TipoTramite = item.TipoTramite,
                        UsuarioId = item.UsuarioId.ToString(),
                        Llamado = item.Llamado
                    };

                    turnsSeachViews.Add(viewModel);
                }

                return Json(turnsSeachViews.OrderBy(x => x.FechaTurno).Take(1000), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                throw;
            }
        }
       
        private string ConvertSiNo (bool value)
        {
            if (value)
                return "SI";
            else
                return "NO";
        }


        public JsonResult SearchTurn(string DNI, string Nombre, string Tipo,string Apellido, string FechaTurnoDesde, string FechaTurnoHasta,string  Asignado, string Multa )
        {


            List<CallCenterIndexViewModel> turns = new List<CallCenterIndexViewModel>();
            turns = ArmarConsulta(Asignado, DNI, Apellido, Nombre, FechaTurnoDesde, FechaTurnoHasta, Tipo, Multa);

            ViewBag.Editar = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId("Call Center", "Editar"));
            ViewBag.Ver = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId("Call Center", "Ver"));
            ViewBag.Baja = PermissionViewModel.TienePermisoBaja(WindowHelper.GetWindowId("Call Center", "Baja"));

            return Json(turns, JsonRequestBehavior.AllowGet);

        }


        private List<CallCenterIndexViewModel> ArmarConsulta(string Asignado, string DNI, string Apellido, string Nombre, string FechaTurnoDesde, string FechaTurnoHasta, string Tipo,string Multa)
        {
            List<CallCenterIndexViewModel> turns = new List<CallCenterIndexViewModel>();
            DateTime? dtFechaDesde = null;
            DateTime? dtFechaHasta = null;
            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59
            bool boolAsignado =false;
            if (Asignado == "SI")
                boolAsignado = true;

            bool boolMulta = false;
            if (Multa == "SI")
                boolMulta = true;

            if (!String.IsNullOrEmpty(FechaTurnoDesde))
                dtFechaDesde = Convert.ToDateTime(FechaTurnoDesde);
            else
                dtFechaDesde = startDateTime;

            if (!String.IsNullOrEmpty(FechaTurnoHasta))
                dtFechaHasta = Convert.ToDateTime(FechaTurnoHasta).AddDays(1).AddTicks(-1);
            else
                dtFechaHasta = endDateTime;


            try
            {


                var lista = db.CallCenterTurns
                .Where(x => !string.IsNullOrEmpty(Asignado) ? (x.Asignado == boolAsignado) : true)
                .Where(x => !string.IsNullOrEmpty(Multa) ? (string.IsNullOrEmpty( x.Llamado) == !boolMulta) : true)
                .Where(x => !string.IsNullOrEmpty(DNI) ? (x.DNI == DNI && x.DNI != null) : true)
                .Where(x => !string.IsNullOrEmpty(Apellido) ? (x.Apellido == Apellido && x.Apellido != null) : true)
                .Where(x => !string.IsNullOrEmpty(Nombre) ? (x.Nombre == Nombre && x.Nombre != null) : true)
                .Where(x => !string.IsNullOrEmpty(Tipo) ? (x.TipoTramite == Tipo && x.TipoTramite != null) : true)
                .Where(x => x.FechaTurno >= dtFechaDesde && x.FechaTurno != null)
                .Where(x => x.FechaTurno <= dtFechaHasta && x.FechaTurno != null)
                .Select(c => new { c.Id, c.Asignado, c.FechaTurno, c.Apellido, c.Nombre, c.TipoTramite, c.DNI,c.UsuarioId, c.Llamado }).OrderBy(x => x.FechaTurno).Take(1000);
                ;

                foreach (var item in lista)
                {
                    CallCenterIndexViewModel turn = new CallCenterIndexViewModel
                    {
                        Apellido = item.Apellido,
                        DNI = item.DNI,
                        FechaTurno = item.FechaTurno.ToString("dd/MM/yyyy HH:mm:ss"),
                        Id = item.Id,
                        Nombre = item.Nombre,
                        TipoTramite = item.TipoTramite,
                        Asignado = ConvertSiNo(item.Asignado),
                        UsuarioId = item.UsuarioId?.ToString(),
                        Llamado = item.Llamado

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
        public JsonResult GetTurno(int id)
        {
            CallCenterTurn callCenterTurn = new CallCenterTurn();
            
            try
            {

                callCenterTurn = db.CallCenterTurns.Find(id);

                CallCenterIndexViewModel callCenterIndexViewModel = new CallCenterIndexViewModel
                {
                    DNI = callCenterTurn.DNI,
                    FechaTurno = callCenterTurn.FechaTurno.ToString("dd/MM/yyyy HH:mm:ss"),
                    Apellido = callCenterTurn.Apellido,
                    Nombre = callCenterTurn.Nombre,
                    Id = callCenterTurn.Id,
                    TipoTramite = callCenterTurn.TipoTramite,
                    UsuarioId = callCenterTurn.UsuarioId.ToString()
                };

                return Json(callCenterIndexViewModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }

        // GET: CallCenterTurns/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CallCenterTurn callCenterTurn = db.CallCenterTurns.Find(id);
            if (callCenterTurn == null)
            {
                return HttpNotFound();
            }
            return View(callCenterTurn);
        }

        public JsonResult CreateTurno(CallCenterTurn  callCenterTurn)
        {
            if (callCenterTurn == null)
            {
                return Json(new { responseCode = "-10" });
            }
            callCenterTurn.Asignado = false;
            callCenterTurn.Estado = "ABIERTO";
            callCenterTurn.Fecha = DateTime.Now;
            callCenterTurn.FechaModificacion = DateTime.Now;
            callCenterTurn.UsuarioId = SessionHelper.GetUser();

            db.CallCenterTurns.Add(callCenterTurn);
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Alta", "Id -" + callCenterTurn.Id.ToString() + " / DNI -" + callCenterTurn.DNI, "callCenterTurn", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);
        }

        public JsonResult EditTurno(CallCenterTurn callCenterTurn)
        {
            CallCenterTurn turn = db.CallCenterTurns.Find(callCenterTurn.Id);
            if (callCenterTurn == null)
            {
                return Json(new { responseCode = "-10" });
            }

            turn.DNI = callCenterTurn.DNI;
            turn.Apellido = callCenterTurn.Apellido;
            turn.Nombre = callCenterTurn.Nombre;
            turn.TipoTramite = callCenterTurn.TipoTramite;
            turn.FechaTurno = callCenterTurn.FechaTurno;
            turn.FechaModificacion = DateTime.Now;
            turn.UsuarioId = SessionHelper.GetUser();

            db.Entry(turn).State = EntityState.Modified;

            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Modificacion", "Id -" + callCenterTurn.Id.ToString() + " / DNI -" + callCenterTurn.DNI, "callCenterTurn", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);
        }

        public JsonResult DeleteTurn(int id)
        {
            if (id == 0)
            {
                return Json(new { responseCode = "-10" });
            }

            
            CallCenterTurn callCenterTurn = db.CallCenterTurns.Find(id);
            db.CallCenterTurns.Remove(callCenterTurn);
            db.SaveChanges();

            AuditHelper.Auditar("Baja", "Id -" + callCenterTurn.Id.ToString() + " / DNI -" + callCenterTurn.DNI, "CallCenterTurn", ModuleDescription, WindowDescription);

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
