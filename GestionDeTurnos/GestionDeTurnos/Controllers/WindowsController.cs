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
    public class WindowsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string ModuleDescription = "ABM Maestros";
        public string WindowDescription = "Sectores";
        // GET: Windows
        public ActionResult Index()
        {
            if (!PermissionViewModel.TienePermisoAcesso(WindowHelper.GetWindowId(ModuleDescription, WindowDescription)))
                return View("~/Views/Shared/AccessDenied.cshtml");
            return View(db.Windows.ToList());
        }

        // GET: Windows/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Window window = db.Windows.Find(id);
            if (window == null)
            {
                return HttpNotFound();
            }
            return View(window);
        }

        // GET: Windows/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Windows/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Descripcion,Enable,Url,Orden")] Window window)
        {
            if (ModelState.IsValid)
            {
                db.Windows.Add(window);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(window);
        }

        // GET: Windows/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Window window = db.Windows.Find(id);
            if (window == null)
            {
                return HttpNotFound();
            }
            return View(window);
        }

        // POST: Windows/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Descripcion,Enable,Url,Orden")] Window window)
        {
            if (ModelState.IsValid)
            {
                db.Entry(window).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(window);
        }

        // GET: Windows/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Window window = db.Windows.Find(id);
            if (window == null)
            {
                return HttpNotFound();
            }
            return View(window);
        }

        // POST: Windows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Window window = db.Windows.Find(id);
            db.Windows.Remove(window);
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
