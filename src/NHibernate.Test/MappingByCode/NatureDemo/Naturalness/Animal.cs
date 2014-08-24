using System.Collections.Generic;

namespace NHibernate.Test.MappingByCode.NatureDemo.Naturalness
{
	public class Animal
	{
		public virtual long Id { get; set; }

		public virtual float BodyWeight { get; set; }

		public virtual ISet<Animal> Offspring { get; set; }

		public virtual Animal Mother { get; set; }

		public virtual Animal Father { get; set; }

		public virtual string Description { get; set; }

		public virtual Zoo Zoo { get; set; }

		public virtual string SerialNumber { get; set; }

		public virtual void AddOffspring(Animal offSpring)
		{
			if (Offspring == null)
			{
				Offspring = new HashSet<Animal>();
			}

			Offspring.Add(offSpring);
		}
	}
}