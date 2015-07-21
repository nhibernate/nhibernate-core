using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH364
{
	public class Link
	{
		private int id;
		private string name;
		private IList<Category> categories;

		public Link()
		{
			categories = new List<Category>();
		}

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public IList<Category> Categories
		{
			get { return categories; }
			set { categories = value; }
		}
	}
}