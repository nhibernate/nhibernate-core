﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NUnit.Framework;
using NHibernate.Linq;

namespace NHibernate.Test.Linq.ByMethod
{
	using System.Threading.Tasks;
	[TestFixture]
	public class CountTestsAsync : LinqTestCase
	{
		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Cfg.Environment.ShowSql, "true");
		}

		[Test]
		public async Task CountDistinctProperty_ReturnsNumberOfDistinctEntriesForThatPropertyAsync()
		{
			if (!TestDialect.SupportsCountDistinct)
				Assert.Ignore("Dialect does not support count distinct");
			//NH-2722
			var result = await (db.Orders
				.Select(x => x.ShippingDate)
				.Distinct()
				.CountAsync());

			Assert.That(result, Is.EqualTo(387));
		}

		[Test]
		public async Task CountProperty_ReturnsNumberOfNonNullEntriesForThatPropertyAsync()
		{
			//NH-2722
			var result = await (db.Orders
				.Select(x => x.ShippingDate)
				.CountAsync());

			Assert.That(result, Is.EqualTo(809));
		}

		[Test]
		public async Task Count_ReturnsNumberOfRecordsAsync()
		{
			//NH-2722
			var result = await (db.Orders
				.CountAsync());

			Assert.That(result, Is.EqualTo(830));
		}

		[Test]
		public async Task LongCountDistinctProperty_ReturnsNumberOfDistinctEntriesForThatPropertyAsync()
		{
			if (!TestDialect.SupportsCountDistinct)
				Assert.Ignore("Dialect does not support count distinct");
			//NH-2722
			var result = await (db.Orders
				.Select(x => x.ShippingDate)
				.Distinct()
				.LongCountAsync());

			Assert.That(result, Is.EqualTo(387));
		}

		[Test]
		public async Task LongCountProperty_ReturnsNumberOfNonNullEntriesForThatPropertyAsync()
		{
			//NH-2722
			var result = await (db.Orders
				.Select(x => x.ShippingDate)
				.LongCountAsync());

			Assert.That(result, Is.EqualTo(809));
		}

		[Test]
		public async Task LongCount_ReturnsNumberOfRecordsAsync()
		{
			//NH-2722
			var result = await (db.Orders
				.LongCountAsync());

			Assert.That(result, Is.EqualTo(830));
		}

		[Test]
		public async Task CountOnJoinedGroupByAsync()
		{
			//NH-3001
			var query = from o in db.Orders
						join ol in db.OrderLines
						on o equals ol.Order
						group ol by ol.Product.ProductId
							into temp
							select new { temp.Key, count = temp.Count() };

			var result = await (query.ToListAsync());

			Assert.That(result.Count, Is.EqualTo(77));
		}

		[Test]
		public async Task CheckSqlFunctionNameLongCountAsync()
		{
			var name = Dialect is MsSql2000Dialect ? "count_big" : "count";
			using (var sqlLog = new SqlLogSpy())
			{
				var result = await (db.Orders.LongCountAsync());
				Assert.That(result, Is.EqualTo(830));

				var log = sqlLog.GetWholeLog();
				Assert.That(log, Does.Contain($"{name}("));
			}
		}

		[Test]
		public async Task CheckSqlFunctionNameForCountAsync()
		{
			using (var sqlLog = new SqlLogSpy())
			{
				var result = await (db.Orders.CountAsync());
				Assert.That(result, Is.EqualTo(830));

				var log = sqlLog.GetWholeLog();
				Assert.That(log, Does.Contain("count("));
			}
		}

		[Test]
		public async Task CheckMssqlCountCastAsync()
		{
			if (!(Dialect is MsSql2000Dialect))
			{
				Assert.Ignore();
			}

			using (var sqlLog = new SqlLogSpy())
			{
				var result = await (db.Orders.CountAsync());
				Assert.That(result, Is.EqualTo(830));

				var log = sqlLog.GetWholeLog();
				Assert.That(log, Does.Not.Contain("cast("));
			}
		}
	}
}
