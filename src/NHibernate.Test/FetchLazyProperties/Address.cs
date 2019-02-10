using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate.Test.FetchLazyProperties
{
	public class Address
	{
		public string City { get; set; }

		public string Country { get; set; }

		public Continent Continent { get; set; }
	}
}
