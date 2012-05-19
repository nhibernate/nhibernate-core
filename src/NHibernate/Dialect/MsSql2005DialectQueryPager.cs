using System;
using System.Collections.Generic;
using NHibernate.SqlCommand;
using NHibernate.Util;

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
			var result = new SqlStringBuilder();

			int insertPoint = GetAfterSelectInsertPoint();

			return result
				.Add(_sourceQuery.Substring(0, insertPoint))
				.Add(" TOP (")
				.Add(limit)
				.Add(") ")
				.Add(_sourceQuery.Substring(insertPoint))
				.ToSqlString();
		}

		private SqlString PageByLimitAndOffset(SqlString offset, SqlString limit)
		{
			int fromIndex = GetFromIndex();
			SqlString select = _sourceQuery.Substring(0, fromIndex);

			List<SqlString> columnsOrAliases;
			Dictionary<SqlString, SqlString> aliasToColumn;
			Dictionary<SqlString, SqlString> columnToAlias;

			Dialect.ExtractColumnOrAliasNames(select, out columnsOrAliases, out aliasToColumn, out columnToAlias);

			int orderIndex = _sourceQuery.LastIndexOfCaseInsensitive(" order by ");
			SqlString fromAndWhere;
			SqlString[] sortExpressions;

			//don't use the order index if it is contained within a larger statement(assuming
			//a statement with non matching parenthesis is part of a larger block)
			if (orderIndex > 0 && HasMatchingParens(_sourceQuery.Substring(orderIndex).ToString()))
			{
				fromAndWhere = _sourceQuery.Substring(fromIndex, orderIndex - fromIndex).Trim();
				SqlString orderBy = _sourceQuery.Substring(orderIndex).Trim().Substring(9);
				sortExpressions = orderBy.SplitWithRegex(@"(?<!\([^\)]*),{1}");
			}
			else
			{
				fromAndWhere = _sourceQuery.Substring(fromIndex).Trim();
				// Use dummy sort to avoid errors
				sortExpressions = new[] { new SqlString("CURRENT_TIMESTAMP") };
			}

			var result = new SqlStringBuilder();

			result.Add("SELECT ");

			if (limit != null)
				result.Add("TOP (").Add(limit).Add(") ");
			else
				// ORDER BY can only be used in subqueries if TOP is also specified.
				result.Add("TOP (" + int.MaxValue + ") ");

			if (IsDistinct())
			{
				result
					.Add(StringHelper.Join(", ", columnsOrAliases))
					.Add(" FROM (SELECT *, ROW_NUMBER() OVER(ORDER BY ");

				AppendSortExpressionsForDistinct(columnToAlias, sortExpressions, result);

				result.Add(") as __hibernate_sort_row ")
					.Add(" FROM (")
					.Add(select)
					.Add(" ")
					.Add(fromAndWhere)
					.Add(") as q_) as query WHERE query.__hibernate_sort_row > ")
					.Add(offset)
					.Add(" ORDER BY query.__hibernate_sort_row");
			}
			else
			{
				result
					.Add(StringHelper.Join(", ", columnsOrAliases))
					.Add(" FROM (")
					.Add(select)
					.Add(", ROW_NUMBER() OVER(ORDER BY ");

				AppendSortExpressions(aliasToColumn, sortExpressions, result);

				result
					.Add(") as __hibernate_sort_row ")
					.Add(fromAndWhere)
					.Add(") as query WHERE query.__hibernate_sort_row > ")
					.Add(offset)
					.Add(" ORDER BY query.__hibernate_sort_row");
			}

			return result.ToSqlString();
		}

		private static SqlString RemoveSortOrderDirection(SqlString sortExpression)
		{
			SqlString trimmedExpression = sortExpression.Trim();
			if (trimmedExpression.EndsWithCaseInsensitive("asc"))
				return trimmedExpression.Substring(0, trimmedExpression.Length - 3).Trim();
			if (trimmedExpression.EndsWithCaseInsensitive("desc"))
				return trimmedExpression.Substring(0, trimmedExpression.Length - 4).Trim();
			return trimmedExpression.Trim();
		}

		/// <summary>
		/// Identify the columns for the <code>ROW_NUMBER OVER(ORDER BY ...)</code> expression.
		/// </summary>
		/// <param name="aliasToColumn"></param>
		/// <param name="sortExpressions"></param>
		/// <param name="result"></param>
		/// <remarks>
		/// This method translates aliased columns (as appear in the SELECT list) to straight column names, to be included in the 
		/// <code>ROW_NUMBER() OVER(ORDER BY ...)</code> expression. For example, <code>datapoint0_.xval as xval4_</code> to
		/// <code>datapoint0_.xval</code>.
		/// </remarks>
		private static void AppendSortExpressions(Dictionary<SqlString, SqlString> aliasToColumn, SqlString[] sortExpressions, SqlStringBuilder result)
		{
			for (int i = 0; i < sortExpressions.Length; i++)
			{
				if (i > 0)
				{
					result.Add(", ");
				}

				SqlString sortExpression = RemoveSortOrderDirection(sortExpressions[i]);
				if (aliasToColumn.ContainsKey(sortExpression))
				{
					result.Add(aliasToColumn[sortExpression]);
				}
				else
				{
					result.Add(sortExpression);
				}
				if (sortExpressions[i].Trim().EndsWithCaseInsensitive("desc"))
				{
					result.Add(" DESC");
				}
			}
		}

		/// <summary>
		/// For a <code>DISTINCT</code> query, identify the columns for the <code>ROW_NUMBER() OVER(ORDER BY ...)</code> expression.
		/// </summary>
		/// <param name="columnToAlias"></param>
		/// <param name="sortExpressions"></param>
		/// <param name="result"></param>
		/// <remarks>
		/// For a paged <code>DISTINCT</code> query, the columns on which ordering will be performed are returned by a sub-query. Therefore the 
		/// columns in the <code>ROW_NUMBER() OVER(ORDER BY ...)</code> expression need to use the aliased column name from the subquery, 
		/// prefixed by the subquery alias, <code>q_</code>.
		/// </remarks>
		private static void AppendSortExpressionsForDistinct(Dictionary<SqlString, SqlString> columnToAlias, SqlString[] sortExpressions, SqlStringBuilder result)
		{
			for (int i = 0; i < sortExpressions.Length; i++)
			{
				if (i > 0)
				{
					result.Add(", ");
				}

				SqlString sortExpression = RemoveSortOrderDirection(sortExpressions[i]);

				if (sortExpression.StartsWithCaseInsensitive("CURRENT_TIMESTAMP"))
					result.Add(sortExpression);

				else if (columnToAlias.ContainsKey(sortExpression))
				{
					result.Add("q_.");
					result.Add(columnToAlias[sortExpression]);
				}
				else
				{
					throw new HibernateException(
						"The dialect was unable to perform paging of a statement that requires distinct results, and "
						+ "is ordered by a column that is not included in the result set of the query.");
				}

				if (sortExpressions[i].Trim().EndsWithCaseInsensitive("desc"))
				{
					result.Add(" DESC");
				}
			}
		}

		/// <summary>
		/// Indicates whether the string fragment contains matching parenthesis
		/// </summary>
		/// <param name="statement"> the statement to evaluate</param>
		/// <returns>true if the statment contains no parenthesis or an equal number of
		///  opening and closing parenthesis;otherwise false </returns>
		private static bool HasMatchingParens(IEnumerable<char> statement)
		{
			//unmatched paren count
			int unmatchedParen = 0;

			//increment the counts based in the opening and closing parens in the statement
			foreach (char item in statement)
			{
				switch (item)
				{
					case '(':
						unmatchedParen++;
						break;
					case ')':
						unmatchedParen--;
						break;
				}
			}

			return unmatchedParen == 0;
		}

		private int GetFromIndex()
		{
			string subselect = _sourceQuery.GetSubselectString().ToString();
			int fromIndex = _sourceQuery.IndexOfCaseInsensitive(subselect);
			if (fromIndex == -1)
			{
				fromIndex = _sourceQuery.ToString().ToLowerInvariant().IndexOf(subselect.ToLowerInvariant());
			}
			return fromIndex;
		}

		private int GetAfterSelectInsertPoint()
		{
			if (_sourceQuery.StartsWithCaseInsensitive("select distinct"))
			{
				return 15;
			}
			if (_sourceQuery.StartsWithCaseInsensitive("select"))
			{
				return 6;
			}
			throw new NotSupportedException("The query should start with 'SELECT' or 'SELECT DISTINCT'");
		}

		/// <remarks>
		/// Perhaps SqlString should have these types of method on it...
		/// </remarks>
		/// <returns></returns>
		private bool IsDistinct()
		{
			return (_sourceQuery.StartsWithCaseInsensitive("select distinct"));
		}
	}
}