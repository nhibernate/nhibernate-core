using System;

namespace NHibernate.Test.NHSpecificTest.NH2865
{
	class OrderLine
	{
		public virtual Guid Id { get; set; }
		public virtual string Quantity { get; set; }
	}
}