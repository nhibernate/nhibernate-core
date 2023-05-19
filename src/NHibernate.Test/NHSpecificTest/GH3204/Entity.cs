using System;

namespace NHibernate.Test.NHSpecificTest.GH3204
{
	public class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual int Version { get; set; }
		public virtual Child OneToOne { get; set; }
	}

	public class Child
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Entity Parent { get; set; }
	}
}
