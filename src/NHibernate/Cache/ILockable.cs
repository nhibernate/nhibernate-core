using System;
using System.Collections;

namespace NHibernate.Cache
{
	/// <summary>
	/// Summary description for ILockable.
	/// </summary>
	public interface ILockable
	{
		CacheLock Lock( long timeout, int id );

		bool IsLock { get; }

		bool IsGettable( long txTimestamp );

		bool IsPuttable( long txTimestamp, object newVersion, IComparer comparator );
	}
}
