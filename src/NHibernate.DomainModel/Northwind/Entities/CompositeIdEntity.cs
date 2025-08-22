using System;

namespace NHibernate.DomainModel.Northwind.Entities
{
	public class CompositeId : IComparable<CompositeId>
	{
		public int ObjectId { get; set; } 
		public int TenantId { get; set; }

		public CompositeId() { }
		public CompositeId(int objectId, int tenantId)
		{
			ObjectId = objectId;
			TenantId = tenantId;
		}

		public override string ToString() => ObjectId + "|" + TenantId;
		protected bool Equals(CompositeId other) => ObjectId == other.ObjectId && TenantId == other.TenantId;
		public static bool operator ==(CompositeId left, CompositeId right) => Equals(left, right);
		public static bool operator !=(CompositeId left, CompositeId right) => !Equals(left, right);

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj) || obj.GetType() != this.GetType())
			{
				return false;
			}
			return ReferenceEquals(this, obj) || Equals((CompositeId)obj);
		}

		public override int GetHashCode() => HashCode.Combine(ObjectId, TenantId);

		public int CompareTo(CompositeId other)
		{
			if (ReferenceEquals(this, other))
			{
				return 0;
			}
			else if (ReferenceEquals(other, null))
			{
				return 1;
			}
			
			var idComparison = ObjectId.CompareTo(other.ObjectId);
			if (idComparison != 0)
			{
				return idComparison;
			}

			return TenantId.CompareTo(other);
		}
	}
    public class CompositeIdEntity
    {
        public virtual CompositeId Id { get; set; }
        public virtual string Name { get; set; }
    }
}
