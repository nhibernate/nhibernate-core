using System;
using System.Linq.Expressions;

using NHibernate.Impl;
using NHibernate.Type;

namespace NHibernate.Criterion
{
	public static class ProjectionsExtensions
	{
		/// <summary>
		/// Create an alias for a projection
		/// </summary>
		/// <param name="projection">the projection instance</param>
		/// <param name="alias">LambdaExpression returning an alias</param>
		/// <returns>return NHibernate.Criterion.IProjection</returns>
		public static IProjection WithAlias(this IProjection			projection,
											Expression<Func<object>>    alias)
		{
			string aliasContainer = ExpressionProcessor.FindPropertyExpression(alias.Body);
			return Projections.Alias(projection, aliasContainer);
		}

		/// <summary>
		/// Create an alias for a projection
		/// </summary>
		/// <param name="projection">the projection instance</param>
		/// <param name="alias">alias</param>
		/// <returns>return NHibernate.Criterion.IProjection</returns>
		public static IProjection WithAlias(this IProjection projection, string alias)
		{
			return Projections.Alias(projection, alias);
		}

		internal static IProjection ProcessYear(System.Linq.Expressions.Expression expression)
		{
			IProjection property = ExpressionProcessor.FindMemberProjection(expression).AsProjection();
			return Projections.SqlFunction("year", NHibernateUtil.Int32, property);
		}

		internal static IProjection ProcessDay(System.Linq.Expressions.Expression expression)
		{
			IProjection property = ExpressionProcessor.FindMemberProjection(expression).AsProjection();
			return Projections.SqlFunction("day", NHibernateUtil.Int32, property);
		}

		internal static IProjection ProcessMonth(System.Linq.Expressions.Expression expression)
		{
			return SqlFunction("month", NHibernateUtil.Int32, expression);
		}

		private static IProjection SqlFunction(string name, IType type, System.Linq.Expressions.Expression projection)
		{
			IProjection property = ExpressionProcessor.FindMemberProjection(projection).AsProjection();
			return Projections.SqlFunction(name, type, property);
		}

		internal static IProjection ProcessHour(System.Linq.Expressions.Expression expression)
		{
			return SqlFunction("hour", NHibernateUtil.Int32, expression);
		}

		internal static IProjection ProcessMinute(System.Linq.Expressions.Expression expression)
		{
			return SqlFunction("minute", NHibernateUtil.Int32, expression);
		}

		internal static IProjection ProcessSecond(System.Linq.Expressions.Expression expression)
		{
			return SqlFunction("second", NHibernateUtil.Int32, expression);
		}

		internal static IProjection ProcessDate(System.Linq.Expressions.Expression expression)
		{
			return SqlFunction("date", NHibernateUtil.Date, expression);
		}

		/// <summary>
		/// Project SQL function sqrt()
		/// Note: throws an exception outside of a QueryOver expression
		/// </summary>
		public static double Sqrt(this double numericProperty)
		{
			throw QueryOver.GetDirectUsageException();
		}

		/// <summary>
		/// Project SQL function sqrt()
		/// Note: throws an exception outside of a QueryOver expression
		/// </summary>
		public static double Sqrt(this int numericProperty)
		{
			throw QueryOver.GetDirectUsageException();
		}

		/// <summary>
		/// Project SQL function sqrt()
		/// Note: throws an exception outside of a QueryOver expression
		/// </summary>
		public static double Sqrt(this long numericProperty)
		{
			throw QueryOver.GetDirectUsageException();
		}

		/// <summary>
		/// Project SQL function sqrt()
		/// Note: throws an exception outside of a QueryOver expression
		/// </summary>
		public static double Sqrt(this decimal numericProperty)
		{
			throw QueryOver.GetDirectUsageException();
		}

		/// <summary>
		/// Project SQL function sqrt()
		/// Note: throws an exception outside of a QueryOver expression
		/// </summary>
		public static double Sqrt(this byte numericProperty)
		{
			throw QueryOver.GetDirectUsageException();
		}

		internal static IProjection ProcessSqrt(MethodCallExpression methodCallExpression)
		{
			IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
			return Projections.SqlFunction("sqrt", NHibernateUtil.Double, property);
		}

		/// <summary>
		/// Project SQL function lower()
		/// Note: throws an exception outside of a QueryOver expression
		/// </summary>
		public static string Lower(this string stringProperty)
		{
			throw QueryOver.GetDirectUsageException();
		}

		internal static IProjection ProcessLower(MethodCallExpression methodCallExpression)
		{
			IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
			return Projections.SqlFunction("lower", NHibernateUtil.String, property);
		}

		/// <summary>
		/// Project SQL function upper()
		/// Note: throws an exception outside of a QueryOver expression
		/// </summary>
		public static string Upper(this string stringProperty)
		{
			throw QueryOver.GetDirectUsageException();
		}

		internal static IProjection ProcessUpper(MethodCallExpression methodCallExpression)
		{
			IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
			return Projections.SqlFunction("upper", NHibernateUtil.String, property);
		}

		/// <summary>
		/// Project SQL function abs()
		/// Note: throws an exception outside of a QueryOver expression
		/// </summary>
		public static int Abs(this int numericProperty)
		{
			throw QueryOver.GetDirectUsageException();
		}

		internal static IProjection ProcessIntAbs(MethodCallExpression methodCallExpression)
		{
			IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
			return Projections.SqlFunction("abs", NHibernateUtil.Int32, property);
		}

		/// <summary>
		/// Project SQL function abs()
		/// Note: throws an exception outside of a QueryOver expression
		/// </summary>
		public static Int64 Abs(this Int64 numericProperty)
		{
			throw QueryOver.GetDirectUsageException();
		}

		internal static IProjection ProcessInt64Abs(MethodCallExpression methodCallExpression)
		{
			IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
			return Projections.SqlFunction("abs", NHibernateUtil.Int64, property);
		}


		internal static IProjection ProcessRound(MethodCallExpression methodCallExpression)
		{
			IProjection innerProjection =
				ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();

			IProjection digitsProjection = Projections.Constant(0);
			if (methodCallExpression.Arguments.Count > 1)
				digitsProjection = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[1]).AsProjection();

			return Projections.SqlFunction("round", NHibernateUtil.Double, innerProjection, digitsProjection);
		}


		/// <summary>
		/// Project SQL function abs()
		/// Note: throws an exception outside of a QueryOver expression
		/// </summary>
		public static double Abs(this double numericProperty)
		{
			throw QueryOver.GetDirectUsageException();
		}

		internal static IProjection ProcessDoubleAbs(MethodCallExpression methodCallExpression)
		{
			IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
			return Projections.SqlFunction("abs", NHibernateUtil.Double, property);
		}

		/// <summary>
		/// Project SQL function trim()
		/// Note: throws an exception outside of a QueryOver expression
		/// </summary>
		public static string TrimStr(this string stringProperty)
		{
			throw QueryOver.GetDirectUsageException();
		}

		internal static IProjection ProcessTrimStr(MethodCallExpression methodCallExpression)
		{
			IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
			return Projections.SqlFunction("trim", NHibernateUtil.String, property);
		}

		/// <summary>
		/// Project SQL function length()
		/// Note: throws an exception outside of a QueryOver expression
		/// </summary>
		public static int StrLength(this string stringProperty)
		{
			throw QueryOver.GetDirectUsageException();
		}

		internal static IProjection ProcessStrLength(MethodCallExpression methodCallExpression)
		{
			IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
			return Projections.SqlFunction("length", NHibernateUtil.String, property);
		}

		/// <summary>
		/// Project SQL function bit_length()
		/// Note: throws an exception outside of a QueryOver expression
		/// </summary>
		public static int BitLength(this string stringProperty)
		{
			throw QueryOver.GetDirectUsageException();
		}

		internal static IProjection ProcessBitLength(MethodCallExpression methodCallExpression)
		{
			IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
			return Projections.SqlFunction("bit_length", NHibernateUtil.String, property);
		}

		/// <summary>
		/// Project SQL function substring()
		/// Note: throws an exception outside of a QueryOver expression
		/// </summary>
		public static string Substr(this string stringProperty, int startIndex, int length)
		{
			throw QueryOver.GetDirectUsageException();
		}

		internal static IProjection ProcessSubstr(MethodCallExpression methodCallExpression)
		{
			IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
			var startIndex = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[1]);
			var length = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[2]);
			return Projections.SqlFunction("substring", NHibernateUtil.String, property, startIndex.AsProjection(), length.AsProjection());
		}

		/// <summary>
		/// Project SQL function locate()
		/// Note: throws an exception outside of a QueryOver expression
		/// </summary>
		public static int CharIndex(this string stringProperty, string theChar, int startLocation)
		{
			throw QueryOver.GetDirectUsageException();
		}

		internal static IProjection ProcessCharIndex(MethodCallExpression methodCallExpression)
		{
			IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
			var theChar = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[1]);
			var startLocation = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[2]);
			return Projections.SqlFunction("locate", NHibernateUtil.String, theChar.AsProjection(), property, startLocation.AsProjection());
		}

		/// <summary>
		/// Project SQL function coalesce()
		/// Note: throws an exception outside of a QueryOver expression
		/// </summary>
		public static T Coalesce<T>(this T objectProperty, T replaceValueIfIsNull)
		{
			throw QueryOver.GetDirectUsageException();
		}

		/// <summary>
		/// Project SQL function coalesce()
		/// Note: throws an exception outside of a QueryOver expression
		/// </summary>
		public static T? Coalesce<T>(this T? objectProperty, T replaceValueIfIsNull) where T : struct
		{
			throw QueryOver.GetDirectUsageException();
		}

		internal static IProjection ProcessCoalesce(MethodCallExpression methodCallExpression)
		{
			IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
			var replaceValueIfIsNull = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[1]);
			return new SqlFunctionProjection("coalesce", returnTypeProjection: property, property, replaceValueIfIsNull.AsProjection());
		}

		/// <summary>
		/// Project SQL function mod()
		/// Note: throws an exception outside of a QueryOver expression
		/// </summary>
		public static int Mod(this int numericProperty, int divisor)
		{
			throw QueryOver.GetDirectUsageException();
		}

		internal static IProjection ProcessMod(MethodCallExpression methodCallExpression)
		{
			IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
			var divisor = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[1]);
			return Projections.SqlFunction("mod", NHibernateUtil.Int32, property, divisor.AsProjection());
		}

		/// <summary>
		/// Project Entity
		/// </summary>
		public static T AsEntity<T>(this T alias) where T:class
		{
			throw QueryOver.GetDirectUsageException();
		}

		internal static IProjection ProcessAsEntity(MethodCallExpression methodCallExpression)
		{
			var expression = methodCallExpression.Arguments[0];
			var aliasName = ExpressionProcessor.FindMemberExpression(expression);
			return
				string.IsNullOrEmpty(aliasName)
					? Projections.RootEntity()
					: Projections.Entity(expression.Type, aliasName);
		}
	}
}
