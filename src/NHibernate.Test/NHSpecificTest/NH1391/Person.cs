using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1391
{
	public class Person
	{
		public Person()
		{
			this.AnimalsGeneric = new List<Animal>();
		}
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual IList<Animal> AnimalsGeneric { get; set; }
	}
}
