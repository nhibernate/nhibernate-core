using System;
using System.Collections;

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
		private IList _subcategories = new ArrayList();
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

		public IList Subcategories
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