using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH257
{
	[Serializable]
	public class UserGroup : Party
	{
		private ISet<User> _users = new HashSet<User>();

		public ISet<User> Users
		{
			get { return _users; }
			set { _users = value; }
		}
	}
}