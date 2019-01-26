using GestionDeTurnos.Helpers;
using GestionDeTurnos.Models;
using GestionDeTurnos.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static GestionDeTurnos.Tags.AutenticadoAttribute;

namespace GestionDeTurnos.Controllers
{
    [NoLoginAttribute]
    public class LoginController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private Usuario um = new Usuario();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }



        public JsonResult Autenticar(LoginViewModel model)
        {
            var rm = new ResponseModel();

            var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();

            if (ModelState.IsValid)
            {
                this.um.Nombreusuario = model.NombreDeUsuario;
                this.um.Contraseña = model.Password;

                rm = um.Autenticarse();

                if (rm.response)
                {

                    //Aca deberia a la pantalla default, y si no tiene permiso deberia ir a una pantalla q le indique q no tiene permisos
                    Rol rol = db.Rols.Find(rm.result);

                    rm.href=rol.Window.Url;

                   // rm.href = Url.Action("Index/Turns");
                }
            }
            else
            {
                rm.SetResponse(false, "Debe llenar los campos para poder autenticarse.");
            }

            return Json(rm);
        }


    }
}