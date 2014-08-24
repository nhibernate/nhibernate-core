using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Test.Hql.Ast
{
	public class Zoo
	{
		private long id;
		private string name;
		private Classification classification;
		private IDictionary<string, Animal> animals;
		private IDictionary<string, Mammal> mammals;
		private Address address;

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual Classification Classification
		{
			get { return classification; }
			set { classification = value; }
		}

		public virtual IDictionary<string, Animal> Animals
		{
			get { return animals; }
			set { animals = value; }
		}

		public virtual IDictionary<string, Mammal> Mammals
		{
			get { return mammals; }
			set { mammals = value; }
		}

		public virtual Address Address
		{
			get { return address; }
			set { address = value; }
		}
	}

	public class PettingZoo : Zoo { }
}