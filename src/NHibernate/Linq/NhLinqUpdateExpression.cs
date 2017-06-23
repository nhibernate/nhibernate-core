using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Linq
{
	public class NhLinqUpdateExpression<T> : NhLinqExpression
	{
		protected override QueryMode QueryMode => _versioned ? QueryMode.UpdateVersioned : QueryMode.Update;

		/// <summary>
		/// Entity type to insert or update when the expression is a DML.
		/// </summary>
		protected override System.Type TargetType => typeof(T);

		private readonly bool _versioned;

		public NhLinqUpdateExpression(Expression expression, ISessionFactoryImplementor sessionFactory, bool versioned, IReadOnlyCollection<Assignment> assignments)
			: base(RewriteForUpdate(expression, assignments), sessionFactory)
		{
			_versioned = versioned;
			Key = (versioned ? "UPDATE VERSIONED " : "UPDATE ") + Key;
		}

		private static Expression RewriteForUpdate(Expression expression, IReadOnlyCollection<Assignment> assignments)
		{
			var lambda = Assignment.ConvertAssignmentsToDictionaryExpression<T>(assignments);

			return Expression.Call(
				ReflectionCache.QueryableMethods.SelectDefinition.MakeGenericMethod(typeof(T), lambda.Body.Type),
				expression, Expression.Quote(lambda));
		}
	}
}
