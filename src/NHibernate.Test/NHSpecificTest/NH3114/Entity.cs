using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3114
{
	public class Component
	{
		public virtual string ComponentProperty { get; set; }
		public virtual ICollection<string> ComponentCollection { get; set; } = new List<string>();
	}
	public class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual Component FirstComponent { get; set; }
		public virtual Component SecondComponent { get; set; }
	}
}
