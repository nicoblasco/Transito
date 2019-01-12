using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }

        public bool Predeterminado { get; set; }
    }
}