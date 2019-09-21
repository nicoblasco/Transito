using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.ViewModel
{
    public class TurnsSeachViewModel
    {
        public int Id { get; set; }
        public string Turno { get; set; }
        public string FechaTurno { get; set; }
        public string Ingreso { get; set; }
        public string Salida { get; set; }
        public string Tipo { get; set; }
        public string DNI { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Estado { get; set; }
        public string IncompletoNoSePresento { get; set; }


    }
}