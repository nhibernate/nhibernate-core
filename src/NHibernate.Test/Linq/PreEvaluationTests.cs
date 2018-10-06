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

		[Test]
		public void CanQueryByNewGuid()
		{
			if (!TestDialect.SupportsSqlType(SqlTypeFactory.Guid))
				Assert.Ignore("Guid are not supported by the target database");

			var isSupported = IsFunctionSupported("new_uuid");
			RunTest(
				isSupported,
				spy =>
				{
					var guid = Guid.NewGuid();
					var x = db.Orders.Count(o => guid != Guid.NewGuid());

					Assert.That(x, Is.GreaterThan(0));
					AssertFunctionInSql("new_uuid", spy);
				});
		}

		[Test]
		public void CanSelectNewGuid()
		{
			if (!TestDialect.SupportsSqlType(SqlTypeFactory.Guid))
				Assert.Ignore("Guid are not supported by the target database");

			var isSupported = IsFunctionSupported("new_uuid");
			RunTest(
				isSupported,
				spy =>
				{
					var x =
						db
							.Orders.Select(o => new { id = o.OrderId, g = Guid.NewGuid() })
							.OrderBy(o => o.id).Take(1).ToList();

					Assert.That(x, Has.Count.GreaterThan(0));
					AssertFunctionInSql("new_uuid", spy);
				});
		}

		[Test]
		public void CanQueryByRandomDouble()
		{
			var isSupported = IsFunctionSupported("random");
			RunTest(
				isSupported,
				spy =>
				{
					var random = new Random();
					var x = db.Orders.Count(o => o.OrderId > random.NextDouble());

					Assert.That(x, Is.GreaterThan(0));
					AssertFunctionInSql("random", spy);
				});
		}

		[Test]
		public void CanSelectRandomDouble()
		{
			var isSupported = IsFunctionSupported("random");
			RunTest(
				isSupported,
				spy =>
				{
					var random = new Random();
					var x =
						db
							.Orders.Select(o => new { id = o.OrderId, r = random.NextDouble() })
							.OrderBy(o => o.id).ToList();

					Assert.That(x, Has.Count.GreaterThan(0));
					var randomValues = x.Select(o => o.r).Distinct().ToArray();
					Assert.That(randomValues, Has.All.GreaterThanOrEqualTo(0).And.LessThan(1));

					if (!LegacyPreEvaluation && IsFunctionSupported("random"))
					{
						// Naïve randomness check
						Assert.That(
							randomValues,
							Has.Length.GreaterThan(x.Count / 2),
							"Generated values do not seem very random");
					}

					AssertFunctionInSql("random", spy);
				});
		}

		[Test]
		public void CanQueryByRandomInt()
		{
			var isSupported = IsFunctionSupported("random") && IsFunctionSupported("floor");
			var idMin = db.Orders.Min(o => o.OrderId);
			RunTest(
				isSupported,
				spy =>
				{
					var random = new Random();
					// Dodge a Firebird driver limitation by putting the constants before the order id.
					// This driver cast parameters to their types in some cases for avoiding Firebird complaining of not
					// knowing the type of the condition. For some reasons the driver considers the casting should not be
					// done next to the conditional operator. Having the cast only on one side is enough for avoiding
					// Firebird complain, so moving the constants on the left side have been put before the order id, in
					// order for these constants to be casted by the driver.
					var x = db.Orders.Count(o => -idMin - 1 + o.OrderId < random.Next());

					Assert.That(x, Is.GreaterThan(0));
					// Next requires support of both floor and rand
					AssertFunctionInSql(IsFunctionSupported("floor") ? "random" : "floor", spy);
				});
		}

		[Test]
		public void CanSelectRandomInt()
		{
			var isSupported = IsFunctionSupported("random") && IsFunctionSupported("floor");
			RunTest(
				isSupported,
				spy =>
				{
					var random = new Random();
					var x =
						db
							.Orders.Select(o => new { id = o.OrderId, r = random.Next() })
							.OrderBy(o => o.id).ToList();

					Assert.That(x, Has.Count.GreaterThan(0));
					var randomValues = x.Select(o => o.r).Distinct().ToArray();
					Assert.That(
						randomValues,
						Has.All.GreaterThanOrEqualTo(0).And.LessThan(int.MaxValue).And.TypeOf<int>());

					if (!LegacyPreEvaluation && IsFunctionSupported("random") && IsFunctionSupported("floor"))
					{
						// Naïve randomness check
						Assert.That(
							randomValues,
							Has.Length.GreaterThan(x.Count / 2),
							"Generated values do not seem very random");
					}

					// Next requires support of both floor and rand
					AssertFunctionInSql(IsFunctionSupported("floor") ? "random" : "floor", spy);
				});
		}

		[Test]
		public void CanQueryByRandomIntWithMax()
		{
			var isSupported = IsFunctionSupported("random") && IsFunctionSupported("floor");
			var idMin = db.Orders.Min(o => o.OrderId);
			RunTest(
				isSupported,
				spy =>
				{
					var random = new Random();
					// Dodge a Firebird driver limitation by putting the constants before the order id.
					// This driver cast parameters to their types in some cases for avoiding Firebird complaining of not
					// knowing the type of the condition. For some reasons the driver considers the casting should not be
					// done next to the conditional operator. Having the cast only on one side is enough for avoiding
					// Firebird complain, so moving the constants on the left side have been put before the order id, in
					// order for these constants to be casted by the driver.
					var x = db.Orders.Count(o => -idMin + o.OrderId <= random.Next(10));

					Assert.That(x, Is.GreaterThan(0).And.LessThan(11));
					// Next requires support of both floor and rand
					AssertFunctionInSql(IsFunctionSupported("floor") ? "random" : "floor", spy);
				});
		}

		[Test]
		public void CanSelectRandomIntWithMax()
		{
			var isSupported = IsFunctionSupported("random") && IsFunctionSupported("floor");
			RunTest(
				isSupported,
				spy =>
				{
					var random = new Random();
					var x =
						db
							.Orders.Select(o => new { id = o.OrderId, r = random.Next(10) })
							.OrderBy(o => o.id).ToList();

					Assert.That(x, Has.Count.GreaterThan(0));
					var randomValues = x.Select(o => o.r).Distinct().ToArray();
					Assert.That(randomValues, Has.All.GreaterThanOrEqualTo(0).And.LessThan(10).And.TypeOf<int>());

					if (!LegacyPreEvaluation && IsFunctionSupported("random") && IsFunctionSupported("floor"))
					{
						// Naïve randomness check
						Assert.That(
							randomValues,
							Has.Length.GreaterThan(Math.Min(10, x.Count) / 2),
							"Generated values do not seem very random");
					}

					// Next requires support of both floor and rand
					AssertFunctionInSql(IsFunctionSupported("floor") ? "random" : "floor", spy);
				});
		}

		[Test]
		public void CanQueryByRandomIntWithMinMax()
		{
			var isSupported = IsFunctionSupported("random") && IsFunctionSupported("floor");
			var idMin = db.Orders.Min(o => o.OrderId);
			RunTest(
				isSupported,
				spy =>
				{
					var random = new Random();
					// Dodge a Firebird driver limitation by putting the constants before the order id.
					// This driver cast parameters to their types in some cases for avoiding Firebird complaining of not
					// knowing the type of the condition. For some reasons the driver considers the casting should not be
					// done next to the conditional operator. Having the cast only on one side is enough for avoiding
					// Firebird complain, so moving the constants on the left side have been put before the order id, in
					// order for these constants to be casted by the driver.
					var x = db.Orders.Count(o => -idMin + o.OrderId < random.Next(1, 10));

					Assert.That(x, Is.GreaterThan(0).And.LessThan(10));
					// Next requires support of both floor and rand
					AssertFunctionInSql(IsFunctionSupported("floor") ? "random" : "floor", spy);
				});
		}

		[Test]
		public void CanSelectRandomIntWithMinMax()
		{
			var isSupported = IsFunctionSupported("random") && IsFunctionSupported("floor");
			RunTest(
				isSupported,
				spy =>
				{
					var random = new Random();
					var x =
						db
							.Orders.Select(o => new { id = o.OrderId, r = random.Next(1, 11) })
							.OrderBy(o => o.id).ToList();

					Assert.That(x, Has.Count.GreaterThan(0));
					var randomValues = x.Select(o => o.r).Distinct().ToArray();
					Assert.That(randomValues, Has.All.GreaterThanOrEqualTo(1).And.LessThan(11).And.TypeOf<int>());

					if (!LegacyPreEvaluation && IsFunctionSupported("random") && IsFunctionSupported("floor"))
					{
						// Naïve randomness check
						Assert.That(
							randomValues,
							Has.Length.GreaterThan(Math.Min(10, x.Count) / 2),
							"Generated values do not seem very random");
					}

					// Next requires support of both floor and rand
					AssertFunctionInSql(IsFunctionSupported("floor") ? "random" : "floor", spy);
				});
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
