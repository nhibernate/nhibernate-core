using System.Collections.Generic;

namespace NHibernate.Test.MappingByCode.IntegrationTests.NH2728
{
	public class Toy
	{
		public Toy()
		{
			Animals = new HashSet<IAnimal>();
		}

		public virtual int Id { get; set; }
		public virtual string Name { get; set; }

		public virtual ICollection<IAnimal> Animals { get; protected set; }
	}
}
