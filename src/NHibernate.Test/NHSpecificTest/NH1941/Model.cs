namespace NHibernate.Test.NHSpecificTest.NH1941
{
	public enum Sex
	{
		Male,
		Female,
	}

	public class Person
	{
		public virtual int Id { get; set; }
		public virtual Sex Sex { get; set; }
	}
}
