using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Multi;
using NHibernate.Util;

namespace NHibernate
{
	// 6.0 TODO: Convert most of these extensions to interface methods
	public static partial class StatelessSessionExtensions
	{
		/// <summary>
		/// Creates a <see cref="IQueryBatch"/> for the session.
		/// </summary>
		/// <param name="session">The session</param>
		/// <returns>A query batch.</returns>
		public static IQueryBatch CreateQueryBatch(this IStatelessSession session)
		{
			return ReflectHelper.CastOrThrow<AbstractSessionImpl>(session, "query batch").CreateQueryBatch();
		}

		// 6.0 TODO: consider if it should be added as a property on IStatelessSession then obsolete this, or if it should stay here as an extension method.
		/// <summary>
		/// Get the current transaction if any is ongoing, else <see langword="null" />.
		/// </summary>
		/// <param name="session">The session.</param>
		/// <returns>The current transaction or <see langword="null" />..</returns>
		public static ITransaction GetCurrentTransaction(this IStatelessSession session)
			=> session.GetSessionImplementation().ConnectionManager.CurrentTransaction;

		//NOTE: Keep it as extension
		/// <summary>
		/// Return the persistent instance of the given entity name with the given identifier, or null
		/// if there is no such persistent instance. (If the instance, or a proxy for the instance, is
		/// already associated with the session, return that instance or proxy.)
		/// </summary>
		/// <typeparam name="T">The entity class.</typeparam>
		/// <param name="session">The session.</param>
		/// <param name="entityName">The entity name.</param>
		/// <param name="id">The entity identifier.</param>
		/// <param name="lockMode">The lock mode to use for getting the entity.</param>
		/// <returns>A persistent instance, or <see langword="null" />.</returns>
		public static T Get<T>(this IStatelessSession session, string entityName, object id, LockMode lockMode)
		{
			return (T) session.Get(entityName, id, lockMode);
		}

		//NOTE: Keep it as extension
		/// <summary>
		/// Return the persistent instance of the given entity name with the given identifier, or null
		/// if there is no such persistent instance. (If the instance, or a proxy for the instance, is
		/// already associated with the session, return that instance or proxy.)
		/// </summary>
		/// <typeparam name="T">The entity class.</typeparam>
		/// <param name="session">The session.</param>
		/// <param name="entityName">The entity name.</param>
		/// <param name="id">The entity identifier.</param>
		/// <returns>A persistent instance, or <see langword="null" />.</returns>
		public static T Get<T>(this IStatelessSession session, string entityName, object id)
		{
			return (T) session.Get(entityName, id);
		}

		/// <summary>
		/// Flush the batcher. When batching is enabled, a stateless session is no more fully stateless. It may retain
		/// in its batcher some state waiting to be flushed to the database.
		/// </summary>
		/// <param name="session">The session.</param>
		public static void FlushBatcher(this IStatelessSession session)
		{
			session.GetSessionImplementation().Flush();
		}

		/// <summary>
		/// Cancel execution of the current query.
		/// </summary>
		/// <remarks>
		/// May be called from one thread to stop execution of a query in another thread.
		/// Use with care!
		/// </remarks>
		public static void CancelQuery(this IStatelessSession session)
		{
			var implementation = session.GetSessionImplementation();
			using (implementation.BeginProcess())
			{
				implementation.Batcher.CancelLastQuery();
			}
		}
	}

	/// <summary>
	/// A command-oriented API for performing bulk operations against a database.
	/// </summary>
	/// <remarks>
	/// A stateless session does not implement a first-level cache nor
	/// interact with any second-level cache, nor does it implement
	/// transactional write-behind or automatic dirty checking, nor do
	/// operations cascade to associated instances. Collections are
	/// ignored by a stateless session. Operations performed via a
	/// stateless session bypass NHibernate's event model and
	/// interceptors. Stateless sessions are vulnerable to data
	/// aliasing effects, due to the lack of a first-level cache.
	/// <para/>
	/// For certain kinds of transactions, a stateless session may
	/// perform slightly faster than a stateful session.
	/// </remarks>
	public partial interface IStatelessSession : IDisposable
	{
		/// <summary>
		/// Returns the current ADO.NET connection associated with this instance.
		/// </summary>
		/// <remarks>
		/// If the session is using aggressive connection release (as in a
		/// CMT environment), it is the application's responsibility to
		/// close the connection returned by this call. Otherwise, the
		/// application should not close the connection.
		/// </remarks>
		DbConnection Connection { get; }
		
		/// <summary>Get the current NHibernate transaction.</summary>
		// Since v5.3
		[Obsolete("Use GetCurrentTransaction extension method instead, and check for null.")]
		ITransaction Transaction { get; }
		
		/// <summary>
		/// Is the <c>IStatelessSession</c> still open?
		/// </summary>
		bool IsOpen { get; }

		/// <summary>
		/// Is the session connected?
		/// </summary>
		/// <value>
		/// <see langword="true" /> if the session is connected.
		/// </value>
		/// <remarks>
		/// A session is considered connected if there is a <see cref="DbConnection"/> (regardless
		/// of its state) or if the field <c>connect</c> is true. Meaning that it will connect
		/// at the next operation that requires a connection.
		/// </remarks>
		bool IsConnected { get; }

		/// <summary>
		/// Gets the stateless session implementation.
		/// </summary>
		/// <remarks>
		/// This method is provided in order to get the <b>NHibernate</b> implementation of the session from wrapper implementations.
		/// Implementors of the <seealso cref="IStatelessSession "/> interface should return the NHibernate implementation of this method.
		/// </remarks>
		/// <returns>
		/// An NHibernate implementation of the <see cref="ISessionImplementor "/> interface
		/// </returns>
		ISessionImplementor GetSessionImplementation();

		/// <summary>Close the stateless session and release the ADO.NET connection.</summary>
		void Close();

		/// <summary>Insert an entity.</summary>
		/// <param name="entity">A new transient instance</param>
		/// <returns>The identifier of the instance</returns>
		object Insert(object entity);

		/// <summary>Insert a row.</summary>
		/// <param name="entityName">The name of the entity to be inserted</param>
		/// <param name="entity">A new transient instance</param>
		/// <returns>The identifier of the instance</returns>
		object Insert(string entityName, object entity);

		/// <summary>Update an entity.</summary>
		/// <param name="entity">A detached entity instance</param>
		void Update(object entity);

		/// <summary>Update an entity.</summary>
		/// <param name="entityName">The name of the entity to be updated</param>
		/// <param name="entity">A detached entity instance</param>
		void Update(string entityName, object entity);

		/// <summary>Delete an entity.</summary>
		/// <param name="entity">A detached entity instance</param>
		void Delete(object entity);

		/// <summary>Delete an entity.</summary>
		/// <param name="entityName">The name of the entity to be deleted</param>
		/// <param name="entity">A detached entity instance</param>
		void Delete(string entityName, object entity);

		/// <summary>Retrieve a entity.</summary>
		/// <returns>A detached entity instance</returns>
		object Get(string entityName, object id);

		/// <summary>
		/// Retrieve an entity.
		/// </summary>
		/// <returns>A detached entity instance</returns>
		T Get<T>(object id);

		/// <summary>
		/// Retrieve an entity, obtaining the specified lock mode.
		/// </summary>
		/// <returns>A detached entity instance</returns>
		object Get(string entityName, object id, LockMode lockMode);

		/// <summary>
		/// Retrieve an entity, obtaining the specified lock mode.
		/// </summary>
		/// <returns>A detached entity instance</returns>
		T Get<T>(object id, LockMode lockMode);

		/// <summary>
		/// Refresh the entity instance state from the database.
		/// </summary>
		/// <param name="entity">The entity to be refreshed.</param>
		void Refresh(object entity);

		/// <summary>
		/// Refresh the entity instance state from the database.
		/// </summary>
		/// <param name="entityName">The name of the entity to be refreshed.</param>
		/// <param name="entity">The entity to be refreshed.</param>
		void Refresh(string entityName, object entity);

		/// <summary>
		/// Refresh the entity instance state from the database.
		/// </summary>
		/// <param name="entity">The entity to be refreshed.</param>
		/// <param name="lockMode">The LockMode to be applied.</param>
		void Refresh(object entity, LockMode lockMode);

		/// <summary>
		/// Refresh the entity instance state from the database.
		/// </summary>
		/// <param name="entityName">The name of the entity to be refreshed.</param>
		/// <param name="entity">The entity to be refreshed.</param>
		/// <param name="lockMode">The LockMode to be applied.</param>
		void Refresh(string entityName, object entity, LockMode lockMode);

		/// <summary>
		/// Create a new instance of <tt>Query</tt> for the given HQL query string.
		/// </summary>
		/// <remarks>Entities returned by the query are detached.</remarks>
		IQuery CreateQuery(string queryString);

		/// <summary>
		/// Obtain an instance of <see cref="IQuery "/> for a named query string defined in
		/// the mapping file.
		/// </summary>
		/// <remarks>
 		/// The query can be either in <c>HQL</c> or <c>SQL</c> format.
		/// Entities returned by the query are detached.
		/// </remarks>
		IQuery GetNamedQuery(string queryName);

		/// <summary>
		/// Create a new <see cref="ICriteria "/> instance, for the given entity class,
		/// or a superclass of an entity class.
		/// </summary>
		/// <typeparam name="T">A class, which is persistent, or has persistent subclasses</typeparam>
		/// <returns>The <see cref="ICriteria "/>.</returns>
		/// <remarks>Entities returned by the query are detached.</remarks>
		ICriteria CreateCriteria<T>() where T: class;

		/// <summary>
		/// Create a new <see cref="ICriteria "/> instance, for the given entity class,
		/// or a superclass of an entity class, with the given alias.
		/// </summary>
		/// <typeparam name="T">A class, which is persistent, or has persistent subclasses</typeparam>
		/// <param name="alias">The alias of the entity</param>
		/// <returns>The <see cref="ICriteria "/>.</returns>
		/// <remarks>Entities returned by the query are detached.</remarks>
		ICriteria CreateCriteria<T>(string alias) where T : class;

		/// <summary>
		/// Create a new <see cref="ICriteria "/> instance, for the given entity class,
		/// or a superclass of an entity class.
		/// </summary>
		/// <param name="entityType">A class, which is persistent, or has persistent subclasses</param>
		/// <returns>The <see cref="ICriteria "/>.</returns>
		/// <remarks>Entities returned by the query are detached.</remarks>
		ICriteria CreateCriteria(System.Type entityType);

		/// <summary>
		/// Create a new <see cref="ICriteria "/> instance, for the given entity class,
		/// or a superclass of an entity class, with the given alias.
		/// </summary>
		/// <param name="entityType">A class, which is persistent, or has persistent subclasses</param>
		/// <param name="alias">The alias of the entity</param>
		/// <returns>The <see cref="ICriteria "/>.</returns>
		/// <remarks>Entities returned by the query are detached.</remarks>
		ICriteria CreateCriteria(System.Type entityType, string alias);

		/// <summary>
		/// Create a new <see cref="ICriteria "/> instance, for the given entity name.
		/// </summary>
		/// <param name="entityName">The entity name.</param>
		/// <returns>The <see cref="ICriteria "/>.</returns>
		/// <remarks>Entities returned by the query are detached.</remarks>
		ICriteria CreateCriteria(string entityName);

		/// <summary>
		/// Create a new <see cref="ICriteria "/> instance, for the given entity name,
		/// with the given alias.
		/// </summary>
		/// <param name="entityName">The entity name.</param>
		/// <param name="alias">The alias of the entity</param>
		/// <returns>The <see cref="ICriteria "/>.</returns>
		/// <remarks>Entities returned by the query are detached.</remarks>
		ICriteria CreateCriteria(string entityName, string alias);

		/// <summary>
		/// Creates a new <c>IQueryOver&lt;T&gt;</c> for the entity class.
		/// </summary>
		/// <typeparam name="T">The entity class</typeparam>
		/// <returns>An ICriteria&lt;T&gt; object</returns>
		IQueryOver<T,T> QueryOver<T>() where T : class;

		/// <summary>
		/// Creates a new <c>IQueryOver&lt;T&gt;</c> for the entity class.
		/// </summary>
		/// <typeparam name="T">The entity class</typeparam>
		/// <returns>An ICriteria&lt;T&gt; object</returns>
		IQueryOver<T,T> QueryOver<T>(Expression<Func<T>> alias) where T : class;

		/// <summary>
		/// Create a new instance of <see cref="ISQLQuery "/> for the given SQL query string.
		/// Entities returned by the query are detached.
		/// </summary>
		/// <param name="queryString">A SQL query</param>
		/// <returns>The <see cref="ISQLQuery "/></returns>
		ISQLQuery CreateSQLQuery(string queryString);

		/// <summary>
		/// Begin a NHibernate transaction
		/// </summary>
		/// <returns>A NHibernate transaction</returns>
		ITransaction BeginTransaction();

		/// <summary>
		/// Begin a NHibernate transaction with the specified isolation level
		/// </summary>
		/// <param name="isolationLevel">The isolation level</param>
		/// <returns>A NHibernate transaction</returns>
		ITransaction BeginTransaction(IsolationLevel isolationLevel);

		/// <summary>
		/// Join the <see cref="System.Transactions.Transaction.Current"/> system transaction.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Sessions auto-join current transaction by default on their first usage within a scope.
		/// This can be disabled with <see cref="IStatelessSessionBuilder.AutoJoinTransaction(bool)"/> from
		/// a session builder obtained with <see cref="ISessionFactory.WithStatelessOptions()"/>.
		/// </para>
		/// <para>
		/// This method allows to explicitly join the current transaction. It does nothing if it is already
		/// joined.
		/// </para>
		/// </remarks>
		/// <exception cref="HibernateException">Thrown if there is no current transaction.</exception>
		void JoinTransaction();

		/// <summary>
		/// Sets the batch size of the session
		/// </summary>
		/// <param name="batchSize">The batch size.</param>
		/// <returns>The same instance of the session for methods chain.</returns>
		IStatelessSession SetBatchSize(int batchSize);

		/// <summary>
		/// Creates a new Linq <see cref="IQueryable{T}"/> for the entity class.
		/// </summary>
		/// <typeparam name="T">The entity class</typeparam>
		/// <returns>An <see cref="IQueryable{T}"/> instance</returns>
		IQueryable<T> Query<T>();

		/// <summary>
		/// Creates a new Linq <see cref="IQueryable{T}"/> for the entity class and with given entity name.
		/// </summary>
		/// <typeparam name="T">The type of entity to query.</typeparam>
		/// <param name="entityName">The entity name.</param>
		/// <returns>An <see cref="IQueryable{T}"/> instance</returns>
		IQueryable<T> Query<T>(string entityName);
	}
}
