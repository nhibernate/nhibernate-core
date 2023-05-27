using System;

namespace NHibernate.Test.NHSpecificTest.GH3513
{
	public class Orders
	{
		public virtual Guid Id { get; set; }
		public virtual Customer Customer1 { get; set; }
	}
}
