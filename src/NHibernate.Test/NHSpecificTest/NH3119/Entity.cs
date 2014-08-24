using System;
using System.Diagnostics;

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
		public Component()
		{
			LastCtorStackTrace = new StackTrace().ToString();
		}

		public string Value { get; set; }
		public string LastCtorStackTrace { get; private set; }
	}
}