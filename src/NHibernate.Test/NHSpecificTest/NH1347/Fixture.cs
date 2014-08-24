using log4net;
using NHibernate.Criterion;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1347
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(Fixture));

		public override string BugNumber
		{
			get { return "NH1347"; }
		}

		[Test]
		public void Bug()
		{
			if((Dialect is SQLiteDialect)==false)
				Assert.Ignore("NH-1347 is sqlite specific");

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Save(new A("1"));
				s.Save(new A("2"));
				s.Save(new A("3"));
				tx.Commit();
			}

			using(SqlLogSpy spy = new SqlLogSpy())
			using (ISession s = OpenSession())
			{
				A a = s.CreateCriteria(typeof (A))
					.AddOrder(Order.Asc("Name"))
					.SetMaxResults(1)
					.UniqueResult<A>();
				Assert.AreEqual("1", a.Name);
				Assert.IsTrue(
					spy.Appender.GetEvents()[0].MessageObject.ToString().Contains("limit")
					);
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from A");
				tx.Commit();
			}
		}
	}
}
