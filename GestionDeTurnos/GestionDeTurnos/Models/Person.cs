﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Dni { get; set; }
        public DateTime FechaNacimiento { get; set; }
    }
}