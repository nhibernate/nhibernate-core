using System;
using System.Collections;
using log4net;
using NHibernate.Cache;

namespace NHibernate.Caches.Prevalence
{
	/// <summary>
	/// Summary description for PrevalenceCache.
	/// </summary>
	public class PrevalenceCache : ICache
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( PrevalenceCache ) );
		private static readonly string _cacheKeyPrefix = "NHibernate-Cache:";
		private string _region;
		private CacheSystem _system;

		/// <summary>
		/// default constructor
		/// </summary>
		public PrevalenceCache() : this( "nhibernate", null )
		{
		}

		/// <summary>
		/// constructor with no properties
		/// </summary>
		/// <param name="region"></param>
		public PrevalenceCache( string region ) : this( region, null )
		{
		}

		/// <summary>
		/// full constructor
		/// </summary>
		/// <param name="region"></param>
		/// <param name="system">the Prevalance container class</param>
		public PrevalenceCache( string region, CacheSystem system )
		{
			_region = region;
			_system = ( system == null ) ? new CacheSystem() : system;
		}

		private string GetCacheKey( object key )
		{
			return String.Concat( _cacheKeyPrefix, _region, ":", key.ToString(), "@", key.GetHashCode() );
		}

		/// <summary></summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public object Get( object key )
		{
			if( key == null )
			{
				return null;
			}
			string cacheKey = GetCacheKey( key );
			if( log.IsDebugEnabled )
			{
				log.Debug( String.Format( "Fetching object '{0}' from the cache.", cacheKey ) );
			}

			object obj = _system.Get( cacheKey );
			if( obj == null )
			{
				return null;
			}

			DictionaryEntry de = ( DictionaryEntry ) obj;

			if( key.Equals( de.Key ) )
			{
				return de.Value;
			}
			else
			{
				return null;
			}
		}

		/// <summary></summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void Put( object key, object value )
		{
			if( key == null )
			{
				if( log.IsErrorEnabled )
				{
					log.Error( "null key passed to 'Put'" );
				}
				throw new ArgumentNullException( "key", "null key not allowed" );
			}
			if( value == null )
			{
				if( log.IsErrorEnabled )
				{
					log.Error( "null value passed to 'Put'" );
				}
				throw new ArgumentNullException( "value", "null value not allowed" );
			}
			string cacheKey = GetCacheKey( key );
			if( log.IsDebugEnabled )
			{
				log.Debug( String.Format( "setting value {1} for key {0}", cacheKey, value.ToString() ) );
			}
			_system.Add( cacheKey, new DictionaryEntry( key, value ) );
		}

		/// <summary></summary>
		/// <param name="key"></param>
		public void Remove( object key )
		{
			if( key == null )
			{
				if( log.IsErrorEnabled )
				{
					log.Error( "null key passed to 'Remove'" );
				}
				throw new ArgumentNullException( "key" );
			}
			string cacheKey = GetCacheKey( key );
			if( log.IsDebugEnabled )
			{
				log.Debug( "removing item with key: " + cacheKey );
			}
			_system.Remove( cacheKey );
		}

		/// <summary></summary>
		public void Clear()
		{
			if( log.IsInfoEnabled )
			{
				log.Info( "clearing all objects from system" );
			}
			_system.Clear();
		}

		/// <summary></summary>
		public void Destroy()
		{
			if( log.IsInfoEnabled )
			{
				log.Info( "'Destroy' was called" );
			}
			Clear();
		}

		/// <summary></summary>
		/// <param name="key"></param>
		public void Lock( object key )
		{
			// Do nothing
		}

		/// <summary></summary>
		/// <param name="key"></param>
		public void Unlock( object key )
		{
			// Do nothing
		}

		/// <summary></summary>
		public long NextTimestamp()
		{
			return Timestamper.Next();
		}

		/// <summary></summary>
		public int Timeout
		{
			get { return Timestamper.OneMs * 60000; } // 60 seconds
		}

		public string RegionName
		{
			get { return _region; }
		}
	}
}
