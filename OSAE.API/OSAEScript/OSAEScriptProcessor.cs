namespace OSAE
{
    public class OSAEScriptProcessor
    {
        /// <summary>
        /// the ID of the script processor entry
        /// </summary>
        public uint ID { get; set; }

        /// <summary>
        /// The name used in visual areas
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// the plugin that will process the script
        /// </summary>
        public string PluginName { get; set; }
    }
}
