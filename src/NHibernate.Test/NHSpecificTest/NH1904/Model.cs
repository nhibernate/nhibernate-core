using System;

namespace NHibernate.Test.NHSpecificTest.NH1904
{
	public class Invoice
	{
		public virtual int ID { get; protected set; }
		public virtual DateTime Issued { get; set; }

		protected virtual DateTime issued { get; set; }
	}

	public class InvoiceWithAddress : Invoice
	{
		public virtual Address BillingAddress { get; set; }
	}

	public struct Address
	{
		public string Line { get; set; }
		public string line { get; set; }
		public string Line2 { get; set; }
		public string City { get; set; }
		public string ZipCode { get; set; }
		public string Country { get; set; }
	}
}
