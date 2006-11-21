#region License
//
//  MemCache - A cache provider for NHibernate using the .NET client
//  (http://sourceforge.net/projects/memcacheddotnet) for memcached,
//  which is located at http://www.danga.com/memcached/.
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
// CLOVER:OFF
//
#endregion

using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using log4net;
using Memcached.ClientLibrary;
using NHibernate.Cache;

namespace NHibernate.Caches.MemCache
{
	public class MemCacheClient : ICache
	{
		private HashAlgorithm hasher = HashAlgorithm.Create();
		private static readonly ILog _log;
		private string _region;
		private int _expiry;
		private MemcachedClient _client;
		
		static MemCacheClient()
		{
			_log = LogManager.GetLogger( typeof( MemCacheClient ) );
		}

		public MemCacheClient() : this( "nhibernate", null )
		{
		}

		public MemCacheClient( string regionName ) : this( regionName, null )
		{
		}

		public MemCacheClient( string regionName, IDictionary properties )
		{
			_region = regionName;
			_client = new MemcachedClient();
			_client.PoolName = "nhibernate";
			_expiry = 300;
			
			if( properties != null )
			{
				if ( properties[ "compression_enabled" ] != null )
				{
					_client.EnableCompression = Convert.ToBoolean( properties[ "compression_enabled" ] );
					if ( _log.IsDebugEnabled )
					{
						_log.DebugFormat( "compression_enabled set to {0}", _client.EnableCompression );
					}
				}
				if( properties[ "expiration" ] != null )
				{
					_expiry = Convert.ToInt32( properties[ "expiration" ] );
					if( _log.IsDebugEnabled )
					{
						_log.DebugFormat( "using expiration of {0} seconds", _expiry );
					}
				}
			}
		}
		
		/// <summary>
		/// Turn the key obj into a string, preperably using human readable
		/// string, and if the srtring is too long (>=250) it will be hashed
		/// </summary>
		private string KeyAsString( object key )
		{
			string fullKey = string.Format( "{0}@{1}", _region, ( key == null ? string.Empty : key.ToString() ) );
			if(fullKey.Length>=250)//max key size for memcache
			{
				byte[] bytes = Encoding.ASCII.GetBytes(fullKey);
				byte[] computedHash = hasher.ComputeHash(bytes);
				return Convert.ToBase64String(computedHash);
			}
			return fullKey.Replace(' ', '-');
		}

		public object Get( object key )
		{
			if( key == null )
			{
				return null;
			}
			if( _log.IsDebugEnabled )
			{
				_log.DebugFormat( "fetching object {0} from the cache", key );
			}
			object maybeObj = _client.Get(KeyAsString(key));
			if(maybeObj==null)
				return null;
			//we need to check here that the key that we stored is really the key that we got
			//the reason is that for long keys, we hash the value, and this mean that we may get
			//hash collisions. The chance is very low, but it is better to be safe
			DictionaryEntry de = (DictionaryEntry)maybeObj;
			if(key.Equals(de.Key)==false)
				return null;
			return de.Value;
		}

		public void Put( object key, object value )
		{
			if ( key == null )
			{
				throw new ArgumentNullException( "key", "null key not allowed" );
			}
			if ( value == null )
			{
				throw new ArgumentNullException( "value", "null value not allowed" );
			}

			if( _log.IsDebugEnabled )
			{
				_log.DebugFormat( "setting value for item {0}", key );
			}
			bool returnOK = _client.Set(KeyAsString(key), new DictionaryEntry(key, value), DateTime.Now.AddSeconds(_expiry));
			if( !returnOK )
			{
				if( _log.IsWarnEnabled )
				{
					_log.WarnFormat( "could not save: {0} => {1}", key, value );
				}
			}
		}

		public void Remove( object key )
		{
			if ( key == null )
			{
				throw new ArgumentNullException( "key" );
			}
			if( _log.IsDebugEnabled )
			{
				_log.DebugFormat( "removing item {0}", key );
			}
			_client.Delete( KeyAsString( key ), DateTime.Now.AddSeconds( _expiry ) );
		}

		public void Clear()
		{
			_client.FlushAll();
		}

		public void Destroy()
		{
			Clear();
		}

		public void Lock( object key )
		{
			// do nothing
		}

		public void Unlock( object key )
		{
			// do nothing
		}

		public long NextTimestamp()
		{
			return Timestamper.Next();
		}

		public int Timeout
		{
			get { return Timestamper.OneMs * 60000; }
		}

		public string RegionName
		{
			get { return _region; }
		}
	}
}
