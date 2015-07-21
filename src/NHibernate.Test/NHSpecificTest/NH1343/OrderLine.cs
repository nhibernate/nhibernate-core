namespace NHibernate.Test.NHSpecificTest.NH1343
{
	public class OrderLine
	{
		protected OrderLine() {}

		public OrderLine(string description, Product product)
		{
			Description = description;
			Product = product;
		}

		public virtual int Id { get; protected set; }

		public virtual string Description { get; set; }

		public virtual Product Product { get; set; }
	}
}