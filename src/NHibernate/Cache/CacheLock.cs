using System;
using System.Collections;
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
	public class CacheLock : ReadWriteCache.ILockable, ISoftLock
	{
		private long unlockTimestamp = -1;
		private int multiplicity = 1;
		private bool concurrentLock = false;
		private long timeout;
		private readonly int id;
		private readonly object version;

		public CacheLock(long timeout, int id, object version)
		{
			this.timeout = timeout;
			this.id = id;
			this.version = version;
		}

		/// <summary>
		/// Increment the lock, setting the
		/// new lock timeout
		/// </summary>
		public CacheLock Lock(long timeout, int id)
		{
			concurrentLock = true;
			multiplicity++;
			this.timeout = timeout;
			return this;
		}

		/// <summary>
		/// Decrement the lock, setting the unlock
		/// timestamp if now unlocked
		/// </summary>
		/// <param name="currentTimestamp"></param>
		public void Unlock(long currentTimestamp)
		{
			if (--multiplicity == 0)
			{
				unlockTimestamp = currentTimestamp;
			}
		}

		/// <summary>
		/// Can the timestamped transaction re-cache this
		/// locked item now?
		/// </summary>
		public bool IsPuttable(long txTimestamp, object newVersion, IComparer comparator)
		{
			if (timeout < txTimestamp)
			{
				return true;
			}
			if (multiplicity > 0)
			{
				return false;
			}
			return version == null ?
			       unlockTimestamp < txTimestamp :
			       comparator.Compare(version, newVersion) < 0;
				//by requiring <, we rely on lock timeout in the case of an unsuccessful update!
		}

		/// <summary>
		/// Was this lock held concurrently by multiple
		/// transactions?
		/// </summary>
		public bool WasLockedConcurrently
		{
			get { return concurrentLock; }
		}

		/// <summary>
		/// Yes, this is a lock
		/// </summary>
		public bool IsLock
		{
			get { return true; }
		}

		/// <summary>
		/// locks are not returned to the client!
		/// </summary>
		public bool IsGettable(long txTimestamp)
		{
			return false;
		}

		public int Id
		{
			get { return id; }
		}

		public override string ToString()
		{
			return "CacheLock{id=" + id +
			       ",version=" + version +
			       ",multiplicity=" + multiplicity +
			       ",unlockTimestamp=" + unlockTimestamp +
			       "}";
		}
	}
}