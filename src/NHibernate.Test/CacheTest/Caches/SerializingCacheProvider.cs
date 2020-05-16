using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate.Cache;

namespace NHibernate.Test.CacheTest.Caches
{
	public class SerializingCacheProvider : ICacheProvider
	{
		#region ICacheProvider Members

		// Since 5.2
		[Obsolete]
		ICache ICacheProvider.BuildCache(string regionName, IDictionary<string, string> properties)
		{
			return BuildCache(regionName, properties);
		}

		public CacheBase BuildCache(string regionName, IDictionary<string, string> properties)
		{
			if (!CacheData.TryGetValue(regionName ?? string.Empty, out var data))
			{
				data = new Hashtable();
				CacheData.Add(regionName ?? string.Empty, data);
			}

			return new SerializingCache(regionName, data);
		}

		public long NextTimestamp()
		{
			return Timestamper.Next();
		}

		public void Start(IDictionary<string, string> properties)
		{
			var serializer = new BinaryFormatter();
			foreach (var cache in SerializedCacheData)
			{
				using (var stream = new MemoryStream(cache.Value))
				{
					CacheData.Add(cache.Key, (IDictionary) serializer.Deserialize(stream));
				}
			}
		}

		public void Stop()
		{
			var serializer = new BinaryFormatter();
			foreach (var cache in CacheData)
			{
				using (var stream = new MemoryStream())
				{
					serializer.Serialize(stream, cache.Value);
					SerializedCacheData[cache.Key] = stream.ToArray();
				}
			}
		}

		#endregion

		private readonly Dictionary<string, IDictionary> CacheData = new Dictionary<string, IDictionary>();
		private static readonly Dictionary<string, byte[]> SerializedCacheData = new Dictionary<string, byte[]>();
	}
}
