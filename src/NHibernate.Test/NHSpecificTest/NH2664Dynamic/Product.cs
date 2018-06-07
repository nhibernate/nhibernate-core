namespace NHibernate.Test.NHSpecificTest.NH2664Dynamic
{
	public class Product
	{
		public virtual string ProductId { get; set; }

		public virtual dynamic Properties { get; set; }
	}
}