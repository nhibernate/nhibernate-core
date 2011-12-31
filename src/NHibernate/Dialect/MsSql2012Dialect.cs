using System.Data;
using NHibernate.Cfg;
using NHibernate.Dialect.Function;
using NHibernate.Driver;
using NHibernate.SqlCommand;

namespace NHibernate.Dialect
{
	public class MsSql2012Dialect : MsSql2008Dialect
	{
        public override SqlString GetLimitString(SqlString queryString, SqlString offset, SqlString limit)
        {
            var result = new SqlStringBuilder(queryString);

            int orderIndex = queryString.LastIndexOfCaseInsensitive(" order by ");

            //don't use the order index if it is contained within a larger statement(assuming
            //a statement with non matching parenthesis is part of a larger block)
            if (orderIndex < 0 || !HasMatchingParens(queryString.Substring(orderIndex).ToString()))
            {
                // Use order by first column if no explicit ordering is provided
                result.Add( " ORDER BY ")
                    .Add("1");
            }

            result.Add(" OFFSET ")
                .Add(offset ?? new SqlString("0"))
                .Add(" ROWS");

            if (limit != null) 
            {
                result.Add(" FETCH FIRST ").Add(limit).Add(" ROWS ONLY");
            }

            return result.ToSqlString();
        }
    }
}