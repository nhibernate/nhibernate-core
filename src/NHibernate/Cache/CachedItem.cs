using System;
using System.Collections;
using System.Runtime.Serialization;

namespace NHibernate.Cache
{
	/// <summary>
	/// An item of cached data, timestamped with the time it was cached, when it was locked,
	/// when it was unlocked
	/// </summary>
	[Serializable]
	[DataContract]
	public class CachedItem : ReadWriteCache.ILockable
	{
		private long freshTimestamp;
		private object value;
		private object version;

		public CachedItem()
		{
		}

		// Since 5.2
		[Obsolete("Use object initializer instead.")]
		public CachedItem(object value, long currentTimestamp, object version)
		{
			Value = value;
			FreshTimestamp = currentTimestamp;
			Version = version;
		}

		internal static CachedItem Create(object value, long currentTimestamp, object version)
		{
			return new CachedItem
			{
				Version = version,
				FreshTimestamp = currentTimestamp,
				Value = value
			};
		}

		/// <summary>
		/// The timestamp on the cached data
		/// </summary>
		// 6.0 TODO convert to auto-property
		[DataMember]
		public long FreshTimestamp
		{
			get => freshTimestamp;
			set => freshTimestamp = value;
		}

		/// <summary>
		/// The actual cached data
		/// </summary>
		// 6.0 TODO convert to auto-property
		[DataMember]
		public object Value
		{
			get => value;
			set => this.value = value;
		}

		/// <summary>
		/// The version of the cached data
		/// </summary>
		// 6.0 TODO convert to auto-property
		[DataMember]
		public object Version
		{
			get => version;
			set => version = value;
		}

		/// <summary>
		/// Lock the item
		/// </summary>
		public CacheLock Lock(long timeout, int id)
		{
			return CacheLock.Create(timeout, id, Version);
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
			return FreshTimestamp < txTimestamp;
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
			return Version != null && comparator.Compare(Version, newVersion) < 0;
		}

		public override string ToString()
		{
			return "Item{version=" + Version +
			       ",freshTimestamp=" + FreshTimestamp +
			       "}";
		}
	}
}
