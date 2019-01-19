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
    public class LicenseClassesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string ModuleDescription = "ABM Maestros";
        public string WindowDescription = "Clases de Licencia";

        // GET: TypesLicenses
        public ActionResult Index()
        {
            ViewBag.AltaModificacion = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Baja = PermissionViewModel.TienePermisoBaja(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            return View(db.LicenseClasses.ToList());
        }

        [HttpPost]
        public JsonResult GetClases()
        {
            //var list = new List<LicenseClass>();
            try
            {
                var list = db.LicenseClasses.Where(x=>x.Enable==true).Select(c => new { c.Id, c.Codigo, c.Descripcion }).ToList();

                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;

            }
        }


        [HttpPost]
        public JsonResult GetClase(int id)
        {
            LicenseClass types = new LicenseClass();
            try
            {
                types = db.LicenseClasses.Find(id);


                return Json(types, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpGet]
        public JsonResult GetDuplicates(int id, string codigo)
        {

            try
            {
                var result = from c in db.LicenseClasses
                             where c.Id != id
                             && c.Codigo.ToUpper() == codigo.ToUpper() && c.Enable==true
                             select c;

                var responseObject = new
                {
                    responseCode = result.Count()
                };

                return Json(responseObject, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public JsonResult CreateClase(LicenseClass clase)
        {
            if (clase == null)
            {
                return Json(new { responseCode = "-10" });
            }

            clase.Enable = true;
            db.LicenseClasses.Add(clase);
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Alta", clase.Id.ToString(), "LicenseClass", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);
        }


        public JsonResult EditClase(LicenseClass clase )
        {
            if (clase == null)
            {
                return Json(new { responseCode = "-10" });
            }
            clase.Enable = true;
            db.Entry(clase).State = EntityState.Modified;
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Modificacion", clase.Id.ToString(), "LicenseClass", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);
        
        }

        public JsonResult DeleteClase(int id)
        {
            if (id == 0)
            {
                return Json(new { responseCode = "-10" });
            }

            LicenseClass licenseClass= db.LicenseClasses.Find(id);
            licenseClass.Enable = false;
            db.Entry(licenseClass).State = EntityState.Modified;
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Baja", id.ToString(), "LicenseClass", ModuleDescription, WindowDescription);

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
