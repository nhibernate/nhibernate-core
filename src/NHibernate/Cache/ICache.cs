using System;

namespace NHibernate.Cache {
	
	/// <summary>
	/// Implementors define a caching algorithm.
	/// </summary>
	/// <remarks>
	/// All implementations MUST be threadsafe
	/// </remarks>
	public interface ICache {

		/// <summary>
		/// Gets or sets cached data
		/// </summary>
		/// <value>The cached object or <c>null</c></value>
		/// <exception cref="CacheException"></exception>
		object this [object key] {
			get;
			set;
		}
	
	}


}
