using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.ViewModel
{
    public class TurnEnProcesoViewModel
    {
        public int Id { get; set; }
        public string Turno { get; set; }
        public string Ingreso { get; set; }
        public string Sector { get; set; }
        public string Tiempo { get; set; }
        public string Usuario { get; set; }
    }
}