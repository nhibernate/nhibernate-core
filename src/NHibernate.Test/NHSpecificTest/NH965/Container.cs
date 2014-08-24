using System;
using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH965
{
    public class Container
    {
        private int id;
        private IList elements;

        public virtual int Id
        {
            get { return id; }
            set { id = value; }
        }

        public virtual IList Elements
        {
            get { return elements; }
            set { elements = value; }
        }
    }
}
