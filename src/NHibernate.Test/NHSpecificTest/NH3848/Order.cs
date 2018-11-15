using System;

namespace NHibernate.Test.NHSpecificTest.NH3848
{
	public class Order
	{
		public virtual Guid Id { get; set; }
		public virtual int Number { get; set; }
		public virtual Customer Customer { get; set; }
	}
}
