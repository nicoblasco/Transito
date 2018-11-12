using GestionDeTurnos.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

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
            Turn turn = db.Turns.Find(id);

            if(turn == null)
            {
                return HttpNotFound();
            }
            ViewBag.nroTurn = turn.Id;
            ViewBag.fechaTurn = turn.FechaIngreso;
            TypesLicense tLicense = db.TypesLicenses.Find(turn.TypesLicenseID);
            ViewBag.TipoTramiteTurn = tLicense.Descripcion;


            Person people = db.People.Find(turn.PersonID);
            ViewBag.PersonID = people.Id;
            ViewBag.Dni = people.Dni;
            ViewBag.Nombre = people.Nombre;
            ViewBag.Apellido = people.Apellido;
            //Calcula Edad
            DateTime nacimiento = people.FechaNacimiento; //Fecha de nacimiento
            int edad = DateTime.Today.AddTicks(-nacimiento.Ticks).Year - 1;
            //Fin Calcula Edad
            ViewBag.Edad = edad;
            ViewBag.Genero = people.Genero;





            List<PersonMedical> listPersonMedicals = db.PersonMedicals.Where(s => s.PersonId == id).ToList();
           // List<PersonMedical> listPersonMedicals = db.PersonMedicals.ToList();
            //Person person =db.personMedicals.Find(id);
            //Array o List de PersonMedical personsMedicals = db.PersonMedicals.Find(PersonId=id)
            //if (personMedical == null)
            //{
             //   return HttpNotFound();
           // }
            //Pasar a la Vista Person - PersonsMedicals
            return View(listPersonMedicals);
            
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
       // [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PersonId,Genero,Avoi,Avod,Fuma,Profesional,ConduceConAnteojos,VisionMonocular,Discromatopsia,HTA,DBT,GAA,AcidoUrico,Colesterol,Observacion")] PersonMedical personMedical)
        {
            //Recover request by Post
            //Add PersonMedicals
            //Redirect to Details/idPerson
            string nro =  Request.Form["nroTurn"];
           // string personId = Request.Form["PersonId"];

            //if (ModelState.IsValid) //Ver ModelStateValid
            //{
                db.PersonMedicals.Add(personMedical);
                db.SaveChanges();
                return RedirectToAction("Details/"+nro);
           // }
            //return HttpNotFound();

          //  ViewBag.PersonId = new SelectList(db.People, "Id", "Nombre", personMedical.PersonId);
          //   return View(personMedical);
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
