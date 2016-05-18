namespace OSAE
{
    using System;
    using MySql.Data.MySqlClient;

    public class OSAEAdminManager
    {
        public static OSAEAdmin GetAdminSettings()
        {
            OSAEAdmin adSets = new OSAEAdmin();
            OSAE.OSAEObject oObject = OSAE.OSAEObjectManager.GetObjectByName("WEB SERVER");

            adSets.ScreenTrust = Convert.ToInt32(oObject.Property("Screen Trust").Value);
            adSets.defaultScreen = Convert.ToString(oObject.Property("Screen Trust").Value);
            adSets.ObjectsTrust = Convert.ToInt32(oObject.Property("Objects Trust").Value);
            adSets.ObjectsAddTrust = Convert.ToInt32(oObject.Property("Objects Add Trust").Value);
            adSets.ObjectsUpdateTrust = Convert.ToInt32(oObject.Property("Objects Update Trust").Value);
            adSets.ObjectsDeleteTrust = Convert.ToInt32(oObject.Property("Objects Delete Trust").Value);
            adSets.AnalyticsTrust = Convert.ToInt32(oObject.Property("Analytics Trust").Value);
            adSets.LogsTrust = Convert.ToInt32(oObject.Property("Logs Trust").Value);
            adSets.LogsClearTrust = Convert.ToInt32(oObject.Property("Logs Clear Trust").Value);
            adSets.EventLogTrust = Convert.ToInt32(oObject.Property("Event Log Trust").Value);
            adSets.MethodLogTrust = Convert.ToInt32(oObject.Property("Method Log Trust").Value);
            adSets.ServerLogTrust = Convert.ToInt32(oObject.Property("Server Log Trust").Value);
            adSets.DebugLogTrust = Convert.ToInt32(oObject.Property("Debug Log Trust").Value);
            adSets.ValuesTrust = Convert.ToInt32(oObject.Property("Values Trust").Value);
            adSets.ManagementTrust = Convert.ToInt32(oObject.Property("Management Trust").Value);
            adSets.ObjectTypeTrust = Convert.ToInt32(oObject.Property("ObjectType Trust").Value);
            adSets.ObjectTypeAddTrust = Convert.ToInt32(oObject.Property("ObjectType Add Trust").Value);
            adSets.ObjectTypeUpdateTrust = Convert.ToInt32(oObject.Property("ObjectType Update Trust").Value);
            adSets.ObjectTypeDeleteTrust = Convert.ToInt32(oObject.Property("ObjectType Delete Trust").Value);
            adSets.ScriptTrust = Convert.ToInt32(oObject.Property("Script Trust").Value);
            adSets.ScriptAddTrust = Convert.ToInt32(oObject.Property("Script Add Trust").Value);
            adSets.ScriptUpdateTrust = Convert.ToInt32(oObject.Property("Script Update Trust").Value);
            adSets.ScriptDeleteTrust = Convert.ToInt32(oObject.Property("Script Delete Trust").Value);
            adSets.ScriptObjectAddTrust = Convert.ToInt32(oObject.Property("Script Object Add Trust").Value);
            adSets.ScriptObjectTypeAddTrust = Convert.ToInt32(oObject.Property("Script ObjectType Add Trust").Value);
            adSets.PatternTrust = Convert.ToInt32(oObject.Property("Pattern Trust").Value);
            adSets.PatternAddTrust = Convert.ToInt32(oObject.Property("Pattern Add Trust").Value);
            adSets.PatternUpdateTrust = Convert.ToInt32(oObject.Property("Pattern Update Trust").Value);
            adSets.PatternDeleteTrust = Convert.ToInt32(oObject.Property("Pattern Delete Trust").Value);
            adSets.ReaderTrust = Convert.ToInt32(oObject.Property("Reader Trust").Value);
            adSets.ReaderAddTrust = Convert.ToInt32(oObject.Property("Reader Add Trust").Value);
            adSets.ReaderUpdateTrust = Convert.ToInt32(oObject.Property("Reader Update Trust").Value);
            adSets.ReaderDeleteTrust = Convert.ToInt32(oObject.Property("Reader Delete Trust").Value);
            adSets.ScheduleTrust = Convert.ToInt32(oObject.Property("Schedule Trust").Value);
            adSets.ScheduleAddTrust = Convert.ToInt32(oObject.Property("Schedule Add Trust").Value);
            adSets.ScheduleUpdateTrust = Convert.ToInt32(oObject.Property("Schedule Update Trust").Value);
            adSets.ScheduleDeleteTrust = Convert.ToInt32(oObject.Property("Schedule Delete Trust").Value);
            adSets.ImagesTrust = Convert.ToInt32(oObject.Property("Images Trust").Value);
            adSets.ImagesAddTrust = Convert.ToInt32(oObject.Property("Images Add Trust").Value);
            adSets.ImagesDeleteTrust = Convert.ToInt32(oObject.Property("Images Delete Trust").Value);
            adSets.ConfigTrust = Convert.ToInt32(oObject.Property("Config Trust").Value);

            return adSets;

            /*
            try
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    DataSet dataset = new DataSet();
                    command.CommandText = "SELECT * FROM osae_admin";
                    dataset = OSAESql.RunQuery(command);
                    if (dataset.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt = dataset.Tables[0];
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (dr["ID"].ToString() == "ScreenTrust")
                                adSets.ScreenTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "defaultScreen")
                                adSets.defaultScreen = dr["Value"].ToString();
                            else if (dr["ID"].ToString() == "ObjectsTrust")
                                adSets.ObjectsTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "ObjectsAddTrust")
                                adSets.ObjectsAddTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "ObjectsUpdateTrust")
                                adSets.ObjectsUpdateTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "ObjectsDeleteTrust")
                                adSets.ObjectsDeleteTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "AnalyticsTrust")
                                adSets.AnalyticsTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "LogsTrust")
                                adSets.LogsTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "LogsClearTrust")
                                adSets.LogsClearTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "EventLogTrust")
                                adSets.EventLogTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "MethodLogTrust")
                                adSets.MethodLogTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "ServerLogTrust")
                                adSets.ServerLogTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "DebugLogTrust")
                                adSets.DebugLogTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "ValuesTrust")
                                adSets.ValuesTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "ManagementTrust")
                                adSets.ManagementTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "ObjectTypeTrust")
                                adSets.ObjectTypeTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "ObjectTypeAddTrust")
                                adSets.ObjectTypeAddTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "ObjectTypeUpdateTrust")
                                adSets.ObjectTypeUpdateTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "ObjectTypeDeleteTrust")
                                adSets.ObjectTypeDeleteTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "ScriptTrust")
                                adSets.ScriptTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "ScriptAddTrust")
                                adSets.ScriptAddTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "ScriptUpdateTrust")
                                adSets.ScriptUpdateTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "ScriptDeleteTrust")
                                adSets.ScriptDeleteTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "ScriptObjectAddTrust")
                                adSets.ScriptObjectAddTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "ScriptObjectTypeAddTrust")
                                adSets.ScriptObjectTypeAddTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "PatternTrust")
                                adSets.PatternTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "PatternAddTrust")
                                adSets.PatternAddTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "PatternUpdateTrust")
                                adSets.PatternUpdateTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "PatternDeleteTrust")
                                adSets.PatternDeleteTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "ReaderTrust")
                                adSets.ReaderTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "ReaderAddTrust")
                                adSets.ReaderAddTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "ReaderUpdateTrust")
                                adSets.ReaderUpdateTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "ReaderDeleteTrust")
                                adSets.ReaderDeleteTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "ScheduleTrust")
                                adSets.ScheduleTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "ScheduleAddTrust")
                                adSets.ScheduleAddTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "ScheduleUpdateTrust")
                                adSets.ScheduleUpdateTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "ScheduleDeleteTrust")
                                adSets.ScheduleDeleteTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "ImagesTrust")
                                adSets.ImagesTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "ImagesAddTrust")
                                adSets.ImagesAddTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "ImagesDeleteTrust")
                                adSets.ImagesDeleteTrust = Convert.ToInt32(dr["Value"].ToString());
                            else if (dr["ID"].ToString() == "ConfigTrust")
                                adSets.ConfigTrust = Convert.ToInt32(dr["Value"].ToString());
                        }
                    }
                    return adSets;
                }
            }
            catch { return null; }
            */
        }


        public static int GetAdminSettingsByName(string name)
        {
            int pVal = Convert.ToInt32(OSAE.OSAEObjectPropertyManager.GetObjectPropertyValue("Web Server", name).Value);
            return pVal;
            /*
            int pVal = 0;
            using (MySqlCommand command = new MySqlCommand())
            {
                DataSet dataset = new DataSet();
                command.CommandText = "SELECT * FROM osae_admin WHERE ID = '" + name + "'";
                dataset = OSAESql.RunQuery(command);
                if (dataset.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = dataset.Tables[0];
                    DataRow dr = dt.Rows[0];
                    pVal = Convert.ToInt32(dr["Value"].ToString());
                }
            }
            return pVal;

                    //OSAEAdmin adSets = new OSAEAdmin();
            //try
            //{
            //    Int32 xSet = 0;
            //    OSAEAdmin adset = OSAEAdminManager.GetAdminSettings();
            //    xSet = Convert.ToInt32(adset.GetType().GetProperty(name).GetValue(adset, null));
            //    return xSet;
            //}
            //catch
            //{
            //    return 0;
            //}
            */
        }

        //public static void UpdateAdminSettingsAll(string ScreenTrust, string ObjectsTrust, string AnalyticsTrust, string LogsTrust, string ValuesTrust, string ConfigTrust, string ManagementTrust, string ObjectTypeTrust, string ScriptTrust, string PatternTrust, string ReaderTrust, string ScheduleTrust, string ImagesTrust, string EventLogTrust, string MethodLogTrust, string ServerLogTrust, string DebugLogTrust, string defaultScreen)
        public static void UpdateAdminSettings(OSAEAdmin adSet)
        {
            try
            {
                foreach (var prop in adSet.GetType().GetProperties())
                {
                    using (MySqlConnection connection = new MySqlConnection(Common.ConnectionString))
                    {
                        MySqlCommand command = new MySqlCommand();
                        command.Connection = connection;
                        //command.CommandText = "UPDATE osae_admin SET ScreenTrust=" + ScreenTrust + ",ObjectsTrust=" + ObjectsTrust + ",AnalyticsTrust=" + AnalyticsTrust + ",LogsTrust=" + LogsTrust + ",ValuesTrust=" + ValuesTrust + ",ConfigTrust=" + ConfigTrust + ",ManagementTrust=" + ManagementTrust + ",ObjectTypeTrust=" + ObjectTypeTrust + ",ScriptTrust=" + ScriptTrust + ",PatternTrust=" + PatternTrust + ",ReaderTrust=" + ReaderTrust + ",ScheduleTrust=" + ScheduleTrust + ",ImagesTrust=" + ImagesTrust + ",EventLogTrust=" + EventLogTrust + ",MethodLogTrust=" + MethodLogTrust + ",ServerLogTrust=" + ServerLogTrust + ",DebugLogTrust=" + DebugLogTrust + ",defaultScreen='" + defaultScreen + "' WHERE primary_key = 1";
                        command.CommandText = "UPDATE osae_admin SET Value = '" + prop.GetValue(adSet, null) + "' WHERE ID = '" + prop.Name + "'";
                        connection.Open();
                        int x = command.ExecuteNonQuery();
                        connection.Dispose();
                    }
                }
            }
            catch { }
        }

        
        public static void UpdateAdminSettingByName(string name, string val)
        {
            OSAE.OSAEObjectPropertyManager.ObjectPropertySet("Web Server", name, val,"SYSTEM");
            /*
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Common.ConnectionString))
                {
                    MySqlCommand command = new MySqlCommand();
                    command.Connection = connection;
                    command.CommandText = "UPDATE osae_admin SET Value = '" + val + "' WHERE ID = '" + name + "'";
                    connection.Open();
                    int x = command.ExecuteNonQuery();
                    connection.Dispose();
                }
            }
            catch { }
            */
        }

    }
}