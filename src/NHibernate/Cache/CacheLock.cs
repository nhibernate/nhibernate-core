using System;
using System.Collections;
using System.Runtime.Serialization;
using NHibernate.Cache.Access;

namespace NHibernate.Cache
{
	/// <summary>
	/// A soft lock which supports concurrent locking,
	/// timestamped with the time it was released
	/// </summary>
	/// <remarks>
	/// This class was named Lock in H2.1
	/// </remarks>
	[Serializable]
	[DataContract]
	public class CacheLock : ReadWriteCache.ILockable, ISoftLock, ReadWriteCache.IMinimalPutAwareLockable
	{
		private long unlockTimestamp = -1;
		private int multiplicity = 1;
		private bool concurrentLock = false;
		private long timeout;
		private int id;
		private object version;

		public CacheLock()
		{
		}

		// Since 5.2
		[Obsolete("Use object initializer instead.")]
		public CacheLock(long timeout, int id, object version)
		{
			Timeout = timeout;
			Id = id;
			Version = version;
		}

		internal static CacheLock Create(long timeout, int id, object version)
		{
			return new CacheLock
			{
				Version = version,
				Timeout = timeout,
				Id = id
			};
		}

		/// <summary>
		/// Increment the lock, setting the
		/// new lock timeout
		/// </summary>
		public CacheLock Lock(long timeout, int id)
		{
			WasLockedConcurrently = true;
			Multiplicity++;
			Timeout = timeout;
			return this;
		}

		/// <summary>
		/// Decrement the lock, setting the unlock
		/// timestamp if now unlocked
		/// </summary>
		/// <param name="currentTimestamp"></param>
		public void Unlock(long currentTimestamp)
		{
			if (--Multiplicity == 0)
			{
				UnlockTimestamp = currentTimestamp;
			}
		}

		/// <summary>
		/// Can the timestamped transaction re-cache this
		/// locked item now?
		/// </summary>
		public bool IsPuttable(long txTimestamp, object newVersion, IComparer comparator, bool minimalPut)
		{
			if (Timeout < txTimestamp)
			{
				return true;
			}
			if (Multiplicity > 0)
			{
				return false;
			}

			return Version == null 
				? !minimalPut && UnlockTimestamp < txTimestamp 
				: comparator.Compare(Version, newVersion) < (minimalPut ? 0 : 1);
			//by requiring <, we rely on lock timeout in the case of an unsuccessful update!
		}

		/// <summary>
		/// Can the timestamped transaction re-cache this
		/// locked item now?
		/// </summary>
		public bool IsPuttable(long txTimestamp, object newVersion, IComparer comparator)
		{
			return IsPuttable(txTimestamp, newVersion, comparator, true);
		}

		/// <summary>
		/// Was this lock held concurrently by multiple
		/// transactions?
		/// </summary>
		// 6.0 TODO convert to auto-property
		[DataMember]
		public bool WasLockedConcurrently
		{
			get => concurrentLock;
			set => concurrentLock = value;
		}

		/// <summary>
		/// Yes, this is a lock
		/// </summary>
		public bool IsLock => true;

		/// <summary>
		/// locks are not returned to the client!
		/// </summary>
		public bool IsGettable(long txTimestamp)
		{
			return false;
		}

		// 6.0 TODO convert to auto-property
		[DataMember]
		public int Id
		{
			get => id;
			set => id = value;
		}

		// 6.0 TODO convert to auto-property
		[DataMember]
		public object Version
		{
			get => version;
			set => version = value;
		}

		// 6.0 TODO convert to auto-property
		[DataMember]
		public long UnlockTimestamp
		{
			get => unlockTimestamp;
			set => unlockTimestamp = value;
		}

		// 6.0 TODO convert to auto-property
		[DataMember]
		public int Multiplicity
		{
			get => multiplicity;
			set => multiplicity = value;
		}

		// 6.0 TODO convert to auto-property
		[DataMember]
		public long Timeout
		{
			get => timeout;
			set => timeout = value;
		}

		public override string ToString()
		{
			return "CacheLock{id=" + Id +
			       ",version=" + Version +
			       ",multiplicity=" + Multiplicity +
			       ",unlockTimestamp=" + UnlockTimestamp +
			       "}";
		}
	}
}
