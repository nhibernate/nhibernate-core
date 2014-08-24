namespace NHibernate.Test.NHSpecificTest.NH2969
{
	public class Person
	{
		public virtual int ID { get; set; }
		public virtual string Name { get; set; }
	}

	public class Cat
	{
		public virtual int ID { get; set; }
		public virtual string Name { get; set; }
	}

	public class DomesticCat : Cat
	{
		public virtual Person Owner { get; set; }
	}

	public class Fish
	{
		public virtual int ID { get; set; }
		public virtual string Name { get; set; }
	}

	public class Goldfish : Fish
	{
		public virtual Person Owner { get; set; }
	}

	public class Bird
	{
		public virtual int ID { get; set; }
		public virtual string Name { get; set; }
	}

	public class Parrot : Bird
	{
		public virtual Person Pirate { get; set; }
	}
}
