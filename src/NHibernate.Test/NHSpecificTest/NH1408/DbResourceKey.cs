namespace NHibernate.Test.NHSpecificTest.NH1408
{
	public class DbResourceKey : Entity
	{
		private readonly object[] keys = new object[2];
		private DbResource resource;

		protected DbResourceKey() {}

		public DbResourceKey(string resourceId) : this(resourceId, null) {}

		public DbResourceKey(string resourceId, string language)
		{
			keys[0] = resourceId;
			keys[1] = language;
		}

		// used just for hibernate mappings
		public virtual object Key0
		{
			get { return keys[0]; }
			set { keys[0] = value; }
		}

		public virtual object Key1
		{
			get { return keys[1]; }
			set { keys[1] = value; }
		}

		public virtual DbResource Resource
		{
			get { return resource; }
			protected internal set { resource = value; }
		}
	}
}