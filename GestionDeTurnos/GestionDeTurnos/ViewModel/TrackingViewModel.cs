using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.ViewModel
{
    public class TrackingViewModel
    {
        public int Id { get; set; }
        public string Turn { get; set; }
        public string Terminal { get; set; }
        public string Sector { get; set; }

        public string Usuario { get; set; }

        public string FechaIngreso { get; set; }
        public string FechaSalida { get; set; }

        public string FechaCreacion { get; set; }

        public string Tiempo { get; set; }

        public string CantidadDeLlamados { get; set; }

        public string Status { get; set; }

    }
}