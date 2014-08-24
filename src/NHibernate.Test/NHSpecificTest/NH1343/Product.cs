namespace NHibernate.Test.NHSpecificTest.NH1343
{
	public class Product
	{
		protected Product() {}

		public Product(string description)
		{
			Description = description;
		}

		public virtual int Id { get; set; }

		public virtual string Description { get; set; }
	}
}