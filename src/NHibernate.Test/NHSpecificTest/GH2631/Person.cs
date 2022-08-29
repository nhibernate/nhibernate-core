using System;

namespace NHibernate.Test.NHSpecificTest.GH2631
{
	public class Person
	{
		public virtual Guid Id { get; set; }

		public virtual string Name { get; set; }

		public virtual Address Address { get; set; }
	}
}
