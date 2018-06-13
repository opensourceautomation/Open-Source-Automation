namespace OSAE
{
    using System;
    using MySql.Data.MySqlClient;

    /// <summary>
    /// Used to manage user trust levels of the OSAEAdmin class
    /// </summary>
    public class OSAEAdminManager
    {
        public static OSAEAdmin GetAdminSettings()
        {
            OSAEAdmin adSets = new OSAEAdmin();
            OSAEObject oObject = OSAEObjectManager.GetObjectByName("WEB SERVER");
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
        }

        public static int GetAdminSettingsByName(string name)
        {
            int pVal = Convert.ToInt32(OSAE.OSAEObjectPropertyManager.GetObjectPropertyValue("Web Server", name).Value);
            return pVal;
        }

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
        }

    }
}