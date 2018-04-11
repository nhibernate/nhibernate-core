using System;

namespace NHibernate.Test.NHSpecificTest.GH1645
{
	public abstract class EntityBase
	{
		public virtual Guid Id { get; protected set; }

		public static bool operator ==(EntityBase left, EntityBase right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(EntityBase left, EntityBase right)
		{
			return !Equals(left, right);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as EntityBase);
		}

		public virtual bool Equals(EntityBase other)
		{
			if (other == null)
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			if (!IsTransient(this) && !IsTransient(other) && Equals(Id, other.Id))
			{
				var otherType = other.GetUnproxiedType();
				var thisType = GetUnproxiedType();
				return thisType.IsAssignableFrom(otherType) ||
					otherType.IsAssignableFrom(thisType);
			}

			return false;
		}

		public override int GetHashCode()
		{
			if (Equals(Id, default(Guid)))
			{
				return base.GetHashCode();
			}

			return Id.GetHashCode();
		}

		private static bool IsTransient(EntityBase obj)
		{
			return obj != null && Equals(obj.Id, default(Guid));
		}

		private System.Type GetUnproxiedType()
		{
			return GetType();
		}
	}
}
