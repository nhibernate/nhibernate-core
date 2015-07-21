using System;

namespace NHibernate.Example.Web.Domain
{
	public class Item
	{
		private int id;
		private decimal price;
		private string description;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual decimal Price
		{
			get { return price; }
			set { price = value; }
		}

		public virtual string Description
		{
			get { return description; }
			set { description = value; }
		}
	}
}