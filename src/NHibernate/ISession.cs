using System;
using System.Data;
using System.Collections;
using NHibernate.Type;

namespace NHibernate {

	/// <summary>
	/// The main runtime interface between a Java application and Hibernate. This is the central
	/// API class abstracting the notion of a persistence service.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The lifecycle of a <c>ISession</c> is bounded by the beginning and end of a logical
	/// transaction. (Long transactions might span several database transactions.)
	/// </para>
	/// <para>
	/// The main function of the <c>ISession</c> is to offer create, find and delete operations
	/// for instances of mapped entity classes. Instances may exist in one of two states:
	/// <list>
	///		<item>transient: not associated with any <c>ISession</c></item>
	///		<item>persistent: associated with a <c>ISession</c></item>
	/// </list>
	/// </para>
	/// <para>
	///	Transient instances may be made persistent by calling <c>Save()</c>, <c>Insert()</c>,
	///	or <c>Update()</c>. Persistent instances may be made transient by calling <c>Delete()</c>.
	///	Any instance returned by a <c>Find()</c>, <c>Iterate()</c>, <c>Load()</c>, or <c>Create</c>
	///	method is persistent.
	/// </para>
	/// <para>
	/// <c>Create()</c>, <c>Save()</c>, or <c>Insert()</c> result in an SQL <c>CREATE</c>, <c>Delete()</c>
	/// in an SQL <c>DELETE</c> and <c>Update()</c> in an SQL <c>UPDATE</c>. Changes to persistent instances
	/// are deteced at flush time and also result in an SQL <c>UPDATE</c>.
	/// </para>
	/// <para>
	/// It is not intended that implementors be threadsave. Instead each thread/transaction should obtain
	/// its own instance from a <c>SessionFactory</c>.
	/// </para>
	/// <para>
	/// A <c>ISession</c> instance is serializable if its persistent classes are serializable
	/// </para>
	/// <para>
	/// A typical transaction should use the following idiom:
	///		<code>
	///			ISession sess = factory.OpenSession();
	///			Transaction tx;
	///			try {
	///				tx = sess.BeginTransaction();
	///				//do some work
	///				...
	///				tx.Commit();
	///			} catch (Exception e) {
	///				if (tx != null) tx.Rollback();
	///				throw e;
	///			} finally {
	///				sess.Close();
	///			}
	///		</code>
	/// </para>
	/// <para>
	///	If the <c>ISession</c> throws an exception, the transaction must be rolled back and the session
	///	discarded. The internal state of the <c>ISession</c> might not be consistent with the database
	///	after the exception occurs.
	/// </para>
	/// </remarks>
	public interface ISession {
		
		/// <summary>
		/// Force the <c>ISession</c> to flush.
		/// </summary>
		/// <remarks>
		/// Must be called at the end of a unit of work, before commiting the transaction and closing
		/// the session (<c>Transaction.Commit()</c> calls this method). <i>Flushing</i> if the process
		/// of synchronising the underlying persistent store with persistable state held in memory.
		/// </remarks>
		void Flush();

		/// <summary>
		/// Determines at which points Hibernate automatically flushes the session.
		/// </summary>
		/// <remarks>
		/// For a readonly session, it is reasonable to set the flush mode to <c>FlushMode.Never</c>
		/// at the start of the session (in order to achieve some extra performance).
		/// </remarks>
		FlushMode FlushMode { get; set; }

		/// <summary>
		/// Gets the ADO.NET connection.
		/// </summary>
		/// <remarks>
		/// Applications are responsible for calling commit/rollback upon the connection before
		/// closing the <c>ISession</c>.
		/// </remarks>
		IDbConnection Connection { get; }

		/// <summary>
		/// Disconnect the <c>ISession</c> from the current ADO.NET connection.
		/// </summary>
		/// <remarks>
		/// If the connection was obtained by Hibernate, close it or return it to the connection
		/// pool. Otherwise return it to the application. This is used by applications which require
		/// long transactions.
		/// </remarks>
		/// <returns>The connection provided by the application or <c>null</c></returns>
		IDbConnection Disconnect();

		/// <summary>
		/// Obtain a new ADO.NET connection.
		/// </summary>
		/// <remarks>
		/// This is used by applications which require long transactions
		/// </remarks>
		void Reconnect();

		/// <summary>
		/// Reconnect to the given ADO.NET connection.
		/// </summary>
		/// <remarks>This is used by applications which require long transactions</remarks>
		/// <param name="connection">An ADO.NET connection</param>
		void Reconnect(IDbConnection connection);

		/// <summary>
		/// End the <c>ISession</c> by disconnecting from the ADO.NET connection and cleaning up.
		/// </summary>
		/// <remarks>
		/// It is not strickly necessary to <c>Close()</c> the <c>ISession</c> but you must
		/// at least <c>Disconnect()</c> it.
		/// </remarks>
		/// <returns>The connection provided by the application or <c>null</c></returns>
		IDbConnection Close();

		/// <summary>
		/// Is the <c>ISession</c> still open?
		/// </summary>
		bool IsOpen { get; }

		/// <summary>
		/// Is the <c>ISession</c> currently connected?
		/// </summary>
		bool IsConnected { get; }

		/// <summary>
		/// Return the identifier of an entity instance cached by the <c>ISession</c>
		/// </summary>
		/// <remarks>
		/// Throws an exception if the instance is transient or associated with a different
		/// <c>ISession</c>
		/// </remarks>
		/// <param name="obj">a persistent instance</param>
		/// <returns>the identifier</returns>
		object GetIdentifier(object obj);

		/// <summary>
		/// Return the persistent instance of the given entity class with the given identifier,
		/// obtaining the specified lock mode.
		/// </summary>
		/// <param name="theType">A persistent class</param>
		/// <param name="id">A valid identifier of an existing persistent instance of the class</param>
		/// <param name="lockMode">The lock level</param>
		/// <returns>the persistent instance</returns>
		object Load(System.Type theType, object id, LockMode lockMode);

		/// <summary>
		/// Return the persistent instance of the given entity class with the given identifier
		/// </summary>
		/// <param name="theType">A persistent class</param>
		/// <param name="id">A valid identifier of an existing persistent instance of the class</param>
		/// <returns>The persistent instance</returns>
		object Load(System.Type theType, object id);

		/// <summary>
		/// Read the persistent state associated with the given identifier into the given transient 
		/// instance.
		/// </summary>
		/// <param name="obj">An "empty" instance of the persistent class</param>
		/// <param name="id">A valid identifier of an existing persistent instance of the class</param>
		void Load(object obj, object id);

		/// <summary>
		/// Persist the given transient instance, first assigning a generated identifier.
		/// </summary>
		/// <remarks>
		/// Save will use the current value of the identifier property if the <c>Assigned</c>
		/// generator is used.
		/// </remarks>
		/// <param name="obj">A transient instance of a persistent class</param>
		/// <returns>The generated identifier</returns>
		object Save(object obj);

		/// <summary>
		/// Persist the given transient instance, using the given identifier.
		/// </summary>
		/// <param name="obj">A transient instance of a persistent class</param>
		/// <param name="id">An unused valid identifier</param>
		void Save(object obj, object id);

		/// <summary>
		/// Either <c>Save()</c> or <c>Update()</c> the given instance, depending upon the value of
		/// its identifier property.
		/// </summary>
		/// <remarks>
		/// By default the instance is always saved. This behaviour may be adjusted by specifying
		/// an <c>unsaved-value</c> attribute of the identifier property mapping
		/// </remarks>
		/// <param name="obj">A transient instance containing new or updated state</param>
		void SaveOrUpdate(object obj);

		/// <summary>
		/// Update the persistent instance with the identifier of the given transient instance.
		/// </summary>
		/// <remarks>
		/// If there is a persistent instance with the same identifier, an exception is thrown. If
		/// the given transient instance has a <c>null</c> identifier, an exception will be thrown.
		/// </remarks>
		/// <param name="obj">A transient instance containing updated state</param>
		void Update(object obj);

		/// <summary>
		/// Update the persistent state associated with the given identifier.
		/// </summary>
		/// <remarks>
		/// An exception is thrown if there is a persistent instance with the same identifier
		/// in the current session.
		/// </remarks>
		/// <param name="obj">A transient instance containing updated state</param>
		/// <param name="id">Identifier of persistent instance</param>
		void Update(object obj, object id);

		/// <summary>
		/// Remove a persistent instance from the datastore.
		/// </summary>
		/// <remarks>
		/// The argument may be an instance associated wit hthe receiving <c>ISession</c> or a
		/// transient instance with an identifier associated with existing persistent state.
		/// </remarks>
		/// <param name="obj">The instance to be removed</param>
		void Delete(object obj);

		/// <summary>
		/// Execute a query
		/// </summary>
		/// <param name="query">A query expressed in Hibernate's query language</param>
		/// <returns>A distinct list of instances</returns>
		IList Find(string query);

		/// <summary>
		/// Execute a query, binding a value to a "?" parameter in the query string.
		/// </summary>
		/// <param name="query">The query string</param>
		/// <param name="value">A value to be bound to a "?" placeholder</param>
		/// <param name="type">The Hibernate type of the value</param>
		/// <returns>A distinct list of instances</returns>
		IList Find(string query, object value, HibernateType type);

		/// <summary>
		/// Execute a query, binding an array of values to a "?" parameters in the query string.
		/// </summary>
		/// <param name="query">The query string</param>
		/// <param name="values">An array of values to be bound to the "?" placeholders</param>
		/// <param name="types">An array of Hibernate types of the values</param>
		/// <returns>A distinct list of instances</returns>
		IList Find(string query, object[] values, HibernateType[] types);

		/// <summary>
		/// Execute a query and return the results in an interator.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If the query has multiple return values, values will be returned in an array of 
		/// type <c>object[]</c>.
		/// </para>
		/// <para>
		/// Entities returned as results are initialized on demand. The first SQL query returns
		/// identifiers only. So <c>Enumerator()</c> is usually a less efficient way to retrieve
		/// object than <c>Find()</c>.
		/// </para>
		/// </remarks>
		/// <param name="query">The query string</param>
		/// <returns>An enumerator</returns>
		IEnumerator Enumerator(string query);

		/// <summary>
		/// Execute a query and return the results in an interator, 
		/// binding a value to a "?" parameter in the query string.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If the query has multiple return values, values will be returned in an array of 
		/// type <c>object[]</c>.
		/// </para>
		/// <para>
		/// Entities returned as results are initialized on demand. The first SQL query returns
		/// identifiers only. So <c>Enumerator()</c> is usually a less efficient way to retrieve
		/// object than <c>Find()</c>.
		/// </para>
		/// </remarks>
		/// <param name="query">The query string</param>
		/// <param name="value">A value to be written to a "?" placeholder in the query string</param>
		/// <param name="type">The hibernate type of the value</param>
		/// <returns>An enumerator</returns>
		IEnumerator Enumerator(string query, object value, HibernateType type);

		/// <summary>
		/// Execute a query and return the results in an interator, 
		/// binding the values to "?"s parameters in the query string.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If the query has multiple return values, values will be returned in an array of 
		/// type <c>object[]</c>.
		/// </para>
		/// <para>
		/// Entities returned as results are initialized on demand. The first SQL query returns
		/// identifiers only. So <c>Enumerator()</c> is usually a less efficient way to retrieve
		/// object than <c>Find()</c>.
		/// </para>
		/// </remarks>
		/// <param name="query">The query string</param>
		/// <param name="values">A list of values to be written to "?" placeholders in the query</param>
		/// <param name="types">A list of hibernate types of the values</param>
		/// <returns>An enumerator</returns>
		IEnumerator Enumerator(string query, object[] values, HibernateType[] types);

		/// <summary>
		/// Apply a filter to a persistent collection.
		/// </summary>
		/// <remarks>
		/// A filter is a Hibernate query that may refer to <c>this</c>, the collection element.
		/// Filters allow efficient access to very large lazy collections. (Executing the filter
		/// does not initialize the collection.)
		/// </remarks>
		/// <param name="collection">A persistent collection to filter</param>
		/// <param name="filter">A filter query string</param>
		/// <returns>The resulting collection</returns>
		ICollection Filter(object collection, string filter);

		/// <summary>
		/// Apply a filter to a persistent collection, binding the given parameter to a "?" placeholder
		/// </summary>
		/// <remarks>
		/// A filter is a Hibernate query that may refer to <c>this</c>, the collection element.
		/// Filters allow efficient access to very large lazy collections. (Executing the filter
		/// does not initialize the collection.)
		/// </remarks>
		/// <param name="collection">A persistent collection to filter</param>
		/// <param name="filter">A filter query string</param>
		/// <param name="value">A value to be written to a "?" placeholder in the query</param>
		/// <param name="type">The hibernate type of value</param>
		/// <returns>A collection</returns>
		ICollection Filter(object collection, string filter, object value, HibernateType type);

		/// <summary>
		/// Apply a filter to a persistent collection, binding the given parameters to "?" placeholders.
		/// </summary>
		/// <remarks>
		/// A filter is a Hibernate query that may refer to <c>this</c>, the collection element.
		/// Filters allow efficient access to very large lazy collections. (Executing the filter
		/// does not initialize the collection.)
		/// </remarks>
		/// <param name="collection">A persistent collection to filter</param>
		/// <param name="filter">A filter query string</param>
		/// <param name="values">The values to be written to "?" placeholders in the query</param>
		/// <param name="types">The hibernate types of the values</param>
		/// <returns>A collection</returns>
		ICollection Filter(object collection, string filter, object[] values, HibernateType[] types);
		
		/// <summary>
		/// Delete all objects returned by the query.
		/// </summary>
		/// <param name="query">The query string</param>
		/// <returns>Returns the number of objects deleted.</returns>
		int Delete(string query);

		/// <summary>
		/// Delete all objects returned by the query.
		/// </summary>
		/// <param name="query">The query string</param>
		/// <param name="value">A value to be written to a "?" placeholer in the query</param>
		/// <param name="type">The hibernate type of value.</param>
		/// <returns>The number of instances deleted</returns>
		int Delete(string query, object value, HibernateType type);

		/// <summary>
		/// Delete all objects returned by the query.
		/// </summary>
		/// <param name="query">The query string</param>
		/// <param name="values">A list of values to be written to "?" placeholders in the query</param>
		/// <param name="types">A list of Hibernate types of the values</param>
		/// <returns>The number of instances deleted</returns>
		int Delete(string query, object[] values, HibernateType[] types);

		/// <summary>
		/// Obtain the specified lock level upon the given object.
		/// </summary>
		/// <param name="obj">A persistent instance</param>
		/// <param name="lockMode">The lock level</param>
		void Lock(object obj, LockMode lockMode);

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
		/// Determine the current lock mode of the given object
		/// </summary>
		/// <param name="obj">A persistent instance</param>
		/// <returns>The current lock mode</returns>
		LockMode GetCurrentLockMode(object obj);

		/// <summary>
		/// Begin a unit of work and return the associated <c>ITransaction</c> object.
		/// </summary>
		/// <remarks>
		/// If a new underlying transaction is required, begin the transaction. Otherwise
		/// continue the new work in the context of the existing underlying transaction.
		/// The class of the returned <c>Trasnsaction</c> object is determined by the property
		/// <c>hibernate.transaction_factory</c>
		/// </remarks>
		/// <returns>A transaction instance</returns>
		ITransaction BeginTransaction();

		/// <summary>
		/// Create a new instance of <c>Query</c> for the given query string
		/// </summary>
		/// <param name="queryString">A hibernate query string</param>
		/// <returns>The query</returns>
		IQuery CreateQuery(string queryString);

		/// <summary>
		/// Create a new instance of <c>Query</c> for the given collection and filter string
		/// </summary>
		/// <param name="collection">A persistent collection</param>
		/// <param name="queryString">A hibernate query</param>
		/// <returns>A query</returns>
		IQuery CreateFilter(object collection, string queryString);

		/// <summary>
		/// Obtain an instance of <c>Query</c> for a named query string defined in the
		/// mapping file.
		/// </summary>
		/// <param name="queryName">The name of a query defined externally</param>
		/// <returns>A queru</returns>
		IQuery GetNamedQuery(string queryName);
	}
}
