using System;

namespace NHibernate.Test.NHSpecificTest.NH2331
{
	public class Person
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}
}
