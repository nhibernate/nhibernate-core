using System;

namespace NHibernate.Test.JoinedSubclass
{
	/// <summary>
	/// An Address that is used as a <c>component</c>.
	/// </summary>
	public class Address
	{
		private string _street;
		private string _zip;
		private string _country;

		public Address()
		{
		}

		public string Street
		{
			get { return _street; }
			set { _street = value; }
		}

		public string Zip
		{
			get { return _zip; }
			set { _zip = value; }
		}

		public string Country
		{
			get { return _country; }
			set { _country = value; }
		}


	}
}
