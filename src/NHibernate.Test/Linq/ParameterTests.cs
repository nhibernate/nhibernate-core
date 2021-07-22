using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using NHibernate.Dialect;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Driver;
using NHibernate.Engine.Query;
using NHibernate.Linq;
using NHibernate.Util;
using NUnit.Framework;
using NUnit.Framework.Constraints;

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
		public void UsingEntityParameterForCollection()
		{
			var item = db.OrderLines.First();
			AssertTotalParameters(
				db.Orders.Where(o => o.OrderLines.Contains(item)),
				1);
		}

		[Test]
		public void UsingProxyParameterForCollection()
		{
			var item = session.Load<Order>(10248);
			Assert.That(NHibernateUtil.IsInitialized(item), Is.False);
			AssertTotalParameters(
				db.Customers.Where(o => o.Orders.Contains(item)),
				1);
		}

		[Test]
		public void UsingFieldProxyParameterForCollection()
		{
			var item = session.Query<AnotherEntityRequired>().First();
			AssertTotalParameters(
				session.Query<AnotherEntityRequired>().Where(o => o.RequiredRelatedItems.Contains(item)),
				1);
		}

		[Test]
		public void UsingEntityParameterInSubQuery()
		{
			var item = db.Customers.First();
			var subQuery = db.Orders.Select(o => o.Customer).Where(o => o == item);
			AssertTotalParameters(
				db.Orders.Where(o => subQuery.Contains(o.Customer)),
				1);
		}

		[Test]
		public void UsingEntityParameterForCollectionSelection()
		{
			var item = db.OrderLines.First();
			AssertTotalParameters(
				db.Orders.SelectMany(o => o.OrderLines).Where(o => o == item),
				1);
		}

		[Test]
		public void UsingFieldProxyParameterForCollectionSelection()
		{
			var item = session.Query<AnotherEntityRequired>().First();
			AssertTotalParameters(
				session.Query<AnotherEntityRequired>().SelectMany(o => o.RequiredRelatedItems).Where(o => o == item),
				1);
		}

		[Test]
		public void UsingEntityListParameterForCollectionSelection()
		{
			var items = new[] {db.OrderLines.First()};
			AssertTotalParameters(
				db.Orders.SelectMany(o => o.OrderLines).Where(o => items.Contains(o)),
				1);
		}

		[Test]
		public void UsingFieldProxyListParameterForCollectionSelection()
		{
			var items = new[] {session.Query<AnotherEntityRequired>().First()};
			AssertTotalParameters(
				session.Query<AnotherEntityRequired>().SelectMany(o => o.RequiredRelatedItems).Where(o => items.Contains(o)),
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
		public void CompareIntegralParametersAndColumns()
		{
			short shortParam = 1;
			var intParam = 2;
			var longParam = 3L;
			short? nullShortParam = 1;
			int? nullIntParam = 2;
			long? nullLongParam = 3L;
			var queriables = new Dictionary<IQueryable<NumericEntity>, string>
			{
				{db.NumericEntities.Where(o => o.Short == shortParam || o.Short < intParam || o.Short > longParam), "Int16"},
				{db.NumericEntities.Where(o => o.NullableShort == shortParam || o.NullableShort <= intParam || o.NullableShort != longParam), "Int16"},
				{db.NumericEntities.Where(o => o.Short == nullShortParam || o.Short < nullIntParam || o.Short > nullLongParam), "Int16"},
				{db.NumericEntities.Where(o => o.NullableShort == nullShortParam || o.NullableShort <= nullIntParam || o.NullableShort != nullLongParam), "Int16"},

				{db.NumericEntities.Where(o => o.Integer == shortParam || o.Integer < intParam || o.Integer > longParam), "Int32"},
				{db.NumericEntities.Where(o => o.NullableInteger == shortParam || o.NullableInteger <= intParam || o.NullableInteger != longParam), "Int32"},
				{db.NumericEntities.Where(o => o.Integer == nullShortParam || o.Integer < nullIntParam || o.Integer > nullLongParam), "Int32"},
				{db.NumericEntities.Where(o => o.NullableInteger == nullShortParam || o.NullableInteger <= nullIntParam || o.NullableInteger != nullLongParam), "Int32"},

				{db.NumericEntities.Where(o => o.Long == shortParam || o.Long < intParam || o.Long > longParam), "Int64"},
				{db.NumericEntities.Where(o => o.NullableLong == shortParam || o.NullableLong <= intParam || o.NullableLong != longParam), "Int64"},
				{db.NumericEntities.Where(o => o.Long == nullShortParam || o.Long < nullIntParam || o.Long > nullLongParam), "Int64"},
				{db.NumericEntities.Where(o => o.NullableLong == nullShortParam || o.NullableLong <= nullIntParam || o.NullableLong != nullLongParam), "Int64"}
			};

			foreach (var pair in queriables)
			{
				// Parameters should be pre-evaluated
				AssertTotalParameters(
					pair.Key,
					3,
					sql =>
					{
						Assert.That(sql, Does.Not.Contain("cast"));
						Assert.That(GetTotalOccurrences(sql, $"Type: {pair.Value}"), Is.EqualTo(3));
					});
			}
		}

		[Test]
		public void CompareIntegralParametersWithFloatingPointColumns()
		{
			short shortParam = 1;
			var intParam = 2;
			var longParam = 3L;
			short? nullShortParam = 1;
			int? nullIntParam = 2;
			long? nullLongParam = 3L;
			var queriables = new Dictionary<IQueryable<NumericEntity>, string>
			{
				{db.NumericEntities.Where(o => o.Decimal == shortParam || o.Decimal < intParam || o.Decimal > longParam), "Decimal"},
				{db.NumericEntities.Where(o => o.NullableDecimal == shortParam || o.NullableDecimal <= intParam || o.NullableDecimal != longParam), "Decimal"},
				{db.NumericEntities.Where(o => o.Decimal == nullShortParam || o.Decimal < nullIntParam || o.Decimal > nullLongParam), "Decimal"},
				{db.NumericEntities.Where(o => o.NullableDecimal == nullShortParam || o.NullableDecimal <= nullIntParam || o.NullableDecimal != nullLongParam), "Decimal"},

				{db.NumericEntities.Where(o => o.Single == shortParam || o.Single < intParam || o.Single > longParam), "Single"},
				{db.NumericEntities.Where(o => o.NullableSingle == shortParam || o.NullableSingle <= intParam || o.NullableSingle != longParam), "Single"},
				{db.NumericEntities.Where(o => o.Single == nullShortParam || o.Single < nullIntParam || o.Single > nullLongParam), "Single"},
				{db.NumericEntities.Where(o => o.NullableSingle == nullShortParam || o.NullableSingle <= nullIntParam || o.NullableSingle != nullLongParam), "Single"},

				{db.NumericEntities.Where(o => o.Double == shortParam || o.Double < intParam || o.Double > longParam), "Double"},
				{db.NumericEntities.Where(o => o.NullableDouble == shortParam || o.NullableDouble <= intParam || o.NullableDouble != longParam), "Double"},
				{db.NumericEntities.Where(o => o.Double == nullShortParam || o.Double < nullIntParam || o.Double > nullLongParam), "Double"},
				{db.NumericEntities.Where(o => o.NullableDouble == nullShortParam || o.NullableDouble <= nullIntParam || o.NullableDouble != nullLongParam), "Double"},
			};

			foreach (var pair in queriables)
			{
				// Parameters should be pre-evaluated
				AssertTotalParameters(
					pair.Key,
					3,
					sql =>
					{
						Assert.That(sql, Does.Not.Contain("cast"));
						Assert.That(GetTotalOccurrences(sql, $"Type: {pair.Value}"), Is.EqualTo(3));
					});
			}
		}

		[Test]
		public void CompareFloatingPointParametersAndColumns()
		{
			var decimalParam = 1.1m;
			var singleParam = 2.2f;
			var doubleParam = 3.3d;
			decimal? nullDecimalParam = 1.1m;
			float? nullSingleParam = 2.2f;
			double? nullDoubleParam = 3.3d;
			var queriables = new Dictionary<IQueryable<NumericEntity>, string>
			{
				{db.NumericEntities.Where(o => o.Decimal == decimalParam), "Decimal"},
				{db.NumericEntities.Where(o => o.NullableDecimal == decimalParam), "Decimal"},
				{db.NumericEntities.Where(o => o.Decimal == nullDecimalParam), "Decimal"},
				{db.NumericEntities.Where(o => o.NullableDecimal == nullDecimalParam), "Decimal"},

				{db.NumericEntities.Where(o => o.Single <= singleParam || o.Single >= doubleParam), "Single"},
				{db.NumericEntities.Where(o => o.NullableSingle == singleParam || o.NullableSingle != doubleParam), "Single"},
				{db.NumericEntities.Where(o => o.Single <= nullSingleParam || o.Single >= nullDoubleParam), "Single"},
				{db.NumericEntities.Where(o => o.NullableSingle == nullSingleParam || o.NullableSingle != nullDoubleParam), "Single"},

				{db.NumericEntities.Where(o => o.Double <= singleParam || o.Double >= doubleParam), "Double"},
				{db.NumericEntities.Where(o => o.NullableDouble == singleParam || o.NullableDouble != doubleParam), "Double"},
				{db.NumericEntities.Where(o => o.Double <= nullSingleParam || o.Double >= nullDoubleParam), "Double"},
				{db.NumericEntities.Where(o => o.NullableDouble == nullSingleParam || o.NullableDouble != nullDoubleParam), "Double"},
			};

			foreach (var pair in queriables)
			{
				var totalParameters = pair.Value == "Decimal" ? 1 : 2;
				// Parameters should be pre-evaluated
				AssertTotalParameters(
					pair.Key,
					totalParameters,
					sql =>
					{
						Assert.That(sql, pair.Value == "Decimal" && Dialect.IsDecimalStoredAsFloatingPointNumber ? Does.Contain("cast") : Does.Not.Contain("cast"));
						Assert.That(GetTotalOccurrences(sql, $"Type: {pair.Value}"), Is.EqualTo(totalParameters));
					});
			}
		}

		[Test]
		public void CompareFloatingPointParametersWithIntegralColumns()
		{
			var decimalParam = 1.1m;
			var singleParam = 2.2f;
			var doubleParam = 3.3d;
			decimal? nullDecimalParam = 1.1m;
			float? nullSingleParam = 2.2f;
			double? nullDoubleParam = 3.3d;
			var queriables = new List<IQueryable<NumericEntity>>
			{
				db.NumericEntities.Where(o => o.Short == decimalParam || o.Short != singleParam || o.Short <= doubleParam),
				db.NumericEntities.Where(o => o.NullableShort <= decimalParam || o.NullableShort == singleParam || o.NullableShort >= doubleParam),
				db.NumericEntities.Where(o => o.Short == nullDecimalParam || o.Short != nullSingleParam || o.Short <= nullDoubleParam),
				db.NumericEntities.Where(o => o.NullableShort <= nullDecimalParam || o.NullableShort == nullSingleParam || o.NullableShort >= nullDoubleParam),

				db.NumericEntities.Where(o => o.Integer == decimalParam || o.Integer != singleParam || o.Integer <= doubleParam),
				db.NumericEntities.Where(o => o.NullableInteger <= decimalParam || o.NullableInteger == singleParam || o.NullableInteger >= doubleParam),
				db.NumericEntities.Where(o => o.Integer == nullDecimalParam || o.Integer != nullSingleParam || o.Integer <= nullDoubleParam),
				db.NumericEntities.Where(o => o.NullableInteger <= nullDecimalParam || o.NullableInteger == nullSingleParam || o.NullableInteger >= nullDoubleParam),

				db.NumericEntities.Where(o => o.Long == decimalParam || o.Long != singleParam || o.Long <= doubleParam),
				db.NumericEntities.Where(o => o.NullableLong <= decimalParam || o.NullableLong == singleParam || o.NullableLong >= doubleParam),
				db.NumericEntities.Where(o => o.Long == nullDecimalParam || o.Long != nullSingleParam || o.Long <= nullDoubleParam),
				db.NumericEntities.Where(o => o.NullableLong <= nullDecimalParam || o.NullableLong == nullSingleParam || o.NullableLong >= nullDoubleParam),
			};

			foreach (var query in queriables)
			{
				// Columns should be casted
				AssertTotalParameters(
					query,
					3,
					sql =>
					{
						var matches = Regex.Matches(sql, @"cast\([\w\d]+\..+\)");
						Assert.That(matches.Count, Is.EqualTo(3));
						Assert.That(GetTotalOccurrences(sql, $"Type: Decimal"), Is.EqualTo(1));
						Assert.That(GetTotalOccurrences(sql, $"Type: Single"), Is.EqualTo(1));
						Assert.That(GetTotalOccurrences(sql, $"Type: Double"), Is.EqualTo(1));
					});
			}
		}

		[Test]
		public void CompareFloatingPointParameterWithIntegralAndFloatingPointColumns()
		{
			var decimalParam = 1.1m;
			var singleParam = 2.2f;
			var doubleParam = 3.3d;
			var queriables = new Dictionary<IQueryable<NumericEntity>, string>
			{
				{db.NumericEntities.Where(o => o.Decimal == decimalParam || o.NullableShort >= decimalParam), "Decimal"},
				{db.NumericEntities.Where(o => o.Decimal == decimalParam || o.Long >= decimalParam), "Decimal"},
				{db.NumericEntities.Where(o => o.NullableDecimal == decimalParam || o.Integer != decimalParam), "Decimal"},
				{db.NumericEntities.Where(o => o.NullableDecimal == decimalParam || o.NullableInteger == decimalParam), "Decimal"},

				{db.NumericEntities.Where(o => o.Single == singleParam || o.NullableShort >= singleParam), "Single"},
				{db.NumericEntities.Where(o => o.Single == singleParam || o.Long >= singleParam), "Single"},
				{db.NumericEntities.Where(o => o.NullableSingle == singleParam || o.Integer != singleParam), "Single"},
				{db.NumericEntities.Where(o => o.NullableSingle == singleParam || o.NullableInteger == singleParam), "Single"},

				{db.NumericEntities.Where(o => o.Double == doubleParam || o.NullableShort >= doubleParam), "Double"},
				{db.NumericEntities.Where(o => o.Double == doubleParam || o.Long >= doubleParam), "Double"},
				{db.NumericEntities.Where(o => o.NullableDouble == doubleParam || o.Integer != doubleParam), "Double"},
				{db.NumericEntities.Where(o => o.NullableDouble == doubleParam || o.NullableInteger == doubleParam), "Double"}
			};
			var odbcDriver = Sfi.ConnectionProvider.Driver is OdbcDriver;

			foreach (var pair in queriables)
			{
				// Integral columns should be casted
				AssertTotalParameters(
					pair.Key,
					1,
					sql =>
					{
						var matches = Regex.Matches(sql, @"cast\([\w\d]+\..+\)");
						Assert.That(matches.Count, Is.EqualTo(1));
						Assert.That(GetTotalOccurrences(sql, $"Type: {pair.Value}"), Is.EqualTo(odbcDriver ? 2 : 1));
					});
			}
		}

		[Test]
		public void CompareFloatingPointParameterWithDifferentFloatingPointColumns()
		{
			if (Sfi.Dialect is FirebirdDialect)
			{
				Assert.Ignore("Due to the regex hack in FirebirdClientDriver, the parameters can be casted twice.");
			}

			var singleParam = 2.2f;
			var doubleParam = 3.3d;
			var queriables = new Dictionary<IQueryable<NumericEntity>, string>
			{
				{db.NumericEntities.Where(o => o.Single == singleParam || o.Double >= singleParam), "Single"},
				{db.NumericEntities.Where(o => o.Double >= singleParam || o.Single == singleParam), "Single"},
				{db.NumericEntities.Where(o => o.Single == singleParam || o.NullableDouble <= singleParam), "Single"},
				{db.NumericEntities.Where(o => o.NullableSingle == singleParam || o.Double != singleParam), "Single"},
				{db.NumericEntities.Where(o => o.NullableSingle == singleParam || o.NullableDouble == singleParam), "Single"},

				{db.NumericEntities.Where(o => o.Double == doubleParam || o.Single >= doubleParam), "Double"},
				{db.NumericEntities.Where(o => o.Single >= doubleParam || o.Double == doubleParam), "Double"},
				{db.NumericEntities.Where(o => o.Double == doubleParam || o.NullableSingle >= doubleParam), "Double"},
				{db.NumericEntities.Where(o => o.NullableDouble == doubleParam || o.Single != doubleParam), "Double"},
				{db.NumericEntities.Where(o => o.NullableDouble == doubleParam || o.NullableSingle == doubleParam), "Double"}
			};
			var sameType = Sfi.Dialect.TryGetCastTypeName(NHibernateUtil.Single.SqlType, out var singleCast) &&
			               Sfi.Dialect.TryGetCastTypeName(NHibernateUtil.Double.SqlType, out var doubleCast) &&
			               singleCast == doubleCast;
			var odbcDriver = Sfi.ConnectionProvider.Driver is OdbcDriver;

			foreach (var pair in queriables)
			{
				// Columns should be casted for Double parameter and parameters for Single parameter
				AssertTotalParameters(
					pair.Key,
					1,
					sql =>
					{
						var matches = pair.Value == "Double"
							? Regex.Matches(sql, @"cast\([\w\d]+\..+\)")
							: Regex.Matches(sql, @"cast\(((@|\?|:)p\d+|\?)\s+as.*\)");
						// SQLiteDialect uses sql cast for transparentcast method
						Assert.That(matches.Count, Is.EqualTo(sameType && !(Sfi.Dialect is SQLiteDialect) ? 0 : 1));
						Assert.That(GetTotalOccurrences(sql, $"Type: {pair.Value}"), Is.EqualTo(odbcDriver ? 2 : 1));
					});
			}
		}

		[Test]
		public void CompareIntegralParameterWithIntegralAndFloatingPointColumns()
		{
			if (Sfi.Dialect is FirebirdDialect)
			{
				Assert.Ignore("Due to the regex hack in FirebirdClientDriver, the parameters can be casted twice.");
			}

			short shortParam = 1;
			var intParam = 2;
			var longParam = 3L;
			var queriables = new Dictionary<IQueryable<NumericEntity>, string>
			{
				{db.NumericEntities.Where(o => o.Short == shortParam || o.Double >= shortParam), "Int16"},
				{db.NumericEntities.Where(o => o.Short == shortParam || o.NullableDouble >= shortParam), "Int16"},
				{db.NumericEntities.Where(o => o.NullableShort == shortParam || o.Decimal != shortParam), "Int16"},
				{db.NumericEntities.Where(o => o.NullableShort == shortParam || o.NullableSingle > shortParam), "Int16"},

				{db.NumericEntities.Where(o => o.Integer == intParam || o.Double >= intParam), "Int32"},
				{db.NumericEntities.Where(o => o.Integer == intParam || o.NullableDouble >= intParam), "Int32"},
				{db.NumericEntities.Where(o => o.NullableInteger == intParam || o.Decimal != intParam), "Int32"},
				{db.NumericEntities.Where(o => o.NullableInteger == intParam || o.NullableSingle > intParam), "Int32"},

				{db.NumericEntities.Where(o => o.Long == longParam || o.Double >= longParam), "Int64"},
				{db.NumericEntities.Where(o => o.Long == longParam || o.NullableDouble >= longParam), "Int64"},
				{db.NumericEntities.Where(o => o.NullableLong == longParam || o.Decimal != longParam), "Int64"},
				{db.NumericEntities.Where(o => o.NullableLong == longParam || o.NullableSingle > longParam), "Int64"}
			};
			var odbcDriver = Sfi.ConnectionProvider.Driver is OdbcDriver;

			foreach (var pair in queriables)
			{
				// Parameters should be casted
				AssertTotalParameters(
					pair.Key,
					1,
					sql =>
					{
						var matches = Regex.Matches(sql, @"cast\(((@|\?|:)p\d+|\?)\s+as.*\)");
						Assert.That(matches.Count, Is.EqualTo(1));
						Assert.That(GetTotalOccurrences(sql, $"Type: {pair.Value}"), Is.EqualTo(odbcDriver ? 2 : 1));
					});
			}
		}

		[Test]
		public void UsingValueTypeParameterOfDifferentType()
		{
			var value = 1;
			var queriables = new List<IQueryable<NumericEntity>>
			{
				db.NumericEntities.Where(o => o.Short == value),
				db.NumericEntities.Where(o => o.Short != value),
				db.NumericEntities.Where(o => o.Short >= value),
				db.NumericEntities.Where(o => o.Short <= value),
				db.NumericEntities.Where(o => o.Short > value),
				db.NumericEntities.Where(o => o.Short < value),

				db.NumericEntities.Where(o => o.NullableShort == value),
				db.NumericEntities.Where(o => o.NullableShort != value),
				db.NumericEntities.Where(o => o.NullableShort >= value),
				db.NumericEntities.Where(o => o.NullableShort <= value),
				db.NumericEntities.Where(o => o.NullableShort > value),
				db.NumericEntities.Where(o => o.NullableShort < value),

				db.NumericEntities.Where(o => o.NullableShort.Value == value),
				db.NumericEntities.Where(o => o.NullableShort.Value != value),
				db.NumericEntities.Where(o => o.NullableShort.Value >= value),
				db.NumericEntities.Where(o => o.NullableShort.Value <= value),
				db.NumericEntities.Where(o => o.NullableShort.Value > value),
				db.NumericEntities.Where(o => o.NullableShort.Value < value)
			};

			foreach (var query in queriables)
			{
				AssertTotalParameters(
					query,
					1,
					sql => Assert.That(sql, Does.Not.Contain("cast")));
			}

			if (Sfi.Dialect is FirebirdDialect)
			{
				Assert.Ignore("Due to the regex bug in FirebirdClientDriver, the parameters are not casted.");
			}

			queriables = new List<IQueryable<NumericEntity>>
			{
				db.NumericEntities.Where(o => o.Short + value > value),
				db.NumericEntities.Where(o => o.Short - value > value),
				db.NumericEntities.Where(o => o.Short * value > value),

				db.NumericEntities.Where(o => o.NullableShort + value > value),
				db.NumericEntities.Where(o => o.NullableShort - value > value),
				db.NumericEntities.Where(o => o.NullableShort * value > value),

				db.NumericEntities.Where(o => o.NullableShort.Value + value > value),
				db.NumericEntities.Where(o => o.NullableShort.Value - value > value),
				db.NumericEntities.Where(o => o.NullableShort.Value * value > value),
			};

			var sameType = Sfi.Dialect.TryGetCastTypeName(NHibernateUtil.Int16.SqlType, out var shortCast) &&
			               Sfi.Dialect.TryGetCastTypeName(NHibernateUtil.Int32.SqlType, out var intCast) &&
			               shortCast == intCast;
			foreach (var query in queriables)
			{
				AssertTotalParameters(
					query,
					1,
					sql => {
						// SQLiteDialect uses sql cast for transparentcast method
						Assert.That(sql, !sameType || Sfi.Dialect is SQLiteDialect ? Does.Match("where\\s+cast") : (IResolveConstraint)Does.Not.Contain("cast"));
						Assert.That(GetTotalOccurrences(sql, "cast"), Is.EqualTo(!sameType || Sfi.Dialect is SQLiteDialect ? 1 : 0));
					});
			}
		}

		[Test]
		public void UsingValueTypeParameterTwiceOnNullableProperty()
		{
			short value = 1;
			AssertTotalParameters(
				db.NumericEntities.Where(o => o.NullableShort == value && o.NullableShort != value && o.Short == value),
				1, sql => {
				
				Assert.That(GetTotalOccurrences(sql, "cast"), Is.EqualTo(0));
			});
		}

		[Test]
		public void UsingValueTypeParameterOnDifferentProperties()
		{
			int value = 1;
			AssertTotalParameters(
				db.NumericEntities.Where(o => o.NullableShort == value && o.NullableShort != value && o.Integer == value),
				1);

			AssertTotalParameters(
				db.NumericEntities.Where(o => o.Integer == value && o.NullableShort == value && o.NullableShort != value),
				1);
		}

		[Test]
		public void UsingParameterInEvaluatableExpression()
		{
			var value = "test";
			db.Orders.Where(o => string.Format("{0}", value) != o.ShippedTo).ToList();
			db.Orders.Where(o => $"{value}_" != o.ShippedTo).ToList();
			db.Orders.Where(o => string.Copy(value) != o.ShippedTo).ToList();

			var guid = Guid.Parse("2D7E6EB3-BD08-4A40-A4E7-5150F7895821");
			db.Orders.Where(o => o.ShippedTo.Contains($"VALUE {guid}")).ToList();

			var names = new[] {"name"};
			db.Users.Where(x => names.Length == 0 || names.Contains(x.Name)).ToList();
			names = new string[] { };
			db.Users.Where(x => names.Length == 0 || names.Contains(x.Name)).ToList();
		}

		[Test]
		public void UsingParameterWithImplicitOperator()
		{
			var id = new GuidImplicitWrapper(new Guid("{356E4A7E-B027-4321-BA40-E2677E6502CF}"));
			Assert.That(db.Shippers.Where(o => o.Reference == id).ToList(), Has.Count.EqualTo(1));

			id = new GuidImplicitWrapper(new Guid("{356E4A7E-B027-4321-BA40-E2677E6502FF}"));
			Assert.That(db.Shippers.Where(o => o.Reference == id).ToList(), Is.Empty);

			AssertTotalParameters(
				db.Shippers.Where(o => o.Reference == id && id == o.Reference),
				1);
		}

		private struct GuidImplicitWrapper
		{
			public readonly Guid Id;

			public GuidImplicitWrapper(Guid id)
			{
				Id = id;
			}

			public static implicit operator Guid(GuidImplicitWrapper idWrapper)
			{
				return idWrapper.Id;
			}
		}

		[Test]
		public void UsingParameterWithExplicitOperator()
		{
			var id = new GuidExplicitWrapper(new Guid("{356E4A7E-B027-4321-BA40-E2677E6502CF}"));
			Assert.That(db.Shippers.Where(o => o.Reference == (Guid) id).ToList(), Has.Count.EqualTo(1));

			id = new GuidExplicitWrapper(new Guid("{356E4A7E-B027-4321-BA40-E2677E6502FF}"));
			Assert.That(db.Shippers.Where(o => o.Reference == (Guid) id).ToList(), Is.Empty);

			AssertTotalParameters(
				db.Shippers.Where(o => o.Reference == (Guid) id && (Guid) id == o.Reference),
				1);
		}

		private struct GuidExplicitWrapper
		{
			public readonly Guid Id;

			public GuidExplicitWrapper(Guid id)
			{
				Id = id;
			}

			public static explicit operator Guid(GuidExplicitWrapper idWrapper)
			{
				return idWrapper.Id;
			}
		}

		[Test]
		public void UsingParameterOnSelectors()
		{
			var user = new User() {Id = 1};
			db.Users.Where(o => o == user).ToList();
			db.Users.FirstOrDefault(o => o == user);
			db.Timesheets.Where(o => o.Users.Any(u => u == user)).ToList();

			var users = new[] {new User() {Id = 1}};
			db.Users.Where(o => users.Contains(o)).ToList();
			db.Users.FirstOrDefault(o => users.Contains(o));
			db.Timesheets.Where(o => o.Users.Any(u => users.Contains(u))).ToList();
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

		[Test(Description = "GH-2872")]
		public void UsingListParameterWithWhere()
		{
			var ids = db.Orders.OrderBy(x => x.OrderId).Take(2).Select(o => o.OrderId).ToList();
			AssertTotalParameters(
				db.Orders.Where(o => ids.Where(i => i == ids[0]).Contains(o.OrderId)),
				1,
				countResults: 1);
		}

		[Test(Description = "GH-2276")]
		public void UsingArrayParameterWithWhereAndSelect()
		{
			var ids = db.Orders.OrderBy(x => x.OrderId).Take(2).ToArray();
			var orderLines = new[] {ids[0].OrderLines.First(), ids[1].OrderLines.First()};
			AssertTotalParameters(
				db.Orders.Where(o => ids.Where(i => i == ids[0]).Contains(o) && orderLines.Select(ol => ol.Order).Where(i => i.OrderId == ids[0].OrderId).Contains(o)),
				2,
				countResults: 1);
		}

		private void AssertTotalParameters<T>(IQueryable<T> query, int parameterNumber, Action<string> sqlAction)
		{
			AssertTotalParameters(query, parameterNumber, null, sqlAction);
		}

		private void AssertTotalParameters<T>(IQueryable<T> query, int parameterNumber, int? linqParameterNumber = null, Action<string> sqlAction = null, int? countResults = null)
		{
			using (var sqlSpy = new SqlLogSpy())
			{
				var queryPlanCacheType = typeof(QueryPlanCache);
				var cache = (SoftLimitMRUCache)
					queryPlanCacheType
						.GetField("planCache", BindingFlags.Instance | BindingFlags.NonPublic)
						.GetValue(Sfi.QueryPlanCache);
				cache.Clear();

				var results = query.ToList();

				// In case of arrays linqParameterNumber and parameterNumber will be different
				Assert.That(
					GetLinqExpression(query).ParameterValuesByName.Count,
					Is.EqualTo(linqParameterNumber ?? parameterNumber),
					"Linq expression has different number of parameters");
				if(countResults != null)
					Assert.That(results.Count, Is.EqualTo(countResults), "Unexpected results count");

				sqlAction?.Invoke(sqlSpy.GetWholeLog());

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
