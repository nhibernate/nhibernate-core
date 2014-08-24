using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2760
{
	public class User
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string Other { get; set; }

		public virtual ICollection<UserGroup> UserGroups { get; set; }

		public User()
		{
			UserGroups = new HashSet<UserGroup>();
		}
	}

	public class UserGroup
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string Other { get; set; }

		public virtual IEnumerable<User> Users { get; set; }

		public UserGroup()
		{
			Users = new List<User>();
		}
	}
}