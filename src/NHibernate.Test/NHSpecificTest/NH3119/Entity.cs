using System;

namespace NHibernate.Test.NHSpecificTest.NH3119
{
	public class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Component Component { get; set; }
	}

	public class Component
	{
		public string Value { get; set; }
	}
}
