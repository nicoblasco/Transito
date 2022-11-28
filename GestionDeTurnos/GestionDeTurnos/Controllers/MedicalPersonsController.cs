using GestionDeTurnos.Helpers;
using GestionDeTurnos.Models;
using GestionDeTurnos.Tags;
using GestionDeTurnos.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace GestionDeTurnos.Controllers
{
    [AutenticadoAttribute]
    public class MedicalPersonsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string ModuleDescription = "Menu Principal";
        public string WindowDescription = "Historial Medico";
        // GET: PersonMedicals
        public ActionResult Index()
        {
            //Verifico los Permisos
            if (!PermissionViewModel.TienePermisoAcesso(WindowHelper.GetWindowId(ModuleDescription, WindowDescription)))
                return View("~/Views/Shared/AccessDenied.cshtml");

            List<TypesLicense> lTypesLicense = new List<TypesLicense>();
            lTypesLicense = db.TypesLicenses.OrderBy(x => x.Descripcion).ToList();
            ViewBag.listaLicencias = lTypesLicense;

            ViewBag.Editar = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId("Call Center", "Editar"));
            ViewBag.Ver = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId("Call Center", "Ver"));
            ViewBag.Baja = PermissionViewModel.TienePermisoBaja(WindowHelper.GetWindowId("Call Center", "Baja"));

            return View();
        }


        public ActionResult PersonByTurn(int? id)
        {
            //Verifico los Permisos
            //if (!PermissionViewModel.TienePermisoAcesso(WindowHelper.GetWindowId(ModuleDescription, WindowDescription)))
            //    return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Turn turn = db.Turns.Find(id);
            Person people = db.People.Find(turn.PersonID);
            TypesLicense tLicense = db.TypesLicenses.Find(turn.TypesLicenseID);

            int? edad = null;
            if (turn == null)
            {
                return HttpNotFound();
            }

            //Calculo la edad
            if (people.FechaNacimiento != null)
            {
                DateTime nacimiento = people.FechaNacimiento.Value; //Fecha de nacimiento
                edad = DateTime.Today.AddTicks(-nacimiento.Ticks).Year - 1;
            }

            MedicalPersonViewModel person = new MedicalPersonViewModel
            {
                Apellido = people.Apellido,
                DNI = people.Dni,
                Edad = edad?.ToString(),
                PersonId = people.Id,
                Nombre = people.Nombre,
                NumeroTurno = turn.Turno,
                TipoTramite = tLicense.Descripcion,
                FechaIngreso = turn.FechaIngreso.ToString("dd/MM/yyyy"),
                FechaNacimiento = people.FechaNacimiento?.ToString("dd/MM/yyyy"),
                TurnId = turn.Id
            };
            person.Edad = edad.ToString();

            return View(person);
        }


        public JsonResult GetHistoriaClinica(int PersonId)
        {
            try
            {

                List<MedicalPersonGridViewModel> listvw = new List<MedicalPersonGridViewModel>();
                List<MedicalPerson> list = db.MedicalPersons.Where(x=>x.PersonId==PersonId).Take(1000).ToList();

                foreach (var item in list)
                {
                    MedicalPersonGridViewModel viewModel = new MedicalPersonGridViewModel
                    {

                        Id = item.Id,
                        AcidoUrico = item.AcidoUrico==false?"NO":"SI",
                        Apellido = item.Person.Apellido,
                        Avod = item.Avod.ToString(),
                        Avoi = item.Avoi.ToString(),
                        PersonId = item.PersonId,
                        Colesterol = item.Colesterol == false ? "NO" : "SI",
                        ConduceConAnteojos = item.ConduceConAnteojos == false ? "NO" : "SI",
                        DBT = item.DBT == false ? "NO" : "SI",
                        Discromatopsia= item.Discromatopsia == false ? "NO" : "SI",
                        DNI = item.Person.Dni,
                        FechaIngreso = item.Turn.FechaIngreso.ToString("dd/MM/yyyy"),
                        FechaNacimiento = item.Person.FechaNacimiento?.ToString("dd/MM/yyyy"),
                        Edad = null,
                        Fuma = item.Fuma == false ? "NO" : "SI",
                        GAA = item.GAA == false ? "NO" : "SI",
                        HTA = item.HTA == false ? "NO" : "SI",
                        Nombre = item.Person.Nombre,
                        NumeroTurno = item.Turn.Turno,
                        Observacion = item.Observacion,
                        Profesional = item.Profesional == false ? "NO" : "SI",
                        TipoTramite = item.Turn.Turno,
                        TurnId = item.TurnId,
                        VisionMonocular =  item.VisionMonocular == false ? "NO" : "SI"

                    };

                    listvw.Add(viewModel);
                }

                return Json(listvw.OrderByDescending(x => x.Id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                throw;
            }
        }


        [HttpPost]
        public JsonResult Get(int id)
        {


            try
            {

                MedicalPerson item = db.MedicalPersons.Find(id);

                MedicalPersonViewModel person = new MedicalPersonViewModel
                {
                    Id = item.Id,
                    AcidoUrico = item.AcidoUrico,
                    Apellido = item.Person.Apellido,
                    Avod = item.Avod,
                    Avoi = item.Avoi,
                    PersonId = item.PersonId,
                    Colesterol = item.Colesterol,
                    ConduceConAnteojos = item.ConduceConAnteojos,
                    DBT = item.DBT,
                    Discromatopsia = item.Discromatopsia,
                    DNI = item.Person.Dni,
                    FechaIngreso = item.Turn.FechaIngreso.ToString("dd/MM/yyyy"),
                    FechaNacimiento = item.Person.FechaNacimiento?.ToString("dd/MM/yyyy"),
                    Fuma = item.Fuma,
                    GAA = item.GAA,
                    HTA = item.HTA,
                    Nombre = item.Person.Nombre,
                    NumeroTurno = item.Turn.Turno,
                    Observacion = item.Observacion,
                    Profesional = item.Profesional,
                    TipoTramite = item.Turn.Turno,
                    TurnId = item.TurnId,
                    VisionMonocular = item.VisionMonocular                   
                };

                return Json(person, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public JsonResult GetAll()
        {


            try
            {

                List<MedicalPerson> list = db.MedicalPersons.ToList();
                List<MedicalPersonGridViewModel> listvm = new List<MedicalPersonGridViewModel>();

                foreach (var item in list)
                {
                    MedicalPersonGridViewModel person = new MedicalPersonGridViewModel
                    {
                        Id = item.Id,
                        AcidoUrico = item.AcidoUrico == false ? "NO" : "SI",
                        Apellido = item.Person.Apellido,
                        Avod = item.Avod.ToString(),
                        Avoi = item.Avoi.ToString(),
                        PersonId = item.PersonId,
                        Colesterol = item.Colesterol == false ? "NO" : "SI",
                        ConduceConAnteojos = item.ConduceConAnteojos == false ? "NO" : "SI",
                        DBT = item.DBT == false ? "NO" : "SI",
                        Discromatopsia = item.Discromatopsia == false ? "NO" : "SI",
                        DNI = item.Person.Dni,
                        FechaIngreso = item.Turn.FechaIngreso.ToString("dd/MM/yyyy"),
                        FechaNacimiento = item.Person.FechaNacimiento?.ToString("dd/MM/yyyy"),
                        Fuma = item.Fuma == false ? "NO" : "SI",
                        GAA = item.GAA == false ? "NO" : "SI",
                        HTA = item.HTA == false ? "NO" : "SI",
                        Nombre = item.Person.Nombre,
                        NumeroTurno = item.Turn.Turno,
                        Observacion = item.Observacion,
                        Profesional = item.Profesional == false ? "NO" : "SI",
                        TipoTramite = item.Turn.TypesLicense.Descripcion,
                        TurnId = item.TurnId,
                        VisionMonocular = item.VisionMonocular == false ? "NO" : "SI"
                    };

                    listvm.Add(person);

                }


                return Json(listvm, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public JsonResult Search(string DNI, string Nombre, string Tipo, string Apellido, string FechaTurnoDesde, string FechaTurnoHasta)
        {


            List<MedicalPersonGridViewModel> turns = new List<MedicalPersonGridViewModel>();
            turns = ArmarConsulta(DNI, Apellido, Nombre, FechaTurnoDesde, FechaTurnoHasta, Tipo);


            return Json(turns, JsonRequestBehavior.AllowGet);

        }


        private List<MedicalPersonGridViewModel> ArmarConsulta(string DNI, string Apellido, string Nombre, string FechaTurnoDesde, string FechaTurnoHasta, string Tipo)
        {


            List<MedicalPersonGridViewModel> turns = new List<MedicalPersonGridViewModel>();
            DateTime? dtFechaDesde = null;
            DateTime? dtFechaHasta = null;


            if (!String.IsNullOrEmpty(FechaTurnoDesde))
                dtFechaDesde = Convert.ToDateTime(FechaTurnoDesde);

            if (!String.IsNullOrEmpty(FechaTurnoHasta))
                dtFechaHasta = Convert.ToDateTime(FechaTurnoHasta).AddDays(1).AddTicks(-1);


            try
            {


                var lista = db.MedicalPersons
                .Where(x => !string.IsNullOrEmpty(DNI) ? (x.Person.Dni == DNI && x.Person.Dni != null) : true)
                .Where(x => !string.IsNullOrEmpty(Apellido) ? (x.Person.Apellido == Apellido && x.Person.Apellido != null) : true)
                .Where(x => !string.IsNullOrEmpty(Nombre) ? (x.Person.Nombre == Nombre && x.Person.Nombre != null) : true)
                .Where(x => !string.IsNullOrEmpty(Tipo) ? (x.Turn.TypesLicense.Descripcion == Tipo && x.Turn.TypesLicense.Descripcion != null) : true)
                .Where(x => !string.IsNullOrEmpty(FechaTurnoDesde) ? (x.Turn.FechaTurno >= dtFechaDesde && x.Turn.FechaTurno != null) : true)
                .Where(x => !string.IsNullOrEmpty(FechaTurnoHasta) ? (x.Turn.FechaTurno <= dtFechaHasta && x.Turn.FechaTurno != null) : true)
                .Select(c => new { c.Id, c.AcidoUrico,c.Avod, c.Avoi, c.Profesional, c.Colesterol, c.ConduceConAnteojos, c.DBT, c.Discromatopsia, c.Fuma,c.GAA,c.HTA,c.Observacion,c.Person,c.PersonId,c.Turn,c.TurnId,c.VisionMonocular }).OrderByDescending(x => x.Turn.FechaTurno).Take(1000);
                ;

                foreach (var item in lista)
                {
                    MedicalPersonGridViewModel vm = new MedicalPersonGridViewModel
                    {
                        Id = item.Id,
                        AcidoUrico = item.AcidoUrico == false ? "NO" : "SI",
                        Apellido = item.Person.Apellido,
                        Avod = item.Avod.ToString(),
                        Avoi = item.Avoi.ToString(),
                        PersonId = item.PersonId,
                        Colesterol = item.Colesterol == false ? "NO" : "SI",
                        ConduceConAnteojos = item.ConduceConAnteojos == false ? "NO" : "SI",
                        DBT = item.DBT == false ? "NO" : "SI",
                        Discromatopsia = item.Discromatopsia == false ? "NO" : "SI",
                        DNI = item.Person.Dni,
                        FechaIngreso = item.Turn.FechaIngreso.ToString("dd/MM/yyyy"),
                        FechaNacimiento = item.Person.FechaNacimiento?.ToString("dd/MM/yyyy"),
                        Edad = null,
                        Fuma = item.Fuma == false ? "NO" : "SI",
                        GAA = item.GAA == false ? "NO" : "SI",
                        HTA = item.HTA == false ? "NO" : "SI",
                        Nombre = item.Person.Nombre,
                        NumeroTurno = item.Turn.Turno,
                        Observacion = item.Observacion,
                        Profesional = item.Profesional == false ? "NO" : "SI",
                        TipoTramite = item.Turn.TypesLicense.Descripcion,
                        TurnId = item.TurnId,
                        VisionMonocular = item.VisionMonocular == false ? "NO" : "SI"
                    };

                    turns.Add(vm);

                }

            }
            catch (Exception e)
            {

                throw;
            }

            return turns;
        }



        public JsonResult Crear(MedicalPersonViewModel viewModel )
        {
            if (viewModel == null)
            {
                return Json(new { responseCode = "-10" });
            }

            MedicalPerson medicalPerson = new MedicalPerson
            {
                AcidoUrico = viewModel.AcidoUrico,
                Avod = viewModel.Avod,
                Avoi = viewModel.Avoi,
                VisionMonocular = viewModel.VisionMonocular,
                TurnId = viewModel.TurnId,
                Colesterol = viewModel.Colesterol,
                ConduceConAnteojos = viewModel.ConduceConAnteojos,
                DBT = viewModel.DBT,
                Discromatopsia = viewModel.Discromatopsia,
                Fuma = viewModel.Fuma,
                GAA = viewModel.GAA,
                HTA = viewModel.HTA,
                Observacion = viewModel.Observacion,
                PersonId = viewModel.PersonId,
                Profesional = viewModel.Profesional                
            };

            db.MedicalPersons.Add(medicalPerson);
            db.SaveChanges();
            //Edito la fecha de Nacimiento
            Person person = db.People.Find(viewModel.PersonId);
            person.FechaNacimiento = Convert.ToDateTime(viewModel.FechaNacimiento);
            db.Entry(person).State = EntityState.Modified;
            db.SaveChanges();




            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);
        }


        public JsonResult Edit(MedicalPersonViewModel viewModel)
        {
            MedicalPerson person = db.MedicalPersons.Find(viewModel.Id);
            if (person == null)
            {
                return Json(new { responseCode = "-10" });
            }

            person.AcidoUrico = viewModel.AcidoUrico;
            person.Avod = viewModel.Avod;
            person.Avoi = viewModel.Avoi;
            person.Colesterol = viewModel.Colesterol;
            person.ConduceConAnteojos = viewModel.ConduceConAnteojos;
            person.DBT = viewModel.DBT;
            person.Discromatopsia = viewModel.Discromatopsia;
            person.Fuma = viewModel.Fuma;
            person.GAA = viewModel.GAA;
            person.HTA = viewModel.HTA;
            person.Observacion = viewModel.Observacion;
            person.Profesional = viewModel.Profesional;
            person.VisionMonocular = viewModel.VisionMonocular;



            db.Entry(person).State = EntityState.Modified;

            db.SaveChanges();

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);
        }

        public JsonResult Delete(int id)
        {
            if (id == 0)
            {
                return Json(new { responseCode = "-10" });
            }

            MedicalPerson medicalPerson = db.MedicalPersons.Find(id);
            db.MedicalPersons.Remove(medicalPerson);
            db.SaveChanges();


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
