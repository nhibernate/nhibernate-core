using System;
using System.Collections;

using log4net;

namespace NHibernate.Cache
{
	/// <summary>
	/// Caches data that is never updated
	/// </summary>
	public class ReadOnlyCache : ICacheConcurrencyStrategy
	{
		private object lockObject = new object();

		private ICache cache;
		private static readonly ILog log = LogManager.GetLogger( typeof( ReadOnlyCache ) );

		/// <summary>
		/// Gets the cache region name.
		/// </summary>
		public string RegionName
		{
			get { return cache.RegionName; }
		}

		public ICache Cache
		{
			get { return cache; }
			set { cache = value; }
		}

		public object Get( object key, long timestamp )
		{
			lock( lockObject )
			{
				object result = cache.Get( key );
				if( result != null && log.IsDebugEnabled )
				{
					log.Debug( "Cache hit: " + key );
				}
				return result;
			}
		}

		/// <summary>
		/// Unsupported!
		/// </summary>
		public ISoftLock Lock( object key, object version )
		{
			log.Error( "Application attempted to edit read only item: " + key );
			throw new InvalidOperationException( "Can't write to a readonly object" );
		}

		public bool Put( object key, object value, long timestamp, object version, IComparer versionComparator, bool minimalPut )
		{
			if( timestamp == long.MinValue )
			{
				// MinValue means cache is disabled
				return false;
			}

			lock( lockObject )
			{
				if( minimalPut && cache.Get( key ) != null )
				{
					if( log.IsDebugEnabled )
					{
						log.Debug( "item already cached: " + key );
					}
					return false;
				}
				if( log.IsDebugEnabled )
				{
					log.Debug( "Caching: " + key );
				}
				cache.Put( key, value );
				return true;
			}
		}

		/// <summary>
		/// Unsupported!
		/// </summary>
		public void Release( object key, ISoftLock @lock )
		{
			log.Error( "Application attempted to edit read only item: " + key );
			//throw new InvalidOperationException( "Can't write to a readonly object" );
		}

		public void Clear()
		{
			cache.Clear();
		}

		public void Remove( object key )
		{
			cache.Remove( key );
		}

		public void Destroy()
		{
			try
			{
				cache.Destroy();
			}
			catch( Exception e )
			{
				log.Warn( "Could not destroy cache", e );
			}
		}

		/// <summary>
		/// Unsupported!
		/// </summary>
		public void AfterUpdate( object key, object value, object version, ISoftLock @lock )
		{
			log.Error( "Application attempted to edit read only item: " + key );
			throw new InvalidOperationException( "Can't write to a readonly object" );
		}

		/// <summary>
		/// Do nothing.
		/// </summary>
		public void AfterInsert( object key, object value, object version )
		{
			// Ignore
		}

		/// <summary>
		/// Do nothing.
		/// </summary>
		public void Evict( object key )
		{
			// NOOP
		}

		/// <summary>
		/// Do nothing.
		/// </summary>
		public void Insert( object key, object value )
		{
			// NOOP
		}

		/// <summary>
		/// Unsupported!
		/// </summary>
		public void Update( object key, object value )
		{
			log.Error( "Application attempted to edit read only item: " + key );
			throw new InvalidOperationException( "Can't write to a readonly object" );
		}
	}
}
