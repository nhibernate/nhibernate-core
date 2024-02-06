using System;

namespace NHibernate.Test.NHSpecificTest.GH3474
{
	public interface IPayment
	{
		public Guid Id { get; set; }
		public decimal Amount { get; set; }
	}

	public class CreditCardPayment : IPayment
	{
		public virtual Guid Id { get; set; }
		public virtual decimal Amount { get; set; }
		public virtual string CreditCardType { get; set; }
	}

	public class CashPayment : IPayment
	{
		public virtual Guid Id { get; set; }
		public virtual decimal Amount { get; set; }
	}

	public class ChequePayment : IPayment
	{
		public virtual Guid Id { get; set; }
		public virtual decimal Amount { get; set; }
		public virtual string Bank { get; set; }
	}
}
