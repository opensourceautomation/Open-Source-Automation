
using System;
using System.IO;


namespace OSAE.Android
{
    public class AndroidDevice
    {
        string pName;
        private string _name;
        private string _owner;
        private string _gcmid;
    
        private Logging logging = Logging.GetLogger("Android");
                
        private GCMSender gcmsender = new GCMSender(null,"AIzaSyAXrHDNsYhU-nQowJzLB-YeMyOG74jjjVs");

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }
        public string GCMID
        {
            get { return _gcmid; }
            set { 
                    _gcmid = value;
                    gcmsender.DeviceToken = _gcmid;
                }
        }


        public AndroidDevice(string name, string pluginname)
        {
            _name = name;
            pName = pluginname;

        }

        private void log(String message, bool alwaysLog)
        {
            try
            {
                logging.AddToLog(message, alwaysLog);
            }
            catch (IOException ex)
            {
                        //do nothing
            }


        }


        public void ProcessCommand(String method_name, String parameter_1, String parameter_2)
        {

            string category;
            string level;
            string osaid; //eventually we will use this to keep track on osa side to handle removing notification once it is seen on one of the devices with the same owner, but for now it is not used
            string messagedate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"); ;

            string payload;
            string strResponse;

            //look up address again in case it changed, but don't update if we get back a null or blank value (this probably means they renamed the object so looking up by the name we have returns nothing)
            //_gcmid = OSAEObjectManager.GetObjectByName(_name).Address;
            if (OSAEObjectPropertyManager.GetObjectPropertyValue(_name, "GCMID").Value != null)
            {
                if (!OSAEObjectPropertyManager.GetObjectPropertyValue(_name, "GCMID").Value.Equals(""))
                {
                    _gcmid = OSAEObjectPropertyManager.GetObjectPropertyValue(_name, "GCMID").Value;
                }
            }

            gcmsender.DeviceToken = _gcmid;


            switch (method_name)
            {
                case "NOTIFY":

                    log("NOTIFY event triggered (" + _name + "), parameter_1=" + parameter_1 + ", parameter_2=" + parameter_2, false);
                    log("address = " + _gcmid, false);

                    category = "default";
                    level = "5";
                    osaid = "-1"; //eventually we will use this to keep track on osa side to handle removing notification once it is seen on one of the devices with the same owner, but for now it is not used

                    if (parameter_2 != "")
                    {
                        string[] parts = parameter_2.Split(',');

                        if (parts.Length > 0) { category = parts[0]; }
                        if (parts.Length > 1) { level = parts[1]; }
                    }


                    payload = "\"type\" : \"notification\" \"message\" : \"" + parameter_1 + "\" \"category\" : \"" + category + "\" \"level\" : \"" + level + "\" \"osaid\" : \"" + osaid + "\" \"messagedate\" : \"" + messagedate + "\" ";

                    strResponse = gcmsender.Send(payload);
                    log("GCM response new version = " + strResponse, false);

                    break;

                case "EXECUTE":

                    log("EXECUTE event triggered (" + _name + "), parameter_1=" + parameter_1 + ", parameter_2=" + parameter_2, false);
                    log("address = " + _gcmid, false);

                    category = "#taskertask#";
                    level = "5";
                    osaid = "-1"; //eventually we will use this to keep track on osa side to handle removing notification once it is seen on one of the devices with the same owner, but for now it is not used

                    payload = "\"type\" : \"taskertask\" \"message\" : \"" + parameter_1 + "\" \"category\" : \"" + category + "\" \"level\" : \"" + level + "\" \"osaid\" : \"" + osaid + "\" \"messagedate\" : \"" + messagedate + "\" ";
                    
                    strResponse = gcmsender.Send(payload);
                    log("GCM response new version = " + strResponse, false);

                    break;
            }

        }


    }
}
