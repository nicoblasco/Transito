using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GestionDeTurnos.Models;
using Newtonsoft.Json;
using GestionDeTurnos.Helpers;
using GestionDeTurnos.Tags;
using GestionDeTurnos.ViewModel;

namespace GestionDeTurnos.Controllers
{
    [AutenticadoAttribute]
    public class RolsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string ModuleDescription = "Configuración";
        public string WindowDescription = "Roles";

        // GET: Rols
        public ActionResult Index()
        {
            ViewBag.AltaModificacion = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Baja = PermissionViewModel.TienePermisoBaja(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            return View(db.Rols.ToList());
        }


        [HttpPost]
        public JsonResult GetRols()
        {
            List<Rol> list = new List<Rol>();
            try
            {
                list = db.Rols.ToList();
                var json = JsonConvert.SerializeObject(list);

                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;

            }
        }


        [HttpPost]
        public JsonResult GetRol(int id)
        {
            Rol rol = new Rol();
            try
            {
                rol = db.Rols.Find(id);
                var json = JsonConvert.SerializeObject(rol);

                return Json(rol, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpGet]
        public JsonResult GetDuplicates(int id, string nombre)
        {

            try
            {
                var result = from c in db.Rols
                             where c.RolId != id
                             && c.Nombre.ToUpper() == nombre.ToUpper()
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

        //Validar si el nombre se repite

        // GET: Rols/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rol rol = db.Rols.Find(id);
            if (rol == null)
            {
                return HttpNotFound();
            }
            return View(rol);
        }

        // GET: Rols/Create
        public ActionResult Create()
        {
            return View();
        }

        public JsonResult CreateRol(Rol rol)
        {
            if (rol == null)
            {
                return Json(new { responseCode = "-10" });
            }

            db.Rols.Add(rol);

            //Creo los permisos para el rol

            foreach (var item in db.Windows.ToList())
            {
                Permission permission = new Permission();
                permission.Role = rol;
                permission.Window = item;
                permission.Baja = false;
                permission.Consulta = false;
                permission.AltaModificacion = false;
                permission.Fecha = DateTime.Now;
                db.Permissions.Add(permission);
            }

            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Alta", rol.RolId.ToString(), "Rol", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);
        }


        public JsonResult EditRol(Rol rol)
        {
            if (rol == null)
            {
                return Json(new { responseCode = "-10" });
            }

            db.Entry(rol).State = EntityState.Modified;
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Modificacion", rol.RolId.ToString(), "Rol", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);
        }

        // POST: Rols/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nombre,Descripcion")] Rol rol)
        {
            if (ModelState.IsValid)
            {
                db.Rols.Add(rol);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(rol);
        }

        // GET: Rols/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rol rol = db.Rols.Find(id);
            if (rol == null)
            {
                return HttpNotFound();
            }
            return View(rol);
        }

        // POST: Rols/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nombre,Descripcion")] Rol rol)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rol).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rol);
        }

        // GET: Rols/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rol rol = db.Rols.Find(id);
            if (rol == null)
            {
                return HttpNotFound();
            }
            return View(rol);
        }

        public JsonResult DeleteRol(int id)
        {
            if (id == 0)
            {
                return Json(new { responseCode = "-10" });
            }

            Rol rol = db.Rols.Find(id);
            db.Rols.Remove(rol);
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Alta", id.ToString(), "Rol", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);


        }

        // POST: Rols/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Rol rol = db.Rols.Find(id);
            db.Rols.Remove(rol);
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
