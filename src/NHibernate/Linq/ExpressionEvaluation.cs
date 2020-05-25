using System;
using System.Linq.Expressions;

namespace NHibernate.Linq
{
	/// <summary>
	/// Contains methods that can be used to force database or client evaluation of an expression in a select statement
	/// when using NHibernate Linq query provider.
	/// </summary>
	public static class ExpressionEvaluation
	{
		/// <summary>
		/// Forces client evaluation of an expression in a select statement when using NHibernate Linq query provider.
		/// </summary>
		/// <typeparam name="T">The return type of <paramref name="expression"/>.</typeparam>
		/// <param name="expression">The expression to force client evaluation.</param>
		/// <exception cref="InvalidOperationException">When the method is used outside NHibernate Linq query.</exception>
		[NoPreEvaluation]
		public static T ClientEval<T>(Expression<Func<T>> expression)
		{
			throw new InvalidOperationException("The method should be used inside NHibernate Linq query");
		}

		/// <summary>
		/// Forces database evaluation of an expression in a select statement when using NHibernate Linq query provider.
		/// </summary>
		/// <typeparam name="T">The return type of <paramref name="expression"/>.</typeparam>
		/// <param name="expression">The expression to force database evaluation.</param>
		/// <exception cref="InvalidOperationException">When the method is used outside NHibernate Linq query.</exception>
		[NoPreEvaluation]
		public static T DatabaseEval<T>(Expression<Func<T>> expression)
		{
			throw new InvalidOperationException("The method should be used inside NHibernate Linq query");
		}
	}
}
