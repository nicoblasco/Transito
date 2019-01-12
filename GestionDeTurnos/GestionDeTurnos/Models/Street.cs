using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.Models
{
    public class Street
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string Baja { get; set; }
        public string DescripcionOficial { get; set; }
        public string Codigo { get; set; }
        public string DescripcionGoogle { get; set; }
    }
}