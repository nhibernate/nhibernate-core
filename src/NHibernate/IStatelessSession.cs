using System;
using System.Data;

namespace NHibernate
{
	/// <summary> 
	/// A command-oriented API for performing bulk operations against a database. 
	/// </summary>
	/// <remarks>
	/// A stateless session does not implement a first-level cache nor
	/// interact with any second-level cache, nor does it implement
	/// transactional write-behind or automatic dirty checking, nor do
	/// operations cascade to associated instances. Collections are
	/// ignored by a stateless session. Operations performed via a
	/// stateless session bypass Hibernate's event model and
	/// interceptors. Stateless sessions are vulnerable to data
	/// aliasing effects, due to the lack of a first-level cache.
	/// <para/>
	/// For certain kinds of transactions, a stateless session may
	/// perform slightly faster than a stateful session. 
	/// </remarks>
	public interface IStatelessSession : IDisposable
	{
		/// <summary> Get the current Hibernate transaction.</summary>
		ITransaction Transaction { get;}

		/// <summary> Close the stateless session and release the ADO.NET connection.</summary>
		void Close();

		/// <summary> Insert a entity.</summary>
		/// <param name="entity">A new transient instance </param>
		/// <returns> the identifier of the instance </returns>
		object Insert(object entity);

		/// <summary> Insert a row. </summary>
		/// <param name="entityName">The entityName for the entity to be inserted </param>
		/// <param name="entity">a new transient instance </param>
		/// <returns> the identifier of the instance </returns>
		object Insert(string entityName, object entity);

		/// <summary> Update a entity.</summary>
		/// <param name="entity">a detached entity instance </param>
		void Update(object entity);

		/// <summary>Update a entity.</summary>
		/// <param name="entityName">The entityName for the entity to be updated </param>
		/// <param name="entity">a detached entity instance </param>
		void Update(string entityName, object entity);

		/// <summary> Delete a entity. </summary>
		/// <param name="entity">a detached entity instance </param>
		void Delete(object entity);

		/// <summary> Delete a entity. </summary>
		/// <param name="entityName">The entityName for the entity to be deleted </param>
		/// <param name="entity">a detached entity instance </param>
		void Delete(string entityName, object entity);

		/// <summary> Retrieve a entity. </summary>
		/// <returns> a detached entity instance </returns>
		object Get(string entityName, object id);

		/// <summary> Retrieve a entity.
		/// 
		/// </summary>
		/// <returns> a detached entity instance
		/// </returns>
		T Get<T>(object id);

		/// <summary> 
		/// Retrieve a entity, obtaining the specified lock mode. 
		/// </summary>
		/// <returns> a detached entity instance </returns>
		object Get(string entityName, object id, LockMode lockMode);

		/// <summary> 
		/// Retrieve a entity, obtaining the specified lock mode. 
		/// </summary>
		/// <returns> a detached entity instance </returns>
		T Get<T>(object id, LockMode lockMode);

		/// <summary> 
		/// Refresh the entity instance state from the database. 
		/// </summary>
		/// <param name="entity">The entity to be refreshed. </param>
		void Refresh(object entity);

		/// <summary> 
		/// Refresh the entity instance state from the database. 
		/// </summary>
		/// <param name="entityName">The entityName for the entity to be refreshed. </param>
		/// <param name="entity">The entity to be refreshed.</param>
		void Refresh(string entityName, object entity);

		/// <summary> 
		/// Refresh the entity instance state from the database. 
		/// </summary>
		/// <param name="entity">The entity to be refreshed. </param>
		/// <param name="lockMode">The LockMode to be applied.</param>
		void Refresh(object entity, LockMode lockMode);

		/// <summary> 
		/// Refresh the entity instance state from the database. 
		/// </summary>
		/// <param name="entityName">The entityName for the entity to be refreshed. </param>
		/// <param name="entity">The entity to be refreshed. </param>
		/// <param name="lockMode">The LockMode to be applied. </param>
		void Refresh(string entityName, object entity, LockMode lockMode);

		/// <summary>
		/// Create a new instance of <tt>Query</tt> for the given HQL query string.
		/// </summary>
		/// <remarks>Entities returned by the query are detached.</remarks>
		IQuery CreateQuery(string queryString);

		/// <summary> 
		/// Obtain an instance of <see cref="IQuery"/> for a named query string defined in
		/// the mapping file.
		/// </summary>
		/// <remarks>
 		/// The query can be either in <c>HQL</c> or <c>SQL</c> format.
		/// Entities returned by the query are detached.
		/// </remarks>
		IQuery GetNamedQuery(string queryName);

		/// <summary>
		/// Create a new <see cref="ICriteria"/> instance, for the given entity class,
		/// or a superclass of an entity class. 
		/// </summary>
		/// <typeparam name="T">A class, which is persistent, or has persistent subclasses</typeparam>
		/// <returns> The <see cref="ICriteria"/>. </returns>
		/// <remarks>Entities returned by the query are detached.</remarks>
		ICriteria CreateCriteria<T>() where T: class;

		/// <summary>
		/// Create a new <see cref="ICriteria"/> instance, for the given entity class,
		/// or a superclass of an entity class, with the given alias. 
		/// </summary>
		/// <typeparam name="T">A class, which is persistent, or has persistent subclasses</typeparam>
		/// <param name="alias">The alias of the entity</param>
		/// <returns> The <see cref="ICriteria"/>. </returns>
		/// <remarks>Entities returned by the query are detached.</remarks>
		ICriteria CreateCriteria<T>(string alias) where T : class;

		/// <summary>
		/// Create a new <see cref="ICriteria"/> instance, for the given entity class,
		/// or a superclass of an entity class. 
		/// </summary>
		/// <param name="entityType">A class, which is persistent, or has persistent subclasses</param>
		/// <returns> The <see cref="ICriteria"/>. </returns>
		/// <remarks>Entities returned by the query are detached.</remarks>
		ICriteria CreateCriteria(System.Type entityType);

		/// <summary>
		/// Create a new <see cref="ICriteria"/> instance, for the given entity class,
		/// or a superclass of an entity class, with the given alias. 
		/// </summary>
		/// <param name="entityType">A class, which is persistent, or has persistent subclasses</param>
		/// <param name="alias">The alias of the entity</param>
		/// <returns> The <see cref="ICriteria"/>. </returns>
		/// <remarks>Entities returned by the query are detached.</remarks>
		ICriteria CreateCriteria(System.Type entityType, string alias);

		/// <summary> 
		/// Create a new <see cref="ICriteria"/> instance, for the given entity name.
		/// </summary>
		/// <param name="entityName">The entity name. </param>
		/// <returns> The <see cref="ICriteria"/>. </returns>
		/// <remarks>Entities returned by the query are detached.</remarks>
		ICriteria CreateCriteria(string entityName);

		/// <summary> 
		/// Create a new <see cref="ICriteria"/> instance, for the given entity name,
		/// with the given alias.  
		/// </summary>
		/// <param name="entityName">The entity name. </param>
		/// <param name="alias">The alias of the entity</param>
		/// <returns> The <see cref="ICriteria"/>. </returns>
		/// <remarks>Entities returned by the query are detached.</remarks>
		ICriteria CreateCriteria(string entityName, string alias);

		/// <summary> 
		/// Create a new instance of <see cref="ISQLQuery"/> for the given SQL query string.
		/// Entities returned by the query are detached.
		/// </summary>
		/// <param name="queryString">a SQL query </param>
		/// <returns> The <see cref="ISQLQuery"/> </returns>
		ISQLQuery CreateSQLQuery(string queryString);

		/// <summary> Begin a NHibernate transaction.</summary>
		ITransaction BeginTransaction();

		/// <summary> 
		/// Returns the current ADO.NET connection associated with this instance.
		/// </summary>
		/// <remarks>
		/// If the session is using aggressive connection release (as in a
		/// CMT environment), it is the application's responsibility to
		/// close the connection returned by this call. Otherwise, the
		/// application should not close the connection.
		/// </remarks>
		IDbConnection Connection { get; }
	}
}
