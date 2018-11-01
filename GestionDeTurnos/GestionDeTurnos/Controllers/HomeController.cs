using GestionDeTurnos.Models;
using GestionDeTurnos.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace GestionDeTurnos.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ConTurno()
        {

            return View("Ingreso");
        }





        public ActionResult TramiteIniciado()
        {
            return View("Ingreso");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TramiteIniciado([Bind(Include = "DNI")] HomeViewModel model)
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
            int? NumeroSecuencia;
            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59

            if (ModelState.IsValid)
            {
                if (model == null)
                {
                    //No tiene turno
                    return View("AccesoDenegado");


                }


                //Con el DNI me fijo si existe la persona
                //Verifico con los datos que vienen del CallCenter
                List<Setting> setting = db.Settings.ToList();
                //Que pasa si tengo mas de un turno???
                CallCenterTurn callCenterTurn = db.CallCenterTurns.Where(x => x.DNI == model.DNI && x.FechaTurno>=startDateTime && x.FechaTurno<=endDateTime && x.Asignado==false ).FirstOrDefault();
                int? TiempoMaximoEspera = setting.Where(x => x.Clave == "TIEMPO_MAXIMO_ESPERA").FirstOrDefault().Numero1;


                if (callCenterTurn == null)
                {
                    //No tiene turno
                    return View("AccesoDenegado");
                }

                else
                //Valido si no se paso de tiempo
                {
                    if (TiempoMaximoEspera != null)
                    {
                        //Si es 0 no lo tomo en cuenta
                        if (TiempoMaximoEspera != 0)
                        {
                            DateTime fechaMax = callCenterTurn.FechaTurno.AddMinutes(TiempoMaximoEspera.Value);
                            if (DateTime.Now > fechaMax)
                            {
                                //Se le paso el turno
                                return View("AccesoDenegado");
                            }
                        }
                    }

                    //Obtengo el tipo de turno
                    //La relacion entre como viene y como esta en el sistema
                    TypesLicense typesLicense = db.TypesLicenses.Where(x => x.Referencia == callCenterTurn.TipoTramite).FirstOrDefault();

                    if (typesLicense == null)
                    {
                        //Le asigno un turno por defecto??
                        typesLicense = db.TypesLicenses.Where(x => x.Codigo == "OR").FirstOrDefault();

                    }


                    //Si paso todas las validaciones

                    //Verifico si la persona existe, si no existe la doy de alta en el maestro de personas

                    Person person = db.People.Where(x => x.Dni == model.DNI).FirstOrDefault();
                    if (person == null)
                    {
                        person = new Person
                        {
                            Nombre = callCenterTurn.Nombre,
                            Apellido = callCenterTurn.Apellido,
                            Dni = callCenterTurn.DNI,
                            FechaNacimiento = callCenterTurn.FechaNacimiento
                        };
                        db.People.Add(person);
                        db.SaveChanges();
                    }

                    //Asigno el turno
                    callCenterTurn.Asignado = true;
                    db.Entry(callCenterTurn).State = EntityState.Modified;



                    //GENERO EL TURNO y Emito el ticket                   



                    if (!db.Turns.Any())
                        NumeroSecuencia = 1;
                    else
                        NumeroSecuencia = db.Turns.Where(x => x.FechaIngreso >= startDateTime && x.FechaIngreso <= endDateTime).Max(x => x.Secuencia);

                    if (NumeroSecuencia == null)
                        NumeroSecuencia = (typesLicense.NumeroInicial == 0) ? 1 : typesLicense.NumeroInicial;
                    else
                        NumeroSecuencia = NumeroSecuencia++;


                    Turn turn = new Turn
                    {
                        FechaIngreso = DateTime.Now,
                        PersonID = person.Id,
                        TypesLicenseID = typesLicense.Id,
                        //Armo el codigo del dia
                        Turno = typesLicense.Codigo + NumeroSecuencia.Value.ToString("0000"),
                        Secuencia = NumeroSecuencia.Value,
                        FechaTurno = callCenterTurn.FechaTurno

                    };
                    db.Turns.Add(turn);



                    db.SaveChanges();


                    //Obtengo el primer Sector del Workflow para el tipo de tramite
                    List<Workflow> workflows = db.Workflows.Where(x => x.TypesLicenseID == typesLicense.Id).ToList();

                    SectorWorkflow sectorWorkflow = db.SectorWorkflows.Where(x => x.Workflow.TypesLicenseID == typesLicense.Id && x.Orden == 1).FirstOrDefault();


                    Tracking tracking = new Tracking
                    {
                        SectorID = sectorWorkflow.SectorID,
                        TurnID = turn.Id
                    };

                    db.Trackings.Add(tracking);
                    db.SaveChanges();


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