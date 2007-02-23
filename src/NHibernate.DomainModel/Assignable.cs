using System;
using System.Collections;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for Assignable.
	/// </summary>
	public class Assignable
	{
		private string _id;
		private IList _categories;

		public string Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public IList Categories
		{
			get { return _categories; }
			set { _categories = value; }
		}
	}
}