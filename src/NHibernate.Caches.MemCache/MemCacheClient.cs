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
using log4net;
using Memcached.ClientLibrary;
using NHibernate.Cache;

namespace NHibernate.Caches.MemCache
{
	public class MemCacheClient : ICache
	{
		private static readonly ILog _log;
		private string _region;
		private int _expiry;
		private MemcachedClient _client;
		
		static MemCacheClient()
		{
			_log = LogManager.GetLogger( typeof( MemCacheClient ) );
		}

		public MemCacheClient() : this( null, null )
		{
		}

		public MemCacheClient( string regionName ) : this( regionName, null )
		{
		}

		public MemCacheClient( string regionName, IDictionary properties )
		{
			_region = regionName;
			_client = new MemcachedClient();
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
		
		private string KeyAsString( object key )
		{
			return string.Format( "{0}@{1}", _region, ( key == null ? string.Empty : key.ToString() ) );
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
			return _client.Get( KeyAsString( key ) );
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
			bool returnOK = _client.Set( KeyAsString( key ), value, DateTime.Now.AddSeconds( _expiry ) );
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
	}
}
