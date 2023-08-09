using System;
using NHibernate.Cfg;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.NHSpecificTest.NH1275
{
	/// <summary>
	/// http://nhibernate.jira.com/browse/NH-1275
	/// </summary>
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return !string.IsNullOrEmpty(dialect.ForUpdateString);
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.FormatSql, "false");
		}

		[Test]
		public void Retrieving()
		{
			object savedId;
			using(ISession s = OpenSession())
			using(ITransaction t = s.BeginTransaction())
			{
				A a  = new A("hunabKu");
				savedId = s.Save(a);
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				using (SqlLogSpy sqlLogSpy = new SqlLogSpy())
				{
					s.Get<A>(savedId, LockMode.Upgrade);
					string sql = sqlLogSpy.Appender.GetEvents()[0].RenderedMessage;
					Assert.That(sql.IndexOf(Dialect.ForUpdateString, StringComparison.Ordinal), Is.GreaterThan(0));
				}
				s.Clear();
				using (SqlLogSpy sqlLogSpy = new SqlLogSpy())
				{
					s.Get<A>(typeof(A).FullName, savedId, LockMode.Upgrade);
					string sql = sqlLogSpy.Appender.GetEvents()[0].RenderedMessage;
					Assert.That(sql.IndexOf(Dialect.ForUpdateString, StringComparison.Ordinal), Is.GreaterThan(0));
				}
				using (SqlLogSpy sqlLogSpy = new SqlLogSpy())
				{
					s.CreateQuery("from A a where a.Id= :pid").SetLockMode("a", LockMode.Upgrade).SetParameter("pid", savedId).
							UniqueResult<A>();
					string sql = sqlLogSpy.Appender.GetEvents()[0].RenderedMessage;
					Assert.That(sql.IndexOf(Dialect.ForUpdateString, StringComparison.Ordinal), Is.GreaterThan(0));
				}
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Delete("from A");
				t.Commit();
			}
		}

		[Test]
		public void Loking()
		{
			object savedId;
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				A a = new A("hunabKu");
				savedId = s.Save(a);
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				A a = s.Get<A>(savedId);
				using (SqlLogSpy sqlLogSpy = new SqlLogSpy())
				{
					s.Lock(a, LockMode.Upgrade);
					string sql = sqlLogSpy.Appender.GetEvents()[0].RenderedMessage;
					Assert.That(sql.IndexOf(Dialect.ForUpdateString, StringComparison.Ordinal), Is.GreaterThan(0));
				}
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Delete("from A");
				t.Commit();
			}
		}
	}
}
