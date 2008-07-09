using System.Collections;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace NHibernate
{
	/// <summary>
	/// Combines several queries into a single DB call
	/// </summary>
	public interface IMultiCriteria
	{
		/// <summary>
		/// Get all the 
		/// </summary>
		IList List();

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
		/// Sets whatevert this criteria is cacheable.
		/// </summary>
		/// <param name="cachable">if set to <c>true</c> [cachable].</param>
		IMultiCriteria SetCacheable(bool cachable);

		///<summary>
		/// Set the cache region for thie criteria
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
}
