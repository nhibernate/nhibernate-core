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
				result.Add(" OFFSET ").Add(offset).Add(" ROWS");
			}

			if (limit != null)
			{
				if (offset == null)
				{
					result.Add(" OFFSET 0 ROWS");

					// According to Oracle Docs: 
					// http://docs.oracle.com/javadb/10.8.3.0/ref/rrefsqljoffsetfetch.html
					// the 'limit' param must be 1 or higher, but we have situations with limit=0.
					// This leads to undetermined behaviour of Oracle DBMS. It seems that the query
					// was executed correctly but the execution takes pretty long time.
					//
					// Empirically estimated that adding 'OFFSET 0 ROWS' to these types of queries 
					// ensures much faster execution. Stated above is a kind of hack to fix 
					// described situation.
				}
				result.Add(" FETCH FIRST ").Add(limit).Add(" ROWS ONLY");
			}

			return result.ToSqlString();
		}
	}
}