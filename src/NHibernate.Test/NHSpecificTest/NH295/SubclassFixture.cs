using System.Collections;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH295
{
	[TestFixture]
	public class SubclassFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[] {"NHSpecificTest.NH295.Subclass.hbm.xml"}; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		[Test]
		public void LoadByIDFailureSameSession()
		{
			User ui1 = new User("User1");

			ISession s = OpenSession();
			s.BeginTransaction();
			object uid1 = s.Save(ui1);
			s.Transaction.Commit();
			s.Close();

			s = OpenSession();
			s.BeginTransaction();

			Assert.IsNotNull(s.Get(typeof(User), uid1));

			UserGroup ug = (UserGroup) s.Get(typeof(UserGroup), uid1);
			Assert.IsNull(ug);

			s.Transaction.Commit();
			s.Close();

			s = OpenSession();
			s.BeginTransaction();
			s.Delete("from Party");
			s.Transaction.Commit();
			s.Close();
		}

		[Test]
		public void LoadByIDFailure()
		{
			UserGroup ug1 = new UserGroup();
			ug1.Name = "Group1";
			User ui1 = new User("User1");

			ISession s = OpenSession();
			s.BeginTransaction();
			object gid1 = s.Save(ug1);
			object uid1 = s.Save(ui1);
			s.Transaction.Commit();
			s.Close();

			s = OpenSession();
			s.BeginTransaction();
			//Load user with USER NAME: 
			ICriteria criteria1 = s.CreateCriteria(typeof(User));
			criteria1.Add(Expression.Eq("Name", "User1"));
			Assert.AreEqual(1, criteria1.List().Count);
			s.Transaction.Commit();
			s.Close();

			s = OpenSession();
			s.BeginTransaction();
			//Load group with USER NAME: 
			ICriteria criteria2 = s.CreateCriteria(typeof(UserGroup));
			criteria2.Add(Expression.Eq("Name", "User1"));
			Assert.AreEqual(0, criteria2.List().Count);
			s.Transaction.Commit();
			s.Close();

			s = OpenSession();
			s.BeginTransaction();
			//Load group with GROUP NAME
			ICriteria criteria3 = s.CreateCriteria(typeof(UserGroup));
			criteria3.Add(Expression.Eq("Name", "Group1"));
			Assert.AreEqual(1, criteria3.List().Count);
			s.Transaction.Commit();
			s.Close();

			s = OpenSession();
			s.BeginTransaction();
			//Load user with GROUP NAME
			ICriteria criteria4 = s.CreateCriteria(typeof(User));
			criteria4.Add(Expression.Eq("Name", "Group1"));
			Assert.AreEqual(0, criteria4.List().Count);
			s.Transaction.Commit();
			s.Close();

			s = OpenSession();
			s.BeginTransaction();
			//Load group with USER IDENTITY
			ug1 = (UserGroup) s.Get(typeof(UserGroup), uid1);
			Assert.IsNull(ug1);
			s.Transaction.Commit();
			s.Close();

			s = OpenSession();
			s.BeginTransaction();
			ui1 = (User) s.Get(typeof(User), gid1);
			Assert.IsNull(ui1);
			s.Transaction.Commit();
			s.Close();

			s = OpenSession();
			s.BeginTransaction();
			Party p = (Party) s.Get(typeof(Party), uid1);
			Assert.IsTrue(p is User);
			p = (Party) s.Get(typeof(Party), gid1);
			Assert.IsTrue(p is UserGroup);
			s.Transaction.Commit();
			s.Close();

			s = OpenSession();
			s.BeginTransaction();
			s.Delete("from Party");
			s.Transaction.Commit();
			s.Close();
		}

		[Test]
		public void List()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				User user = new User();
				user.Name = "user";
				UserGroup group = new UserGroup();
				group.Name = "user";

				group.Users.Add(user);

				s.Save(group);
				s.Save(user);

				s.CreateCriteria(typeof(Party)).List();

				s.Delete("from Party");
				t.Commit();
			}
		}
	}
}