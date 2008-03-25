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

		public virtual string Street
		{
			get { return _street; }
			set { _street = value; }
		}

		public virtual string Zip
		{
			get { return _zip; }
			set { _zip = value; }
		}

		public virtual string Country
		{
			get { return _country; }
			set { _country = value; }
		}
	}
}