using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1077
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			// Specific to MsSql2000Dialect and MsSql2005Dialect
			return dialect is MsSql2000Dialect;
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
					Assert.Less(0, sql.IndexOf("with (updlock"));
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