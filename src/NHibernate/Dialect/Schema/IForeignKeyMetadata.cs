namespace NHibernate.Dialect.Schema
{
	public interface IForeignKeyMetadata
	{
		string Name { get; }

		void AddColumn(IColumnMetadata column);

		IColumnMetadata[] Columns { get; }
	}
}