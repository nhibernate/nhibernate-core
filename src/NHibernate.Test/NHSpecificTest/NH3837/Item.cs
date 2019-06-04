using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3837
{
	public class Item
	{
		public virtual Guid Id { get; set; }
		public virtual int Version { get; private set; }
		public virtual string Description { get; set; }
		public virtual IList<Bid> Bids { get; set; } = new List<Bid>();

		internal void AddBid(Bid bid)
		{
			Bids.Add(bid);
		}
	}
}
