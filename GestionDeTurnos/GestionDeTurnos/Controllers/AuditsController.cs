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
    public class AuditsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string ModuleDescription = "Configuración";
        public string WindowDescription = "Auditoria";
        // GET: Audits
        public ActionResult Index()
        {
            //Verifico los Permisos
            if (!PermissionViewModel.TienePermisoAcesso(WindowHelper.GetWindowId(ModuleDescription, WindowDescription)))
                return View("~/Views/Shared/AccessDenied.cshtml");

            return View(db.Audits.ToList().Take(2000));
        }


        [HttpPost]
        public JsonResult GetAudits()
        {
            //List<Contravention> list = new List<Contravention>();
            try
            {
                //list = db.Contraventions.ToList().Where(x => x.Enable == true).ToList();
                //var json = JsonConvert.SerializeObject(list);
                //var list = db.Audits.OrderByDescending(c => c.Fecha).Select(c => new { c.Id, c.Fecha, c.User.Nombreusuario,c.Window.Module.Descripcion, c.Window.Descripcion, c.Accion });
                var list = db.Audits.OrderByDescending(x => x.Fecha).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
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
