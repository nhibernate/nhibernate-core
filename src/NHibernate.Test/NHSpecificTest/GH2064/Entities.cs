using System;

namespace NHibernate.Test.NHSpecificTest.GH2064
{
	public class OneToOneEntity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}

	public class ParentEntity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual OneToOneEntity OneToOne { get; set; }
	}
}