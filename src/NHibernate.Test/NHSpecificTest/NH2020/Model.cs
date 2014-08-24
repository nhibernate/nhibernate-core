namespace NHibernate.Test.NHSpecificTest.NH2020
{
	public class One
	{
		public virtual long Id { get; set; }
	}

	public class Many
	{
		public virtual long Id { get; set; }
		public virtual One One { get; set; }
	}
}
