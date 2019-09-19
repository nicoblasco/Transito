using GestionDeTurnos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.Helpers
{
    public class UserHelper
    {
        public static string GetUserName(int userId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string name="";
            //Primero obtengo el Modulo
            Usuario user = db.Usuarios.Where(x => x.UsuarioId == userId).FirstOrDefault();
            if (user != null)
            {
                if (String.IsNullOrEmpty(user.Apellido) && String.IsNullOrEmpty(user.Nombre))
                    name = user.Nombreusuario;
                else
                {
                    if (String.IsNullOrEmpty(user.Apellido))
                        name = user.Nombre;
                    else
                        name = user.Apellido + " " + user.Nombre;
                }
            }

            return name;
        }
    }
}