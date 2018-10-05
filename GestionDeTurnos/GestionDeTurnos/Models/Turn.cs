using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.Models
{
    public class Turn
    {
        public int Id { get; set; }
        public string Turno { get; set; }

        public int TypesLicenseID { get; set; }
        public virtual TypesLicense TypesLicense { get; set; }

        public int PersonID { get; set; }
        public virtual Person Person { get; set; }

        public DateTime FechaIngreso { get; set; }

    }
}