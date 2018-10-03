using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.ViewModel
{
    public class HomeViewModel
    {
        public bool ConTurno { get; set; }
        [Required]
        public string DNI { get; set; }
    }
}