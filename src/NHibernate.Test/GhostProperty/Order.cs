namespace NHibernate.Test.GhostProperty
{
	public class Order
	{
		public virtual int Id { get; set; }
		private Payment payment;

		public virtual Payment Payment
		{
			get { return payment; }
			set { payment = value; }
		}

		public virtual string ALazyProperty { get; set; }
		public virtual string NoLazyProperty { get; set; }
	}

	public abstract class Payment
	{
		public virtual int Id { get; set; }
	}

	public class WireTransfer : Payment{}
	public class CreditCard : Payment { }
}