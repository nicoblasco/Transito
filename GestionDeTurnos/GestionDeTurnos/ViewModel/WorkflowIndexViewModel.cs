using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.ViewModel
{
    public class WorkflowIndexViewModel
    {
        public int Id { get; set; }
        public int TypesLicenseID { get; set; }

        public int SectorID { get; set; }

        public int Orden { get; set; }
    }
}