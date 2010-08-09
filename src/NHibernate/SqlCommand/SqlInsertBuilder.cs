using System.Collections.Generic;

using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// A class that builds an <c>INSERT</c> sql statement.
	/// </summary>
	public class SqlInsertBuilder : ISqlStringBuilder
	{
		private static readonly ILogger log = LoggerProvider.LoggerFor(typeof(SqlInsertBuilder));

		private readonly ISessionFactoryImplementor factory;
		private string tableName;
		private string comment;

		// columns-> (ColumnName, Value) or (ColumnName, SqlType) for parametrized column
		private readonly LinkedHashMap<string, object> columns = new LinkedHashMap<string, object>();

		public SqlInsertBuilder(ISessionFactoryImplementor factory)
		{
			this.factory = factory;
		}

		protected internal Dialect.Dialect Dialect
		{
			get{return factory.Dialect;}
		}

		public virtual SqlInsertBuilder SetComment(string comment)
		{
			this.comment = comment;
			return this;
		}

		public SqlInsertBuilder SetTableName(string tableName)
		{
			this.tableName = tableName;
			return this;
		}

		/// <summary>
		/// Adds the Property's columns to the INSERT sql
		/// </summary>
		/// <param name="columnName">The column name for the Property</param>
		/// <param name="propertyType">The IType of the property.</param>
		/// <returns>The SqlInsertBuilder.</returns>
		/// <remarks>The column will be associated with a parameter.</remarks>
		public virtual SqlInsertBuilder AddColumn(string columnName, IType propertyType)
		{
			SqlType[] sqlTypes = propertyType.SqlTypes(factory);
			if (sqlTypes.Length > 1)
				throw new AssertionFailure("Adding one column for a composed IType.");
			columns[columnName] = sqlTypes[0];
			return this;
		}

		/// <summary>
		/// Add a column with a specific value to the INSERT sql
		/// </summary>
		/// <param name="columnName">The name of the Column to add.</param>
		/// <param name="val">The value to set for the column.</param>
		/// <param name="literalType">The NHibernateType to use to convert the value to a sql string.</param>
		/// <returns>The SqlInsertBuilder.</returns>
		public SqlInsertBuilder AddColumn(string columnName, object val, ILiteralType literalType)
		{
			return AddColumn(columnName, literalType.ObjectToSQLString(val, Dialect));
		}


		/// <summary>
		/// Add a column with a specific value to the INSERT sql
		/// </summary>
		/// <param name="columnName">The name of the Column to add.</param>
		/// <param name="val">A valid sql string to set as the value of the column.</param>
		/// <returns>The SqlInsertBuilder.</returns>
		public SqlInsertBuilder AddColumn(string columnName, string val)
		{
			columns[columnName] = val;
			return this;
		}

		public SqlInsertBuilder AddColumns(string[] columnNames, bool[] insertable, IType propertyType)
		{
			SqlType[] sqlTypes = propertyType.SqlTypes(factory);

			for (int i = 0; i < columnNames.Length; i++)
			{
				if (insertable == null || insertable[i])
				{
					if (i >= sqlTypes.Length)
						throw new AssertionFailure("Different columns and it's IType.");
					columns[columnNames[i]] = sqlTypes[i];
				}
			}

			return this;
		}

		public virtual SqlInsertBuilder AddIdentityColumn(string columnName)
		{
			string value = Dialect.IdentityInsertString;
			if (value != null)
			{
				AddColumn(columnName, value);
			}
			return this;
		}

		#region ISqlStringBuilder Members

		public virtual SqlString ToSqlString()
		{
			// 5 = "INSERT INTO", tableName, " (" , ") VALUES (", and ")"
			int initialCapacity = 5;

			// 2 = the first column is just the columnName and columnValue
			initialCapacity += 2;

			// eachColumn after the first one is 4 because of the ", ", columnName 
			// and the ", " columnValue
			if (columns.Count > 0)
			{
				initialCapacity += ((columns.Count - 1) * 4);
			}

			if (!string.IsNullOrEmpty(comment))
				initialCapacity++;

			SqlStringBuilder sqlBuilder = new SqlStringBuilder(initialCapacity + 2);
			if (!string.IsNullOrEmpty(comment))
			{
				sqlBuilder.Add("/* " + comment + " */ ");
			}

			sqlBuilder.Add("INSERT INTO ")
				.Add(tableName);

			if (columns.Count == 0)
			{
				sqlBuilder.Add(" ").Add(factory.Dialect.NoColumnsInsertString);
			}
			else
			{
				sqlBuilder.Add(" (");

				// do we need a comma before we add the column to the INSERT list
				// when we get started the first column doesn't need one.
				bool commaNeeded = false;
				foreach (string columnName in columns.Keys)
				{
					// build up the column list
					if (commaNeeded)
						sqlBuilder.Add(StringHelper.CommaSpace);
					commaNeeded = true;

					sqlBuilder.Add(columnName);
				}

				sqlBuilder.Add(") VALUES (");

				commaNeeded = false;
				foreach (object obj in columns.Values)
				{
					if (commaNeeded)
						sqlBuilder.Add(StringHelper.CommaSpace);
					commaNeeded = true;

					SqlType param = obj as SqlType;
					if (param != null)
						sqlBuilder.Add(Parameter.Placeholder);
					else
						sqlBuilder.Add((string) obj);
				}

				sqlBuilder.Add(")");
			}

			if (log.IsDebugEnabled)
			{
				if (initialCapacity < sqlBuilder.Count)
				{
					log.Debug(
						"The initial capacity was set too low at: " + initialCapacity + " for the InsertSqlBuilder " +
						"that needed a capacity of: " + sqlBuilder.Count + " for the table " + tableName);
				}
				else if (initialCapacity > 16 && ((float) initialCapacity / sqlBuilder.Count) > 1.2)
				{
					log.Debug(
						"The initial capacity was set too high at: " + initialCapacity + " for the InsertSqlBuilder " +
						"that needed a capacity of: " + sqlBuilder.Count + " for the table " + tableName);
				}
			}

			return sqlBuilder.ToSqlString();
		}

		#endregion

		public SqlCommandInfo ToSqlCommandInfo()
		{
			SqlString text = ToSqlString();
			return new SqlCommandInfo(text, GetParametersTypeArray());
		}

		public SqlType[] GetParametersTypeArray()
		{
			return (new List<SqlType>(new SafetyEnumerable<SqlType>(columns.Values))).ToArray();
		}
	}
}
