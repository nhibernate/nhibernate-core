using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.SqlTypes;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.Linq
{
	[TestFixture(false, false)]
	[TestFixture(true, false)]
	[TestFixture(false, true)]
	public class PreEvaluationTests : LinqTestCase
	{
		private readonly bool LegacyPreEvaluation;
		private readonly bool FallbackOnPreEvaluation;

		public PreEvaluationTests(bool legacy, bool fallback)
		{
			LegacyPreEvaluation = legacy;
			FallbackOnPreEvaluation = fallback;
		}

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);

			configuration.SetProperty(Environment.FormatSql, "false");
			configuration.SetProperty(Environment.LinqToHqlLegacyPreEvaluation, LegacyPreEvaluation.ToString());
			configuration.SetProperty(Environment.LinqToHqlFallbackOnPreEvaluation, FallbackOnPreEvaluation.ToString());
		}

		[Test]
		public void CanQueryByDateTimeNowUsingNotEqual()
		{
			var isSupported = IsFunctionSupported("current_timestamp");
			RunTest(
				isSupported,
				spy =>
				{
					var x = db.Orders.Count(o => o.OrderDate.Value != DateTime.Now);

					Assert.That(x, Is.GreaterThan(0));
					AssertFunctionInSql("current_timestamp", spy);
				});
		}

		[Test]
		public void CanQueryByDateTimeNow()
		{
			var isSupported = IsFunctionSupported("current_timestamp");
			RunTest(
				isSupported,
				spy =>
				{
					var x = db.Orders.Count(o => o.OrderDate.Value < DateTime.Now);

					Assert.That(x, Is.GreaterThan(0));
					AssertFunctionInSql("current_timestamp", spy);
				});
		}

		[Test]
		public void CanSelectDateTimeNow()
		{
			var isSupported = IsFunctionSupported("current_timestamp");
			RunTest(
				isSupported,
				spy =>
				{
					var x =
						db
							.Orders.Select(o => new { id = o.OrderId, d = DateTime.Now })
							.OrderBy(o => o.id).Take(1).ToList();

					Assert.That(x, Has.Count.GreaterThan(0));
					Assert.That(x[0].d.Kind, Is.EqualTo(DateTimeKind.Local));
					AssertFunctionInSql("current_timestamp", spy);
				});
		}

		[Test]
		public void CanQueryByDateTimeUtcNow()
		{
			var isSupported = IsFunctionSupported("current_utctimestamp");
			RunTest(
				isSupported,
				spy =>
				{
					var x = db.Orders.Count(o => o.OrderDate.Value < DateTime.UtcNow);

					Assert.That(x, Is.GreaterThan(0));
					AssertFunctionInSql("current_utctimestamp", spy);
				});
		}

		[Test]
		public void CanSelectDateTimeUtcNow()
		{
			var isSupported = IsFunctionSupported("current_utctimestamp");
			RunTest(
				isSupported,
				spy =>
				{
					var x =
						db
							.Orders.Select(o => new { id = o.OrderId, d = DateTime.UtcNow })
							.OrderBy(o => o.id).Take(1).ToList();

					Assert.That(x, Has.Count.GreaterThan(0));
					Assert.That(x[0].d.Kind, Is.EqualTo(DateTimeKind.Utc));
					AssertFunctionInSql("current_utctimestamp", spy);
				});
		}

		[Test]
		public void CanQueryByDateTimeToday()
		{
			var isSupported = IsFunctionSupported("current_date");
			RunTest(
				isSupported,
				spy =>
				{
					var x = db.Orders.Count(o => o.OrderDate.Value < DateTime.Today);

					Assert.That(x, Is.GreaterThan(0));
					AssertFunctionInSql("current_date", spy);
				});
		}

		[Test]
		public void CanSelectDateTimeToday()
		{
			var isSupported = IsFunctionSupported("current_date");
			RunTest(
				isSupported,
				spy =>
				{
					var x =
						db
							.Orders.Select(o => new { id = o.OrderId, d = DateTime.Today })
							.OrderBy(o => o.id).Take(1).ToList();

					Assert.That(x, Has.Count.GreaterThan(0));
					Assert.That(x[0].d.Kind, Is.EqualTo(DateTimeKind.Local));
					AssertFunctionInSql("current_date", spy);
				});
		}

		[Test]
		public void CanQueryByDateTimeOffsetTimeNow()
		{
			if (!TestDialect.SupportsSqlType(SqlTypeFactory.DateTimeOffSet))
				Assert.Ignore("Dialect does not support DateTimeOffSet");

			var isSupported = IsFunctionSupported("current_timestamp_offset");
			RunTest(
				isSupported,
				spy =>
				{
					var testDate = DateTimeOffset.Now.AddDays(-1);
					var x = db.Orders.Count(o => testDate < DateTimeOffset.Now);

					Assert.That(x, Is.GreaterThan(0));
					AssertFunctionInSql("current_timestamp_offset", spy);
				});
		}

		[Test]
		public void CanSelectDateTimeOffsetNow()
		{
			if (!TestDialect.SupportsSqlType(SqlTypeFactory.DateTimeOffSet))
				Assert.Ignore("Dialect does not support DateTimeOffSet");

			var isSupported = IsFunctionSupported("current_timestamp_offset");
			RunTest(
				isSupported,
				spy =>
				{
					var x =
						db
							.Orders.Select(o => new { id = o.OrderId, d = DateTimeOffset.Now })
							.OrderBy(o => o.id).Take(1).ToList();

					Assert.That(x, Has.Count.GreaterThan(0));
					Assert.That(x[0].d.Offset, Is.EqualTo(DateTimeOffset.Now.Offset));
					AssertFunctionInSql("current_timestamp_offset", spy);
				});
		}

		[Test]
		public void CanQueryByDateTimeOffsetUtcNow()
		{
			if (!TestDialect.SupportsSqlType(SqlTypeFactory.DateTimeOffSet))
				Assert.Ignore("Dialect does not support DateTimeOffSet");

			var isSupported = IsFunctionSupported("current_utctimestamp_offset");
			RunTest(
				isSupported,
				spy =>
				{
					var testDate = DateTimeOffset.UtcNow.AddDays(-1);
					var x = db.Orders.Count(o => testDate < DateTimeOffset.UtcNow);

					Assert.That(x, Is.GreaterThan(0));
					AssertFunctionInSql("current_utctimestamp_offset", spy);
				});
		}

		[Test]
		public void CanSelectDateTimeOffsetUtcNow()
		{
			if (!TestDialect.SupportsSqlType(SqlTypeFactory.DateTimeOffSet))
				Assert.Ignore("Dialect does not support DateTimeOffSet");

			var isSupported = IsFunctionSupported("current_utctimestamp_offset");
			RunTest(
				isSupported,
				spy =>
				{
					var x =
						db
							.Orders.Select(o => new { id = o.OrderId, d = DateTimeOffset.UtcNow })
							.OrderBy(o => o.id).Take(1).ToList();

					Assert.That(x, Has.Count.GreaterThan(0));
					Assert.That(x[0].d.Offset, Is.EqualTo(TimeSpan.Zero));
					AssertFunctionInSql("current_utctimestamp_offset", spy);
				});
		}

		private void RunTest(bool isSupported, Action<SqlLogSpy> test)
		{
			using (var spy = new SqlLogSpy())
			{
				try
				{
					test(spy);
				}
				catch (QueryException)
				{
					if (!isSupported && !FallbackOnPreEvaluation)
						// Expected failure
						return;
					throw;
				}
			}

			if (!isSupported && !FallbackOnPreEvaluation)
				Assert.Fail("The test should have thrown a QueryException, but has not thrown anything");
		}

		private void AssertFunctionInSql(string functionName, SqlLogSpy spy)
		{
			if (!IsFunctionSupported(functionName))
				Assert.Inconclusive($"{functionName} is not supported by the dialect");

			var function = Dialect.Functions[functionName].Render(new List<object>(), Sfi).ToString();

			if (LegacyPreEvaluation)
				Assert.That(spy.GetWholeLog(), Does.Not.Contain(function));
			else
				Assert.That(spy.GetWholeLog(), Does.Contain(function));
		}
	}
}
