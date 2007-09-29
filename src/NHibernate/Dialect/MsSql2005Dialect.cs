namespace NHibernate.Dialect
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using Mapping;
	using SqlCommand;
	using Util;

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
		/// 
		/// SELECT TOP last (columns) FROM (
		/// SELECT ROW_NUMBER() OVER(ORDER BY __hibernate_sort_expr_1__ {sort direction 1} [, __hibernate_sort_expr_2__ {sort direction 2}, ...]) as row, (query.columns) FROM (
		///		{original select query part}, {sort field 1} as __hibernate_sort_expr_1__ [, {sort field 2} as __hibernate_sort_expr_2__, ...]
		///		{remainder of original query minus the order by clause}
		/// ) query
		/// ) page WHERE page.row > offset
		/// 
		/// </code>
		/// 
		/// Note that we need to add explicitly specify the columns, because we need to be able to use them
		/// in a paged subselect. NH-1155
		/// </remarks>
		public override SqlString GetLimitString(SqlString querySqlString, int offset, int last)
		{
			int fromIndex = querySqlString.IndexOfCaseInsensitive(" from ");
			SqlString select = querySqlString.Substring(0, fromIndex);
			List<string> columnsOrAliases;
			Dictionary<string, string> aliasToColumn;
			ExtractColumnOrAliasNames(select, out columnsOrAliases, out aliasToColumn);

			int orderIndex = querySqlString.LastIndexOfCaseInsensitive(" order by ");
			SqlString from;
			string[] sortExpressions;
			if (orderIndex > 0)
			{
				from = querySqlString.Substring(fromIndex, orderIndex - fromIndex).Trim();
				string orderBy = querySqlString.Substring(orderIndex).ToString().Trim();
				sortExpressions = orderBy.Substring(9).Split(',');
			}
			else
			{
				from = querySqlString.Substring(fromIndex).Trim();
				// Use dummy sort to avoid errors
				sortExpressions = new string[] { "CURRENT_TIMESTAMP" };
			}

			SqlStringBuilder result = new SqlStringBuilder()
				.Add("SELECT TOP ")
				.Add(last.ToString())
				.Add(" ")
				.Add(StringHelper.Join(", ", columnsOrAliases))
				.Add(" FROM (SELECT ROW_NUMBER() OVER(ORDER BY ");

			for (int i = 1; i <= sortExpressions.Length; i++)
			{
				if (i > 1)
					result.Add(", ");

				result.Add("__hibernate_sort_expr_")
					.Add(i.ToString())
					.Add("__");

				if (sortExpressions[i - 1].Trim().ToLower().EndsWith("desc"))
					result.Add(" DESC");
			}

			result.Add(") as row, ");

			for (int i = 0; i < columnsOrAliases.Count; i++)
			{
				result.Add("query.").Add(columnsOrAliases[i]);
				bool notLastColumn = i != columnsOrAliases.Count-1;
				if (notLastColumn)
					result.Add(", ");
			}

			result.Add(" FROM (")
				.Add(select);

			for (int i = 1; i <= sortExpressions.Length; i++)
			{
				string sortExpression = sortExpressions[i - 1].Trim().Split(' ')[0];
				if (sortExpression.EndsWith(")desc", StringComparison.InvariantCultureIgnoreCase))
				{
					sortExpression = sortExpression.Remove(sortExpression.Length - 4);
				}

				if (aliasToColumn.ContainsKey(sortExpression))
					sortExpression = aliasToColumn[sortExpression];

				result.Add(", ")
					.Add(sortExpression)
					.Add(" as __hibernate_sort_expr_")
					.Add(i.ToString())
					.Add("__");
			}

			result.Add(" ")
				.Add(from)
				.Add(") query ) page WHERE page.row > ")
				.Add(offset.ToString());

			return result.ToSqlString();
		}

		private static void ExtractColumnOrAliasNames(SqlString select, 
			out List<string> columnsOrAliases,
			out Dictionary<string, string> aliasToColumn)
		{
			columnsOrAliases = new List<string>();
			aliasToColumn = new Dictionary<string, string>();

			string selectString = select  + ",";
			int currentIndex, lastIndex = 7;// equals to "select ".Length
			while ((currentIndex = selectString.IndexOf(",", lastIndex)) != -1)
			{
				string columnAndAlias = selectString.Substring(lastIndex, currentIndex - lastIndex);
				int seperatorPosition = columnAndAlias.IndexOf(" as ");
				string columnOrAliasName;
				if (seperatorPosition != -1)
				{
					string alias = columnAndAlias.Substring(seperatorPosition + 4);
					string column = columnAndAlias.Substring(0, seperatorPosition);
					aliasToColumn[alias] = column;
					columnOrAliasName = alias;
				}
				else
				{
					seperatorPosition = columnAndAlias.IndexOf(".");
					columnOrAliasName = columnAndAlias.Substring(seperatorPosition + 1);
					aliasToColumn[columnOrAliasName] = columnOrAliasName;
				}
				columnsOrAliases.Add(columnOrAliasName);
				lastIndex = currentIndex + 1;
			}
		}

		/// <summary>
		/// Sql Server 2005 supports a query statement that provides <c>LIMIT</c>
		/// functionality.
		/// </summary>
		/// <value><c>true</c></value>
		public override bool SupportsLimit
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Sql Server 2005 supports a query statement that provides <c>LIMIT</c>
		/// functionality with an offset.
		/// </summary>
		/// <value><c>true</c></value>
		public override bool SupportsLimitOffset
		{
			get
			{
				return true;
			}
		}

		protected override string GetSelectExistingObject(string name, Table table)
		{
			string objName = table.GetQuotedSchemaName(this) + this.Quote(name);
			return string.Format("select 1 from sys.objects where object_id = OBJECT_ID(N'{0}') AND parent_object_id = OBJECT_ID('{1}')",
								 objName, table.GetQuotedName(this));
		}


		/// <summary>
		/// Sql Server 2005 supports a query statement that provides <c>LIMIT</c>
		/// functionality with an offset.
		/// </summary>
		/// <value><c>false</c></value>
		public override bool UseMaxForLimit
		{
			get
			{
				return false;
			}
		}
	}
}
