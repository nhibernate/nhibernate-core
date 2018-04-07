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
	public partial class BatchableCache : ICache, IBatchableReadWriteCache
	{
		private readonly IDictionary _hashtable = new Hashtable();
		private readonly string _regionName;

		public List<object[]> GetMultipleCalls { get; } = new List<object[]>();

		public List<object[]> PutMultipleCalls { get; } = new List<object[]>();

		public List<object[]> LockMultipleCalls { get; } = new List<object[]>();

		public List<object[]> UnlockMultipleCalls { get; } = new List<object[]>();

		public List<object> GetCalls { get; } = new List<object>();

		public List<object> PutCalls { get; } = new List<object>();

		public void PutMultiple(object[] keys, object[] values)
		{
			PutMultipleCalls.Add(keys);
			for (int i = 0; i < keys.Length; i++)
			{
				_hashtable[keys[i]] = values[i];
			}
		}

		public void LockMultiple(object[] keys)
		{
			LockMultipleCalls.Add(keys);
		}

		public void UnlockMultiple(object[] keys)
		{
			UnlockMultipleCalls.Add(keys);
		}

		#region ICache Members

		public BatchableCache(string regionName)
		{
			_regionName = regionName;
		}

		/// <summary></summary>
		public object Get(object key)
		{
			GetCalls.Add(key);
			return _hashtable[key];
		}

		public object[] GetMultiple(object[] keys)
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
		public int Timeout
		{
			get
			{
				return Timestamper.OneMs * 60000; // ie. 60 seconds
			}
		}

		public string RegionName
		{
			get { return _regionName; }
		}

		#endregion
	}
}
