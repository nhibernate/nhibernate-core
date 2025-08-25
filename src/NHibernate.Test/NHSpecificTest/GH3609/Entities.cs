using System;

namespace NHibernate.Test.NHSpecificTest.GH3609
{
	public class Order
	{
		public virtual long Id { get; set; }

		public virtual string UniqueId { get; set; } = Guid.NewGuid().ToString();

		public virtual DateTime CreatedDate { get; set; }
	}

	public class LineItem
	{
		public virtual long Id { get; set; }

		public virtual Order Order { get; set; }

		public virtual string ItemName { get; set; }

		public virtual decimal Amount { get; set; }
	}

	public class CleanLineItem
	{
		public virtual long Id { get; set; }

		public virtual Order Order { get; set; }

		public virtual string ItemName { get; set; }

		public virtual decimal Amount { get; set; }
	}
}
