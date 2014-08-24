namespace NHibernate.Test.NHSpecificTest.NH2408
{
	public class Animal
	{
		public virtual int Id { get; set; }

		public virtual string Name { get; set; }
	}

	public class Dog : Animal
	{
	}

	public class Cat : Animal
	{
	}
}
