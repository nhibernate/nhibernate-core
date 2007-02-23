using System;
using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH508
{
	public class User
	{
		private int userId;
		private string login;
		private IList friendList;
		private IList friendOfList;

		public User()
		{
			login = string.Empty;
			friendList = new ArrayList();
			friendOfList = new ArrayList();
		}

		public User(string login) : this()
		{
			this.login = login;
		}

		public virtual int UserId
		{
			get { return userId; }
			set { userId = value; }
		}

		public virtual string Login
		{
			get { return login; }
			set { login = value; }
		}

		public virtual IList FriendList
		{
			get { return friendList; }
			set { friendList = value; }
		}

		public virtual IList FriendOfList
		{
			get { return friendOfList; }
			set { friendOfList = value; }
		}

		public override int GetHashCode()
		{
			return 1;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}

			User other = obj as User;
			if (other == null)
			{
				return false;
			}

			return Equals(UserId, other.UserId);
		}
	}
}