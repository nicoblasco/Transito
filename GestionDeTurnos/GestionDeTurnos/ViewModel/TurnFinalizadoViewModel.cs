using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.ViewModel
{
    public class TurnFinalizadoViewModel
    {
        public int Id { get; set; }
        public string Turno { get; set; }
        public string Ingreso { get; set; }

        public string Finalizado { get; set; }
        public string FechaTurno { get; set; }
        public string Tiempo { get; set; }
    }
}