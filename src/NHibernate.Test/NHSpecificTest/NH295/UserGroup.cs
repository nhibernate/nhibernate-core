using System;
using Iesi.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH295
{
	[Serializable]
	public class UserGroup : Party
	{
		private ISet<User> _users = new HashedSet<User>();

		public UserGroup()
		{
		}

		public ISet<User> Users
		{
			get { return _users; }
			set { _users = value; }
		}
	}
}