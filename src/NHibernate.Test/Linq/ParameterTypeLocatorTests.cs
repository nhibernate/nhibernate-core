using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
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
	public class ParameterTypeLocatorTests : LinqTestCase
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
		public void EqualStringEnumTestWithFetch()
		{
			AssertResults(
				new Dictionary<string, Predicate<IType>>
				{
					{"3", o => o is EnumStoredAsStringType}
				},
				db.Users.Fetch(o => o.Role).ThenFetch(o => o.ParentRole).Where(o => o.Enum1 == EnumStoredAsString.Large),
				db.Users.Fetch(o => o.Role).ThenFetch(o => o.ParentRole).Where(o => EnumStoredAsString.Large == o.Enum1)
			);
		}

		[Test]
		public void EqualStringEnumTestWithSubQuery()
		{
			AssertResults(
				new Dictionary<string, Predicate<IType>>
				{
					{"3", o => o is EnumStoredAsStringType}
				},
				db.Users.Where(o => db.Users.Any(u => u.Enum1 == EnumStoredAsString.Large)),
				db.Users.Where(o => db.Users.Any(u => EnumStoredAsString.Large == u.Enum1))
			);
		}

		[Test]
		public void EqualStringEnumTestWithMaxSubQuery()
		{
			AssertResults(
				new Dictionary<string, Predicate<IType>>
				{
					{"3", o => o is EnumStoredAsStringType}
				},
				db.Users.Fetch(o => o.Role).Where(o => db.Users.Max(u => u.Enum1 == EnumStoredAsString.Large ? u.Id : -u.Id) == o.Id),
				db.Users.Fetch(o => o.Role).Where(o => db.Users.Max(u => EnumStoredAsString.Large == u.Enum1 ? u.Id : -u.Id) == o.Id),
				db.Users.Where(o => db.Users.Max(u => u.Enum1 == EnumStoredAsString.Large ? u.Id : -u.Id) == o.Id),
				db.Users.Where(o => db.Users.Max(u => EnumStoredAsString.Large == u.Enum1 ? u.Id : -u.Id) == o.Id)
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
		public void EqualEntityTest()
		{
			var order = new Order();
			AssertResults(
				new Dictionary<string, Predicate<IType>>
				{
					{
						$"value({typeof(Order).FullName})",
						o => o is ManyToOneType manyToOne && manyToOne.Name == typeof(Order).FullName
					}
				},
				db.Orders.Where(o => o == order),
				db.Orders.Where(o => order == o)
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
					{"Unspecified", o => o is EnumStoredAsStringType}
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
					{"Unspecified", o => o is EnumStoredAsStringType}
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
		public void DynamicMemberTest()
		{
			AssertResults(
				new Dictionary<string, Predicate<IType>>
				{
					{"\"test\"", o => o is AnsiStringType},
				},
				db.DynamicUsers.Where("Properties.Name == @0", "test"),
				db.DynamicUsers.Where("@0 == Properties.Name", "test")
			);
		}

		[Test]
		public void DynamicDictionaryMemberTest()
		{
			AssertResults(
				new Dictionary<string, Predicate<IType>>
				{
					{"\"test\"", o => o is AnsiStringType},
				},
#pragma warning disable CS0252
				db.DynamicUsers.Where(o => o.Settings["Property1"] == "test"),
#pragma warning restore CS0252
#pragma warning disable CS0253
				db.DynamicUsers.Where(o => "test" == o.Settings["Property1"])
#pragma warning restore CS0253
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
			var parameters = ExpressionParameterVisitor.Visit(result);
			expression = result.Expression;
			var queryModel = NhRelinqQueryParser.Parse(expression);
			ParameterTypeLocator.SetParameterTypes(parameters, queryModel, targetType, Sfi);
			Assert.That(parameters.Count, Is.EqualTo(expectedResults.Count), "Incorrect number of parameters");
			foreach (var pair in parameters)
			{
				var origCulture = CultureInfo.CurrentCulture;
				try
				{
					CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
					var expressionText = pair.Key.ToString();
					Assert.That(expectedResults.ContainsKey(expressionText), Is.True, $"{expressionText} constant is not expected");
					Assert.That(expectedResults[expressionText](pair.Value.Type), Is.True, $"Invalid type, actual type: {pair.Value?.Type?.Name ?? "null"}");
				}
				finally
				{
					CultureInfo.CurrentCulture = origCulture;
				}
			}
		}
	}
}
