using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.LazyComponentTest
{
	public class Address
	{
		public virtual string Country { get; set; }
		public virtual string City { get; set; }
	}
}
