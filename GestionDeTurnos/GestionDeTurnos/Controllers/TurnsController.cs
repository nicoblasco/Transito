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

namespace GestionDeTurnos.Controllers
{
    public class TurnsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Turns
        public ActionResult Index()
        {
            string IP = Request.UserHostName;
            string terminalName = CompNameHelper.DetermineCompName(IP);
            
            //Obtengo el sector al que estoy asociado
            Sector sector = db.Sectors.Where(x => x.Descripcion.ToUpper() == terminalName.ToUpper()).FirstOrDefault();
            if (sector != null)
            {
                ViewBag.terminalName = terminalName;
                var turns = db.Turns.Include(t => t.Person).Include(t => t.TypesLicense);
                return View(turns.ToList());
            }
            else
                return View("Create");


        }




        public ActionResult Search()
        {
            var turns = db.Turns.Include(t => t.Person).Include(t => t.TypesLicense);
            return View(turns.ToList());
        }


        public ActionResult Licenses()
        {
            var turns = db.Turns.Include(t => t.Person).Include(t => t.TypesLicense);
            return View(turns.ToList());
        }


        // GET: Turns/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Turn turn = db.Turns.Find(id);
            if (turn == null)
            {
                return HttpNotFound();
            }
            return View(turn);
        }

        // GET: Turns/Create
        public ActionResult Create()
        {
            ViewBag.PersonID = new SelectList(db.People, "Id", "Nombre");
            ViewBag.TypesLicenseID = new SelectList(db.TypesLicenses, "Id", "Descripcion");
            return View();
        }

        // POST: Turns/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Turno,TypesLicenseID,PersonID")] Turn turn)
        {
            if (ModelState.IsValid)
            {
                db.Turns.Add(turn);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PersonID = new SelectList(db.People, "Id", "Nombre", turn.PersonID);
            ViewBag.TypesLicenseID = new SelectList(db.TypesLicenses, "Id", "Descripcion", turn.TypesLicenseID);
            return View(turn);
        }

        // GET: Turns/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Turn turn = db.Turns.Find(id);
            if (turn == null)
            {
                return HttpNotFound();
            }
            ViewBag.PersonID = new SelectList(db.People, "Id", "Nombre", turn.PersonID);
            ViewBag.TypesLicenseID = new SelectList(db.TypesLicenses, "Id", "Descripcion", turn.TypesLicenseID);
            return View(turn);
        }

        // POST: Turns/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Turno,TypesLicenseID,PersonID")] Turn turn)
        {
            if (ModelState.IsValid)
            {
                db.Entry(turn).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PersonID = new SelectList(db.People, "Id", "Nombre", turn.PersonID);
            ViewBag.TypesLicenseID = new SelectList(db.TypesLicenses, "Id", "Descripcion", turn.TypesLicenseID);
            return View(turn);
        }

        // GET: Turns/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Turn turn = db.Turns.Find(id);
            if (turn == null)
            {
                return HttpNotFound();
            }
            return View(turn);
        }

        // POST: Turns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Turn turn = db.Turns.Find(id);
            db.Turns.Remove(turn);
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
