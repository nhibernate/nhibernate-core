using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1297
{
	public class Model
	{
		private int id;
		private string name;
		private IList<Item> items = new List<Item>();

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual IList<Item> Items
		{
			get { return items; }
			set { items = value; }
		}

		public virtual void AddItem(Item item)
		{
			this.items.Add(item);
		}
	}

	public class Item
	{
		private string name;
		private int quantity;

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual int Quantity
		{
			get { return quantity; }
			set { quantity = value; }
		}
	}
}