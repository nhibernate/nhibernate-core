namespace NHibernate.Test.NHSpecificTest.NH2530
{
	public abstract class Product
	{
		public virtual int Id { get; set; }
		public virtual string Title { get; set; }
	}

	public class Customer
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
	}
}