﻿using System;
using System.Collections.Generic;

namespace NHibernate.DomainModel.Northwind.Entities
{
    public class Animal
    {
        public virtual int Id { get; set; }
        public virtual string Description { get; set; }
        public virtual double BodyWeight { get; set; }
        public virtual Animal Mother { get; set; }
        public virtual Animal Father { get; set; }
        public virtual IList<Animal> Children { get; set; }
        public virtual string SerialNumber { get; set; }

		public virtual Animal FatherOrMother => Father ?? Mother;
	}

	public interface IReptile
	{
		EnumStoredAsString Enum1 { get; }
	}

    public abstract class Reptile : Animal, IReptile
    {
        public virtual double BodyTemperature { get; set; }

		public virtual EnumStoredAsString Enum1 { get; set; }
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
