﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace GestionDeTurnos.ViewModel
{
    public class LoginViewModel
    {
        [Required]
        public string NombreDeUsuario { get; set; }

        [Required]
        public string Password { get; set; }
    }
}