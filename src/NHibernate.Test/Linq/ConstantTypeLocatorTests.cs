using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Engine.Query;
using NHibernate.Linq;
using NHibernate.Linq.Visitors;
using NHibernate.Type;
using NUnit.Framework;
using Remotion.Linq.Clauses;

namespace NHibernate.Test.Linq
{
	public class ConstantTypeLocatorTests : LinqTestCase
	{
		[Test]
		public void AddIntegerTest()
		{
			AssertResults(
				new Dictionary<string, Predicate<IType>>
				{
					{"2.1", o => o is DoubleType},
					{"5", o => o is Int32Type},
				},
				db.Users.Where(o => o.Id + 5 > 2.1),
				db.Users.Where(o => 2.1 < 5 + o.Id)
			);
		}

		[Test]
		public void AddDecimalTest()
		{
			AssertResults(
				new Dictionary<string, Predicate<IType>>
				{
					{"2.1", o => o is DecimalType},
					{"5.2", o => o is DecimalType},
				},
				db.Users.Where(o => o.Id + 5.2m > 2.1m),
				db.Users.Where(o => 2.1m < 5.2m + o.Id)
			);
		}

		[Test]
		public void SubtractFloatTest()
		{
			AssertResults(
				new Dictionary<string, Predicate<IType>>
				{
					{"2.1", o => o is DoubleType},
					{"5.2", o => o is SingleType},
				},
				db.Users.Where(o => o.Id - 5.2f > 2.1),
				db.Users.Where(o => 2.1 < 5.2f - o.Id)
			);
		}

		[Test]
		public void GreaterThanTest()
		{
			AssertResults(
				new Dictionary<string, Predicate<IType>>
				{
					{"2.1", o => o is Int32Type}
				},
				db.Users.Where(o => o.Id > 2.1),
				db.Users.Where(o => 2.1 > o.Id)
			);
		}

		[Test]
		public void EqualStringEnumTest()
		{
			AssertResults(
				new Dictionary<string, Predicate<IType>>
				{
					{"3", o => o is EnumStoredAsStringType}
				},
				db.Users.Where(o => o.Enum1 == EnumStoredAsString.Large),
				db.Users.Where(o => EnumStoredAsString.Large == o.Enum1)
			);
		}

		[Test]
		public void EqualStringTest()
		{
			AssertResults(
				new Dictionary<string, Predicate<IType>>
				{
					{"\"London\"", o => o is StringType stringType && stringType.SqlType.Length == 15}
				},
				db.Orders.Where(o => o.ShippingAddress.City == "London"),
				db.Orders.Where(o => "London" == o.ShippingAddress.City)
			);
		}

		[Test]
		public void DoubleEqualTest()
		{
			AssertResults(
				new Dictionary<string, Predicate<IType>>
				{
					{"3", o => o is EnumStoredAsStringType},
					{"1", o => o is PersistentEnumType}
				},
				db.Users.Where(o => o.Enum1 == EnumStoredAsString.Large && o.Enum2 == EnumStoredAsInt32.High),
				db.Users.Where(o => EnumStoredAsInt32.High == o.Enum2 && EnumStoredAsString.Large == o.Enum1)
			);
		}

		[Test]
		public void NotEqualTest()
		{
			AssertResults(
				new Dictionary<string, Predicate<IType>>
				{
					{"3", o => o is EnumStoredAsStringType}
				},
				db.Users.Where(o => o.Enum1 != EnumStoredAsString.Large),
				db.Users.Where(o => EnumStoredAsString.Large != o.Enum1)
			);
		}

		[Test]
		public void DoubleNotEqualTest()
		{
			AssertResults(
				new Dictionary<string, Predicate<IType>>
				{
					{"3", o => o is EnumStoredAsStringType},
					{"1", o => o is PersistentEnumType}
				},
				db.Users.Where(o => o.Enum1 != EnumStoredAsString.Large || o.NullableEnum2 != EnumStoredAsInt32.High),
				db.Users.Where(o => EnumStoredAsInt32.High != o.NullableEnum2 || o.Enum1 != EnumStoredAsString.Large)
			);
		}

		[Test]
		public void CoalesceTest()
		{
			AssertResults(
				new Dictionary<string, Predicate<IType>>
				{
					{"2", o => o is EnumStoredAsStringType},
					{"Large", o => o is EnumStoredAsStringType}
				},
				db.Users.Where(o => (o.NullableEnum1 ?? EnumStoredAsString.Large) == EnumStoredAsString.Medium),
				db.Users.Where(o => EnumStoredAsString.Medium == (o.NullableEnum1 ?? EnumStoredAsString.Large))
			);
		}

		[Test]
		public void DoubleCoalesceTest()
		{
			AssertResults(
				new Dictionary<string, Predicate<IType>>
				{
					{"2", o => o is EnumStoredAsStringType},
					{"Large", o => o is EnumStoredAsStringType},
				},
				db.Users.Where(o => ((o.NullableEnum1 ?? (EnumStoredAsString?) EnumStoredAsString.Large) ?? o.Enum1) == EnumStoredAsString.Medium),
				db.Users.Where(o => EnumStoredAsString.Medium == ((o.NullableEnum1 ?? (EnumStoredAsString?) EnumStoredAsString.Large) ?? o.Enum1))
			);
		}

		[Test]
		public void ConditionalTest()
		{
			AssertResults(
				new Dictionary<string, Predicate<IType>>
				{
					{"2", o => o is EnumStoredAsStringType},
					{"Unspecified", o => o is EnumStoredAsStringType},
					{"null", o => o is PersistentEnumType}, // HasValue
				},
				db.Users.Where(o => (o.NullableEnum2.HasValue ? o.Enum1 : EnumStoredAsString.Unspecified) == EnumStoredAsString.Medium),
				db.Users.Where(o => EnumStoredAsString.Medium == (o.NullableEnum2.HasValue ? EnumStoredAsString.Unspecified : o.Enum1))
			);
		}

		[Test]
		public void DoubleConditionalTest()
		{
			AssertResults(
				new Dictionary<string, Predicate<IType>>
				{
					{"0", o => o is PersistentEnumType},
					{"2", o => o is EnumStoredAsStringType},
					{"Small", o => o is EnumStoredAsStringType},
					{"Unspecified", o => o is EnumStoredAsStringType},
					{"null", o => o is PersistentEnumType}, // HasValue
				},
				db.Users.Where(o => (o.Enum2 != EnumStoredAsInt32.Unspecified
										? (o.NullableEnum2.HasValue ? o.Enum1 : EnumStoredAsString.Unspecified)
										: EnumStoredAsString.Small) == EnumStoredAsString.Medium),
				db.Users.Where(o => EnumStoredAsString.Medium == (o.Enum2 != EnumStoredAsInt32.Unspecified
										? EnumStoredAsString.Small
										: (o.NullableEnum2.HasValue ? EnumStoredAsString.Unspecified : o.Enum1)))
			);
		}

		[Test]
		public void CoalesceMemberTest()
		{
			AssertResults(
				new Dictionary<string, Predicate<IType>>
				{
					{"2", o => o is EnumStoredAsStringType}
				},
				db.Users.Where(o => (o.NotMappedUser ?? o).Enum1 == EnumStoredAsString.Medium),
				db.Users.Where(o => EnumStoredAsString.Medium == (o ?? o.NotMappedUser).Enum1)
			);
		}

		[Test]
		public void ConditionalMemberTest()
		{
			AssertResults(
				new Dictionary<string, Predicate<IType>>
				{
					{"2", o => o is EnumStoredAsStringType},
					{"\"test\"", o => o is AnsiStringType},
				},
				db.Users.Where(o => (o.Name == "test" ? o.NotMappedUser : o).Enum1 == EnumStoredAsString.Medium),
				db.Users.Where(o => EnumStoredAsString.Medium == (o.Name == "test" ? o : o.NotMappedUser).Enum1)
			);
		}

		[Test]
		public void AssignMemberTest()
		{
			AssertResult(
				new Dictionary<string, Predicate<IType>>
				{
					{"0", o => o is Int32Type},
					{"\"val\"", o => o is AnsiStringType},
					{"Large", o => o is EnumStoredAsStringType},
				},
				QueryMode.Insert,
				db.Users.Where(o => o.InvalidLoginAttempts > 0),
				o => new User {Name = "val", Enum1 = EnumStoredAsString.Large}
			);
		}

		[Test]
		public void AssignComponentMemberTest()
		{
			AssertResult(
				new Dictionary<string, Predicate<IType>>
				{
					{"0", o => o is Int32Type},
					{"\"prop1\"", o => o is AnsiStringType}
				},
				QueryMode.Insert,
				db.Users.Where(o => o.InvalidLoginAttempts > 0),
				o => new User {Component = new UserComponent {Property1 = "prop1"}}
			);
		}

		[Test]
		public void AssignNestedComponentMemberTest()
		{
			AssertResult(
				new Dictionary<string, Predicate<IType>>
				{
					{"0", o => o is Int32Type},
					{"\"other\"", o => o is AnsiStringType}
				},
				QueryMode.Insert,
				db.Users.Where(o => o.InvalidLoginAttempts > 0),
				o => new User
				{
					Component = new UserComponent {OtherComponent = new UserComponent2 {OtherProperty1 = "other"}}
				}
			);
		}

		[Test]
		public void AnonymousAssignMemberTest()
		{
			AssertResult(
				new Dictionary<string, Predicate<IType>>
				{
					{"0", o => o is Int32Type},
					{"\"val\"", o => o is AnsiStringType},
					{"Large", o => o is EnumStoredAsStringType},
				},
				QueryMode.Insert,
				db.Users.Where(o => o.InvalidLoginAttempts > 0),
				o => new {Name = "val", Enum1 = EnumStoredAsString.Large}
			);
		}

		[Test]
		public void AnonymousAssignComponentMemberTest()
		{
			AssertResult(
				new Dictionary<string, Predicate<IType>>
				{
					{"0", o => o is Int32Type},
					{"\"prop1\"", o => o is AnsiStringType}
				},
				QueryMode.Insert,
				db.Users.Where(o => o.InvalidLoginAttempts > 0),
				o => new {Component = new {Property1 = "prop1"}}
			);
		}

		[Test]
		public void AnonymousAssignNestedComponentMemberTest()
		{
			AssertResult(
				new Dictionary<string, Predicate<IType>>
				{
					{"0", o => o is Int32Type},
					{"\"other\"", o => o is AnsiStringType}
				},
				QueryMode.Insert,
				db.Users.Where(o => o.InvalidLoginAttempts > 0),
				o => new {Component = new {OtherComponent = new {OtherProperty1 = "other"}}}
			);
		}

		private void AssertResults(
			Dictionary<string, Predicate<IType>> expectedResults,
			params IQueryable[] queries)
		{
			foreach (var query in queries)
			{
				AssertResult(expectedResults, query);
			}
		}

		private void AssertResult(
			Dictionary<string, Predicate<IType>> expectedResults,
			IQueryable query)
		{
			AssertResult(expectedResults, QueryMode.Select, query.Expression, query.Expression.Type);
		}

		private void AssertResult<T>(
			Dictionary<string, Predicate<IType>> expectedResults,
			QueryMode queryMode,
			IQueryable<T> query,
			Expression<Func<T, T>> expression)
		{
			var dmlExpression = expression != null
				? DmlExpressionRewriter.PrepareExpression(query.Expression, expression)
				: query.Expression;

			AssertResult(expectedResults, queryMode, dmlExpression, typeof(T));
		}

		private void AssertResult<T>(
			Dictionary<string, Predicate<IType>> expectedResults,
			QueryMode queryMode,
			IQueryable<T> query,
			Expression<Func<T, object>> expression)
		{
			var dmlExpression = expression != null
				? DmlExpressionRewriter.PrepareExpressionFromAnonymous(query.Expression, expression)
				: query.Expression;

			AssertResult(expectedResults, queryMode, dmlExpression, typeof(T));
		}

		private void AssertResult(
			Dictionary<string, Predicate<IType>> expectedResults,
			QueryMode queryMode,
			Expression expression,
			System.Type targetType)
		{
			var result = NhRelinqQueryParser.PreTransform(expression, new PreTransformationParameters(queryMode, Sfi));
			expression = result.Expression;
			var queryModel = NhRelinqQueryParser.Parse(expression);
			var types = ConstantTypeLocator.GetTypes(queryModel, targetType, Sfi);
			Assert.That(types.Count, Is.EqualTo(expectedResults.Count), "Incorrect number of constants");
			foreach (var pair in types)
			{
				var origCulture = CultureInfo.CurrentCulture;
				try
				{
					CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
					var expressionText = pair.Key.ToString();
					Assert.That(expectedResults.ContainsKey(expressionText), Is.True, $"{expressionText} constant is not expected");
					Assert.That(expectedResults[expressionText](pair.Value), Is.True, $"Invalid type, actual type: {pair.Value?.Name ?? "null"}");
				}
				finally
				{
					CultureInfo.CurrentCulture = origCulture;
				}
			}
		}
	}
}
