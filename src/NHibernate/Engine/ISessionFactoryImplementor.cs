using System;
using System.Collections;
using System.Data;
using NHibernate.Connection;
using NHibernate.Persister;
using NHibernate.Collection;
using NHibernate.Dialect;
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
		/// TODO: determine if this is more appropriate for ISessionFactory
		/// </summary>
		IConnectionProvider ConnectionProvider {get;}
		
		/// <summary>
		/// Gets the IsolationLevel an IDbTransaction should be set to.
		/// </summary>
		/// <remarks>
		/// This is only applicable to manually controlled NHibernate Transactions.
		/// </remarks>
		IsolationLevel Isolation { get; }

		/// <summary>
		/// Get the persister for a class
		/// </summary>
		IClassPersister GetPersister(System.Type clazz);

		/// <summary>
		/// Get the persister for the named class
		/// </summary>
		/// <param name="className"></param>
		/// <returns></returns>
		IClassPersister GetPersister(string className);

		/// <summary>
		/// Get the persister object for a collection role
		/// </summary>
		/// <param name="role"></param>
		/// <returns></returns>
		CollectionPersister GetCollectionPersister(string role);

		/// <summary>
		/// Is outerjoin fetching enabled?
		/// </summary>
		bool EnableJoinedFetch { get; }

		/// <summary>
		/// Are scrollable <c>ResultSet</c>s supported
		/// </summary>
		bool UseScrollableResultSets { get; } //TODO: Depricate, as there is no such thing

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
		IType[] GetReturnTypes(string queryString);

		/// <summary>
		/// Get the named parameter names for a query
		/// </summary>
		/// <param name="queryString"></param>
		/// <returns></returns>
		ICollection GetNamedParameters(string queryString);

		/// <summary>
		/// Obtain an ADO.NET connection
		/// </summary>
		/// <returns></returns>
		IDbConnection OpenConnection();

		/// <summary>
		/// Release an ADO.NET connection
		/// </summary>
		/// <param name="conn"></param>
		void CloseConnection(IDbConnection conn);

		/// <summary>
		/// Get the names of all persistent classes that implement/extend the given interface/class
		/// </summary>
		/// <param name="clazz"></param>
		/// <returns></returns>
		string[] GetImplementors(System.Type clazz);

		/// <summary>
		/// Get a class name, using query language imports
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		string GetImportedClassName(string name);

		/// <summary>
		/// The ADO.NET batch size 
		/// </summary>
		int ADOBatchSize { get; } //TODO: Depricate, should always be 0

		/// <summary>
		/// Set the fetch size
		/// </summary>
		/// <param name="command"></param>
		void SetFetchSize(IDbCommand command);
	}
}
