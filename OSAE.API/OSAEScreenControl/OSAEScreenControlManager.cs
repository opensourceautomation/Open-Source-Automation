namespace OSAE
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using MySql.Data.MySqlClient;

    public static class OSAEScreenControlManager
    {
        public static void ScreenObjectAdd(string screen, string objectName, string controlName)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_screen_object_add(@Screen, @ObjectName, @ControlName)";
                command.Parameters.AddWithValue("@Screen", screen);
                command.Parameters.AddWithValue("@ObjectName", objectName);
                command.Parameters.AddWithValue("@ControlName", controlName);

                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("ScreenObjectAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        public static void ScreenObjectUpdate(string screen, string objectName, string controlName)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_screen_object_update(@Screen, @ObjectName, @ControlName)";
                command.Parameters.AddWithValue("@Screen", screen);
                command.Parameters.AddWithValue("@ObjectName", objectName);
                command.Parameters.AddWithValue("@ControlName", controlName);

                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("ScreenObjectUpdate error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }


        public static void ScreenObjectDelete(string screen, string objectName, string controlName)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_screen_object_delete(@Screen, @ObjectName, @ControlName)";
                command.Parameters.AddWithValue("@Screen", screen);
                command.Parameters.AddWithValue("@ObjectName", objectName);
                command.Parameters.AddWithValue("@ControlName", controlName);

                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("ScreenObjectDelete error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }



        public static List<OSAEScreenControl> GetScreenControls(string screenName)
        {           
            List<OSAEScreenControl> controls = new List<OSAEScreenControl>();

            try
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    DataSet dataset = new DataSet();
                    OSAEScreenControl ctrl = new OSAEScreenControl();

                    command.CommandText = "SELECT object_name, control_name, control_type, state_name, coalesce(last_updated,NOW()) as last_updated, coalesce(property_last_updated,NOW()) as property_last_updated, coalesce(time_in_state, 0) as time_in_state FROM osae_v_screen_object WHERE screen_name=@ScreenName AND control_enabled = 1";
                    command.Parameters.AddWithValue("@ScreenName", screenName);
                    dataset = OSAESql.RunQuery(command);

                    if (dataset.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in dataset.Tables[0].Rows)
                        {
                            ctrl = new OSAEScreenControl();
                            ctrl.ObjectState = dr["state_name"].ToString();

                            ctrl.TimeInState = Convert.ToInt64(dr["time_in_state"]).ToString();
                            ctrl.ControlName = dr["control_name"].ToString();
                            ctrl.ControlType = dr["control_type"].ToString();
                            ctrl.LastUpdated = DateTime.Parse(dr["last_updated"].ToString());
                            ctrl.PropertyLastUpdated = DateTime.Parse(dr["property_last_updated"].ToString());
                            ctrl.ObjectName = dr["object_name"].ToString();

                            controls.Add(ctrl);
                        }
                        return controls;
                    }
                }
                return controls;
            }
            catch (Exception ex)
            {
                Logging.GetLogger().AddToLog("API - GetObjectsByBaseType error: " + ex.Message, true);
                return controls;
            }
        }
    }
}
