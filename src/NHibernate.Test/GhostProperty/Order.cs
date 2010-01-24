namespace NHibernate.Test.GhostProperty
{
	public class Order
	{
		public virtual int Id { get; set; }
		public virtual Payment Payment { get; set; }
	}

	public abstract class Payment
	{
		public virtual int Id { get; set; }
	}

	public class WireTransfer : Payment{}
	public class CreditCard : Payment { }
}