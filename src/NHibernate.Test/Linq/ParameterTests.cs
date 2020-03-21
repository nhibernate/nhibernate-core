using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class ParameterTests : LinqTestCase
	{
		[Test]
		public void UsingSameArrayParameterTwice()
		{
			var ids = new[] {11008, 11019, 11039};
			AssertTotalParameters(
				db.Orders.Where(o => ids.Contains(o.OrderId) && ids.Contains(o.OrderId)),
				ids.Length);
		}

		[Test]
		public void UsingDifferentArrayParameters()
		{
			var ids = new[] { 11008, 11019, 11039 };
			var ids2 = new[] { 11008, 11019, 11039 };
			AssertTotalParameters(
				db.Orders.Where(o => ids.Contains(o.OrderId) && ids2.Contains(o.OrderId)),
				ids.Length + ids2.Length);
		}

		[Test]
		public void UsingSameListParameterTwice()
		{
			var ids = new List<int> { 11008, 11019, 11039 };
			AssertTotalParameters(
				db.Orders.Where(o => ids.Contains(o.OrderId) && ids.Contains(o.OrderId)),
				ids.Count);
		}

		[Test]
		public void UsingDifferentListParameters()
		{
			var ids = new List<int> { 11008, 11019, 11039 };
			var ids2 = new List<int> { 11008, 11019, 11039 };
			AssertTotalParameters(
				db.Orders.Where(o => ids.Contains(o.OrderId) && ids2.Contains(o.OrderId)),
				ids.Count + ids2.Count);
		}

		[Test]
		public void UsingSameEntityParameterTwice()
		{
			var order = db.Orders.First();
			AssertTotalParameters(
				db.Orders.Where(o => o == order && o != order),
				1);
		}

		[Test]
		public void UsingDifferentEntityParameters()
		{
			var order = db.Orders.First();
			var order2 = db.Orders.Skip(1).First();
			AssertTotalParameters(
				db.Orders.Where(o => o == order && o != order2),
				2);
		}

		[Test]
		public void UsingSameValueTypeParameterTwice()
		{
			var value = 1;
			AssertTotalParameters(
				db.Orders.Where(o => o.OrderId == value && o.OrderId != value),
				1);
		}

		[Test]
		public void UsingDifferentValueTypeParameters()
		{
			var value = 1;
			var value2 = 2;
			AssertTotalParameters(
				db.Orders.Where(o => o.OrderId == value && o.OrderId != value2),
				2);
		}

		[Test]
		public void UsingSameStringParameterTwice()
		{
			var value = "test";
			AssertTotalParameters(
				db.Products.Where(o => o.Name == value && o.Name != value),
				1);
		}

		[Test]
		public void UsingDifferentStringParameters()
		{
			var value = "test";
			var value2 = "test2";
			AssertTotalParameters(
				db.Products.Where(o => o.Name == value && o.Name != value2),
				2);
		}

		private static void AssertTotalParameters<T>(IQueryable<T> query, int parameterNumber)
		{
			using (var sqlSpy = new SqlLogSpy())
			{
				query.ToList();
				var sqlParameters = sqlSpy.GetWholeLog().Split(';')[1];
				var matches = Regex.Matches(sqlParameters, @"([\d\w]+)[\s]+\=", RegexOptions.IgnoreCase);

				// Due to ODBC drivers not supporting parameter names, we have to do a distinct of parameter names.
				var distinctParameters = matches.Select(m => m.Groups[1].Value.Trim()).Distinct().ToList();
				Assert.That(distinctParameters, Has.Count.EqualTo(parameterNumber));
			}
		}
	}
}
