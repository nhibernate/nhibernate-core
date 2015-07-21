using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1747
{
	public class Payment
	{
		private Int32 id;
		public virtual Int32 Id { get { return id; } set { id = value; } }

		private Decimal amount;
		public virtual Decimal Amount { get { return amount; } set { amount = value; } }
		private PaymentBatch paymentBatch;
		public virtual PaymentBatch PaymentBatch { get { return paymentBatch; } set { paymentBatch = value; } }
	}

	public class PaymentBatch
	{
		private Int32 id;
		public virtual Int32 Id { get { return id; } set { id = value; } }

		private IList<Payment> payments = new List<Payment>();
		public virtual IList<Payment> Payments { get { return payments; } }

		public virtual void AddPayment(Payment p)
		{
			payments.Add(p);
			p.PaymentBatch = this;
		}
	}
}
