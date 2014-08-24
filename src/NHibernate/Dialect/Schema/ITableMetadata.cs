namespace NHibernate.Dialect.Schema
{
	public interface ITableMetadata
	{
		string Name { get; }

		string Catalog { get; }

		string Schema { get; }

		IColumnMetadata GetColumnMetadata(string columnName);
		IForeignKeyMetadata GetForeignKeyMetadata(string keyName);
		IIndexMetadata GetIndexMetadata(string indexName);
		bool NeedPhysicalConstraintCreation(string fkName);
	}
}