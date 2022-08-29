using System;

namespace NHibernate.Test.NHSpecificTest.GH2071
{
	public class Cat
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}

	public class DomesticCat : Cat
	{
		public virtual string OwnerName { get; set; }
	}
}
