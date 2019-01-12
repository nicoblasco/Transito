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
    public class CallCenterTurnsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string ModuleDescription = "Menu Principal";
        public string WindowDescription = "Call Center";
        // GET: CallCenterTurns
        public ActionResult Index()
        {
            List<TypesLicense> lTypesLicense = new List<TypesLicense>();
            lTypesLicense = db.TypesLicenses.OrderBy(x => x.Descripcion).ToList();
            ViewBag.listaLicencias = lTypesLicense;

            ViewBag.Editar = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId("Call Center", "Editar"));
            ViewBag.Ver = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId("Call Center", "Ver"));
            ViewBag.Baja = PermissionViewModel.TienePermisoBaja(WindowHelper.GetWindowId("Call Center", "Baja"));
            return View();
        }


        [HttpPost]
        public JsonResult GetTurnsSearch()
        {
            try
            {

                List<CallCenterIndexViewModel> turnsSeachViews = new List<CallCenterIndexViewModel>();
                List<CallCenterTurn> turns = db.CallCenterTurns.Take(1000).ToList();

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
                        TipoTramite = item.TipoTramite
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
       
        private string ConvertSiNo (bool value)
        {
            if (value)
                return "SI";
            else
                return "NO";
        }


        public JsonResult SearchTurn(string DNI, string Nombre, string Tipo,string Apellido, string FechaTurnoDesde, string FechaTurnoHasta,string  Asignado )
        {


            List<CallCenterIndexViewModel> turns = new List<CallCenterIndexViewModel>();
            turns = ArmarConsulta(Asignado, DNI, Apellido, Nombre, FechaTurnoDesde, FechaTurnoHasta, Tipo);

            ViewBag.Editar = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId("Call Center", "Editar"));
            ViewBag.Ver = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId("Call Center", "Ver"));
            ViewBag.Baja = PermissionViewModel.TienePermisoBaja(WindowHelper.GetWindowId("Call Center", "Baja"));

            return Json(turns, JsonRequestBehavior.AllowGet);

        }


        private List<CallCenterIndexViewModel> ArmarConsulta(string Asignado, string DNI, string Apellido, string Nombre, string FechaTurnoDesde, string FechaTurnoHasta, string Tipo)
        {


            List<CallCenterIndexViewModel> turns = new List<CallCenterIndexViewModel>();
            DateTime? dtFechaDesde = null;
            DateTime? dtFechaHasta = null;
            bool boolAsignado =false;
            if (Asignado == "SI")
                boolAsignado = true;

            if (!String.IsNullOrEmpty(FechaTurnoDesde))
                dtFechaDesde = Convert.ToDateTime(FechaTurnoDesde);

            if (!String.IsNullOrEmpty(FechaTurnoHasta))
                dtFechaHasta = Convert.ToDateTime(FechaTurnoHasta).AddDays(1).AddTicks(-1);


            try
            {


                var lista = db.CallCenterTurns
                .Where(x => !string.IsNullOrEmpty(Asignado) ? (x.Asignado == boolAsignado) : true)
                .Where(x => !string.IsNullOrEmpty(DNI) ? (x.DNI == DNI && x.DNI != null) : true)
                .Where(x => !string.IsNullOrEmpty(Apellido) ? (x.Apellido == Apellido && x.Apellido != null) : true)
                .Where(x => !string.IsNullOrEmpty(Nombre) ? (x.Nombre == Nombre && x.Nombre != null) : true)
                .Where(x => !string.IsNullOrEmpty(Tipo) ? (x.TipoTramite == Tipo && x.TipoTramite != null) : true)
                .Where(x => !string.IsNullOrEmpty(FechaTurnoDesde) ? (x.FechaTurno >= dtFechaDesde && x.FechaTurno != null) : true)
                .Where(x => !string.IsNullOrEmpty(FechaTurnoHasta) ? (x.FechaTurno <= dtFechaHasta && x.FechaTurno != null) : true)
                .Select(c => new { c.Id, c.Asignado, c.FechaTurno, c.Apellido, c.Nombre, c.TipoTramite, c.DNI }).OrderByDescending(x => x.FechaTurno).Take(1000);
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
                        Asignado = ConvertSiNo(item.Asignado)
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
                    TipoTramite = callCenterTurn.TipoTramite
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
