using System;

namespace NHibernate.Test.NHSpecificTest.NH3455
{
	class Address
	{
		public virtual Guid Id { get; set; }
		public virtual string Street { get; set; }
		public virtual string City { get; set; }
		public virtual string Zip { get; set; }
		public virtual string State { get; set; }
	}
}