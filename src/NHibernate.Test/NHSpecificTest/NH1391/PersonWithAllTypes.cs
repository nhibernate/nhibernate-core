using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1391
{
	public class PersonWithAllTypes:Person
	{
		public virtual IList<Dog> DogsNonGeneric { get; set; }
		public virtual IList<SivasKangal> SivasKangalsNonGeneric { get; set; }
		public virtual IList<Cat> CatsNonGeneric { get; set; }


		public virtual IList<Dog> DogsGeneric { get; set; }
		public virtual IList<SivasKangal> SivasKangalsGeneric { get; set; }
		public virtual IList<Cat> CatsGeneric { get; set; }
	}

}
