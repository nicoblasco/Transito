using GestionDeTurnos.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GestionDeTurnos.Controllers
{
    public class HomeController : Controller
    {
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