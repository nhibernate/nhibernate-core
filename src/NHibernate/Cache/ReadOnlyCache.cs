using System;
using log4net;

namespace NHibernate.Cache
{
	/// <summary>
	/// Caches data that is never updated
	/// </summary>
	public class ReadOnlyCache : ICacheConcurrencyStrategy
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( ReadOnlyCache ) );

		private object lockObject = new object();
		private ICache _cache;

		/// <summary></summary>
		public ReadOnlyCache()
		{
		}

		#region ICacheConcurrencyStrategy Members

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
				if( log.IsDebugEnabled )
				{
					log.Debug( "Cache lookup: " + key );
				}

				object result = _cache.Get( key );
				if( result != null && log.IsDebugEnabled )
				{
					log.Debug( "Cache hit" );
				}
				else if( log.IsDebugEnabled )
				{
					log.Debug( "Cache miss" );
				}
				return result;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		public ISoftLock Lock( object key )
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
		public bool Put( object key, object value, long timestamp )
		{
			lock( lockObject )
			{
				if( log.IsDebugEnabled )
				{
					log.Debug( "Caching: " + key );
				}
				_cache.Put( key, value );
				return true;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="lock"></param>
		public void Release( object key, ISoftLock @lock )
		{
			log.Error( "Application attempted to edit read only item: " + key );
			throw new InvalidOperationException( "Can't write to a readonly object" );
		}

		/// <summary>
		/// 
		/// </summary>
		public void Clear()
		{
			_cache.Clear();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		public void Remove( object key )
		{
			_cache.Remove( key );
		}

		/// <summary>
		/// 
		/// </summary>
		public void Destroy()
		{
			try
			{
				_cache.Destroy();
			}
			catch( Exception e )
			{
				log.Warn( "Could not destroy cache", e );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public ICache Cache
		{
			get { return _cache; }
			set { _cache = value; }
		}

		#endregion
	}
}