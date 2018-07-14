using System;
using NHibernate.Cfg;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Multi;
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

		protected override bool AppliesTo(ISessionFactoryImplementor factory)
		{
			// Test current implementation allows to test mmostly SQL Server. Other databases
			// tend to (validly) send InvalidOperationException during prepare phase due to the closed
			// connection, which get not converted. For testing other case, maybe a failure caused by a
			// schema mismatch (like done in transaction tests) would be better.
			return factory.ConnectionProvider.Driver is SqlClientDriver;
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

		[Test, Obsolete]
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
		public void QueryBatchShouldThrowUserException()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var multi = s.CreateQueryBatch();
				multi.Add<int>(s.CreateQuery(hqlQuery));
				s.Connection.Close();
				Assert.Throws<UnitTestException>(multi.Execute);
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

		[Test, Obsolete]
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

		[Test]
		public void CriteriaQueryBatchShouldThrowUserException()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var multi = s.CreateQueryBatch();
				multi.Add<ClassA>(s.CreateCriteria(typeof(ClassA)));
				s.Connection.Close();
				Assert.Throws<UnitTestException>(multi.Execute);
			}
		}
	}
}
