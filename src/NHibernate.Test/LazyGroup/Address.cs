using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate.Test.LazyGroup
{
	public class Address
	{
		public string City { get; set; }

		public string Street { get; set; }

		public int PostCode { get; set; }
	}
}
