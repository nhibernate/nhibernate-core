using System.Collections;
using System.Data;
using NHibernate.Metadata;

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
	public interface ISessionFactory
	{
		/// <summary>
		/// Open a <c>ISession</c> on the given connection
		/// </summary>
		/// <param name="conn">A connection provided by the application</param>
		/// <returns>A session</returns>
		ISession OpenSession( IDbConnection conn );

		/// <summary>
		/// Create database connection and open a <c>ISession</c> on it, specifying an interceptor
		/// </summary>
		/// <param name="interceptor">A session-scoped interceptor</param>
		/// <returns>A session</returns>
		ISession OpenSession( IInterceptor interceptor );

		/// <summary>
		/// Open a <c>ISession</c> on the given connection, specifying an interceptor
		/// </summary>
		/// <param name="conn">A connection provided by the application</param>
		/// <param name="interceptor">A session-scoped interceptor</param>
		/// <returns>A session</returns>
		ISession OpenSession( IDbConnection conn, IInterceptor interceptor );

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
		IClassMetadata GetClassMetadata( System.Type persistentType );

		/// <summary>
		/// Get the <c>CollectionMetadata</c> associated with the named collection role
		/// </summary>
		/// <param name="roleName"></param>
		/// <returns></returns>
		ICollectionMetadata GetCollectionMetadata( string roleName );

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
		void Evict( System.Type persistentClass );

		/// <summary>
		/// Evict an entry from the process-level cache.  This method occurs outside
		/// of any transaction; it performs an immediate "hard" remove, so does not respect
		/// any transaction isolation semantics of the usage strategy.  Use with care.
		/// </summary>
		/// <param name="persistentClass"></param>
		/// <param name="id"></param>
		void Evict( System.Type persistentClass, object id );

		/// <summary>
		/// Evict all entries from the process-level cache.  This method occurs outside
		/// of any transaction; it performs an immediate "hard" remove, so does not respect
		/// any transaction isolation semantics of the usage strategy.  Use with care.
		/// </summary>
		/// <param name="roleName"></param>
		void EvictCollection( string roleName );

		/// <summary>
		/// Evict an entry from the process-level cache.  This method occurs outside
		/// of any transaction; it performs an immediate "hard" remove, so does not respect
		/// any transaction isolation semantics of the usage strategy.  Use with care.
		/// </summary>
		/// <param name="roleName"></param>
		/// <param name="id"></param>
		void EvictCollection( string roleName, object id );

		/// <summary>
		/// Evict any query result sets cached in the default query cache region.
		/// </summary>
		void EvictQueries( );

		/// <summary>
		/// Evict any query result sets cached in the named query cache region.
		/// </summary>
		/// <param name="cacheRegion"></param>
		void EvictQueries( string cacheRegion );
	}
}