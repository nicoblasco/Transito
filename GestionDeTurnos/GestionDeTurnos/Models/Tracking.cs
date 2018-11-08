using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.Models
{
    public class Tracking
    {
        public int Id { get; set; }
        public int TurnID { get; set; }
        public virtual Turn Turn { get; set; }

        public int? TerminalID { get; set; }
        public virtual Terminal Terminal { get; set; }

        public int SectorID { get; set; }
        public virtual Sector Sector { get; set; }

        public int? UsuarioID { get; set; }
        public virtual Usuario Usuario { get; set; }

        public DateTime? FechaIngreso { get; set; }
        public DateTime? FechaSalida { get; set; }

        public DateTime FechaCreacion { get; set; }

        public TimeSpan? Tiempo { get; set; }

        public int CantidadDeLlamados { get; set; }

        public int? StatusID { get; set; }
        public virtual Status  Status { get; set; }

    }
}