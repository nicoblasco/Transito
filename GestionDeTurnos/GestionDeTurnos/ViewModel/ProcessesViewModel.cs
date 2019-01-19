using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.ViewModel
{
    public class ProcessesViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public string FechaProcesamientoInicio { get; set; }
        public string FechaProcesamientoFin { get; set; }

        public string FechaProcesada { get; set; }

    }
}