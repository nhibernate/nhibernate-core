using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1863
{
	public class Customer
	{
		private ISet<Category> _categories = new HashSet<Category>();

		public virtual int Id { get; set; }

		public virtual string Name { get; set; }

		public virtual ISet<Category> Categories
		{
			get { return _categories; }
			set { _categories = value; }
		}
	}

	public class Category
	{
		public virtual int Id { get; set; }

		public virtual bool IsActive { get; set; }
	}

}
