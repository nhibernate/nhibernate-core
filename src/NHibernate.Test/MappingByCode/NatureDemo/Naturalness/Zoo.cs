using System.Collections.Generic;

namespace NHibernate.Test.MappingByCode.NatureDemo.Naturalness
{
	public class Zoo
	{
		public virtual long Id { get; set; }

		public virtual string Name { get; set; }

		public virtual Classification Classification { get; set; }

		public virtual IDictionary<string, Animal> Animals { get; set; }

		public virtual IDictionary<string, Mammal> Mammals { get; set; }

		public virtual Address Address { get; set; }
	}

	public class PettingZoo : Zoo {}
}