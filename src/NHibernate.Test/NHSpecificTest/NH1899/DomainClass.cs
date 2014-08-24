

using System.Collections;
using System.Collections.Generic;
namespace NHibernate.Test.NHSpecificTest.NH1899
{
    public class Parent
    {
        private int id;
        private IDictionary<Key, Value> _relations;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public IDictionary<Key, Value> Relations {
            get { return _relations; }
            set { _relations = value; }
        }
    }

    public enum Key {
        One,
        Two
    }

    public enum Value {
        ValOne,
        ValTwo
    }
}