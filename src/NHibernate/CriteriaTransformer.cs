using NHibernate.Criterion;
using NHibernate.Engine;
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
			CriteriaImpl clonedCriteria = TransformToRowCount((CriteriaImpl)criteria.GetCriteriaImpl().Clone());
			return new DetachedCriteria(clonedCriteria);
		}

		///<summary>
		/// Returns a clone of the original criteria, which will return the count 
		/// of rows that are returned by the original criteria query.
		///</summary>
		public static ICriteria TransformToRowCount(ICriteria criteria)
		{
			return TransformToRowCount((CriteriaImpl)criteria.Clone());
		}

		private static CriteriaImpl TransformToRowCount(CriteriaImpl criteria)
		{
			criteria.ClearOrders();
			criteria.SetFirstResult(0).SetMaxResults(RowSelection.NoValue).SetProjection(Projections.RowCount());
			return criteria;
		}

		/// <summary>
		/// Creates an exact clone of the criteria
		/// </summary>
		/// <returns></returns>
		public static DetachedCriteria Clone(DetachedCriteria criteria)
		{
			CriteriaImpl clonedCriteria = (CriteriaImpl) criteria.GetCriteriaImpl().Clone();
			return new DetachedCriteria(clonedCriteria);
		}

		/// <summary>
		/// Creates an exact clone of the criteria
		/// </summary>
		/// <returns></returns>
		public static ICriteria Clone(ICriteria criteria)
		{
			return (CriteriaImpl) criteria.Clone();
		}
	}
}
