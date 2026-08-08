using NHibernate.SqlCommand;

namespace NHibernate.Dialect
{
	/// <summary>
	/// A dialect for Firebird 3 and above.
	/// </summary>
	/// <remarks>
	/// Firebird 3 supports the SQL standard <c>OFFSET</c>/<c>FETCH</c> clause. Unlike the
	/// <c>FIRST</c>/<c>SKIP</c> clause used by <see cref="FirebirdDialect" />, it is placed at the end of
	/// the statement and applies to the whole query, including unions.
	/// </remarks>
	public class Firebird3Dialect : FirebirdDialect
	{
		public override SqlString GetLimitString(SqlString queryString, SqlString offset, SqlString limit)
		{
			var result = new SqlStringBuilder(queryString);

			if (offset != null)
			{
				result.Add(" offset ").Add(offset).Add(" rows");
			}

			if (limit != null)
			{
				result.Add(" fetch first ").Add(limit).Add(" rows only");
			}

			return result.ToSqlString();
		}
	}
}
