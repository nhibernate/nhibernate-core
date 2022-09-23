using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NHibernate.Test.NHSpecificTest.NH3530
{
	public class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual IList<Child> Children { get; set; } = new List<Child>();
	}

	public class EntityComponent
	{
		public virtual int Id1 { get; set; }
		public virtual int Id2 { get; set; }
		public virtual string Name { get; set; }
		public virtual IList<ChildForComponent> Children { get; set; } = new List<ChildForComponent>();

		public override bool Equals(object obj)
		{
			return obj == this;
		}

		public override int GetHashCode()
		{
			return RuntimeHelpers.GetHashCode(this);
		}
	}

	public class ComponentId
	{
		public int Id1 { get; set; }
		public int Id2 { get; set; }

		protected bool Equals(ComponentId other)
		{
			return Id1 == other.Id1 && Id2 == other.Id2;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((ComponentId) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Id1 * 397) ^ Id2;
			}
		}
	}

	public class EntityComponentId
	{
		public virtual ComponentId Id { get; set; }

		public virtual string Name { get; set; }
		public virtual IList<ChildForComponent> Children { get; set; } = new List<ChildForComponent>();

		public override bool Equals(object obj)
		{
			return obj == this;
		}

		public override int GetHashCode()
		{
			return RuntimeHelpers.GetHashCode(this);
		}
	}

	public class Child
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Guid ParentId { get; set; }
	}

	public class ChildForComponent
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual int ParentId1 { get; set; }
		public virtual int ParentId2 { get; set; }
	}
}
