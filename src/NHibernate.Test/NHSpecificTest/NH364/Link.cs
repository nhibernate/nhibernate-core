using System;
using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH364
{
	public class Link
	{
		private int id;
		private string name;
		private IList categories;

		public Link()
		{
			categories = new ArrayList();
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

		public IList Categories
		{
			get { return categories; }
			set { categories = value; }
		}
	}
}