using System;
using System.Collections;
using System.Text;
using Iesi.Collections;

namespace NHibernate.Test.SecondLevelCacheTests
{
    public class Item
    {
        int id;
        IList children = new ArrayList();
		Item parent;

    	public virtual Item Parent
    	{
    		get { return parent; }
    		set { parent = value; }
    	}

    	public virtual int Id
        {
            get { return id; }
            set { id = value; }
        }

        public virtual IList Children
        {
            get { return children; }
            set { children = value; }
        }

    }
}
