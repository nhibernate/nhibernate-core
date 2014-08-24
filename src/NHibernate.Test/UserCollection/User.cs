using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Test.UserCollection
{
	public class User
	{
		private string userName;
		private IList<Email> emailAddresses = new MyList();
		private ISet<object> sessionData = new HashSet<object>();

		public string UserName
		{
			get { return userName; }
			set { userName = value; }
		}

		public IList<Email> EmailAddresses
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