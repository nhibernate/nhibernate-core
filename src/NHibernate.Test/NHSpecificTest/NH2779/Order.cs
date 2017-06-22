using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2779
{
	public class Order
	{
		private IList<LineItem> lineItems = new List<LineItem>();

		public virtual string OrderId { get; set; }
		public virtual int InternalOrderId { get; set; }
		public virtual IList<LineItem> LineItems { get { return lineItems; } }
	}
}
