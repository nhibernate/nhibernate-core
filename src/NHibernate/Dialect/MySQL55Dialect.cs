
using System.Data;
using NHibernate.Dialect.Function;

namespace NHibernate.Dialect
{
	public class MySQL55Dialect : MySQL5Dialect
	{
		public MySQL55Dialect()
		{
			RegisterColumnType(DbType.Guid, "CHAR(36)");
		}
	}
}
