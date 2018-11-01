using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.Models
{
    public class TypesLicense
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string Codigo { get; set; }

        //La referencia para asociar el nombre que viene del call center
        public string Referencia { get; set; }

        //Numero inicial del tipo de turno, para no prestar confuciones
        public int NumeroInicial { get; set; }

    }
}