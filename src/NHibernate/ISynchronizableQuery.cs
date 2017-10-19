using System.Collections.Generic;

namespace NHibernate
{
	public interface ISynchronizableQuery<out T> where T : ISynchronizableQuery<T>
	{
		/// <summary>
		/// Adds a query space for auto-flush synchronization and second level cache invalidation.
		/// </summary>
		/// <param name="querySpace"></param>
		/// <returns></returns>
		T AddSynchronizedQuerySpace(string querySpace);

		/// <summary>
		/// Adds an entity name for auto-flush synchronization and second level cache invalidation.
		/// </summary>
		/// <param name="entityName"></param>
		/// <returns></returns>
		T AddSynchronizedEntityName(string entityName);

		/// <summary>
		/// Adds an entity type for auto-flush synchronization and second level cache invalidation.
		/// </summary>
		/// <param name="entityType"></param>
		/// <returns></returns>
		T AddSynchronizedEntityClass(System.Type entityType);

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		IReadOnlyCollection<string> GetSynchronizedQuerySpaces();
	}
}
