using NHibernate.SqlCommand;

namespace NHibernate.Dialect
{
	public class MySQL5Dialect : MySQLDialect
	{
		//Reference 5.x
		//Numeric:
		//http://dev.mysql.com/doc/refman/5.0/en/numeric-type-overview.html
		//Date and time:
		//http://dev.mysql.com/doc/refman/5.0/en/date-and-time-type-overview.html
		//String:
		//http://dev.mysql.com/doc/refman/5.0/en/string-type-overview.html
		//default:
		//http://dev.mysql.com/doc/refman/5.0/en/data-type-defaults.html

		public override bool SupportsVariableLimit
		{
			get
			{
				//note: why false?
				return false;
			}
		}

		public override bool SupportsSubSelects
		{
			get
			{
				//subquery in mysql? yes! From 4.1!
				//http://dev.mysql.com/doc/refman/5.1/en/subqueries.html
				return true;
			}
		}

		public override SqlString GetLimitString(SqlString querySqlString, int offset, int limit)
		{
			var pagingBuilder = new SqlStringBuilder();

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

		public override string SelectGUIDString
		{
			get
			{
				return "select uuid()";
			}
		}
	}
}