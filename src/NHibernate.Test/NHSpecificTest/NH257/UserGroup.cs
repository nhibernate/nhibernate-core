using System;
using System.Collections;
using Iesi.Collections;

namespace NHibernate.Test.NHSpecificTest.NH257
{
	[Serializable]
	public class UserGroup : Party
	{
		private ISet _users = new HashedSet();

		public ISet Users
		{
			get { return _users; }
			set { _users = value; }
		}
	}
}
