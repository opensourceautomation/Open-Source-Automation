using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OSAE
{
    public class OSAEObjectType
    {
        private string _name;
        private string _baseType;
        private string _description;
        private string _ownedBy;
        private bool _owner;
        private bool _container;
        private bool _sysType;
        private bool _hideRedundant;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string BaseType
        {
            get { return _baseType; }
            set { _baseType = value; }
        }
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        public string OwnedBy
        {
            get { return _ownedBy; }
            set { _ownedBy = value; }
        }
        public bool Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }
        public bool Container
        {
            get { return _container; }
            set { _container = value; }
        }
        public bool SysType
        {
            get { return _sysType; }
            set { _sysType = value; }
        }
        public bool HideRedundant
        {
            get { return _hideRedundant; }
            set { _hideRedundant = value; }
        }
    }
}
