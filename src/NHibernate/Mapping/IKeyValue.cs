using System;

namespace NHibernate.Mapping
{
	/// <summary>
	/// Represents an identifying key of a table: the value for primary key
	/// of an entity, or a foreign key of a collection or join table or
	/// joined subclass table.
	/// </summary>
	/// <remarks>
	/// Author: Gavin King
	/// </remarks>
	public interface IKeyValue : IValue
	{
		// TODO H3:
		//void CreateForeignKeyOfEntity( string entityName );

		// TODO H3:
		//bool IsCascadeDeleteEnabled { get; }

		// TODO H3:
		//bool IsIdentityColumn(Dialect.Dialect dialect);

		string NullValue { get; }

		// TODO H3:
		//bool IsUpdateable { get; }

		// TODO H3:
		//IIdentifierGenerator CreateIdentifierGenerator(
		//	Dialect.Dialect dialect, 
		//	string defaultCatalog, 
		//	string defaultSchema, 
		//	RootClass rootClass );
	}
}
