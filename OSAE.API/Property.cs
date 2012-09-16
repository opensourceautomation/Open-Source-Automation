namespace OSAE
{
    [System.Obsolete("use class ObjectProperty")]
    public class Property
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string DataType { get; set; }
        public string LastUpdated { get; set; }
    }
}
