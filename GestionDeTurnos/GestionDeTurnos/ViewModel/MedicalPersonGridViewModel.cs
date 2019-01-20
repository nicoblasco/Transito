using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.ViewModel
{
    public class MedicalPersonGridViewModel
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public int TurnId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string DNI { get; set; }
        public string Edad { get; set; }
        public string FechaIngreso { get; set; }
        public string NumeroTurno { get; set; }
        public string TipoTramite { get; set; }
        public string FechaNacimiento { get; set; }
        public string Avoi { get; set; }
        public string Avod { get; set; }
        public string Fuma { get; set; }
        public string Profesional { get; set; }
        public string ConduceConAnteojos { get; set; }
        public string VisionMonocular { get; set; }
        public string Discromatopsia { get; set; }
        public string HTA { get; set; }
        public string DBT { get; set; }
        public string GAA { get; set; }
        public string AcidoUrico { get; set; }
        public string Colesterol { get; set; }
        public String Observacion { get; set; }
    }
}