using System.Collections.Generic;


namespace NHibernate.SqlCommand
{
	public class InsertSelect : ISqlStringBuilder
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(InsertSelect));

		private string tableName;
		private string comment;
		private readonly List<string> columnNames = new List<string>();
		private SqlSelectBuilder select;

		public virtual InsertSelect SetTableName(string tableName)
		{
			this.tableName = tableName;
			return this;
		}

		public virtual InsertSelect SetComment(string comment)
		{
			this.comment = comment;
			return this;
		}

		public virtual InsertSelect AddColumn(string columnName)
		{
			columnNames.Add(columnName);
			return this;
		}

		public virtual InsertSelect AddColumns(string[] columnNames)
		{
			this.columnNames.AddRange(columnNames);
			return this;
		}

		public virtual InsertSelect SetSelect(SqlSelectBuilder select)
		{
			this.select = select;
			return this;
		}

		public SqlString ToSqlString()
		{
			if (tableName == null)
				throw new HibernateException("no table name defined for insert-select");
			if (select == null)
				throw new HibernateException("no select defined for insert-select");

			var buf = new SqlStringBuilder(columnNames.Count + 4);
			if (comment != null)
			{
				buf.Add("/* " + comment + " */ ");
			}
			buf.Add("insert into ").Add(tableName);
			if (!(columnNames.Count == 0))
			{
				buf.Add(" (");
				bool commaNeeded= false;
				foreach (var columnName in columnNames)
				{
					if(commaNeeded)
					{
						buf.Add(", ");
					}
					buf.Add(columnName);
					commaNeeded = true;
				}
				buf.Add(")");
			}
			buf.Add(" ").Add(select.ToStatementString());
			return buf.ToSqlString();
		}
	}
}