using System.Data;

namespace NHibernate.Dialect
{
	public class MySQL8Dialect : MySQL57Dialect
	{
		public MySQL8Dialect() => RegisterColumnType(DbType.Boolean, "BOOLEAN");
	}
}
