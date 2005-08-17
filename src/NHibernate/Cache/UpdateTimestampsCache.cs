using System;
using System.Collections;
using System.Runtime.CompilerServices;
using log4net;

namespace NHibernate.Cache
{
	/// <summary>
	/// Tracks the timestamps of the most recent updates to particular tables. It is
	/// important that the cache timeout of the underlying cache implementation be set
	/// to a higher value than the timeouts of any of the query caches. In fact, we 
	/// recommend that the the underlying cache not be configured for expiry at all.
	/// Note, in particular, that an LRU cache expiry policy is never appropriate.
	/// </summary>
	public class UpdateTimestampsCache
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( UpdateTimestampsCache ) );
		private static readonly string RegionName = typeof( UpdateTimestampsCache ).Name;

		private ICache updateTimestamps;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="props"></param>
		public UpdateTimestampsCache( ICacheProvider provider, IDictionary props )
		{
			log.Info( "starting update timestamps cache at region: " + RegionName );
			this.updateTimestamps = provider.BuildCache( RegionName, props );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="spaces"></param>
		[MethodImpl( MethodImplOptions.Synchronized )]
		public void PreInvalidate( object[] spaces )
		{
			//TODO: to handle concurrent writes correctly, this should return a Lock to the client
			long ts = updateTimestamps.NextTimestamp() + updateTimestamps.Timeout;
			for ( int i = 0; i < spaces.Length; i++ )
			{
				updateTimestamps.Put( spaces[ i ], ts );
			}
			//TODO: return new Lock(ts);
		}

		/// <summary></summary>
		[MethodImpl( MethodImplOptions.Synchronized )]
		public void Invalidate( object[] spaces )
		{
			long ts = updateTimestamps.NextTimestamp();
			//TODO: if lock.getTimestamp().equals(ts)
			for ( int i = 0; i < spaces.Length; i++ )
			{
				log.Debug( string.Format( "Invalidating space [{0}]", spaces[ i ] ) );
				updateTimestamps.Put( spaces[ i ], ts );
			}
		}
	}
}
