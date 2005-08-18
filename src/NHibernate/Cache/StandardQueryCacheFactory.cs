using System;
using System.Collections;

namespace NHibernate.Cache
{
	/// <summary>
	/// Standard Hibernate implementation of the IQueryCacheFactory interface.  Returns
	/// instances of <see cref="StandardQueryCache" />.
	/// </summary>
	public class StandardQueryCacheFactory : IQueryCacheFactory
	{
		public IQueryCache GetQueryCache( string regionName, ICacheProvider provider,
		                                  UpdateTimestampsCache updateTimestampsCache,
		                                  IDictionary props )
		{
			return new StandardQueryCache( provider, props, updateTimestampsCache, regionName );
		}
	}
}