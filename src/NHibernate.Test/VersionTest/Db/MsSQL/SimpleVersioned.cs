namespace NHibernate.Test.VersionTest.Db.MsSQL
{
	public class SimpleVersioned
	{
		public virtual int Id { get; private set; }
		public virtual byte[] LastModified { get; private set; }
		public virtual string Something { get; set; }
	}
}