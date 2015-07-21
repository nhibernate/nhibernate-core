using System;
using System.Collections.Generic;

namespace NHibernate.Test.FilterTest
{
	public class Order
	{
		private long id;
		private String region;
		private DateTime placementDate;
		private DateTime fulfillmentDate;
		private Salesperson salesperson;
		private String buyer;
		private IList<LineItem> lineItems = new List<LineItem>();

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Region
		{
			get { return region; }
			set { region = value; }
		}

		public virtual DateTime PlacementDate
		{
			get { return placementDate; }
			set { placementDate = value; }
		}

		public virtual DateTime FulfillmentDate
		{
			get { return fulfillmentDate; }
			set { fulfillmentDate = value; }
		}

		public virtual Salesperson Salesperson
		{
			get { return salesperson; }
			set { salesperson = value; }
		}

		public virtual string Buyer
		{
			get { return buyer; }
			set { buyer = value; }
		}

		public virtual IList<LineItem> LineItems
		{
			get { return lineItems; }
			set { lineItems = value; }
		}

		public virtual LineItem AddLineItem(Product product, long quantity)
		{
			return LineItem.generate(this, LineItems.Count, product, quantity);
		}

		public virtual void RemoveLineItem(LineItem item)
		{
			RemoveLineItem(item.Sequence);
		}

		public virtual void RemoveLineItem(int sequence)
		{
			LineItems.RemoveAt(sequence);
		}
	}
}