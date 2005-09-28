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
		private bool minimalPuts;

		/// <summary></summary>
		public ReadOnlyCache()
		{
		}

		public ICache Cache
		{
			get { return cache; }
			set { cache = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="timestamp"></param>
		/// <returns></returns>
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
		/// <param name="key"></param>
		/// <param name="version"></param>
		public ISoftLock Lock( object key, object version )
		{
			log.Error( "Application attempted to edit read only item: " + key );
			throw new InvalidOperationException( "Can't write to a readonly object" );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="timestamp"></param>
		/// <returns></returns>
		public bool Put( object key, object value, long timestamp, object version, IComparer versionComparator )
		{
			if( timestamp == long.MinValue )
			{
				// MinValue means cache is disabled
				return false;
			}

			lock( lockObject )
			{
				if( minimalPuts && cache.Get( key ) != null )
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
		/// <param name="key"></param>
		/// <param name="lock"></param>
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
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="version"></param>
		/// <param name="lock"></param>
		public void AfterUpdate( object key, object value, object version, ISoftLock @lock )
		{
			log.Error( "Application attempted to edit read only item: " + key );
			throw new InvalidOperationException( "Can't write to a readonly object" );
		}

		/// <summary>
		/// Do nothing.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="version"></param>
		public void AfterInsert( object key, object value, object version )
		{
			// Ignore
		}

		/// <summary>
		/// Do nothing.
		/// </summary>
		/// <param name="key"></param>
		public void Evict( object key )
		{
			// NOOP
		}

		/// <summary>
		/// Do nothing.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void Insert( object key, object value )
		{
			// NOOP
		}

		/// <summary>
		/// Unsupported!
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void Update( object key, object value )
		{
			log.Error( "Application attempted to edit read only item: " + key );
			throw new InvalidOperationException( "Can't write to a readonly object" );
		}

		public bool MinimalPuts
		{
			set { minimalPuts = value; }	
		}
	}
}