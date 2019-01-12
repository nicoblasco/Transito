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
using Newtonsoft.Json;

namespace GestionDeTurnos.Controllers
{
    [AutenticadoAttribute]
    public class TypesLicensesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string ModuleDescription = "ABM Maestros";
        public string WindowDescription = "Tipos de Licencia";

        // GET: TypesLicenses
        public ActionResult Index()
        {
            ViewBag.AltaModificacion = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Baja = PermissionViewModel.TienePermisoBaja(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            return View(db.TypesLicenses.ToList());
        }

        [HttpPost]
        public JsonResult GetTipos()
        {
            List<TypesLicense> list = new List<TypesLicense>();
            try
            {
                list = db.TypesLicenses.ToList();

                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;

            }
        }


        [HttpPost]
        public JsonResult GetTipo(int id)
        {
            TypesLicense types = new TypesLicense();
            try
            {
                types = db.TypesLicenses.Find(id);


                return Json(types, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpGet]
        public JsonResult GetDuplicates(int id, string tipo)
        {

            try
            {
                var result = from c in db.TypesLicenses
                             where c.Id != id
                             && c.Descripcion.ToUpper() == tipo.ToUpper()
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

        public JsonResult CreateTipo(TypesLicense types)
        {
            if (types == null)
            {
                return Json(new { responseCode = "-10" });
            }

            db.TypesLicenses.Add(types);
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Alta", types.Id.ToString(), "TypesLicense", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);
        }


        public JsonResult EditTipo(TypesLicense types)
        {
            if (types == null)
            {
                return Json(new { responseCode = "-10" });
            }

            db.Entry(types).State = EntityState.Modified;
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Modificacion", types.Id.ToString(), "TypesLicense", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);
        }

        // GET: TypesLicenses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypesLicense typesLicense = db.TypesLicenses.Find(id);
            if (typesLicense == null)
            {
                return HttpNotFound();
            }
            return View(typesLicense);
        }

        public JsonResult DeleteTipo(int id)
        {
            if (id == 0)
            {
                return Json(new { responseCode = "-10" });
            }

            TypesLicense types = db.TypesLicenses.Find(id);
            db.TypesLicenses.Remove(types);
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Baja", id.ToString(), "TypesLicense", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);


        }


        // GET: TypesLicenses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TypesLicenses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Descripcion,Codigo,Referencia")] TypesLicense typesLicense)
        {
            if (ModelState.IsValid)
            {
                db.TypesLicenses.Add(typesLicense);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(typesLicense);
        }

        // GET: TypesLicenses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypesLicense typesLicense = db.TypesLicenses.Find(id);
            if (typesLicense == null)
            {
                return HttpNotFound();
            }
            return View(typesLicense);
        }

        // POST: TypesLicenses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Descripcion,Codigo,Referencia")] TypesLicense typesLicense)
        {
            if (ModelState.IsValid)
            {
                db.Entry(typesLicense).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(typesLicense);
        }

        // GET: TypesLicenses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypesLicense typesLicense = db.TypesLicenses.Find(id);
            if (typesLicense == null)
            {
                return HttpNotFound();
            }
            return View(typesLicense);
        }

        // POST: TypesLicenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TypesLicense typesLicense = db.TypesLicenses.Find(id);
            db.TypesLicenses.Remove(typesLicense);
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
