using System;
using System.Collections;
using NHibernate.Cfg;

namespace NHibernate.Cache
{
	/// <summary>
	/// Defines a factory for query cache instances.  These factories are responsible for
	/// creating individual QueryCache instances.
	/// </summary>
	public interface IQueryCacheFactory
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="regionName"></param>
		/// <param name="updateTimestampsCache"></param>
		/// <param name="settings"></param>
		/// <param name="props"></param>
		/// <returns></returns>
		IQueryCache GetQueryCache( string regionName, UpdateTimestampsCache updateTimestampsCache, Settings settings, IDictionary props );
	}
}
