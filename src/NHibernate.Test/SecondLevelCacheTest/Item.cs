using System;
using System.Collections.Generic;

namespace NHibernate.Test.SecondLevelCacheTests
{
	public class Item
	{
		private int id;
		private IList<Item> children = new List<Item>();
		private Item parent;
		private string name;

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

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

		public virtual IList<Item> Children
		{
			get { return children; }
			set { children = value; }
		}
	}
}