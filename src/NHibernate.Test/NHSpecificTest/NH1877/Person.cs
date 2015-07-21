using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1877
{
	public class Person
	{
		public virtual long Id { get; set; }
		public virtual DateTime BirthDate { get; set; }
	}
}
