using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GestionDeTurnos.Models;
using GestionDeTurnos.ViewModel;

namespace GestionDeTurnos.Controllers
{
    public class SettingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();




        // GET: Settings/Edit
        public ActionResult Edit()
        {
            
            int intTiempoMaximoEspera = db.Settings.Where(x => x.Clave == "TIEMPO_MAXIMO_ESPERA").Select(x => x.Numero1).FirstOrDefault() ?? 0;
            int intCantidadDeLlamados = db.Settings.Where(x => x.Clave == "CANTIDAD_DE_LLAMADOS").Select(x => x.Numero1).FirstOrDefault() ?? 0;
            int intNumeroInicialTurno = db.Settings.Where(x => x.Clave == "NUMERO_INICIAL_TURNO").Select(x => x.Numero1).FirstOrDefault() ?? 0;
            string strVideo = db.Settings.Where(x => x.Clave == "VIDEO").Select(x => x.Texto1).FirstOrDefault();
            SettingsViewModel settingsViewModel = new SettingsViewModel
            {
                CantidadLlamadosPermitidos = intCantidadDeLlamados,
                NumeroInicialTurno = intNumeroInicialTurno,
                TiempoMaximoEspera = intTiempoMaximoEspera,
                VideoBorrado = false,
                Video = strVideo
            };

            return View(settingsViewModel);
        }

        // POST: Settings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TiempoMaximoEspera, NumeroInicialTurno, CantidadLlamadosPermitidos,VideoBorrado")] SettingsViewModel setting, HttpPostedFileBase fileVideo)
        {
            if (ModelState.IsValid)
            {

                Setting settingTiempoMaximoEspera = db.Settings.Where(x => x.Clave == "TIEMPO_MAXIMO_ESPERA").FirstOrDefault();

                if (settingTiempoMaximoEspera != null)
                {
                    settingTiempoMaximoEspera.Numero1 = setting.TiempoMaximoEspera;
                    db.Entry(settingTiempoMaximoEspera).State = EntityState.Modified;
                    db.SaveChanges();
                }

                Setting settingCantidadLlamados = db.Settings.Where(x => x.Clave == "CANTIDAD_DE_LLAMADOS").FirstOrDefault();

                if (settingCantidadLlamados != null)
                {
                    settingCantidadLlamados.Numero1 = setting.CantidadLlamadosPermitidos;
                    db.Entry(settingCantidadLlamados).State = EntityState.Modified;
                    db.SaveChanges();
                }

                Setting settingNumeroInicial = db.Settings.Where(x => x.Clave == "NUMERO_INICIAL_TURNO").FirstOrDefault();

                if (settingNumeroInicial != null)
                {
                    settingNumeroInicial.Numero1 = setting.NumeroInicialTurno;
                    db.Entry(settingNumeroInicial).State = EntityState.Modified;
                    db.SaveChanges();
                }

                Setting settingVideo = db.Settings.Where(x => x.Clave == "VIDEO").FirstOrDefault();
                //Guardo los archivos
                if (fileVideo == null)
                {
                    //puede venir nulo porque no hizo ningun cambio o porque lo borro

                    //Si la borro
                    if (setting.VideoBorrado)
                    {
                        //fisicamente
                        var file = Path.Combine(settingVideo.Texto1);
                        if (System.IO.File.Exists(file))
                            System.IO.File.Delete(file);
                        //logicamente
                        setting.Video = null;

                    }
                    //Si no la borro, lo dejo como esta
                }
                else
                {
                    //Borro la anterior por si la reemplazo
                    if (!String.IsNullOrEmpty(setting.Video))
                    {
                        //fisicamente
                        var file = Path.Combine(setting.Video);
                        if (System.IO.File.Exists(file))
                            System.IO.File.Delete(file);
                    }

                    if (fileVideo.ContentLength > 0)
                    {
                        setting.Video = SaveFile(fileVideo, "video");
                    }
                    else
                    {
                        setting.Video = null;
                    }
                }



               


                    
                    if  (settingVideo!=null)
                    {
                        settingVideo.Texto1 = setting.Video;
                        db.Entry(settingVideo).State = EntityState.Modified;
                        db.SaveChanges();
                    }



                return RedirectToAction("Edit");
            }
            return View(setting);
        }

        private string SaveFile(HttpPostedFileBase fileUpload, string folder)
        {

            var fileName = Path.GetFileName(fileUpload.FileName);
            var path = Path.Combine(Server.MapPath("~/Content/" + folder + "/" ), "video.mp4");
            System.IO.FileInfo file = new System.IO.FileInfo(path);
            file.Directory.Create(); // If the directory already exists, this method does nothing.
            fileUpload.SaveAs(path);
            return path;
        }

        public FileResult Download(string filePath)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(@filePath);
            string fileName = Path.GetFileName(filePath);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
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
