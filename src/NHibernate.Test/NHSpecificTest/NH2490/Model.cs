
namespace NHibernate.Test.NHSpecificTest.NH2490
{
    public class Base
    {
        public virtual int Id
        {
            get;
            set;
        }

        public virtual string ShortContent
        {
            get;
            set;
        }

        public virtual string LongContent
        {
            get;
            set;
        }
    }

    public class Derived : Base
    {
        public virtual string ShortContent2
        {
            get;
            set;
        }

        public virtual string LongContent2
        {
            get;
            set;
        }
    }
}
