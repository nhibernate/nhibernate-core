using System.Data;
using NHibernate.Cache;
using NHibernate.Id;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace NHibernate.Engine
{
	/// <summary>
	/// Defines the internal contract between the <c>ISessionFactory</c> and other parts of NHibernate
	/// such as implementors of <c>IType</c>.
	/// </summary>
	public interface ISessionFactoryImplementor : IMapping, ISessionFactory
	{
		/// <summary>
		/// Get the persister for a class
		/// </summary>
		IEntityPersister GetEntityPersister( System.Type clazz );

		/// <summary>
		/// Get the persister for the named class
		/// </summary>
		/// <param name="className">The name of the class that is persisted.</param>
		/// <returns>The <see cref="IEntityPersister"/> for the class.</returns>
		/// <exception cref="MappingException">If no <see cref="IEntityPersister"/> can be found.</exception>
		IEntityPersister GetEntityPersister( string className );

		/// <summary>
		/// Get the persister for the named class
		/// </summary>
		/// <param name="className">The name of the class that is persisted.</param>
		/// <param name="throwIfNotFound">Whether to throw an exception if the class is not found,
		/// or just return <c>null</c></param>
		/// <returns>The <see cref="IEntityPersister"/> for the class.</returns>
		/// <exception cref="MappingException">If no <see cref="IEntityPersister"/> can be found
		/// and throwIfNotFound is true.</exception>
		IEntityPersister GetEntityPersister( string className, bool throwIfNotFound );
		
		/// <summary>
		/// Get the persister object for a collection role
		/// </summary>
		/// <param name="role"></param>
		/// <returns></returns>
		ICollectionPersister GetCollectionPersister( string role );

		/// <summary>
		/// Is outerjoin fetching enabled?
		/// </summary>
		bool IsOuterJoinedFetchEnabled { get; }
		
		/// <summary>
		/// Are scrollable <c>ResultSet</c>s supported?
		/// </summary>
		bool IsScrollableResultSetsEnabled { get; }

		/// <summary>
		/// Is <c>PreparedStatement.getGeneratedKeys</c> supported (Java-specific?)
		/// </summary>
		bool IsGetGeneratedKeysEnabled { get; }

		/// <summary>
		/// Get the database schema specified in <c>hibernate.default_schema</c>
		/// </summary>
		string DefaultSchema { get; }

		/// <summary>
		/// Get the return types of a query
		/// </summary>
		/// <param name="queryString"></param>
		/// <returns></returns>
		IType[ ] GetReturnTypes( string queryString );

		/// <summary>
		/// Get the names of all persistent classes that implement/extend the given interface/class
		/// </summary>
		/// <param name="clazz"></param>
		/// <returns></returns>
		string[ ] GetImplementors( System.Type clazz );

		/// <summary>
		/// Get a class name, using query language imports
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		string GetImportedClassName( string name );

		/// <summary>
		/// 
		/// </summary>
		int BatchSize { get; }

		/// <summary>
		/// 
		/// </summary>
		int FetchSize { get; }

		/// <summary>
		/// Maximum depth of outer join fetching
		/// </summary>
		int MaximumFetchDepth { get; }

		/// <summary>
		/// Are we logging SQL to the console?
		/// </summary>
		bool IsShowSqlEnabled { get; }

		/// <summary>
		/// Get the default query cache
		/// </summary>
		IQueryCache QueryCache { get; }

		/// <summary>
		/// Get a particular named query cache, or the default cache
		/// </summary>
		/// <param name="regionName">the name of the cache region, or null for the default
		/// query cache</param>
		/// <returns>the existing cache, or a newly created cache if none by that
		/// region name</returns>
		IQueryCache GetQueryCache( string regionName );
		
		/// <summary>
		/// Is query caching enabled?
		/// </summary>
		bool IsQueryCacheEnabled { get; }

		/// <summary>
		/// Obtain an ADO.NET connection
		/// </summary>
		/// <returns></returns>
		IDbConnection OpenConnection();

		/// <summary>
		/// Release an ADO.NET connection
		/// </summary>
		/// <param name="conn"></param>
		void CloseConnection( IDbConnection conn );

		/// <summary>
		/// Gets the IsolationLevel an IDbTransaction should be set to.
		/// </summary>
		/// <remarks>
		/// This is only applicable to manually controlled NHibernate Transactions.
		/// </remarks>
		IsolationLevel Isolation { get; }

		/// <summary>
		/// Gets a boolean indicating if the sql statement should be prepared.  The value
		/// is read from <c>hibernate.prepare_sql</c>.
		/// </summary>
		bool PrepareSql { get; }

		// TODO H2.1:
		// bool IsWrapDataReadersEnabled { get; }

		/// <summary>
		/// Get the identifier generator for the hierarchy
		/// </summary>
		IIdentifierGenerator GetIdentifierGenerator( System.Type rootClass );

		ResultSetMappingDefinition GetResultSetMapping(string resultSetRef);
	}
}