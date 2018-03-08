using System;
using System.Linq.Expressions;
using NHibernate.Criterion;
using NHibernate.Impl;
using NHibernate.Loader;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate
{
	public static class SelectModeExtensions
	{
		/// <summary>
		/// Applies SelectMode for given current criteria association paths:
		/// curCriteriaEntityType => curCriteriaEntityType or
		/// curCriteriaEntityType => curCriteriaEntityType.ChildEntity.SubEntity
		/// </summary>
		public static IQueryOver<TRoot, TSubType> With<TRoot, TSubType>(this IQueryOver<TRoot,TSubType> queryOver, SelectMode mode, params Expression<Func<TSubType, object>>[] associationPaths)
		{
			var q = CastOrThrow<ISupportSelectModeQueryOver<TRoot, TSubType>>(queryOver);
			foreach (var associationPath in associationPaths)
			{
				q.SetSelectMode(mode, associationPath);
			}

			return queryOver;
		}

		/// <summary>
		/// Applies SelectMode for given aliased criteria association paths:
		/// () => aliasedCriteria or () => aliasedCriteria.ChildEntity.SubEntity
		/// </summary>
		public static TThis With<TThis>(this TThis queryOver, SelectMode mode, params Expression<Func<object>>[] aliasedAssociationPaths) where TThis: IQueryOver
		{
			var criteria = queryOver.UnderlyingCriteria;
			foreach (var aliasedPath in aliasedAssociationPaths)
			{
				var expressionPath = ExpressionProcessor.FindMemberExpression(aliasedPath.Body);

				StringHelper.IsNotRoot(expressionPath, out var alias, out var path);

				criteria.With(mode, path, alias);
			}

			return queryOver;
		}

		/// <summary>
		/// Applies select mode for given aliased or current criteria
		/// </summary>
		/// <param name="criteria">Current criteria</param>
		/// <param name="mode">Select mode</param>
		/// <param name="associationPath">Association path for given <paramref name="alias"/> criteria</param>
		/// <param name="alias">Criteria alias; current criteria if null/empty</param>
		/// <returns></returns>
		public static ICriteria With(this ICriteria criteria, SelectMode mode, string associationPath, string alias)
		{
			var q = CastOrThrow<ISupportSelectModeCriteria>(criteria);
			q.SetSelectMode(mode, associationPath, alias);

			return criteria;
		}

		private static T CastOrThrow<T>(object obj) where T : class
		{
			return TypeHelper.CastOrThrow<T>(obj, "SelectMode");
		}
	}
}
