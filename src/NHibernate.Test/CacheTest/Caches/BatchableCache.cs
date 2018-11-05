using System.Collections;
using System.Collections.Generic;
using NHibernate.Cache;

namespace NHibernate.Test.CacheTest.Caches
{
	public partial class BatchableCache : CacheBase
	{
		public override bool PreferMultipleGet => true;

		private readonly IDictionary _hashtable = new Hashtable();

		public List<object[]> GetMultipleCalls { get; } = new List<object[]>();

		public List<object[]> PutMultipleCalls { get; } = new List<object[]>();

		public List<object[]> LockMultipleCalls { get; } = new List<object[]>();

		public List<object[]> UnlockMultipleCalls { get; } = new List<object[]>();

		public List<object> GetCalls { get; } = new List<object>();

		public List<object> PutCalls { get; } = new List<object>();

		public BatchableCache(string regionName)
		{
			RegionName = regionName;
		}

		public override object[] GetMany(object[] keys)
		{
			GetMultipleCalls.Add(keys);
			var result = new object[keys.Length];
			for (var i = 0; i < keys.Length; i++)
			{
				result[i] = _hashtable[keys[i]];
			}
			return result;
		}

		public override void PutMany(object[] keys, object[] values)
		{
			PutMultipleCalls.Add(keys);
			for (int i = 0; i < keys.Length; i++)
			{
				_hashtable[keys[i]] = values[i];
			}
		}

		public override object LockMany(object[] keys)
		{
			LockMultipleCalls.Add(keys);
			return null;
		}

		public override void UnlockMany(object[] keys, object lockValue)
		{
			UnlockMultipleCalls.Add(keys);
		}

		public override object Get(object key)
		{
			GetCalls.Add(key);
			return _hashtable[key];
		}

		public override void Put(object key, object value)
		{
			PutCalls.Add(key);
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

		public void ClearStatistics()
		{
			GetCalls.Clear();
			GetMultipleCalls.Clear();
			PutMultipleCalls.Clear();
			PutCalls.Clear();
			UnlockMultipleCalls.Clear();
			LockMultipleCalls.Clear();
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
