using System;

namespace NHibernate.Test.NHSpecificTest.GH1496
{
	// Entity with ManyToOne type (Address) with regular (int) Id
	public class Person
	{
		public virtual int Id { get; set; }

		public virtual string Name { get; set; }

		public virtual Address Address { get; set; }

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

	// Entity with ManyToOne type (Contact) with composite (ContactId) Id.
	public class Employee
	{
		public virtual int Id { get; set; }

		public virtual string Name { get; set; }

		public virtual Contact Contact { get; set; }

		public Employee()
		{
		}

		public Employee(int id, string name)
		{
			Id = id;
			Name = name;
		}
	}

	public class Address
	{
		public virtual int Id { get; set; }
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
	}

	public class Contact
	{
		public virtual ContactIdentifier ContactIdentifier { get; set; }
		public virtual string Phone { get; set; }
	}

	public class ContactIdentifier : IEquatable<ContactIdentifier>
	{
		public virtual string TypeName { get; set; }
		public virtual string ContactId { get; set; }

		public ContactIdentifier(string typeName, string contactId)
		{
			ContactId = contactId;
			TypeName = typeName;
		}

		public ContactIdentifier() { }

		public virtual bool Equals(ContactIdentifier other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(TypeName, other.TypeName) && string.Equals(ContactId, other.ContactId);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((ContactIdentifier) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((TypeName != null ? TypeName.GetHashCode() : 0) * 397) ^ (ContactId != null ? ContactId.GetHashCode() : 0);
			}
		}

		public static bool operator ==(ContactIdentifier left, ContactIdentifier right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(ContactIdentifier left, ContactIdentifier right)
		{
			return !Equals(left, right);
		}
	}
}
