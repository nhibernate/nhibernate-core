using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1270
{
	public class User
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ICollection<Role> Roles { get; set; }
	}

	public class Role
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ICollection<User> Users { get; set; }
	}
}