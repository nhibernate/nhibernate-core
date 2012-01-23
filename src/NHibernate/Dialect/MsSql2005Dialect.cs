using System.Data;
using System.Text;
using NHibernate.Driver;
using NHibernate.Mapping;
using NHibernate.SqlCommand;
using NHibernate.SqlCommand.Parser;

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
			if (offset == null)
			{
				int insertPoint;
				return TryFindLimitInsertPoint(queryString, out insertPoint)
					? queryString.Insert(insertPoint, new SqlString("TOP (", limit, ") "))
					: null;
			}

			var queryParser = new MsSqlSelectParser(queryString);
			if (queryParser.SelectIndex < 0) return null;

			var result = new SqlStringBuilder();
			BuildSelectClauseForPagingQuery(queryParser, limit, result);
			BuildFromClauseForPagingQuery(queryParser, result);
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
			foreach (var column in sqlQuery.ColumnDefinitions)
			{
				if (sb.Length > 0) sb.Append(", ");
				sb.Append(column.Alias);
			}

			result.Add(sb.ToString());
		}

		private static void BuildFromClauseForPagingQuery(MsSqlSelectParser sqlQuery, SqlStringBuilder result)
		{
			var selectClause = sqlQuery.Sql.Substring(sqlQuery.SelectIndex, sqlQuery.FromIndex - sqlQuery.SelectIndex);
			var subselectClause = sqlQuery.OrderByIndex >= 0
				? sqlQuery.Sql.Substring(sqlQuery.FromIndex, sqlQuery.OrderByIndex - sqlQuery.FromIndex)
				: sqlQuery.Sql.Substring(sqlQuery.FromIndex);

			result.Add(" FROM (")
				.Add(selectClause.Trim())
				.Add(", ROW_NUMBER() OVER(ORDER BY ");

			int orderIndex = 0;
			foreach (var order in sqlQuery.OrderDefinitions)
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
				.Add(subselectClause.Trim())
				.Add(") as query");
		}

		private static void BuildWhereAndOrderClausesForPagingQuery(SqlString offset, SqlStringBuilder result)
		{
			result.Add(" WHERE query.__hibernate_sort_row > ")
				.Add(offset)
				.Add(" ORDER BY query.__hibernate_sort_row");
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