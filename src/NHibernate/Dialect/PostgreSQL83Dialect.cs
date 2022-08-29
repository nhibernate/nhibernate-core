using System.Data;

namespace NHibernate.Dialect
{
	/// <summary>
	/// An SQL dialect for PostgreSQL 8.3 and above.
	/// </summary>
	/// <remarks>
	/// PostgreSQL 8.3 supports xml type
	/// </remarks>
	public class PostgreSQL83Dialect : PostgreSQL82Dialect
	{
		public PostgreSQL83Dialect()
		{
			//https://www.postgresql.org/docs/8.3/static/datatype-xml.html
			RegisterColumnType(DbType.Xml, "xml");
		}
	}
}
