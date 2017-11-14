using System.Collections.Generic;

namespace NHibernate.Test.Insertordering.AnimalModel
{
	public class PersonWithDogs : Person
	{
		public virtual IList<Dog> DogsGeneric { get; set; } = new List<Dog>();
	}
}
