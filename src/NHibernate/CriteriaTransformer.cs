using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Criterion;
using NHibernate.Impl;

namespace NHibernate
{
	/// <summary>
	/// Transforms Criteria queries
	/// </summary>
	public static class CriteriaTransformer
	{
		///<summary>
		/// Returns a clone of the original criteria, which will return the count 
		/// of rows that are returned by the original criteria query.
		///</summary>
		public static DetachedCriteria TransformToRowCount(DetachedCriteria criteria)
		{
			ICriteria clonedCriteria = TransformToRowCount(criteria.GetCriteriaImpl());
			DetachedCriteria detachedCriteria = DetachedCriteria.For(clonedCriteria.CriteriaClass, clonedCriteria.Alias);
			detachedCriteria.SetCriteriaImpl((CriteriaImpl) clonedCriteria);
			return detachedCriteria;
		}

		///<summary>
		/// Returns a clone of the original criteria, which will return the count 
		/// of rows that are returned by the original criteria query.
		///</summary>
		public static ICriteria TransformToRowCount(ICriteria criteria)
		{
			ICriteria cloned = Clone(criteria);
			cloned.Orders.Clear();
			cloned.SetFirstResult(0)
				.SetMaxResults(RowSelection.NoValue)
				.SetProjection(Projections.RowCount());
			return cloned;
		}

		/// <summary>
		/// Creates an exact clone of the criteria
		/// </summary>
		/// <returns></returns>
		public static DetachedCriteria Clone(DetachedCriteria criteria)
		{
			ICriteria clonedCriteria = Clone(criteria.GetCriteriaImpl());
			DetachedCriteria detachedCriteria = DetachedCriteria.For(clonedCriteria.CriteriaClass, clonedCriteria.Alias);
			detachedCriteria.SetCriteriaImpl((CriteriaImpl) clonedCriteria);
			return detachedCriteria;
		}

		/// <summary>
		/// Creates an exact clone of the criteria
		/// </summary>
		/// <returns></returns>
		public static ICriteria Clone(ICriteria criteria)
		{
			CriteriaImpl root = GetRootCriteria(criteria);
			CriteriaImpl clone = new CriteriaImpl(root.CriteriaClass, root.Alias, root.Session);
			CloneSubcriteria(clone, root);

			foreach (KeyValuePair<string, FetchMode> de in root.FetchModes)
			{
				clone.SetFetchMode(de.Key, de.Value);
			}
			foreach (KeyValuePair<string, LockMode> de in root.LockModes)
			{
				clone.SetLockMode(de.Key, de.Value);
			}
			clone.SetMaxResults(root.MaxResults);
			clone.SetFirstResult(root.FirstResult);
			clone.SetTimeout(root.Timeout);
			clone.SetFetchSize(root.FetchSize);
			clone.SetCacheable(root.Cacheable);
			if (string.IsNullOrEmpty(root.CacheRegion) == false)
				clone.SetCacheRegion(root.CacheRegion);
			clone.SetProjection(root.Projection);
			CloneProjectCrtieria(clone, root);
			clone.SetResultTransformer(root.ResultTransformer);

			return clone;
		}

		private static CriteriaImpl GetRootCriteria(ICriteria criteria)
		{
			if (criteria is CriteriaImpl)
				return (CriteriaImpl) criteria;
			return GetRootCriteria(((CriteriaImpl.Subcriteria) criteria).Parent);
		}

		private static void CloneProjectCrtieria(CriteriaImpl clone, ICriteria original)
		{
			if (original.ProjectionCriteria != null)
			{
				if (original.ProjectionCriteria == original)
				{
					clone.SetProjectionCriteria(clone);
				}
				else
				{
					ICriteria clonedProjectionCriteria = Clone(original.ProjectionCriteria);
					clone.SetProjectionCriteria(clonedProjectionCriteria);
				}
			}
		}

		/// <summary>
		/// When cloning subcriterias we need to preserve the relations of the dependent objects (orders, restrictions).
		/// </summary>
		/// <param name="clone"></param>
		/// <param name="original"></param>
		private static void CloneSubcriteria(CriteriaImpl clone, CriteriaImpl original)
		{
			//we need to preserve the parent criteria, we rely on the ordering when creating the 
			//subcriterias initially here, so we don't need to make more than a single pass
			Hashtable newParents = new Hashtable();
			newParents[original] = clone;
			foreach (CriteriaImpl.Subcriteria subcriteria in original.SubcriteriaList)
			{
				ICriteria currentParent = (ICriteria) newParents[subcriteria.Parent];
				if (currentParent == null)
					throw new InvalidOperationException("Could not find parent for subcriteria in the previous subcriteria. If you see this error, it is a bug");
				CriteriaImpl.Subcriteria clonedSubCriteria = new CriteriaImpl.Subcriteria(clone, currentParent, subcriteria.Path, subcriteria.Alias, subcriteria.JoinType);
				clonedSubCriteria.SetLockMode(subcriteria.LockMode);
				newParents[subcriteria] = clonedSubCriteria;
			}

			// remap the orders
			foreach (CriteriaImpl.OrderEntry orderEntry in original.Orders)
			{
				ICriteria currentParent = (ICriteria) newParents[orderEntry.Criteria];
				if (currentParent == null)
					throw new InvalidOperationException("Could not find parent for order in the previous criteria. If you see this error, it is a bug");
				currentParent.AddOrder(orderEntry.Order);
			}


			// remap the restrictions to appropriate criterias
			foreach (CriteriaImpl.CriterionEntry criterionEntry in original.Restrictions) {
				ICriteria currentParent = (ICriteria)newParents[criterionEntry.Criteria];
				if (currentParent == null)
					throw new InvalidOperationException("Could not find parent for restriction in the previous criteria. If you see this error, it is a bug.");
				currentParent.Add(criterionEntry.Criterion);
			}
		}
	}
}
