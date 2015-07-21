namespace NHibernate.Test.VersionTest.Db.MsSQL
{
	public class SimpleVersioned
	{
		public virtual int Id { get; protected set; }
		public virtual byte[] LastModified { get; protected set; }
		public virtual string Something { get; set; }
	}
}