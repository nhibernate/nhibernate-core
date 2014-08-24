using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1391
{
	public class PersonWithDogs : Person
	{
		public PersonWithDogs()
		{
			this.DogsGeneric = new List<Dog>();
		}
		public virtual IList<Dog> DogsGeneric { get; set; }
	}
}
