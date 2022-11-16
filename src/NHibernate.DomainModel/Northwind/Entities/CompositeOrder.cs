using System;

namespace NHibernate.DomainModel.Northwind.Entities
{
	public class CompositeOrder : IEquatable<CompositeOrder>
	{
		public virtual int OrderId { get; set; }

		public virtual Customer Customer { get; set; }

		public virtual DateTime? OrderDate { get; set; }

		public virtual DateTime? RequiredDate { get; set; }

		public virtual DateTime? ShippingDate { get; set; }

		public virtual Shipper Shipper { get; set; }

		public virtual decimal? Freight { get; set; }

		public virtual string ShippedTo { get; set; }

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((CompositeOrder) obj);
		}

		public virtual bool Equals(CompositeOrder other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return OrderId == other.OrderId && Equals(Customer, other.Customer);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (OrderId * 397) ^ (Customer != null ? Customer.GetHashCode() : 0);
			}
		}
	}
}
