using System.Collections.Generic;

namespace NHibernate.Test.Insertordering.AnimalModel
{
	public class PersonWithCats : Person
	{
		public virtual IList<Cat> CatsGeneric { get; set; } = new List<Cat>();
	}
}
