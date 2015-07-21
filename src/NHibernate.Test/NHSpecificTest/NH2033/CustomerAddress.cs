using System;

namespace NHibernate.Test.NHSpecificTest.NH2033
{
	public class CustomerAddress : IEquatable<CustomerAddress>
	{
		public virtual Customer Customer { get; set; }
		public virtual string Type { get; set; }
		public virtual string Address { get; set; }
		public virtual string City { get; set; }
		public virtual Customer OtherCustomer { get; set; }

		public virtual bool Equals(CustomerAddress other)
		{
			return ReferenceEquals(this, other)
				   || (other != null
					   && Equals(Customer.AssignedId, other.Customer.AssignedId)
					   && Type == other.Type);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as CustomerAddress);
		}

		public override int GetHashCode()
		{
			return (Customer != null ? Customer.AssignedId.GetHashCode() : 0)
				^ Type.GetHashCode();
		}
	}
}