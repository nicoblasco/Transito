using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.ViewModel
{
    public class TurnDemoraViewModel
    {
        public int Id { get; set; }
        public string Turno { get; set; }
        public string Desde { get; set; }
        public string Sector { get; set; }
        public string Tiempo { get; set; }
    }
}