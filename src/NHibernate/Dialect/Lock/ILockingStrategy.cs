using NHibernate.Engine;

namespace NHibernate.Dialect.Lock
{
	/// <summary> 
	/// A strategy abstraction for how locks are obtained in the underlying database.
	/// </summary>
	/// <remarks>
	/// All locking provided implementations assume the underlying database supports
	/// (and that the connection is in) at least read-committed transaction isolation.
	/// The most glaring exclusion to this is HSQLDB which only offers support for
	/// READ_UNCOMMITTED isolation.
	/// </remarks>
	/// <seealso cref="NHibernate.Dialect.Dialect.GetLockingStrategy"/>
	public partial interface ILockingStrategy
	{
		/// <summary> 
		/// Acquire an appropriate type of lock on the underlying data that will
		/// endure until the end of the current transaction.
		/// </summary>
		/// <param name="id">The id of the row to be locked </param>
		/// <param name="version">The current version (or null if not versioned) </param>
		/// <param name="obj">The object logically being locked (currently not used) </param>
		/// <param name="session">The session from which the lock request originated </param>
		void Lock(object id, object version, object obj, ISessionImplementor session);
	}
}