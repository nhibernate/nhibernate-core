using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Linq
{
	public class NhLinqInsertExpression<TSource, TTarget> : NhLinqExpression
	{
		protected override QueryMode QueryMode => QueryMode.Insert;

		/// <summary>
		/// Entity type to insert or update when the expression is a DML.
		/// </summary>
		protected override System.Type TargetType => typeof(TTarget);

		public NhLinqInsertExpression(Expression expression, ISessionFactoryImplementor sessionFactory, IReadOnlyCollection<Assignment> assignments)
			: base(RewriteForInsert(expression, assignments), sessionFactory)
		{
			Key = "INSERT " + Key;
		}

		private static Expression RewriteForInsert(Expression expression, IReadOnlyCollection<Assignment> assignments)
		{
			var lambda = Assignment.ConvertAssignmentsToDictionaryExpression<TSource>(assignments);

			return Expression.Call(
				ReflectionCache.QueryableMethods.SelectDefinition.MakeGenericMethod(typeof(TSource), lambda.Body.Type),
				expression, Expression.Quote(lambda));
		}
	}
}
