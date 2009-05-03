using System.Collections;

namespace NHibernate.Test.HQL.Ast
{
	public class Zoo
	{
		private long id;
		private string name;
		private Classification classification;
		private IDictionary animals;
		private IDictionary mammals;
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

		public virtual IDictionary Animals
		{
			get { return animals; }
			set { animals = value; }
		}

		public virtual IDictionary Mammals
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