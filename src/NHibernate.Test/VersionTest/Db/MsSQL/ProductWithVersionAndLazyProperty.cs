namespace NHibernate.Test.VersionTest.Db.MsSQL
{
	public class ProductWithVersionAndLazyProperty
	{
		byte[] _version = null;

		public virtual int Id { get; set; }

		public virtual string Summary { get; set; }

		public virtual byte[] Version { get { return _version; } }
	}
}