using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.Models
{
    public class PersonMedical
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }
        public int Avoi { get; set; }
        public int Avod { get; set; }
        public bool Fuma { get; set; }
        public bool Profesional { get; set; }
        public bool ConduceConAnteojos { get; set; }
        public bool VisionMonocular { get; set; }
        public bool Discromatopsia { get; set; }
        public bool HTA { get; set; }
        public bool DBT { get; set; }
        public bool GAA { get; set; }
        public bool AcidoUrico { get; set; }
        public bool Colesterol { get; set; }  
        public String Observacion { get; set; }

    }
}