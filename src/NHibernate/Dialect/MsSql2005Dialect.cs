using System;
using System.Collections.Generic;
using System.Data;
using NHibernate.Driver;
using NHibernate.Mapping;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Dialect
{
	public class MsSql2005Dialect : MsSql2000Dialect
	{
		public MsSql2005Dialect()
		{
			RegisterColumnType(DbType.Xml, "XML");
		}

		protected override void RegisterCharacterTypeMappings()
		{
			base.RegisterCharacterTypeMappings();
			RegisterColumnType(DbType.String, SqlClientDriver.MaxSizeForClob, "NVARCHAR(MAX)");
			RegisterColumnType(DbType.AnsiString, SqlClientDriver.MaxSizeForAnsiClob, "VARCHAR(MAX)");
		}

		protected override void RegisterLargeObjectTypeMappings()
		{
			base.RegisterLargeObjectTypeMappings();
			RegisterColumnType(DbType.Binary, "VARBINARY(MAX)");
			RegisterColumnType(DbType.Binary, SqlClientDriver.MaxSizeForLengthLimitedBinary, "VARBINARY($l)");
			RegisterColumnType(DbType.Binary, SqlClientDriver.MaxSizeForBlob, "VARBINARY(MAX)");
		}

		protected override void RegisterKeywords()
		{
			base.RegisterKeywords();
			RegisterKeyword("xml");
		}

		public override SqlString GetLimitString(SqlString queryString, SqlString offset, SqlString limit)
		{
			var result = new SqlStringBuilder();

			if (offset == null)
			{
				int insertPoint = GetAfterSelectInsertPoint(queryString);

				return result
					.Add(queryString.Substring(0, insertPoint))
					.Add(" TOP (")
					.Add(limit)
					.Add(") ")
					.Add(queryString.Substring(insertPoint))
					.ToSqlString();
			}

			int fromIndex = GetFromIndex(queryString);
			SqlString select = queryString.Substring(0, fromIndex);

			List<SqlString> columnsOrAliases;
			Dictionary<SqlString, SqlString> aliasToColumn;
			ExtractColumnOrAliasNames(select, out columnsOrAliases, out aliasToColumn);

			int orderIndex = queryString.LastIndexOfCaseInsensitive(" order by ");
			SqlString fromAndWhere;
			SqlString[] sortExpressions;

			//don't use the order index if it is contained within a larger statement(assuming
			//a statement with non matching parenthesis is part of a larger block)
			if (orderIndex > 0 && HasMatchingParens(queryString.Substring(orderIndex).ToString()))
			{
				fromAndWhere = queryString.Substring(fromIndex, orderIndex - fromIndex).Trim();
				SqlString orderBy = queryString.Substring(orderIndex).Trim();
				sortExpressions = orderBy.Substring(9).Split(",");
			}
			else
			{
				fromAndWhere = queryString.Substring(fromIndex).Trim();
				// Use dummy sort to avoid errors
				sortExpressions = new[] { new SqlString("CURRENT_TIMESTAMP") };
			}

			result.Add("SELECT ");

			if (limit != null)
				result.Add("TOP (").Add(limit).Add(") ");
			else
				// ORDER BY can only be used in subqueries if TOP is also specified.
				result.Add("TOP (" + int.MaxValue + ") ");

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

		private static int GetFromIndex(SqlString querySqlString)
		{
			string subselect = querySqlString.GetSubselectString().ToString();
			int fromIndex = querySqlString.IndexOfCaseInsensitive(subselect);
			if (fromIndex == -1)
			{
				fromIndex = querySqlString.ToString().ToLowerInvariant().IndexOf(subselect.ToLowerInvariant());
			}
			return fromIndex;
		}

		private int GetAfterSelectInsertPoint(SqlString sql)
		{
			if (sql.StartsWithCaseInsensitive("select distinct"))
			{
				return 15;
			}
			if (sql.StartsWithCaseInsensitive("select"))
			{
				return 6;
			}
			throw new NotSupportedException("The query should start with 'SELECT' or 'SELECT DISTINCT'");
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

		/// <summary>
		/// Sql Server 2005 supports a query statement that provides <c>LIMIT</c>
		/// functionality.
		/// </summary>
		/// <value><c>true</c></value>
		public override bool SupportsLimit
		{
			get { return true; }
		}

		/// <summary>
		/// Sql Server 2005 supports a query statement that provides <c>LIMIT</c>
		/// functionality with an offset.
		/// </summary>
		/// <value><c>true</c></value>
		public override bool SupportsLimitOffset
		{
			get { return true; }
		}

		public override bool SupportsVariableLimit
		{
			get { return true; }
		}

		protected override string GetSelectExistingObject(string name, Table table)
		{
			string schema = table.GetQuotedSchemaName(this);
			if (schema != null)
			{
				schema += ".";
			}
			string objName = string.Format("{0}{1}", schema, Quote(name));
			string parentName = string.Format("{0}{1}", schema, table.GetQuotedName(this));
			return
				string.Format(
					"select 1 from sys.objects where object_id = OBJECT_ID(N'{0}') AND parent_object_id = OBJECT_ID('{1}')", objName,
					parentName);
		}

		/// <summary>
		/// Sql Server 2005 supports a query statement that provides <c>LIMIT</c>
		/// functionality with an offset.
		/// </summary>
		/// <value><c>false</c></value>
		public override bool UseMaxForLimit
		{
			get { return false; }
		}
	}
}