using System;

namespace NHibernate.Test.NHSpecificTest.GH1486
{
	public class Person
	{
		public virtual int Id { get; set; }

		public virtual string Name { get; set; }

		public virtual Address Address { get; set; }

		public virtual int Version { get; set; }

		public Person()
		{
		}

		public Person(int id, string name, Address address)
		{
			Id = id;
			Name = name;
			Address = address;
		}
	}

	public class Address : IEquatable<Address>
	{
		public virtual string PostalCode { get; set; }

		public virtual string State { get; set; }

		public virtual string Street { get; set; }

		public Address()
		{ }

		public Address(string postalCode, string state, string street)
		{
			PostalCode = postalCode;
			State = state;
			Street = street;
		}

		public bool Equals(Address other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Street, other.Street) && string.Equals(State, other.State) && string.Equals(PostalCode, other.PostalCode);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((Address) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (PostalCode != null ? PostalCode.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (State != null ? State.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Street != null ? Street.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(Address left, Address right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Address left, Address right)
		{
			return !Equals(left, right);
		}
	}
}
