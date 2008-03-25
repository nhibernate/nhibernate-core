using System;
using System.Collections;
using Iesi.Collections;

namespace NHibernate.Test.UserCollection
{
	public class User
	{
		private string userName;
		private IList emailAddresses = new MyList();
		private ISet sessionData = new HashedSet();

		public string UserName
		{
			get { return userName; }
			set { userName = value; }
		}

		public IList EmailAddresses
		{
			get { return emailAddresses; }
			set { emailAddresses = value; }
		}

		public ISet SessionData
		{
			get { return sessionData; }
			set { sessionData = value; }
		}

		public User()
		{
		}

		public User(string userName)
		{
			this.userName = userName;
		}
	}
}