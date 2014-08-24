namespace NHibernate.Test.NHSpecificTest.NH1693
{
	public class Invoice
	{
		public virtual int ID { get; protected set; }
		public virtual string Mode { get; set; }
		public virtual int Category { get; set; }
		public virtual int Num { get; set; }
	}
}
