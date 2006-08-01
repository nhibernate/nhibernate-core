using System;
using System.Collections;
using System.Text;
using Iesi.Collections;

namespace NHibernate.Test.SecondLevelCacheTests
{
    public class Item
    {
        int id;
        ISet children = new HashedSet();


        public virtual int Id
        {
            get { return id; }
            set { id = value; }
        }

        public virtual ISet Children
        {
            get { return children; }
            set { children = value; }
        }

    }
}
