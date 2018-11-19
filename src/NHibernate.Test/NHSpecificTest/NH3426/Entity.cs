using System;

namespace NHibernate.Test.NHSpecificTest.NH3426
{
	public class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}
}
