using System.Collections;

namespace NHibernate.Cache
{
	/// <summary>
	/// A simple <see cref="Hashtable" />-based cache
	/// </summary>
	public partial class HashtableCache : ICache
	{
		private IDictionary hashtable = new Hashtable();
		private readonly string regionName;

		#region ICache Members

		public HashtableCache(string regionName)
		{
			this.regionName = regionName;
		}

		/// <summary></summary>
		public object Get(object key)
		{
			return hashtable[key];
		}

		/// <summary></summary>
		public void Put(object key, object value)
		{
			hashtable[key] = value;
		}

		/// <summary></summary>
		public void Remove(object key)
		{
			hashtable.Remove(key);
		}

		/// <summary></summary>
		public void Clear()
		{
			hashtable.Clear();
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
			get { return Timestamper.OneMs * 60000; // ie. 60 seconds
			}
		}

		public string RegionName
		{
			get { return regionName; }
		}

		#endregion
	}
}