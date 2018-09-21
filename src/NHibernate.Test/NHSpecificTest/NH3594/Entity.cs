using System;

namespace NHibernate.Test.NHSpecificTest.NH3594
{
	public class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}
}
