using System.Collections;
using System.Data;
using NHibernate.Collection;
using NHibernate.Connection;
using NHibernate.Persister;
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
		IClassPersister GetPersister( System.Type clazz );

		/// <summary>
		/// Get the persister for the named class
		/// </summary>
		/// <param name="className">The name of the class that is persisted.</param>
		/// <returns>The <see cref="IClassPersister"/> for the class.</returns>
		/// <exception cref="MappingException">If no <see cref="IClassPersister"/> can be found.</exception>
		IClassPersister GetPersister( string className );

		/// <summary>
		/// Get the persister for the named class
		/// </summary>
		/// <param name="className">The name of the class that is persisted.</param>
		/// <param name="throwException"><c>true</c> if the exception should be thrown if no <see cref="IClassPersister"/> is found.</param>
		/// <returns>The <see cref="IClassPersister"/> for the class.</returns>
		/// <exception cref="MappingException">If no <see cref="IClassPersister"/> can be found and throwException is <c>true</c>.</exception>
		IClassPersister GetPersister( string className, bool throwException );

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
		/// Are scrollable <tt>ResultSet</tt>s supported?
		/// </summary>
		bool IsScrollableResultSetsEnabled { get; }

		/// <summary>
		/// Get the database schema specified in <tt>hibernate.default_schema</tt>
		/// </summary>
		bool IsGetGeneratedKeysEnabled { get; }

		/// <summary>
		/// Get the database schema specified in <c>hibernate.default_schema</c>
		/// </summary>
		string DefaultSchema { get; }

		/// <summary>
		/// Get the SQL <c>Dialect</c>
		/// </summary>
		Dialect.Dialect Dialect { get; }

		/// <summary>
		/// Get the return types of a query
		/// </summary>
		/// <param name="queryString"></param>
		/// <returns></returns>
		IType[ ] GetReturnTypes( string queryString );

		/// <summary>
		/// TODO: determine if this is more appropriate for ISessionFactory
		/// </summary>
		IConnectionProvider ConnectionProvider { get; }

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
		/// 
		/// </summary>
		int MaximumFetchDepth { get; }

		/// <summary>
		/// Get the named parameter names for a query
		/// </summary>
		/// <param name="queryString"></param>
		/// <returns></returns>
		ICollection GetNamedParameters( string queryString );

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
	}
}