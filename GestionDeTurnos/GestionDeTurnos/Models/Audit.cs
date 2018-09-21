using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.Models
{
    public class Audit
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public virtual Usuario User { get; set; }

        public int WindowId { get; set; }
        public virtual Window Window { get; set; }
        public string Accion { get; set; }
        public DateTime Fecha { get; set; }

        public string Clave { get; set; }
        public string Entidad { get; set; }
    }
}