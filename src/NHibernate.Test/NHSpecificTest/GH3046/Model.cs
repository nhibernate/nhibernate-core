namespace NHibernate.Test.NHSpecificTest.GH3046
{
	public class Customer
	{
		public virtual int Id { get; set; }
		public virtual bool Deleted { get; set; }
		public virtual string Name { get; set; }
	}

	public class Person
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
	}
}
