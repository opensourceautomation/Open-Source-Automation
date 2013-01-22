namespace OSAE
{
    public class ObjectState
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
