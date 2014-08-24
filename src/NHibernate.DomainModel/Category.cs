using System;
using System.Collections.Generic;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for Category.
	/// </summary>
	public class Category
	{
		public static readonly string RootCategory = "/";

		private long _id;
		private string _name;
		private IList<Category> _subcategories = new List<Category>();
		private Assignable _assignable;

		public long Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public IList<Category> Subcategories
		{
			get { return _subcategories; }
			set { _subcategories = value; }
		}

		public Assignable Assignable
		{
			get { return _assignable; }
			set { _assignable = value; }
		}
	}
}