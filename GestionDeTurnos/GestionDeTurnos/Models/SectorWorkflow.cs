using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.Models
{
    public class SectorWorkflow
    {
        [Key, Column(Order = 0)]
        public int WorkflowID { get; set; }
       [Key, Column(Order = 1)]
        public int SectorID { get; set; }
        public virtual Workflow Workflow { get; set; }
        public virtual Sector Sector { get; set; }

        public int Orden { get; set; }
    }
}