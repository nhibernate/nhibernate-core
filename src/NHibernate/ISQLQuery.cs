using System;
using System.Collections.Generic;
using NHibernate.Type;

namespace NHibernate
{
	// 6.0 TODO: remove after having done ISynchronizableSQLQuery todo
	public static class SQLQueryExtension
	{
		/// <summary>
		/// Adds a query space for auto-flush synchronization and second level cache invalidation.
		/// </summary>
		/// <param name="sqlQuery">The query.</param>
		/// <param name="querySpace">The query space.</param>
		/// <returns>The query.</returns>
		public static ISQLQuery AddSynchronizedQuerySpace(this ISQLQuery sqlQuery, string querySpace)
		{
			if (!(sqlQuery is ISynchronizableSQLQuery ssq))
				throw new NotSupportedException($"The query must implement {nameof(ISynchronizableSQLQuery)}");
			return ssq.AddSynchronizedQuerySpace(querySpace);
		}

		/// <summary>
		/// Adds an entity name for auto-flush synchronization and second level cache invalidation.
		/// </summary>
		/// <param name="sqlQuery">The query.</param>
		/// <param name="entityName">The entity name.</param>
		/// <returns>The query.</returns>
		public static ISQLQuery AddSynchronizedEntityName(this ISQLQuery sqlQuery, string entityName)
		{
			if (!(sqlQuery is ISynchronizableSQLQuery ssq))
				throw new NotSupportedException($"The query must implement {nameof(ISynchronizableSQLQuery)}");
			return ssq.AddSynchronizedEntityName(entityName);
		}

		/// <summary>
		/// Adds an entity type for auto-flush synchronization and second level cache invalidation.
		/// </summary>
		/// <param name="sqlQuery">The query.</param>
		/// <param name="entityType">The entity type.</param>
		/// <returns>The query.</returns>
		public static ISQLQuery AddSynchronizedEntityClass(this ISQLQuery sqlQuery, System.Type entityType)
		{
			if (!(sqlQuery is ISynchronizableSQLQuery ssq))
				throw new NotSupportedException($"The query must implement {nameof(ISynchronizableSQLQuery)}");
			return ssq.AddSynchronizedEntityClass(entityType);
		}

		/// <summary>
		/// Returns the synchronized query spaces added to the query.
		/// </summary>
		/// <param name="sqlQuery">The query.</param>
		/// <returns>The synchronized query spaces.</returns>
		public static IReadOnlyCollection<string> GetSynchronizedQuerySpaces(this ISQLQuery sqlQuery)
		{
			if (!(sqlQuery is ISynchronizableSQLQuery ssq))
				throw new NotSupportedException($"The query must implement {nameof(ISynchronizableSQLQuery)}");
			return ssq.GetSynchronizedQuerySpaces();
		}
	}

	// 6.0 TODO: obsolete ISynchronizableSQLQuery and have ISQLQuery directly extending ISynchronizableQuery<ISQLQuery>
	public interface ISynchronizableSQLQuery : ISQLQuery, ISynchronizableQuery<ISynchronizableSQLQuery>
	{
	}

	public interface ISQLQuery : IQuery
	{
		/// <summary>
		/// Declare a "root" entity, without specifying an alias
		/// </summary>
		ISQLQuery AddEntity(string entityName);

		/// <summary>
		/// Declare a "root" entity
		/// </summary>
		ISQLQuery AddEntity(string alias, string entityName);

		/// <summary>
		/// Declare a "root" entity, specifying a lock mode
		/// </summary>
		ISQLQuery AddEntity(string alias, string entityName, LockMode lockMode);

		/// <summary>
		/// Declare a "root" entity, without specifying an alias
		/// </summary>
		ISQLQuery AddEntity(System.Type entityClass);

		/// <summary>
		/// Declare a "root" entity
		/// </summary>
		ISQLQuery AddEntity(string alias, System.Type entityClass);

		/// <summary>
		/// Declare a "root" entity, specifying a lock mode
		/// </summary>
		ISQLQuery AddEntity(string alias, System.Type entityClass, LockMode lockMode);

		/// <summary>
		/// Declare a "joined" entity
		/// </summary>
		ISQLQuery AddJoin(string alias, string path);

		/// <summary>
		/// Declare a "joined" entity, specifying a lock mode
		/// </summary>
		ISQLQuery AddJoin(string alias, string path, LockMode lockMode);

		/// <summary>
		/// Declare a scalar query result
		/// </summary>
		ISQLQuery AddScalar(string columnAlias, IType type);

		/// <summary>
		/// Use a predefined named ResultSetMapping
		/// </summary>
		ISQLQuery SetResultSetMapping(string name);
	}
}
