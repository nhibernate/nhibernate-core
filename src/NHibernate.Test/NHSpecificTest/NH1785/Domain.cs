using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1785
{
	public class Entity1
	{
		public virtual Guid Id { get; set; }
		public virtual ISet<Entity2> Entities2 { get; set; }
	}

	public class Entity2Id
	{
		public virtual Entity1 Entity1 { get; set; }
		public virtual Entity3 Entity3 { get; set; }

		public bool Equals(Entity2Id other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return Equals(other.Entity1, Entity1) && Equals(other.Entity3, Entity3);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Entity2Id);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Entity1 != null ? Entity1.GetHashCode() : 0) * 397) ^ (Entity3 != null ? Entity3.GetHashCode() : 0);
			}
		}
	}

	public class Entity2
	{
		public virtual Entity2Id Id { get; set; }
	}

	public class Entity3
	{
		public virtual Guid Id { get; set; }
		public virtual Entity4 Entity4 { get; set; }
	}

	public class Entity4
	{
		public virtual Guid Id { get; set; }
	}
}
