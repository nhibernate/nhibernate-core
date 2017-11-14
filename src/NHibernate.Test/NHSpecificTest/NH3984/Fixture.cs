using System;
using System.Linq;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3984
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.CreateSQLQuery("delete from LogEntry").ExecuteUpdate();
				tx.Commit();
			}
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2008Dialect;
		}

		[Test]
		public void ShouldBePossibleToReadMillisecondsFromDatetime2ViaCreateSQLQuery()
		{
			var now = DateTime.UtcNow;

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var entry = new LogEntry { Text = "Test", CreatedAt = now };
				session.Save(entry);

				session.Flush();
				transaction.Commit();
			}

			using (var session = OpenSession())
			{
				var datetimeViaQuery = session
					.Query<LogEntry>()
					.Select(x => x.CreatedAt)
					.Single();

				Assert.That(datetimeViaQuery.Millisecond, Is.EqualTo(now.Millisecond));

				var datetimeViaCreateSqlQuery = session
					.CreateSQLQuery("SELECT CreatedAt FROM LogEntry")
					.List()
					.Cast<DateTime>()
					.Single();

				Assert.That(datetimeViaCreateSqlQuery.Millisecond, Is.EqualTo(now.Millisecond));
			}
		}

		[Test]
		public void ShouldBePossibleToReadTicksFromDatetime2ViaCreateSQLQuery()
		{
			var now = DateTime.Now;

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var entry = new LogEntry { Text = "Test", CreatedAt = now };
				session.Save(entry);

				session.Flush();
				transaction.Commit();
			}

			using (var session = OpenSession())
			{
				var datetimeViaQuery = session
					.Query<LogEntry>()
					.Select(x => x.CreatedAt)
					.Single();

				Assert.That(datetimeViaQuery.Ticks, Is.EqualTo(now.Ticks));

				var datetimeViaCreateSqlQuery = session
					.CreateSQLQuery("SELECT CreatedAt FROM LogEntry")
					.List()
					.Cast<DateTime>()
					.Single();

				Assert.That(datetimeViaCreateSqlQuery.Ticks, Is.EqualTo(now.Ticks));
			}
		}
	}
}
