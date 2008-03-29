using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// Summary description for SqlSimpleSelectBuilder.
	/// </summary>
	public class SqlSimpleSelectBuilder : SqlBaseBuilder, ISqlStringBuilder
	{
		private string tableName;

		private readonly IList<string> columnNames = new List<string>();
		private readonly IDictionary<string, string> aliases = new Dictionary<string, string>(); //key=column Name, value=column Alias
		private LockMode lockMode = LockMode.Read;
		private string comment;

		private readonly List<SqlString> whereStrings = new List<SqlString>();

		//these can be plain strings because a forUpdate and orderBy will have
		// no parameters so using a SqlString will only complicate matters - or 
		// maybe simplify because any Sql will be contained in a known object type...
		private string orderBy;

		public SqlSimpleSelectBuilder(Dialect.Dialect dialect, IMapping factory)
			: base(dialect, factory) {}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public SqlSimpleSelectBuilder SetTableName(string tableName)
		{
			this.tableName = tableName;
			return this;
		}


		/// <summary>
		/// Adds a columnName to the SELECT fragment.
		/// </summary>
		/// <param name="columnName">The name of the column to add.</param>
		/// <returns>The SqlSimpleSelectBuilder</returns>
		public SqlSimpleSelectBuilder AddColumn(string columnName)
		{
			columnNames.Add(columnName);
			return this;
		}

		/// <summary>
		/// Adds a columnName and its Alias to the SELECT fragment.
		/// </summary>
		/// <param name="columnName">The name of the column to add.</param>
		/// <param name="alias">The alias to use for the column</param>
		/// <returns>The SqlSimpleSelectBuilder</returns>
		public SqlSimpleSelectBuilder AddColumn(string columnName, string alias)
		{
			columnNames.Add(columnName);
			aliases[columnName] = alias;
			return this;
		}

		/// <summary>
		/// Adds an array of columnNames to the SELECT fragment.
		/// </summary>
		/// <param name="columnNames">The names of the columns to add.</param>
		/// <returns>The SqlSimpleSelectBuilder</returns>
		public SqlSimpleSelectBuilder AddColumns(string[] columnNames)
		{
			for (int i = 0; i < columnNames.Length; i++)
			{
				if (columnNames[i] != null)
					AddColumn(columnNames[i]);
			}
			return this;
		}

		/// <summary>
		/// Adds an array of columnNames with their Aliases to the SELECT fragment.
		/// </summary>
		/// <param name="columnNames">The names of the columns to add.</param>
		/// <param name="aliases">The aliases to use for the columns</param>
		/// <returns>The SqlSimpleSelectBuilder</returns>
		public SqlSimpleSelectBuilder AddColumns(string[] columnNames, string[] aliases)
		{
			for (int i = 0; i < columnNames.Length; i++)
			{
				if (columnNames[i] != null)
					AddColumn(columnNames[i], aliases[i]);
			}
			return this;
		}

		public SqlSimpleSelectBuilder AddColumns(string[] columns, string[] aliases, bool[] ignore)
		{
			for (int i = 0; i < ignore.Length; i++)
			{
				if (!ignore[i] && columns[i] != null)
					AddColumn(columns[i], aliases[i]);
			}
			return this;
		}

		public virtual SqlSimpleSelectBuilder SetLockMode(LockMode lockMode)
		{
			this.lockMode = lockMode;
			return this;
		}

		/// <summary>
		/// Gets the Alias that should be used for the column
		/// </summary>
		/// <param name="columnName">The name of the column to get the Alias for.</param>
		/// <returns>The Alias if one exists, null otherwise</returns>
		public string GetAlias(string columnName)
		{
			string result;
			aliases.TryGetValue(columnName, out result);
			return result;
		}

		/// <summary>
		/// Sets the IdentityColumn for the <c>SELECT</c> sql to use.
		/// </summary>
		/// <param name="columnNames">An array of the column names for the Property</param>
		/// <param name="identityType">The IType of the Identity Property.</param>
		/// <returns>The SqlSimpleSelectBuilder.</returns>
		public SqlSimpleSelectBuilder SetIdentityColumn(string[] columnNames, IType identityType)
		{
			whereStrings.Add(ToWhereString(columnNames));
			return this;
		}

		/// <summary>
		/// Sets the VersionColumn for the <c>SELECT</c> sql to use.
		/// </summary>
		/// <param name="columnNames">An array of the column names for the Property</param>
		/// <param name="versionType">The IVersionType of the Version Property.</param>
		/// <returns>The SqlSimpleSelectBuilder.</returns>
		public SqlSimpleSelectBuilder SetVersionColumn(string[] columnNames, IVersionType versionType)
		{
			whereStrings.Add(ToWhereString(columnNames));
			return this;
		}

		/// <summary>
		/// Set the Order By fragment of the Select Command
		/// </summary>
		/// <param name="orderBy">The OrderBy fragment.  It should include the SQL "ORDER BY"</param>
		/// <returns>The SqlSimpleSelectBuilder</returns>
		public SqlSimpleSelectBuilder SetOrderBy(string orderBy)
		{
			this.orderBy = orderBy;
			return this;
		}

		/// <summary>
		/// Adds the columns for the Type to the WhereFragment
		/// </summary>
		/// <param name="columnNames">The names of the columns to add.</param>
		/// <param name="type">The IType of the property.</param>
		/// <param name="op">The operator to put between the column name and value.</param>
		/// <returns>The SqlSimpleSelectBuilder</returns>
		public SqlSimpleSelectBuilder AddWhereFragment(string[] columnNames, IType type, string op)
		{
			whereStrings.Add(ToWhereString(columnNames, op));
			return this;
		}

		public virtual SqlSimpleSelectBuilder SetComment(System.String comment)
		{
			this.comment = comment;
			return this;
		}

		#region ISqlStringBuilder Members

		/// <summary></summary>
		public SqlString ToSqlString()
		{
			//TODO: add default capacity
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();

			if (comment != null)
			{
				sqlBuilder.Add("/* " + comment + " */ ");
			}

			bool commaNeeded = false;
			sqlBuilder.Add("SELECT ");

			for (int i = 0; i < columnNames.Count; i++)
			{
				string column = columnNames[i];
				string alias = GetAlias(column);

				if (commaNeeded)
				{
					sqlBuilder.Add(StringHelper.CommaSpace);
				}

				sqlBuilder.Add(column);
				if (alias != null && !alias.Equals(column))
				{
					sqlBuilder.Add(" AS ")
						.Add(alias);
				}

				commaNeeded = true;
			}


			sqlBuilder.Add(" FROM ")
				.Add(Dialect.AppendLockHint(lockMode, tableName));

			sqlBuilder.Add(" WHERE ");

			if (whereStrings.Count > 1)
				sqlBuilder.Add(whereStrings.ToArray(), null, "AND", null, false);
			else
				sqlBuilder.Add(whereStrings[0]);

			if (orderBy != null)
			{
				sqlBuilder.Add(orderBy);
			}

			if (lockMode != null)
			{
				sqlBuilder.Add(Dialect.GetForUpdateString(lockMode));
			}

			return sqlBuilder.ToSqlString();
		}

		#endregion
	}
}