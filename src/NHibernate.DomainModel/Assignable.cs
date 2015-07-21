using System;
using System.Collections.Generic;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for Assignable.
	/// </summary>
	public class Assignable
	{
		private string _id;
		private IList<Category> _categories;

		public string Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public IList<Category> Categories
		{
			get { return _categories; }
			set { _categories = value; }
		}
	}
}