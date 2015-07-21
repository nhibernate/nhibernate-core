using System.Text;
using NHibernate.SqlCommand.Parser;
using NHibernate.SqlCommand;

namespace NHibernate.Dialect
{
	/// <summary>
	/// Transforms a T-SQL SELECT statement into a statement that will - when executed - return a 'page' of results. The page is defined
	/// by a page size ('limit'), and/or a starting page number ('offset').
	/// </summary>
	internal class MsSql2005DialectQueryPager
	{
		private readonly SqlString _sourceQuery;

		public MsSql2005DialectQueryPager(SqlString sourceQuery)
		{
			_sourceQuery = sourceQuery;
		}

		/// <summary>
		/// Returns a TSQL SELECT statement that will - when executed - return a 'page' of results.
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="limit"></param>
		/// <returns></returns>
		public SqlString PageBy(SqlString offset, SqlString limit)
		{
			if (offset == null)
				return PageByLimitOnly(limit);

			return PageByLimitAndOffset(offset, limit);
		}

		private SqlString PageByLimitOnly(SqlString limit)
		{
			var tokenEnum = new SqlTokenizer(_sourceQuery).GetEnumerator();
			if (!tokenEnum.TryParseUntilFirstMsSqlSelectColumn()) return null;
			
			int insertPoint = tokenEnum.Current.SqlIndex;
			return _sourceQuery.Insert(insertPoint, new SqlString("TOP (", limit, ") "));
		}

		private SqlString PageByLimitAndOffset(SqlString offset, SqlString limit)
		{
			var queryParser = new MsSqlSelectParser(_sourceQuery);
			if (queryParser.SelectIndex < 0) return null;

			var result = new SqlStringBuilder();
			BuildSelectClauseForPagingQuery(queryParser, limit, result);
			if (queryParser.IsDistinct)
			{
				BuildFromClauseForPagingDistinctQuery(queryParser, result);
			}
			else
			{
				BuildFromClauseForPagingQuery(queryParser, result);
			}
			BuildWhereAndOrderClausesForPagingQuery(offset, result);
			return result.ToSqlString();
		}

		private static void BuildSelectClauseForPagingQuery(MsSqlSelectParser sqlQuery, SqlString limit, SqlStringBuilder result)
		{
			result.Add(sqlQuery.Sql.Substring(0, sqlQuery.SelectIndex));
			result.Add("SELECT ");

			if (limit != null)
			{
				result.Add("TOP (").Add(limit).Add(") ");
			}
			else
			{
				// ORDER BY can only be used in subqueries if TOP is also specified.
				result.Add("TOP (" + int.MaxValue + ") ");
			}

			var sb = new StringBuilder();
			foreach (var column in sqlQuery.SelectColumns)
			{
				if (sb.Length > 0) sb.Append(", ");
				sb.Append(column.Alias);
			}

			result.Add(sb.ToString());
		}

		private static void BuildFromClauseForPagingQuery(MsSqlSelectParser sqlQuery, SqlStringBuilder result)
		{
			result.Add(" FROM (")
				.Add(sqlQuery.SelectClause)
				.Add(", ROW_NUMBER() OVER(ORDER BY ");

			var orderIndex = 0;
			foreach (var order in sqlQuery.Orders)
			{
				if (orderIndex++ > 0) result.Add(", ");
				if (order.Column.Name != null)
				{
					result.Add(order.Column.Name);
				}
				else
				{
					result.Add(sqlQuery.Sql.Substring(order.Column.SqlIndex, order.Column.SqlLength).Trim());
				}
				if (order.IsDescending) result.Add(" DESC");
			}

			if (orderIndex == 0)
			{
				result.Add("CURRENT_TIMESTAMP");
			}

			result.Add(") as __hibernate_sort_row ")
				.Add(sqlQuery.FromAndWhereClause)
				.Add(") as query");
		}

		private static void BuildFromClauseForPagingDistinctQuery(MsSqlSelectParser sqlQuery, SqlStringBuilder result)
		{
			result.Add(" FROM (SELECT *, ROW_NUMBER() OVER(ORDER BY ");

			int orderIndex = 0;
			foreach (var order in sqlQuery.Orders)
			{
				if (orderIndex++ > 0) result.Add(", ");
				if (!order.Column.InSelectClause)
				{
					throw new HibernateException(
						"The dialect was unable to perform paging of a statement that requires distinct results, and " +
						"is ordered by a column that is not included in the result set of the query.");
				}
				result.Add("q_.").Add(order.Column.Alias);
				if (order.IsDescending) result.Add(" DESC");
			}
			if (orderIndex == 0)
			{
				result.Add("CURRENT_TIMESTAMP");
			}

			result.Add(") as __hibernate_sort_row  FROM (")
				.Add(sqlQuery.SelectClause)
				.Add(" ")
				.Add(sqlQuery.FromAndWhereClause)
				.Add(") as q_) as query");
		}

		private static void BuildWhereAndOrderClausesForPagingQuery(SqlString offset, SqlStringBuilder result)
		{
			result.Add(" WHERE query.__hibernate_sort_row > ")
				.Add(offset)
				.Add(" ORDER BY query.__hibernate_sort_row");
		}
	}
}