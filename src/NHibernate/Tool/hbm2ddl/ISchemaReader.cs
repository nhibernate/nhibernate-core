namespace NHibernate.Tool.hbm2ddl
{
	using System.Data;

	public interface ISchemaReader
	{
		DataTable GetTables(string schema, string name, string[] types);
		DataTable GetColumns(string schema, string name);
		DataTable GetIndexInfo(string schema, string name);
		DataTable GetForeignKeys(string schema, string name);
		DataTable GetIndexColumns(string schema, string name, string constraint);
	}
}