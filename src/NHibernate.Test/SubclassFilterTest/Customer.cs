using System;

namespace NHibernate.Test.SubclassFilterTest
{
	public class Customer : Person
	{
		private Employee contactOwner;

		public Customer()
		{
		}

		public Customer(string name) : base(name)
		{
		}

		public virtual Employee ContactOwner
		{
			get { return contactOwner; }
			set { contactOwner = value; }
		}
	}
}