using GestionDeTurnos.Models;
using GestionDeTurnos.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace GestionDeTurnos.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ConTurno()
        {
            return View("Ingreso");
        }




        public ActionResult SinTurno()
        {
            return View("Ingreso");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SinTurno([Bind(Include = "DNI")] HomeViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                {
                    //No tiene turno
                    return View("AccesoDenegado");
                }


                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConTurno([Bind(Include = "DNI")] HomeViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                {
                    //No tiene turno
                    return View("AccesoDenegado");


                }


                //Grabo algunos datos hasta tener los datos verdaderos

                //Con el DNI me fijo si existe la persona

                //Person person = db.People.Where(x => x.Dni == model.DNI).FirstOrDefault();
                ////SI existe actualizo los datos con el "WEBSERVICE"
                //Person personWS = new Person();
                //person.Nombre = "Nicolas";
                //person.Apellido = "Blasco";
                //person.Dni = model.DNI;
                //person.FechaNacimiento = Convert.ToDateTime("20/10/1983");
                

                //SI no existe creo la persona en nuestra base de datos

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}