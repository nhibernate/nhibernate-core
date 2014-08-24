namespace NHibernate.Test.NHSpecificTest.NH2856
{
	public class Person
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Address Address { get; set; }
	}

	public class Address
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
	}
}