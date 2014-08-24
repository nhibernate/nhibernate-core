using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1098
{
    class A
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private int valueA;

        public int ValueA
        {
            get { return valueA; }
            set { valueA = value; }
        }

        private bool enabled;

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public IDictionary<int, string> C
        {
            get { return c; }
            set { c = value; }
        }

        private IDictionary<int, string> c = new Dictionary<int,string>();
    }

    class B
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private int valueB;

        public int ValueB
        {
            get { return valueB; }
            set { valueB = value; }
        }

        private bool enabled;

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }
    }
}
