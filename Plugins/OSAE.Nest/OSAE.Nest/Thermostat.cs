using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OSAE.Nest
{
    public class Thermostat
    {
        public string device_id;
        public string locale ;
        public string software_version ;
        public string structure_id ;
        public string name ;
        public string name_long ;
        public string last_connection ;
        public bool is_online ;
        public bool can_cool ;
        public bool can_heat ;
        public bool is_using_emergency_heat ;
        public bool has_fan ;
        public bool fan_timer_active ;
        public string fan_timer_timeout ;
        public bool has_leaf ;
        public string temperature_scale ;
        public float target_temperature_f ;
        public float target_temperature_c ;
        public float target_temperature_high_f ;
        public float target_temperature_high_c ;
        public float target_temperature_low_f ;
        public float target_temperature_low_c ;
        public float away_temperature_high_f ;
        public float away_temperature_high_c ;
        public float away_temperature_low_f ;
        public float away_temperature_low_c ;
        public string hvac_mode ;
        public float ambient_temperature_f ;
        public float ambient_temperature_c ;
        public float humidity ;

        public void tempToScale() //for comparing new data to old data since we don't save both temps to an osa object
        {
            if (temperature_scale.ToLower().Equals("f"))
            {
                target_temperature_c = target_temperature_f;
                target_temperature_high_c = target_temperature_high_f;

                target_temperature_low_c = target_temperature_low_f;
                away_temperature_high_c = away_temperature_high_f;
                away_temperature_low_c = away_temperature_low_f;
                ambient_temperature_c = ambient_temperature_f;
            }
            else
            {
                target_temperature_f = target_temperature_c;
                target_temperature_high_f = target_temperature_high_c;
                target_temperature_low_f = target_temperature_low_c;
                away_temperature_high_f = away_temperature_high_c;
                away_temperature_low_f = away_temperature_low_c;
                ambient_temperature_f = ambient_temperature_c;
            }
        }

        public void convertLastConnection(){
            
            //for testing 
            //last_connection = "2015-02-10T20:43:16.772Z";

            DateTime _lastConnection;

            if (!DateTime.TryParse(last_connection, out _lastConnection))
            {
                last_connection = "";
            }
            else
            {
                //last_connection = _lastConnection.ToLocalTime().ToString("G"); 
                last_connection = _lastConnection.ToString("G"); 
            }
        }

        public void loadData(string objectName)
        {
                                    
            hvac_mode = OSAEObjectStateManager.GetObjectStateValue(objectName).Value.ToLower();

            device_id = OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Id").Value;
            name = OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Name").Value;
            name_long = OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Name Long").Value;
            temperature_scale = OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Temperature Scale").Value;
            locale = OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Locale").Value;
            software_version = OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Software Version").Value;
            structure_id = OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Structure ID").Value;
            fan_timer_timeout = OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Fan Timer Timeout").Value;

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

            Boolean _has_leaf;
            Boolean _is_online;
            Boolean _can_cool;
            Boolean _can_heat;
            Boolean _has_fan;
            Boolean _is_using_emergency_heat;
            Boolean _fan_timer_active;

            Boolean.TryParse(OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Has Leaf").Value, out _has_leaf);
            has_leaf = _has_leaf;
            Boolean.TryParse(OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Is Online").Value, out _is_online);
            is_online = _is_online;
            Boolean.TryParse(OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Can Cool").Value, out _can_cool);
            can_cool = _can_cool;
            Boolean.TryParse(OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Can Heat").Value, out _can_heat);
            can_heat = _can_heat;
            Boolean.TryParse(OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Has Fan").Value, out _has_fan);
            has_fan = _has_fan;
            Boolean.TryParse(OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Using Emergency Heat").Value, out _is_using_emergency_heat);
            is_using_emergency_heat = _is_using_emergency_heat;
            Boolean.TryParse(OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Fan Timer Active").Value, out _fan_timer_active);
            fan_timer_active = _fan_timer_active;


            float _ambient_temperature;
            float _humidity;
            float _target_temperature;
            float _target_temperature_high;
            float _target_temperature_low;
            float _away_temperature_high;
            float _away_temperature_low;

            float.TryParse(OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Humidity").Value, out _humidity);
            humidity = _humidity;
            float.TryParse(OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Ambient Temperature").Value, out _ambient_temperature);
            ambient_temperature_f = _ambient_temperature;
            ambient_temperature_c = _ambient_temperature;
            float.TryParse(OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Target Temperature").Value, out _target_temperature);
            target_temperature_f = _target_temperature;
            target_temperature_c = _target_temperature;
            float.TryParse(OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Target Temperature High").Value, out _target_temperature_high);
            target_temperature_high_f = _target_temperature_high;
            target_temperature_high_c = _target_temperature_high;
            float.TryParse(OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Target Temperature Low").Value, out _target_temperature_low);
            target_temperature_low_f = _target_temperature_low;
            target_temperature_low_c = _target_temperature_low;
            float.TryParse(OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Away Temperature High").Value, out _away_temperature_high);
            away_temperature_high_f = _away_temperature_high;
            away_temperature_high_c = _away_temperature_high;
            float.TryParse(OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Away Temperature Low").Value, out _away_temperature_low);
            away_temperature_low_f = _away_temperature_low;
            away_temperature_low_c = _away_temperature_low;

        }
    }

}
