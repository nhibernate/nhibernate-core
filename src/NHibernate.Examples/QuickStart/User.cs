using System;

namespace NHibernate.Examples.QuickStart
{
	/// <summary>
	/// Summary description for User.
	/// </summary>
	public class User
	{
		private string id;
		private string userName;
		private string password;
		private string emailAddress;
		private DateTime lastLogon;


		public User()
		{
		}

		public string Id
		{
			get { return id; }
			set { id = value; }
		}

		public string UserName
		{
			get { return userName; }
			set { userName = value; }
		}

		public string Password
		{
			get { return password; }
			set { password = value; }
		}

		public string EmailAddress
		{
			get { return emailAddress; }
			set { emailAddress = value; }
		}

		public DateTime LastLogon
		{
			get { return lastLogon; }
			set { lastLogon = value; }
		}
	}
}