using GestionDeTurnos.Helpers;
using GestionDeTurnos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.ViewModel
{
    public class PermissionViewModel
    {
        public static bool TienePermisoAlta(int WindowId)
        {
            bool boPermiso = false;
            if (IsAdmin())
                return true;

            List<Permission> permission = new List<Permission>();
            permission = GetPermisos();
            boPermiso = permission.Where(x => x.WindowId == WindowId).Select(x => x.AltaModificacion).FirstOrDefault();
            return boPermiso;
        }

        public static bool TienePermisoBaja(int WindowId)
        {
            bool boPermiso = false;

            if (IsAdmin())
                return true;

            List<Permission> permission = new List<Permission>();
            permission = GetPermisos();
            boPermiso = permission.Where(x => x.WindowId == WindowId).Select(x => x.Baja).FirstOrDefault();
            return boPermiso;


        }

        public static bool TienePermisoAcesso(int WindowId)
        {
            bool boPermiso = false;

            if (IsAdmin())
                return true;

            List<Permission> permission = new List<Permission>();
            permission = GetPermisos();
            boPermiso = permission.Where(x => x.WindowId == WindowId).Select(x => x.Consulta).FirstOrDefault();
            return boPermiso;
        }

        public static List<Permission> GetPermisos()
        {
            Usuario usuario = new Usuario();

            usuario = usuario.Obtener(SessionHelper.GetUser());
            return new Permission().Obtener(usuario.RolId);

        }

        public static bool IsAdmin()
        {

            using (var ctx = new ApplicationDbContext())
            {
                Usuario usuario = ctx.Usuarios.Find(SessionHelper.GetUser());
                if (usuario == null)
                    return false;

                Rol rol = ctx.Rols.Find(usuario.RolId);

                //if (rol.Nombre == "Administrador")
                if (rol.IsAdmin)
                    return true;
                else
                    return false;


            }
        }
    }
}