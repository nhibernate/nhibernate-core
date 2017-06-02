using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.SqlConverterAndMultiQuery
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private const string hqlQuery = "select a.Id from ClassA a";

		protected override void Configure(Configuration configuration)
		{
			configuration.DataBaseIntegration(x => x.ExceptionConverter<SqlConverter>());
		}

		[Test]
		public void NormalHqlShouldThrowUserException()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				s.Connection.Close();
				Assert.Throws<UnitTestException>(() =>
												 s.CreateQuery(hqlQuery).List());
			}
		}

		[Test]
		public void MultiHqlShouldThrowUserException()
		{
			var driver = Sfi.ConnectionProvider.Driver;
			if (!driver.SupportsMultipleQueries)
				Assert.Ignore("Driver {0} does not support multi-queries", driver.GetType().FullName);

			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var multi = s.CreateMultiQuery();
				multi.Add(hqlQuery);
				s.Connection.Close();
				Assert.Throws<UnitTestException>(() => multi.List());
			}
		}

		[Test]
		public void NormalCriteriaShouldThrowUserException()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				s.Connection.Close();
				Assert.Throws<UnitTestException>(() =>
												 s.CreateCriteria(typeof (ClassA)).List());
			}
		}

		[Test]
		public void MultiCriteriaShouldThrowUserException()
		{
			var driver = Sfi.ConnectionProvider.Driver;
			if (!driver.SupportsMultipleQueries)
				Assert.Ignore("Driver {0} does not support multi-queries", driver.GetType().FullName);

			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var multi = s.CreateMultiCriteria();
				multi.Add(s.CreateCriteria(typeof (ClassA)));
				s.Connection.Close();
				Assert.Throws<UnitTestException>(() => multi.List());
			}
		}
	}
}
