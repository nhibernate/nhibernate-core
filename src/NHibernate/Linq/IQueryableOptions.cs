using System;

namespace NHibernate.Linq
{
	// Methods signatures taken from IQuery.
	/// <summary>
	/// Expose NH queryable options.
	/// </summary>
	//Since v5.1
	[Obsolete("Please use NhQueryableOptions instead.")]
	public interface IQueryableOptions
	{
		/// <summary>
		/// Enable caching of this query result set.
		/// </summary>
		/// <param name="cacheable">Should the query results be cacheable?</param>
		/// <returns><see langword="this"/> (for method chaining).</returns>
		IQueryableOptions SetCacheable(bool cacheable);

		/// <summary>
		/// Set the name of the cache region.
		/// </summary>
		/// <param name="cacheRegion">The name of a query cache region, or <see langword="null" />
		/// for the default query cache</param>
		/// <returns><see langword="this"/> (for method chaining).</returns>
		IQueryableOptions SetCacheRegion(string cacheRegion);

		/// <summary>
		/// Override the current session cache mode, just for this query.
		/// </summary>
		/// <param name="cacheMode">The cache mode to use.</param>
		/// <returns><see langword="this"/> (for method chaining).</returns>
		IQueryableOptions SetCacheMode(CacheMode cacheMode);

		/// <summary>
		/// Set a timeout for the underlying ADO.NET query.
		/// </summary>
		/// <param name="timeout">The timeout in seconds.</param>
		/// <returns><see langword="this" /> (for method chaining).</returns>
		IQueryableOptions SetTimeout(int timeout);

		/// <summary>
		/// Set a T-SQL Query hint as an Option to the query
		/// </summary>
		/// <param name="hint">The t-sql query hint</param>
		/// <returns><see langword="this" /> (for method chaining).</returns>
		IQueryableOptions SetHint(string hint);
	}
}
