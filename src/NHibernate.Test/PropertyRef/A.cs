using System;
using Iesi.Collections;

namespace NHibernate.Test.PropertyRef
{
    public class A
    {
        private int _id;
        private int _extraId;
        private string _name;
        private ISet _items = new HashedSet();

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public int ExtraId
        {
            get { return _extraId; }
            set { _extraId = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public virtual ISet Items
        {
            get { return _items; }
            set { _items = value; }
        }
    }
}