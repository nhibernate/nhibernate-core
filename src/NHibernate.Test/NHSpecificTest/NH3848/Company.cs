using System;

namespace NHibernate.Test.NHSpecificTest.NH3848
{
	public class Company
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Customer Customer { get; set; }
	}
}
