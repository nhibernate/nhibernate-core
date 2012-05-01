using NHibernate.SqlCommand;
using NHibernate.SqlCommand.Parser;

namespace NHibernate.Dialect
{
	public class MsSql2012Dialect : MsSql2008Dialect
	{
		public override SqlString GetLimitString(SqlString querySqlString, SqlString offset, SqlString limit)
		{
			var tokenEnum = new SqlTokenizer(querySqlString).GetEnumerator();
			if (!tokenEnum.TryParseUntilFirstMsSqlSelectColumn()) return null;

			var result = new SqlStringBuilder(querySqlString);
			if (!tokenEnum.TryParseUntil("order"))
			{
				result.Add(" ORDER BY CURRENT_TIMESTAMP");
			}

			result.Add(" OFFSET ");
			if (offset != null)
			{
				result.Add(offset).Add(" ROWS");
			}
			else
			{
				result.Add("0 ROWS");
			}

            if (limit != null) 
            {
                result.Add(" FETCH FIRST ").Add(limit).Add(" ROWS ONLY");
            }

			return result.ToSqlString();
		}
	}
}