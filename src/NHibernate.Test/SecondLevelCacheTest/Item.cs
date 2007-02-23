using System;
using System.Collections;

namespace NHibernate.Test.SecondLevelCacheTests
{
	public class Item
	{
		private int id;
		private IList children = new ArrayList();
		private Item parent;

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