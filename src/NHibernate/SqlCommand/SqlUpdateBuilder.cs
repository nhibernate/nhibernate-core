using System.Collections.Generic;

using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// A class that builds an <c>UPDATE</c> sql statement.
	/// </summary>
	public class SqlUpdateBuilder : SqlBaseBuilder, ISqlStringBuilder
	{
		private static readonly ILogger log = LoggerProvider.LoggerFor(typeof(SqlUpdateBuilder));

		private string tableName;
		private string comment;

		// columns-> (ColumnName, Value) or (ColumnName, SqlType) for parametrized column
		private readonly LinkedHashMap<string, object> columns = new LinkedHashMap<string, object>();

		private List<SqlString> whereStrings = new List<SqlString>();
		private readonly List<SqlType> whereParameterTypes = new List<SqlType>();
		private SqlString assignments;

		public SqlUpdateBuilder(Dialect.Dialect dialect, IMapping mapping)
			: base(dialect, mapping) {}

		public SqlUpdateBuilder SetTableName(string tableName)
		{
			this.tableName = tableName;
			return this;
		}

		public SqlUpdateBuilder SetComment(string comment)
		{
			this.comment = comment;
			return this;
		}

		/// <summary>
		/// Add a column with a specific value to the UPDATE sql
		/// </summary>
		/// <param name="columnName">The name of the Column to add.</param>
		/// <param name="val">The value to set for the column.</param>
		/// <param name="literalType">The NHibernateType to use to convert the value to a sql string.</param>
		/// <returns>The SqlUpdateBuilder.</returns>
		public SqlUpdateBuilder AddColumn(string columnName, object val, ILiteralType literalType)
		{
			return AddColumn(columnName, literalType.ObjectToSQLString(val, Dialect));
		}


		/// <summary>
		/// Add a column with a specific value to the UPDATE sql
		/// </summary>
		/// <param name="columnName">The name of the Column to add.</param>
		/// <param name="val">A valid sql string to set as the value of the column.</param>
		/// <returns>The SqlUpdateBuilder.</returns>
		public SqlUpdateBuilder AddColumn(string columnName, string val)
		{
			columns[columnName] = val;
			return this;
		}

		/// <summary>
		/// Adds columns with a specific value to the UPDATE sql
		/// </summary>
		/// <param name="columnsName">The names of the Columns to add.</param>
		/// <param name="val">A valid sql string to set as the value of the column.  This value is assigned to each column.</param>
		/// <returns>The SqlUpdateBuilder.</returns>
		public SqlUpdateBuilder AddColumns(string[] columnsName, string val)
		{
			foreach (string columnName in columnsName)
				columns[columnName] = val;

			return this;
		}

		public virtual SqlUpdateBuilder AddColumn(string columnName, IType propertyType)
		{
			SqlType[] sqlTypes = propertyType.SqlTypes(Mapping);
			if (sqlTypes.Length > 1)
				throw new AssertionFailure("Adding one column for a composed IType.");
			columns[columnName] = sqlTypes[0];
			return this;
		}

		/// <summary>
		/// Adds the Property's columns to the UPDATE sql
		/// </summary>
		/// <param name="columnNames">An array of the column names for the Property</param>
		/// <param name="propertyType">The IType of the property.</param>
		/// <returns>The SqlUpdateBuilder.</returns>
		public SqlUpdateBuilder AddColumns(string[] columnNames, IType propertyType)
		{
			return AddColumns(columnNames, null, propertyType);
		}

		/// <summary>
		/// Adds the Property's updatable columns to the UPDATE sql
		/// </summary>
		/// <param name="columnNames">An array of the column names for the Property</param>
		/// <param name="updateable">An array of updatable column flags.  If this array is <c>null</c>, all supplied columns are considered updatable.</param>
		/// <param name="propertyType">The IType of the property.</param>
		/// <returns>The SqlUpdateBuilder.</returns>
		public SqlUpdateBuilder AddColumns(string[] columnNames, bool[] updateable, IType propertyType)
		{
			SqlType[] sqlTypes = propertyType.SqlTypes(Mapping);
			for (int i = 0; i < columnNames.Length; i++)
			{
				if (updateable == null || updateable[i])
				{
					if (i >= sqlTypes.Length)
						throw new AssertionFailure("Different columns and it's IType.");
					columns[columnNames[i]] = sqlTypes[i];
				}
			}

			return this;
		}

		public SqlUpdateBuilder AppendAssignmentFragment(SqlString fragment)
		{
			// SqlString is immutable
			assignments = assignments == null ? fragment : assignments.Append(", ").Append(fragment);
			return this;
		}

		public SqlUpdateBuilder SetWhere(string whereSql)
		{
			if (StringHelper.IsNotEmpty(whereSql))
			{
				whereStrings = new List<SqlString>(new[] { new SqlString(whereSql) });
			}
			return this;
		}

		/// <summary>
		/// Sets the IdentityColumn for the <c>UPDATE</c> sql to use.
		/// </summary>
		/// <param name="columnNames">An array of the column names for the Property</param>
		/// <param name="identityType">The IType of the Identity Property.</param>
		/// <returns>The SqlUpdateBuilder.</returns>
		public SqlUpdateBuilder SetIdentityColumn(string[] columnNames, IType identityType)
		{
			whereStrings.Add(ToWhereString(columnNames));
			whereParameterTypes.AddRange(identityType.SqlTypes(Mapping));
			return this;
		}

		/// <summary>
		/// Sets the VersionColumn for the <c>UPDATE</c> sql to use.
		/// </summary>
		/// <param name="columnNames">An array of the column names for the Property</param>
		/// <param name="versionType">The IVersionType of the Version Property.</param>
		/// <returns>The SqlUpdateBuilder.</returns>
		public SqlUpdateBuilder SetVersionColumn(string[] columnNames, IVersionType versionType)
		{
			whereStrings.Add(ToWhereString(columnNames));
			whereParameterTypes.AddRange(versionType.SqlTypes(Mapping));
			return this;
		}

		/// <summary>
		/// Adds the columns for the Type to the WhereFragment
		/// </summary>
		/// <param name="columnNames">The names of the columns to add.</param>
		/// <param name="type">The IType of the property.</param>
		/// <param name="op">The operator to put between the column name and value.</param>
		/// <returns>The SqlUpdateBuilder</returns>
		public SqlUpdateBuilder AddWhereFragment(string[] columnNames, IType type, string op)
		{
			if (columnNames.Length > 0)
			{
				// Don't add empty conditions - we get extra ANDs
				whereStrings.Add(ToWhereString(columnNames, op));
				whereParameterTypes.AddRange(type.SqlTypes(Mapping));
			}

			return this;
		}

		public SqlUpdateBuilder AddWhereFragment(string[] columnNames, SqlType[] types, string op)
		{
			if (columnNames.Length > 0)
			{
				// Don't add empty conditions - we get extra ANDs
				whereStrings.Add(ToWhereString(columnNames, op));
				whereParameterTypes.AddRange(types);
			}
			return this;
		}

		public SqlUpdateBuilder AddWhereFragment(string columnName, SqlType type, string op)
		{
			if (!string.IsNullOrEmpty(columnName))
			{
				whereStrings.Add(ToWhereString(columnName, op));
				whereParameterTypes.Add(type);
			}
			return this;
		}

		/// <summary>
		/// Adds a string to the WhereFragment
		/// </summary>
		/// <param name="whereSql">A well formed sql string with no parameters.</param>
		/// <returns>The SqlUpdateBuilder</returns>
		public SqlUpdateBuilder AddWhereFragment(string whereSql)
		{
			// Don't add empty conditions - we get extra ANDs
			if (!string.IsNullOrEmpty(whereSql))
			{
				whereStrings.Add(new SqlString(whereSql));
			}
			return this;
		}

		#region ISqlStringBuilder Members

		/// <summary></summary>
		public SqlString ToSqlString()
		{
			// 3 = "UPDATE", tableName, "SET"
			int initialCapacity = 3;

			// will have a comma for all but the first column, and then for each column
			// will have a name, " = ", value so multiply by 3
			if (columns.Count > 0)
			{
				initialCapacity += (columns.Count - 1) + (columns.Count * 3);
			}
			// 1 = "WHERE" 
			initialCapacity++;

			// the "AND" before all but the first whereString
			if (whereStrings.Count > 0)
			{
				initialCapacity += (whereStrings.Count - 1);
				foreach (SqlString whereString in whereStrings)
					initialCapacity += whereString.Count;
			}

			if (!string.IsNullOrEmpty(comment))
				initialCapacity++;

			var sqlBuilder = new SqlStringBuilder(initialCapacity + 2);
			if (!string.IsNullOrEmpty(comment))
				sqlBuilder.Add("/* " + comment + " */ ");

			sqlBuilder.Add("UPDATE ")
				.Add(tableName)
				.Add(" SET ");

			bool assignmentsAppended = false;
			bool commaNeeded = false;
			foreach (KeyValuePair<string, object> valuePair in columns)
			{
				if (commaNeeded)
					sqlBuilder.Add(StringHelper.CommaSpace);
				commaNeeded = true;


				sqlBuilder.Add(valuePair.Key)
					.Add(" = ");

				SqlType param = valuePair.Value as SqlType;
				if (param != null)
					sqlBuilder.Add(Parameter.Placeholder);
				else
					sqlBuilder.Add((string) valuePair.Value);
				assignmentsAppended = true;
			}
			if (assignments != null)
			{
				if (assignmentsAppended)
				{
					sqlBuilder.Add(", ");
				}
				sqlBuilder.Add(assignments);
			}

			sqlBuilder.Add(" WHERE ");
			bool andNeeded = false;
			foreach (SqlString whereString in whereStrings)
			{
				if (andNeeded)
					sqlBuilder.Add(" AND ");
				andNeeded = true;

				sqlBuilder.Add(whereString);
			}

			if (log.IsDebugEnabled)
			{
				if (initialCapacity < sqlBuilder.Count)
				{
					log.Debug(
						"The initial capacity was set too low at: " + initialCapacity + " for the UpdateSqlBuilder " +
						"that needed a capacity of: " + sqlBuilder.Count + " for the table " + tableName);
				}
				else if (initialCapacity > 16 && ((float) initialCapacity / sqlBuilder.Count) > 1.2)
				{
					log.Debug(
						"The initial capacity was set too high at: " + initialCapacity + " for the UpdateSqlBuilder " +
						"that needed a capacity of: " + sqlBuilder.Count + " for the table " + tableName);
				}
			}

			return sqlBuilder.ToSqlString();
		}

		#endregion

		public SqlCommandInfo ToSqlCommandInfo()
		{
			SqlString text = ToSqlString();
			List<SqlType> parameterTypes = new List<SqlType>(new SafetyEnumerable<SqlType>(columns.Values));
			parameterTypes.AddRange(whereParameterTypes);
			return new SqlCommandInfo(text, parameterTypes.ToArray());
		}
	}
}
