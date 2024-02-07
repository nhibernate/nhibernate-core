namespace NHibernate.Test.NHSpecificTest.GH1313
{
	public class Account
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual int OldAccountNumber { get; set; }
	}
}
