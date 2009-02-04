using System.Data;

namespace NHibernate.Dialect
{
    public class MsSql2008Dialect : MsSql2005Dialect
    {
        public MsSql2008Dialect()
        {
            RegisterColumnType(DbType.DateTime2, "DATETIME2");
            RegisterColumnType(DbType.DateTimeOffset, "DATETIMEOFFSET");
			RegisterColumnType(DbType.Date, "DATE");
			//RegisterColumnType(DbType.Time, "TIME");
        }
    }
}