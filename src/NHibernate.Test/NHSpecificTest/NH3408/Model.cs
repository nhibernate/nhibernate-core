namespace NHibernate.Test.NHSpecificTest.NH3408
{
	public class Person
	{
		public virtual int Id { get; set; }

		public virtual string Name { get; set; }

		public virtual byte[] Photo { get; set; }
	}
}