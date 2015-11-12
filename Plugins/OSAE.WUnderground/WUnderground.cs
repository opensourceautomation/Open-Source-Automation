namespace OSAE.WUnderground
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading;
    using System.Timers;
    using System.Xml;

    public class WUnderground : OSAEPluginBase
    {
        string pName;
        //OSAELog
        private OSAE.General.OSAELog Log = new General.OSAELog();
       // Logging logging = Logging.GetLogger("WUnderground");

        Thread updateConditionsThread, updateForecastThread, updateDayNightThread;
        string pws = "";
        System.Timers.Timer ConditionsUpdateTimer, ForecastUpdateTimer, DayNightUpdateTimer;
        //string feedUrl = "";
        //string ForecastUrl;
        string latitude ="", longitude="";
        int Conditionsupdatetime, Forecastupdatetime, DayNightupdatetime = 60000;
        Boolean FirstUpdateRun;
        Boolean FirstForcastRun;
        String DayNight, WeatherObjName;
        Boolean Metric;
        
        public override void RunInterface(string pluginName)
        {
            try
            {
                FirstUpdateRun = true;
                FirstForcastRun = true;
                Log.Info("Running Interface");
                pName = pluginName;

                OSAEObjectCollection objects = OSAEObjectManager.GetObjectsByType("WEATHER");
                if (objects.Count == 0)
                {
                    OSAEObjectManager.ObjectAdd("Weather", "Weather", "Weather Data", "WEATHER", "", "", true);
                    WeatherObjName = "Weather";
                }
                else
                    WeatherObjName = objects[0].Name;

                Log.Info("Linked to Weather object to store data.");
                try
                {
                    if (Boolean.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Metric").Value))
                    {
                        Metric = true;
                        this.Log.Info("Using metric units");
                    }
                }
                catch {}

                Conditionsupdatetime = Int32.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Conditions Interval").Value);
                if (Conditionsupdatetime > 0)
                {
                    ConditionsUpdateTimer = new System.Timers.Timer();
                    ConditionsUpdateTimer.Interval = Conditionsupdatetime * 60000;
                    ConditionsUpdateTimer.Start();
                    ConditionsUpdateTimer.Elapsed += new ElapsedEventHandler(ConditionsUpdateTime);

                    this.updateConditionsThread = new Thread(new ThreadStart(updateconditions));
                    this.updateConditionsThread.Start();

                  //  Thread.Sleep(10000);
                }
                else
                {
                    latitude = OSAEObjectPropertyManager.GetObjectPropertyValue(WeatherObjName, "latitude").Value;
                    longitude = OSAEObjectPropertyManager.GetObjectPropertyValue(WeatherObjName, "longitude").Value;
                    Log.Debug("Read in properties: Lat=" + latitude + ", Long=" + longitude);
                }

                do
                {
                    Thread.Sleep(5000);
                } while (FirstUpdateRun);

                Forecastupdatetime = Int32.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Forecast Interval").Value);
                if (Forecastupdatetime > 0)
                {
                    ForecastUpdateTimer = new System.Timers.Timer();
                    ForecastUpdateTimer.Interval = Forecastupdatetime * 60000;
                    ForecastUpdateTimer.Start();
                    ForecastUpdateTimer.Elapsed += new ElapsedEventHandler(ForecastUpdateTime);

                    this.updateForecastThread = new Thread(new ThreadStart(updateforecast));
                    this.updateForecastThread.Start();             
                }

                do
                {
                    Thread.Sleep(5000);
                } while (FirstForcastRun);

                this.Log.Info("Updated " + WeatherObjName + ", setting Weather object to Updated.");
                OSAE.OSAEMethodManager.MethodQueueAdd(WeatherObjName, "Updated", "", "", pName);

                DayNightUpdateTimer = new System.Timers.Timer();
                DayNightUpdateTimer.Interval = DayNightupdatetime;
                DayNightUpdateTimer.Start();
                DayNightUpdateTimer.Elapsed += new ElapsedEventHandler(DayNightUpdateTime);

                this.updateDayNightThread = new Thread(new ThreadStart(updateDayNight));
                this.updateDayNightThread.Start();             
            }
            catch (Exception ex)
            {
                this.Log.Error("Error initializing the plugin ", ex);
            }
         }
                
        public override void ProcessCommand(OSAEMethod method)
        {
            if (method.MethodName == "UPDATE")
                update();
        }
        
        public override void Shutdown()
        {
            this.Log.Info("Shutting down");
            if (Forecastupdatetime > 0)
                ForecastUpdateTimer.Stop();

            if (Conditionsupdatetime > 0)
                ConditionsUpdateTimer.Stop();

            DayNightUpdateTimer.Stop();
        }
       
        public void ConditionsUpdateTime(object sender, EventArgs eArgs)
        {
            if (sender == ConditionsUpdateTimer)
            {
                if (!updateConditionsThread.IsAlive)
                {
                    this.updateConditionsThread = new Thread(new ThreadStart(updateconditions));
                    this.updateConditionsThread.Start();
                }
            }
        }

        public void ForecastUpdateTime(object sender, EventArgs eArgs)
        {
            if (sender == ForecastUpdateTimer)
            {
                if (!updateForecastThread.IsAlive)
                {
                    this.updateForecastThread = new Thread(new ThreadStart(updateforecast));
                    this.updateForecastThread.Start();
                }
            }
        }

        public void DayNightUpdateTime(object sender, EventArgs eArgs)
        {
            if (sender == DayNightUpdateTimer)
            {
                if (!updateDayNightThread.IsAlive)
                {
                    this.updateDayNightThread = new Thread(new ThreadStart(updateDayNight));
                    this.updateDayNightThread.Start();
                }
            }
        }

        private string GetNodeValue(XmlDocument xml, string xPathQuery)
        {
            string nodeValue = string.Empty;
            System.Xml.XmlNode node;
            node = xml.SelectSingleNode(xPathQuery);
            if (node != null) nodeValue = node.InnerText;
            return nodeValue;
        }

        private void ReportFieldValue(string fieldName, string fieldValue)
        {
            if (fieldValue.Length > 0)
            {
                OSAEObjectPropertyManager.ObjectPropertySet(WeatherObjName, fieldName, fieldValue, pName);
                if (fieldName == "Temp")
                    this.Log.Debug("Found " + fieldName + ": " + fieldValue);
                else
                    this.Log.Debug("Found " + fieldName + ": " + fieldValue);
            }
            else
            {
                if (fieldName != "Windchill" & fieldName != "Visibility" & fieldName != "Conditions")
                    this.Log.Debug("NOT FOUND " + fieldName);
            }
        }

        private void GetFieldFromXmlAndReport(XmlDocument xml, string fieldName, string xPathQuery)
        {
            string fieldValue = GetNodeValue(xml, xPathQuery);
            ReportFieldValue(fieldName, fieldValue);
        }

        public void update()
        {
            if (Conditionsupdatetime > 0) updateconditions();
            if (Forecastupdatetime > 0) updateforecast();
            this.Log.Info("Updated " + WeatherObjName + ", setting Weather object to Updated.");
            OSAE.OSAEMethodManager.MethodQueueAdd(WeatherObjName, "Updated", "", "", pName);
        }

        public void updateconditions()
        {
            string feedUrl;
            string sXml;
            WebClient webClient = new WebClient();
            XmlDocument xml;
            Log.Debug("***  Reading Conditions  ***");
            try
            {
                pws = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "PWS").Value;
                if (pws != "")
                {
                    feedUrl = "http://api.wunderground.com/weatherstation/WXCurrentObXML.asp?ID=" + pws;
                    Log.Debug("Reading Feed: " + feedUrl);
                    sXml = webClient.DownloadString(feedUrl);
                    xml = new XmlDocument();
                    xml.LoadXml(sXml);
                    //update all the weather variables

                    #region Current Observation
                    // Seems to be returned from both pws and airport.
                    GetFieldFromXmlAndReport(xml, "Wind Speed", "current_observation/wind_mph");
                    GetFieldFromXmlAndReport(xml, "Wind Directions", "current_observation/wind_dir");
                    GetFieldFromXmlAndReport(xml, "Humidity", "current_observation/relative_humidity");
                    GetFieldFromXmlAndReport(xml, "Image", "current_observation/image/url");
                    GetFieldFromXmlAndReport(xml, "Last Updated", "current_observation/observation_time");
                    
                    if (!Metric)
                    {
                        GetFieldFromXmlAndReport(xml, "Temp", "current_observation/temp_f");
                        GetFieldFromXmlAndReport(xml, "Pressure", "current_observation/pressure_in");
                        GetFieldFromXmlAndReport(xml, "Dewpoint", "current_observation/dewpoint_f");
                        GetFieldFromXmlAndReport(xml, "Windchill", "current_observation/windchill_f");
                    }
                    else
                    {
                        GetFieldFromXmlAndReport(xml, "Temp", "current_observation/temp_c"); 
                        GetFieldFromXmlAndReport(xml, "Pressure", "current_observation/pressure_mb");
                        GetFieldFromXmlAndReport(xml, "Dewpoint", "current_observation/dewpoint_c");
                        GetFieldFromXmlAndReport(xml, "Windchill", "current_observation/windchill_c");
                    }
                    
                    // Only returned for airports.
                    GetFieldFromXmlAndReport(xml, "Visibility", "current_observation/visibility_mi");
                    GetFieldFromXmlAndReport(xml, "Conditions", "current_observation/weather");

                    if (FirstUpdateRun)
                    {
                        latitude = GetNodeValue(xml,"current_observation/location/latitude");
                        longitude = GetNodeValue(xml, "current_observation/location/longitude");
                        OSAEObjectPropertyManager.ObjectPropertySet(WeatherObjName, "latitude", latitude, pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(WeatherObjName, "longitude", longitude, pName);
                    }
                    //ForecastUrl = GetNodeValue(xml, "current_observation/ob_url");
                    #endregion
                }                                    
            }
            catch (Exception ex)
            {
                this.Log.Error("Error updating current weather - ", ex);
            }
            FirstUpdateRun = false;
        }

        public void updateforecast()
        {
            string feedUrl;
            string sXml;
            WebClient webClient = new WebClient();
            XmlDocument xml;

            try
            {
                Log.Info("***  Reading Forecast  ***");
                if (latitude != "" && longitude != "")
                {
                   // Now get the forecast.
                    feedUrl = "http://api.wunderground.com/auto/wui/geo/ForecastXML/index.xml?query=" + latitude + "," + longitude;
                    sXml = webClient.DownloadString(feedUrl);
                    xml = new XmlDocument();
                    xml.LoadXml(sXml);

                    ReportFieldValue("Sunrise", GetNodeValue(xml, "forecast/moon_phase/sunrise/hour") + ":" + GetNodeValue(xml, "forecast/moon_phase/sunrise/minute"));
                    ReportFieldValue("Sunset", GetNodeValue(xml, "forecast/moon_phase/sunset/hour") + ":" + GetNodeValue(xml, "forecast/moon_phase/sunset/minute"));

                    // NOTE:
                    // Need to grab a few different forecasts at various times of day to see how wunderground
                    // manages the daynumber.  Can't find any specification that lays it out clearly.
                    // This is the closest to a specification found:
                    // http://wiki.wunderground.com/index.php/API_-_XML
                    // 


                    GetFieldFromXmlAndReport(xml, "Today Precip", @"forecast/simpleforecast/forecastday[period=1]/pop");
                    GetFieldFromXmlAndReport(xml, "Today Forecast", @"forecast/simpleforecast/forecastday[period=1]/conditions");
                    GetFieldFromXmlAndReport(xml, "Today Image", @"forecast/simpleforecast/forecastday[period=1]/icons/icon_set[@name='Contemporary']/icon_url");
                    GetFieldFromXmlAndReport(xml, "Today Summary", @"forecast/txt_forecast/forecastday[period=1]/fcttext");

                    //GetFieldFromXmlAndReport(xml, "Tonight Precip", @"forecast/");
                    //GetFieldFromXmlAndReport(xml, "Tonight Forecast", @"forecast/");
                    //GetFieldFromXmlAndReport(xml, "Tonight Image", @"forecast/");
                    GetFieldFromXmlAndReport(xml, "Tonight Summary", @"forecast/txt_forecast/forecastday[period=1]/fcttext");


                    #region Period1
                    GetFieldFromXmlAndReport(xml, "Day1 Precip", @"forecast/simpleforecast/forecastday[period=2]/pop");
                    GetFieldFromXmlAndReport(xml, "Day1 Forecast", @"forecast/simpleforecast/forecastday[period=2]/conditions");
                    GetFieldFromXmlAndReport(xml, "Day1 Summary", @"forecast/txt_forecast/forecastday[period=2]/fcttext");
                    GetFieldFromXmlAndReport(xml, "Day1 Label", @"forecast/simpleforecast/forecastday[period=2]/conditions");
                    GetFieldFromXmlAndReport(xml, "Day1 Image", @"forecast/simpleforecast/forecastday[period=2]/icons/icon_set[@name='Contemporary']/icon_url");

                    if (!Metric)
                    {
                        GetFieldFromXmlAndReport(xml, "Day1 High", @"forecast/simpleforecast/forecastday[period=2]/high/fahrenheit");
                        GetFieldFromXmlAndReport(xml, "Night1 Low", @"forecast/simpleforecast/forecastday[period=2]/low/fahrenheit");
                    }
                    else
                    {
                        GetFieldFromXmlAndReport(xml, "Day1 High", @"forecast/simpleforecast/forecastday[period=2]/high/celsius");
                        GetFieldFromXmlAndReport(xml, "Night1 Low", @"forecast/simpleforecast/forecastday[period=2]/low/celsius");
                    }
                    

                    //GetFieldFromXmlAndReport(xml, "Night1 Precip", @"forecast/simpleforecast/forecastday[period=2]/");
                    //GetFieldFromXmlAndReport(xml, "Night1 Forecast", @"forecast/simpleforecast/forecastday[period=2]/");
                    //GetFieldFromXmlAndReport(xml, "Night1 Summary", @"forecast/simpleforecast/forecastday[period=2]/");
                    //GetFieldFromXmlAndReport(xml, "Night1 Label", @"forecast/simpleforecast/forecastday[period=2]/");
                    //GetFieldFromXmlAndReport(xml, "Night1 Image", @"forecast/simpleforecast/forecastday[period=2]/");
                    #endregion

                    #region Period2
                    GetFieldFromXmlAndReport(xml, "Day2 Precip", @"forecast/simpleforecast/forecastday[period=3]/pop");
                    GetFieldFromXmlAndReport(xml, "Day2 Forecast", @"forecast/simpleforecast/forecastday[period=3]/conditions");
                    GetFieldFromXmlAndReport(xml, "Day2 Summary", @"forecast/txt_forecast/forecastday[period=3]/fcttext");
                    GetFieldFromXmlAndReport(xml, "Day2 Label", @"forecast/simpleforecast/forecastday[period=3]/conditions");
                    GetFieldFromXmlAndReport(xml, "Day2 Image", @"forecast/simpleforecast/forecastday[period=3]/icons/icon_set[@name='Contemporary']/icon_url");

                    if (!Metric)
                    {
                        GetFieldFromXmlAndReport(xml, "Day2 High", @"forecast/simpleforecast/forecastday[period=3]/high/fahrenheit");
                        GetFieldFromXmlAndReport(xml, "Night2 Low", @"forecast/simpleforecast/forecastday[period=3]/low/fahrenheit");
                    }
                    else
                    {
                        GetFieldFromXmlAndReport(xml, "Day2 High", @"forecast/simpleforecast/forecastday[period=3]/high/celsius");
                        GetFieldFromXmlAndReport(xml, "Night2 Low", @"forecast/simpleforecast/forecastday[period=3]/low/celsius");
                    }

                    //GetFieldFromXmlAndReport(xml, "Night2 Precip", @"forecast/simpleforecast/forecastday[period=3]/");
                    //GetFieldFromXmlAndReport(xml, "Night2 Forecast", @"forecast/simpleforecast/forecastday[period=3]/");
                    //GetFieldFromXmlAndReport(xml, "Night2 Summary", @"forecast/simpleforecast/forecastday[period=3]/");
                    //GetFieldFromXmlAndReport(xml, "Night2 Label", @"forecast/simpleforecast/forecastday[period=3]/");
                    //GetFieldFromXmlAndReport(xml, "Night2 Image", @"forecast/simpleforecast/forecastday[period=3]/");
                    #endregion

                    #region Period3
                    GetFieldFromXmlAndReport(xml, "Day3 Precip", @"forecast/simpleforecast/forecastday[period=4]/pop");
                    GetFieldFromXmlAndReport(xml, "Day3 Forecast", @"forecast/simpleforecast/forecastday[period=4]/conditions");
                    GetFieldFromXmlAndReport(xml, "Day3 Summary", @"forecast/txt_forecast/forecastday[period=4]/fcttext");
                    GetFieldFromXmlAndReport(xml, "Day3 Label", @"forecast/simpleforecast/forecastday[period=4]/conditions");
                    GetFieldFromXmlAndReport(xml, "Day3 Image", @"forecast/simpleforecast/forecastday[period=4]/icons/icon_set[@name='Contemporary']/icon_url");

                    if (!Metric)
                    {
                        GetFieldFromXmlAndReport(xml, "Day3 High", @"forecast/simpleforecast/forecastday[period=4]/high/fahrenheit");
                        GetFieldFromXmlAndReport(xml, "Night3 Low", @"forecast/simpleforecast/forecastday[period=4]/low/fahrenheit");
                    }
                    else
                    {
                        GetFieldFromXmlAndReport(xml, "Day3 High", @"forecast/simpleforecast/forecastday[period=4]/high/celsius");
                        GetFieldFromXmlAndReport(xml, "Night3 Low", @"forecast/simpleforecast/forecastday[period=4]/low/celsius");
                    }


                    //GetFieldFromXmlAndReport(xml, "Night3 Precip", @"forecast/simpleforecast/forecastday[period=4]/");
                    //GetFieldFromXmlAndReport(xml, "Night3 Forecast", @"forecast/simpleforecast/forecastday[period=4]/");
                    //GetFieldFromXmlAndReport(xml, "Night3 Summary", @"forecast/simpleforecast/forecastday[period=4]/");
                    //GetFieldFromXmlAndReport(xml, "Night3 Label", @"forecast/simpleforecast/forecastday[period=4]/");
                    //GetFieldFromXmlAndReport(xml, "Night3 Image", @"forecast/simpleforecast/forecastday[period=4]/");
                    #endregion

                    #region Period4
                    GetFieldFromXmlAndReport(xml, "Day4 Precip", @"forecast/simpleforecast/forecastday[period=5]/pop");
                    GetFieldFromXmlAndReport(xml, "Day4 Forecast", @"forecast/simpleforecast/forecastday[period=5]/conditions");
                    GetFieldFromXmlAndReport(xml, "Day4 Summary", @"forecast/txt_forecast/forecastday[period=5]/fcttext");
                    GetFieldFromXmlAndReport(xml, "Day4 Label", @"forecast/simpleforecast/forecastday[period=5]/conditions");
                    GetFieldFromXmlAndReport(xml, "Day4 Image", @"forecast/simpleforecast/forecastday[period=5]/icons/icon_set[@name='Contemporary']/icon_url");

                    if (!Metric)
                    {
                        GetFieldFromXmlAndReport(xml, "Day4 High", @"forecast/simpleforecast/forecastday[period=5]/high/fahrenheit");
                        GetFieldFromXmlAndReport(xml, "Night4 Low", @"forecast/simpleforecast/forecastday[period=5]/low/fahrenheit");
                    }
                    else
                    {
                        GetFieldFromXmlAndReport(xml, "Day4 High", @"forecast/simpleforecast/forecastday[period=5]/high/celsius");
                        GetFieldFromXmlAndReport(xml, "Night4 Low", @"forecast/simpleforecast/forecastday[period=5]/low/celsius");
                    }


                    //GetFieldFromXmlAndReport(xml, "Night4 Precip", @"forecast/simpleforecast/forecastday[period=5]/");
                    //GetFieldFromXmlAndReport(xml, "Night4 Forecast", @"forecast/simpleforecast/forecastday[period=5]/");
                    //GetFieldFromXmlAndReport(xml, "Night4 Summary", @"forecast/simpleforecast/forecastday[period=5]/");
                    //GetFieldFromXmlAndReport(xml, "Night4 Label", @"forecast/simpleforecast/forecastday[period=5]/");
                    //GetFieldFromXmlAndReport(xml, "Night4 Image", @"forecast/simpleforecast/forecastday[period=5]/");
                    #endregion

                    #region Period5
                    GetFieldFromXmlAndReport(xml, "Day5 Precip", @"forecast/simpleforecast/forecastday[period=6]/pop");
                    GetFieldFromXmlAndReport(xml, "Day5 Forecast", @"forecast/simpleforecast/forecastday[period=6]/conditions");
                    //GetFieldFromXmlAndReport(xml, "Day5 Summary", @"forecast/txt_forecast/forecastday[period=6]/fcttext");
                    GetFieldFromXmlAndReport(xml, "Day5 Label", @"forecast/simpleforecast/forecastday[period=6]/conditions");
                    GetFieldFromXmlAndReport(xml, "Day5 Image", @"forecast/simpleforecast/forecastday[period=6]/icons/icon_set[@name='Contemporary']/icon_url");

                    if (!Metric)
                    {
                        GetFieldFromXmlAndReport(xml, "Day5 High", @"forecast/simpleforecast/forecastday[period=6]/high/fahrenheit");
                        GetFieldFromXmlAndReport(xml, "Nigh5 Low", @"forecast/simpleforecast/forecastday[period=6]/low/fahrenheit");
                    }
                    else
                    {
                        GetFieldFromXmlAndReport(xml, "Day5 High", @"forecast/simpleforecast/forecastday[period=6]/high/celsius");
                        GetFieldFromXmlAndReport(xml, "Night5 Low", @"forecast/simpleforecast/forecastday[period=6]/low/celsius");
                    }

                    //GetFieldFromXmlAndReport(xml, "Night5 Precip", @"forecast/simpleforecast/forecastday[period=6]/");
                    //GetFieldFromXmlAndReport(xml, "Night5 Forecast", @"forecast/simpleforecast/forecastday[period=6]/");
                    //GetFieldFromXmlAndReport(xml, "Night5 Summary", @"forecast/simpleforecast/forecastday[period=6]/");
                    //GetFieldFromXmlAndReport(xml, "Night5 Label", @"forecast/simpleforecast/forecastday[period=6]/");
                    //GetFieldFromXmlAndReport(xml, "Night5 Image", @"forecast/simpleforecast/forecastday[period=6]/");
                    #endregion

                    #region Period6
                    /*  WUNDERGROUND doesn't go this far.
                    GetFieldFromXmlAndReport(xml, "Day6 High", @"forecast/simpleforecast/forecastday[period=2]/high/fahrenheit");
                    //GetFieldFromXmlAndReport(xml, "Day6 Precip", @"forecast/simpleforecast/forecastday[period=2]/");
                    GetFieldFromXmlAndReport(xml, "Day6 Forecast", @"forecast/simpleforecast/forecastday[period=2]/conditions");
                    GetFieldFromXmlAndReport(xml, "Day6 Summary", @"forecast/simpleforecast/forecastday[period=2]/conditions");
                    GetFieldFromXmlAndReport(xml, "Day6 Label", @"forecast/simpleforecast/forecastday[period=2]/conditions");
                    GetFieldFromXmlAndReport(xml, "Day6 Image", @"forecast/simpleforecast/forecastday[period=2]/icons/icon_set[@name='Contemporary']/icon_url");

                    GetFieldFromXmlAndReport(xml, "Night6 Low", @"forecast/simpleforecast/forecastday[period=2]/low/fahrenheit");
                    //GetFieldFromXmlAndReport(xml, "Night6 Precip", @"forecast/simpleforecast/forecastday[period=2]/");
                    //GetFieldFromXmlAndReport(xml, "Night6 Forecast", @"forecast/simpleforecast/forecastday[period=2]/");
                    //GetFieldFromXmlAndReport(xml, "Night6 Summary", @"forecast/simpleforecast/forecastday[period=2]/");
                    //GetFieldFromXmlAndReport(xml, "Night6 Label", @"forecast/simpleforecast/forecastday[period=2]/");
                    //GetFieldFromXmlAndReport(xml, "Night6 Image", @"forecast/simpleforecast/forecastday[period=2]/");
                     * */
                    #endregion

                    #region Period7
                    /*  WUNDERGROUND doesn't go this far.
                    GetFieldFromXmlAndReport(xml, "Day7 High", @"forecast/simpleforecast/forecastday[period=2]/high/fahrenheit");
                    //GetFieldFromXmlAndReport(xml, "Day7 Precip", @"forecast/simpleforecast/forecastday[period=2]/");
                    GetFieldFromXmlAndReport(xml, "Day7 Forecast", @"forecast/simpleforecast/forecastday[period=2]/conditions");
                    GetFieldFromXmlAndReport(xml, "Day7 Summary", @"forecast/simpleforecast/forecastday[period=2]/conditions");
                    GetFieldFromXmlAndReport(xml, "Day7 Label", @"forecast/simpleforecast/forecastday[period=2]/conditions");
                    GetFieldFromXmlAndReport(xml, "Day7 Image", @"forecast/simpleforecast/forecastday[period=2]/icons/icon_set[@name='Contemporary']/icon_url");

                    GetFieldFromXmlAndReport(xml, "Night7 Low", @"forecast/simpleforecast/forecastday[period=2]/low/fahrenheit");
                    //GetFieldFromXmlAndReport(xml, "Night7 Precip", @"forecast/simpleforecast/forecastday[period=2]/");
                    //GetFieldFromXmlAndReport(xml, "Night7 Forecast", @"forecast/simpleforecast/forecastday[period=2]/");
                    //GetFieldFromXmlAndReport(xml, "Night7 Summary", @"forecast/simpleforecast/forecastday[period=2]/");
                    //GetFieldFromXmlAndReport(xml, "Night7 Label", @"forecast/simpleforecast/forecastday[period=2]/");
                    //GetFieldFromXmlAndReport(xml, "Night7 Image", @"forecast/simpleforecast/forecastday[period=2]/");
                     * */
                    #endregion
                }
            }
            catch (Exception ex)
            { this.Log.Error("Error updating forecasted weather - " + ex.Message); }
            FirstForcastRun = false;
        }

        public void updateDayNight()
        {
            TimeSpan Now;
            TimeSpan DuskStart;
            TimeSpan DuskEnd;
            TimeSpan DawnStart;
            TimeSpan DawnEnd;
            TimeSpan Sunrise;
            TimeSpan Sunset;
            String DawnPreString;
            String DawnPostString;
            String DuskPreString;
            String DuskPostString;
            Int32 DuskPre;
            Int32 DuskPost;
            Int32 DawnPre;
            Int32 DawnPost;
            Int32 Number;

            Log.Info("***  Reading Day/Night  ***");
            try
            {
                    Now = DateTime.Now.TimeOfDay;
                    Sunrise = DateTime.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(WeatherObjName, "Sunrise").Value).TimeOfDay;
                    Sunset = DateTime.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(WeatherObjName, "Sunset").Value).TimeOfDay;
                    try
                    {
                    DawnPreString = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "DawnPre").Value;
                    if (Int32.TryParse(DawnPreString, out Number))
                        DawnPre = Number;
                    else
                        DawnPre = 0;

                    DawnPostString = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "DawnPost").Value;
                    if (Int32.TryParse(DawnPostString, out Number))
                        DawnPost = Number;
                    else
                        DawnPost = 0;

                    DuskPreString = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "DuskPre").Value;
                    if (Int32.TryParse(DuskPreString, out Number))
                        DuskPre = Number;
                    else
                        DuskPre = 0;

                    DuskPostString = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "DuskPost").Value;
                    if (Int32.TryParse(DuskPostString, out Number))
                        DuskPost = Number;
                    else
                        DuskPost = 0;

                    DawnStart = Sunrise - TimeSpan.FromMinutes(DawnPre);
                    DawnEnd = Sunrise + TimeSpan.FromMinutes(DawnPost);

                    DuskStart = Sunset - TimeSpan.FromMinutes(DuskPre);
                    DuskEnd = Sunset + TimeSpan.FromMinutes(DuskPost);

                String John = " " + " " + Convert.ToString(DuskStart);

                this.Log.Debug(Convert.ToString(DawnStart) + " " + Convert.ToString(DawnEnd) + " " + Convert.ToString(DuskStart) + " " + Convert.ToString(DuskEnd) + " ");
                }
                catch (Exception ex)
                {
                    this.Log.Error("Error setting times in updating day/night ", ex);
                    DawnStart = Sunrise;
                    DawnEnd = Sunrise;

                    DuskStart = Sunset;
                    DuskEnd = Sunset;

                }


                if (Now >= DawnEnd & Now < DuskStart)
                {
                    if (DayNight != "Day")
                    {
                        OSAEObjectPropertyManager.ObjectPropertySet(WeatherObjName, "DayNight", "Day", pName);
                        //if (DayNight == "Night" | DayNight == "Dawn")
                       // {
                        //    logging.EventLogAdd(WeatherObjName, "Day");
                        //}
                        DayNight = "Day";
                        this.Log.Debug("Day");
                        
                    }
                }
                else if (Now >= DuskEnd | Now < DawnStart)
                {
                    if (DayNight != "Night")
                    {
                        OSAEObjectPropertyManager.ObjectPropertySet(WeatherObjName, "DayNight", "Night",pName);
                      //  if (DayNight == "Day" | DayNight == "Dusk")
                       // {
                       //     logging.EventLogAdd(WeatherObjName, "Night");
                       // }
                        DayNight = "Night";
                         this.Log.Info("Night");
                    }
                }
                else if (Now >= DawnStart & Now < DawnEnd)
                {
                    if (DayNight != "Dawn")
                    {
                        OSAEObjectPropertyManager.ObjectPropertySet(WeatherObjName, "DayNight", "Dawn", pName);
                     //   if (DayNight == "Night")
                     //   {
                     //       logging.EventLogAdd(WeatherObjName, "Dawn");
                      //  }
                        DayNight = "Dawn";
                        this.Log.Info("Dawn");
                    }
                }

                else if (Now >= DuskStart & Now < DuskEnd)
                {
                    if (DayNight != "Dusk")
                    {
                        OSAEObjectPropertyManager.ObjectPropertySet(WeatherObjName, "DayNight", "Dusk", pName);
                     //   if (DayNight == "Day")
                      //  {
                      //      logging.EventLogAdd(WeatherObjName, "Dusk");
                      //  }
                        DayNight = "Dusk";
                        this.Log.Info("Dusk");
                    }
                }
            }
            catch (Exception ex)
            {
                this.Log.Error("Error updating day/night ",ex);
            }

        }

        public void OwnTypes()
        {
            //Added the follow to automatically own WUnderground Base types that have no owner.
            OSAEObjectType oType = OSAEObjectTypeManager.ObjectTypeLoad("WUNDERGROUND");

            if (oType.OwnedBy == "")
            {
                OSAEObjectTypeManager.ObjectTypeUpdate(oType.Name, oType.Name, oType.Description, pName, oType.BaseType, oType.Owner, oType.SysType, oType.Container, oType.HideRedundant);
                Log.Info("WUnderground Plugin took ownership of the WUNDERGROUND Object Type.");
            }
            else
            {
                Log.Info("WUnderground Plugin correctly owns the WUNDERGROUND Object Type.");
            }

            //Added the follow for SYSTEM to automatically own Weather Base types that have no owner.
            oType = OSAEObjectTypeManager.ObjectTypeLoad("WUNDERGROUND");

            if (oType.OwnedBy == "")
            {
                OSAEObjectTypeManager.ObjectTypeUpdate(oType.Name, oType.Name, oType.Description, "SYSTEM", oType.BaseType, oType.Owner, oType.SysType, oType.Container, oType.HideRedundant);
                Log.Info("SYSTEM took ownership of the WEATHER Object Type.");
            }
            else
            {
                Log.Info("SYSTEM Plugin correctly owns the WEATHER Object Type.");
            }
        }
    }
}