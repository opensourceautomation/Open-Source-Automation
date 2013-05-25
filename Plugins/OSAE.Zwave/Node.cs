using System;
using System.Collections.Generic;
using System.Text;
using OpenZWaveDotNet;

namespace OSAE.Zwave
{
    class Node
    {
        private Byte m_id = 0;
        public Byte ID
        {
            get { return m_id; }
            set { m_id = value; }
        }

        private UInt32 m_homeId = 0;
        public UInt32 HomeID
        {
            get { return m_homeId; }
            set { m_homeId = value; }
        }

        private String m_label = "";
        public String Label
        {
            get { return m_label; }
            set { m_label = value; }
        }

        private String m_manufacturer = "";
        public String Manufacturer
        {
            get { return m_manufacturer; }
            set { m_manufacturer = value; }
        }

        private String m_product = "";
        public String Product
        {
            get { return m_product; }
            set { m_product = value; }
        }

        private String m_level;
        public String Level
        {
            get { return m_level; }
            set { m_level = value; }
        }

        private List<Value> m_values;
        public List<Value> Values
        {
            get { return m_values; }
        }

        private String m_name;
        public String Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        public Node()
        {
            m_values = new List<Value>();
        }

        public Value GetValue(ZWValueID zwv)
        {
            foreach (Value value in m_values)
            {
                if (value.CommandClassID == zwv.GetCommandClassId().ToString() && value.Index == zwv.GetIndex().ToString())
                {
                    return value;
                }
            }
            return new Value();
        }

        public void AddValue(Value val)
        {
             m_values.Add(val);
        }

        public void RemoveValue(Value val)
        {
            m_values.Remove(val);
        }
    }

    class Value
    {
        private ZWValueID m_valueID;
        public ZWValueID ValueID
        {
            get { return m_valueID; }
            set { m_valueID = value; }
        }

        private ZWValueID.ValueGenre m_genre;
        public ZWValueID.ValueGenre Genre
        {
            get { return m_genre; }
            set { m_genre = value; }
        }

        private String m_cmdClassID;
        public String CommandClassID
        {
            get { return m_cmdClassID; }
            set { m_cmdClassID = value; }
        }

        private String m_index;
        public String Index
        {
            get { return m_index; }
            set { m_index = value; }
        }

        private ZWValueID.ValueType m_type;
        public ZWValueID.ValueType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        private String m_val;
        public String Val
        {
            get { return m_val; }
            set { m_val = value; }
        }

        private String m_label;
        public String Label
        {
            get { return m_label; }
            set { m_label = value; }
        }
    }
}
