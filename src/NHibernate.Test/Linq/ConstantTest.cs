﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Criterion;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Engine.Query;
using NHibernate.Linq;
using NHibernate.Linq.Visitors;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	// Mainly adapted from tests contributed by Nicola Tuveri on NH-2500 (NH-2500.patch file)
	[TestFixture]
	public class ConstantTest : LinqTestCase
	{
		[Test]
		[Ignore("Linq query not supported yet")]
		public void ConstantNonCached()
		{
			var c1 = (from c in db.Customers
			          select "customer1").First();

			var c2 = (from c in db.Customers
			          select "customer2").First();

			Assert.That(c1, Is.EqualTo("customer1"));
			Assert.That(c2, Is.EqualTo("customer2"));
		}

		[Test]
		public void ConstantNonCachedInAnonymousNewExpression()
		{
			var c1 = (from c in db.Customers
			          where c.CustomerId == "ALFKI"
			          select new { c.CustomerId, c.ContactName, Constant = 1 }).First();

			var c2 = (from c in db.Customers
			          where c.CustomerId == "ANATR"
			          select new { c.CustomerId, c.ContactName, Constant = 2 }).First();

			Assert.That(c1.Constant, Is.EqualTo(1), "c1.Constant");
			Assert.That(c2.Constant, Is.EqualTo(2), "c2.Constant");
			Assert.That(c1.CustomerId, Is.EqualTo("ALFKI"), "c1.CustomerId");
			Assert.That(c2.CustomerId, Is.EqualTo("ANATR"), "c2.CustomerId");
		}

		[Test]
		public void ConstantNonCachedInNestedAnonymousNewExpressions()
		{
			var c1 = (from c in db.Customers
			          select new
			          {
				          c.ContactName,
				          Number = 1,
				          Customer = new { c.CustomerId, Label = "customer1" }
			          }).First();

			var c2 = (from c in db.Customers
			          select new
			          {
				          c.ContactName,
				          Number = 2,
				          Customer = new { c.CustomerId, Label = "customer2" }
			          }).First();

			Assert.That(c1.Number, Is.EqualTo(1), "c1.Number");
			Assert.That(c1.Customer.Label, Is.EqualTo("customer1"), "c1.Customer.Label");
			Assert.That(c2.Number, Is.EqualTo(2), "c1.Number");
			Assert.That(c2.Customer.Label, Is.EqualTo("customer2"), "c2.Customer.Label");
		}

		[Test]
		public void ConstantNonCachedInNewExpression()
		{
			var c1 = (from c in db.Customers
			          where c.CustomerId == "ALFKI"
			          select new KeyValuePair<string, string>(c.ContactName, "one")).First();

			var c2 = (from c in db.Customers
			          where c.CustomerId == "ANATR"
			          select new KeyValuePair<string, string>(c.ContactName, "two")).First();

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
		public void ConstantNonCachedInMemberInitExpression()
		{
			var s1 = (from s in db.Shippers
			          select new ShipperDto
			          {
				          Number = 1,
				          CompanyName = s.CompanyName,
				          Name = "shipper1"
			          }).ToList();

			var s2 = (from s in db.Shippers
			          select new ShipperDto
			          {
				          Number = 2,
				          CompanyName = s.CompanyName,
				          Name = "shipper2"
			          }).ToList();

			Assert.That(s1, Has.Count.GreaterThan(0), "s1 Count");
			Assert.That(s2, Has.Count.GreaterThan(0), "s2 Count");
			Assert.That(s1, Has.All.Property("Number").EqualTo(1), "s1 Numbers");
			Assert.That(s1, Has.All.Property("Name").EqualTo("shipper1"), "s1 Names");
			Assert.That(s2, Has.All.Property("Number").EqualTo(2), "s2 Numbers");
			Assert.That(s2, Has.All.Property("Name").EqualTo("shipper2"), "s2 Names");
		}

		[Test]
		public void ConstantInNewArrayExpression()
		{
			var c1 = (from c in db.Categories
			          select new[] { c.Name, "category1" }).ToList();

			var c2 = (from c in db.Categories
			          select new[] { c.Name, "category2" }).ToList();

			Assert.That(c1, Has.Count.GreaterThan(0), "c1 Count");
			Assert.That(c2, Has.Count.GreaterThan(0), "c2 Count");
			Assert.That(c1.All(c => c[1] == "category1"), Is.True, "c1 second item");
			Assert.That(c2.All(c => c[1] == "category2"), Is.True, "c2 second item");
		}

		[Test]
		public void ConstantsInNewArrayExpression()
		{
			var p1 = (from p in db.Products
			          select new Dictionary<string, int>()
			          {
				          { p.Name, 1 },
				          { "product1", p.ProductId }
			          }).First();

			var p2 = (from p in db.Products
			          select new Dictionary<string, int>()
			          {
				          { p.Name, 2 },
				          { "product2", p.ProductId }
			          }).First();

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
		public void ObjectConstants()
		{
			var builder = new InfoBuilder(1);
			var v1 = (from p in db.Products
			          select builder.GetItemValue(p)).First();
			builder = new InfoBuilder(2);
			var v2 = (from p in db.Products
			          select builder.GetItemValue(p)).First();

			Assert.That(v1, Is.EqualTo(1), "v1");
			Assert.That(v2, Is.EqualTo(2), "v2");
		}

		private int TestFunc(Product item, int closureValue)
		{
			return closureValue;
		}

		// Adapted from NH-3673
		[Test]
		public void ConstantsInFuncCall()
		{
			var closureVariable = 1;
			var v1 = (from p in db.Products
			          select TestFunc(p, closureVariable)).First();
			closureVariable = 2;
			var v2 = (from p in db.Products
			          select TestFunc(p, closureVariable)).First();

			Assert.That(v1, Is.EqualTo(1), "v1");
			Assert.That(v2, Is.EqualTo(2), "v2");
		}

		[Test]
		public void ConstantInWhereDoesNotCauseManyKeys()
		{
			var q1 = (from c in db.Customers
			          where c.CustomerId == "ALFKI"
			          select c);
			var q2 = (from c in db.Customers
			          where c.CustomerId == "ANATR"
			          select c);
			var preTransformParameters = new PreTransformationParameters(QueryMode.Select, Sfi);
			var preTransformResult = NhRelinqQueryParser.PreTransform(q1.Expression, preTransformParameters);
			var parameters1 = ExpressionParameterVisitor.Visit(preTransformResult);
			var k1 = ExpressionKeyVisitor.Visit(preTransformResult.Expression, parameters1, Sfi);

			var preTransformResult2 = NhRelinqQueryParser.PreTransform(q2.Expression, preTransformParameters);
			var parameters2 = ExpressionParameterVisitor.Visit(preTransformResult2);
			var k2 = ExpressionKeyVisitor.Visit(preTransformResult2.Expression, parameters2, Sfi);

			Assert.That(parameters1, Has.Count.GreaterThan(0), "parameters1");
			Assert.That(parameters2, Has.Count.GreaterThan(0), "parameters2");
			Assert.That(k2, Is.EqualTo(k1));
		}

		[Test]
		public void PlansAreCached()
		{
			var queryPlanCacheType = typeof(QueryPlanCache);

			var cache = (SoftLimitMRUCache)
				queryPlanCacheType
					.GetField("planCache", BindingFlags.Instance | BindingFlags.NonPublic)
					.GetValue(Sfi.QueryPlanCache);
			cache.Clear();

			(from c in db.Customers
			 where c.CustomerId == "ALFKI"
			 select new { c.CustomerId, c.ContactName }).First();
			Assert.That(
				cache,
				Has.Count.EqualTo(1),
				"First query plan should be cached.");

			using (var spy = new LogSpy(queryPlanCacheType))
			{
				// Should hit plan cache.
				(from c in db.Customers
				 where c.CustomerId == "ANATR"
				 select new { c.CustomerId, c.ContactName }).First();
				Assert.That(cache, Has.Count.EqualTo(1), "Second query should not cause a plan to be cache.");
				Assert.That(
					spy.GetWholeLog(),
					Does
						.Contain("located HQL query plan in cache")
						.And.Not.Contain("unable to locate HQL query plan in cache"));
			}
		}

		[Test]
		public void DmlPlansAreCached()
		{
			var queryPlanCacheType = typeof(QueryPlanCache);

			var cache = (SoftLimitMRUCache)
				queryPlanCacheType
					.GetField("planCache", BindingFlags.Instance | BindingFlags.NonPublic)
					.GetValue(Sfi.QueryPlanCache);
			cache.Clear();

			using (session.BeginTransaction())
			{
				db.Customers.Where(c => c.CustomerId == "UNKNOWN").Update(x => new Customer {CompanyName = "Constant1"});
				db.Customers.Where(c => c.CustomerId == "ALFKI").Update(x => new Customer {CompanyName = x.CompanyName});
				db.Customers.Where(c => c.CustomerId == "UNKNOWN").Update(x => new Customer {ContactName = "Constant1"});
				Assert.That(
					cache,
					Has.Count.EqualTo(3),
					"Query plans should be cached.");

				using (var spy = new LogSpy(queryPlanCacheType))
				{
					//Queries below should hit plan cache.
					using (var sqlSpy = new SqlLogSpy())
					{
						db.Customers.Where(c => c.CustomerId == "ANATR").Update(x => new Customer {CompanyName = x.CompanyName});
						db.Customers.Where(c => c.CustomerId == "UNKNOWN").Update(x => new Customer {CompanyName = "Constant2"});
						db.Customers.Where(c => c.CustomerId == "UNKNOWN").Update(x => new Customer {ContactName = "Constant2"});

						var sqlEvents = sqlSpy.Appender.GetEvents();
						Assert.That(
							sqlEvents[0].RenderedMessage,
							Does.Contain("ANATR").And.Not.Contain("UNKNOWN").And.Not.Contain("Constant1"),
							"Unexpected constant parameter value");
						Assert.That(
							sqlEvents[1].RenderedMessage,
							Does.Contain("UNKNOWN").And.Contain("Constant2").And.Contain("CompanyName").IgnoreCase
								.And.Not.Contain("Constant1"),
							"Unexpected constant parameter value");
						Assert.That(
							sqlEvents[2].RenderedMessage,
							Does.Contain("UNKNOWN").And.Contain("Constant2").And.Contain("ContactName").IgnoreCase
								.And.Not.Contain("Constant1"),
							"Unexpected constant parameter value");
					}

					Assert.That(cache, Has.Count.EqualTo(3), "Additional queries should not cause a plan to be cached.");
					Assert.That(
						spy.GetWholeLog(),
						Does
							.Contain("located HQL query plan in cache")
							.And.Not.Contain("unable to locate HQL query plan in cache"));

					db.Customers.Where(c => c.CustomerId == "ANATR").Update(x => new Customer {ContactName = x.ContactName});
					Assert.That(cache, Has.Count.EqualTo(4), "Query should be cached");
				}
			}
		}

		[Test]
		public void PlansWithNonParameterizedConstantsAreCached()
		{
			var queryPlanCacheType = typeof(QueryPlanCache);

			var cache = (SoftLimitMRUCache)
				queryPlanCacheType
					.GetField("planCache", BindingFlags.Instance | BindingFlags.NonPublic)
					.GetValue(Sfi.QueryPlanCache);
			cache.Clear();

			(from c in db.Customers
			 where c.CustomerId == "ALFKI"
			 select new { c.CustomerId, c.ContactName, Constant = 1 }).First();
			Assert.That(
				cache,
				Has.Count.EqualTo(1),
				"Query plan should be cached.");
		}

		[Test]
		public void PlansWithNonParameterizedConstantsAreCachedForExpandedQuery()
		{
			var queryPlanCacheType = typeof(QueryPlanCache);

			var cache = (SoftLimitMRUCache)
				queryPlanCacheType
					.GetField("planCache", BindingFlags.Instance | BindingFlags.NonPublic)
					.GetValue(Sfi.QueryPlanCache);
			cache.Clear();

			var ids = new[] {"ANATR", "UNKNOWN"}.ToList();
			db.Customers.Where(x => ids.Contains(x.CustomerId)).Select(
				c => new {c.CustomerId, c.ContactName, Constant = 1}).First();

			Assert.That(
				cache,
				Has.Count.EqualTo(2), // The second one is for the expanded expression that has two parameters
				"Query plan should be cached.");
		}

		//GH-2298 - Different Update queries - same query cache plan
		[Test]
		public void DmlPlansForExpandedQuery()
		{
			var queryPlanCacheType = typeof(QueryPlanCache);

			var cache = (SoftLimitMRUCache)
				queryPlanCacheType
					.GetField("planCache", BindingFlags.Instance | BindingFlags.NonPublic)
					.GetValue(Sfi.QueryPlanCache);
			cache.Clear();

			using (session.BeginTransaction())
			{
				var list = new[] {"UNKNOWN", "UNKNOWN2"}.ToList();
				db.Customers.Where(x => list.Contains(x.CustomerId)).Update(
					x => new Customer
					{
						CompanyName = "Constant1"
					});

				db.Customers.Where(x => list.Contains(x.CustomerId))
				.Update(
					x => new Customer
					{
						ContactName = "Constant1"
					});

				Assert.That(
					cache.Count,
					//2 original queries + 2 expanded queries are expected in cache
					Is.EqualTo(0).Or.EqualTo(4),
					"Query plans should either be cached separately or not cached at all.");
			}
		}
	}
}
