namespace NHibernate.Test.NHSpecificTest.NH1408
{
	public abstract class Entity
	{
		private long oid;
		private int version;

		public virtual long Oid
		{
			get { return oid; }
			set { oid = value; }
		}

		public virtual int Version
		{
			get { return version; }
			set { version = value; }
		}
	}
}