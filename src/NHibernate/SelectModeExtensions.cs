using System;
using System.Linq.Expressions;
using NHibernate.Criterion;
using NHibernate.Impl;
using NHibernate.Util;

namespace NHibernate
{
	public static class SelectModeExtensions
	{
		/// <summary>
		/// Applies a select mode for the given current criteria association paths:
		/// <c>curCriteriaEntityType => curCriteriaEntityType</c> or
		/// <c>curCriteriaEntityType => curCriteriaEntityType.ChildEntity.SubEntity</c>.
		/// </summary>
		public static IQueryOver<TRoot, TSubType> Fetch<TRoot, TSubType>(
			this IQueryOver<TRoot, TSubType> queryOver, SelectMode mode, params Expression<Func<TSubType, object>>[] associationPaths)
		{
			var q = CastOrThrow<ISupportSelectModeQueryOver<TRoot, TSubType>>(queryOver);
			foreach (var associationPath in associationPaths)
			{
				q.Fetch(mode, associationPath);
			}

			return queryOver;
		}

		/// <summary>
		/// Applies a select mode for the given current criteria association paths:
		/// <c>curCriteriaEntityType => curCriteriaEntityType</c> or
		/// <c>curCriteriaEntityType => curCriteriaEntityType.ChildEntity.SubEntity</c>.
		/// </summary>
		public static QueryOver<TRoot, TSubType> Fetch<TRoot, TSubType>(
			this QueryOver<TRoot, TSubType> queryOver, SelectMode mode,
			params Expression<Func<TSubType, object>>[] associationPaths)
		{
			Fetch((IQueryOver<TRoot, TSubType>) queryOver, mode, associationPaths);
			return queryOver;
		}

		/// <summary>
		/// Fetches the given current criteria association paths:
		/// <c>curCriteriaEntityType => curCriteriaEntityType</c> or
		/// <c>curCriteriaEntityType => curCriteriaEntityType.ChildEntity.SubEntity</c>.
		/// </summary>
		public static QueryOver<TRoot, TSubType> Fetch<TRoot, TSubType>(
			this QueryOver<TRoot, TSubType> queryOver, params Expression<Func<TSubType, object>>[] associationPaths)
		{
			return Fetch(queryOver, SelectMode.Fetch, associationPaths);
		}

		/// <summary>
		/// Fetches the given current criteria association paths:
		/// <c>curCriteriaEntityType => curCriteriaEntityType</c> or
		/// <c>curCriteriaEntityType => curCriteriaEntityType.ChildEntity.SubEntity</c>.
		/// </summary>
		public static IQueryOver<TRoot, TSubType> Fetch<TRoot, TSubType>(
			this IQueryOver<TRoot, TSubType> queryOver, params Expression<Func<TSubType, object>>[] associationPaths)
		{
			return Fetch(queryOver, SelectMode.Fetch, associationPaths);
		}

		/// <summary>
		/// Applies a select mode for the given aliased criteria association paths:
		/// <c>() => aliasedCriteria</c> or <c>() => aliasedCriteria.ChildEntity.SubEntity</c>.
		/// </summary>
		public static TThis Fetch<TThis>(
			this TThis queryOver, SelectMode mode, params Expression<Func<object>>[] aliasedAssociationPaths)
			where TThis : IQueryOver
		{
			var criteria = queryOver.UnderlyingCriteria;
			foreach (var aliasedPath in aliasedAssociationPaths)
			{
				var expressionPath = ExpressionProcessor.FindMemberExpression(aliasedPath.Body);

				StringHelper.IsNotRoot(expressionPath, out var alias, out var path);

				criteria.Fetch(mode, path, alias);
			}

			return queryOver;
		}

		/// <summary>
		/// Applies a select mode for the given aliased criteria or the current criteria
		/// </summary>
		/// <param name="criteria">The current criteria.</param>
		/// <param name="mode">The select mode to apply.</param>
		/// <param name="associationPath">The association path for the given <paramref name="alias"/> criteria.</param>
		/// <param name="alias">The criteria alias. If null or empty, the current criteria will be used.</param>
		/// <returns>The current criteria.</returns>
		public static ICriteria Fetch(this ICriteria criteria, SelectMode mode, string associationPath, string alias = null)
		{
			var q = CastOrThrow<ISupportSelectModeCriteria>(criteria);
			q.Fetch(mode, associationPath, alias);

			return criteria;
		}

		/// <summary>
		/// Fetches the given current criteria association paths:
		/// <c>curCriteriaEntityType => curCriteriaEntityType</c> or
		/// <c>curCriteriaEntityType => curCriteriaEntityType.ChildEntity.SubEntity</c>.
		/// </summary>
		public static ICriteria Fetch(this ICriteria criteria, string associationPath, string alias = null)
		{
			return Fetch(criteria, SelectMode.Fetch, associationPath, alias);
		}

		/// <summary>
		/// Fetches the given aliased criteria or the current criteria association path
		/// </summary>
		/// <param name="criteria">The current criteria.</param>
		/// <param name="associationPath">The association path for the given <paramref name="alias"/> criteria.</param>
		/// <param name="alias">The criteria alias. If null or empty, the current criteria will be used.</param>
		/// <returns>The current criteria.</returns>
		public static DetachedCriteria Fetch(this DetachedCriteria criteria, string associationPath, string alias = null)
		{
			return Fetch(criteria, SelectMode.Fetch, associationPath, alias);
		}

		/// <summary>
		/// Applies a select mode for the given aliased criteria or the current criteria
		/// </summary>
		/// <param name="criteria">The current criteria.</param>
		/// <param name="mode">The select mode to apply.</param>
		/// <param name="associationPath">The association path for the given <paramref name="alias"/> criteria.</param>
		/// <param name="alias">The criteria alias. If null or empty, the current criteria will be used.</param>
		/// <returns>The current criteria.</returns>
		public static DetachedCriteria Fetch(this DetachedCriteria criteria, SelectMode mode, string associationPath, string alias = null)
		{
			Fetch(criteria.GetCriteriaImpl(), mode, associationPath, alias);
			return criteria;
		}

		private static T CastOrThrow<T>(object obj) where T : class
		{
			return ReflectHelper.CastOrThrow<T>(obj, "SelectMode");
		}
	}
}
