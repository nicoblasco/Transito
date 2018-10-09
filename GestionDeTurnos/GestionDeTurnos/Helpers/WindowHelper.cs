using GestionDeTurnos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.Helpers
{
    public class WindowHelper
    {
        public static int GetWindowId(string Modulo, string Pantalla)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            //Primero obtengo el Modulo
            int ModuleId = db.Modules.Where(x => x.Descripcion == Modulo && x.Enable == true).Select(x => x.Id).FirstOrDefault();
            return db.Windows.Where(x => x.Descripcion == Pantalla && x.Enable == true && x.ModuleId == ModuleId).Select(x => x.Id).FirstOrDefault();
        }

        
    }
}