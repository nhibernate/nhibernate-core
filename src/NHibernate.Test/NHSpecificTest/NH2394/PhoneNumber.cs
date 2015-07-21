using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH2394
{
	public class PhoneNumber
	{
		public PhoneNumber(int countryCode, string number)
		{
			CountryCode = countryCode;
			Number = number;
		}

		public int CountryCode { get; private set; }
		public string Number { get; private set; }

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			if (obj.GetType() != GetType())
				return false;

			PhoneNumber that = (PhoneNumber) obj;

			return
				CountryCode == that.CountryCode &&
				Number == that.Number;
		}

		public override int GetHashCode()
		{
			return CountryCode.GetHashCode() ^ (Number ?? "").GetHashCode();
		}
	}
}
