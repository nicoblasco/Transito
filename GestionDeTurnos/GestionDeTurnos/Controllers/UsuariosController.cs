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
    public class UsuariosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string ModuleDescription = "Configuración";
        public string WindowDescription = "Usuarios";

        // GET: Usuarios
        public ActionResult Index()
        {
            ViewBag.AltaModificacion = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Baja = PermissionViewModel.TienePermisoBaja(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            List<Usuario> list = db.Usuarios.ToList();
            List<Rol> lRoles = new List<Rol>();
            lRoles = db.Rols.ToList();
            ViewBag.listaRoles = lRoles;

            return View(list);
        }

        public ActionResult Login()
        {

            return View();
        }

        public ActionResult Logout()
        {
            SessionHelper.LogoutSession();

            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public JsonResult GetUsuarios()
        {
            List<Usuario> list = new List<Usuario>();
            try
            {
                list = db.Usuarios.ToList();
                var json = JsonConvert.SerializeObject(list);

                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpPost]
        public JsonResult GetUsuario(int id)
        {
            Usuario usuario = new Usuario();
            try
            {
                usuario = db.Usuarios.Find(id);
                var json = JsonConvert.SerializeObject(usuario);

                return Json(usuario, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpGet]
        public JsonResult GetDuplicates(int id, string usuario)
        {

            try
            {
                var result = from c in db.Usuarios
                             where c.UsuarioId != id
                             && c.Nombreusuario.ToUpper() == usuario.ToUpper()
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


        // GET: Usuarios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // GET: Usuarios/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nombre,Apellido")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                db.Usuarios.Add(usuario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(usuario);
        }


        public JsonResult CreateUsuario(Usuario usuario)
        {
            if (usuario == null)
            {
                return Json(new { responseCode = "-10" });
            }

            usuario.Enable = true;

            db.Usuarios.Add(usuario);
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Alta", usuario.UsuarioId.ToString(), "Usuario", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);
        }

        public JsonResult EditUsuario(Usuario usuario)
        {
            if (usuario == null)
            {
                return Json(new { responseCode = "-10" });
            }

            db.Entry(usuario).State = EntityState.Modified;
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Modificacion", usuario.UsuarioId.ToString(), "Usuario", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);
        }

        // GET: Usuarios/Edit/5
        public ActionResult Edit()
        {
            int id = SessionHelper.GetUser();
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UsuarioId,Contraseña")] Usuario usuario)
        {
            Usuario user = db.Usuarios.Find(SessionHelper.GetUser());
            user.Contraseña = usuario.Contraseña;
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Acts");
            }
            return View(usuario);
        }


        public JsonResult DeleteUsuario(int id)
        {
            if (id == 0)
            {
                return Json(new { responseCode = "-10" });
            }

            Usuario usuario = db.Usuarios.Find(id);
            db.Usuarios.Remove(usuario);
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Baja", id.ToString(), "Usuario", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);


        }

        // GET: Usuarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuario usuario = db.Usuarios.Find(id);
            db.Usuarios.Remove(usuario);
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
