using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1391
{
	public class Person
	{
		public Person()
		{
			this.AnimalsGeneric = new List<Animal>();
			this.AnimalsNonGeneric = new ArrayList();
		}
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual IList<Animal> AnimalsGeneric { get; set; }
		public virtual IList AnimalsNonGeneric { get; set; }
	}
}
