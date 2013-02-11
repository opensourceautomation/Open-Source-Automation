namespace OSAE
{
    using System;
    using MySql.Data.MySqlClient;
    using System.Collections.Generic;
    using System.Data;

    public class OSAEMethodManager
    {
        /// <summary>
        /// Adds a method to the queue
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="method"></param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        public static void MethodQueueAdd(string objectName, string method, string param1, string param2, string plugin)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_method_queue_add (@Name, @Method, @Param1, @Param2, @FromObject, @DebugInfo)";
                command.Parameters.AddWithValue("@Name", objectName);
                command.Parameters.AddWithValue("@Method", method);
                command.Parameters.AddWithValue("@Param1", param1);
                command.Parameters.AddWithValue("@Param2", param2);
                command.Parameters.AddWithValue("@FromObject", PluginManager.GetPluginName(plugin, Common.ComputerName));
                command.Parameters.AddWithValue("@DebugInfo", null);
                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - MethodQueueAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        public static void ClearMethodQueue()
        {
            using (MySqlConnection connection = new MySqlConnection(Common.ConnectionString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SET sql_safe_updates=0; DELETE FROM osae_method_queue;";
                    OSAESql.RunQuery(command);
                }
            }
        }

        /// <summary>
        /// Delete method from the queue
        /// </summary>
        /// <param name="methodID"></param>
        public static void MethodQueueDelete(int methodID)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_method_queue_delete (@ID)";
                command.Parameters.AddWithValue("@ID", methodID);
                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - MethodQueueDelete error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }  
     
        public static List<OSAEMethod> GetMethodsInQueue()
        {
            List<OSAEMethod> methods = new List<OSAEMethod>();

            DataSet dataset = new DataSet();
            MySqlCommand command = new MySqlCommand();
            command.CommandText = "SELECT method_queue_id, object_name, address, method_name, parameter_1, parameter_2, object_owner FROM osae_v_method_queue ORDER BY entry_time";
            dataset = OSAESql.RunQuery(command);

            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                OSAEMethod method = new OSAEMethod(row["method_name"].ToString(), row["object_name"].ToString(), row["parameter_1"].ToString(), row["parameter_2"].ToString(), row["address"].ToString(), row["object_owner"].ToString());
                methods.Add(method);
            }

            return methods;
        }
    }
}
