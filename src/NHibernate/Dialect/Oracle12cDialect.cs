using NHibernate.SqlCommand;

namespace NHibernate.Dialect
{
	/// <summary> 
	/// A dialect specifically for use with Oracle 10g.
	/// </summary>
	/// <remarks>
	/// The main difference between this dialect and <see cref="Oracle12cDialect"/>
	/// is the use of "ANSI join syntax" here...
	/// </remarks>
	public class Oracle12cDialect : Oracle10gDialect
	{
		/// <summary>
		/// Oracle 12c supports a query statement that provides <c>LIMIT</c>
		/// functionality with an offset.
		/// </summary>
		/// <value><c>false</c></value>
		public override bool UseMaxForLimit
		{
			get { return false; }
		}

		public override SqlString GetLimitString(SqlString querySqlString, SqlString offset, SqlString limit)
		{
			var result = new SqlStringBuilder(querySqlString);

			if (offset != null)
			{
				result.Add(" OFFSET ");
				result.Add(offset).Add(" ROWS");
			}

			if (limit != null)
			{
				result.Add(" FETCH FIRST ").Add(limit).Add(" ROWS ONLY");
			}

			return result.ToSqlString();
		}
	}
}