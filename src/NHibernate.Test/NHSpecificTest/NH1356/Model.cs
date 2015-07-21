using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1356
{
	public class Person
	{
		private ICollection<Address> addresses;
		private int id;
		private string name;

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public ICollection<Address> Addresses
		{
			get { return addresses; }
			set { addresses = value; }
		}
	}

	public struct Address
	{
		private readonly string city;
		private readonly string postalCode;
		private readonly string state;
		private readonly string street;

		public Address(string street, string city, string state, string postalCode)
		{
			this.street = street;
			this.city = city;
			this.state = state;
			this.postalCode = postalCode;
		}

		public string Street
		{
			get { return street; }
		}

		public string City
		{
			get { return city; }
		}

		public string State
		{
			get { return state; }
		}

		public string PostalCode
		{
			get { return postalCode; }
		}
	}
}