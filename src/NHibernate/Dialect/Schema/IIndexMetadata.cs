namespace NHibernate.Dialect.Schema
{
	public interface IIndexMetadata
	{
		string Name { get; }

		void AddColumn(IColumnMetadata column);

		IColumnMetadata[] Columns { get; }
	}
}