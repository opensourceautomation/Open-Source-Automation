using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OSAE.Nest
{
    public class Structure
    {
        public string name ;
        public string country_code ;
        public string postal_code ;
        public string time_zone ;
        public string away ;
        public string structure_id ;
        public string peak_period_start_time ;
        public string peak_period_end_time ;

        public void loadData(string objectName)
        {
            away = OSAEObjectStateManager.GetObjectStateValue(objectName).Value.ToLower();
            structure_id=OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Id").Value;
            name = OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Name").Value;
            country_code = OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Country Code").Value;
            postal_code = OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Postal Code").Value;
            peak_period_start_time = OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Peak Start Time").Value;
            peak_period_end_time = OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Peak End Time").Value;
            time_zone = OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Time Zone").Value;
        }
    }
}
