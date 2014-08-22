using System;
using System.Data;
using System.Linq.Expressions;
using NHibernate.Engine;

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
	/// stateless session bypass NHibernate's event model and
	/// interceptors. Stateless sessions are vulnerable to data
	/// aliasing effects, due to the lack of a first-level cache.
	/// <para/>
	/// For certain kinds of transactions, a stateless session may
	/// perform slightly faster than a stateful session.
	/// </remarks>
	public interface IStatelessSession : ISharedSessionContract
	{
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

		/// <summary>
		/// Retrieve an entity, obtaining the specified lock mode.
		/// </summary>
		/// <returns>A detached entity instance</returns>
		object Get(string entityName, object id, LockMode lockMode);

		/// <summary>
		/// Refresh the entity instance state from the database.
		/// </summary>
		/// <param name="entityName">The name of the entity to be refreshed.</param>
		/// <param name="entity">The entity to be refreshed.</param>
		void Refresh(string entityName, object entity);

		/// <summary>
		/// Refresh the entity instance state from the database.
		/// </summary>
		/// <param name="entityName">The name of the entity to be refreshed.</param>
		/// <param name="entity">The entity to be refreshed.</param>
		/// <param name="lockMode">The LockMode to be applied.</param>
		void Refresh(string entityName, object entity, LockMode lockMode);

		/// <summary>
		/// Sets the batch size of the session
		/// </summary>
		/// <param name="batchSize">The batch size.</param>
		/// <returns>The same instance of the session for methods chain.</returns>
		IStatelessSession SetBatchSize(int batchSize);
	}
}