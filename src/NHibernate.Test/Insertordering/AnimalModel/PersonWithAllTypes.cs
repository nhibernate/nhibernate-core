using System.Collections.Generic;

namespace NHibernate.Test.Insertordering.AnimalModel
{
	public class PersonWithAllTypes : Person
	{
		public virtual IList<Dog> DogsGeneric { get; set; }
		public virtual IList<SivasKangal> SivasKangalsGeneric { get; set; }
		public virtual IList<Cat> CatsGeneric { get; set; }
	}
}
