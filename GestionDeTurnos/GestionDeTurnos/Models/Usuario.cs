using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GestionDeTurnos.Helpers;
using GestionDeTurnos.Models;

namespace GestionDeTurnos.Models
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }

        public string Nombreusuario { get; set; }

        public string Contraseña { get; set; }

        public bool Enable { get; set; }

        public int RolId { get; set; }
        public virtual Rol Rol { get; set; }

        public ResponseModel Autenticarse()
        {
            var rm = new ResponseModel();

            try
            {
                using (var ctx = new ApplicationDbContext())
                {
                    var usuario = ctx.Usuarios.Where(x => x.Nombreusuario == this.Nombreusuario && x.Contraseña == this.Contraseña).SingleOrDefault();
                    if (usuario != null)
                    {
                        //SessionHelper.AddUserToSession(usuario.UsuarioId.ToString());
                        SessionHelper.AddUserToSessionTicket(usuario.UsuarioId, usuario.Nombreusuario, usuario.RolId.ToString());

                        rm.result = usuario.RolId;
                        rm.SetResponse(true);
                    }
                    else
                    {
                        rm.SetResponse(false, "Acceso denegado al sistema");
                    }
                }
            }
            catch (Exception e)
            {
                rm.SetResponse(false, "Error en el sistema, contacte a su administrador");//Base de datos
            }
            return rm;
        }

        public Usuario Obtener(int Usuarioid)
        {

            using (var ctx = new ApplicationDbContext())
            {
                Usuario usuario = ctx.Usuarios.Find(Usuarioid);

                return usuario;

            }
        }

    }
}