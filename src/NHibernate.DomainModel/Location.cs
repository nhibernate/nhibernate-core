using System;
using System.Globalization;

namespace NHibernate.DomainModel 
{
	/// <summary>
	/// Summary description for Location.
	/// </summary>
	[Serializable]
	public class Location 
	{
		private int _streetNumber;
		private string _city;
		private string _streetName;
		private string _countryCode;
		private CultureInfo _locale;
		private string _description;
		
		public int StreetNumber
		{
			get { return _streetNumber; }
			set { _streetNumber = value; }
		}

		public string City
		{
			get { return _city; }
			set { _city = value; }
		}

		public string StreetName
		{
			get { return _streetName; }
			set { _streetName = value; }
		}

		public string CountryCode
		{
			get { return _countryCode; }
			set { _countryCode = value; }
		}

			
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		public CultureInfo Locale
		{
			get { return _locale; }
			set { _locale = value; }
		}

		#region System.Object Members

		public override bool Equals(object obj)
		{
			if(this==obj) return true;

			Location rhs = obj as Location;
			if(rhs==null) return false;

			return ( rhs.City.Equals(this.City) 
				&& rhs.StreetName.Equals(this.StreetName) 
				&& rhs.CountryCode.Equals(this.CountryCode)
				&& rhs.StreetNumber.Equals(this.StreetNumber)
				);
		}

		public override int GetHashCode()
		{
			return _streetName.GetHashCode();
		}

		#endregion
	}
}
