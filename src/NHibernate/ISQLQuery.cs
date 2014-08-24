using NHibernate.Type;

namespace NHibernate
{
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
