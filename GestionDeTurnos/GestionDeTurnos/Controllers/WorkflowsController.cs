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

namespace GestionDeTurnos.Controllers
{
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


        public JsonResult CreateWorflow(WorkflowIndexViewModel workflowIndex)
        {
            if (workflowIndex == null)
            {
                return Json(new { responseCode = "-10" });
            }



           
            SectorWorkflow sectorWorkflow = new SectorWorkflow();
            sectorWorkflow.SectorID = workflowIndex.SectorID;
            sectorWorkflow.Orden = workflowIndex.Orden;
            Workflow workflow = db.Workflows.Where(x => x.TypesLicenseID == workflowIndex.TypesLicenseID).FirstOrDefault();
            
            if (workflow == null)
            {
                //Grabo

                workflow = new Workflow();
                workflow.TypesLicenseID = workflowIndex.TypesLicenseID;
                db.Workflows.Add(workflow);
                db.SaveChanges();

                if (workflow.Id>0)
                {
                    sectorWorkflow.WorkflowID = workflow.Id;
                }

            }
            else
            {
                sectorWorkflow.SectorID = workflowIndex.SectorID;
                sectorWorkflow.Orden = workflowIndex.Orden;
                sectorWorkflow.WorkflowID = workflow.Id;

            }

            db.SectorWorkflows.Add(sectorWorkflow);
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
