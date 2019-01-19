using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GestionDeTurnos.Models;
using GestionDeTurnos.ViewModel;

namespace GestionDeTurnos.Controllers
{
    public class ProcessesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Processes
        public ActionResult Index()
        {
            return View(db.Process.ToList());
        }


        [HttpPost]
        public JsonResult GetProcesos()
        {
            List<ProcessesViewModel> list = new List<ProcessesViewModel>();
            try
            {

                var process = db.Process.OrderByDescending(x => x.Id).Take(2000).ToList();

                foreach (var item in process)
                {
                    ProcessesViewModel processesViewModel = new ProcessesViewModel
                    {
                        Id = item.Id,
                        FechaProcesada = item.Detalle,
                        FechaProcesamientoFin = item.FechaFin?.ToString("dd/MM/yyyy HH:mm:ss"),
                        FechaProcesamientoInicio = item.FechaInicio.ToString("dd/MM/yyyy HH:mm:ss"),
                        Nombre = item.Name
                    };
                    list.Add(processesViewModel);
                }


                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                throw;

            }
        }

        [HttpPost]
        public JsonResult GetProcesoLogError()
        {
            List<ProcessLogsViewModel> list = new List<ProcessLogsViewModel>();
            try
            {

                var process = db.ProcessLogs.OrderByDescending(x => x.Id).Take(2000).ToList();

                foreach (var item in process)
                {
                    ProcessLogsViewModel processesViewModel = new ProcessLogsViewModel
                    {
                        Id = item.Id,
                        ErrorDescription = item.ErrorDescripcion,
                        Fecha = item.Fecha.ToString("dd/MM/yyyy HH:mm:ss"),
                        Nombre = item.Name,
                        ProcesoId = item.ProcessId
                    };
                    list.Add(processesViewModel);
                }


                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
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
