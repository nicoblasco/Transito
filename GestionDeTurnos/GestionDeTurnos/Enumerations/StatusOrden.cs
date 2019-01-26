using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.Enumerations
{

    public enum StatusOrden : int
    {
        EN_ESPERA = 1,
        LLAMANDO = 2,
        EN_PROCESO = 3,
        FINALIZADO = 4,
        NO_SE_PRESENTO =5,
        INCOMPLETO = 6,
        FIN_POR_CAMBIO_TURNO=7


    }
}