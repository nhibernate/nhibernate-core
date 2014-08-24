using System;
using System.Collections;
using System.Threading;
using NUnit.Framework;

namespace NHibernate.Test.VersionTest.Db
{
	[TestFixture]
	public class DbVersionFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new[] {"VersionTest.Db.User.hbm.xml"}; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		[Test]
		public void CollectionVersion()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			var guy = new User { Username = "guy" };
			s.Persist(guy);
			var admin = new Group {Name = "admin"};
			s.Persist(admin);
			t.Commit();
			s.Close();

			DateTime guyTimestamp = guy.Timestamp;

			// For dialects (Oracle8 for example) which do not return "true
			// timestamps" sleep for a bit to allow the db date-time increment...
			Thread.Sleep(1500);

			s = OpenSession();
			t = s.BeginTransaction();
			guy = s.Get<User>(guy.Id);
			admin = s.Get<Group>(admin.Id);
			guy.Groups.Add(admin);
			admin.Users.Add(guy);
			t.Commit();
			s.Close();

			Assert.That(!NHibernateUtil.Timestamp.IsEqual(guyTimestamp, guy.Timestamp), "owner version not incremented");

			guyTimestamp = guy.Timestamp;
			Thread.Sleep(1500);

			s = OpenSession();
			t = s.BeginTransaction();
			guy = s.Get<User>(guy.Id);
			guy.Groups.Clear();
			t.Commit();
			s.Close();

			Assert.That(!NHibernateUtil.Timestamp.IsEqual(guyTimestamp, guy.Timestamp), "owner version not incremented");

			s = OpenSession();
			t = s.BeginTransaction();
			s.Delete(s.Load<User>(guy.Id));
			s.Delete(s.Load<Group>(admin.Id));
			t.Commit();
			s.Close();
		}

		[Test]
		public void CollectionNoVersion()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			var guy = new User {Username = "guy"};
			s.Persist(guy);
			var perm = new Permission {Name = "silly", Access = "user", Context = "rw"};
			s.Persist(perm);
			t.Commit();
			s.Close();

			DateTime guyTimestamp = guy.Timestamp;

			s = OpenSession();
			t = s.BeginTransaction();
			guy = s.Get<User>(guy.Id);
			perm = s.Get<Permission>(perm.Id);
			guy.Permissions.Add(perm);
			t.Commit();
			s.Close();

			const string ownerVersionWasIncremented = "owner version was incremented ({0:o} => {1:o})";
			Assert.That(NHibernateUtil.Timestamp.IsEqual(guyTimestamp, guy.Timestamp),
			            string.Format(ownerVersionWasIncremented, guyTimestamp, guy.Timestamp));
			Console.WriteLine(string.Format(ownerVersionWasIncremented, guyTimestamp, guy.Timestamp));

			s = OpenSession();
			t = s.BeginTransaction();
			guy = s.Get<User>(guy.Id);
			guy.Permissions.Clear();
			t.Commit();
			s.Close();

			Assert.That(NHibernateUtil.Timestamp.IsEqual(guyTimestamp, guy.Timestamp),
			            string.Format(ownerVersionWasIncremented, guyTimestamp, guy.Timestamp));

			s = OpenSession();
			t = s.BeginTransaction();
			s.Delete(s.Load<User>(guy.Id));
			s.Delete(s.Load<Permission>(perm.Id));
			t.Commit();
			s.Close();
		}
	}
}