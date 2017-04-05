using System;
using System.Collections.Generic;

namespace NHibernate.Test.Insertordering.AnimalModel
{
	public class Person
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual IList<Animal> AnimalsGeneric { get; set; } = new List<Animal>();
	}
}
