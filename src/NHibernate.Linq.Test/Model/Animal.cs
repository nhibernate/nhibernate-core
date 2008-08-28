using System;
using System.Collections.Generic;
using Iesi.Collections.Generic;

namespace NHibernate.Linq.Test.Model
{
	public class Animal
	{
		public Animal()
		{
			this.Offspring = new HashedSet<Animal>();
		}
		public virtual int Id { get; set; }
		public virtual string Description { get; set; }
		public virtual double BodyWeight { get; set; }
		public virtual Animal Mother { get; set; }
		public virtual Animal Father { get; set; }
		public virtual Zoo Zoo { get; set; }
		public virtual string SerialNumber { get; set; }
		public virtual ISet<Animal> Offspring { get; set; }
	}

	public class Reptile : Animal
	{
		public virtual double BodyTemperature { get; set; }
	}

	public class Lizard : Reptile { }

	public class Mammal : Animal
	{
		public virtual bool Pregnant { get; set; }
		public virtual DateTime? BirthDate { get; set; }
	}

	public class DomesticAnimal : Mammal
	{
		public virtual Human Owner { get; set; }
	}

	public class Cat : DomesticAnimal { }

	public class Dog : DomesticAnimal { }

	public class Human : Mammal
	{
		public virtual HumanName Name { get; set; }
		public virtual string NickName { get; set; }
		public virtual double Height { get; set; }
		public virtual int IntValue { get; set; }
		public virtual float FloatValue { get; set; }
		public virtual decimal DecimalValue { get; set; }
		public virtual Int64 Int64Value { get; set; }
		public virtual IList<Human> Friends { get; set; }
		public virtual IDictionary<string, Human> Family { get; set; }
		public virtual IList<DomesticAnimal> Pets { get; set; }
		public virtual ISet<string> NickNames { get; set; }
		public virtual IDictionary<string, Address> Addresses { get; set; }
	}

	public struct Address
	{
		public string Street { get; set; }
		public string City { get; set; }
		public string PostalCode { get; set; }
		public string Country { get; set; }
		public StateProvince StateProvince { get; set; }
	}

	public struct HumanName
	{
		public string First { get; set; }
		public string Initial { get; set; }
		public string Last { get; set; }
	}

	public class User
	{
		public virtual int Id { get; set; }
		public virtual Human Human { get; set; }
		public virtual string UserName { get; set; }
		public virtual IList<string> Permissions { get; set; }
	}

	public class Zoo
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ClassificationType Classification { get; set; }
		public virtual IDictionary<string, Mammal> Mammals { get; set; }
		public virtual IDictionary<string, Animal> Animals { get; set; }
		public virtual Address Address { get; set; }
	}

	public enum ClassificationType { Large, Medium, Small }

	public class PettingZoo : Zoo { }

	public class StateProvince
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string IsoCode { get; set; }
	}

	public class Joiner
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string JoinedName { get; set; }
	}
}
