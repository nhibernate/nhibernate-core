using System.Collections;

namespace NHibernate.Cache
{
	/// <summary> 
	/// Contract for sources of optimistically lockable data sent to the second level cache.
	/// </summary>
	/// <remarks>
	/// Note currently <see cref="NHibernate.Persister.Entity.IEntityPersister">EntityPersisters</see> are
	/// the only viable source.
	/// </remarks>
	public interface IOptimisticCacheSource
	{
		/// <summary> 
		/// Does this source represent versioned (i.e., and thus optimistically lockable) data? 
		/// </summary>
		/// <returns> True if this source represents versioned data; false otherwise. </returns>
		bool IsVersioned { get;}

		/// <summary> Get the comparator used to compare two different version values together. </summary>
		/// <returns> An appropriate comparator. </returns>
		IComparer VersionComparator { get;}
	}
}