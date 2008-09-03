using System;
using System.Collections;

namespace NHibernate.Cache
{
	/// <summary>
	/// An item of cached data, timestamped with the time it was cached, when it was locked,
	/// when it was unlocked
	/// </summary>
	[Serializable]
	public class CachedItem : ReadWriteCache.ILockable
	{
		private readonly long freshTimestamp;
		private readonly object value;
		private readonly object version;

		public CachedItem(object value, long currentTimestamp, object version)
		{
			this.value = value;
			freshTimestamp = currentTimestamp;
			this.version = version;
		}

		/// <summary>
		/// The timestamp on the cached data
		/// </summary>
		public long FreshTimestamp
		{
			get { return freshTimestamp; }
		}

		/// <summary>
		/// The actual cached data
		/// </summary>
		public object Value
		{
			get { return value; }
		}

		/// <summary>
		/// Lock the item
		/// </summary>
		public CacheLock Lock(long timeout, int id)
		{
			return new CacheLock(timeout, id, version);
		}

		/// <summary>
		/// Not a lock!
		/// </summary>
		public bool IsLock
		{
			get { return false; }
		}

		/// <summary>
		/// Is this item visible to the timestamped transaction?
		/// </summary>
		/// <param name="txTimestamp"></param>
		/// <returns></returns>
		public bool IsGettable(long txTimestamp)
		{
			return freshTimestamp < txTimestamp;
		}

		/// <summary>
		/// Don't overwrite already cached items
		/// </summary>
		/// <param name="txTimestamp"></param>
		/// <param name="newVersion"></param>
		/// <param name="comparator"></param>
		/// <returns></returns>
		public bool IsPuttable(long txTimestamp, object newVersion, IComparer comparator)
		{
			// we really could refresh the item if it  
			// is not a lock, but it might be slower
			//return freshTimestamp < txTimestamp
			return version != null && comparator.Compare(version, newVersion) < 0;
		}

		public override string ToString()
		{
			return "Item{version=" + version +
			       ",freshTimestamp=" + freshTimestamp +
			       "}";
		}
	}
}
