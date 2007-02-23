using System;
using System.Data;
using NHibernate.SqlCommand;

namespace NHibernate.Dialect
{
	/// <summary>
	/// A <see cref="Dialect"/> for <c>Microsoft Sql Server 2005</c>.
	/// </summary>
	public class MsSql2005Dialect : MsSql2000Dialect
	{
		public MsSql2005Dialect()
		{
			RegisterColumnType(DbType.String, 1073741823, "NVARCHAR(MAX)");
			RegisterColumnType(DbType.AnsiString, 2147483647, "VARCHAR(MAX)");
			RegisterColumnType(DbType.Binary, 2147483647, "VARBINARY(MAX)");
		}

		/// <summary>
		/// Add a <c>LIMIT</c> clause to the given SQL <c>SELECT</c>
		/// </summary>
		/// <param name="querySqlString">The <see cref="SqlString"/> to base the limit query off of.</param>
		/// <param name="offset">Offset of the first row to be returned by the query (zero-based)</param>
		/// <param name="last">Maximum number of rows to be returned by the query</param>
		/// <returns>A new <see cref="SqlString"/> with the <c>LIMIT</c> clause applied.</returns>
		/// <remarks>
		/// The <c>LIMIT</c> SQL will look like
		/// <code>
		/// WITH query AS
		///     (SELECT TOP last ROW_NUMBER() OVER (ORDER BY orderby) as __hibernate_row_nr__, ... original_query)
		///  SELECT * 
		///    FROM query
		///    WHERE __hibernate_row_nr__ > offset
		///    ORDER BY __hibernate_row_nr__
		/// </code>
		/// </remarks>
		public override SqlString GetLimitString(SqlString querySqlString, int offset, int last)
		{
			string distinctStr;

			int index;
			if (querySqlString.StartsWithCaseInsensitive("select distinct"))
			{
				distinctStr = "DISTINCT ";
				index = 15;
			}
			else if (querySqlString.StartsWithCaseInsensitive("select "))
			{
				distinctStr = string.Empty;
				index = 6;
			}
			else
			{
				throw new ArgumentException("querySqlString should start with select", "querySqlString");
			}

			SqlString afterSelect = querySqlString.Substring(index);

			string orderBy = querySqlString.SubstringStartingWithLast("order by").ToString();
			if (orderBy.Length == 0)
			{
				// If no ORDER BY is specified use fake ORDER BY field to avoid errors 
				orderBy = "ORDER BY CURRENT_TIMESTAMP";
			}

			SqlStringBuilder result = new SqlStringBuilder();
			return result
				.Add("WITH query AS (SELECT ")
				.Add(distinctStr)
				.Add("TOP ")
				.Add(last.ToString())
				.Add(" ROW_NUMBER() OVER (")
				.Add(orderBy)
				.Add(") as __hibernate_row_nr__, ")
				.Add(afterSelect)
				.Add(") SELECT * FROM query WHERE __hibernate_row_nr__ > ")
				.Add(offset.ToString())
				.Add(" ORDER BY __hibernate_row_nr__")
				.ToSqlString();
		}

		/// <summary>
		/// Sql Server 2005 supports a query statement that provides <c>LIMIT</c>
		/// functionallity.
		/// </summary>
		/// <value><c>true</c></value>
		public override bool SupportsLimit
		{
			get { return true; }
		}

		/// <summary>
		/// Sql Server 2005 supports a query statement that provides <c>LIMIT</c>
		/// functionallity with an offset.
		/// </summary>
		/// <value><c>true</c></value>
		public override bool SupportsLimitOffset
		{
			get { return true; }
		}
	}
}