using System;
using System.Collections.Generic;

namespace NHibernate.Test.VersionTest.Db
{
	public class User
	{
		public virtual long Id { get; set; }

		public virtual DateTime Timestamp { get; set; }

		public virtual string Username { get; set; }

		public virtual ISet<Group> Groups { get; set; }

		public virtual ISet<Permission> Permissions { get; set; }
	}
}