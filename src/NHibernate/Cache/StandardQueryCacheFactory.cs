using System;
using System.Collections;
using NHibernate.Cfg;

namespace NHibernate.Cache
{
	/// <summary>
	/// Standard Hibernate implementation of the IQueryCacheFactory interface.  Returns
	/// instances of <see cref="StandardQueryCache" />.
	/// </summary>
	public class StandardQueryCacheFactory : IQueryCacheFactory
	{
		public IQueryCache GetQueryCache( string regionName,
		                                  UpdateTimestampsCache updateTimestampsCache,
		                                  Settings settings,
		                                  IDictionary props )
		{
			return new StandardQueryCache( settings, props, updateTimestampsCache, regionName );
		}
	}
}
