using NHibernate.Id;

namespace NHibernate.Mapping
{
	/// <summary>
	/// Represents an identifying key of a table: the value for primary key
	/// of an entity, or a foreign key of a collection or join table or
	/// joined subclass table.
	/// </summary>
	public interface IKeyValue : IValue
	{
		void CreateForeignKeyOfEntity(string entityName);

		bool IsCascadeDeleteEnabled { get; }

		bool IsIdentityColumn(Dialect.Dialect dialect);

		string NullValue { get; }

		bool IsUpdateable { get; }

		IIdentifierGenerator CreateIdentifierGenerator(Dialect.Dialect dialect, string defaultCatalog, string defaultSchema, RootClass rootClass);
	}
}
