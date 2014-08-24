using System;

namespace NHibernate.Test.PropertyRef
{
    public class B
    {
        private int _id;
        private string _name;

        public virtual int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

    }
}