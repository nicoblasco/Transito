using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.ViewModel
{
    public class RolViewModel
    {
        public int RolId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public string IsAdmin { get; set; }

        public string Window { get; set; }
    }
}