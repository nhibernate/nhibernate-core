using System;


namespace NHibernate.Test.NHSpecificTest.GH1486
{

	public class Person
	{
		private int id;
		private string name;
		private Address address;
		private int version;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual Address Address
		{
			get { return address; }
			set { address = value; }
		}

		public virtual int Version
		{
			get { return version; }
			set { version = value; }
		}

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
		private string postalCode;
		private string state;
		private string street;

		public virtual string PostalCode
		{
			get { return postalCode; }
			set { postalCode = value; }
		}

		public virtual string State
		{
			get { return state; }
			set { state = value; }
		}

		public virtual string Street
		{
			get { return street; }
			set { street = value; }
		}


		public Address()
		{ }

		public Address(string postalCode, string state, string street)
		{
			this.postalCode = postalCode;
			this.state = state;
			this.street = street;
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
			if (obj.GetType() != this.GetType()) return false;
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
