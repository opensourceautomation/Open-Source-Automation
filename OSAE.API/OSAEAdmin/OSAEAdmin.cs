namespace OSAE
{

    using MySql.Data.MySqlClient;
    using System;
    using System.Data;
    using System.Runtime.Serialization;


    /// <summary>
    /// Class used for WebUI Admin settings
    /// </summary>
    [Serializable]
    public class OSAEAdmin
    {

        public int Id = 1; // Only 1 record in this table, Primarykey is always 1

        public int ScreenTrust { get; set; }
        public string defaultScreen { get; set; }
        public int ObjectsTrust { get; set; }
        public int ObjectsAddTrust { get; set; }
        public int ObjectsUpdateTrust { get; set; }
        public int ObjectsDeleteTrust { get; set; }
        public int AnalyticsTrust { get; set; }
        public int ManagementTrust { get; set; }
        public int ObjectTypeTrust { get; set; }
        public int ObjectTypeAddTrust { get; set; }
        public int ObjectTypeUpdateTrust { get; set; }
        public int ObjectTypeDeleteTrust { get; set; }
        public int ScriptTrust { get; set; }
        public int ScriptAddTrust { get; set; }
        public int ScriptUpdateTrust { get; set; }
        public int ScriptDeleteTrust { get; set; }
        public int ScriptObjectAddTrust { get; set; }
        public int ScriptObjectTypeAddTrust { get; set; }
        public int PatternTrust { get; set; }
        public int PatternAddTrust { get; set; }
        public int PatternUpdateTrust { get; set; }
        public int PatternDeleteTrust { get; set; }
        public int ReaderTrust { get; set; }
        public int ReaderAddTrust { get; set; }
        public int ReaderUpdateTrust { get; set; }
        public int ReaderDeleteTrust { get; set; }
        public int ScheduleTrust { get; set; }
        public int ScheduleAddTrust { get; set; }
        public int ScheduleUpdateTrust { get; set; }
        public int ScheduleDeleteTrust { get; set; }
        public int ImagesTrust { get; set; }
        public int ImagesAddTrust { get; set; }
        public int ImagesDeleteTrust { get; set; }
        public int LogsTrust { get; set; }
        public int LogsClearTrust { get; set; }
        public int ValuesTrust { get; set; }
        public int ConfigTrust { get; set; }
        public int EventLogTrust { get; set; }
        public int MethodLogTrust { get; set; }
        public int ServerLogTrust { get; set; }
        public int DebugLogTrust { get; set; }
    }
}
