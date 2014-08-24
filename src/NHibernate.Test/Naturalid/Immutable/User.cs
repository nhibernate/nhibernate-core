namespace NHibernate.Test.Naturalid.Immutable
{
	public class User
	{
		private int myUserId;
		private int version;
		private string userName;
		private string password;
		private string email;
		public User() {}

		public User(string userName, string password)
		{
			this.userName = userName;
			this.password = password;
		}

		public virtual int MyUserId
		{
			get { return myUserId; }
			set { myUserId = value; }
		}

		public virtual int Version
		{
			get { return version; }
			set { version = value; }
		}

		public virtual string UserName
		{
			get { return userName; }
			set { userName = value; }
		}

		public virtual string Password
		{
			get { return password; }
			set { password = value; }
		}

		public virtual string Email
		{
			get { return email; }
			set { email = value; }
		}
	}
}