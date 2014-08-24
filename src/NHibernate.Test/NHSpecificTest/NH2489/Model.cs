using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2489
{
    public class Base
    {
        public virtual int Id
        {
            get;
            set;
        }

        public virtual IList<Child> Children
        {
            get;
            set;
        }

        public virtual IDictionary<string, Child> NamedChildren
        {
            get;
            set;
        }

        public virtual IDictionary<string, AnotherChild> OneToManyNamedChildren
        {
            get;
            set;
        }
    }

    public class Child
    {
        public virtual int Id
        {
            get;
            set;
        }
    }

    public class AnotherChild
    {
        public virtual int Id
        {
            get;
            set;
        }

        public virtual string Name
        {
            get;
            set;
        }
    }
}
