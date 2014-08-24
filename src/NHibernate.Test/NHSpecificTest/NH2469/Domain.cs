using System;

namespace NHibernate.Test.NHSpecificTest.NH2469
{
	public class Entity1
	{
		public virtual Guid Id { get; set; }
		public virtual int Foo { get; set; }
	}

	public class Entity2
	{
		public virtual Entity1 Entity1 { get; set; }

		public virtual bool Equals(Entity2 other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return Equals(other.Entity1, Entity1);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Entity2);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Entity1 != null ? Entity1.GetHashCode() : 0) * 397);
			}
		}
	}
}