namespace OSAE
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using MySql.Data.MySqlClient;

    public static class OSAEScreenControlManager
    {
        public static List<OSAEScreenControl> GetScreenControls(string screenName)
        {           
            List<OSAEScreenControl> controls = new List<OSAEScreenControl>();

            try
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    DataSet dataset = new DataSet();
                    OSAEScreenControl ctrl = new OSAEScreenControl();

                    command.CommandText = "SELECT object_name, control_name, control_type, state_name, last_updated, coalesce(time_in_state, 0) as time_in_state FROM osae_v_screen_object WHERE screen_name=@ScreenName";
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
