using System.Collections.Generic;

namespace NHibernate.Test.Hql.Ast
{
	public class Animal
	{
		private long id;
		private float bodyWeight;
		private ISet<Animal> offspring;
		private Animal mother;
		private Animal father;
		private string description;
		private Zoo zoo;
		private string serialNumber;

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual float BodyWeight
		{
			get { return bodyWeight; }
			set { bodyWeight = value; }
		}

		public virtual ISet<Animal> Offspring
		{
			get { return offspring; }
			set { offspring = value; }
		}

		public virtual Animal Mother
		{
			get { return mother; }
			set { mother = value; }
		}

		public virtual Animal Father
		{
			get { return father; }
			set { father = value; }
		}

		public virtual string Description
		{
			get { return description; }
			set { description = value; }
		}

		public virtual Zoo Zoo
		{
			get { return zoo; }
			set { zoo = value; }
		}

		public virtual string SerialNumber
		{
			get { return serialNumber; }
			set { serialNumber = value; }
		}

		public virtual void AddOffspring(Animal offSpring)
		{
			if (offspring == null)
			{
				offspring = new HashSet<Animal>();
			}

			offspring.Add(offSpring);
		}
	}
}