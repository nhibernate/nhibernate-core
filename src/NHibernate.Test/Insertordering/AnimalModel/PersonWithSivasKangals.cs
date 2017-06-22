using System.Collections.Generic;

namespace NHibernate.Test.Insertordering.AnimalModel
{
	public class PersonWithSivasKangals : Person
	{
		public virtual IList<SivasKangal> SivasKangalsGeneric { get; set; } = new List<SivasKangal>();
	}
}
