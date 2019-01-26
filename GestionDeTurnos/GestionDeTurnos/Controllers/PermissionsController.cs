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
    public class PermissionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string ModuleDescription = "Configuración";
        public string WindowDescription = "Permisos";

        // GET: Permissions
        public ActionResult Index()
        {
            //Verifico los Permisos
            if (!PermissionViewModel.TienePermisoAcesso(WindowHelper.GetWindowId(ModuleDescription, WindowDescription)))
                return View("~/Views/Shared/AccessDenied.cshtml");

            ViewBag.AltaModificacion = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Baja = PermissionViewModel.TienePermisoBaja(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            List<Permission> list = db.Permissions.ToList();
            List<Rol> lRoles = new List<Rol>();
            lRoles = db.Rols.Where(x => x.IsAdmin!=true).ToList();
            ViewBag.listaRoles = lRoles;
            //return View(db.Permissions.ToList());
            return View(list);
        }

        [HttpPost]
        public JsonResult GetPermisos(int Rolid)
        {
            List<Permission> list = new List<Permission>();
            try
            {
                list = db.Permissions.ToList().Where(x => x.RolId == Rolid).ToList();

                //foreach (var item in list)
                //{
                //    item.Window.Module = db.Modules.Find(item.Window.M);
                //}

                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public JsonResult EditPermiso(int id, string tipo, bool permiso)
        {
            if (tipo == null || id == 0)
            {
                return Json(new { responseCode = "-10" });
            }

            Permission permission = db.Permissions.Find(id);

            switch (tipo)
            {
                case "Consulta":
                    permission.Consulta = permiso;
                    break;
                case "AltaModificacion":
                    permission.AltaModificacion = permiso;
                    break;
                case "Baja":
                    permission.Baja = permiso;
                    break;
                default:
                    break;
            }


            db.Entry(permission).State = EntityState.Modified;
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Modificacion", permission.Id.ToString(), "Permission", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);
        }

        [HttpPost]
        public JsonResult UpdatePermisos(int idRol, string tipo, bool permiso)
        {
            if (tipo == null || idRol == 0)
            {
                return Json(new { responseCode = "-10" });
            }

            List<Permission> lista = db.Permissions.Where(x => x.RolId == idRol).ToList();

            if (lista != null)
            {
                foreach (var item in lista)
                {
                    switch (tipo)
                    {
                        case "Consulta":
                            item.Consulta = permiso;
                            break;
                        case "AltaModificacion":
                            item.AltaModificacion = permiso;
                            break;
                        case "Baja":
                            item.Baja = permiso;
                            break;
                        default:
                            break;
                    }

                    db.Entry(item).State = EntityState.Modified;
                }

                db.SaveChanges();

            }
            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);
        }

        // GET: Permissions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permission permission = db.Permissions.Find(id);
            if (permission == null)
            {
                return HttpNotFound();
            }
            return View(permission);
        }

        // GET: Permissions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Permissions/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Consulta,AltaModificacion,Baja,Fecha")] Permission permission)
        {
            if (ModelState.IsValid)
            {
                db.Permissions.Add(permission);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(permission);
        }

        // GET: Permissions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permission permission = db.Permissions.Find(id);
            if (permission == null)
            {
                return HttpNotFound();
            }
            return View(permission);
        }

        // POST: Permissions/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Consulta,AltaModificacion,Baja,Fecha")] Permission permission)
        {
            if (ModelState.IsValid)
            {
                db.Entry(permission).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(permission);
        }

        // GET: Permissions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permission permission = db.Permissions.Find(id);
            if (permission == null)
            {
                return HttpNotFound();
            }
            return View(permission);
        }

        // POST: Permissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Permission permission = db.Permissions.Find(id);
            db.Permissions.Remove(permission);
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
