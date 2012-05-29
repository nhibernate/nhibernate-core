using System;
using System.Collections;
using Iesi.Collections.Generic;

namespace NHibernate.Test.UserCollection
{
	public class User
	{
		private string userName;
		private IList emailAddresses = new MyList();
		private ISet<object> sessionData = new HashedSet<object>();

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

		public ISet<object> SessionData
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