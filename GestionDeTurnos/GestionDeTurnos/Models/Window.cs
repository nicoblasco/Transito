using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.Models
{
    public class Window
    {
        public int Id { get; set; }

        public int ModuleId { get; set; }
        public virtual Module Module { get; set; }
        public string Descripcion { get; set; }
        public bool Enable { get; set; }
        public string Url { get; set; }
        public string Orden { get; set; }

    }
}