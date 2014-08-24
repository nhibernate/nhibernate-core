using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// The base class for all of the SqlBuilders.
	/// </summary>
	public abstract class SqlBaseBuilder
	{
		private readonly Dialect.Dialect dialect;
		private readonly IMapping mapping;

		protected SqlBaseBuilder(Dialect.Dialect dialect, IMapping mapping)
		{
			this.dialect = dialect;
			this.mapping = mapping;
		}

		protected IMapping Mapping
		{
			get { return mapping; }
		}

		public Dialect.Dialect Dialect
		{
			get { return dialect; }
		}

		/// <summary>
		/// Converts the ColumnNames and ColumnValues to a WhereFragment
		/// </summary>
		/// <param name="columnNames">The names of the Columns to Add to the WhereFragment</param>
		/// <returns>A SqlString that contains the WhereFragment</returns>
		/// <remarks>This just calls the overloaded ToWhereFragment() with the operator as " = " and the tableAlias null.</remarks>
		protected SqlString ToWhereString(string[] columnNames)
		{
			return ToWhereString(columnNames, " = ");
		}

		/// <summary>
		/// Converts the ColumnNames and ColumnValues to a WhereFragment
		/// </summary>
		/// <param name="tableAlias">The Alias for the Table.</param>
		/// <param name="columnNames">The names of the Columns to Add to the WhereFragment</param>
		/// <returns>A SqlString that contains the WhereFragment</returns>
		/// <remarks>This defaults the op to " = "</remarks>
		protected SqlString ToWhereString(string tableAlias, string[] columnNames)
		{
			return ToWhereString(tableAlias, columnNames, " = ");
		}

		/// <summary>
		/// Converts the ColumnNames and ColumnValues to a WhereFragment
		/// </summary>
		/// <param name="columnNames">The names of the Columns to Add to the WhereFragment</param>
		/// <param name="op">The operator to use between the names &amp; values.  For example " = " or "!="</param>
		/// <returns>A SqlString that contains the WhereFragment</returns>
		protected SqlString ToWhereString(string[] columnNames, string op)
		{
			return ToWhereString(null, columnNames, op);
		}

		/// <summary>
		/// Converts the ColumnNames and ColumnValues to a WhereFragment
		/// </summary>
		/// <param name="tableAlias">The Alias for the Table.</param>
		/// <param name="columnNames">The names of the Columns to Add to the WhereFragment</param>
		/// <param name="op">The operator to use between the names &amp; values.  For example " = " or "!="</param>
		/// <returns>A SqlString that contains the WhereFragment</returns>
		protected SqlString ToWhereString(string tableAlias, string[] columnNames, string op)
		{
			SqlStringBuilder sqlBuilder = new SqlStringBuilder((columnNames.Length * 2) + 5);

			bool andNeeded = false;

			for (int i = 0; i < columnNames.Length; i++)
			{
				if (string.IsNullOrEmpty(columnNames[i])) continue;// prevent empty column name

				if (andNeeded)
				{
					sqlBuilder.Add(" AND ");
				}
				andNeeded = true;

				string columnName;
				if (tableAlias != null && tableAlias.Length > 0)
				{
					columnName = tableAlias + StringHelper.Dot + columnNames[i];
				}
				else
				{
					columnName = columnNames[i];
				}

				sqlBuilder
					.Add(columnName)
					.Add(op)
					.AddParameter();
			}

			return sqlBuilder.ToSqlString();
		}

		protected SqlString ToWhereString(string columnName, string op)
		{
			if (string.IsNullOrEmpty(columnName)) return null;
			return new SqlString(columnName, op, Parameter.Placeholder);
		}

	}
}