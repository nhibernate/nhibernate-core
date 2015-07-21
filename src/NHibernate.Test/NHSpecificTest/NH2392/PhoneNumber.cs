using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH2392
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
	}
}
