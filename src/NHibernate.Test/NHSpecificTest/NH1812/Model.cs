using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1812
{
	public class Person
	{
		public virtual int Id {get; set;}
		public virtual IList<Period> PeriodCollection { get; set; }

		public Person(){PeriodCollection=new List<Period>();}
	}

	public class Period
	{
		public virtual int Id { get; set; }
		public virtual DateTime? Start { get; set; }
	}
}