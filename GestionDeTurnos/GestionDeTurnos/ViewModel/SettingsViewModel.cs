using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.ViewModel
{
    public class SettingsViewModel
    {
        public int TiempoMaximoEspera { get; set; }

        public int NumeroInicialTurno { get; set; }

        public int CantidadLlamadosPermitidos { get; set; }

        public string Video { get; set; }
        public bool VideoBorrado { get; set; }
    }
}