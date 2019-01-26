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
    public class WorkflowsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string ModuleDescription = "ABM Maestros";
        public string WindowDescription = "Sectores";

        // GET: Workflows
        public ActionResult Index()
        {
            ViewBag.TypesLicenseID = new SelectList(db.TypesLicenses, "Id", "Descripcion");
//            ViewBag.SectorID = new SelectList(db.TypesLicenses, "Id", "Descripcion");
            ViewBag.AltaModificacion = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            return View();
        }

        [HttpPost]
        public JsonResult GetSectores(int IdTipo)
        {
            List<Sector> sectors = new List<Sector>();
            List<SectorWorkflow> sectorWorkflows = new List<SectorWorkflow>();
            List<SectorWorkflowViewModel> sectorWorkflowViews = new List<SectorWorkflowViewModel>();
            try
            {
                sectors = db.Sectors.ToList();
                sectorWorkflows = db.SectorWorkflows.Where(x => x.Workflow.TypesLicenseID == IdTipo).ToList();

                foreach (var item in sectors)
                {
                    SectorWorkflowViewModel sectorWorkflowView = new SectorWorkflowViewModel();
                    sectorWorkflowView.Id = item.Id;
                    sectorWorkflowView.Descripcion = item.Descripcion;

                    if (sectorWorkflows.Where(x=> x.SectorID== item.Id).Any())
                    {
                        //Saco de la lista los que ya estan
                        sectorWorkflowView.Selected = true;
                        sectorWorkflowView.Orden = sectorWorkflows.Where(x => x.SectorID == item.Id).FirstOrDefault().Orden;
                    }
                    sectorWorkflowViews.Add(sectorWorkflowView);

                }


                return Json(sectorWorkflowViews.OrderBy(x=>x.Orden), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;

            }
        }

        //public ActionResult Create()

        //{
        //    ViewBag.TypesLicenseID = new SelectList(db.TypesLicenses, "Id", "Descripcion");
        //    //            ViewBag.SectorID = new SelectList(db.TypesLicenses, "Id", "Descripcion");
        //    ViewBag.AltaModificacion = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
        //    return View();
        //}

        //// POST: /Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,Descripcion,Enable,Url,Orden")] Window window)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Windows.Add(window);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(window);
        //}


        public JsonResult CreateWorflow(WorkflowIndexViewModel workflowIndex)
        {
            if (workflowIndex == null)
            {
                return Json(new { responseCode = "-10" });
            }

            
            Workflow workflow = db.Workflows.Where(x => x.TypesLicenseID == workflowIndex.TypesLicenseID).FirstOrDefault();
            
            if (workflow == null)
            {
                //Grabo

                workflow = new Workflow();
                workflow.TypesLicenseID = workflowIndex.TypesLicenseID;
                db.Workflows.Add(workflow);
                db.SaveChanges();


            }
            else
            {
                //Primero borro todos los sectores
                db.SectorWorkflows.RemoveRange(db.SectorWorkflows.Where(x => x.WorkflowID == workflow.Id));

            }



            //Ahora grabo los sectores
            for (int i = 0; i < workflowIndex.Sectores.Length ; i++)
            {
                SectorWorkflow sectorWorkflow = new SectorWorkflow();
                sectorWorkflow.WorkflowID = workflow.Id;
                sectorWorkflow.SectorID = workflowIndex.Sectores[i];
                sectorWorkflow.Orden = i + 1;
                db.SectorWorkflows.Add(sectorWorkflow);
            }
            
            db.SaveChanges();





            //Audito
            AuditHelper.Auditar("Modificacion", workflow.Id.ToString(), "Workflows", ModuleDescription, WindowDescription);

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
