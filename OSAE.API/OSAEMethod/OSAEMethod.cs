namespace OSAE
{
    using MySql.Data.MySqlClient;
    using System;
    using System.Data;
    using System.Runtime.Serialization;

    /// <summary>
    /// Method passed to plugins to allow them to process an event in the system
    /// </summary>
    [Serializable, DataContract]
    public class OSAEMethod
    {
        public int Id { get; set; }

        /// <summary>
        /// The name of the method
        /// </summary>
        [DataMember]
        public string MethodName { get; set; }

        [DataMember]
        public string MethodLabel { get; set; }

        [DataMember]
        public string ObjectName { get; set; }

        /// <summary>
        /// The first parameter passed which may contain information that
        /// the plugin needs to process the request
        /// </summary>
        [DataMember]
        public string Parameter1 { get; set; }
       
        /// <summary>
        /// Second parameter passed which may contain information that 
        /// the plugin needs to process the request
        /// </summary>
        [DataMember]
        public string Parameter2 { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string Owner { get; set; }

        public OSAEMethod()
        {

        }
       
        public OSAEMethod(string methodName, string methodLabel, string objName, string param1, string param2, string address, string owner)
        {
            MethodName = methodName;
            MethodLabel = methodLabel;
            ObjectName = objName;
            Parameter1 = param1;
            Parameter2 = param2;
            Address = address;
            Owner = owner;
        }

        public OSAEMethod(int id, string methodName, string methodLabel, string objName, string param1, string param2, string address, string owner)
        {
            Id = id;
            MethodName = methodName;
            MethodLabel = methodLabel;
            ObjectName = objName;
            Parameter1 = param1;
            Parameter2 = param2;
            Address = address;
            Owner = owner;
        }

        public void Run()
        {
            if (this.Id == null | this.Id == 0)
            {
                throw new ArgumentException("Cannot invoke run when Id is not available");
            }

            if(string.IsNullOrEmpty(this.MethodName))
            {
                throw new ArgumentException("Cannot invoke run when method name is not available");
            }    
      
            if(string.IsNullOrEmpty(this.ObjectName))
            {
                this.ObjectName = GetObjectNameFromMethodId(Id);
            }

            OSAEMethodManager.MethodQueueAdd(this.ObjectName, this.MethodName, this.Parameter1, this.Parameter2, "API");                
        }

        private string GetObjectNameFromMethodId(int methodId)
        {            
            string objectName = null;

            try
            {
                DataSet dataset = new DataSet();

                using (MySqlCommand command = new MySqlCommand())
                {

                    command.CommandText = "SELECT object_name FROM  osae_v_method_queue WHERE method_id=@MethodId";
                    command.Parameters.AddWithValue("@MethodId", this.Id);
                    dataset = OSAESql.RunQuery(command);
                }

                if (dataset.Tables[0].Rows.Count > 0)
                {
                    objectName = dataset.Tables[0].Rows[0]["object_name"].ToString();
                }
            }
            catch (Exception ex)
            {
                Logging.GetLogger().AddToLog("API - GetObjectNameFromMethodId (" + methodId + ")error: " + ex.Message, true);                    
            }

            return ObjectName;
        }

        public override string ToString()
        {
            return this.MethodName;
        }
    }
}
