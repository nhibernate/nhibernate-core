using System.Collections.Generic;

namespace NHibernate
{
	public interface ISynchronizableQuery<out T> where T : ISynchronizableQuery<T>
	{
		/// <summary>
		/// Adds a query space for auto-flush synchronization and second level cache invalidation.
		/// </summary>
		/// <param name="querySpace">The query space.</param>
		/// <returns>The query.</returns>
		T AddSynchronizedQuerySpace(string querySpace);

		/// <summary>
		/// Adds an entity name for auto-flush synchronization and second level cache invalidation.
		/// </summary>
		/// <param name="entityName">The entity name.</param>
		/// <returns>The query.</returns>
		T AddSynchronizedEntityName(string entityName);

		/// <summary>
		/// Adds an entity type for auto-flush synchronization and second level cache invalidation.
		/// </summary>
		/// <param name="entityType">The entity type.</param>
		/// <returns>The query.</returns>
		T AddSynchronizedEntityClass(System.Type entityType);

		/// <summary>
		/// Returns the synchronized query spaces added to the query.
		/// </summary>
		/// <returns>The synchronized query spaces.</returns>
		IReadOnlyCollection<string> GetSynchronizedQuerySpaces();
	}
}
