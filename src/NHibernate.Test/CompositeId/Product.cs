namespace NHibernate.Test.CompositeId
{
	public class Product
	{
		private string productId;
		private string description;
		private decimal price;
		private int numberAvailable;
		private int numberOrdered;

		public virtual string ProductId
		{
			get { return productId; }
			set { productId = value; }
		}

		public virtual string Description
		{
			get { return description; }
			set { description = value; }
		}

		public virtual decimal Price
		{
			get { return price; }
			set { price = value; }
		}

		public virtual int NumberAvailable
		{
			get { return numberAvailable; }
			set { numberAvailable = value; }
		}

		public virtual int NumberOrdered
		{
			get { return numberOrdered; }
			set { numberOrdered = value; }
		}
	}
}