using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.ViewModel
{
    public class LicenseIndexViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Dni { get; set; }
        public string Estado { get; set; }

        public string FechaRecibo { get; set; }
        public string FechaRetiro { get; set; }
        public string FechaDeNacimiento { get; set; }

        public string Domicilio { get; set; }
        public int? CalleId { get; set; }

        public string DomicilioNro { get; set; }
        public int? Nacionalidad { get; set; }
        public string Otorgamiendo { get; set; }
        public string Vencimiento { get; set; }

        public int? PersonId { get; set; }

        public string Tel_Particular { get; set; }

        public string Tel_Celular { get; set; }

        public string Email { get; set; }

        public virtual int[] SelectedClases { get; set; }


    }
}