namespace NHibernate.Test.NHSpecificTest.NH3074
{
	public class Animal
	{
		public virtual int Id { get; set; }
		public virtual int Weight { get; set; }
	}

	public class Cat : Animal
	{
		public virtual int NumberOfLegs { get; set; }
	}
}