using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GestionDeTurnos.Models;

namespace GestionDeTurnos.Controllers
{
    public class PersonMedicalsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PersonMedicals
        public ActionResult Index()
        {
            var personMedicals = db.PersonMedicals.Include(p => p.Person);
            return View(personMedicals.ToList());
        }

        // GET: PersonMedicals/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PersonMedical personMedical = db.PersonMedicals.Find(id);
            if (personMedical == null)
            {
                return HttpNotFound();
            }
            return View(personMedical);
        }

        // GET: PersonMedicals/Create
        public ActionResult Create()
        {
            ViewBag.PersonId = new SelectList(db.People, "Id", "Nombre");
            return View();
        }

        // POST: PersonMedicals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PersonId,Genero,Avoi")] PersonMedical personMedical)
        {
            if (ModelState.IsValid)
            {
                db.PersonMedicals.Add(personMedical);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PersonId = new SelectList(db.People, "Id", "Nombre", personMedical.PersonId);
            return View(personMedical);
        }

        // GET: PersonMedicals/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PersonMedical personMedical = db.PersonMedicals.Find(id);
            //Prueba
            if (personMedical == null)
            {
                return HttpNotFound();
            }
            ViewBag.PersonId = new SelectList(db.People, "Id", "Nombre", personMedical.PersonId);
            return View(personMedical);
        }

        // POST: PersonMedicals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PersonId,Genero,Avoi")] PersonMedical personMedical)
        {
            if (ModelState.IsValid)
            {
                db.Entry(personMedical).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PersonId = new SelectList(db.People, "Id", "Nombre", personMedical.PersonId);
            return View(personMedical);
        }

        // GET: PersonMedicals/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PersonMedical personMedical = db.PersonMedicals.Find(id);            
            if (personMedical == null)
            {
                return HttpNotFound();
            }
            return View(personMedical);
        }

        // POST: PersonMedicals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PersonMedical personMedical = db.PersonMedicals.Find(id);
            db.PersonMedicals.Remove(personMedical);
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
