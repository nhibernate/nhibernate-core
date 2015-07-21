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

		public Zoo()
		{
		}

		public Zoo(string name, Address address)
		{
			this.name = name;
			this.address = address;
		}

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

		public override bool Equals(object obj)
		{
			if (!(obj is Zoo))
				return false;

			var zoo = ((Zoo)obj);

			if (Name == null ^ zoo.Name == null)
			{
				return false;
			}

			if (Name != null && zoo.Name != null && !zoo.Name.Equals(Name))
			{
				return false;
			}

			if (Address == null ^ zoo.Address == null)
			{
				return false;
			}

			if (Address != null && zoo.Address != null && !zoo.Address.Equals(Address))
			{
				return false;
			}

			return true;
		}

		public override int GetHashCode()
		{
			int result = (Name != null ? Name.GetHashCode() : 0);
			result = 31 * result + (Address != null ? Address.GetHashCode() : 0);
			return result;
		}
	}

	public class PettingZoo : Zoo { }
}