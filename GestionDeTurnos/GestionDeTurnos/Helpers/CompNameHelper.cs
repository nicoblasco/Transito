using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace GestionDeTurnos.Helpers
{
    public class CompNameHelper
    {
        public static string DetermineCompName(string IP)
        {
            IPAddress myIP = IPAddress.Parse(IP);
            IPHostEntry GetIPHost = Dns.GetHostEntry(myIP);
            List<string> compName = GetIPHost.HostName.ToString().Split('.').ToList();
            return compName.First();
        }

    }
}