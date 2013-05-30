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
            try
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
            catch (Exception ex)
            {
                Logging.GetLogger().AddToLog("Error clearing method queue details: \r\n" + ex.Message, true);
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

            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "SELECT method_queue_id, object_name, address, method_name, method_label, parameter_1, parameter_2, object_owner FROM osae_v_method_queue ORDER BY entry_time";
                DataSet dataset = OSAESql.RunQuery(command);

                foreach (DataRow row in dataset.Tables[0].Rows)
                {
                    object methodId = row["method_queue_id"];
                    object methodName = row["method_name"];
                    object methodLabel = row["method_label"];
                    object objectName = row["object_name"];
                    object parameter1 = row["parameter_1"];
                    object parameter2 = row["parameter_2"];
                    object address = row["address"];
                    object owner = row["object_owner"];
                                        
                    OSAEMethod method = new OSAEMethod();

                    // These parameters should never be null so we won't check them
                    method.Id = int.Parse(methodId.ToString());
                    method.MethodName = methodName.ToString();
                    method.ObjectName = objectName.ToString();
                    
                    // These parameters could be null
                    if (parameter1 != null)
                    {
                        method.Parameter1 = parameter1.ToString();
                    }

                    if (parameter2 != null)
                    {
                        method.Parameter2 = parameter2.ToString();
                    }

                    if (address != null)
                    {
                        method.Address = address.ToString();
                    }

                    if (owner != null)
                    {
                        method.Owner = owner.ToString();
                    }

                    methods.Add(method);                    
                }
            }
            
            return methods;
        }
    }
}
