using System;
using Iesi.Collections;

namespace NHibernate.Test.NHSpecificTest.NH295
{
	[Serializable]
	public class UserGroup : Party
	{
		private ISet _users = new HashedSet();

		public UserGroup()
		{
		}

		public ISet Users
		{
			get { return _users; }
			set { _users = value; }
		}
	}
}