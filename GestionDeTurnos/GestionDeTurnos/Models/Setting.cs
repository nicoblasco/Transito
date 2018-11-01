using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.Models
{
    public class Setting
    {
        public int Id { get; set; }
        public string Clave { get; set; }
        public string Texto1 { get; set; }
        public string Texto2 { get; set; }
        public int? Numero1 { get; set; }
        public int? Numero2 { get; set; }
        public bool? Logico1 { get; set; }
        public bool? Logico2 { get; set; }

        public DateTime? Fecha1 { get; set; }
        public DateTime? Fecha2 { get; set; }


    }
}