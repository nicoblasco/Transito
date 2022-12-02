using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
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
    public class LicensesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string ModuleDescription = "Licencias";
        public string WindowDescription = "Licencias";

        public ActionResult Index()
        {
            //Verifico los Permisos
            if (!PermissionViewModel.TienePermisoAcesso(WindowHelper.GetWindowId(ModuleDescription, WindowDescription)))
                return View("~/Views/Shared/AccessDenied.cshtml");

            List<Setting> lEstados = new List<Setting>();
            List<Country> lCountries = new List<Country>();
            List<LicenseClass> lClases = new List<LicenseClass>();
            lEstados = db.Settings.Where(x => x.Clave == "ESTADOS_LICENCIAS").ToList();
            lCountries = db.Countries.OrderByDescending(x=>x.Predeterminado).ToList();
            lClases = db.LicenseClasses.Where(x => x.Enable == true).OrderBy(x=>x.Codigo).ToList();
            ViewBag.Estados = lEstados;
            ViewBag.Paises = lCountries;
            ViewBag.Clases = lClases;
            ViewBag.listaCalles = GetCalles();
            ViewBag.listaBarrios = GetBarrios();                                      
            ViewBag.AltaModificacion = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Baja = PermissionViewModel.TienePermisoBaja(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            return View(db.Licenses.ToList());
        }

        [HttpGet]
        public JsonResult GetCalles()
        {
            List<Street> list = new List<Street>();
            try
            {
                list = db.Streets.ToList();
                var json = JsonConvert.SerializeObject(list.Select(item =>
                                  new { data = item.Id.ToString(), value = item.Descripcion }));

                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;

            }
        }

        [HttpPost]
        public JsonResult GetLicencias(string Estado, string Dni, string FechaReciboDesde, string FechaReciboHasta,string FechaEntregaDesde, string FechaEntregaHasta)
        {
            List<LicenseIndexViewModel> list = new List<LicenseIndexViewModel>();
            try
            {

                DateTime? dtFechaReciboDesde = null;
                DateTime? dtFechaReciboHasta = null;
                DateTime? dtFechaEntregaDesde = null;
                DateTime? dtFechaEntregaHasta = null;


                if (!String.IsNullOrEmpty(FechaReciboDesde))
                    dtFechaReciboDesde = Convert.ToDateTime(FechaReciboDesde);

                if (!String.IsNullOrEmpty(FechaReciboHasta))
                    dtFechaReciboHasta = Convert.ToDateTime(FechaReciboHasta).AddDays(1).AddTicks(-1);

                if (!String.IsNullOrEmpty(FechaEntregaDesde))
                    dtFechaEntregaDesde = Convert.ToDateTime(FechaEntregaDesde);

                if (!String.IsNullOrEmpty(FechaEntregaHasta))
                    dtFechaEntregaHasta = Convert.ToDateTime(FechaEntregaHasta).AddDays(1).AddTicks(-1);



                var licenses = db.Licenses
                .Where(x => !string.IsNullOrEmpty(Estado) ? (x.Estado == Estado && x.Estado != null) : true)
                .Where(x => !string.IsNullOrEmpty(Dni) ? (x.Person.Dni == Dni && x.Person.Dni != null) : true)
                .Where(x => !string.IsNullOrEmpty(FechaReciboDesde) ? (x.FechaRecibo >= dtFechaReciboDesde && x.FechaRecibo != null) : true)
                .Where(x => !string.IsNullOrEmpty(FechaReciboHasta) ? (x.FechaRecibo <= dtFechaReciboHasta && x.FechaRecibo != null) : true)
                .Where(x => !string.IsNullOrEmpty(FechaEntregaDesde) ? (x.FechaRetiro >= dtFechaEntregaDesde && x.FechaRetiro != null) : true)
                .Where(x => !string.IsNullOrEmpty(FechaEntregaHasta) ? (x.FechaRetiro <= dtFechaEntregaHasta && x.FechaRetiro != null) : true)
                .Select(c => new { c.Id, c.Person, c.Estado, c.Firma, c.FechaRecibo,c.FechaRetiro }).OrderByDescending(x => x.Id).Take(2000).ToList();

                foreach (var item in licenses)
                {
                    LicenseIndexViewModel license = new LicenseIndexViewModel
                    {
                        Apellido = item.Person.Apellido,
                        Dni = item.Person.Dni,
                        Estado = item.Estado,
                        Nombre = item.Person.Nombre,
                        Id = item.Id,
                        Sign = item.Firma,
                        FechaRecibo = item.FechaRecibo?.ToString("dd/MM/yyyy"),
                        FechaRetiro = item.FechaRetiro?.ToString("dd/MM/yyyy")                   
                    };
                    list.Add(license);
                }
                

                return Json(list.Take(500), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                throw;

            }
        }

        [HttpPost]
        public JsonResult GetLicencia(int id)
        {
            int i = 0;
            License license = new License();
            try
            {
                license = db.Licenses.Find(id);

                LicenseIndexViewModel viewModel = new LicenseIndexViewModel
                {
                    Apellido = license.Person.Apellido,
                    CalleId = license.Person.StreetId,
                    Dni = license.Person.Dni,
                    Domicilio = license.Person.Calle,
                    Estado = license.Estado,
                    FechaDeNacimiento = license.Person.FechaNacimiento?.ToString("dd/MM/yyyy"),
                    Id = license.Id,
                    Nacionalidad = license.Person.CountryId,
                    Nombre = license.Person.Nombre,
                    Otorgamiendo = license.FechaOtorgamiento?.ToString("dd/MM/yyyy"),
                    Vencimiento = license.FechaVencimiento?.ToString("dd/MM/yyyy"),
                    DomicilioNro = license.Person.CalleNro,
                    SelectedClases = new int[license.LicenseClasses.Count],
                    Email = license.Person.Email,
                    Tel_Celular = license.Person.Tel_Celular,
                    Tel_Particular = license.Person.Tel_Particular,
                    Sign = license.Firma,
                    Barrio = license.Person.Barrio,
                    BarrioId = license.Person.NighborhoodId,
                    NroRegistro = license.NroRegistro ?? license.Person.Dni


                };


                if (license.FechaRecibo != null)
                    viewModel.FechaRecibo = "Recibida el: " + license.FechaRecibo?.ToString("dd/MM/yyyy");

                if (license.FechaRetiro != null)
                    viewModel.FechaRetiro = "Retirada el: " + license.FechaRetiro?.ToString("dd/MM/yyyy");

                foreach (var item in license.LicenseClasses)
                {
                    viewModel.SelectedClases[i] = item.Id;
                    i++;
                }

                return Json(viewModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public JsonResult EditLicense(LicenseIndexViewModel licenseIndexViewModel)
        {
            License license = db.Licenses.Find(licenseIndexViewModel.Id);
            if (license == null)
            {
                return Json(new { responseCode = "-10" });
            }

            license.Person.Nombre = licenseIndexViewModel.Nombre;
            license.Person.Apellido = licenseIndexViewModel.Apellido;
            license.Person.Dni = licenseIndexViewModel.Dni;
            license.Person.FechaNacimiento = string.IsNullOrEmpty(licenseIndexViewModel.FechaDeNacimiento) ? (DateTime?)null : DateTime.Parse(licenseIndexViewModel.FechaDeNacimiento);
            license.Person.Calle = licenseIndexViewModel.Domicilio;
            license.Person.StreetId = licenseIndexViewModel.CalleId;
            license.Person.CalleNro = licenseIndexViewModel.DomicilioNro;
            license.Person.Barrio = licenseIndexViewModel.Barrio;
            license.Person.NighborhoodId = licenseIndexViewModel.BarrioId;
            license.FechaOtorgamiento =  string.IsNullOrEmpty(licenseIndexViewModel.Otorgamiendo) ? (DateTime?)null : DateTime.Parse(licenseIndexViewModel.Otorgamiendo);
            license.FechaVencimiento = string.IsNullOrEmpty(licenseIndexViewModel.Vencimiento) ? (DateTime?)null : DateTime.Parse(licenseIndexViewModel.Vencimiento);
            license.Person.Email = licenseIndexViewModel.Email;
            license.Person.Tel_Particular = licenseIndexViewModel.Tel_Particular;
            license.Person.Tel_Celular = licenseIndexViewModel.Tel_Celular;
            license.NroRegistro = licenseIndexViewModel.NroRegistro;

            if (license.LicenseClasses != null)
            {
                List<LicenseClass> LicenseClassAux = new List<LicenseClass>();
                foreach (LicenseClass item in license.LicenseClasses)
                {
                    LicenseClassAux.Add(item);
                }
                foreach (LicenseClass licenseClass in LicenseClassAux)
                {
                    license.LicenseClasses.Remove(licenseClass);
                }
            }

            license.LicenseClasses = new List<LicenseClass>();
            if (licenseIndexViewModel.SelectedClases != null)
            {
                foreach (int id in licenseIndexViewModel.SelectedClases)
                {
                    license.LicenseClasses.Add(db.LicenseClasses.Find(id));
                }
            }


            db.Entry(license).State = EntityState.Modified;

            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Modificacion", "Id -" + license.Id.ToString() + " / DNI -" + license.Person.Dni, "license", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);
        }


        public JsonResult RecibirLicense(LicenseIndexViewModel licenseIndexViewModel)
        {
            License license = db.Licenses.Find(licenseIndexViewModel.Id);
            if (license == null)
            {
                return Json(new { responseCode = "-10" });
            }

            license.Person.Nombre = licenseIndexViewModel.Nombre;
            license.Person.Apellido = licenseIndexViewModel.Apellido;
            license.Person.Dni = licenseIndexViewModel.Dni;
            license.Person.FechaNacimiento = string.IsNullOrEmpty(licenseIndexViewModel.FechaDeNacimiento) ? (DateTime?)null : DateTime.Parse(licenseIndexViewModel.FechaDeNacimiento);
            license.Person.Calle = licenseIndexViewModel.Domicilio;
            license.Person.StreetId = licenseIndexViewModel.CalleId;
            license.Person.CalleNro = licenseIndexViewModel.DomicilioNro;
            license.Person.Barrio = licenseIndexViewModel.Barrio;
            license.Person.NighborhoodId = licenseIndexViewModel.BarrioId;
            license.FechaOtorgamiento = string.IsNullOrEmpty(licenseIndexViewModel.Otorgamiendo) ? (DateTime?)null : DateTime.Parse(licenseIndexViewModel.Otorgamiendo);
            license.FechaVencimiento = string.IsNullOrEmpty(licenseIndexViewModel.Vencimiento) ? (DateTime?)null : DateTime.Parse(licenseIndexViewModel.Vencimiento);
            license.FechaRecibo = DateTime.Now;
            license.FechaRetiro = null;
            license.Estado = db.Settings.Where(x => x.Clave == "ESTADOS_LICENCIAS" && x.Numero1 == 2).Select(x => x.Texto1).FirstOrDefault();
            license.Person.Email = licenseIndexViewModel.Email;
            license.Person.Tel_Particular = licenseIndexViewModel.Tel_Particular;
            license.Person.Tel_Celular = licenseIndexViewModel.Tel_Celular;
            license.NroRegistro = licenseIndexViewModel.NroRegistro;


            if (license.LicenseClasses != null)
            {
                List<LicenseClass> LicenseClassAux = new List<LicenseClass>();
                foreach (LicenseClass item in license.LicenseClasses)
                {
                    LicenseClassAux.Add(item);
                }
                foreach (LicenseClass licenseClass in LicenseClassAux)
                {
                    license.LicenseClasses.Remove(licenseClass);
                }
            }

            license.LicenseClasses = new List<LicenseClass>();
            if (licenseIndexViewModel.SelectedClases != null)
            {
                foreach (int id in licenseIndexViewModel.SelectedClases)
                {
                    license.LicenseClasses.Add(db.LicenseClasses.Find(id));
                }
            }


            db.Entry(license).State = EntityState.Modified;

            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Modificacion", "Id -" + license.Id.ToString() + " / DNI -" + license.Person.Dni, "license", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);
        }

        public JsonResult EntregarLicense(LicenseIndexViewModel licenseIndexViewModel)
        {
            License license = db.Licenses.Find(licenseIndexViewModel.Id);
            if (license == null)
            {
                return Json(new { responseCode = "-10" });
            }

            license.Person.Nombre = licenseIndexViewModel.Nombre;
            license.Person.Apellido = licenseIndexViewModel.Apellido;
            license.Person.Dni = licenseIndexViewModel.Dni;
            license.Person.FechaNacimiento = string.IsNullOrEmpty(licenseIndexViewModel.FechaDeNacimiento) ? (DateTime?)null : DateTime.Parse(licenseIndexViewModel.FechaDeNacimiento);
            license.Person.Calle = licenseIndexViewModel.Domicilio;
            license.Person.StreetId = licenseIndexViewModel.CalleId;
            license.Person.CalleNro = licenseIndexViewModel.DomicilioNro;
            license.FechaOtorgamiento = string.IsNullOrEmpty(licenseIndexViewModel.Otorgamiendo) ? (DateTime?)null : DateTime.Parse(licenseIndexViewModel.Otorgamiendo);
            license.FechaVencimiento = string.IsNullOrEmpty(licenseIndexViewModel.Vencimiento) ? (DateTime?)null : DateTime.Parse(licenseIndexViewModel.Vencimiento);
            license.FechaRetiro = DateTime.Now;
            license.Person.Email = licenseIndexViewModel.Email;
            license.Person.Tel_Particular = licenseIndexViewModel.Tel_Particular;
            license.Person.Tel_Celular = licenseIndexViewModel.Tel_Celular;
            license.Estado = db.Settings.Where(x => x.Clave == "ESTADOS_LICENCIAS" && x.Numero1 == 3).Select(x => x.Texto1).FirstOrDefault();
            license.Firma = licenseIndexViewModel.Sign.Replace("undefined","");
            license.NroRegistro = licenseIndexViewModel.NroRegistro;
            license.Person.Barrio = licenseIndexViewModel.Barrio;
            license.Person.NighborhoodId = licenseIndexViewModel.BarrioId;
            if (license.LicenseClasses != null)
            {
                List<LicenseClass> LicenseClassAux = new List<LicenseClass>();
                foreach (LicenseClass item in license.LicenseClasses)
                {
                    LicenseClassAux.Add(item);
                }
                foreach (LicenseClass licenseClass in LicenseClassAux)
                {
                    license.LicenseClasses.Remove(licenseClass);
                }
            }

            license.LicenseClasses = new List<LicenseClass>();
            if (licenseIndexViewModel.SelectedClases != null)
            {
                foreach (int id in licenseIndexViewModel.SelectedClases)
                {
                    license.LicenseClasses.Add(db.LicenseClasses.Find(id));
                }
            }


            db.Entry(license).State = EntityState.Modified;

            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Modificacion", "Id -" + license.Id.ToString() + " / DNI -" + license.Person.Dni, "license", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);
        }


        public JsonResult CrearLicense(LicenseIndexViewModel licenseIndexViewModel)
        {
            Person person;
            var responseObject = new
            {
                responseCode = -1
            };
            if (licenseIndexViewModel == null)
            {
                return Json(new { responseCode = "-10" });
            }

            try
            {


                License license = new License();

                //Verifico si la persona existe
                if (licenseIndexViewModel.PersonId!=null)
                {
                    person = db.People.Find(licenseIndexViewModel.PersonId);
                }
                else
                {
                     person = new Person();
                }


                person.Nombre = licenseIndexViewModel.Nombre;
                person.Apellido = licenseIndexViewModel.Apellido;
                person.Dni = licenseIndexViewModel.Dni;
                person.FechaNacimiento = string.IsNullOrEmpty(licenseIndexViewModel.FechaDeNacimiento) ? (DateTime?)null : DateTime.Parse(licenseIndexViewModel.FechaDeNacimiento);
                person.Calle = licenseIndexViewModel.Domicilio;
                person.StreetId = licenseIndexViewModel.CalleId;
                person.CalleNro = licenseIndexViewModel.DomicilioNro;
                person.Email = licenseIndexViewModel.Email;
                person.Tel_Particular = licenseIndexViewModel.Tel_Particular;
                person.Tel_Celular = licenseIndexViewModel.Tel_Celular;
                person.Barrio = licenseIndexViewModel.Barrio;
                person.NighborhoodId = licenseIndexViewModel.BarrioId;


                //Actualizo la persona
                if (person.Id>0)
                {                    
                    db.Entry(person).State = EntityState.Modified;
                }
                else
                {
                    db.People.Add(person);
                }
                
                db.SaveChanges();

                //Creo la licencia            

                if (person.Id>0)
                {
                    license.LicenseClasses = new List<LicenseClass>();
                    if (licenseIndexViewModel.SelectedClases != null)
                    {
                        foreach (int id in licenseIndexViewModel.SelectedClases)
                        {
                            license.LicenseClasses.Add(db.LicenseClasses.Find(id));
                        }
                    }

                    license.PersonId = person.Id;
                    license.NroRegistro = license.NroRegistro;
                    license.FechaOtorgamiento = string.IsNullOrEmpty(licenseIndexViewModel.Otorgamiendo) ? (DateTime?)null : DateTime.Parse(licenseIndexViewModel.Otorgamiendo);
                    license.FechaVencimiento = string.IsNullOrEmpty(licenseIndexViewModel.Vencimiento) ? (DateTime?)null : DateTime.Parse(licenseIndexViewModel.Vencimiento);
                    license.Estado = db.Settings.Where(x => x.Clave == "ESTADOS_LICENCIAS" && x.Numero1 == 1).Select(x => x.Texto1).FirstOrDefault();
                    db.Licenses.Add(license);
                    db.SaveChanges();
                }





                if (license.Id>0)

                {
                    //Audito
                    AuditHelper.Auditar("Modificacion", "Id -" + license.Id.ToString() + " / DNI -" + license.Person.Dni, "license", ModuleDescription, WindowDescription);

                      responseObject = new
                    {
                        responseCode = 0
                    };
                }

            }
            catch (Exception e)
            {
                responseObject = new
                {
                    responseCode = -1
                };
                
            }


            return Json(responseObject);
        }


        [HttpPost]
        public JsonResult GetDatosDePersona(string dni)
        {
            Person person = db.People.Where(x => x.Dni == dni).FirstOrDefault();
            try
            {
                if(person!= null)
                {
                    PersonViewModel viewModel = new PersonViewModel
                    {
                        Apellido = person.Apellido,
                        CalleId = person.StreetId,
                        Dni = person.Dni,
                        Domicilio = person.Calle,
                        DomicilioNro = person.CalleNro,
                        BarrioId = person.NighborhoodId,
                        Barrio = person.Barrio,
                        FechaDeNacimiento = person.FechaNacimiento?.ToString("dd/MM/yyyy"),
                        Id = person.Id,
                        Nacionalidad = person.CountryId,
                        Nombre = person.Nombre,
                        Email = person.Email,
                        Tel_Celular = person.Tel_Celular,
                        Tel_Particular = person.Tel_Particular
                        
                    };
                    return Json(viewModel, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(person, JsonRequestBehavior.AllowGet);
                }


                
            }
            catch (Exception e)
            {

                throw;
            }
        }


        [HttpGet]
        public JsonResult GetBarrios()
        {
            List<Nighborhood> list = new List<Nighborhood>();
            try
            {
                //Filtro los habilitados
                list = db.Nighborhoods.ToList();
                var json = JsonConvert.SerializeObject(list.Select(item =>
                                  new { data = item.Codigo.ToString(), value = item.Nombre }));

                return Json(json, JsonRequestBehavior.AllowGet);
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
