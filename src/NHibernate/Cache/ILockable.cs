using System;

namespace NHibernate.Cache
{
	/// <summary>
	/// Summary description for ILockable.
	/// </summary>
	public interface ILockable
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="timeout"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		ISoftLock Lock( long timeout, int id );

		/// <summary>
		/// 
		/// </summary>
		bool IsLock { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="txTimestamp"></param>
		/// <returns></returns>
		bool IsGettable( long txTimestamp );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="txTimestamp"></param>
		/// <param name="newVersion"></param>
		/// <returns></returns>
		bool IsPuttable( long txTimestamp, object newVersion ); // IComparator comparator );
	}
}
