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
using System.Configuration;
using System.Text;
using log4net;
using Memcached.ClientLibrary;
using NHibernate.Cache;

namespace NHibernate.Caches.MemCache
{
	/// <summary>
	/// Cache provider using the .NET client (http://sourceforge.net/projects/memcacheddotnet)
	///  for memcached, which is located at http://www.danga.com/memcached/
	/// </summary>
	public class MemCacheProvider : ICacheProvider
	{
		private static readonly ILog _log;
		private static int[] _weights;
		private static string[] _servers;

		static MemCacheProvider()
		{
			_log = LogManager.GetLogger( typeof( MemCacheProvider ) );
			MemCacheConfig[] configs = ConfigurationSettings.GetConfig( "memcache" ) as MemCacheConfig[];
			if( configs != null )
			{
				ArrayList myWeights = new ArrayList();
				ArrayList myServers = new ArrayList();
				foreach( MemCacheConfig config in configs )
				{
					myServers.Add( string.Format( "{0}:{1}", config.Host, config.Port.ToString() ) );
					if( _log.IsDebugEnabled )
					{
						_log.DebugFormat( "adding config for memcached on host {0}", config.Host );
					}
					if( config.Weight > 0 )
					{
						myWeights.Add( config.Weight );
					}
				}
				_servers = (string[]) myServers.ToArray( typeof( string ) );
				_weights = (int[]) myWeights.ToArray( typeof( int ) );
			}
		}

		public ICache BuildCache( string regionName, IDictionary properties )
		{
			if( regionName == null )
			{
				regionName = "";
			}
			if( properties == null )
			{
				properties = new Hashtable();
			}
			if ( _log.IsDebugEnabled )
			{
				StringBuilder sb = new StringBuilder();
				foreach ( DictionaryEntry de in properties )
				{
					sb.Append( "name=" );
					sb.Append( de.Key.ToString() );
					sb.Append( "&value=" );
					sb.Append( de.Value.ToString() );
					sb.Append( ";" );
				}
				_log.Debug( "building cache with region: " + regionName + ", properties: " + sb.ToString() );
			}
			return new MemCacheClient( regionName, properties );
		}

		public long NextTimestamp()
		{
			return Timestamper.Next();
		}

		public void Start( IDictionary properties )
		{
			SockIOPool pool = SockIOPool.GetInstance();
			if( _servers != null && _servers.Length > 0 )
			{
				pool.SetServers( _servers );
			}
			if( _weights != null && _weights.Length > 0 && _weights.Length == _servers.Length )
			{
				pool.SetWeights( _weights );
			}
			if( properties[ "failover" ] != null )
			{
				pool.Failover = Convert.ToBoolean( properties[ "failover" ] );
				if( _log.IsDebugEnabled )
				{
					_log.DebugFormat( "failover set to {0}", pool.Failover );
				}
			}
			if ( properties[ "initial_connections" ] != null )
			{
				pool.InitConnections = Convert.ToInt32( properties[ "initial_connections" ] );
				if ( _log.IsDebugEnabled )
				{
					_log.DebugFormat( "initial_connections set to {0}", pool.InitConnections );
				}
			}
			if ( properties[ "maintenance_sleep" ] != null )
			{
				pool.MaintenanceSleep = Convert.ToInt64( properties[ "maintenance_sleep" ] );
				if ( _log.IsDebugEnabled )
				{
					_log.DebugFormat( "maintenance_sleep set to {0}", pool.MaintenanceSleep );
				}
			}
			if ( properties[ "max_busy" ] != null )
			{
				pool.MaxBusy = Convert.ToInt64( properties[ "max_busy" ] );
				if ( _log.IsDebugEnabled )
				{
					_log.DebugFormat( "max_busy set to {0}", pool.MaxBusy );
				}
			}
			if ( properties[ "max_connections" ] != null )
			{
				pool.MaxConnections = Convert.ToInt32( properties[ "max_connections" ] );
				if ( _log.IsDebugEnabled )
				{
					_log.DebugFormat( "max_connections set to {0}", pool.MaxConnections );
				}
			}
			if ( properties[ "max_idle" ] != null )
			{
				pool.MaxIdle = Convert.ToInt64( properties[ "max_idle" ] );
				if ( _log.IsDebugEnabled )
				{
					_log.DebugFormat( "max_idle set to {0}", pool.MaxIdle );
				}
			}
			if ( properties[ "min_connections" ] != null )
			{
				pool.MinConnections = Convert.ToInt32( properties[ "min_connections" ] );
				if ( _log.IsDebugEnabled )
				{
					_log.DebugFormat( "min_connections set to {0}", pool.MinConnections );
				}
			}
			if ( properties[ "nagle" ] != null )
			{
				pool.Nagle = Convert.ToBoolean( properties[ "nagle" ] );
				if ( _log.IsDebugEnabled )
				{
					_log.DebugFormat( "nagle set to {0}", pool.Nagle );
				}
			}
			if ( properties[ "socket_timeout" ] != null )
			{
				pool.SocketTimeout = Convert.ToInt32( properties[ "socket_timeout" ] );
				if ( _log.IsDebugEnabled )
				{
					_log.DebugFormat( "socket_timeout set to {0}", pool.SocketTimeout );
				}
			}
			if ( properties[ "socket_connect_timeout" ] != null )
			{
				pool.SocketConnectTimeout = Convert.ToInt32( properties[ "socket_connect_timeout" ] );
				if ( _log.IsDebugEnabled )
				{
					_log.DebugFormat( "socket_connect_timeout set to {0}", pool.SocketConnectTimeout );
				}
			}
			pool.Initialize();
		}

		public void Stop()
		{
			SockIOPool.GetInstance().Shutdown();
		}
	}
}