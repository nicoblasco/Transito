using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.Models
{
    public class License
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }

        public DateTime? FechaOtorgamiento { get; set; }
        public DateTime? FechaVencimiento { get; set; }

        public int? TypesLicenseId { get; set; }
        public virtual TypesLicense TypesLicense { get; set; }

        public virtual ICollection<LicenseClass>  LicenseClasses { get; set; }

        public string Estado { get; set; }

        public DateTime? FechaRecibo { get; set; }
        public DateTime? FechaRetiro { get; set; }
        public string Firma { get; set; }

        public int? TurnId { get; set; }
        public virtual Turn Turn { get; set; }


    }
}