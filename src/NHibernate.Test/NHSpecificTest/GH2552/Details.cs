namespace NHibernate.Test.NHSpecificTest.GH2552
{
	public abstract class Details
	{
		public virtual int Id { get; protected set; }
		public virtual Person Person { get; set; }
		public virtual string Data { get; set; }
	}
	public class DetailsByFK : Details { }
	public class DetailsByRef : Details { }
}
