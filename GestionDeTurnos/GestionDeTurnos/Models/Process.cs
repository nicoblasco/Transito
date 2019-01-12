using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.Models
{
    public class Process
    {
        public int Id { get; set; }
        public DateTime FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public string Name { get; set; }

        public string Detalle { get; set; }



    }
}