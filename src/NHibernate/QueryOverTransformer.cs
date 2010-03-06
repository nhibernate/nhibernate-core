using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.Impl;

namespace NHibernate
{
	/// <summary>
	/// Transforms QueryOver queries
	/// </summary>
	public static class QueryOverTransformer
	{
		///<summary>
		/// Returns a clone of the original QueryOver, which will return the count
		/// of rows that are returned by the original QueryOver query.
		///</summary>
		public static QueryOver<T,T> TransformToRowCount<T>(QueryOver<T> query)
		{
			QueryOver<T,T> clonedQuery = (QueryOver<T,T>)query.Clone();
			return (QueryOver<T,T>)TransformToRowCount((IQueryOver<T>)clonedQuery);
		}

		///<summary>
		/// Returns a clone of the original IQueryOver, which will return the count
		/// of rows that are returned by the original IQueryOver query.
		///</summary>
		public static IQueryOver<T,T> TransformToRowCount<T>(IQueryOver<T> query)
		{
			IQueryOver<T,T> clonedQuery = (IQueryOver<T,T>)query.Clone();

			return
				clonedQuery
					.ClearOrders()
					.Skip(0)
					.Take(RowSelection.NoValue)
					.Select(Projections.RowCount());
		}

		/// <summary>
		/// Creates an exact clone of the IQueryOver
		/// </summary>
		public static IQueryOver<T,T> Clone<T>(IQueryOver<T> query)
		{
			return (IQueryOver<T,T>)query.Clone();
		}

		/// <summary>
		/// Creates an exact clone of the QueryOver
		/// </summary>
		public static QueryOver<T,T> Clone<T>(QueryOver<T> query)
		{
			return (QueryOver<T,T>)query.Clone();
		}
	}
}
