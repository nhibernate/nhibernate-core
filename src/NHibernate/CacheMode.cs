using System;

namespace NHibernate
{
	/// <summary>
	/// Controls how the session interacts with the second-level
	/// cache and query cache.
	/// </summary>
	[Serializable, Flags]
	public enum CacheMode
	{
		/// <summary> 
		/// The session will never interact with the cache, except to invalidate
		/// cache items when updates occur
		/// </summary>
		Ignore = 0,

		/// <summary> 
		/// The session will never read items from the cache, but will add items
		/// to the cache as it reads them from the database.
		/// </summary>
		Put = 1,

		/// <summary> 
		/// The session may read items from the cache, but will not add items, 
		/// except to invalidate items when updates occur
		/// </summary>
		Get = 2,

		/// <summary> The session may read items from the cache, and add items to the cache</summary>
		Normal = Put | Get,

		/// <summary> 
		/// The session will never read items from the cache, but will add items
		/// to the cache as it reads them from the database. In this mode, the
		/// effect of <tt>hibernate.cache.use_minimal_puts</tt> is bypassed, in
		/// order to <em>force</em> a cache refresh
		/// </summary>
		Refresh = Put | 4 // NH: include Put but have a different value
	}
}