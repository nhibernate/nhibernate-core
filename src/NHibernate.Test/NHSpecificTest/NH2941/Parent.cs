using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2941
{
    public class Parent
    {
        private string name;
        private int id;
        private int version;
        private IList<Child> children;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public int Version
        {
            get { return version; }
            set { version = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public IList<Child> Children
        {
            get { return children; }
            set { children = value; }
        }
    }
}