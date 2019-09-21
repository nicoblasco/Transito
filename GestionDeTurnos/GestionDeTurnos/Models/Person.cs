using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Dni { get; set; }
        public DateTime? FechaNacimiento { get; set; }

        public string Calle { get; set; }
        public string CalleNro { get; set; }
        public int? StreetId { get; set; }
        public virtual Street Street { get; set; }
        public string Altura { get; set; }

        public int? CountryId { get; set; }

        public virtual Country Country { get; set; }

        public string Tel_Particular { get; set; }

        public string Tel_Celular { get; set; }

        public string Email { get; set; }


        public DateTime? Vencimiento_licencia { get; set; }

        public string Barrio { get; set; }

        public int? NighborhoodId { get; set; }
        public virtual Nighborhood Nighborhood { get; set; }
    }
}