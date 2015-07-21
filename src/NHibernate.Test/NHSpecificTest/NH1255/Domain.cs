using System;

namespace NHibernate.Test.NHSpecificTest.NH1255
{
	public class Customer
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}

	public class CustomerValue
	{
		public CustomerValueId Id { get; set; }
		public decimal Value { get; set; }
	}

	public class CustomerValueId : IEquatable<CustomerValueId>
	{
		private int? requestedHashCode;
		public Customer Customer { get; set; }
		public int CustomKey { get; set; }

		public bool Equals(CustomerValueId other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return Equals(other.Customer, Customer) && other.CustomKey == CustomKey;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			if (obj.GetType() != typeof (CustomerValueId))
			{
				return false;
			}
			return Equals((CustomerValueId) obj);
		}

		public override int GetHashCode()
		{
			if (!requestedHashCode.HasValue)
			{
				unchecked
				{
					requestedHashCode = ((Customer != null ? Customer.GetHashCode() : base.GetHashCode()) * 397) ^ CustomKey;
				}
			}
			return requestedHashCode.Value;
		}
	}
}
