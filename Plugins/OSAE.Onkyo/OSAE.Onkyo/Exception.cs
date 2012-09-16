using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OSAE.Onkyo
{
   
    public class ParseVarsException : Exception
    {
        public ParseVarsException() : base("IP Address is Null or cannot be parsed") { }
    }
    public class InvalidOnkyoStringException : Exception
    {
        public InvalidOnkyoStringException() : base("Invalid Onkyo server reply string") { }
    }
    
}
