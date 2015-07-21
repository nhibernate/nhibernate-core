namespace NHibernate.Dialect.Schema
{
	public interface IColumnMetadata
	{
		string Name { get; }

		string TypeName { get; }

		int ColumnSize { get; }

		int NumericalPrecision { get; }

		string Nullable { get; }
	}
}