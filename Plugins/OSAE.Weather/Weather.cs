using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading;
using System.Timers;
using System.Xml;


namespace OSAE.WeatherPlugin
{
    public class WeatherPlugin : OSAEPluginBase
    {
        Thread updateThread;
        Thread SunriseSunsetThread;
        string zipcode = "";
        OSAE osae = new OSAE("Weather");
        int updateInterval = 60;
        System.Timers.Timer Clock, Clock2;
        string feedUrl = "";
        string pName;


        public override void ProcessCommand(OSAEMethod method)
        {
            //This plugin does not process commands
            if (method.MethodName == "UPDATE")
                update();
        }

        public override void RunInterface(string pluginName)
        {
            pName = pluginName;
            List<OSAEObject> objects = osae.GetObjectsByType("WEATHER");
            if (objects.Count == 0)
                osae.ObjectAdd("Weather Data", "Weather Data", "WEATHER", "", "SYSTEM",true);
            if (!Directory.Exists(osae.APIpath + "/Images/Weather"))
            {
                DirectoryInfo di = Directory.CreateDirectory(osae.APIpath + "/Images/Weather");
            }

            osae.AddToLog("Running Interface!", true);
            Clock = new System.Timers.Timer();
            Clock.Interval = Int32.Parse(osae.GetObjectPropertyValue(pName, "Update Interval").Value) * 60000;
            Clock.Start();
            Clock.Elapsed += new ElapsedEventHandler(Timer_Tick);

            Clock2 = new System.Timers.Timer();
            Clock2.Interval = 24 * 60 * 60000;
            Clock2.Start();
            Clock2.Elapsed += new ElapsedEventHandler(Timer_Tick);

            this.SunriseSunsetThread = new Thread(new ThreadStart(SunriseSunset));
            this.SunriseSunsetThread.Start();

            this.updateThread = new Thread(new ThreadStart(update));
            this.updateThread.Start();
        }

        public override void Shutdown()
        {
            Clock.Stop();
            Clock2.Stop();
        }

        public void Timer_Tick(object sender, EventArgs eArgs)
        {
            if (sender == Clock)
            {
                if (!updateThread.IsAlive)
                {
                    this.updateThread = new Thread(new ThreadStart(update));
                    this.updateThread.Start();
                }
            }
            else if(sender == Clock2)
            {
                if (!updateThread.IsAlive)
                {
                    this.SunriseSunsetThread = new Thread(new ThreadStart(SunriseSunset));
                    this.SunriseSunsetThread.Start();
                }
            }

        }

        public void update()
        {
            osae.AddToLog("Starting Update", false);
            try
            {
                string zipcode = osae.GetObjectPropertyValue(pName, "Zipcode").Value;
                osae.AddToLog("ZipCode: " + zipcode, false);
                if (zipcode != "")
                {
                    List<ObservationStation> stationList = new List<ObservationStation>();
                    PointF pt = GetLatLonFromZip(zipcode);
                    ObservationStation myStation = new ObservationStation();
                    feedUrl = osae.GetObjectPropertyValue(pName, "Feed URL").Value;
                    osae.AddToLog("pt.X: " + pt.X.ToString(), false);
                    osae.AddToLog("pt.Y: " + pt.Y.ToString(), false);
                    if (pt.X != 0 || pt.Y != 0)
                    {
                        osae.AddToLog("Enter Update", false);
                        osae.AddToLog("feedUrl: " + feedUrl, false);

                        if (feedUrl == "")
                        {

                            WebClient webClient = new WebClient();
                            string strSource = webClient.DownloadString("http://www.weather.gov/xml/current_obs/index.xml");
                            webClient.Dispose();

                            XmlDocument xml = new XmlDocument();
                            xml.LoadXml(strSource);

                            XmlNodeList xnList = xml.SelectNodes("/wx_station_index/station");
                            foreach (XmlNode xn in xnList)
                            {
                                ObservationStation obs = new ObservationStation(xn);
                                stationList.Add(obs);
                            }

                            osae.AddToLog("lat: " + pt.X.ToString(), false);
                            osae.AddToLog("lon: " + pt.Y.ToString(), false);
                            osae.AddToLog("# of stations: " + stationList.Count.ToString(), false);
                            myStation = GetClosest((decimal)pt.Y, (decimal)pt.X, stationList);
                            feedUrl = myStation.CurrentObsXmlUrl;
                            osae.AddToLog("Found feed: " + feedUrl, false);
                        }

                        if (feedUrl != "")
                        {
                            #region current conditions
                            WebClient webClient = new WebClient();
                            string strSource = webClient.DownloadString(feedUrl);
                            webClient.Dispose();

                            XmlDocument xml = new XmlDocument();
                            xml.LoadXml(strSource);
                            //osae.AddToLog(strSource);

                            //update all the weather variables
                            try
                            {
                                osae.ObjectPropertySet("Weather Data", "Temp", xml.SelectSingleNode("//temp_f").InnerText);
                                osae.AddToLog("Found Temp: " + xml.SelectSingleNode("//temp_f").InnerText, false);
                            }
                            catch 
                            { 
                                osae.AddToLog("Error getting Temp", false);
                                osae.ObjectPropertySet("Weather Data", "Temp", "");
                            }

                            try
                            {
                                osae.ObjectPropertySet("Weather Data", "Conditions", xml.SelectSingleNode("//weather").InnerText);
                                osae.AddToLog("Found Conditions: " + xml.SelectSingleNode("//weather").InnerText, false);
                            }
                            catch 
                            { 
                                osae.AddToLog("Error getting Conditions", false);
                                osae.ObjectPropertySet("Weather Data", "Conditions", "");
                            }

                            try
                            {
                                osae.ObjectPropertySet("Weather Data", "Wind Speed", xml.SelectSingleNode("//wind_mph").InnerText);
                                osae.AddToLog("Found Wind Speed: " + xml.SelectSingleNode("//wind_mph").InnerText, false);
                            }
                            catch 
                            { 
                                osae.AddToLog("Error getting Wind Speed", false);
                                osae.ObjectPropertySet("Weather Data", "Wind Speed", "");
                            }

                            try
                            {
                                osae.ObjectPropertySet("Weather Data", "Wind Direction", xml.SelectSingleNode("//wind_dir").InnerText);
                                osae.AddToLog("Found Wind Direction: " + xml.SelectSingleNode("//wind_dir").InnerText, false);
                            }
                            catch 
                            { 
                                osae.AddToLog("Error getting Wind Direction", false);
                                osae.ObjectPropertySet("Weather Data", "Wind Direction", "");
                            }

                            try
                            {
                                osae.ObjectPropertySet("Weather Data", "Humidity", xml.SelectSingleNode("//relative_humidity").InnerText);
                                osae.AddToLog("Found Humidity: " + xml.SelectSingleNode("//relative_humidity").InnerText, false);
                            }
                            catch 
                            { 
                                osae.AddToLog("Error getting Humidity", false);
                                osae.ObjectPropertySet("Weather Data", "Humidity", "");
                            }

                            try
                            {
                                osae.ObjectPropertySet("Weather Data", "Pressure", xml.SelectSingleNode("//pressure_in").InnerText);
                                osae.AddToLog("Found Pressure: " + xml.SelectSingleNode("//pressure_in").InnerText, false);
                            }
                            catch 
                            { 
                                osae.AddToLog("Error getting Pressure", false);
                                osae.ObjectPropertySet("Weather Data", "Pressure", "");
                            }

                            try
                            {
                                osae.ObjectPropertySet("Weather Data", "Dewpoint", xml.SelectSingleNode("//dewpoint_f").InnerText);
                                osae.AddToLog("Found Dewpoint: " + xml.SelectSingleNode("//dewpoint_f").InnerText, false);
                            }
                            catch 
                            {
                                osae.AddToLog("Error getting Dewpoint", false);
                                osae.ObjectPropertySet("Weather Data", "Dewpoint", "");
                            }

                            try
                            {
                                string curpath = @"\Images\Weather\" + xml.SelectSingleNode("//icon_url_name").InnerText;
                                if (!File.Exists(osae.APIpath + @"\Images\Weather\" + xml.SelectSingleNode("//icon_url_name").InnerText))
                                    {
                                        DownloadImage di = new DownloadImage(xml.SelectSingleNode("//icon_url_base").InnerText + xml.SelectSingleNode("//icon_url_name").InnerText);
                                        
                                        osae.AddToLog("Saving image: " + curpath, false);
                                        di.Download();
                                        di.SaveImage(osae.APIpath + curpath, ImageFormat.Jpeg);
                                    }

                                osae.ObjectPropertySet("Weather Data", "Image", curpath);
                                osae.AddToLog("Found Image: " + curpath, false);
                            }
                            catch 
                            { 
                                osae.AddToLog("Error getting Image", false);
                                osae.ObjectPropertySet("Weather Data", "Image", "");
                            }

                            try
                            {
                                osae.ObjectPropertySet("Weather Data", "Visibility", xml.SelectSingleNode("//visibility_mi").InnerText);
                                osae.AddToLog("Found Visibility: " + xml.SelectSingleNode("//visibility_mi").InnerText, false);
                            }
                            catch 
                            {
                                osae.AddToLog("Error getting Visibility", false);
                                osae.ObjectPropertySet("Weather Data", "Visibility", "");
                            }

                            try
                            {
                                osae.ObjectPropertySet("Weather Data", "Windchill", xml.SelectSingleNode("//windchill_f").InnerText);
                                osae.AddToLog("Found Windchill: " + xml.SelectSingleNode("//windchill_f").InnerText, false);
                            }
                            catch 
                            {
                                osae.AddToLog("Error getting Windchill", false);
                                osae.ObjectPropertySet("Weather Data", "Windchill", "");
                            }

                            #endregion 
                        }
                        
                        #region Forecasts
                        try
                        {
                            WebClient webClientForcast = new WebClient();
                            string strSourceForcast = webClientForcast.DownloadString("http://forecast.weather.gov/MapClick.php?lat=" + pt.X.ToString() + "&lon=" + pt.Y.ToString() + "&FcstType=dwml");
                            webClientForcast.Dispose();
                            osae.AddToLog("Forecast XML: " + "http://forecast.weather.gov/MapClick.php?lat=" + pt.X.ToString() + "&lon=" + pt.Y.ToString() + "&FcstType=dwml", false);

                            XmlDocument xmlForcast = new XmlDocument();
                            xmlForcast.LoadXml(strSourceForcast);

                            XmlNode temp = null;
                            XmlNodeList tempList = xmlForcast.SelectNodes("//time-layout");
                            foreach (XmlNode xn in tempList)
                            {
                                osae.AddToLog("nodes: " + xn.ChildNodes.Count.ToString(), false);
                                if (xn.ChildNodes.Count > 10)
                                {
                                    temp = xn;
                                    break;
                                }
                            }
                            
                            int day = 2;
                            int today = 0;
                            try
                            {
                                
                                foreach (XmlNode xn in temp.ChildNodes)
                                {

                                    if (xn.Name == "start-valid-time")
                                    {
                                        if (xn.Attributes["period-name"].Value == "Today" || xn.Attributes["period-name"].Value == "This Afternoon")
                                            today = 1;
                                        if (xn.Attributes["period-name"].Value != "Today" && xn.Attributes["period-name"].Value != "Tonight" && xn.Attributes["period-name"].Value != "Overnight")
                                        {
                                            if (day % 2 == 0)
                                            {
                                                osae.ObjectPropertySet("Weather Data", "Day" + (day / 2).ToString() + " Label", xn.Attributes["period-name"].Value);
                                                osae.AddToLog("Day" + (day / 2).ToString() + " Label: " + xn.Attributes["period-name"].Value, false);
                                            }
                                            else
                                            {
                                                osae.ObjectPropertySet("Weather Data", "Night" + (day / 2).ToString() + " Label", xn.Attributes["period-name"].Value);
                                                osae.AddToLog("Night" + (day / 2).ToString() + " Label: " + xn.Attributes["period-name"].Value, false);

                                            }
                                            day++;
                                        }
                                    }


                                }
                            }
                            catch { }

                            try
                            {
                                temp = xmlForcast.SelectSingleNode("//temperature[@type='maximum']");
                                day = 1;
                                foreach (XmlNode xn in temp.ChildNodes)
                                {

                                    if (xn.Name == "value")
                                    {
                                        if (today == 1 && day == 1)
                                        {
                                            osae.ObjectPropertySet("Weather Data", "Today High", xn.InnerText);
                                            osae.AddToLog("Found Today High: " + xn.InnerText, false);
                                        }
                                        else
                                        {
                                            osae.ObjectPropertySet("Weather Data", "Day" + (day - today).ToString() + " High", xn.InnerText);
                                            osae.AddToLog("Found Day" + (day - today).ToString() + " High: " + xn.InnerText, false);
                                        }
                                        day++;
                                    }

                                }
                            }
                            catch { }

                            try
                            {
                                temp = xmlForcast.SelectSingleNode("//temperature[@type='minimum']");
                                day = 1;
                                foreach (XmlNode xn in temp.ChildNodes)
                                {

                                    if (xn.Name == "value")
                                    {
                                        if (day == 1)
                                        {
                                            osae.ObjectPropertySet("Weather Data", "Tonight Low", xn.InnerText);
                                            osae.AddToLog("Found Tonight Low: " + xn.InnerText, false);
                                        }
                                        else
                                        {
                                            osae.ObjectPropertySet("Weather Data", "Night" + (day - 1).ToString() + " Low", xn.InnerText);
                                            osae.AddToLog("Found Night" + (day - 1).ToString() + " Low: " + xn.InnerText, false);
                                        }
                                        day++;
                                    }

                                }
                            }
                            catch { }

                            try
                            {
                                temp = xmlForcast.SelectSingleNode("//probability-of-precipitation");
                                day = 2;
                                foreach (XmlNode xn in temp.ChildNodes)
                                {

                                    if (xn.Name == "value")
                                    {

                                        if (day % 2 == today)
                                        {
                                            if ((today == 0 && day == 2) || (today == 1 && day == 3))
                                            {
                                                osae.ObjectPropertySet("Weather Data", "Tonight Precip", xn.InnerText);
                                                osae.AddToLog("Tonight Precip: " + xn.InnerText, false);
                                            }
                                            else
                                            {
                                                osae.ObjectPropertySet("Weather Data", "Night" + ((day / 2) - 1).ToString() + " Precip", xn.InnerText);
                                                osae.AddToLog("Night" + ((day / 2) - 1).ToString() + " Precip: " + xn.InnerText, false);
                                            }
                                        }
                                        else
                                        {
                                            if (today == 1 && day == 2)
                                            {
                                                osae.ObjectPropertySet("Weather Data", "Today Precip", xn.InnerText);
                                                osae.AddToLog("Today Precip: " + xn.InnerText, false);
                                            }
                                            else
                                            {
                                                osae.ObjectPropertySet("Weather Data", "Day" + ((day / 2)-today).ToString() + " Precip", xn.InnerText);
                                                osae.AddToLog("Day" + ((day / 2)-today).ToString() + " Precip: " + xn.InnerText, false);
                                            }
                                        }
                                        day++;

                                    }


                                }
                            }
                            catch { }

                            try
                            {
                                temp = xmlForcast.SelectSingleNode("//weather");
                                day = 2;
                                foreach (XmlNode xn in temp.ChildNodes)
                                {

                                    if (xn.Name == "weather-conditions")
                                    {
                                        if (day % 2 == today)
                                        {

                                            if ((today == 0 && day == 2) || (today == 1 && day == 3))
                                            {
                                                osae.ObjectPropertySet("Weather Data", "Tonight Forecast", xn.Attributes["weather-summary"].Value);
                                                osae.AddToLog("Tonight Forecast: " + xn.Attributes["weather-summary"].Value, false);
                                            }
                                            else
                                            {
                                                osae.ObjectPropertySet("Weather Data", "Night" + ((day / 2) - 1).ToString() + " Forecast", xn.Attributes["weather-summary"].Value);
                                                osae.AddToLog("Night" + ((day / 2) - 1).ToString() + " Forecast: " + xn.Attributes["weather-summary"].Value, false);
                                            }
                                        }
                                        else
                                        {
                                            if (today == 1 && day == 2)
                                            {
                                                osae.ObjectPropertySet("Weather Data", "Today Forecast", xn.Attributes["weather-summary"].Value);
                                                osae.AddToLog("Today Forecast: " + xn.Attributes["weather-summary"].Value, false);
                                            }
                                            else
                                            {
                                                osae.ObjectPropertySet("Weather Data", "Day" + ((day / 2)-today).ToString() + " Forecast", xn.Attributes["weather-summary"].Value);
                                                osae.AddToLog("Day" + ((day / 2)-today).ToString() + " Forecast: " + xn.Attributes["weather-summary"].Value, false);
                                            }
                                        }
                                        day++;
                                    }


                                }
                            }
                            catch { }

                            try
                            {
                                temp = xmlForcast.SelectSingleNode("//conditions-icon");
                                string path = "";
                                day = 2;
                                foreach (XmlNode xn in temp.ChildNodes)
                                {

                                    if (xn.Name == "icon-link")
                                    {
                                        path = @"\Images\Weather\" + xn.SelectSingleNode("//icon_url_name").InnerText;
                                        if (!File.Exists(osae.APIpath + @"\Images\Weather\" + xn.SelectSingleNode("//icon_url_name").InnerText))
                                        {

                                            DownloadImage di = new DownloadImage(xn.InnerText);

                                            osae.AddToLog("Saving image: " + path, false);
                                            di.Download();
                                            di.SaveImage(osae.APIpath + path, ImageFormat.Jpeg);
                                        }
                                        if (day % 2 == today)
                                        {
                                            if ((today == 0 && day == 2) || (today == 1 && day == 3))
                                            {

                                                osae.ObjectPropertySet("Weather Data", "Tonight Image", path);
                                                osae.AddToLog("Tonight Image: " + xn.InnerText, false);
                                            }
                                            else
                                            {
                                                osae.ObjectPropertySet("Weather Data", "Night" + ((day / 2) - 1).ToString() + " Image", path);
                                                osae.AddToLog("Night" + ((day / 2) - 1).ToString() + " Image: " + xn.InnerText, false);
                                            }
                                        }
                                        else
                                        {
                                            if (today == 1 && day == 2)
                                            {
                                                osae.ObjectPropertySet("Weather Data", "Today Image", path);
                                                osae.AddToLog("Today Image: " + xn.InnerText, false);
                                            }
                                            else
                                            {
                                                osae.ObjectPropertySet("Weather Data", "Day" + ((day / 2)-today).ToString() + " Image", path);
                                                osae.AddToLog("Day" + ((day / 2)-today).ToString() + " Image: " + xn.InnerText, false);
                                            }
                                        }
                                        day++;
                                    }


                                }
                            }
                            catch {  }

                            try
                            {
                                temp = xmlForcast.SelectSingleNode("//wordedForecast");
                                day = 2;
                                foreach (XmlNode xn in temp.ChildNodes)
                                {

                                    if (xn.Name == "text")
                                    {

                                        if (day % 2 == today)
                                        {
                                            if ((today == 0 && day == 2) || (today == 1 && day == 3))
                                            {
                                                osae.ObjectPropertySet("Weather Data", "Tonight Summary", xn.InnerText);
                                                osae.AddToLog("Tonight Summary: " + xn.InnerText, false);
                                            }
                                            else
                                            {
                                                osae.ObjectPropertySet("Weather Data", "Night" + ((day / 2) - 1).ToString() + " Summary", xn.InnerText);
                                                osae.AddToLog("Night" + ((day / 2) - 1).ToString() + " Summary: " + xn.InnerText, false);
                                            }

                                        }
                                        else
                                        {
                                            if (today == 1 && day == 2)
                                            {
                                                osae.ObjectPropertySet("Weather Data", "Today Summary", xn.InnerText);
                                                osae.AddToLog("Today Summary: " + xn.InnerText, false);
                                            }
                                            else
                                            {
                                                osae.ObjectPropertySet("Weather Data", "Day" + ((day / 2)-today).ToString() + " Summary", xn.InnerText);
                                                osae.AddToLog("Day" + ((day / 2)-today).ToString() + " Summary: " + xn.InnerText, false);
                                            }
                                        }
                                        day++;

                                    }






                                }
                            }
                            catch { }
                        }
                        catch (Exception ex)
                        {
                            osae.AddToLog("Error Forecast XML: " + ex.Message, true);
                        }
                        #endregion


                        osae.ObjectStateSet("Weather Data", "ON");
                        osae.ObjectPropertySet("Weather Data", "Last Updated", DateTime.Now.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                osae.AddToLog("Error updating weather - " + ex.Message, true);
            }
        }

        public void SunriseSunset()
        {
            try
            {
                osae.AddToLog("Starting sunrise and sunset update", false);
                PointF pt = GetLatLonFromZip(osae.GetObjectPropertyValue(pName, "Zipcode").Value);

                WebClient webClientTimezone = new WebClient();
                string strSourceTimezone = webClientTimezone.DownloadString("http://www.earthtools.org/timezone/" + pt.X.ToString() + "/" + pt.Y.ToString());
                webClientTimezone.Dispose();
                XmlDocument xmlTimezone = new XmlDocument();
                xmlTimezone.LoadXml(strSourceTimezone);
                string timeZone = xmlTimezone.SelectSingleNode("//offset").InnerText;
                string dst = xmlTimezone.SelectSingleNode("//dst").InnerText;
                string date = xmlTimezone.SelectSingleNode("//isotime").InnerText;
                string[] tmp = date.Split(' ');
                tmp = tmp[0].Split('-');
                string day = tmp[2];
                string month = tmp[1];
                if (dst == "True")
                    dst = "1";
                else
                    dst = "0";
                osae.AddToLog("http://www.earthtools.org/sun/" + pt.X.ToString() + "/" + pt.Y.ToString() + "/" + day + "/" + month + "/" + timeZone + "/" + dst, false);



                WebClient webClientSunriseSunset = new WebClient();
                string strSourceSunriseSunset = webClientSunriseSunset.DownloadString("http://www.earthtools.org/sun/" + pt.X.ToString() + "/" + pt.Y.ToString() + "/" + day + "/" + month + "/" + timeZone + "/" + dst);
                webClientSunriseSunset.Dispose();
                XmlDocument xmlSunriseSunset = new XmlDocument();
                xmlSunriseSunset.LoadXml(strSourceSunriseSunset);
                osae.ObjectPropertySet("Weather Data", "Sunrise", xmlSunriseSunset.SelectSingleNode("//sunrise").InnerText);
                osae.AddToLog("Found sunrise: " + Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + xmlSunriseSunset.SelectSingleNode("//sunrise").InnerText).ToString(), false);
                osae.ObjectPropertySet("Weather Data", "Sunset", xmlSunriseSunset.SelectSingleNode("//sunset").InnerText);
                osae.AddToLog("Found sunset: " + Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + xmlSunriseSunset.SelectSingleNode("//sunset").InnerText).ToString(), false);

                //osae.ScheduleQueueAdd(Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + xmlSunriseSunset.SelectSingleNode("//sunrise").InnerText), "", "", "", "", "Sunrise", 0);
                //osae.ScheduleQueueAdd(Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + xmlSunriseSunset.SelectSingleNode("//sunset").InnerText), "", "", "", "", "Sunset", 0);
            }
            catch (Exception ex)
            {
                osae.AddToLog("Error getting sunrise/sunset: " + ex.Message, true);
            }
        }

        /// <summary>
        /// A brute force great-circle computation to find the closest NWS observation
        /// station.
        /// </summary>
        /// <param name="longitude">longitude for evaluation</param>
        /// <param name="latitude">latitude for evaluation</param>
        /// <param name="distance">distance to closest station</param>
        /// <returns>closest observation station</returns>
        public ObservationStation GetClosest(decimal longitude, decimal latitude, List<ObservationStation> stations)
        {
            osae.AddToLog("Getting closest", true);
            ObservationStation closestStation = null;
            double minDistance = 10000;

            if (stations != null)
            {
                foreach (ObservationStation station in stations)
                {
                    double distance = GetDistance((double)latitude, (double)longitude, (double)station.Latitude, (double)station.Longitude);
                    if (distance < minDistance)
                    {
                        closestStation = station;
                        minDistance = distance;
                        osae.AddToLog("closer distance: " + distance.ToString(), true);
                        osae.AddToLog("closer station: " + closestStation.StationName, true);
                    }
                }
            }

            return closestStation;
        }

        /// <summary>
        /// The radius of the earth, used for computing the distance from
        /// a specific latitude and longitude to a observation station.
        /// </summary>
        private const double EarthRadius = 6378.16; // in km

        /// <summary>
        /// A quick and dirty great-circle distance computation using the
        /// spherical law of cosines. You can find a nice description here:
        /// http://www.movable-type.co.uk/scripts/latlong.html
        /// </summary>
        /// <param name="lat1">point1 latitude</param>
        /// <param name="lon1">point1 longitude</param>
        /// <param name="lat2">point2 latitude</param>
        /// <param name="lon2">point2 longitude</param>
        /// <returns>distance in km.</returns>
        private double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double radLat1 = ToRadians(lat1);
            double radLon1 = ToRadians(lon1);
            double radLat2 = ToRadians(lat2);
            double radLon2 = ToRadians(lon2);
            return Math.Acos((Math.Sin(radLat1) * Math.Sin(radLat2)) +
                             (Math.Cos(radLat1) * Math.Cos(radLat2) *
                              Math.Cos(radLon2 - radLon1))) * EarthRadius;
        }

        /// <summary>
        /// Convert an angle in (decimal) degrees to radians.
        /// </summary>
        /// <param name="degrees">angle in decimal degrees</param>
        /// <returns>angle in radians</returns>
        private double ToRadians(double degrees)
        {
            return (degrees * Math.PI) / 180;
        }


        private static PointF GetLatLonFromZip(string zipString)
        {
            OSAE osae = new OSAE("Weather");
            if (string.IsNullOrEmpty(zipString))
            {
                throw new ArgumentNullException("zipString");
            }

            PointF latLon = new PointF();
            Weather.gov.weather.www.ndfdXML weatherService = new global::OSAE.Weather.gov.weather.www.ndfdXML();
            try
            {
                string latLongXml = weatherService.LatLonListZipCode(zipString);
                XmlDocument latLongDoc = new XmlDocument();
                latLongDoc.LoadXml(latLongXml);
                string latLongList = latLongDoc.SelectSingleNode("//latLonList").InnerText;
                // split latLon string and assign them to their respective variables
                string[] latLongArray = latLongList.Split(',');
                float lat = float.Parse(latLongArray[0]);
                float lon = float.Parse(latLongArray[1]);

                latLon.X = lat;
                latLon.Y = lon;
            }
            catch(Exception ex)
            {
                osae.AddToLog("Error getting Lat/Long: " + ex.Message, false);
                latLon.X = 0;
                latLon.Y = 0;
            }
            return latLon;
        }

    }

    public class DownloadImage
    {
        private string imageUrl;
        private Bitmap bitmap;

        public DownloadImage(string imageUrl)
        {
            this.imageUrl = imageUrl;
        }
        public void Download()
        {
            try
            {
                WebClient client = new WebClient();
                Stream stream = client.OpenRead(imageUrl);
                bitmap = new Bitmap(stream);
                stream.Flush();
                stream.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public Bitmap GetImage()
        {
            return bitmap;
        }
        public void SaveImage(string filename, ImageFormat format)
        {
            if (bitmap != null)
            {
                bitmap.Save(filename, format);
            }
        }
    }
}
