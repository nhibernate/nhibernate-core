using System;
using System.Collections.Generic;

namespace NHibernate.Test.VersionTest.Db
{
	public class Group
	{
		public virtual long Id { get; set; }

		public virtual DateTime Timestamp { get; set; }

		public virtual string Name { get; set; }

		public virtual ISet<User> Users { get; set; }
	}
}