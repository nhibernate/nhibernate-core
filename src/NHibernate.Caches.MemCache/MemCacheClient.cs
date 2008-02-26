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
using System.Collections.Generic;
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
		private MD5 md5 = MD5.Create();
		private static readonly ILog _log;
		private string _region;
		private string _regionPrefix = "";
		private int _expiry;
		private MemcachedClient _client;

		static MemCacheClient()
		{
			_log = LogManager.GetLogger(typeof(MemCacheClient));
		}

		public MemCacheClient()
			: this("nhibernate", null)
		{
		}

		public MemCacheClient(string regionName)
			: this(regionName, null)
		{
		}

		public MemCacheClient(string regionName, IDictionary<string,string> properties)
		{
			_region = regionName;
			_client = new MemcachedClient();
			_client.PoolName = "nhibernate";
			_expiry = 300;

			if (properties != null)
			{
				if (properties.ContainsKey("compression_enabled"))
				{
					_client.EnableCompression = Convert.ToBoolean(properties["compression_enabled"]);
					if (_log.IsDebugEnabled)
					{
						_log.DebugFormat("compression_enabled set to {0}", _client.EnableCompression);
					}
				}
				if (properties.ContainsKey("expiration"))
				{
					_expiry = Convert.ToInt32(properties["expiration"]);
					if (_log.IsDebugEnabled)
					{
						_log.DebugFormat("using expiration of {0} seconds", _expiry);
					}
				}

				if (properties.ContainsKey("regionPrefix"))
				{
					_regionPrefix = properties["regionPrefix"].ToString();

					if (_log.IsDebugEnabled)
					{
						_log.DebugFormat("new regionPrefix :{0}", _regionPrefix);
					}
				}
				else
				{
					if (_log.IsDebugEnabled)
					{
						_log.Debug("no regionPrefix value given, using defaults");
					}
				}
			}
		}

		/// <summary>
		/// Turn the key obj into a string, preperably using human readable
		/// string, and if the string is too long (>=250) it will be hashed
		/// </summary>
		private string KeyAsString(object key)
		{
			string fullKey = FullKeyAsString(key);
			if (fullKey.Length >= 250) //max key size for memcache
				return ComputeHash(fullKey, hasher);
			else
				return fullKey.Replace(' ', '-');
		}

		/// <summary>
		/// Turn the key object into a human readable string.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		private string FullKeyAsString(object key)
		{
			return string.Format("{0}{1}@{2}", _regionPrefix, _region, (key == null ? string.Empty : key.ToString()));
		}

		/// <summary>
		/// Compute the hash of the full key string using the given hash algorithm
		/// </summary>
		/// <param name="fullKeyString">The full key return by call FullKeyAsString</param>
		/// <param name="hashAlgorithm">The hash algorithm used to hash the key</param>
		/// <returns>The hashed key as a string</returns>
		private string ComputeHash(string fullKeyString, HashAlgorithm hashAlgorithm)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(fullKeyString);
			byte[] computedHash = hashAlgorithm.ComputeHash(bytes);
			return Convert.ToBase64String(computedHash);
		}

		/// <summary>
		/// Compute an alternate key hash; used as a check that the looked-up value is 
		/// in fact what has been put there in the first place.
		/// </summary>
		/// <param name="key"></param>
		/// <returns>The alternate key hash (using the MD5 algorithm)</returns>
		private string GetAlternateKeyHash(object key)
		{
			string fullKey = FullKeyAsString(key);
			if (fullKey.Length >= 250)
				return ComputeHash(fullKey, md5);
			else
				return fullKey.Replace(' ', '-');
		}

		public object Get(object key)
		{
			if (key == null)
			{
				return null;
			}
			if (_log.IsDebugEnabled)
			{
				_log.DebugFormat("fetching object {0} from the cache", key);
			}
			object maybeObj = _client.Get(KeyAsString(key));
			if (maybeObj == null)
				return null;
			//we need to check here that the key that we stored is really the key that we got
			//the reason is that for long keys, we hash the value, and this mean that we may get
			//hash collisions. The chance is very low, but it is better to be safe
			DictionaryEntry de = (DictionaryEntry) maybeObj;
			string checkKeyHash = GetAlternateKeyHash(key);
			if (checkKeyHash.Equals(de.Key))
				return de.Value;
			else
				return null;
		}

		public void Put(object key, object value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key", "null key not allowed");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value", "null value not allowed");
			}

			if (_log.IsDebugEnabled)
			{
				_log.DebugFormat("setting value for item {0}", key);
			}
			bool returnOK = _client.Set(KeyAsString(key), new DictionaryEntry(GetAlternateKeyHash(key), value), DateTime.Now.AddSeconds(_expiry)); 
			if (!returnOK)
			{
				if (_log.IsWarnEnabled)
				{
					_log.WarnFormat("could not save: {0} => {1}", key, value);
				}
			}
		}

		public void Remove(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (_log.IsDebugEnabled)
			{
				_log.DebugFormat("removing item {0}", key);
			}
			_client.Delete(KeyAsString(key), DateTime.Now.AddSeconds(_expiry));
		}

		public void Clear()
		{
			_client.FlushAll();
		}

		public void Destroy()
		{
			Clear();
		}

		public void Lock(object key)
		{
			// do nothing
		}

		public void Unlock(object key)
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
