using System;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class TransactionHistoryArchive
	{
		public virtual int TransactionId { get; set; }
		public virtual decimal ActualCost { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual int ProductId { get; set; }
		public virtual int Quantity { get; set; }
		public virtual int ReferenceOrderId { get; set; }
		public virtual int ReferenceOrderLineId { get; set; }
		public virtual DateTime TransactionDate { get; set; }
		public virtual string TransactionType { get; set; }
	}
}
