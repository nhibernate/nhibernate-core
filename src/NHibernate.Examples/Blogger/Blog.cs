using System;
using System.Collections;

namespace NHibernate.Examples.Blogger
{
	public class Blog
	{
		private long id;
		private string name;
		private IList items;

		public long Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public IList Items
		{
			get { return items; }
			set { items = value; }
		}
	}
}