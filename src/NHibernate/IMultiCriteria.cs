using System;
using System.Collections;
using NHibernate.Criterion;
using NHibernate.Impl;
using NHibernate.Transform;

namespace NHibernate
{
	/// <summary>
	/// Combines several queries into a single DB call
	/// </summary>
	// Since v5.2
	[Obsolete("Use Multi.IQueryBatch instead, obtainable with ISession.CreateQueryBatch.")]
	public partial interface IMultiCriteria
	{
		/// <summary>
		/// Get all the results
		/// </summary>
		IList List();

		/// <summary>
		/// Adds the specified criteria to the query. The result will be contained in a <see cref="System.Collections.Generic.List{T}"/>
		/// </summary>
		/// <param name="resultGenericListType">Return results in a <see cref="System.Collections.Generic.List{resultGenericListType}"/></param>
		/// <param name="criteria">The criteria.</param>
		/// <returns></returns>
		IMultiCriteria Add(System.Type resultGenericListType, ICriteria criteria);

		/// <summary>
		/// Adds the specified criteria to the query. The result will be contained in a <see cref="System.Collections.Generic.List{T}"/>
		/// </summary>
		/// <param name="criteria">The criteria.</param>
		/// <returns></returns>
		IMultiCriteria Add<T>(ICriteria criteria);

		/// <summary>
		/// Adds the specified criteria to the query, and associates it with the given key. The result will be contained in a <see cref="System.Collections.Generic.List{T}"/>
		/// </summary>
		/// <param name="key">The key</param>
		/// <param name="criteria">The criteria</param>
		/// <returns></returns>
		IMultiCriteria Add<T>(string key, ICriteria criteria);

		/// <summary>
		/// Adds the specified detached criteria. The result will be contained in a <see cref="System.Collections.Generic.List{T}"/>
		/// </summary>
		/// <param name="detachedCriteria">The detached criteria.</param>
		/// <returns></returns>
		IMultiCriteria Add<T>(DetachedCriteria detachedCriteria);

		/// <summary>
		/// Adds the specified detached criteria, and associates it with the given key. The result will be contained in a <see cref="System.Collections.Generic.List{T}"/>
		/// </summary>
		/// <param name="key">The key</param>
		/// <param name="detachedCriteria">The detached criteria</param>
		/// <returns></returns>
		IMultiCriteria Add<T>(string key, DetachedCriteria detachedCriteria);

		/// <summary>
		/// Adds the specified criteria to the query
		/// </summary>
		/// <param name="criteria">The criteria.</param>
		/// <returns></returns>
		IMultiCriteria Add(ICriteria criteria);

		/// <summary>
		/// Adds the specified criteria to the query, and associates it with the given key
		/// </summary>
		/// <param name="key">The key</param>
		/// <param name="criteria">The criteria</param>
		/// <returns></returns>
		IMultiCriteria Add(string key, ICriteria criteria);

		/// <summary>
		/// Adds the specified detached criteria.
		/// </summary>
		/// <param name="detachedCriteria">The detached criteria.</param>
		/// <returns></returns>
		IMultiCriteria Add(DetachedCriteria detachedCriteria);

		/// <summary>
		/// Adds the specified detached criteria, and associates it with the given key
		/// </summary>
		/// <param name="key">The key</param>
		/// <param name="detachedCriteria">The detached criteria</param>
		/// <returns></returns>
		IMultiCriteria Add(string key, DetachedCriteria detachedCriteria);

		/// <summary>
		/// Adds the specified IQueryOver to the query. The result will be contained in a <see cref="System.Collections.Generic.List{resultGenericListType}"/>
		/// </summary>
		/// <param name="resultGenericListType">Return results in a <see cref="System.Collections.Generic.List{resultGenericListType}"/></param>
		/// <param name="queryOver">The IQueryOver.</param>
		/// <returns></returns>
		IMultiCriteria Add(System.Type resultGenericListType, IQueryOver queryOver);

		/// <summary>
		/// Adds the specified IQueryOver to the query. The result will be contained in a <see cref="System.Collections.Generic.List{T}"/>
		/// </summary>
		/// <param name="queryOver">The IQueryOver.</param>
		/// <returns></returns>
		IMultiCriteria Add<T>(IQueryOver<T> queryOver);

		/// <summary>
		/// Adds the specified IQueryOver to the query. The result will be contained in a <see cref="System.Collections.Generic.List{U}"/>
		/// </summary>
		/// <param name="queryOver">The IQueryOver.</param>
		/// <returns></returns>
		IMultiCriteria Add<U>(IQueryOver queryOver);

		/// <summary>
		/// Adds the specified IQueryOver to the query, and associates it with the given key. The result will be contained in a <see cref="System.Collections.Generic.List{T}"/>
		/// </summary>
		/// <param name="key">The key</param>
		/// <param name="queryOver">The IQueryOver</param>
		/// <returns></returns>
		IMultiCriteria Add<T>(string key, IQueryOver<T> queryOver);

		/// <summary>
		/// Adds the specified IQueryOver to the query, and associates it with the given key. The result will be contained in a <see cref="System.Collections.Generic.List{U}"/>
		/// </summary>
		/// <param name="key">The key</param>
		/// <param name="queryOver">The IQueryOver</param>
		/// <returns></returns>
		IMultiCriteria Add<U>(string key, IQueryOver queryOver);

		/// <summary>
		/// Sets whatever this criteria is cacheable.
		/// </summary>
		/// <param name="cachable">if set to <c>true</c> [cachable].</param>
		IMultiCriteria SetCacheable(bool cachable);

		///<summary>
		/// Set the cache region for the criteria
		///</summary>
		///<param name="region">The region</param>
		///<returns></returns>
		IMultiCriteria SetCacheRegion(string region);

		///<summary>
		/// Force a cache refresh
		///</summary>
		///<param name="forceRefresh"></param>
		///<returns></returns>
		IMultiCriteria ForceCacheRefresh(bool forceRefresh);

		/// <summary>
		/// Sets the result transformer for all the results in this mutli criteria instance
		/// </summary>
		/// <param name="resultTransformer">The result transformer.</param>
		/// <returns></returns>
		IMultiCriteria SetResultTransformer(IResultTransformer resultTransformer);

		/// <summary>
		/// Returns the result of one of the Criteria based on the key
		/// </summary>
		/// <param name="key">The key</param>
		/// <returns></returns>
		object GetResult(string key);
	}

	// Since v5.2
	[Obsolete("Use Multi.IQueryBatch instead, obtainable with ISession.CreateQueryBatch.")]
	public static class MultiCriteriaExtensions
	{
		/// <summary>
		/// Set a timeout for the underlying ADO.NET query.
		/// </summary>
		/// <param name="timeout">The timeout in seconds.</param>
		/// <param name="multiCriteria">The <see cref="IMultiCriteria" /> on which to set the timeout.</param>
		/// <returns><paramref name="multiCriteria" /> (for method chaining).</returns>
		public static IMultiCriteria SetTimeout(this IMultiCriteria multiCriteria, int timeout)
		{
			if (multiCriteria == null)
			{
				throw new ArgumentNullException(nameof(multiCriteria));
			}

			if (multiCriteria is MultiCriteriaImpl impl)
			{
				return impl.SetTimeout(timeout);
			}

			throw new NotSupportedException(multiCriteria.GetType() + " does not support SetTimeout");
		}
	}
}
