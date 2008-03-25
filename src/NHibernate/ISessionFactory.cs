using System;
using System.Collections;
using System.Data;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Engine;
using NHibernate.Metadata;
using NHibernate.Stat;
using System.Collections.Generic;

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
		ISession OpenSession(IDbConnection conn);

		/// <summary>
		/// Create database connection and open a <c>ISession</c> on it, specifying an interceptor
		/// </summary>
		/// <param name="interceptor">A session-scoped interceptor</param>
		/// <returns>A session</returns>
		ISession OpenSession(IInterceptor interceptor);

		/// <summary>
		/// Open a <c>ISession</c> on the given connection, specifying an interceptor
		/// </summary>
		/// <param name="conn">A connection provided by the application</param>
		/// <param name="interceptor">A session-scoped interceptor</param>
		/// <returns>A session</returns>
		/// <remarks>
		/// Note that the second-level cache will be disabled if you
		/// supply a ADO.NET connection. NHibernate will not be able to track
		/// any statements you might have executed in the same transaction.
		/// Consider implementing your own <see cref="IConnectionProvider" />.
		/// </remarks>
		ISession OpenSession(IDbConnection conn, IInterceptor interceptor);

		/// <summary>
		/// Create a database connection and open a <c>ISession</c> on it
		/// </summary>
		/// <returns></returns>
		ISession OpenSession();

		/// <summary>
		/// Create a new databinder.
		/// </summary>
		/// <returns></returns>
		IDatabinder OpenDatabinder();

		/// <summary>
		/// Get the <c>ClassMetadata</c> associated with the given entity class
		/// </summary>
		/// <param name="persistentType"></param>
		/// <returns></returns>
		IClassMetadata GetClassMetadata(System.Type persistentType);

		/// <summary>
		/// Get the <c>CollectionMetadata</c> associated with the named collection role
		/// </summary>
		/// <param name="roleName"></param>
		/// <returns></returns>
		ICollectionMetadata GetCollectionMetadata(string roleName);

		/// <summary>
		/// Get all <c>ClassMetadata</c> as a <c>IDictionary</c> from <c>Type</c>
		/// to metadata object
		/// </summary>
		/// <returns></returns>
		IDictionary GetAllClassMetadata();

		/// <summary>
		/// Get all <c>CollectionMetadata</c> as a <c>IDictionary</c> from role name
		/// to metadata object
		/// </summary>
		/// <returns></returns>
		IDictionary GetAllCollectionMetadata();

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

		/// <summary>
		/// Get the <see cref="IConnectionProvider" /> used.
		/// </summary>
		IConnectionProvider ConnectionProvider { get; }

		/// <summary>
		/// Get the SQL <c>Dialect</c>
		/// </summary>
		Dialect.Dialect Dialect { get; }

		/// <summary>
		/// Obtain a set of the names of all filters defined on this SessionFactory.
		/// </summary>
		/// <return>The set of filter names.</return>
		ICollection<string> DefinedFilterNames { get; }

		/// <summary>
		/// Obtain the definition of a filter by name.
		/// </summary>
		/// <param name="filterName">The name of the filter for which to obtain the definition.</param>
		/// <return>The filter definition.</return>
		FilterDefinition GetFilterDefinition(string filterName);

		Settings Settings { get; }

		/// <summary>
		/// This collections allows external libraries
		/// to add their own configuration to the NHibernate session factory.
		/// This is needed in such cases where the library is tightly coupled to NHibernate, such
		/// as the case of NHibernate Search
		/// </summary>
		IDictionary Items { get; }

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

		/// <summary> Get a new stateless session.</summary>
		IStatelessSession OpenStatelessSession();

		/// <summary> Get a new stateless session for the given ADO.NET connection.</summary>
		IStatelessSession OpenStatelessSession(IDbConnection connection);

		/// <summary> Get the statistics for this session factory</summary>
		IStatistics Statistics { get;}
	}
}
