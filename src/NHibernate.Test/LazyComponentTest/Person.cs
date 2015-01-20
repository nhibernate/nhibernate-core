using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.LazyComponentTest
{
	public class Person
	{
		public virtual string Name { get; set; }
		public virtual Address Address { get; set; }
	}
}
