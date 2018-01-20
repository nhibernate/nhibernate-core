using System;
using System.Linq.Expressions;
using NHibernate.Criterion;
using NHibernate.Impl;
using NHibernate.SqlCommand;

namespace NHibernate
{
	public static class EntityJoinExtensions
	{
		// 6.0 TODO: merge into 'IQueryOver<TType, TSubType>
		public static TThis JoinEntityAlias<TThis, TAlias>(this TThis queryOver, Expression<Func<TAlias>> alias, JoinType joinType, ICriterion withClause, string entityName = null) where TThis : IQueryOver
		{
			var q = CastOrThrow<ISupportEntityJoinQueryOver>(queryOver);
			q.JoinEntityAlias(alias, joinType, withClause, entityName);
			return queryOver;
		}

		public static IQueryOver<TRoot, U> JoinEntityQueryOver<TRoot, U>(this IQueryOver<TRoot> queryOver, Expression<Func<U>> alias, JoinType joinType, ICriterion withClause, string entityName = null)
		{
			var q = CastOrThrow<ISupportEntityJoinQueryOver<TRoot>>(queryOver);
			return q.JoinEntityQueryOver(alias, joinType, withClause, entityName);
		}

		// 6.0 TODO: merge into 'ICriteria'
		public static ICriteria CreateEntityAlias(this ICriteria criteria, string alias, JoinType joinType, ICriterion withClause, string entityName)
		{
			CreateEntityCriteria(criteria, alias, joinType, withClause, entityName);
			return criteria;
		}

		public static ICriteria CreateEntityCriteria(this ICriteria criteria, string alias, JoinType joinType, ICriterion withClause, string entityName)
		{
			var c = CastOrThrow<ISupportEntityJoinCriteria>(criteria);
			return c.CreateEntityCriteria(alias, joinType, withClause, entityName);
		}

		private static T CastOrThrow<T>(object obj) where T: class
		{
			return obj as T
					?? throw new ArgumentException($"{obj.GetType().FullName} requires to implement {nameof(T)} interface to support explicit entity joins.");
		}
	}
}
