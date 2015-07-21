using NHibernate.SqlTypes;

namespace NHibernate.Persister.Entity
{
	/// <summary> 
	/// Contract for things that can be locked via a <see cref="NHibernate.Dialect.Lock.ILockingStrategy"/>.
	/// </summary>
	/// <remarks>
	/// Currently only the root table gets locked, except for the case of HQL and Criteria queries
	/// against dialects which do not support either (1) FOR UPDATE OF or (2) support hint locking
	/// (in which case *all* queried tables would be locked).
	/// </remarks>
	public interface ILockable : IEntityPersister
	{
		/// <summary> 
		/// Locks are always applied to the "root table".
		///  </summary>
		string RootTableName { get;}

		/// <summary> 
		/// Get the names of columns on the root table used to persist the identifier. 
		/// </summary>
		string[] RootTableIdentifierColumnNames { get;}

		/// <summary> 
		/// For versioned entities, get the name of the column (again, expected on the
		/// root table) used to store the version values. 
		/// </summary>
		string VersionColumnName { get;}

		/// <summary> 
		/// Get the SQL alias this persister would use for the root table
		/// given the passed driving alias. 
		/// </summary>
		/// <param name="drivingAlias">
		/// The driving alias; or the alias for the table mapped by this persister in the hierarchy.
		/// </param>
		/// <returns> The root table alias. </returns>
		string GetRootTableAlias(string drivingAlias);

		#region NH Specific
		/// <summary>
		/// To build the SQL command in pessimistic lock
		/// </summary>
		SqlType[] IdAndVersionSqlTypes { get;}

		#endregion
	}
}