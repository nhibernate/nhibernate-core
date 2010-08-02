using NHibernate.SqlCommand;

namespace NHibernate.Dialect
{
    public class MsSqlCe40Dialect : MsSqlCeDialect
    {
        public override SqlString GetLimitString(SqlString querySqlString, int offset, int limit)
        {
            if (querySqlString.IndexOfCaseInsensitive(" ORDER BY ") < 0)
                querySqlString = querySqlString.Append(" ORDER BY GETDATE()");
            return querySqlString.Append(string.Format(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", offset, limit));
        }

        public override bool SupportsLimit
        {
            get
            {
                return true;
            }
        }

        public override bool SupportsLimitOffset
        {
            get
            {
                return true;
            }
        }
    }
}
