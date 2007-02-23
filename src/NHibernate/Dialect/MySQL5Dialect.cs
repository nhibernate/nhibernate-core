using NHibernate.SqlCommand;

namespace NHibernate.Dialect
{
	internal class MySQL5Dialect : MySQLDialect
	{
		public override bool SupportsVariableLimit
		{
			get { return false; }
		}

		public override SqlString GetLimitString(SqlString querySqlString, int offset, int limit)
		{
			SqlStringBuilder pagingBuilder = new SqlStringBuilder();

			pagingBuilder.Add(querySqlString);
			pagingBuilder.Add(" limit ");
			if (offset > 0)
			{
				pagingBuilder.Add(offset.ToString());
				pagingBuilder.Add(", ");
			}

			pagingBuilder.Add(limit.ToString());


			return pagingBuilder.ToSqlString();
		}
	}
}