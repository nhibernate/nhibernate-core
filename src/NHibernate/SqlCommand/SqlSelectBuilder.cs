
using System;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// Builds a <c>SELECT</c> SQL statement.
	/// </summary>
	public class SqlSelectBuilder : SqlBaseBuilder, ISqlStringBuilder
	{
		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(SqlSelectBuilder));

		private SqlString selectClause;
		private string fromClause;
		private SqlString outerJoinsAfterFrom;
		private SqlString whereClause;
		private SqlString outerJoinsAfterWhere;
		private SqlString orderByClause;
		private SqlString groupByClause;
		private SqlString havingClause;
		private LockMode lockMode;
		private string mainTableAlias;
		private string comment;

		public SqlSelectBuilder(ISessionFactoryImplementor factory)
			: base(factory.Dialect, factory) {}

		public SqlSelectBuilder SetComment(string comment)
		{
			this.comment = comment;
			return this;
		}

		/// <summary>
		/// Sets the text that should appear after the FROM 
		/// </summary>
		/// <param name="fromClause">The fromClause to set</param>
		/// <returns>The SqlSelectBuilder</returns>
		public SqlSelectBuilder SetFromClause(string fromClause)
		{
			this.fromClause = fromClause;
			return this;
		}

		/// <summary>
		/// Sets the text that should appear after the FROM 
		/// </summary>
		/// <param name="tableName">The name of the Table to get the data from</param>
		/// <param name="alias">The Alias to use for the table name.</param>
		/// <returns>The SqlSelectBuilder</returns>
		public SqlSelectBuilder SetFromClause(string tableName, string alias)
		{
			fromClause = tableName + " " + alias;
			return this;
		}

		/// <summary>
		/// Sets the text that should appear after the FROM
		/// </summary>
		/// <param name="fromClause">The fromClause in a SqlString</param>
		/// <returns>The SqlSelectBuilder</returns>
		public SqlSelectBuilder SetFromClause(SqlString fromClause)
		{
			// it is safe to do this because a fromClause will have no parameters
			return SetFromClause(fromClause.ToString());
		}

		/// <summary>
		/// Sets the text that should appear after the ORDER BY.
		/// </summary>
		/// <param name="orderByClause">The orderByClause to set</param>
		/// <returns>The SqlSelectBuilder</returns>
		public SqlSelectBuilder SetOrderByClause(SqlString orderByClause)
		{
			this.orderByClause = orderByClause;
			return this;
		}

		/// <summary>
		/// Sets the text that should appear after the GROUP BY.
		/// </summary>
		/// <param name="groupByClause">The groupByClause to set</param>
		/// <returns>The SqlSelectBuilder</returns>
		public SqlSelectBuilder SetGroupByClause(SqlString groupByClause)
		{
			this.groupByClause = groupByClause;
			return this;
		}

		/// <summary>
		/// Sets the SqlString for the OUTER JOINs.  
		/// </summary>
		/// <remarks>
		/// All of the Sql needs to be included in the SELECT.  No OUTER JOINS will automatically be
		/// added.
		/// </remarks>
		/// <param name="outerJoinsAfterFrom">The outerJoinsAfterFrom to set</param>
		/// <param name="outerJoinsAfterWhere">The outerJoinsAfterWhere to set</param>
		/// <returns>The SqlSelectBuilder</returns>
		public SqlSelectBuilder SetOuterJoins(SqlString outerJoinsAfterFrom, SqlString outerJoinsAfterWhere)
		{
			this.outerJoinsAfterFrom = outerJoinsAfterFrom;

			SqlString tmpOuterJoinsAfterWhere = outerJoinsAfterWhere.Trim();
			if (tmpOuterJoinsAfterWhere.StartsWithCaseInsensitive("and"))
			{
				tmpOuterJoinsAfterWhere = tmpOuterJoinsAfterWhere.Substring(4);
			}

			this.outerJoinsAfterWhere = tmpOuterJoinsAfterWhere;
			return this;
		}

		/// <summary>
		/// Sets the text for the SELECT
		/// </summary>
		/// <param name="selectClause">The selectClause to set</param>
		/// <returns>The SqlSelectBuilder</returns>
		public SqlSelectBuilder SetSelectClause(SqlString selectClause)
		{
			this.selectClause = selectClause;
			return this;
		}

		/// <summary>
		/// Sets the text for the SELECT
		/// </summary>
		/// <param name="selectClause">The selectClause to set</param>
		/// <returns>The SqlSelectBuilder</returns>
		public SqlSelectBuilder SetSelectClause(string selectClause)
		{
			this.selectClause = new SqlString(selectClause);
			return this;
		}

		/// <summary>
		/// Sets the criteria to use for the WHERE.  It joins all of the columnNames together with an AND.
		/// </summary>
		/// <param name="tableAlias"></param>
		/// <param name="columnNames">The names of the columns</param>
		/// <param name="whereType">The Hibernate Type</param>
		/// <returns>The SqlSelectBuilder</returns>
		public SqlSelectBuilder SetWhereClause(string tableAlias, string[] columnNames, IType whereType)
		{
			return SetWhereClause(ToWhereString(tableAlias, columnNames));
		}

		/// <summary>
		/// Sets the prebuilt SqlString to the Where clause
		/// </summary>
		/// <param name="whereSqlString">The SqlString that contains the sql and parameters to add to the WHERE</param>
		/// <returns>This SqlSelectBuilder</returns>
		public SqlSelectBuilder SetWhereClause(SqlString whereSqlString)
		{
			whereClause = whereSqlString;
			return this;
		}

		/// <summary>
		/// Sets the criteria to use for the WHERE.  It joins all of the columnNames together with an AND.
		/// </summary>
		/// <param name="tableAlias"></param>
		/// <param name="columnNames">The names of the columns</param>
		/// <param name="whereType">The Hibernate Type</param>
		/// <returns>The SqlSelectBuilder</returns>
		public SqlSelectBuilder SetHavingClause(string tableAlias, string[] columnNames, IType whereType)
		{
			return SetHavingClause(ToWhereString(tableAlias, columnNames));
		}

		/// <summary>
		/// Sets the prebuilt SqlString to the Having clause
		/// </summary>
		/// <param name="havingSqlString">The SqlString that contains the sql and parameters to add to the HAVING</param>
		/// <returns>This SqlSelectBuilder</returns>
		public SqlSelectBuilder SetHavingClause(SqlString havingSqlString)
		{
			havingClause = havingSqlString;
			return this;
		}

		[Obsolete("For some DBMS's such as PostgreSQL, a lock on query with OUTER JOIN is not possible without specifying the not-null side. " +
				  "Use the new method SetLockMode(LockMode, mainTableAlias) instead.")]
		public SqlSelectBuilder SetLockMode(LockMode lockMode)
		{
			this.lockMode = lockMode;
			return this;
		}

		public SqlSelectBuilder SetLockMode(LockMode lockMode, string mainTableAlias)
		{
			this.lockMode = lockMode;
			this.mainTableAlias = mainTableAlias;
			return this;
		}

		#region ISqlStringBuilder Members

		/// <summary>
		/// ToSqlString() is named ToStatementString() in H3
		/// </summary>
		/// <returns></returns>
		public SqlString ToStatementString()
		{
			return ToSqlString();
		}

		/// <summary></summary>
		public SqlString ToSqlString()
		{
			// 4 = the "SELECT", selectClause, "FROM", fromClause are straight strings
			// plus the number of parts in outerJoinsAfterFrom SqlString.
			// 1 = the "WHERE" 
			// plus the number of parts in outerJoinsAfterWhere SqlString.
			// 1 = the whereClause
			// 2 = the "ORDER BY" and orderByClause
			var joinAfterFrom = outerJoinsAfterFrom != null ? outerJoinsAfterFrom.Count : 0;
			var joinAfterWhere = outerJoinsAfterWhere != null ? outerJoinsAfterWhere.Count : 0;
			int initialCapacity = 4 + joinAfterFrom + 1 + joinAfterWhere + 1 + 2;
			if (!string.IsNullOrEmpty(comment))
				initialCapacity++;

			SqlStringBuilder sqlBuilder = new SqlStringBuilder(initialCapacity + 2);
			if (!string.IsNullOrEmpty(comment))
				sqlBuilder.Add("/* " + comment + " */ ");

			sqlBuilder.Add("SELECT ")
				.Add(selectClause)
				.Add(" FROM ")
				.Add(fromClause);

			if (SqlStringHelper.IsNotEmpty(outerJoinsAfterFrom))
			{
				sqlBuilder.Add(outerJoinsAfterFrom);
			}

			if (SqlStringHelper.IsNotEmpty(whereClause) || SqlStringHelper.IsNotEmpty(outerJoinsAfterWhere))
			{
				sqlBuilder.Add(" WHERE ");
				// the outerJoinsAfterWhere needs to come before where clause to properly
				// handle dynamic filters
				if (SqlStringHelper.IsNotEmpty(outerJoinsAfterWhere))
				{
					sqlBuilder.Add(outerJoinsAfterWhere);
					if (SqlStringHelper.IsNotEmpty(whereClause))
					{
						sqlBuilder.Add(" AND ");
					}
				}

				if (SqlStringHelper.IsNotEmpty(whereClause))
				{
					sqlBuilder.Add(whereClause);
				}
			}

			if (SqlStringHelper.IsNotEmpty(groupByClause))
			{
				sqlBuilder.Add(" GROUP BY ")
					.Add(groupByClause);
			}

			if(SqlStringHelper.IsNotEmpty(havingClause))
			{
				sqlBuilder.Add(" HAVING ")
					.Add(havingClause);
			}

			if (SqlStringHelper.IsNotEmpty(orderByClause))
			{
				sqlBuilder.Add(" ORDER BY ")
					.Add(orderByClause);
			}

			if (lockMode != null)
			{
				sqlBuilder.Add(GetForUpdateString());
			}

			if (log.IsDebugEnabled())
			{
				if (initialCapacity < sqlBuilder.Count)
				{
					log.Debug("The initial capacity was set too low at: {0} for the SelectSqlBuilder that needed a capacity of: {1} for the table {2}",
					          initialCapacity,
					          sqlBuilder.Count,
					          fromClause);
				}
				else if (initialCapacity > 16 && ((float) initialCapacity / sqlBuilder.Count) > 1.2)
				{
					log.Debug("The initial capacity was set too high at: {0} for the SelectSqlBuilder that needed a capacity of: {1} for the table {2}",
					          initialCapacity,
					          sqlBuilder.Count,
					          fromClause);
				}
			}

			return sqlBuilder.ToSqlString();
		}

		private string GetForUpdateString()
		{
			if (!Dialect.SupportsOuterJoinForUpdate && HasOuterJoin())
			{
				var isUpgrade = Equals(lockMode, LockMode.Upgrade);
				var isUpgradeNoWait = !isUpgrade && (
					Equals(lockMode, LockMode.UpgradeNoWait) || Equals(lockMode, LockMode.Force));
				if (!isUpgrade && !isUpgradeNoWait)
					return string.Empty;

				if (!Dialect.SupportsForUpdateOf)
				{
					log.Warn(
						"Unsupported 'for update' case: 'for update' query with an outer join using a dialect not" +
						"supporting it and not supporting 'for update of' clause. Discarding 'for" +
						"update' clause.");
					return string.Empty;
				}

				if (Dialect.UsesColumnsWithForUpdateOf)
				{
					log.Warn(
						"Unimplemented 'for update' case: 'for update' query with an outer join using a dialect not" +
						"supporting it and requiring columns for its 'for update of' syntax. Discarding 'for" +
						"update' clause.");
					return string.Empty;
				}

				return isUpgrade ? Dialect.GetForUpdateString(mainTableAlias) : Dialect.GetForUpdateNowaitString(mainTableAlias);
			}

			return Dialect.GetForUpdateString(lockMode);

			bool HasOuterJoin() =>
				outerJoinsAfterFrom?.IsEmptyOrWhitespace() == false ||
				StringHelper.ContainsCaseInsensitive(fromClause, "outer join");
		}
		#endregion
	}
}
