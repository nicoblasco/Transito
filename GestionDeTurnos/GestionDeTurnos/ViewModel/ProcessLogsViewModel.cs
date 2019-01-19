using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.ViewModel
{
    public class ProcessLogsViewModel
    {
        public int Id { get; set; }
        public string Fecha { get; set; }
        public string Nombre { get; set; }

        public string ErrorDescription { get; set; }
        public int ProcesoId { get; set; }

    }
}