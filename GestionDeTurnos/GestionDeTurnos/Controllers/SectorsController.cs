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
    public class SectorsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string ModuleDescription = "ABM Maestros";
        public string WindowDescription = "Sectores";

        // GET: Sectors
        public ActionResult Index()
        {
            ViewBag.AltaModificacion = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Baja = PermissionViewModel.TienePermisoBaja(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            return View(db.Sectors.ToList());
        }


        [HttpPost]
        public JsonResult GetSectores()
        {
            List<Sector> list = new List<Sector>();
            List<SectorViewModel> listvm = new List<SectorViewModel>();
            try
            {
                list = db.Sectors.ToList();

                foreach (var item in list)
                {
                    SectorViewModel sectorViewModel = new SectorViewModel
                    {
                        Descripcion = item.Descripcion,
                        Id = item.Id,
                        Medico = item.Medico==false?"NO":"SI"
                    };
                    listvm.Add(sectorViewModel);
                } 

                return Json(listvm, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;

            }
        }


        [HttpPost]
        public JsonResult GetSector(int id)
        {
            Sector sector = new Sector();
            try
            {
                sector = db.Sectors.Find(id);


                return Json(sector, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpGet]
        public JsonResult GetDuplicates(int id, string descripcion)
        {

            try
            {
                var result = from c in db.Sectors
                             where c.Id != id
                             && c.Descripcion.ToUpper() == descripcion.ToUpper()
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

        public JsonResult CreateSector(Sector sector)
        {
            if (sector == null)
            {
                return Json(new { responseCode = "-10" });
            }

            db.Sectors.Add(sector);
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Alta", sector.Id.ToString(), "Sectors", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);
        }


        public JsonResult EditSector(Sector sector)
        {
            if (sector == null)
            {
                return Json(new { responseCode = "-10" });
            }

            db.Entry(sector).State = EntityState.Modified;
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Modificacion", sector.Id.ToString(), "Sectors", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);
        }

        public JsonResult DeleteSector(int id)
        {
            if (id == 0)
            {
                return Json(new { responseCode = "-10" });
            }

            Sector sector = db.Sectors.Find(id);
            db.Sectors.Remove(sector);
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Baja", id.ToString(), "Sectors", ModuleDescription, WindowDescription);

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
