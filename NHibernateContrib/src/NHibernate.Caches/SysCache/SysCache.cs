#region License
//
//  SysCache - A cache provider for NHibernate using System.Web.Caching.Cache.
//
//  This library is free software; you can redistribute it and/or
//  modify it under the terms of the GNU Lesser General Public
//  License as published by the Free Software Foundation; either
//  version 2.1 of the License, or (at your option) any later version.
//
//  This library is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//  Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
#endregion

using System;
using System.Collections;
using System.Web;

using AspCache = System.Web.Caching; // clash with new NHibernate namespace below
using log4net;
using NHibernate.Cache;

namespace NHibernate.Caches.SysCache
{
	/// <summary>
	/// Pluggable cache implementation using the System.Web.Caching classes
	/// </summary>
	public class SysCache : ICache
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( SysCache ) );
		private string _region;
		private AspCache.Cache _cache;
		private DateTime _absExpiration;
		private TimeSpan _slidingExpiration;
		private AspCache.CacheItemPriority _priority;
		private static readonly TimeSpan _defaultRelativeExpiration = TimeSpan.FromSeconds( 300 );
		private static readonly string _cacheKeyPrefix = "NHibernate-Cache:";
		private readonly string _rootCacheKey;
		private bool _rootCacheKeyStored;

		/// <summary>
		/// default constructor
		/// </summary>
		public SysCache() : this( null, null )
		{
		}

		/// <summary>
		/// constructor with no properties
		/// </summary>
		/// <param name="region"></param>
		public SysCache( string region ) : this( region, null )
		{
		}

		/// <summary>
		/// full constructor
		/// </summary>
		/// <param name="region"></param>
		/// <param name="properties">cache configuration properties</param>
		/// <remarks>
		/// There are three (3) configurable parameters:
		/// <ul>
		///		<li>staticExpiration = a specific DateTime to expire each item on</li>
		///		<li>relativeExpiration = number of seconds to wait before expiring each item</li>
		///		<li>priority = a numeric cost of expiring each item, where 1 is a low cost, 5 is the highest, and 3 is normal. Only values 1 through 5 are valid.</li>
		/// </ul>
		/// staticExpiration and relativeExpiration are exclusive - you can only specify one or the other, not both.
		/// All parameters are optional. The defaults are a relativeExpiration of 300 seconds and the default priority of 3.
		/// </remarks>
		/// <exception cref="IndexOutOfRangeException">The "priority" property is not between 1 and 5</exception>
		/// <exception cref="NotSupportedException">"staticExpiration" and "relativeExpiration" properties were both specified.</exception>
		/// <exception cref="ArgumentOutOfRangeException">The "staticExpiration" property is not in the future.</exception>
		/// <exception cref="ArgumentException">The "relativeExpiration" property could not be parsed.</exception>
		public SysCache( string region, IDictionary properties )
		{
			_region = region;
			_cache = HttpRuntime.Cache;
			_rootCacheKey = GenerateRootCacheKey();
			Configure( properties );
			StoreRootCacheKey();
		}

		private void Configure( IDictionary props )
		{
			if( props == null )
			{
				if( log.IsDebugEnabled )
				{
					log.Debug( "configuring cache with default values" );
				}
				_absExpiration = AspCache.Cache.NoAbsoluteExpiration;
				_slidingExpiration = _defaultRelativeExpiration;
				_priority = AspCache.CacheItemPriority.Default;
			}
			else
			{
				if( props["priority"] != null )
				{
					int priority = Convert.ToInt32( props["priority"] );
					if( priority < 1 || priority > 5 )
					{
						if( log.IsWarnEnabled )
						{
							log.Warn( "priority value out of range: " + priority.ToString() );
						}
						throw new IndexOutOfRangeException( "priority must be between 1 and 5" );
					}
					_priority = (AspCache.CacheItemPriority)priority;
					if( log.IsDebugEnabled )
					{
						log.Debug( "new priority: " + _priority.ToString() );
					}
				}
				else
				{
					_priority = AspCache.CacheItemPriority.Default;
				}
				if( props["relativeExpiration"] == null && props["staticExpiration"] == null )
				{
					if( log.IsDebugEnabled )
					{
						log.Debug( "no expiration values given, using defaults" );
					}
					_absExpiration = AspCache.Cache.NoAbsoluteExpiration;
					_slidingExpiration = _defaultRelativeExpiration;
					return;
				}
				if( props["relativeExpiration"] != null && props["staticExpiration"] != null )
				{
					if( log.IsWarnEnabled )
					{
						log.Warn( "both expiration types specified" );
					}
					throw new NotSupportedException( "staticExpiration and relativeExpiration are exclusive - specify one or the other, not both." );
				}
				if( props["relativeExpiration"] == null && props["staticExpiration"] != null )
				{
					DateTime exp;
					if( props["staticExpiration"] is DateTime )
					{
						exp = (DateTime)props["staticExpiration"];
					}
					else
					{
						try
						{
							exp = DateTime.Parse( props["staticExpiration"].ToString() );
						}
						catch( Exception ex )
						{
							if( log.IsWarnEnabled )
							{
								log.Warn( "could not parse static expiration" );
							}
							throw new ArgumentException( "could not parse as DateTime", "staticExpiration", ex );
						}
					}
					if( exp > DateTime.Now )
					{
						_absExpiration = exp;
						if( log.IsDebugEnabled )
						{
							log.Debug( "new static expiration value: " + _absExpiration.ToString() );
						}
					}
					else
					{
						if( log.IsWarnEnabled )
						{
							log.Warn( "static expiration not a future time or date" );
						}
						throw new ArgumentOutOfRangeException( "staticExpiration", exp.ToString(), "staticExpiration must be in the future" );
					}
					_slidingExpiration = _defaultRelativeExpiration;
				}
				else
				{
					_absExpiration = AspCache.Cache.NoAbsoluteExpiration;
					try
					{
						int seconds = Convert.ToInt32( props["relativeExpiration"] );
						_slidingExpiration = TimeSpan.FromSeconds( seconds );
						if( log.IsDebugEnabled )
						{
							log.Debug( "new relative expiration value: " + seconds.ToString() );
						}
					}
					catch( Exception ex )
					{
						if( log.IsWarnEnabled )
						{
							log.Warn( "error parsing relative expiration value" );
						}
						throw new ArgumentException( "could not parse relativeException as a number of seconds", "relativeException", ex );
					}
				}
			}
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
			const string NOT = "not ";

			if( key == null )
			{
				return null;
			}
			string cacheKey = GetCacheKey( key );

			object obj = _cache.Get( cacheKey );
			if( obj == null )
			{
				return null;
			}

			DictionaryEntry de = ( DictionaryEntry ) obj;
			object returnValue = null;

			if( key.Equals( de.Key ) )
			{
				returnValue = de.Value;
			}

			if( log.IsDebugEnabled )
			{
				log.Debug( String.Format( "Attempted to fetch object '{0}' from the cache - {1}FOUND", cacheKey, returnValue == null ? NOT : string.Empty ) );
			}

			return returnValue;
		}

		/// <summary></summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void Put( object key, object value )
		{
			if( key == null )
			{
				throw new ArgumentNullException( "key", "null key not allowed" );
			}
			if( value == null )
			{
				throw new ArgumentNullException( "value", "null value not allowed" );
			}
			string cacheKey = GetCacheKey( key );
			if( _cache[ cacheKey ] != null )
			{
				// Cache item already exists, remove it in preperation for adding it again (otherwise the sliding refresh doesn't work)
				if( log.IsDebugEnabled )
				{
					log.Debug( String.Format("updating value of key '{0}' to '{1}'.", cacheKey, value.ToString() ) );
				}

				_cache.Remove( cacheKey );
			}
			else
			{
				if( log.IsDebugEnabled )
				{
					log.Debug( String.Format("adding new data: key={0}&value={1}&sliding={2}", cacheKey, value.ToString(), _slidingExpiration.ToString() ) );
				}
			}

			if (!_rootCacheKeyStored)
			{
				StoreRootCacheKey();
			}

			// Now add the item with expiration policy
			_cache.Add(
				cacheKey, new DictionaryEntry( key, value ), new AspCache.CacheDependency(null, new string[] { _rootCacheKey }), AspCache.Cache.NoAbsoluteExpiration,
				_slidingExpiration, _priority, null
			);
		}

		/// <summary></summary>
		/// <param name="key"></param>
		public void Remove( object key )
		{
			if( key == null )
			{
				throw new ArgumentNullException( "key" );
			}
			string cacheKey = GetCacheKey( key );
			if( log.IsDebugEnabled )
			{
				log.Debug( "removing item with key: " + cacheKey );
			}
			_cache.Remove( cacheKey );
		}

		/// <summary></summary>
		public void Clear()
		{
			RemoveRootCacheKey();

			StoreRootCacheKey();
		}

		/// <summary>
		/// Generate a unique root key for all cache items to be dependant upon
		/// </summary>
		/// <returns></returns>
		private string GenerateRootCacheKey()
		{
			return GetCacheKey(Guid.NewGuid());
		}

		private void RootCacheKeyRemoved(string key, object value, AspCache.CacheItemRemovedReason reason)
		{
			_rootCacheKeyStored = false;
		}

		/// <summary></summary>
		private void StoreRootCacheKey()
		{
			_rootCacheKeyStored = true;
			_cache.Add(
				_rootCacheKey,
				_rootCacheKey,
				null,
				AspCache.Cache.NoAbsoluteExpiration,
				AspCache.Cache.NoSlidingExpiration,
				AspCache.CacheItemPriority.Default,
				new AspCache.CacheItemRemovedCallback(RootCacheKeyRemoved));
		}

		/// <summary></summary>
		private void RemoveRootCacheKey()
		{
			_cache.Remove(_rootCacheKey);
		}

		/// <summary></summary>
		public void Destroy()
		{
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

		/// <summary></summary>
		public string Region
		{
			set { _region = value; }
		}
	}
}
