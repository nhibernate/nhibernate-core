using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1391
{
	public class PersonWithCats : Person
	{
		public PersonWithCats()
		{
			this.CatsGeneric = new List<Cat>();
			this.CatsNonGeneric = new ArrayList();
		}
		public virtual IList CatsNonGeneric { get; set; }
		public virtual IList<Cat> CatsGeneric { get; set; }
	}
}
