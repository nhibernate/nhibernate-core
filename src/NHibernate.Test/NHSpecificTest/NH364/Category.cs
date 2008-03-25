using System;

namespace NHibernate.Test.NHSpecificTest.NH364
{
	/// <summary>
	/// Summary description for Category.
	/// </summary>
	public class Category
	{
		private int id;
		private string name;
		private Category parent;

		public Category()
		{
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

		public Category Parent
		{
			get { return parent; }
			set { parent = value; }
		}
	}
}