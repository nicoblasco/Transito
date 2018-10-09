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
using GestionDeTurnos.ViewModel;
using Newtonsoft.Json;

namespace GestionDeTurnos.Controllers
{
    public class TerminalsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string ModuleDescription = "ABM Maestros";
        public string WindowDescription = "Terminales";

        // GET: Terminals
        public ActionResult Index()
        {
            ViewBag.AltaModificacion = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Baja = PermissionViewModel.TienePermisoBaja(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            string IP = Request.UserHostName;
            string terminalName = CompNameHelper.DetermineCompName(IP);
            List<Terminal> list = db.Terminals.ToList();
            List<Sector> lSectores = new List<Sector>();
            lSectores = db.Sectors.ToList();
            ViewBag.listaSectores = lSectores;
            ViewBag.terminalName = terminalName;

            return View(list);
        }

        [HttpPost]
        public JsonResult GetTerminales()
        {
            List<Terminal> list = new List<Terminal>();
            try
            {
                list = db.Terminals.ToList();

                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpPost]
        public JsonResult GetTerminal(int id)
        {
            Terminal terminal = new Terminal();
            try
            {
                terminal = db.Terminals.Find(id);

                return Json(terminal, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpGet]
        public JsonResult GetDuplicates(int id, string IP)
        {

            try
            {
                var result = from c in db.Terminals
                             where c.Id != id
                             && c.IP.ToUpper() == IP.ToUpper()
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

        public JsonResult CreateTerminal(Terminal terminal)
        {
            if (terminal == null)
            {
                return Json(new { responseCode = "-10" });
            }

            terminal.Enable = true;

            db.Terminals.Add(terminal);
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Alta", terminal.Id.ToString(), "Terminal", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);
        }

        public JsonResult EditTerminal(Terminal terminal)
        {
            if (terminal == null)
            {
                return Json(new { responseCode = "-10" });
            }

            db.Entry(terminal).State = EntityState.Modified;
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Modificacion", terminal.Id.ToString(), "Terminal", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);
        }


        public JsonResult DeleteTerminal(int id)
        {
            if (id == 0)
            {
                return Json(new { responseCode = "-10" });
            }

            Terminal terminal = db.Terminals.Find(id);

            db.Terminals.Remove(terminal);
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Baja", id.ToString(), "Terminal", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);


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
