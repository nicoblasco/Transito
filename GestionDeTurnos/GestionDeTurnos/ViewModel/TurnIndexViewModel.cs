using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.ViewModel
{
    public class TurnIndexViewModel
    {
        public int Id { get; set; }
        public string Turno { get; set; }
        public string Ingreso { get; set; }
        public string Salida { get; set; }
        public string Tipo { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Estado { get; set; }

     //   public string SectorAnterior { get; set; }
        public string SectorActual { get; set; }
        public string SectorProximo { get; set; }

    }
}