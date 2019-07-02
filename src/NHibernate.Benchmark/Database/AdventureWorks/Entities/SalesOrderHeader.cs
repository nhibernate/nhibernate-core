using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class SalesOrderHeader
	{
		public virtual int Id { get; set; }
		public virtual string AccountNumber { get; set; }
		public virtual string Comment { get; set; }
		public virtual string CreditCardApprovalCode { get; set; }
		public virtual DateTime DueDate { get; set; }
		public virtual decimal Freight { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual bool OnlineOrderFlag { get; set; }
		public virtual DateTime OrderDate { get; set; }
		public virtual string PurchaseOrderNumber { get; set; }
		public virtual byte RevisionNumber { get; set; }
		public virtual Guid RowGuid { get; set; }
		public virtual string SalesOrderNumber { get; set; }
		public virtual DateTime? ShipDate { get; set; }
		public virtual byte Status { get; set; }
		public virtual decimal SubTotal { get; set; }
		public virtual decimal TaxAmt { get; set; }
		public virtual decimal TotalDue { get; set; }

		public virtual ICollection<SalesOrderDetail> SalesOrderDetail { get; set; } = new HashSet<SalesOrderDetail>();
		public virtual ICollection<SalesOrderHeaderSalesReason> SalesOrderHeaderSalesReason { get; set; } = new HashSet<SalesOrderHeaderSalesReason>();
		public virtual Address BillToAddress { get; set; }
		public virtual CreditCard CreditCard { get; set; }
		public virtual CurrencyRate CurrencyRate { get; set; }
		public virtual Customer Customer { get; set; }
		public virtual SalesPerson SalesPerson { get; set; }
		public virtual ShipMethod ShipMethod { get; set; }
		public virtual Address ShipToAddress { get; set; }
		public virtual SalesTerritory Territory { get; set; }
	}
}
