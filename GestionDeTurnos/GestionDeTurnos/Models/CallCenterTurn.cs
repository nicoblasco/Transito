using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.Models
{
    public class CallCenterTurn
    {
        public int Id { get; set; }

        public string DNI { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string TipoTramite { get; set; }

        public DateTime FechaTurno { get; set; }
        public DateTime Fecha { get; set; }

        public bool Asignado { get; set; }

    }
}