using System;
using System.Data;
using System.Collections;
using NHibernate.Metadata;

namespace NHibernate {
	
	/// <summary>
	/// Creates <c>ISession</c>s.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Usually an application has a single <c>SessionFactory</c>. Threads servicing client requests
	/// obtain <c>ISession</c>s from the factory. Implementors must be threadsafe.
	/// </para>
	/// <para>
	/// The <c>ISessionFactory</c> is controlled by the following properties. Properties may either
	/// be <c>System</c> properties, properties defined in a resource named <c>/hibernate.properties</c>
	/// in the path.
	/// </para>
	/// </remarks>
	public interface ISessionFactory {
		
		/// <summary>
		/// Open a <c>ISession</c> on the given connection
		/// </summary>
		/// <param name="conn">A connection provided by the application</param>
		/// <returns>A session</returns>
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
		ClassMetadata GetClassMetadata(System.Type persistentType);

		/// <summary>
		/// Get the <c>CollectionMetadata</c> associated with the named collection role
		/// </summary>
		/// <param name="roleName"></param>
		/// <returns></returns>
		CollectionMetadata GetCollectionMetadata(string roleName);

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
	}
}
