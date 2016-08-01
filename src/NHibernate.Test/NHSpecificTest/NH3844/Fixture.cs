using System.Linq;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3844
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return !(dialect is FirebirdDialect) && !(dialect is MsSqlCeDialect);
		}

		protected override bool AppliesTo(ISessionFactoryImplementor factory)
		{
			// SQL Server seems unable to match complex group by and select list arguments when running over ODBC.";
			return !(factory.ConnectionProvider.Driver is OdbcDriver);
		}

		protected override void OnSetUp()
		{
			var job1 = new Job { Name = "Not a Job", BillingType = BillingType.None };
			var job2 = new Job { Name = "Contract Job", BillingType = BillingType.Fixed };
			var job3 = new Job { Name = "Pay as You Go Job", BillingType = BillingType.Hourly };

			var project1 = new Project { Name = "ProjectOne", Job = job1 };
			var compP1_x = new Component() { Name = "P1x", Project = project1 };
			var compP1_y = new Component() { Name = "P1y", Project = project1 };

			var project2 = new Project { Name = "ProjectTwo", Job = job2 };
			var compP2_x = new Component() { Name = "P2x", Project = project2 };
			var compP2_y = new Component() { Name = "P2y", Project = project2 };

			var project3 = new Project { Name = "ProjectThree", Job = job3 };
			var compP3_x = new Component() { Name = "P3x", Project = project3 };
			var compP3_y = new Component() { Name = "P3y", Project = project3 };

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Save(job1);
				session.Save(project1);
				session.Save(compP1_x);
				session.Save(compP1_y);
				session.Save(job2);
				session.Save(project2);
				session.Save(compP2_x);
				session.Save(compP2_y);
				session.Save(job3);
				session.Save(project3);
				session.Save(compP3_x);
				session.Save(compP3_y);


				session.Save(new TimeRecord { TimeInHours = 1, Project = project1, Components = { } });
				session.Save(new TimeRecord { TimeInHours = 2, Project = project1, Components = { compP1_x } });
				session.Save(new TimeRecord { TimeInHours = 3, Project = project1, Components = { compP1_y } });
				session.Save(new TimeRecord { TimeInHours = 4, Project = project1, Components = { compP1_x, compP1_y } });

				session.Save(new TimeRecord { TimeInHours = 5, Project = project2, Components = { } });
				session.Save(new TimeRecord { TimeInHours = 6, Project = project2, Components = { compP2_x } });
				session.Save(new TimeRecord { TimeInHours = 7, Project = project2, Components = { compP2_y } });
				session.Save(new TimeRecord { TimeInHours = 8, Project = project2, Components = { compP2_x, compP2_y } });

				session.Save(new TimeRecord { TimeInHours = 9, Project = project3, Components = { } });
				session.Save(new TimeRecord { TimeInHours = 10, Project = project3, Components = { compP3_x } });
				session.Save(new TimeRecord { TimeInHours = 11, Project = project3, Components = { compP3_y } });
				session.Save(new TimeRecord { TimeInHours = 12, Project = project3, Components = { compP3_x, compP3_y } });

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from TimeRecord");
				session.Delete("from Component");
				session.Delete("from Project");
				session.Delete("from Job");

				transaction.Commit();
			}
		}

		[Test]
		public void ConditionalGroupKeyFromArrayAccess()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var baseQuery = session.Query<TimeRecord>();

				Assert.That(baseQuery.Sum(x => x.TimeInHours), Is.EqualTo(78));

				var query = baseQuery.Select(t => new object[] { t })
					.GroupBy(j => new object[] { ((TimeRecord)j[0]).Project.Job.BillingType == BillingType.None ? 0 : 1 }, j => (TimeRecord)j[0])
					.Select(g => new object[] { g.Key, g.Count(), g.Sum(t => (decimal?)t.TimeInHours) });

				var results = query.ToList().OrderBy(x => (int)((object[])x[0])[0]);
				Assert.That(results.Select(x => x[1]), Is.EquivalentTo(new[] { 4, 8 }));
				Assert.That(results.Select(x => x[2]), Is.EquivalentTo(new[] { 10, 68 }));

				Assert.That(results.Sum(x => (decimal?)x[2]), Is.EqualTo(78));

				transaction.Rollback();
			}
		}

		[Test]
		public void ConditionalGroupKeyFromSubqueryArrayAccess()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var baseQuery = session.Query<TimeRecord>();

				Assert.That(baseQuery.Sum(x => x.TimeInHours), Is.EqualTo(78));

				var query = baseQuery.Select(t => new object[] { t })
					.SelectMany(t => ((TimeRecord)t[0]).Components.Select(c => (object)c.Id).DefaultIfEmpty().Select(c => new[] { t[0], c }))
					.GroupBy(j => new object[] { ((TimeRecord)j[0]).Project.Job.BillingType == BillingType.None ? 0 : 1 }, j => (TimeRecord)j[0])
					.Select(g => new object[] { g.Key, g.Count(), g.Sum(t => (decimal?)t.TimeInHours) });

				var results = query.ToList().OrderBy(x => (int)((object[])x[0])[0]);
				Assert.That(results.Select(x => x[1]), Is.EquivalentTo(new[] { 5, 10 }));
				Assert.That(results.Select(x => x[2]), Is.EquivalentTo(new[] { 14, 88 }));

				Assert.That(results.Sum(x => (decimal?)x[2]), Is.EqualTo(102));

				transaction.Rollback();
			}
		}

		[Test]
		public void ConditionalInComplexGroupKeyFromSubqueryArrayAccess()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var baseQuery = session.Query<TimeRecord>();

				Assert.That(baseQuery.Sum(x => x.TimeInHours), Is.EqualTo(78));

				var query = baseQuery.Select(t => new object[] { t })
					.SelectMany(t => ((TimeRecord)t[0]).Components.Select(c => (object)c.Id).DefaultIfEmpty().Select(c => new[] { t[0], c }))
					.GroupBy(j => new object[] { ((TimeRecord)j[0]).Project.Job.BillingType == BillingType.None ? 0 : 1, ((Component)j[1]).Name }, j => (TimeRecord)j[0])
					.Select(g => new object[] { g.Key, g.Count(), g.Sum(t => (decimal?)t.TimeInHours) });

				var results = query.ToList().OrderBy(x => (int)((object[])x[0])[0]).ThenBy(x => (string)((object[])x[0])[1]);
				Assert.That(results.Select(x => x[1]), Is.EquivalentTo(new[] { 1, 2, 2, 2, 2, 2, 2, 2 }));
				Assert.That(results.Select(x => x[2]), Is.EquivalentTo(new[] { 1, 6, 7, 14, 14, 15, 22, 23 }));

				Assert.That(results.Sum(x => (decimal?)x[2]), Is.EqualTo(102));

				transaction.Rollback();
			}
		}
	}
}
