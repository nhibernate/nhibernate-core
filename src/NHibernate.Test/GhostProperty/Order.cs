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
	}

	public abstract class Payment
	{
		public virtual int Id { get; set; }
	}

	public class WireTransfer : Payment{}
	public class CreditCard : Payment { }
}