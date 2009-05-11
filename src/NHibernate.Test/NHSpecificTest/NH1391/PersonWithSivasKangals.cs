using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1391
{
	public class PersonWithSivasKangals:Person
	{
		public PersonWithSivasKangals()
		{
			this.SivasKangalsGeneric = new List<SivasKangal>();
			this.SivasKangalsNonGeneric = new ArrayList();
		}
		public virtual IList SivasKangalsNonGeneric { get; set; }
		public virtual IList<SivasKangal> SivasKangalsGeneric { get; set; }
	}
}
