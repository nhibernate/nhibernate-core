using System.Data;
using System.Text;
using NHibernate.Driver;
using NHibernate.Mapping;
using NHibernate.SqlCommand;

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
		    return new MsSql2005SelectQuery(queryString).GetLimitString(offset, limit);
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

        public class MsSql2005SelectQuery : SelectQuery
        {
            public MsSql2005SelectQuery(SqlString sql)
                : base(sql)
            { }

            public override SqlString GetLimitString(SqlString offset, SqlString limit)
            {
                if (limit == null && offset == null) return this.Sql;
                if (limit != null && offset == null) return GetLimitOnlyString(limit);

                var result = new SqlStringBuilder();
                BuildSelectClauseForPagingQuery(limit, result);
                BuildFromClauseForPagingQuery(result);
                BuildWhereAndOrderClausesForPagingQuery(offset, result);
                return result.ToSqlString();
            }

            private SqlString GetLimitOnlyString(SqlString limit)
            {
                var columnDefinitionsBeginIndex = this.ColumnsBeginIndex;
                if (columnDefinitionsBeginIndex < 0) return null;

                var result = new SqlStringBuilder();

                return result
                    .Add(this.Sql.Substring(0, columnDefinitionsBeginIndex))
                    .Add(" TOP (")
                    .Add(limit)
                    .Add(")")
                    .Add(this.Sql.Substring(columnDefinitionsBeginIndex))
                    .ToSqlString();
            }

            private void BuildSelectClauseForPagingQuery(SqlString limit, SqlStringBuilder result)
            {
                result.Add(this.Sql.Substring(0, this.SelectIndex));
                result.Add("SELECT");

                if (limit != null)
                {
                    result.Add(" TOP (").Add(limit).Add(") ");
                }
                else
                {
                    // ORDER BY can only be used in subqueries if TOP is also specified.
                    result.Add(" TOP (" + int.MaxValue + ") ");
                }

                var sb = new StringBuilder();
                foreach (var column in this.ColumnDefinitions)
                {
                    if (sb.Length > 0) sb.Append(", ");
                    sb.Append(column.Alias);
                }

                result.Add(sb.ToString());
            }

            private void BuildFromClauseForPagingQuery(SqlStringBuilder result)
            {
                var selectClause = this.Sql.Substring(this.SelectIndex, this.FromIndex - this.SelectIndex);
                var subselectClause = this.OrderByIndex >= 0
                    ? this.Sql.Substring(this.FromIndex, this.OrderByIndex - this.FromIndex)
                    : this.Sql.Substring(this.FromIndex);

                result.Add(" FROM (")
                    .Add(selectClause.Trim())
                    .Add(", ROW_NUMBER() OVER(ORDER BY ");

                int orderIndex = 0;
                foreach (var order in this.OrderDefinitions)
                {
                    if (orderIndex++ > 0) result.Add(", ");
                    if (order.Column.Name != null)
                    {
                        result.Add(order.Column.Name);
                    }
                    else
                    {
                        result.Add(this.Sql.Substring(order.Column.SqlIndex, order.Column.SqlLength).Trim());
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
        }
    }
}