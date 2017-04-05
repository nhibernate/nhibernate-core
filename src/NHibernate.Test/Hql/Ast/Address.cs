namespace NHibernate.Test.Hql.Ast
{
	public class Address
	{
		private string street;
		private string city;
		private string postalCode;
		private string country;
		private StateProvince stateProvince;

		public virtual string Street
		{
			get { return street; }
			set { street = value; }
		}

		public virtual string City
		{
			get { return city; }
			set { city = value; }
		}

		public virtual string PostalCode
		{
			get { return postalCode; }
			set { postalCode = value; }
		}

		public virtual string Country
		{
			get { return country; }
			set { country = value; }
		}

		public virtual StateProvince StateProvince
		{
			get { return stateProvince; }
			set { stateProvince = value; }
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Address))
				return false;

			var otherAddress = ((Address)obj);

			if (Street == null ^ otherAddress.Street == null)
			{
				return false;
			} 

			if (Street != null && otherAddress.Street != null && otherAddress.Street != Street)
			{
				return false;
			}

			if (City == null ^ otherAddress.City == null)
			{
				return false;
			}

			if (City != null && otherAddress.City != null && otherAddress.City != City)
			{
				return false;
			}

			if (PostalCode == null ^ otherAddress.PostalCode == null)
			{
				return false;
			}

			if (PostalCode != null && otherAddress.PostalCode != null && otherAddress.PostalCode != PostalCode)
			{
				return false;
			}

			if (Country == null ^ otherAddress.Country == null)
			{
				return false;
			}

			if (Country != null && otherAddress.Country != null && otherAddress.Country != Country)
			{
				return false;
			}

			if (StateProvince == null ^ otherAddress.StateProvince == null)
			{
				return false;
			}

			if (StateProvince != null && otherAddress.StateProvince != null && !otherAddress.StateProvince.Equals(StateProvince))
			{
				return false;
			}

			return true;
		}

		public override int GetHashCode()
		{
			int result = street != null ? street.GetHashCode() : 0;
			result = 31 * result + (city != null ? city.GetHashCode() : 0);
			result = 31 * result + (postalCode != null ? postalCode.GetHashCode() : 0);
			result = 31 * result + (country != null ? country.GetHashCode() : 0);
			result = 31 * result + (stateProvince != null ? stateProvince.GetHashCode() : 0);
			return result;
		}
	}
}