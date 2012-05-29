using Iesi.Collections.Generic;

namespace NHibernate.Test.Ondelete
{
	public class Salesperson : Employee
	{
		private ISet<Person> customers = new HashedSet<Person>();

		public virtual ISet<Person> Customers
		{
			get { return customers; }
			set { customers = value; }
		}
	}
}
