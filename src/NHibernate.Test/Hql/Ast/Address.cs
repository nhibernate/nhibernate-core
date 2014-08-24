namespace NHibernate.Test.Hql.Ast
{
	public class Address
	{
		private string street;
		private string city;
		private string postalCode;
		private string country;
		private StateProvince stateProvince;

		public string Street
		{
			get { return street; }
			set { street = value; }
		}

		public string City
		{
			get { return city; }
			set { city = value; }
		}

		public string PostalCode
		{
			get { return postalCode; }
			set { postalCode = value; }
		}

		public string Country
		{
			get { return country; }
			set { country = value; }
		}

		public StateProvince StateProvince
		{
			get { return stateProvince; }
			set { stateProvince = value; }
		}
	}
}