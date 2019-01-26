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
        public string TipoTramite { get; set; }

        public string Gestion { get; set; }
        public string Tel_Particular { get; set; }
        public string Tel_Celular { get; set; }
        public string Estado { get; set; }

        public string Barrio { get; set; }


        public string Vencimiento_licencia { get; set; }

        public DateTime FechaTurno { get; set; }
        public DateTime Fecha { get; set; }



        public bool Asignado { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public int? UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }

    }
}