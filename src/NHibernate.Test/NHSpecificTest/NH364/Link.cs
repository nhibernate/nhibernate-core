using System;
using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH364
{
	public class Link
	{
		int id;
		string name;
		IList categories;

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
