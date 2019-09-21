using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.Models
{
    public class Nighborhood
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Direccion { get; set; }
        public string Altura { get; set; }
        public string Partida { get; set; }
        public string Barrio { get; set; }
        public string Nombre { get; set; }
        public string Zona { get; set; }
        public string NombreCorto { get; set; }
        public int Codigo { get; set; }
        public string Habitantes2010 { get; set; }
        public string ProyecionHabitantes2016 { get; set; }
        public string ProyecionHabitantes2020 { get; set; }
        public string Detalle { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string Perimetro { get; set; }
        public string Area { get; set; }
    }
}