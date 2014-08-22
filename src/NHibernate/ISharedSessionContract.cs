using System;
using System.Collections;
using System.Data;
using System.Linq.Expressions;
using NHibernate.Engine;
using NHibernate.Stat;
using NHibernate.Type;

namespace NHibernate
{
	/// <summary>
	/// Base interface for <see cref="ISession"/> and <see cref="IStatelessSession"/>.
	/// </summary>
	public interface ISharedSessionContract : IDisposable
	{
		/// <summary>
		/// Gets the ADO.NET connection.
		/// </summary>
		/// <remarks>
		/// Applications are responsible for calling commit/rollback upon the connection before
		/// closing the <c>ISession</c>.
		/// </remarks>
		IDbConnection Connection { get; }

		/// <summary>
		/// Is the <c>ISession</c> still open?
		/// </summary>
		bool IsOpen { get; }

		/// <summary>
		/// Is the <c>ISession</c> currently connected?
		/// </summary>
		bool IsConnected { get; }

		/// <summary>
		/// Update the persistent instance with the identifier of the given transient instance.
		/// </summary>
		/// <remarks>
		/// If there is a persistent instance with the same identifier, an exception is thrown. If
		/// the given transient instance has a <see langword="null" /> identifier, an exception will be thrown.
		/// </remarks>
		/// <param name="obj">A transient instance containing updated state</param>
		void Update(object obj);

		/// <summary>
		/// Update the persistent instance with the identifier of the given detached
		/// instance.
		/// </summary>
		/// <param name="entityName">The Entity name.</param>
		/// <param name="obj">a detached instance containing updated state </param>
		/// <remarks>
		/// If there is a persistent instance with the same identifier,
		/// an exception is thrown. This operation cascades to associated instances
		/// if the association is mapped with <tt>cascade="save-update"</tt>.
		/// </remarks>
		void Update(string entityName, object obj);

		/// <summary>
		/// Remove a persistent instance from the datastore.
		/// </summary>
		/// <remarks>
		/// The argument may be an instance associated with the receiving <c>ISession</c> or a
		/// transient instance with an identifier associated with existing persistent state.
		/// </remarks>
		/// <param name="obj">The instance to be removed</param>
		void Delete(object obj);

		/// <summary>
		/// Remove a persistent instance from the datastore. The <b>object</b> argument may be
		/// an instance associated with the receiving <see cref="ISession"/> or a transient
		/// instance with an identifier associated with existing persistent state.
		/// This operation cascades to associated instances if the association is mapped
		/// with <tt>cascade="delete"</tt>.
		/// </summary>
		/// <param name="entityName">The entity name for the instance to be removed. </param>
		/// <param name="obj">the instance to be removed </param>
		void Delete(string entityName, object obj);

		/// <summary>
		/// Re-read the state of the given instance from the underlying database.
		/// </summary>
		/// <remarks>
		/// <para>
		/// It is inadvisable to use this to implement long-running sessions that span many
		/// business tasks. This method is, however, useful in certain special circumstances.
		/// </para>
		/// <para>
		/// For example,
		/// <list>
		///		<item>Where a database trigger alters the object state upon insert or update</item>
		///		<item>After executing direct SQL (eg. a mass update) in the same session</item>
		///		<item>After inserting a <c>Blob</c> or <c>Clob</c></item>
		/// </list>
		/// </para>
		/// </remarks>
		/// <param name="obj">A persistent instance</param>
		void Refresh(object obj);

		/// <summary>
		/// Re-read the state of the given instance from the underlying database, with
		/// the given <c>LockMode</c>.
		/// </summary>
		/// <remarks>
		/// It is inadvisable to use this to implement long-running sessions that span many
		/// business tasks. This method is, however, useful in certain special circumstances.
		/// </remarks>
		/// <param name="obj">a persistent or transient instance</param>
		/// <param name="lockMode">the lock mode to use</param>
		void Refresh(object obj, LockMode lockMode);

		/// <summary>
		/// Begin a unit of work and return the associated <c>ITransaction</c> object.
		/// </summary>
		/// <remarks>
		/// If a new underlying transaction is required, begin the transaction. Otherwise
		/// continue the new work in the context of the existing underlying transaction.
		/// The class of the returned <see cref="ITransaction" /> object is determined by
		/// the property <c>transaction_factory</c>
		/// </remarks>
		/// <returns>A transaction instance</returns>
		ITransaction BeginTransaction();

		/// <summary>
		/// Begin a transaction with the specified <c>isolationLevel</c>
		/// </summary>
		/// <param name="isolationLevel">Isolation level for the new transaction</param>
		/// <returns>A transaction instance having the specified isolation level</returns>
		ITransaction BeginTransaction(IsolationLevel isolationLevel);

		/// <summary>
		/// Get the current Unit of Work and return the associated <c>ITransaction</c> object.
		/// </summary>
		ITransaction Transaction { get; }

		/// <summary>
		/// Creates a new <c>Criteria</c> for the entity class.
		/// </summary>
		/// <typeparam name="T">The entity class</typeparam>
		/// <returns>An ICriteria object</returns>
		ICriteria CreateCriteria<T>() where T : class;

		/// <summary>
		/// Creates a new <c>Criteria</c> for the entity class with a specific alias
		/// </summary>
		/// <typeparam name="T">The entity class</typeparam>
		/// <param name="alias">The alias of the entity</param>
		/// <returns>An ICriteria object</returns>
		ICriteria CreateCriteria<T>(string alias) where T : class;

		/// <summary>
		/// Creates a new <c>Criteria</c> for the entity class.
		/// </summary>
		/// <param name="persistentClass">The class to Query</param>
		/// <returns>An ICriteria object</returns>
		ICriteria CreateCriteria(System.Type persistentClass);

		/// <summary>
		/// Creates a new <c>Criteria</c> for the entity class with a specific alias
		/// </summary>
		/// <param name="persistentClass">The class to Query</param>
		/// <param name="alias">The alias of the entity</param>
		/// <returns>An ICriteria object</returns>
		ICriteria CreateCriteria(System.Type persistentClass, string alias);

		/// <summary>
		/// Create a new <c>Criteria</c> instance, for the given entity name.
		/// </summary>
		/// <param name="entityName">The name of the entity to Query</param>
		/// <returns>An ICriteria object</returns>
		ICriteria CreateCriteria(string entityName);

		/// <summary>
		/// Create a new <c>Criteria</c> instance, for the given entity name,
		/// with the given alias.
		/// </summary>
		/// <param name="entityName">The name of the entity to Query</param>
		/// <param name="alias">The alias of the entity</param>
		/// <returns>An ICriteria object</returns>
		ICriteria CreateCriteria(string entityName, string alias);

		/// <summary>
		/// Creates a new <c>IQueryOver&lt;T&gt;</c> for the entity class.
		/// </summary>
		/// <typeparam name="T">The entity class</typeparam>
		/// <returns>An IQueryOver&lt;T&gt; object</returns>
		IQueryOver<T, T> QueryOver<T>() where T : class;

		/// <summary>
		/// Creates a new <c>IQueryOver&lt;T&gt;</c> for the entity class.
		/// </summary>
		/// <typeparam name="T">The entity class</typeparam>
		/// <param name="alias">The alias of the entity</param>
		/// <returns>An IQueryOver&lt;T&gt; object</returns>
		IQueryOver<T, T> QueryOver<T>(Expression<Func<T>> alias) where T : class;

		/// <summary>
		/// Create a new instance of <c>Query</c> for the given query string
		/// </summary>
		/// <param name="queryString">A hibernate query string</param>
		/// <returns>The query</returns>
		IQuery CreateQuery(string queryString);

		/// <summary>
		/// Obtain an instance of <see cref="IQuery" /> for a named query string defined in the
		/// mapping file.
		/// </summary>
		/// <param name="queryName">The name of a query defined externally.</param>
		/// <returns>An <see cref="IQuery"/> from a named query string.</returns>
		/// <remarks>
		/// The query can be either in <c>HQL</c> or <c>SQL</c> format.
		/// </remarks>
		IQuery GetNamedQuery(string queryName);

		/// <summary>
		/// Create a new instance of <see cref="ISQLQuery" /> for the given SQL query string.
		/// </summary>
		/// <param name="queryString">a query expressed in SQL</param>
		/// <returns>An <see cref="ISQLQuery"/> from the SQL string</returns>
		ISQLQuery CreateSQLQuery(string queryString);

		/// <summary>
		/// Return the persistent instance of the given named entity with the given identifier,
		/// or null if there is no such persistent instance. (If the instance, or a proxy for the
		/// instance, is already associated with the session, return that instance or proxy.)
		/// </summary>
		/// <param name="entityName">the entity name </param>
		/// <param name="id">an identifier </param>
		/// <returns> a persistent instance or null </returns>
		object Get(string entityName, object id);

		/// <summary>
		/// Strongly-typed version of Get(System.Type, object)
		/// </summary>
		T Get<T>(object id);

		/// <summary>
		/// Strongly-typed version of Get(System.Type, object, LockMode)
		/// </summary>
		T Get<T>(object id, LockMode lockMode);

		/// <summary>
		/// Gets the session implementation.
		/// </summary>
		/// <remarks>
		/// This method is provided in order to get the <b>NHibernate</b> implementation of the session from wrapper implementions.
		/// Implementors of the <seealso cref="ISession"/> interface should return the NHibernate implementation of this method.
		/// </remarks>
		/// <returns>
		/// An NHibernate implementation of the <seealso cref="ISessionImplementor"/> interface
		/// </returns>
		ISessionImplementor GetSessionImplementation();
	}
}
