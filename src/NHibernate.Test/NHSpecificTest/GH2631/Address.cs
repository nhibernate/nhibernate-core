using System;

namespace NHibernate.Test.NHSpecificTest.GH2631
{
	public class Address
	{
		public virtual Guid Id { get; set; }

		public virtual Person Person { get; set; }

		public virtual string Street { get; set; }
	}
}
