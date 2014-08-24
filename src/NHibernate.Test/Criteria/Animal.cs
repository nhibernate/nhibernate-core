using System;
using System.Collections.Generic;

namespace NHibernate.Test.Criteria
{
	public class Animal
	{
		private long id;

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		private float bodyWeight;

		public virtual float BodyWeight
		{
			get { return bodyWeight; }
			set { bodyWeight = value; }
		}

		private ISet<Animal> offspring;

		public virtual ISet<Animal> Offspring
		{
			get { return offspring; }
			set { offspring = value; }
		}

		private Animal mother;

		public virtual Animal Mother
		{
			get { return mother; }
			set { mother = value; }
		}

		private Animal father;

		public virtual Animal Father
		{
			get { return father; }
			set { father = value; }
		}

		private string description;

		public virtual string Description
		{
			get { return description; }
			set { description = value; }
		}

		private string serialNumber;

		public virtual string SerialNumber
		{
			get { return serialNumber; }
			set { serialNumber = value; }
		}

		private bool howHappyIsHe;

		public virtual bool HowHappyIsHe
		{
			get { return howHappyIsHe; }
			set { howHappyIsHe = value; }
		}

		public Animal()
		{
		}

		public Animal(string description, float bodyWeight)
		{
			this.description = description;
			this.bodyWeight = bodyWeight;
		}

		public virtual void AddOffspring(Animal offspring)
		{
			if (this.Offspring == null)
			{
				this.Offspring = new HashSet<Animal>();
			}
			this.Offspring.Add(offspring);
		}
	}
}
