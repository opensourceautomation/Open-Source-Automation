using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.EnterpriseServices.Internal;
using System.IO;
using System.Reflection;

namespace GAC
{
    class Program
    {
        static void Main(string[] args)
        {
            Publish p = new Publish();
            p.GacInstall(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/lib/OSAE.API.dll");
            p.GacInstall(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/lib/MySql.Data.dll");
        }
    }
}
