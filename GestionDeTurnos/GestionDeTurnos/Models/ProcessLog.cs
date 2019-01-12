using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionDeTurnos.Models
{
    public class ProcessLog
    {
        public int Id { get; set; }

        public int ProcessId { get; set; }
        public virtual Process Process { get; set; }

        public DateTime Fecha { get; set; }
        public string Name { get; set; }

        public bool IsOk { get; set; }
        public string ErrorDescripcion { get; set; }

    }
}