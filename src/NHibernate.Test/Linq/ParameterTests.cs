using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using NHibernate.DomainModel.Northwind.Entities;
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
				ids.Length);
		}

		[Test]
		public void UsingTwoArrayParameters()
		{
			var ids = new[] {11008, 11019, 11039};
			var ids2 = new[] {11008, 11019, 11039};
			AssertTotalParameters(
				db.Orders.Where(o => ids.Contains(o.OrderId) && ids2.Contains(o.OrderId)),
				ids.Length + ids2.Length);
		}

		[Test]
		public void UsingListParameterTwice()
		{
			var ids = new List<int> {11008, 11019, 11039};
			AssertTotalParameters(
				db.Orders.Where(o => ids.Contains(o.OrderId) && ids.Contains(o.OrderId)),
				ids.Count);
		}

		[Test]
		public void UsingTwoListParameters()
		{
			var ids = new List<int> {11008, 11019, 11039};
			var ids2 = new List<int> {11008, 11019, 11039};
			AssertTotalParameters(
				db.Orders.Where(o => ids.Contains(o.OrderId) && ids2.Contains(o.OrderId)),
				ids.Count + ids2.Count);
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
		public void UsingValueTypeParameterTwice()
		{
			var value = 1;
			AssertTotalParameters(
				db.Orders.Where(o => o.OrderId == value && o.OrderId != value),
				1);
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

		private static void AssertTotalParameters<T>(IQueryable<T> query, int parameterNumber)
		{
			using (var sqlSpy = new SqlLogSpy())
			{
				query.ToList();
				var sqlParameters = sqlSpy.GetWholeLog().Split(';')[1];
				var matches = Regex.Matches(sqlParameters, @"([\d\w]+)[\s]+\=", RegexOptions.IgnoreCase);

				// Due to ODBC drivers not supporting parameter names, we have to do a distinct of parameter names.
				var distinctParameters = matches.OfType<Match>().Select(m => m.Groups[1].Value.Trim()).Distinct().ToList();
				Assert.That(distinctParameters, Has.Count.EqualTo(parameterNumber));
			}
		}
	}
}
