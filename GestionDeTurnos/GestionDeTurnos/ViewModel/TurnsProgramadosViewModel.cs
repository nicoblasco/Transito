using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.ViewModel
{
    public class TurnsProgramadosViewModel
    {
        public int Id { get; set; }
        public string Turno { get; set; }
        public string Horario { get; set; }
        public string Tipo { get; set; }
        public string Dni { get; set; }

        public string Nombre { get; set; }

        public string Apellido { get; set; }
    }
}