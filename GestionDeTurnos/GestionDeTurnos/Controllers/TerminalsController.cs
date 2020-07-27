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
using Newtonsoft.Json;

namespace GestionDeTurnos.Controllers
{
    [AutenticadoAttribute]
    public class TerminalsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string ModuleDescription = "ABM Maestros";
        public string WindowDescription = "Terminales";

        // GET: Terminals
        public ActionResult Index()
        {
            if (!PermissionViewModel.TienePermisoAcesso(WindowHelper.GetWindowId(ModuleDescription, WindowDescription)))
                return View("~/Views/Shared/AccessDenied.cshtml");
            ViewBag.AltaModificacion = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Baja = PermissionViewModel.TienePermisoBaja(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            try
            {
                //string IP = Request.UserHostName;
                //string terminalName = CompNameHelper.DetermineCompName(IP);
                //string terminalName;// = Request.UserHostAddress;  //Request.UserHostName;

                //string clientIP;
                //string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                //if (!string.IsNullOrEmpty(ip))
                //{
                //    string[] ipRange = ip.Trim().Split(',');
                //    int le = ipRange.Length - 1;
                //    clientIP = ipRange[le];
                //}
                //else
                //    clientIP = Request.ServerVariables["REMOTE_ADDR"];
                int userId = SessionHelper.GetUser();
                string strUserName = db.Usuarios.Where(x => x.UsuarioId == userId).Select(x => x.Nombreusuario).FirstOrDefault();

                //terminalName = clientIP;

                List<Terminal> list = db.Terminals.ToList();
                List<Sector> lSectores = new List<Sector>();
                lSectores = db.Sectors.ToList();
                ViewBag.listaSectores = lSectores;
                ViewBag.terminalName = strUserName;

                return View(list);
            }
            catch (Exception e)
            {

                string terminalName = "Terminal - ERR";
                List<Terminal> list = db.Terminals.ToList();
                List<Sector> lSectores = new List<Sector>();
                lSectores = db.Sectors.ToList();
                ViewBag.listaSectores = lSectores;
                ViewBag.terminalName = terminalName;
                ViewBag.exception = e.Message;
                return View(list);
            }

            //string terminalName = HttpContext.Current.Server.MachineName;


        }

        [HttpPost]
        public JsonResult GetTerminales()
        {
            List<Terminal> list = new List<Terminal>();
            try
            {
                list = db.Terminals.Where(x=>x.Enable==true).ToList();

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
                             && c.IP.ToUpper() == IP.ToUpper() && c.Enable==true
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

            terminal.UsuarioId = SessionHelper.GetUser();
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

            terminal.Enable = true;
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
            terminal.Enable = false;
            //db.Terminals.Remove(terminal);
            db.Entry(terminal).State = EntityState.Modified;
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
