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

		public NhLinqUpdateExpression(Expression expression, Assignments<T, T> assignments, ISessionFactoryImplementor sessionFactory, bool versioned)
			: base(RewriteForUpdate(expression, assignments), sessionFactory)
		{
			_versioned = versioned;
			Key = $"UPDATE {(versioned ? "VERSIONNED " : "")}{Key}";
		}

		private static Expression RewriteForUpdate(Expression expression, Assignments<T, T> assignments)
		{
			var lambda = assignments.ConvertToDictionaryExpression();

			return Expression.Call(
				ReflectionCache.QueryableMethods.SelectDefinition.MakeGenericMethod(typeof(T), lambda.Body.Type),
				expression, Expression.Quote(lambda));
		}
	}
}