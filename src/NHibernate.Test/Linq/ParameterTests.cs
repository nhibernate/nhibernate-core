using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Engine.Query;
using NHibernate.Linq;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class ParameterTests : LinqTestCase
	{
		[Test]
		public void UsingArrayParameterTwice()
		{
			var ids = new[] {11008, 11019, 11039};
			AssertTotalParameters(
				db.Orders.Where(o => ids.Contains(o.OrderId) && ids.Contains(o.OrderId)),
				ids.Length,
				1);
		}

		[Test]
		public void UsingTwoArrayParameters()
		{
			var ids = new[] {11008, 11019, 11039};
			var ids2 = new[] {11008, 11019, 11039};
			AssertTotalParameters(
				db.Orders.Where(o => ids.Contains(o.OrderId) && ids2.Contains(o.OrderId)),
				ids.Length + ids2.Length,
				2);
		}

		[Test]
		public void UsingListParameterTwice()
		{
			var ids = new List<int> {11008, 11019, 11039};
			AssertTotalParameters(
				db.Orders.Where(o => ids.Contains(o.OrderId) && ids.Contains(o.OrderId)),
				ids.Count,
				1);
		}

		[Test]
		public void UsingTwoListParameters()
		{
			var ids = new List<int> {11008, 11019, 11039};
			var ids2 = new List<int> {11008, 11019, 11039};
			AssertTotalParameters(
				db.Orders.Where(o => ids.Contains(o.OrderId) && ids2.Contains(o.OrderId)),
				ids.Count + ids2.Count,
				2);
		}

		[Test]
		public void UsingEntityParameterTwice()
		{
			var order = db.Orders.First();
			AssertTotalParameters(
				db.Orders.Where(o => o == order && o != order),
				1);
		}

		[Test]
		public void UsingTwoEntityParameters()
		{
			var order = db.Orders.First();
			var order2 = db.Orders.First();
			AssertTotalParameters(
				db.Orders.Where(o => o == order && o != order2),
				2);
		}

		[Test]
		public void UsingEntityEnumerableParameterTwice()
		{
			if (!Dialect.SupportsSubSelects)
			{
				Assert.Ignore();
			}

			var enumerable = db.DynamicUsers.First();
			AssertTotalParameters(
				db.DynamicUsers.Where(o => o == enumerable && o != enumerable),
				1);
		}

		[Test]
		public void UsingEntityEnumerableListParameterTwice()
		{
			if (!Dialect.SupportsSubSelects)
			{
				Assert.Ignore();
			}

			var enumerable = new[] {db.DynamicUsers.First()};
			AssertTotalParameters(
				db.DynamicUsers.Where(o => enumerable.Contains(o) && enumerable.Contains(o)),
				1);
		}

		[Test]
		public void UsingValueTypeParameterTwice()
		{
			var value = 1;
			AssertTotalParameters(
				db.Orders.Where(o => o.OrderId == value && o.OrderId != value),
				1);
		}

		[Test]
		public void ValidateMixingTwoParametersCacheKeys()
		{
			var value = 1;
			var value2 = 1;
			var expression1 = GetLinqExpression(db.Orders.Where(o => o.OrderId == value && o.OrderId != value));
			var expression2 = GetLinqExpression(db.Orders.Where(o => o.OrderId == value && o.OrderId != value2));
			var expression3 = GetLinqExpression(db.Orders.Where(o => o.OrderId == value2 && o.OrderId != value));
			var expression4 = GetLinqExpression(db.Orders.Where(o => o.OrderId == value2 && o.OrderId != value2));

			Assert.That(expression1.Key, Is.Not.EqualTo(expression2.Key));
			Assert.That(expression1.Key, Is.Not.EqualTo(expression3.Key));
			Assert.That(expression1.Key, Is.EqualTo(expression4.Key));

			Assert.That(expression2.Key, Is.EqualTo(expression3.Key));
			Assert.That(expression2.Key, Is.Not.EqualTo(expression4.Key));

			Assert.That(expression3.Key, Is.Not.EqualTo(expression4.Key));
		}

		[Test]
		public void UsingNegateValueTypeParameterTwice()
		{
			var value = 1;
			AssertTotalParameters(
				db.Orders.Where(o => o.OrderId == -value && o.OrderId != -value),
				1);
		}

		[Test]
		public void UsingNegateValueTypeParameter()
		{
			var value = 1;
			AssertTotalParameters(
				db.Orders.Where(o => o.OrderId == value && o.OrderId != -value),
				1);
		}

		[Test]
		public void UsingValueTypeParameterInArray()
		{
			var id = 11008;
			AssertTotalParameters(
				db.Orders.Where(o => new[] {id, 11019}.Contains(o.OrderId) && new[] {id, 11019}.Contains(o.OrderId)),
				4,
				2);
		}

		[Test]
		public void UsingTwoValueTypeParameters()
		{
			var value = 1;
			var value2 = 1;
			AssertTotalParameters(
				db.Orders.Where(o => o.OrderId == value && o.OrderId != value2),
				2);
		}

		[Test]
		public void UsingStringParameterTwice()
		{
			var value = "test";
			AssertTotalParameters(
				db.Products.Where(o => o.Name == value && o.Name != value),
				1);
		}

		[Test]
		public void UsingTwoStringParameters()
		{
			var value = "test";
			var value2 = "test";
			AssertTotalParameters(
				db.Products.Where(o => o.Name == value && o.Name != value2),
				2);
		}

		[Test]
		public void UsingObjectPropertyParameterTwice()
		{
			var value = new Product {Name = "test"};
			AssertTotalParameters(
				db.Products.Where(o => o.Name == value.Name && o.Name != value.Name),
				1);
		}

		[Test]
		public void UsingTwoObjectPropertyParameters()
		{
			var value = new Product {Name = "test"};
			var value2 = new Product {Name = "test"};
			AssertTotalParameters(
				db.Products.Where(o => o.Name == value.Name && o.Name != value2.Name),
				2);
		}

		[Test]
		public void UsingParameterInWhereSkipTake()
		{
			var value3 = 1;
			var q1 = db.Products.Where(o => o.ProductId < value3).Take(value3).Skip(value3);
			AssertTotalParameters(q1, 3);
		}

		[Test]
		public void UsingParameterInTwoWhere()
		{
			var value3 = 1;
			var q1 = db.Products.Where(o => o.ProductId < value3).Where(o => o.ProductId < value3);
			AssertTotalParameters(q1, 1);
		}

		[Test]
		public void UsingObjectNestedPropertyParameterTwice()
		{
			var value = new Employee {Superior = new Employee {Superior = new Employee {FirstName = "test"}}};
			AssertTotalParameters(
				db.Employees.Where(o => o.FirstName == value.Superior.Superior.FirstName && o.FirstName != value.Superior.Superior.FirstName),
				1);
		}

		[Test]
		public void UsingDifferentObjectNestedPropertyParameter()
		{
			var value = new Employee {Superior = new Employee {FirstName = "test", Superior = new Employee {FirstName = "test"}}};
			AssertTotalParameters(
				db.Employees.Where(o => o.FirstName == value.Superior.FirstName && o.FirstName != value.Superior.Superior.FirstName),
				2);
		}

		[Test]
		public void UsingMethodObjectPropertyParameterTwice()
		{
			var value = new Product {Name = "test"};
			AssertTotalParameters(
				db.Products.Where(o => o.Name == value.Name.Trim() && o.Name != value.Name.Trim()),
				2);
		}

		[Test]
		public void UsingStaticMethodObjectPropertyParameterTwice()
		{
			var value = new Product {Name = "test"};
			AssertTotalParameters(
				db.Products.Where(o => o.Name == string.Copy(value.Name) && o.Name != string.Copy(value.Name)),
				2);
		}

		[Test]
		public void UsingObjectPropertyParameterWithSecondLevelClosure()
		{
			var value = new Product {Name = "test"};
			Expression<Func<Product, bool>> predicate = o => o.Name == value.Name && o.Name != value.Name;
			AssertTotalParameters(
				db.Products.Where(predicate),
				1);
		}

		[Test]
		public void UsingObjectPropertyParameterWithThirdLevelClosure()
		{
			var value = new Product {Name = "test"};
			Expression<Func<OrderLine, bool>> orderLinePredicate = o => o.Order.ShippedTo == value.Name && o.Order.ShippedTo != value.Name;
			Expression<Func<Product, bool>> predicate = o => o.Name == value.Name && o.OrderLines.AsQueryable().Any(orderLinePredicate);
			AssertTotalParameters(
				db.Products.Where(predicate),
				1);
		}

		[Test]
		public void UsingParameterInDMLInsertIntoFourTimes()
		{
			var value = "test";
			AssertTotalParameters(
				QueryMode.Insert,
				db.Customers.Where(c => c.CustomerId == value),
				x => new Customer {CustomerId = value, ContactName = value, CompanyName = value},
				4);
		}

		[Test]
		public void UsingFourParametersInDMLInsertInto()
		{
			var value = "test";
			var value2 = "test";
			var value3 = "test";
			var value4 = "test";
			AssertTotalParameters(
				QueryMode.Insert,
				db.Customers.Where(c => c.CustomerId == value3),
				x => new Customer {CustomerId = value4, ContactName = value2, CompanyName = value},
				4);
		}

		[Test]
		public void DMLInsertIntoShouldHaveSameCacheKeys()
		{
			var value = "test";
			var value2 = "test";
			var value3 = "test";
			var value4 = "test";
			var expression1 = GetLinqExpression(
				QueryMode.Insert,
				db.Customers.Where(c => c.CustomerId == value),
				x => new Customer {CustomerId = value, ContactName = value, CompanyName = value});
			var expression2 = GetLinqExpression(
				QueryMode.Insert,
				db.Customers.Where(c => c.CustomerId == value3),
				x => new Customer {CustomerId = value4, ContactName = value2, CompanyName = value});

			Assert.That(expression1.Key, Is.EqualTo(expression2.Key));
		}

		[Test]
		public void UsingParameterInDMLUpdateThreeTimes()
		{
			var value = "test";
			AssertTotalParameters(
				QueryMode.Update,
				db.Customers.Where(c => c.CustomerId == value),
				x => new Customer {ContactName = value, CompanyName = value},
				3);
		}

		[Test]
		public void UsingThreeParametersInDMLUpdate()
		{
			var value = "test";
			var value2 = "test";
			var value3 = "test";
			AssertTotalParameters(
				QueryMode.Update,
				db.Customers.Where(c => c.CustomerId == value3),
				x => new Customer { ContactName = value2, CompanyName = value },
				3);
		}

		[TestCase(QueryMode.Update)]
		[TestCase(QueryMode.UpdateVersioned)]
		public void DMLUpdateIntoShouldHaveSameCacheKeys(QueryMode queryMode)
		{
			var value = "test";
			var value2 = "test";
			var value3 = "test";
			var expression1 = GetLinqExpression(
				queryMode,
				db.Customers.Where(c => c.CustomerId == value),
				x => new Customer {ContactName = value, CompanyName = value});
			var expression2 = GetLinqExpression(
				queryMode,
				db.Customers.Where(c => c.CustomerId == value3),
				x => new Customer {ContactName = value2, CompanyName = value});

			Assert.That(expression1.Key, Is.EqualTo(expression2.Key));
		}

		[Test]
		public void UsingParameterInDMLDeleteTwice()
		{
			var value = "test";
			AssertTotalParameters(
				QueryMode.Delete,
				db.Customers.Where(c => c.CustomerId == value && c.CompanyName == value),
				2);
		}

		[Test]
		public void UsingTwoParametersInDMLDelete()
		{
			var value = "test";
			var value2 = "test";
			AssertTotalParameters(
				QueryMode.Delete,
				db.Customers.Where(c => c.CustomerId == value && c.CompanyName == value2),
				2);
		}

		[Test]
		public void DMLDeleteShouldHaveSameCacheKeys()
		{
			var value = "test";
			var value2 = "test";
			var expression1 = GetLinqExpression(
				QueryMode.Delete,
				db.Customers.Where(c => c.CustomerId == value && c.CompanyName == value));
			var expression2 = GetLinqExpression(
				QueryMode.Delete,
				db.Customers.Where(c => c.CustomerId == value && c.CompanyName == value2));

			Assert.That(expression1.Key, Is.EqualTo(expression2.Key));
		}

		private void AssertTotalParameters<T>(IQueryable<T> query, int parameterNumber, int? linqParameterNumber = null)
		{
			using (var sqlSpy = new SqlLogSpy())
			{
				// In case of arrays linqParameterNumber and parameterNumber will be different
				Assert.That(
					GetLinqExpression(query).ParameterValuesByName.Count,
					Is.EqualTo(linqParameterNumber ?? parameterNumber),
					"Linq expression has different number of parameters");

				var queryPlanCacheType = typeof(QueryPlanCache);
				var cache = (SoftLimitMRUCache)
					queryPlanCacheType
						.GetField("planCache", BindingFlags.Instance | BindingFlags.NonPublic)
						.GetValue(Sfi.QueryPlanCache);
				cache.Clear();

				query.ToList();

				// In case of arrays two query plans will be stored, one with an one without expended parameters
				Assert.That(cache, Has.Count.EqualTo(linqParameterNumber.HasValue ? 2 : 1), "Query should be cacheable");

				AssertParameters(sqlSpy, parameterNumber);
			}
		}

		private static void AssertTotalParameters<T>(QueryMode queryMode, IQueryable<T> query, int parameterNumber)
		{
			AssertTotalParameters(queryMode, query, null, parameterNumber);
		}

		private static void AssertTotalParameters<T>(QueryMode queryMode, IQueryable<T> query, Expression<Func<T, T>> expression, int parameterNumber)
		{
			var provider = query.Provider as INhQueryProvider;
			Assert.That(provider, Is.Not.Null);

			var dmlExpression = expression != null
				? DmlExpressionRewriter.PrepareExpression(query.Expression, expression)
				: query.Expression;

			using (var sqlSpy = new SqlLogSpy())
			{
				Assert.That(provider.ExecuteDml<T>(queryMode, dmlExpression), Is.EqualTo(0), "The DML query updated the data"); // Avoid updating the data
				AssertParameters(sqlSpy, parameterNumber);
			}
		}

		private static void AssertParameters(SqlLogSpy sqlSpy, int parameterNumber)
		{
			var sqlParameters = sqlSpy.GetWholeLog().Split(';')[1];
			var matches = Regex.Matches(sqlParameters, @"([\d\w]+)[\s]+\=", RegexOptions.IgnoreCase);

			// Due to ODBC drivers not supporting parameter names, we have to do a distinct of parameter names.
			var distinctParameters = matches.OfType<Match>().Select(m => m.Groups[1].Value.Trim()).Distinct().ToList();
			Assert.That(distinctParameters, Has.Count.EqualTo(parameterNumber));
		}

		private NhLinqExpression GetLinqExpression<T>(QueryMode queryMode, IQueryable<T> query, Expression<Func<T, T>> expression)
		{
			return GetLinqExpression(queryMode, DmlExpressionRewriter.PrepareExpression(query.Expression, expression));
		}

		private NhLinqExpression GetLinqExpression<T>(QueryMode queryMode, IQueryable<T> query)
		{
			return GetLinqExpression(queryMode, query.Expression);
		}

		private NhLinqExpression GetLinqExpression<T>(IQueryable<T> query)
		{
			return GetLinqExpression(QueryMode.Select, query.Expression);
		}

		private NhLinqExpression GetLinqExpression(QueryMode queryMode, Expression expression)
		{
			return queryMode == QueryMode.Select
				? new NhLinqExpression(expression, Sfi)
				: new NhLinqDmlExpression<Customer>(queryMode, expression, Sfi);
		}
	}
}
