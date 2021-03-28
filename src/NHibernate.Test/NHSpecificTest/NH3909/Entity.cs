using System;

namespace NHibernate.Test.NHSpecificTest.NH3909
{
	public class ParentEntity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}

	public class ChildEntity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ParentEntity Parent { get; set; }
	}
}
