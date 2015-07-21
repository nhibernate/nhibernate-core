using System;

namespace NHibernate.Test.NHSpecificTest.NH2691
{
	public abstract class Animal
	{
		public virtual int Id { get; set; }
		public virtual string Description { get; set; }
		public virtual int Sequence { get; set; }
	}

	public abstract class Reptile : Animal
	{
		public virtual double BodyTemperature { get; set; }
	}

	public class Lizard : Reptile { }

	public abstract class Mammal : Animal
	{
		public virtual bool Pregnant { get; set; }
		public virtual DateTime? BirthDate { get; set; }
	}

	public class Dog : Mammal { }

	public class Cat : Mammal { }
}