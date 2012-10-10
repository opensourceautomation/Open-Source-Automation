namespace OSAE.API.Images
{
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
    }
}
