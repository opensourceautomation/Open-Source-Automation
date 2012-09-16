using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace OSAE.Onkyo
{
    class DnsUtils
    {
        public static string GetLocalIP() 
        {
            string _IP = null;
            // Resolves a host name or IP address to an IPHostEntry instance.  
            // IPHostEntry - Provides a container class for Internet host address information.   
            System.Net.IPHostEntry _IPHostEntry = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());  
 
            // IPAddress class contains the address of a computer on an IP network.  
            foreach (System.Net.IPAddress _IPAddress in _IPHostEntry.AddressList)  
            {  
                // InterNetwork indicates that an IP version 4 address is expected  
                // when a Socket connects to an endpoint 
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork") 
                {  
                    _IP = _IPAddress.ToString(); 
                }
                if (_IP != null) { break; } // Found it so we can exit loop
            }  
            return _IP;  
           
        }
       
    }
}
