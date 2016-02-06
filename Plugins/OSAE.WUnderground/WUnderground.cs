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
        bool gDebug = false;
        private OSAE.General.OSAELog Log;// = new General.OSAELog();
        Thread updateConditionsThread, updateForecastThread, updateDayNightThread;
        System.Timers.Timer ConditionsUpdateTimer, ForecastUpdateTimer, DayNightUpdateTimer;
        string latitude ="", longitude="";
        int Conditionsupdatetime, Forecastupdatetime, DayNightupdatetime = 300000;
        bool FirstUpdateRun, FirstForcastRun, Metric;
        string DayNight, WeatherObjName, pKey, pCity, pState;

        public override void RunInterface(string pluginName)
        {
            try
            {
                pName = pluginName;
                Log = new General.OSAELog(pName);
                FirstUpdateRun = true;
                FirstForcastRun = true;
                Log.Info("Running Interface");
                OwnTypes();

                OSAEObjectCollection objects = OSAEObjectManager.GetObjectsByType("WEATHER");
                if (objects.Count == 0)
                {
                    OSAEObjectManager.ObjectAdd("Weather", "Weather", "Weather Data", "WEATHER", "", "", 30, true);
                    WeatherObjName = "Weather";
                }
                else
                    WeatherObjName = objects[0].Name;

                Log.Info("Linked to Weather object to store data.");
                try
                {
                    if (bool.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Metric").Value))
                    {
                        Metric = true;
                        Log.Info("Using metric units");
                    }
                }
                catch {}

                try
                {
                    gDebug = Convert.ToBoolean(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Debug").Value);
                }
                catch
                { Log.Info("The WUnderground Object Type seems to be missing the Debug Property!"); }
                Log.Info("Debug Mode Set to " + gDebug);

                try
                {
                    pKey = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Key").Value;
                    if (pKey.Length < 1)
                        Log.Info("!!! You need an WUnderground Key for full weather feeds !!!");
                    else
                        Log.Info("Found WUnderground Key (" + pKey + ")");
                }
                catch (Exception ex)
                { Log.Error("Error reading your Key.", ex); }

                try
                {
                    pCity = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "City").Value;
                    if (pCity.Length < 1)
                        Log.Info("!!! You need a City for full weather feeds !!!");
                    else
                        Log.Info("Found WUnderground City (" + pCity + ")");
                }
                catch (Exception ex)
                { Log.Error("Error reading your City.", ex); }

                try
                {
                    pState = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "State").Value;
                    if (pState.Length < 1)
                        Log.Info("!!! You need a State for full weather feeds !!!");
                    else
                        Log.Info("Found State (" + pState + ")");
                }
                catch (Exception ex)
                { Log.Error("Error reading your State.", ex); }


                Conditionsupdatetime = Int32.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Conditions Interval").Value);
                if (Conditionsupdatetime > 0)
                {
                    ConditionsUpdateTimer = new System.Timers.Timer();
                    ConditionsUpdateTimer.Interval = Conditionsupdatetime * 60000;
                    ConditionsUpdateTimer.Start();
                    ConditionsUpdateTimer.Elapsed += new ElapsedEventHandler(ConditionsUpdateTime);

                    updateConditionsThread = new Thread(new ThreadStart(updateconditions));
                    updateConditionsThread.Start();

                  //  Thread.Sleep(10000);
                }
                else
                {
                    latitude = OSAEObjectPropertyManager.GetObjectPropertyValue(WeatherObjName, "latitude").Value;
                    longitude = OSAEObjectPropertyManager.GetObjectPropertyValue(WeatherObjName, "longitude").Value;
                    if (gDebug) Log.Debug("Read in properties: Lat=" + latitude + ", Long=" + longitude);
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

                    updateForecastThread = new Thread(new ThreadStart(updateforecast));
                    updateForecastThread.Start();             
                }

                do
                {
                    Thread.Sleep(5000);
                } while (FirstForcastRun);

                Log.Info("Updated " + WeatherObjName + ", setting Weather object to Updated.");
                OSAE.OSAEMethodManager.MethodQueueAdd(WeatherObjName, "Updated", "", "", pName);

                DayNightUpdateTimer = new System.Timers.Timer();
                DayNightUpdateTimer.Interval = DayNightupdatetime;
                DayNightUpdateTimer.Start();
                DayNightUpdateTimer.Elapsed += new ElapsedEventHandler(DayNightUpdateTime);

                updateDayNightThread = new Thread(new ThreadStart(updateDayNight));
                updateDayNightThread.Start();             
            }
            catch (Exception ex)
            { Log.Error("Error initializing the plugin ", ex); }
         }

        public override void ProcessCommand(OSAEMethod method)
        {
            if (method.MethodName == "UPDATE") update();
        }
        
        public override void Shutdown()
        {
            Log.Info("Shutting down");
            if (Forecastupdatetime > 0) ForecastUpdateTimer.Stop();
            if (Conditionsupdatetime > 0) ConditionsUpdateTimer.Stop();

            DayNightUpdateTimer.Stop();
        }
       
        public void ConditionsUpdateTime(object sender, EventArgs eArgs)
        {
            if (sender == ConditionsUpdateTimer)
            {
                if (!updateConditionsThread.IsAlive)
                {
                    updateConditionsThread = new Thread(new ThreadStart(updateconditions));
                    updateConditionsThread.Start();
                }
            }
        }

        public void ForecastUpdateTime(object sender, EventArgs eArgs)
        {
            if (sender == ForecastUpdateTimer)
            {
                if (!updateForecastThread.IsAlive)
                {
                    updateForecastThread = new Thread(new ThreadStart(updateforecast));
                    updateForecastThread.Start();
                }
            }
        }

        public void DayNightUpdateTime(object sender, EventArgs eArgs)
        {
            if (sender == DayNightUpdateTimer)
            {
                if (!updateDayNightThread.IsAlive)
                {
                    updateDayNightThread = new Thread(new ThreadStart(updateDayNight));
                    updateDayNightThread.Start();
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
                    if (gDebug) Log.Debug("...Found " + fieldName + ": " + fieldValue);
                else
                    if (gDebug) Log.Debug("...Found " + fieldName + ": " + fieldValue);
            }
            else
            {
                if (fieldName != "Windchill" & fieldName != "Visibility" & fieldName != "Conditions")
                    if (gDebug) Log.Debug("NOT FOUND " + fieldName);
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
            Log.Info("Updated " + WeatherObjName + ", setting Weather object to Updated.");
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
                    feedUrl = "http://api.wunderground.com/api/" + pKey + "/conditions/q/" + pState + "/" + pCity + ".xml";
                    if (gDebug) Log.Debug("Reading Feed: " + feedUrl);
                    sXml = webClient.DownloadString(feedUrl);
                    xml = new XmlDocument();
                    xml.LoadXml(sXml);
                    //update all the weather variables

                    #region Current Observation
                    GetFieldFromXmlAndReport(xml, "Wind Speed", "response/current_observation/wind_mph");
                    GetFieldFromXmlAndReport(xml, "Wind Directions", "response/current_observation/wind_dir");
                    GetFieldFromXmlAndReport(xml, "Humidity", "response/current_observation/relative_humidity");
                    GetFieldFromXmlAndReport(xml, "Image", "response/current_observation/image/url");
                    GetFieldFromXmlAndReport(xml, "Last Updated", "response/current_observation/observation_time");
                    
                    if (!Metric)
                    {
                        GetFieldFromXmlAndReport(xml, "Temp", "response/current_observation/temp_f");
                        GetFieldFromXmlAndReport(xml, "Pressure", "response/current_observation/pressure_in");
                        GetFieldFromXmlAndReport(xml, "Dewpoint", "response/current_observation/dewpoint_f");
                        GetFieldFromXmlAndReport(xml, "Windchill", "response/current_observation/windchill_f");
                    }
                    else
                    {
                        GetFieldFromXmlAndReport(xml, "Temp", "response/current_observation/temp_c"); 
                        GetFieldFromXmlAndReport(xml, "Pressure", "response/current_observation/pressure_mb");
                        GetFieldFromXmlAndReport(xml, "Dewpoint", "response/current_observation/dewpoint_c");
                        GetFieldFromXmlAndReport(xml, "Windchill", "response/current_observation/windchill_c");
                    }
                    
                    // Only returned for airports.
                    GetFieldFromXmlAndReport(xml, "Visibility", "response/current_observation/visibility_mi");
                    GetFieldFromXmlAndReport(xml, "Conditions", "response/current_observation/weather");

                    if (FirstUpdateRun)
                    {
                        latitude = GetNodeValue(xml, "response/current_observation/observation_location/latitude");
                        longitude = GetNodeValue(xml, "response/current_observation/observation_location/longitude");
                        OSAEObjectPropertyManager.ObjectPropertySet(WeatherObjName, "latitude", latitude, pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(WeatherObjName, "longitude", longitude, pName);
                    }
                    #endregion
            }
            catch (Exception ex)
            { Log.Error("Error updating current weather - ", ex); }

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
                Log.Info("***  Sunrise/Sunset  ***");
                if (latitude != "" && longitude != "")
                {
                    feedUrl = "http://api.wunderground.com/api/" + pKey + "/astronomy/q/" + pState + "/" + pCity + ".xml";
                    if (gDebug) Log.Debug("Reading Sunset: " + feedUrl);
                    sXml = webClient.DownloadString(feedUrl);
                    xml = new XmlDocument();
                    xml.LoadXml(sXml);

                    ReportFieldValue("Sunrise", GetNodeValue(xml, "response/moon_phase/sunrise/hour") + ":" + GetNodeValue(xml, "response/moon_phase/sunrise/minute"));
                    ReportFieldValue("Sunset", GetNodeValue(xml, "response/moon_phase/sunset/hour") + ":" + GetNodeValue(xml, "response/moon_phase/sunset/minute"));
                }
            }
            catch (Exception ex)
            { Log.Error("Error updating sunset - " + ex.Message); }

            try
            {
                Log.Info("***  Reading Forecast  ***");
                if (latitude != "" && longitude != "")
                {
                    // Now get the forecast.
                    feedUrl = "http://api.wunderground.com/api/d50de2112f55424b/forecast10day/q/" + pState + "/" + pCity + ".xml";
                    if (gDebug) Log.Debug("Reading Forcast: " + feedUrl);
                    sXml = webClient.DownloadString(feedUrl);
                    xml = new XmlDocument();
                    xml.LoadXml(sXml);

                    // NOTE:
                    // New API has Today + 9 day forcast.   In Simple, it only has Daytime, so 4 Periods. text has 2 periods per day (night)
                    // I could be wrong, but I am mapping it as period 0=today,1=tonight,2=tomorrow,3=tomorrow night

                    //We are basing the Period here on Simple. so 1 per day +1 in txt_ will equal the night
                    #region Period1/0-1
                    //(Today & Tonight)
                    GetFieldFromXmlAndReport(xml, "Today Precip", @"response/forecast/txt_forecast/forecastdays/forecastday[period=0]/pop");
                    GetFieldFromXmlAndReport(xml, "Today Forecast", @"response/forecast/simpleforecast/forecastdays/forecastday[period=1]/conditions");
                    GetFieldFromXmlAndReport(xml, "Today Summary", @"response/forecast/txt_forecast/forecastdays/forecastday[period=0]/fcttext");
                    GetFieldFromXmlAndReport(xml, "Today Image", @"response/forecast/txt_forecast/forecastdays/forecastday[period=0]/icon_url");

                    GetFieldFromXmlAndReport(xml, "Tonight Precip", @"response/forecast/txt_forecast/forecastdays/forecastday[period=1]/pop");
                    GetFieldFromXmlAndReport(xml, "Tonight Summary", @"response/forecast/txt_forecast/forecastdays/forecastday[period=1]/fcttext");
                    GetFieldFromXmlAndReport(xml, "Tonight Image", @"response/forecast/txt_forecast/forecastdays/forecastday[period=1]/icon_url");
                    #endregion


                    #region Period2/2-3
                    GetFieldFromXmlAndReport(xml, "Day1 Precip", @"response/forecast/txt_forecast/forecastdays/forecastday[period=2]/pop");
                    GetFieldFromXmlAndReport(xml, "Day1 Forecast", @"response/forecast/simpleforecast/forecastdays/forecastday[period=2]/conditions");
                    GetFieldFromXmlAndReport(xml, "Day1 Summary", @"response/forecast/txt_forecast/forecastdays/forecastday[period=2]/fcttext");
                    GetFieldFromXmlAndReport(xml, "Day1 Image", @"response/forecast/simpleforecast/forecastdays/forecastday[period=2]/icon_url");
                    if (!Metric)
                    {
                        GetFieldFromXmlAndReport(xml, "Day1 High", @"response/forecast/simpleforecast/forecastdays/forecastday[period=2]/high/fahrenheit");
                        GetFieldFromXmlAndReport(xml, "Night1 Low", @"response/forecast/simpleforecast/forecastdays/forecastday[period=2]/low/fahrenheit");
                    }
                    else
                    {
                        GetFieldFromXmlAndReport(xml, "Day1 High", @"response/forecast/simpleforecast/forecastdays/forecastday[period=2]/high/celsius");
                        GetFieldFromXmlAndReport(xml, "Night1 Low", @"response/forecast/simpleforecast/forecastdays/forecastday[period=2]/low/celsius");
                    }
                    GetFieldFromXmlAndReport(xml, "Night1 Precip", @"response/forecast/txt_forecast/forecastdays/forecastday[period=3]/pop");
                    GetFieldFromXmlAndReport(xml, "Night1 Summary", @"response/forecast/txt_forecast/forecastdays/forecastday[period=3]/fcttext");
                    GetFieldFromXmlAndReport(xml, "Night1 Image", @"response/forecast/txt_forecast/forecastdays/forecastday[period=3]/icon_url");
                    #endregion

                    #region Period3/4-5
                    GetFieldFromXmlAndReport(xml, "Day2 Precip", @"response/forecast/txt_forecast/forecastdays/forecastday[period=4]/pop");
                    GetFieldFromXmlAndReport(xml, "Day2 Forecast", @"response/forecast/simpleforecast/forecastdays/forecastday[period=3]/conditions");
                    GetFieldFromXmlAndReport(xml, "Day2 Summary", @"response/forecast/txt_forecast/forecastdays/forecastday[period=4]/fcttext");
                    GetFieldFromXmlAndReport(xml, "Day2 Image", @"response/forecast/simpleforecast/forecastdays/forecastday[period=4]/icon_url");
                    if (!Metric)
                    {
                        GetFieldFromXmlAndReport(xml, "Day2 High", @"response/forecast/simpleforecast/forecastdays/forecastday[period=3]/high/fahrenheit");
                        GetFieldFromXmlAndReport(xml, "Night2 Low", @"response/forecast/simpleforecast/forecastdays/forecastday[period=3]/low/fahrenheit");
                    }
                    else
                    {
                        GetFieldFromXmlAndReport(xml, "Day2 High", @"response/forecast/simpleforecast/forecastdays/forecastday[period=3]/high/celsius");
                        GetFieldFromXmlAndReport(xml, "Night2 Low", @"response/forecast/simpleforecast/forecastdays/forecastday[period=3]/low/celsius");
                    }
                    GetFieldFromXmlAndReport(xml, "Night2 Precip", @"response/forecast/txt_forecast/forecastdays/forecastday[period=5]/pop");
                    GetFieldFromXmlAndReport(xml, "Night2 Summary", @"response/forecast/txt_forecast/forecastdays/forecastday[period=5]/fcttext");
                    GetFieldFromXmlAndReport(xml, "Night2 Image", @"response/forecast/txt_forecast/forecastdays/forecastday[period=5]/icon_url");
                    #endregion


                    #region Period4/6-7
                    GetFieldFromXmlAndReport(xml, "Day3 Precip", @"response/forecast/txt_forecast/forecastdays/forecastday[period=6]/pop");
                    GetFieldFromXmlAndReport(xml, "Day3 Forecast", @"response/forecast/simpleforecast/forecastdays/forecastday[period=4]/conditions");
                    GetFieldFromXmlAndReport(xml, "Day3 Summary", @"response/forecast/txt_forecast/forecastdays/forecastday[period=6]/fcttext");
                    GetFieldFromXmlAndReport(xml, "Day3 Image", @"response/forecast/simpleforecast/forecastdays/forecastday[period=4]/icon_url");
                    if (!Metric)
                    {
                        GetFieldFromXmlAndReport(xml, "Day3 High", @"response/forecast/simpleforecast/forecastdays/forecastday[period=4]/high/fahrenheit");
                        GetFieldFromXmlAndReport(xml, "Night3 Low", @"response/forecast/simpleforecast/forecastdays/forecastday[period=4]/low/fahrenheit");
                    }
                    else
                    {
                        GetFieldFromXmlAndReport(xml, "Day3 High", @"response/forecast/simpleforecast/forecastdays/forecastday[period=4]/high/celsius");
                        GetFieldFromXmlAndReport(xml, "Night3 Low", @"response/forecast/simpleforecast/forecastdays/forecastday[period=4]/low/celsius");
                    }
                    GetFieldFromXmlAndReport(xml, "Night3 Precip", @"response/forecast/txt_forecast/forecastdays/forecastday[period=7]/pop");
                    GetFieldFromXmlAndReport(xml, "Night3 Summary", @"response/forecast/txt_forecast/forecastdays/forecastday[period=7]/fcttext");
                    GetFieldFromXmlAndReport(xml, "Night3 Image", @"response/forecast/txt_forecast/forecastdays/forecastday[period=7]/icon_url");
                    #endregion

                    #region Period5/8-9
                    GetFieldFromXmlAndReport(xml, "Day4 Precip", @"response/forecast/txt_forecast/forecastdays/forecastday[period=8]/pop");
                    GetFieldFromXmlAndReport(xml, "Day4 Forecast", @"response/forecast/simpleforecast/forecastdays/forecastday[period=5]/conditions");
                    GetFieldFromXmlAndReport(xml, "Day4 Summary", @"response/forecast/txt_forecast/forecastdays/forecastday[period=8]/fcttext");
                    GetFieldFromXmlAndReport(xml, "Day4 Image", @"response/forecast/simpleforecast/forecastdays/forecastday[period=5]/icon_url");
                    if (!Metric)
                    {
                        GetFieldFromXmlAndReport(xml, "Day4 High", @"response/forecast/simpleforecast/forecastdays/forecastday[period=5]/high/fahrenheit");
                        GetFieldFromXmlAndReport(xml, "Night4 Low", @"response/forecast/simpleforecast/forecastdays/forecastday[period=5]/low/fahrenheit");
                    }
                    else
                    {
                        GetFieldFromXmlAndReport(xml, "Day4 High", @"response/forecast/simpleforecast/forecastdays/forecastday[period=5]/high/celsius");
                        GetFieldFromXmlAndReport(xml, "Night4 Low", @"response/forecast/simpleforecast/forecastdays/forecastday[period=5]/low/celsius");
                    }
                    GetFieldFromXmlAndReport(xml, "Night4 Precip", @"response/forecast/txt_forecast/forecastdays/forecastday[period=9]/pop");
                    GetFieldFromXmlAndReport(xml, "Night4 Summary", @"response/forecast/txt_forecast/forecastdays/forecastday[period=9]/fcttext");
                    GetFieldFromXmlAndReport(xml, "Night4 Image", @"response/forecast/txt_forecast/forecastdays/forecastday[period=9]/icon_url");
                    #endregion

                    #region Period6/10-11
                    GetFieldFromXmlAndReport(xml, "Day5 Precip", @"response/forecast/txt_forecast/forecastdays/forecastday[period=10]/pop");
                    GetFieldFromXmlAndReport(xml, "Day5 Forecast", @"response/forecast/simpleforecast/forecastdays/forecastday[period=6]/conditions");
                    GetFieldFromXmlAndReport(xml, "Day5 Summary", @"response/forecast/txt_forecast/forecastdays/forecastday[period=10]/fcttext");
                    GetFieldFromXmlAndReport(xml, "Day5 Image", @"response/forecast/simpleforecast/forecastdays/forecastday[period=6]/icon_url");
                    if (!Metric)
                    {
                        GetFieldFromXmlAndReport(xml, "Day5 High", @"response/forecast/simpleforecast/forecastdays/forecastday[period=6]/high/fahrenheit");
                        GetFieldFromXmlAndReport(xml, "Night5 Low", @"response/forecast/simpleforecast/forecastdays/forecastday[period=6]/low/fahrenheit");
                    }
                    else
                    {
                        GetFieldFromXmlAndReport(xml, "Day5 High", @"response/forecast/simpleforecast/forecastdays/forecastday[period=6]/high/celsius");
                        GetFieldFromXmlAndReport(xml, "Night5 Low", @"response/forecast/simpleforecast/forecastdays/forecastday[period=6]/low/celsius");
                    }
                    GetFieldFromXmlAndReport(xml, "Night5 Precip", @"response/forecast/txt_forecast/forecastdays/forecastday[period=11]/pop");
                    GetFieldFromXmlAndReport(xml, "Night5 Summary", @"response/forecast/txt_forecast/forecastdays/forecastday[period=11]/fcttext");
                    GetFieldFromXmlAndReport(xml, "Night5 Image", @"response/forecast/txt_forecast/forecastdays/forecastday[period=11]/icon_url");
                    #endregion

                    #region Period7/12-13
                    GetFieldFromXmlAndReport(xml, "Day6 Precip", @"response/forecast/txt_forecast/forecastdays/forecastday[period=12]/pop");
                    GetFieldFromXmlAndReport(xml, "Day6 Forecast", @"response/forecast/simpleforecast/forecastdays/forecastday[period=7]/conditions");
                    GetFieldFromXmlAndReport(xml, "Day6 Summary", @"response/forecast/txt_forecast/forecastdays/forecastday[period=12]/fcttext");
                    GetFieldFromXmlAndReport(xml, "Day6 Image", @"response/forecast/simpleforecast/forecastdays/forecastday[period=7]/icon_url");
                    if (!Metric)
                    {
                        GetFieldFromXmlAndReport(xml, "Day6 High", @"response/forecast/simpleforecast/forecastdays/forecastday[period=7]/high/fahrenheit");
                        GetFieldFromXmlAndReport(xml, "Night6 Low", @"response/forecast/simpleforecast/forecastdays/forecastday[period=7]/low/fahrenheit");
                    }
                    else
                    {
                        GetFieldFromXmlAndReport(xml, "Day6 High", @"response/forecast/simpleforecast/forecastdays/forecastday[period=7]/high/celsius");
                        GetFieldFromXmlAndReport(xml, "Night6 Low", @"response/forecast/simpleforecast/forecastdays/forecastday[period=7]/low/celsius");
                    }
                    GetFieldFromXmlAndReport(xml, "Night6 Precip", @"response/forecast/txt_forecast/forecastdays/forecastday[period=13]/pop");
                    GetFieldFromXmlAndReport(xml, "Night6 Summary", @"response/forecast/txt_forecast/forecastdays/forecastday[period=13]/fcttext");
                    GetFieldFromXmlAndReport(xml, "Night6 Image", @"response/forecast/txt_forecast/forecastdays/forecastday[period=13]/icon_url");
                    #endregion

                    #region Period8
                    GetFieldFromXmlAndReport(xml, "Day7 Precip", @"response/forecast/txt_forecast/forecastdays/forecastday[period=12]/pop");
                    GetFieldFromXmlAndReport(xml, "Day7 Forecast", @"response/forecast/simpleforecast/forecastdays/forecastday[period=7]/conditions");
                    GetFieldFromXmlAndReport(xml, "Day7 Summary", @"response/forecast/txt_forecast/forecastdays/forecastday[period=12]/fcttext");
                    GetFieldFromXmlAndReport(xml, "Day7 Image", @"response/forecast/simpleforecast/forecastdays/forecastday[period=7]/icon_url");
                    if (!Metric)
                    {
                        GetFieldFromXmlAndReport(xml, "Day7 High", @"response/forecast/simpleforecast/forecastdays/forecastday[period=7]/high/fahrenheit");
                        GetFieldFromXmlAndReport(xml, "Night7 Low", @"response/forecast/simpleforecast/forecastdays/forecastday[period=7]/low/fahrenheit");
                    }
                    else
                    {
                        GetFieldFromXmlAndReport(xml, "Day7 High", @"response/forecast/simpleforecast/forecastdays/forecastday[period=7]/high/celsius");
                        GetFieldFromXmlAndReport(xml, "Night7 Low", @"response/forecast/simpleforecast/forecastdays/forecastday[period=7]/low/celsius");
                    }
                    GetFieldFromXmlAndReport(xml, "Night7 Precip", @"response/forecast/txt_forecast/forecastdays/forecastday[period=13]/pop");
                    GetFieldFromXmlAndReport(xml, "Night7 Summary", @"response/forecast/txt_forecast/forecastdays/forecastday[period=13]/fcttext");
                    GetFieldFromXmlAndReport(xml, "Night7 Image", @"response/forecast/txt_forecast/forecastdays/forecastday[period=13]/icon_url");
                    #endregion
                }
            }
            catch (Exception ex)
            { Log.Error("Error updating forecasted weather - " + ex.Message); }
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

                string John = " " + " " + Convert.ToString(DuskStart);

                    if (gDebug) Log.Debug("Checking for Dawn/Dusk (Dawn start: " + Convert.ToString(DawnStart) + ", Dawn end: " + Convert.ToString(DawnEnd) + " Dusk start: " + Convert.ToString(DuskStart) + " Dusk end: " + Convert.ToString(DuskEnd)+ ")");
                }
                catch (Exception ex)
                {
                    Log.Error("Error setting times in updating day/night ", ex);
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
                        if (DayNight == "Night" | DayNight == "Dawn")
                            OSAE.OSAEObjectManager.EventTrigger(WeatherObjName, "Day");

                        DayNight = "Day";
                        Log.Debug("Day event Triggered");
                    }
                }
                else if (Now >= DuskEnd | Now < DawnStart)
                {
                    if (DayNight != "Night")
                    {
                        OSAEObjectPropertyManager.ObjectPropertySet(WeatherObjName, "DayNight", "Night",pName);
                        if (DayNight == "Day" | DayNight == "Dusk")
                            OSAE.OSAEObjectManager.EventTrigger(WeatherObjName, "Night");

                        DayNight = "Night";
                        Log.Info("Night event Triggered");
                    }
                }
                else if (Now >= DawnStart & Now < DawnEnd)
                {
                    if (DayNight != "Dawn")
                    {
                        OSAEObjectPropertyManager.ObjectPropertySet(WeatherObjName, "DayNight", "Dawn", pName);
                        DayNight = "Dawn";
                        Log.Info("Dawn");
                    }
                }

                else if (Now >= DuskStart & Now < DuskEnd)
                {
                    if (DayNight != "Dusk")
                    {
                        OSAEObjectPropertyManager.ObjectPropertySet(WeatherObjName, "DayNight", "Dusk", pName);
                        DayNight = "Dusk";
                        Log.Info("Dusk");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error updating day/night ",ex);
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
                Log.Info("WUnderground Plugin correctly owns the WUNDERGROUND Object Type.");

            //Added the follow for SYSTEM to automatically own Weather Base types that have no owner.
            oType = OSAEObjectTypeManager.ObjectTypeLoad("WUNDERGROUND");

            if (oType.OwnedBy == "")
            {
                OSAEObjectTypeManager.ObjectTypeUpdate(oType.Name, oType.Name, oType.Description, "SYSTEM", oType.BaseType, oType.Owner, oType.SysType, oType.Container, oType.HideRedundant);
                Log.Info("SYSTEM took ownership of the WEATHER Object Type.");
            }
            else
                Log.Info("SYSTEM Plugin correctly owns the WEATHER Object Type.");
        }
    }
}