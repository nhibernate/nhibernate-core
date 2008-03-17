using Iesi.Collections;

namespace NHibernate.Test.Ondelete
{
	public class Salesperson : Employee
	{
		private ISet customers = new HashedSet();
		public virtual ISet Customers
		{
			get { return customers; }
			set { customers = value; }
		}
	}
}
