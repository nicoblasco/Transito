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
    public class LicensesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string ModuleDescription = "Licencias";
        public string WindowDescription = "Licencias";

        public ActionResult Index()
        {
            ViewBag.AltaModificacion = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Baja = PermissionViewModel.TienePermisoBaja(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            return View(db.Licenses.ToList());
        }

        [HttpPost]
        public JsonResult GetLicencias()
        {
            List<LicenseIndexViewModel> list = new List<LicenseIndexViewModel>();
            try
            {
                List<License> licenses = db.Licenses.ToList();

                foreach (var item in licenses)
                {
                    LicenseIndexViewModel license = new LicenseIndexViewModel
                    {
                        Apellido = item.Person.Apellido,
                        Dni = item.Person.Dni,
                        Estado = item.Estado,
                        Nombre = item.Person.Nombre,
                        Id = item.Id
                    };
                    list.Add(license);
                }
                

                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;

            }
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
