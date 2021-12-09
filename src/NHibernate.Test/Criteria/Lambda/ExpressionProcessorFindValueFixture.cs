using System;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Impl;
using NUnit.Framework;
using Expression = System.Linq.Expressions.Expression;

namespace NHibernate.Test.Criteria.Lambda
{
	public static class DateTimeExtensions
	{
		public static long ToLongDateTime(this DateTime date)
		{
			return Convert.ToInt64(date.ToString("yyyyMMddHHmmss"));
		}
	}

	[TestFixture]
	public class ExpressionProcessorFindValueFixture
	{
		private static object GetValue<T>(Expression<Func<T>> expression)
		{
			try
			{
				return ExpressionProcessor.FindValue(expression.Body);
			}
			catch (TargetInvocationException e)
			{
				throw e.InnerException;
			}
		}

		private static int GetIntegerDate()
		{
			return Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
		}

		[Test]
		public void StaticPropertyInstanceMethodCall()
		{
			var actual = GetValue(() => DateTime.Now.ToString("yyyyMMdd"));
			var expected = DateTime.Now.ToString("yyyyMMdd");

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void StaticPropertyInstanceMultipleMethodCall()
		{
			var actual = GetValue(() => DateTime.Now.AddDays(365).ToString("yyyyMMdd"));
			var expected = DateTime.Now.AddDays(365).ToString("yyyyMMdd");

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void StaticPropertyInstanceMethodCallThenCast()
		{
			var actual = GetValue(() => Convert.ToInt32(DateTime.Now.AddDays(365).ToString("yyyyMMdd")));
			var expected = Convert.ToInt32(DateTime.Now.AddDays(365).ToString("yyyyMMdd"));

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void StaticMethodCall()
		{
			var actual = GetValue(() => GetIntegerDate());
			var expected = GetIntegerDate();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ExtensionMethodCall()
		{
			var date = DateTime.Now;
			var actual = GetValue(() => date.ToLongDateTime());
			var expected = date.ToLongDateTime();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void NestedPropertyAccess()
		{
			var animal = new { Snake = new { Animal = new { Name = "Scorpion" } } };
			var actual = GetValue(() => animal.Snake.Animal.Name);
			var expected = animal.Snake.Animal.Name;

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void GuidToStringCast()
		{
			var guid = Guid.NewGuid();
			Expression<Func<string>> expression = () => $"{guid}";
			var actual = GetValue(expression);

			//Check with expression compile and invoke
			var lambdaExpression = Expression.Lambda(expression).Compile();
			var expected = ((dynamic) lambdaExpression.DynamicInvoke()).Invoke();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void NestedPropertyToIntegerCast()
		{
			var animal = new { Snake = new { Animal = new { Weight = 9.89 } } };
			Expression<Func<int>> expression = () => (int) animal.Snake.Animal.Weight;
			var actual = GetValue(expression);

			//Check with expression compile and invoke
			var lambdaExpression = Expression.Lambda(expression).Compile();
			var expected = ((dynamic) lambdaExpression.DynamicInvoke()).Invoke();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void NestedPropertyToIntegerConvert()
		{
			var animal = new { Snake = new { Animal = new { Weight = 9.89 } } };
			Expression<Func<int>> expression = () => Convert.ToInt32(animal.Snake.Animal.Weight);
			var actual = GetValue(expression);

			//Check with expression compile and invoke
			var lambdaExpression = Expression.Lambda(expression).Compile();
			var expected = ((dynamic) lambdaExpression.DynamicInvoke()).Invoke();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void NullToIntegerCastFails()
		{
			object value = null;
			Expression<Func<int>> expression = () => (int) value;

			Assert.Throws<NullReferenceException>(() => GetValue(expression));

			//Check with expression compile and invoke
			var lambdaExpression = Expression.Lambda(expression).Compile();

			Assert.Throws<NullReferenceException>(() => ((dynamic) lambdaExpression.DynamicInvoke()).Invoke());
		}

		[Test]
		public void NullableIntegerToIntegerCastFails()
		{
			int? value = null;
			Expression<Func<int>> expression = () => (int) value;

			Assert.Throws<InvalidOperationException>(() => GetValue(expression));

			//Check with expression compile and invoke
			var lambdaExpression = Expression.Lambda(expression).Compile();

			Assert.Throws<InvalidOperationException>(() => ((dynamic) lambdaExpression.DynamicInvoke()).Invoke());
		}

		[Test]
		public void NullableIntegerToIntegerCast()
		{
			int? value = 1;
			Expression<Func<int>> expression = () => (int) value;

			var actual = GetValue(expression);

			//Check with expression compile and invoke
			var lambdaExpression = Expression.Lambda(expression).Compile();

			var expected = ((dynamic) lambdaExpression.DynamicInvoke()).Invoke();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void NullableIntegerToNullableLongImplicitCast()
		{
			int? value = 1;
			Expression<Func<long?>> expression = () => value;

			var actual = GetValue(expression);

			//Check with expression compile and invoke
			var lambdaExpression = Expression.Lambda(expression).Compile();

			var expected = ((dynamic) lambdaExpression.DynamicInvoke()).Invoke();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void IntegerToIntegerCast()
		{
			int value = 1;
			Expression<Func<int>> expression = () => (int) value;

			var actual = GetValue(expression);

			//Check with expression compile and invoke
			var lambdaExpression = Expression.Lambda(expression).Compile();

			var expected = ((dynamic) lambdaExpression.DynamicInvoke()).Invoke();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void IntegerToNullableIntegerImplicitCast()
		{
			int value = 12345;
			Expression<Func<int?>> expression = () => value;

			var actual = GetValue(expression);

			//Check with expression compile and invoke
			var lambdaExpression = Expression.Lambda(expression).Compile();

			var expected = ((dynamic) lambdaExpression.DynamicInvoke()).Invoke();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void IntegerToNullableLongImplicitCast()
		{
			int value = 12345;
			Expression<Func<long?>> expression = () => value;

			var actual = GetValue(expression);

			//Check with expression compile and invoke
			var lambdaExpression = Expression.Lambda(expression).Compile();

			var expected = ((dynamic) lambdaExpression.DynamicInvoke()).Invoke();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void IntegerToNullableDecimalImplicitCast()
		{
			int value = 12345;
			Expression<Func<decimal?>> expression = () => value;

			var actual = GetValue(expression);

			//Check with expression compile and invoke
			var lambdaExpression = Expression.Lambda(expression).Compile();

			var expected = ((dynamic) lambdaExpression.DynamicInvoke()).Invoke();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Int16ToIntegerImplicitCast()
		{
			short value = 12345;
			Expression<Func<int>> expression = () => value;

			var actual = GetValue(expression);

			//Check with expression compile and invoke
			var lambdaExpression = Expression.Lambda(expression).Compile();

			var expected = ((dynamic) lambdaExpression.DynamicInvoke()).Invoke();

			Assert.AreEqual(expected, actual);

		}

		[Test]
		public void NullableDecimalToDecimalCast()
		{
			decimal? value = 9.89m;
			Expression<Func<decimal>> expression = () => (decimal) value;

			var actual = GetValue(expression);

			//Check with expression compile and invoke
			var lambdaExpression = Expression.Lambda(expression).Compile();
			var expected = ((dynamic) lambdaExpression.DynamicInvoke()).Invoke();

			Assert.AreEqual(expected, actual);

		}

		[Test]
		public void StringToIntegerCastFails()
		{
			object value = "Abatay";
			Expression<Func<int>> expression = () => (int) value;

			Assert.Throws<InvalidCastException>(() => GetValue(expression));

			//Check with expression compile and invoke
			var lambdaExpression = Expression.Lambda(expression).Compile();

			Assert.Throws<InvalidCastException>(() => ((dynamic) lambdaExpression.DynamicInvoke()).Invoke());
		}

		[Test]
		public void BooleanToCharCastFails()
		{
			object isTrue = true;
			Expression<Func<char>> expression = () => (char) isTrue;

			Assert.Throws<InvalidCastException>(() => GetValue(expression));

			//Check with expression compile and invoke
			var lambdaExpression = Expression.Lambda(expression).Compile();

			Assert.Throws<InvalidCastException>(() => ((dynamic) lambdaExpression.DynamicInvoke()).Invoke());
		}
	}
}
