namespace OSAE
{
    /// <summary>
    /// This class is used to hold information about the state of an object.
    /// </summary>
    public class OSAEObjectState
    {
        /// <summary>
        /// The state the object was in
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The amount of time the object was in the state
        /// </summary>
        public long TimeInState { get; set; }
    }
}
