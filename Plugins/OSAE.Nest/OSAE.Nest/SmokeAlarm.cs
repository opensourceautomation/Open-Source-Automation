using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OSAE.Nest
{
    public class SmokeAlarm
    {
        public string device_id;
        public string locale;
        public string software_version;
        public string structure_id;
        public string name;
        public string name_long;
        public string last_connection;
        public bool is_online;
        public string battery_health;
        public string co_alarm_state;
        public string smoke_alarm_state;
        public bool is_manual_test_active;
        public string last_manual_test_time;
        public string ui_color_state;

        public void convertLastConnection()
        {

            //for testing 
            //last_connection = " 2015-02-02T20:43:16.772Z";

            DateTime _lastConnection;

            if (!DateTime.TryParse(last_connection, out _lastConnection))
            {
                last_connection = "";
            }
            else
            {
                //last_connection = _lastConnection.ToLocalTime().ToString("G"); // 1/29/2015 8:43:16 PM 
                last_connection = _lastConnection.ToString("G"); 
            }
        }

        public void loadData(string objectName)
        {

            device_id = OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Id").Value;
            name = OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Name").Value;
            name_long = OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Name Long").Value;
            battery_health = OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Battery Health").Value;
            co_alarm_state = OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "CO Alarm State").Value;
            smoke_alarm_state = OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Smoke Alarm State").Value;
            last_manual_test_time = OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Last Manual Test").Value;
            ui_color_state = OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "UI Color State").Value;
            locale = OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Locale").Value;
            software_version = OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Software Version").Value;
            structure_id = OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Structure ID").Value;
            

            //convert _lastConnection to standard format for comparison
            DateTime _lastConnection;
            last_connection = OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Last Connection").Value;

            if (!DateTime.TryParse(last_connection, out _lastConnection))
            {
                last_connection = "";
            }
            else
            {
                last_connection = _lastConnection.ToString("G"); // 1/29/2015 8:43:16 PM 
            }

            Boolean _is_manual_test_active;
            Boolean _is_online;

            Boolean.TryParse(OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Manual Test Active").Value, out _is_manual_test_active);
            is_manual_test_active = _is_manual_test_active;
            Boolean.TryParse(OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Is Online").Value, out _is_online);
            is_online = _is_online;

        }
    }

}
