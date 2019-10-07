using System;
using System.Collections.Generic;

namespace NHibernate.Test.Insertordering.AnimalModel
{
	public class PersonWithSivasKangals : Person
	{
		public virtual IList<SivasKangal> SivasKangalsGeneric { get; set; } = new List<SivasKangal>();
	}

	public class PersonWithWrongEquals : Person
	{
		public override bool Equals(object obj)
		{
			throw new ApplicationException("Equals call is unexpected.");
		}
		public override int GetHashCode()
		{
			throw new ApplicationException("GetHashCode call is unexpected.");
		}

		public virtual IList<SivasKangal> SivasKangalsGeneric { get; set; } = new List<SivasKangal>();
	}
}
