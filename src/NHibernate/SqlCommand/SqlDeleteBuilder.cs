using System.Collections.Generic;
using log4net;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// A class that builds an <c>DELETE</c> sql statement.
	/// </summary>
	public class SqlDeleteBuilder : SqlBaseBuilder, ISqlStringBuilder
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(SqlDeleteBuilder));
		private string tableName;

		private readonly List<SqlString> whereStrings = new List<SqlString>();
		private readonly List<SqlType> parameterTypes = new List<SqlType>();
		private string comment;

		public SqlDeleteBuilder(Dialect.Dialect dialect, IMapping mapping)
			: base(dialect, mapping) {}

		public SqlDeleteBuilder SetTableName(string tableName)
		{
			this.tableName = tableName;
			return this;
		}

		public SqlDeleteBuilder SetComment(string comment)
		{
			this.comment = comment;
			return this;
		}

		/// <summary>
		/// Sets the IdentityColumn for the <c>DELETE</c> sql to use.
		/// </summary>
		/// <param name="columnNames">An array of the column names for the Property</param>
		/// <param name="identityType">The IType of the Identity Property.</param>
		/// <returns>The SqlDeleteBuilder.</returns>
		public SqlDeleteBuilder SetIdentityColumn(string[] columnNames, IType identityType)
		{
			whereStrings.Add(ToWhereString(columnNames));
			parameterTypes.AddRange(identityType.SqlTypes(Mapping));
			return this;
		}

		/// <summary>
		/// Sets the VersionColumn for the <c>DELETE</c> sql to use.
		/// </summary>
		/// <param name="columnNames">An array of the column names for the Property</param>
		/// <param name="versionType">The IVersionType of the Version Property.</param>
		/// <returns>The SqlDeleteBuilder.</returns>
		public SqlDeleteBuilder SetVersionColumn(string[] columnNames, IVersionType versionType)
		{
			whereStrings.Add(ToWhereString(columnNames));
			parameterTypes.AddRange(versionType.SqlTypes(Mapping));
			return this;
		}

		/// <summary>
		/// Adds the columns for the Type to the WhereFragment
		/// </summary>
		/// <param name="columnNames">The names of the columns to add.</param>
		/// <param name="type">The IType of the property.</param>
		/// <param name="op">The operator to put between the column name and value.</param>
		/// <returns>The SqlDeleteBuilder</returns>
		public SqlDeleteBuilder AddWhereFragment(string[] columnNames, IType type, string op)
		{
			whereStrings.Add(ToWhereString(columnNames, op));
			parameterTypes.AddRange(type.SqlTypes(Mapping));
			return this;
		}

		public SqlDeleteBuilder AddWhereFragment(string[] columnNames, SqlType[] types, string op)
		{
			whereStrings.Add(ToWhereString(columnNames, op));
			parameterTypes.AddRange(types);
			return this;
		}


		public SqlDeleteBuilder AddWhereFragment(string columnName, SqlType type, string op)
		{
			if (!string.IsNullOrEmpty(columnName))
			{
				whereStrings.Add(ToWhereString(columnName, op));
				parameterTypes.Add(type);
			}
			return this;
		}
		/// <summary>
		/// Adds a string to the WhereFragement
		/// </summary>
		/// <param name="whereSql">A well formed sql statement with no parameters.</param>
		/// <returns>The SqlDeleteBuilder</returns>
		public SqlDeleteBuilder AddWhereFragment(string whereSql)
		{
			if (StringHelper.IsNotEmpty(whereSql))
				whereStrings.Add(new SqlString(whereSql));

			return this;
		}

		#region ISqlStringBuilder Members

		public SqlString ToSqlString()
		{
			// will for sure have 3 parts and then each item in the WhereStrings
			int initialCapacity = 3;

			// add an "AND" for each whereString except the first one.
			initialCapacity += (whereStrings.Count - 1);

			for (int i = 0; i < whereStrings.Count; i++)
				initialCapacity += whereStrings[i].Count;

			if (!string.IsNullOrEmpty(comment))
				initialCapacity++;

			SqlStringBuilder sqlBuilder = new SqlStringBuilder(initialCapacity + 2);
			if (!string.IsNullOrEmpty(comment))
				sqlBuilder.Add("/* " + comment + " */ ");

			sqlBuilder.Add("DELETE FROM ")
				.Add(tableName)
				.Add(" WHERE ");

			if (whereStrings.Count > 1)
			{
				sqlBuilder.Add(whereStrings.ToArray(), null, "AND", null, false);
			}
			else
			{
				sqlBuilder.Add(whereStrings[0]);
			}

			if (log.IsDebugEnabled)
			{
				if (initialCapacity < sqlBuilder.Count)
				{
					log.Debug(
						"The initial capacity was set too low at: " + initialCapacity + " for the DeleteSqlBuilder " +
						"that needed a capacity of: " + sqlBuilder.Count + " for the table " + tableName);
				}
				else if (initialCapacity > 16 && ((float) initialCapacity / sqlBuilder.Count) > 1.2)
				{
					log.Debug(
						"The initial capacity was set too high at: " + initialCapacity + " for the DeleteSqlBuilder " +
						"that needed a capacity of: " + sqlBuilder.Count + " for the table " + tableName);
				}
			}
			return sqlBuilder.ToSqlString();
		}

		#endregion

		public SqlCommandInfo ToSqlCommandInfo()
		{
			return new SqlCommandInfo(ToSqlString(), parameterTypes.ToArray());
		}
	}
}