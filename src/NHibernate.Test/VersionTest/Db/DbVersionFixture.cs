using System;
using System.Collections;
using System.Threading;
using NHibernate.Driver;
using NHibernate.Engine;
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

		protected override bool AppliesTo(ISessionFactoryImplementor factory)
		{
			// ODBC driver DateTime handling with SQL Server 2008+ Client is broken and forbids using it as a time stamp
			// generated on db-side.
			// Due to NH-3895, we have to force the scale on date-time parameters to 3 (3 significant fractional seconds digits)
			// when using ODBC + SQL Server 2008+, otherwise DateTime values having milliseconds will be rejected. But the SQL
			// Server DateTime does not have actually a one millisecond resolution (it has 3.333), causing ODBC to convert the
			// parameter to DateTime2. A DateTime value ending by 3ms (indeed 3.333) or 7ms (indeed 6.666) is
			// to be transmitted as having 3ms or 7ms and will match if transmitted as a DateTime. But when transmitted as
			// DateTime2, it will no more be considered equal, causing the test to be flaky and failing two thirds of tries.
			// Example failing update captured with profiler:
			// exec sp_executesql N'UPDATE book SET name_column = @P1 WHERE id = @P2 AND version_column = @P3',
			//     N'@P1 nvarchar(18),@P2 int,@P3 datetime2',N'modified test book',1,'2017-08-02 16:37:16.0630000'
			// Setting the scale to 2 still causes failure for two thirds of tries, due to 3ms/7ms being truncated in such case
			// with ODBC and SQL Server 2008+ Client, which is rejected bu ODBC.
			// (Affects NH-1756 too.)
			return !(factory.ConnectionProvider.Driver is OdbcDriver);
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