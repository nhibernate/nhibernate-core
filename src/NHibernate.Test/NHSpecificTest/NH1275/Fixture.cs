using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1275
{
	/// <summary>
	/// http://nhibernate.jira.com/browse/NH-1275
	/// </summary>
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1275"; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return !string.IsNullOrEmpty(dialect.ForUpdateString);
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
					Assert.Less(0, sql.IndexOf(Dialect.ForUpdateString));
				}
				using (SqlLogSpy sqlLogSpy = new SqlLogSpy())
				{
					s.CreateQuery("from A a where a.Id= :pid").SetLockMode("a", LockMode.Upgrade).SetParameter("pid", savedId).
							UniqueResult<A>();
					string sql = sqlLogSpy.Appender.GetEvents()[0].RenderedMessage;
					Assert.Less(0, sql.IndexOf(Dialect.ForUpdateString));
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
					Assert.Less(0, sql.IndexOf(Dialect.ForUpdateString));
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
