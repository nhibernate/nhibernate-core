using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH508
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH508"; }
		}

		[Test]
		public void Bug()
		{
			User friend1 = new User("friend1");
			User friend2 = new User("friend2");
			User friend3 = new User("friend3");

			// create a new user with 3 friends
			User user = new User();
			user.Login = "admin";
			user.FriendList.Add(friend2);
			user.FriendList.Add(friend1);
			user.FriendList.Add(friend3);

			object userId = null;

			using (ISession session = Sfi.OpenSession())
			using (ITransaction tran = session.BeginTransaction())
			{
				session.Save(friend1);
				session.Save(friend2);
				session.Save(friend3);
				userId = session.Save(user);
				tran.Commit();
			}

			// reload the user and remove one of the 3 friends
			using (ISession session = Sfi.OpenSession())
			using (ITransaction tran = session.BeginTransaction())
			{
				User reloadedFriend = (User) session.Load(typeof(User), friend1.UserId);
				User reloadedUser = (User) session.Load(typeof(User), userId);
				reloadedUser.FriendList.Remove(reloadedFriend);
				tran.Commit();
			}

			using (ISession session = Sfi.OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				User admin = (User) session.Get(typeof(User), userId);
				Assert.IsFalse(admin.FriendList.Contains(friend1));
				Assert.IsTrue(admin.FriendList.Contains(friend2));
				Assert.IsTrue(admin.FriendList.Contains(friend3));
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Delete("from User");
				t.Commit();
			}
		}
	}
}