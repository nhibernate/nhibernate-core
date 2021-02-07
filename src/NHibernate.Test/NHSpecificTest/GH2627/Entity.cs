using System;

namespace NHibernate.Test.NHSpecificTest.GH2627
{
	public class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}
}
