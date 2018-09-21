using GestionDeTurnos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.Helpers
{
    public class AuditHelper
    {
        public static void Auditar(string Accion, string Id, string Entidad, string ModuleDescription, string WindowDescription)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            //Primero obtengo el Modulo
            //Audito


            Audit audit = new Audit();
            audit.Accion = Accion;
            audit.Clave = Id;
            audit.Entidad = Entidad;
            audit.Fecha = DateTime.Now;
            audit.UsuarioId = SessionHelper.GetUser();
            audit.WindowId = WindowHelper.GetWindowId(ModuleDescription, WindowDescription);
            db.Audits.Add(audit);
            db.SaveChanges();

        }
    }
}