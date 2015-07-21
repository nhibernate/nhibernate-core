using System;

namespace NHibernate.Test.VersionTest.Db
{
	public class Permission
	{
		public virtual long Id { get; set; }

		public virtual DateTime Timestamp { get; set; }

		public virtual string Name { get; set; }

		public virtual string Context { get; set; }

		public virtual string Access { get; set; }
	}
}