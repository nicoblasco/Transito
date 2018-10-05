using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.ViewModel
{
    public class SectorWorkflowViewModel
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }

        public bool Selected { get; set; }

        public int Orden { get; set; }


    }
}