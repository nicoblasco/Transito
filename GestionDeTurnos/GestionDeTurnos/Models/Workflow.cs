using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.Models
{
    public class Workflow
    {
        public int Id { get; set; }
        public int TypesLicenseID { get; set; }
        public virtual TypesLicense TypesLicense { get; set; }

        public virtual ICollection<SectorWorkflow> SectorWorkflows { get; set; }



    }
}