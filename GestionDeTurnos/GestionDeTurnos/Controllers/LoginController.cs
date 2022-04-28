using GestionDeTurnos.Models;
using GestionDeTurnos.ViewModel;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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
            Session["EsReintento"] = false;
            List<TerminalViewModel> terminalesVM = new List<TerminalViewModel>();
            var lTerminales = db.Terminals.Where(x => x.Enable).OrderBy(x => x.Descripcion).ToList();

            foreach (var item in lTerminales)
            {
                TerminalViewModel vm = new TerminalViewModel
                {
                    Id = item.Id,
                    Descripcion = item.Descripcion
                };
                terminalesVM.Add(vm);
            }

            ViewBag.listaTerminales = terminalesVM;

            return View();
        }



        public JsonResult Autenticar(LoginViewModel model)
        {
            var rm = new ResponseModel();
            string usuarioEnUso;

            ModelState.Remove("EsReintento");
            var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();

            if (ModelState.IsValid)
            {
                this.um.Nombreusuario = model.NombreDeUsuario;
                this.um.Contraseña = model.Password;

                //Verifico si la terminal esta en uso

                Terminal terminal = db.Terminals.Find(model.TerminalId);

                bool esReintento = (bool)Session["EsReintento"];

                if (terminal.Usuario!= null && !esReintento)
                {
                    if (terminal.Usuario.Nombreusuario != model.NombreDeUsuario)
                    {
                        usuarioEnUso = terminal.Usuario.Nombreusuario;
                        Session["EsReintento"] = true;
                        rm.SetResponse(false, "La Terminal se encuentra en uso por el usuario " + usuarioEnUso + ". Si usted va a usar la terminal seleccionada vuelva a presionar Ingresar.");
                        return Json(rm);
                    }

                }

                

                rm = um.Autenticarse();

                if (rm.response)
                {

                    //Aca deberia a la pantalla default, y si no tiene permiso deberia ir a una pantalla q le indique q no tiene permisos
                    Rol rol = db.Rols.Find(rm.result);

                    if (terminal.Usuario?.Nombreusuario != model.NombreDeUsuario)
                    {
                        terminal.UsuarioId = rm.usuarioId;
                        db.Entry(terminal).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    rm.href = rol.Window.Url;

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