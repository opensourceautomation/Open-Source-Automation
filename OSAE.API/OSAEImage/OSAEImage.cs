namespace OSAE
{
    /// <summary>
    /// This class holds an Instant of an OSA Image 
    /// </summary>
    public class OSAEImage
    {
        /// <summary>
        /// The DB id of the image
        /// </summary>
        public uint ID { get; set; }

        /// <summary>
        /// The image type e.g. jpg
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The name of the image
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The binary data for the image
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// The width of the image
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The height of the image
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The dpi of the image
        /// </summary>
        public int DPI { get; set; }

    }
}
