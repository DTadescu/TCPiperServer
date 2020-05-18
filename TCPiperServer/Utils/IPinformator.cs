using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Net;

namespace TCPiperServer.Utils
{
   static class IPinformator
    {
        

        public static String GetMyIp()
        {
            // Получение имени компьютера.
            String host = Dns.GetHostName();
            // Получение ip-адреса.
            IPAddress[] ipArray = Dns.GetHostEntry(host).AddressList;
            string ips = $"Host: {host}\r\n";
            foreach (var ip in ipArray)
            {
                if (ip.ToString().Length > 10 && ip.ToString().Length < 16) return ip.ToString();
            }
            return ips;
        }
    }
}
