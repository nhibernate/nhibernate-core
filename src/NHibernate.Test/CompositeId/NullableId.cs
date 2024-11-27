using System;

namespace NHibernate.Test.CompositeId
{
	public class NullableId : IComparable<NullableId>
	{
		public int? Id { get; set; } 
		public int WarehouseId { get; set; }

		public NullableId() { }
		public NullableId(int? id, int warehouseId)
		{
			Id = id;
			WarehouseId = warehouseId;
		}

		public override string ToString() => Id + "|" + WarehouseId;
		protected bool Equals(NullableId other) => Id == other.Id && WarehouseId == other.WarehouseId;
		public static bool operator ==(NullableId left, NullableId right) => Equals(left, right);
		public static bool operator !=(NullableId left, NullableId right) => !Equals(left, right);

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj) || obj.GetType() != this.GetType())
			{
				return false;
			}

			return ReferenceEquals(this, obj) || Equals((NullableId)obj);
		}

		public override int GetHashCode() => HashCode.Combine(Id, WarehouseId);

		public int CompareTo(NullableId other)
		{
			if (ReferenceEquals(this, other))
			{
				return 0;
			}
			else if (ReferenceEquals(other, null) || !other.Id.HasValue)
			{
				return 1;
			}
			else if (!Id.HasValue)
			{
				return -1;
			}

			var idComparison = Id.Value.CompareTo(other.Id);
			if (idComparison != 0)
			{
				return idComparison;
			}

			return WarehouseId.CompareTo(other.WarehouseId);
		}
	}
}
