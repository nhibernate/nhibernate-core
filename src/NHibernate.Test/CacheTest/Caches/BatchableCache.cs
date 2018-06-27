using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Cache;

namespace NHibernate.Test.CacheTest.Caches
{
	public partial class BatchableCache : ICache, IBatchableCache
	{
		private readonly IDictionary _hashtable = new Hashtable();

		public List<object[]> GetMultipleCalls { get; } = new List<object[]>();

		public List<object[]> PutMultipleCalls { get; } = new List<object[]>();

		public List<object[]> LockMultipleCalls { get; } = new List<object[]>();

		public List<object[]> UnlockMultipleCalls { get; } = new List<object[]>();

		public List<object> GetCalls { get; } = new List<object>();

		public List<object> PutCalls { get; } = new List<object>();

		public void PutMany(object[] keys, object[] values)
		{
			PutMultipleCalls.Add(keys);
			for (int i = 0; i < keys.Length; i++)
			{
				_hashtable[keys[i]] = values[i];
			}
		}

		public object LockMany(object[] keys)
		{
			LockMultipleCalls.Add(keys);
			return null;
		}

		public void UnlockMany(object[] keys, object lockValue)
		{
			UnlockMultipleCalls.Add(keys);
		}

		#region ICache Members

		public BatchableCache(string regionName)
		{
			RegionName = regionName;
		}

		/// <summary></summary>
		public object Get(object key)
		{
			GetCalls.Add(key);
			return _hashtable[key];
		}

		public object[] GetMany(object[] keys)
		{
			GetMultipleCalls.Add(keys);
			var result = new object[keys.Length];
			for (var i = 0; i < keys.Length; i++)
			{
				result[i] = _hashtable[keys[i]];
			}
			return result;
		}

		/// <summary></summary>
		public void Put(object key, object value)
		{
			PutCalls.Add(key);
			_hashtable[key] = value;
		}

		/// <summary></summary>
		public void Remove(object key)
		{
			_hashtable.Remove(key);
		}

		/// <summary></summary>
		public void Clear()
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

		/// <summary></summary>
		public void Destroy()
		{
		}

		/// <summary></summary>
		public void Lock(object key)
		{
			// local cache, so we use synchronization
		}

		/// <summary></summary>
		public void Unlock(object key)
		{
			// local cache, so we use synchronization
		}

		/// <summary></summary>
		public long NextTimestamp()
		{
			return Timestamper.Next();
		}

		/// <summary></summary>
		public int Timeout => Timestamper.OneMs * 60000;

		public string RegionName { get; }

		#endregion
	}
}
