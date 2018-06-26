using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH1760
{
	class User
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}

	class Group
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }

		public virtual IDictionary<string, User> UsersByName { get; set; } = new Dictionary<string, User>();
	}
}
