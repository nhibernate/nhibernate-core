using System;

namespace NHibernate.Test.NHSpecificTest.GH3563
{
	public class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual EntityStatus Status { get; set; }
	}

	public enum EntityStatus
	{
		Active,
		Inactive
	}

	public class BaseClass
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}

	public class Subclass : BaseClass
	{
	}

	public enum Kind
	{
		Base,
		Derived
	}
}
