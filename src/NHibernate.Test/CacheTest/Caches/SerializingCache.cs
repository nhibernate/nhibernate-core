using System.Collections;
using NHibernate.Cache;

namespace NHibernate.Test.CacheTest.Caches
{
	public partial class SerializingCache : CacheBase
	{
		private readonly IDictionary _hashtable;

		public SerializingCache(string regionName, IDictionary data)
		{
			RegionName = regionName;
			_hashtable = data;
		}

		public override object Get(object key)
		{
			return _hashtable[key];
		}

		public override void Put(object key, object value)
		{
			_hashtable[key] = value;
		}

		public override void Remove(object key)
		{
			_hashtable.Remove(key);
		}

		public override void Clear()
		{
			_hashtable.Clear();
		}

		public override void Destroy()
		{
		}

		public override object Lock(object key)
		{
			// local cache, no need to actually lock.
			return null;
		}

		public override void Unlock(object key, object lockValue)
		{
			// local cache, no need to actually lock.
		}

		public override long NextTimestamp()
		{
			return Timestamper.Next();
		}

		public override int Timeout => Timestamper.OneMs * 60000;

		public override string RegionName { get; }
	}
}
