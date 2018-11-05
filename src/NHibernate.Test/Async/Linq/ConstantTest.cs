﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Engine.Query;
using NHibernate.Linq.Visitors;
using NHibernate.Util;
using NUnit.Framework;
using NHibernate.Linq;

namespace NHibernate.Test.Linq
{
	using System.Threading.Tasks;
	// Mainly adapted from tests contributed by Nicola Tuveri on NH-2500 (NH-2500.patch file)
	[TestFixture]
	public class ConstantTestAsync : LinqTestCase
	{
		[Test]
		[Ignore("Linq query not supported yet")]
		public async Task ConstantNonCachedAsync()
		{
			var c1 = await ((from c in db.Customers
			          select "customer1").FirstAsync());

			var c2 = await ((from c in db.Customers
			          select "customer2").FirstAsync());

			Assert.That(c1, Is.EqualTo("customer1"));
			Assert.That(c2, Is.EqualTo("customer2"));
		}

		[Test]
		public async Task ConstantNonCachedInAnonymousNewExpressionAsync()
		{
			var c1 = await ((from c in db.Customers
			          where c.CustomerId == "ALFKI"
			          select new { c.CustomerId, c.ContactName, Constant = 1 }).FirstAsync());

			var c2 = await ((from c in db.Customers
			          where c.CustomerId == "ANATR"
			          select new { c.CustomerId, c.ContactName, Constant = 2 }).FirstAsync());

			Assert.That(c1.Constant, Is.EqualTo(1), "c1.Constant");
			Assert.That(c2.Constant, Is.EqualTo(2), "c2.Constant");
			Assert.That(c1.CustomerId, Is.EqualTo("ALFKI"), "c1.CustomerId");
			Assert.That(c2.CustomerId, Is.EqualTo("ANATR"), "c2.CustomerId");
		}

		[Test]
		public async Task ConstantNonCachedInNestedAnonymousNewExpressionsAsync()
		{
			var c1 = await ((from c in db.Customers
			          select new
			          {
				          c.ContactName,
				          Number = 1,
				          Customer = new { c.CustomerId, Label = "customer1" }
			          }).FirstAsync());

			var c2 = await ((from c in db.Customers
			          select new
			          {
				          c.ContactName,
				          Number = 2,
				          Customer = new { c.CustomerId, Label = "customer2" }
			          }).FirstAsync());

			Assert.That(c1.Number, Is.EqualTo(1), "c1.Number");
			Assert.That(c1.Customer.Label, Is.EqualTo("customer1"), "c1.Customer.Label");
			Assert.That(c2.Number, Is.EqualTo(2), "c1.Number");
			Assert.That(c2.Customer.Label, Is.EqualTo("customer2"), "c2.Customer.Label");
		}

		[Test]
		public async Task ConstantNonCachedInNewExpressionAsync()
		{
			var c1 = await ((from c in db.Customers
			          where c.CustomerId == "ALFKI"
			          select new KeyValuePair<string, string>(c.ContactName, "one")).FirstAsync());

			var c2 = await ((from c in db.Customers
			          where c.CustomerId == "ANATR"
			          select new KeyValuePair<string, string>(c.ContactName, "two")).FirstAsync());

			Assert.That(c1.Value, Is.EqualTo("one"), "c1.Value");
			Assert.That(c2.Value, Is.EqualTo("two"), "c2.Value");
		}

		public class ShipperDto
		{
			public int Number { get; set; }
			public string CompanyName { get; set; }
			public string Name { get; set; }
		}

		[Test]
		public async Task ConstantNonCachedInMemberInitExpressionAsync()
		{
			var s1 = await ((from s in db.Shippers
			          select new ShipperDto
			          {
				          Number = 1,
				          CompanyName = s.CompanyName,
				          Name = "shipper1"
			          }).ToListAsync());

			var s2 = await ((from s in db.Shippers
			          select new ShipperDto
			          {
				          Number = 2,
				          CompanyName = s.CompanyName,
				          Name = "shipper2"
			          }).ToListAsync());

			Assert.That(s1, Has.Count.GreaterThan(0), "s1 Count");
			Assert.That(s2, Has.Count.GreaterThan(0), "s2 Count");
			Assert.That(s1, Has.All.Property("Number").EqualTo(1), "s1 Numbers");
			Assert.That(s1, Has.All.Property("Name").EqualTo("shipper1"), "s1 Names");
			Assert.That(s2, Has.All.Property("Number").EqualTo(2), "s2 Numbers");
			Assert.That(s2, Has.All.Property("Name").EqualTo("shipper2"), "s2 Names");
		}

		[Test]
		public async Task ConstantInNewArrayExpressionAsync()
		{
			var c1 = await ((from c in db.Categories
			          select new[] { c.Name, "category1" }).ToListAsync());

			var c2 = await ((from c in db.Categories
			          select new[] { c.Name, "category2" }).ToListAsync());

			Assert.That(c1, Has.Count.GreaterThan(0), "c1 Count");
			Assert.That(c2, Has.Count.GreaterThan(0), "c2 Count");
			Assert.That(c1.All(c => c[1] == "category1"), Is.True, "c1 second item");
			Assert.That(c2.All(c => c[1] == "category2"), Is.True, "c2 second item");
		}

		[Test]
		public async Task ConstantsInNewArrayExpressionAsync()
		{
			var p1 = await ((from p in db.Products
			          select new Dictionary<string, int>()
			          {
				          { p.Name, 1 },
				          { "product1", p.ProductId }
			          }).FirstAsync());

			var p2 = await ((from p in db.Products
			          select new Dictionary<string, int>()
			          {
				          { p.Name, 2 },
				          { "product2", p.ProductId }
			          }).FirstAsync());

			Assert.That(p1.ElementAt(0).Value == 1 && p1.ElementAt(1).Key == "product1", Is.True, "p1");
			Assert.That(p2.ElementAt(0).Value == 2 && p2.ElementAt(1).Key == "product2", Is.True, "p2");
		}

		public class InfoBuilder
		{
			private readonly int _value;

			public InfoBuilder(int value)
			{
				_value = value;
			}

			public int GetItemValue(Product p)
			{
				return _value;
			}
		}

		// Adapted from NH-2500 first test case by Andrey Titov (file NHTest3.zip)
		[Test]
		public async Task ObjectConstantsAsync()
		{
			var builder = new InfoBuilder(1);
			var v1 = await ((from p in db.Products
			          select builder.GetItemValue(p)).FirstAsync());
			builder = new InfoBuilder(2);
			var v2 = await ((from p in db.Products
			          select builder.GetItemValue(p)).FirstAsync());

			Assert.That(v1, Is.EqualTo(1), "v1");
			Assert.That(v2, Is.EqualTo(2), "v2");
		}

		private int TestFunc(Product item, int closureValue)
		{
			return closureValue;
		}

		// Adapted from NH-3673
		[Test]
		public async Task ConstantsInFuncCallAsync()
		{
			var closureVariable = 1;
			var v1 = await ((from p in db.Products
			          select TestFunc(p, closureVariable)).FirstAsync());
			closureVariable = 2;
			var v2 = await ((from p in db.Products
			          select TestFunc(p, closureVariable)).FirstAsync());

			Assert.That(v1, Is.EqualTo(1), "v1");
			Assert.That(v2, Is.EqualTo(2), "v2");
		}

		[Test]
		public async Task PlansAreCachedAsync()
		{
			var queryPlanCacheType = typeof(QueryPlanCache);

			var cache = (SoftLimitMRUCache)
				queryPlanCacheType
					.GetField("planCache", BindingFlags.Instance | BindingFlags.NonPublic)
					.GetValue(Sfi.QueryPlanCache);
			cache.Clear();

			await ((from c in db.Customers
			 where c.CustomerId == "ALFKI"
			 select new { c.CustomerId, c.ContactName }).FirstAsync());
			Assert.That(
				cache,
				Has.Count.EqualTo(1),
				"First query plan should be cached.");

			using (var spy = new LogSpy(queryPlanCacheType))
			{
				// Should hit plan cache.
				await ((from c in db.Customers
				 where c.CustomerId == "ANATR"
				 select new { c.CustomerId, c.ContactName }).FirstAsync());
				Assert.That(cache, Has.Count.EqualTo(1), "Second query should not cause a plan to be cache.");
				Assert.That(
					spy.GetWholeLog(),
					Does
						.Contain("located HQL query plan in cache")
						.And.Not.Contain("unable to locate HQL query plan in cache"));
			}
		}

		[Test]
		public async Task PlansWithNonParameterizedConstantsAreNotCachedAsync()
		{
			var queryPlanCacheType = typeof(QueryPlanCache);

			var cache = (SoftLimitMRUCache)
				queryPlanCacheType
					.GetField("planCache", BindingFlags.Instance | BindingFlags.NonPublic)
					.GetValue(Sfi.QueryPlanCache);
			cache.Clear();

			await ((from c in db.Customers
			 where c.CustomerId == "ALFKI"
			 select new { c.CustomerId, c.ContactName, Constant = 1 }).FirstAsync());
			Assert.That(
				cache,
				Has.Count.EqualTo(0),
				"Query plan should not be cached.");
		}
	}
}
