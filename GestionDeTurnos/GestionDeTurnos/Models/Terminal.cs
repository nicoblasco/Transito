using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.Models
{
    public class Terminal
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }

        public string IP { get; set; }

        public int SectorID { get; set; }
        public virtual Sector Sector { get; set; }

        public bool Enable { get; set; }

        public int? UsuarioId { get; set; }
        public virtual Usuario  Usuario { get; set; }
    }
}