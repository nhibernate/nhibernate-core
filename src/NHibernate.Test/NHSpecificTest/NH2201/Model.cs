using System;

namespace NHibernate.Test.NHSpecificTest.NH2201
{
	public class Parent
	{
		public virtual Guid Id { get; set; }
	}

	public class SubClass : Parent
	{
		public virtual string Name { get; set; }
	}
}
