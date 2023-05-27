using System;

namespace NHibernate.Test.NHSpecificTest.GH3513
{
	public class Customer
	{
		public virtual Guid Id { get; set; }
		public virtual string SerialNumber { get; set; }
	}
}
