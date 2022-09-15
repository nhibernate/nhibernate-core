namespace NHibernate.Test.NHSpecificTest.GH2201
{
	public class Detail
	{
		public virtual int Id { get; protected set; }
		public virtual Person Person { get; set; }
		public virtual string Data { get; set; }
	}
}
