using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;  // Windows PowerShell assembly.


namespace OSAE.PowerShellTools
{
    [Cmdlet(VerbsCommon.Get, "OSAPS")]
    public class OSAPS : Cmdlet
    {
        [Parameter(Mandatory = true)]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string name;

        [Parameter(Mandatory = false)]
        public string Property
        {
            get { return property; }
            set { property = value; }
        }
        private string property = "";

        // Overide the ProcessRecord method to process
        // the supplied user name and write out a 
        // greeting to the user by calling the WriteObject
        // method.
        protected override void ProcessRecord()
        {
            OSAE osae = new OSAE(name);

            OSAEObject obj = osae.GetObjectByName(name);

            if (property != "")
            {
                ObjectProperty objProp = osae.GetObjectPropertyValue(name, property);

                WriteObject(objProp);
            }
            else
            {

                WriteObject(obj);
            }

            //WriteObject("Hello " + name + "!");
        }

    }

    [Cmdlet(VerbsCommon.Set, "OSAPS")]
    public class OSAPSSet : Cmdlet
    {
        [Parameter(Mandatory = true)]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string name;

        [Parameter(Mandatory = true)]
        public string Property
        {
            get { return property; }
            set { property = value; }
        }
        private string property = "";

        [Parameter(Mandatory = true)]
        public string Value
        {
            get { return val; }
            set { val = value; }
        }
        private string val = "";

        // Overide the ProcessRecord method to process
        // the supplied user name and write out a 
        // greeting to the user by calling the WriteObject
        // method.
        protected override void ProcessRecord()
        {
            OSAE osae = new OSAE(name);

            OSAEObject obj = osae.GetObjectByName(name);

            osae.ObjectPropertySet(name, property, val);

            WriteObject(true);


            //WriteObject("Hello " + name + "!");
        }

    }

    [Cmdlet(VerbsLifecycle.Invoke, "OSAPS")]
    public class OSAPSInovke : Cmdlet
    {
        [Parameter(Mandatory = true)]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string name;

        [Parameter(Mandatory = true)]
        public string Method
        {
            get { return method; }
            set { method = value; }
        }
        private string method = "";

        [Parameter(Mandatory = true)]
        public string Value
        {
            get { return val; }
            set { val = value; }
        }
        private string val = "";

        // Overide the ProcessRecord method to process
        // the supplied user name and write out a 
        // greeting to the user by calling the WriteObject
        // method.
        protected override void ProcessRecord()
        {
            OSAE osae = new OSAE(name);

            osae.MethodQueueAdd(name, Method, Value, "");


            WriteObject(true);


            //WriteObject("Hello " + name + "!");
        }

    }

    [Cmdlet(VerbsCommon.Show, "OSAPS")]
    public class OSAPSShow : Cmdlet
    {
        [Parameter(Mandatory = true)]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string name;



        // Overide the ProcessRecord method to process
        // the supplied user name and write out a 
        // greeting to the user by calling the WriteObject
        // method.
        protected override void ProcessRecord()
        {
            OSAE osae = new OSAE(name);

            OSAEObject obj = osae.GetObjectByName(name);

            WriteObject("Name: " + obj.Name);
            WriteObject("State: " + obj.State);
            WriteObject("Description: " + obj.Description);
            WriteObject("Container: " + obj.Container);
            WriteObject("Address: " + obj.Address);
            WriteObject("Enabled: " + obj.Enabled);
            WriteObject("Base Type: " + obj.BaseType);
            WriteObject("Type: " + obj.Type);

            foreach (string method in obj.Methods)
            {
                WriteObject("Method: " + method);
            }

            foreach (ObjectProperty prop in obj.Properties)
            {
                WriteObject("Property (" + prop.DataType + "): " + prop.Name + " Value = " + prop.Value);
            }

            WriteObject("Updated: " + obj.LastUpd);




            //WriteObject("Hello " + name + "!");
        }

    }

}
