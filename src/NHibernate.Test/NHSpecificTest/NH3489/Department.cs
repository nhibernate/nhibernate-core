using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3489
{
	public class Department
	{
		public Department()
		{
			Orders = new HashSet<Order>();
		}

		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ISet<Order> Orders { get; set; }
	}
}