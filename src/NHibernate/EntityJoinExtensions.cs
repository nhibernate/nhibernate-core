using System;
using System.Linq.Expressions;
using NHibernate.Criterion;
using NHibernate.Impl;
using NHibernate.SqlCommand;

namespace NHibernate
{
	public static class EntityJoinExtensions
	{
		public static TThis JoinEntityAlias<TThis, TEntity>(this TThis queryOver, Expression<Func<TEntity>> alias, ICriterion withClause, JoinType joinType = JoinType.InnerJoin, string entityName = null) where TThis : IQueryOver
		{
			var q = CastOrThrow<ISupportEntityJoinQueryOver>(queryOver);
			q.JoinEntityAlias(alias, withClause, joinType, entityName);
			return queryOver;
		}

		public static TThis JoinEntityAlias<TThis, TEntity>(this TThis queryOver, Expression<Func<TEntity>> alias, Expression<Func<bool>> withClause, JoinType joinType = JoinType.InnerJoin, string entityName = null) where TThis : IQueryOver
		{
			return JoinEntityAlias(queryOver, alias, Restrictions.Where(withClause), joinType, entityName);
		}

		public static IQueryOver<TRoot, TEntity> JoinEntityQueryOver<TRoot, TEntity>(this IQueryOver<TRoot> queryOver, Expression<Func<TEntity>> alias, Expression<Func<bool>> withClause, JoinType joinType = JoinType.InnerJoin, string entityName = null)
		{
			return JoinEntityQueryOver(queryOver, alias, Restrictions.Where(withClause), joinType, entityName);
		}

		public static IQueryOver<TRoot, TEntity> JoinEntityQueryOver<TRoot, TEntity>(this IQueryOver<TRoot> queryOver, Expression<Func<TEntity>> alias, ICriterion withClause, JoinType joinType = JoinType.InnerJoin, string entityName = null)
		{
			var q = CastOrThrow<ISupportEntityJoinQueryOver<TRoot>>(queryOver);
			return q.JoinEntityQueryOver(alias, withClause, joinType, entityName);
		}

		public static ICriteria CreateEntityAlias(this ICriteria criteria, string alias, ICriterion withClause, JoinType joinType, string entityName)
		{
			CreateEntityCriteria(criteria, alias, withClause, joinType, entityName);
			return criteria;
		}

		public static ICriteria CreateEntityCriteria(this ICriteria criteria, string alias, ICriterion withClause, JoinType joinType, string entityName)
		{
			var c = CastOrThrow<ISupportEntityJoinCriteria>(criteria);
			return c.CreateEntityCriteria(alias, withClause, joinType, entityName);
		}

		private static T CastOrThrow<T>(object obj) where T : class
		{
			return obj as T
					?? throw new ArgumentException($"{obj.GetType().FullName} requires to implement {nameof(T)} interface to support explicit entity joins.");
		}
	}
}
