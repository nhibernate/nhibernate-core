using NHibernate.SqlCommand;

namespace NHibernate.Dialect
{
	public class MsSqlCe40Dialect : MsSqlCeDialect
	{
		public override bool SupportsLimitOffset
		{
			get { return true; }
		}

		public override bool SupportsVariableLimit
		{
			get { return true; }
		}

		public override SqlString GetLimitString(SqlString queryString, SqlString offset, SqlString limit)
		{
			//TODO: Share code with MsSql2012Dialect.GetLimitString
			var builder = new SqlStringBuilder(queryString);
			if (queryString.IndexOfCaseInsensitive(" ORDER BY ") < 0)
				builder.Add(" ORDER BY GETDATE()");

			builder.Add(" OFFSET ");
			if (offset == null)
				builder.Add("0");
			else
				builder.Add(offset);
			builder.Add(" ROWS");

			if (limit != null)
			{
				builder.Add(" FETCH NEXT ");
				builder.Add(limit);
				builder.Add(" ROWS ONLY");
			}

			return builder.ToSqlString();
		}
	}
}