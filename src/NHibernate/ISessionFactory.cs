using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using NHibernate.Connection;
using NHibernate.Engine;
using NHibernate.Metadata;
using NHibernate.Stat;

namespace NHibernate
{
	/// <summary>
	/// Creates <c>ISession</c>s.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Usually an application has a single <c>SessionFactory</c>. Threads servicing client requests
	/// obtain <c>ISession</c>s from the factory. Implementors must be threadsafe.
	/// </para>
	/// <para>
	/// <c>ISessionFactory</c>s are immutable. The behaviour of a <c>SessionFactory</c>
	/// is controlled by properties supplied at configuration time.
	/// These properties are defined on <c>Environment</c>
	/// </para>
	/// </remarks>
	public interface ISessionFactory : IDisposable
	{
		/// <summary>
		/// Open a <c>ISession</c> on the given connection
		/// </summary>
		/// <param name="conn">A connection provided by the application</param>
		/// <returns>A session</returns>
		/// <remarks>
		/// Note that the second-level cache will be disabled if you
		/// supply a ADO.NET connection. NHibernate will not be able to track
		/// any statements you might have executed in the same transaction.
		/// Consider implementing your own <see cref="IConnectionProvider" />.
		/// </remarks>
		ISession OpenSession(DbConnection conn);

		/// <summary>
		/// Create database connection and open a <c>ISession</c> on it, specifying an interceptor
		/// </summary>
		/// <param name="sessionLocalInterceptor">A session-scoped interceptor</param>
		/// <returns>A session</returns>
		ISession OpenSession(IInterceptor sessionLocalInterceptor);

		/// <summary>
		/// Open a <c>ISession</c> on the given connection, specifying an interceptor
		/// </summary>
		/// <param name="conn">A connection provided by the application</param>
		/// <param name="sessionLocalInterceptor">A session-scoped interceptor</param>
		/// <returns>A session</returns>
		/// <remarks>
		/// Note that the second-level cache will be disabled if you
		/// supply a ADO.NET connection. NHibernate will not be able to track
		/// any statements you might have executed in the same transaction.
		/// Consider implementing your own <see cref="IConnectionProvider" />.
		/// </remarks>
		ISession OpenSession(DbConnection conn, IInterceptor sessionLocalInterceptor);

		/// <summary>
		/// Create a database connection and open a <c>ISession</c> on it
		/// </summary>
		/// <returns></returns>
		ISession OpenSession();

		/// <summary>
		/// Get the <see cref="IClassMetadata"/> associated with the given entity class
		/// </summary>
		/// <param name="persistentClass">the given entity type.</param>
		/// <returns>The class metadata or <see langword="null"/> if not found.</returns>
		/// <seealso cref="IClassMetadata"/>
		IClassMetadata GetClassMetadata(System.Type persistentClass);

		/// <summary> Get the <see cref="IClassMetadata"/> associated with the given entity name </summary>
		/// <param name="entityName">the given entity name.</param>
		/// <returns>The class metadata or <see langword="null"/> if not found.</returns>
		/// <seealso cref="IClassMetadata"/>
		IClassMetadata GetClassMetadata(string entityName);

		/// <summary>
		/// Get the <c>CollectionMetadata</c> associated with the named collection role
		/// </summary>
		/// <param name="roleName"></param>
		/// <returns></returns>
		ICollectionMetadata GetCollectionMetadata(string roleName);

		/// <summary> 
		/// Get all <see cref="IClassMetadata"/> as a <see cref="IDictionary"/> from entityname <see langword="string"/>
		/// to metadata object
		/// </summary>
		/// <returns> A dictionary from <see langword="string"/> an entity name to <see cref="IClassMetadata"/> </returns>
		IDictionary<string, IClassMetadata> GetAllClassMetadata();

		/// <summary>
		/// Get all <c>CollectionMetadata</c> as a <c>IDictionary</c> from role name
		/// to metadata object
		/// </summary>
		/// <returns></returns>
		IDictionary<string, ICollectionMetadata> GetAllCollectionMetadata();

		/// <summary>
		/// Destroy this <c>SessionFactory</c> and release all resources 
		/// connection pools, etc). It is the responsibility of the application
		/// to ensure that there are no open <c>Session</c>s before calling
		/// <c>close()</c>. 
		/// </summary>
		void Close();

		/// <summary>
		/// Evict all entries from the process-level cache.  This method occurs outside
		/// of any transaction; it performs an immediate "hard" remove, so does not respect
		/// any transaction isolation semantics of the usage strategy.  Use with care.
		/// </summary>
		/// <param name="persistentClass"></param>
		void Evict(System.Type persistentClass);

		/// <summary>
		/// Evict an entry from the process-level cache.  This method occurs outside
		/// of any transaction; it performs an immediate "hard" remove, so does not respect
		/// any transaction isolation semantics of the usage strategy.  Use with care.
		/// </summary>
		/// <param name="persistentClass"></param>
		/// <param name="id"></param>
		void Evict(System.Type persistentClass, object id);

		/// <summary> 
		/// Evict all entries from the second-level cache. This method occurs outside
		/// of any transaction; it performs an immediate "hard" remove, so does not respect
		/// any transaction isolation semantics of the usage strategy. Use with care.
		/// </summary>
		void EvictEntity(string entityName);

		/// <summary> 
		/// Evict an entry from the second-level  cache. This method occurs outside
		/// of any transaction; it performs an immediate "hard" remove, so does not respect
		/// any transaction isolation semantics of the usage strategy. Use with care.
		/// </summary>
		void EvictEntity(string entityName, object id);

		/// <summary>
		/// Evict all entries from the process-level cache.  This method occurs outside
		/// of any transaction; it performs an immediate "hard" remove, so does not respect
		/// any transaction isolation semantics of the usage strategy.  Use with care.
		/// </summary>
		/// <param name="roleName"></param>
		void EvictCollection(string roleName);

		/// <summary>
		/// Evict an entry from the process-level cache.  This method occurs outside
		/// of any transaction; it performs an immediate "hard" remove, so does not respect
		/// any transaction isolation semantics of the usage strategy.  Use with care.
		/// </summary>
		/// <param name="roleName"></param>
		/// <param name="id"></param>
		void EvictCollection(string roleName, object id);

		/// <summary>
		/// Evict any query result sets cached in the default query cache region.
		/// </summary>
		void EvictQueries();

		/// <summary>
		/// Evict any query result sets cached in the named query cache region.
		/// </summary>
		/// <param name="cacheRegion"></param>
		void EvictQueries(string cacheRegion);

		/// <summary> Get a new stateless session.</summary>
		IStatelessSession OpenStatelessSession();

		/// <summary> Get a new stateless session for the given ADO.NET connection.</summary>
		IStatelessSession OpenStatelessSession(DbConnection connection);

		/// <summary>
		/// Obtain the definition of a filter by name.
		/// </summary>
		/// <param name="filterName">The name of the filter for which to obtain the definition.</param>
		/// <return>The filter definition.</return>
		FilterDefinition GetFilterDefinition(string filterName);

		/// <summary>
		/// Obtains the current session.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The definition of what exactly "current" means is controlled by the <see cref="NHibernate.Context.ICurrentSessionContext" />
		/// implementation configured for use.
		/// </para>
		/// </remarks>
		/// <returns>The current session.</returns>
		/// <exception cref="HibernateException">Indicates an issue locating a suitable current session.</exception>
		ISession GetCurrentSession();

		/// <summary> Get the statistics for this session factory</summary>
		IStatistics Statistics { get;}

		/// <summary> Was this <see cref="ISessionFactory"/> already closed?</summary>
		bool IsClosed { get;}

		/// <summary>
		/// Obtain a set of the names of all filters defined on this SessionFactory.
		/// </summary>
		/// <return>The set of filter names.</return>
		ICollection<string> DefinedFilterNames { get; }
	}
}
