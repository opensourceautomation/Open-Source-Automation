namespace OSAE
{

    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// This class is used to hold information about the state of an object.
    /// </summary>
    [Serializable, DataContract]
    public class OSAEObjectState
    {
        private string _value;
        private string _label;
        private long _timeInState;
        private DateTime _lastStateChange;

        /// <summary>
        /// The state the object was in
        /// </summary>=
        [DataMember]
        public string Value 
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// The amount of time the object was in the state
        /// </summary>
        [DataMember]
        public long TimeInState
        {
            get { return _timeInState; }
            set { _timeInState = value; }
        }

        /// <summary>
        /// The label of the state the object was in
        /// </summary>=
        [DataMember]
        public string Label
        {
            get { return _label; }
            set { _label = Label; }
        }

        public DateTime LastStateChange
        {
            get { return _lastStateChange; }
            set { _lastStateChange = value; }
        }

        public override string ToString()
        {
            return this.Value;
        }
    }
}
