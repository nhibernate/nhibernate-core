using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Text;

using NHibernate.Connection;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.SqlCommand 
{
	/// <summary>
	/// A class that builds an <c>INSERT</c> sql statement.
	/// </summary>
	public class SqlSelectBuilder: SqlBaseBuilder, ISqlStringBuilder	 
	{
		private string selectClause;
		private string fromClause;
		private SqlString outerJoinsAfterFrom;
		private SqlString outerJoinsAfterWhere;
		private string orderByClause;

		IList whereSqlStrings = new ArrayList();

		public SqlSelectBuilder(ISessionFactoryImplementor factory): base(factory)
		{
			
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
			this.fromClause = tableName + " " + alias;
			return this;
		}

		/// <summary>
		/// Sets the text that should appear after the ORDER BY.
		/// </summary>
		/// <param name="orderByClause">The orderByClause to set</param>
		/// <returns>The SqlSelectBuilder</returns>
		public SqlSelectBuilder SetOrderByClause(string orderByClause) 
		{
			this.orderByClause = orderByClause;
			return this;
		}

		// TODO: figure out if OuterJoins can ever use the ? operator - I don't know
		// why they would be able to - but you never know.
		/// <summary>
		/// Sets the SQL for the OUTER JOINs.  
		/// </summary>
		/// <remarks>
		/// All of the SQL needs to be included in the SELECT.  No OUTER JOINS will automatically be
		/// added.
		/// </remarks>
		/// <param name="outerJoinsAfterFrom">The outerJoinsAfterFrom to set</param>
		/// <param name="outerJoinsAfterWhere">The outerJoinsAfterWhere to set</param>
		/// <returns>The SqlSelectBuilder</returns>
		[Obsolete("Should use SqlString version instead")]
		public SqlSelectBuilder SetOuterJoins(string outerJoinsAfterFrom, string outerJoinsAfterWhere) 
		{
			return this.SetOuterJoins( new SqlString(outerJoinsAfterFrom), new SqlString(outerJoinsAfterWhere) );
		}

		[Obsolete("Should use SqlString version instead")]
		public SqlSelectBuilder SetOuterJoins(SqlString outerJoinsAfterFrom, string outerJoinsAfterWhere) 
		{
			return this.SetOuterJoins( outerJoinsAfterFrom, new SqlString(outerJoinsAfterWhere) );
		}

		[Obsolete("Should use SqlString version instead")]
		public SqlSelectBuilder SetOuterJoins(string outerJoinsAfterFrom, SqlString outerJoinsAfterWhere) 
		{
			return this.SetOuterJoins( new SqlString(outerJoinsAfterFrom), outerJoinsAfterWhere );
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
			this.outerJoinsAfterWhere = outerJoinsAfterWhere;
			return this;
		}

		/// <summary>
		/// Sets the text for the SELECT
		/// </summary>
		/// <param name="selectClause">The selectClause to set</param>
		/// <returns>The SqlSelectBuilder</returns>
		public SqlSelectBuilder SetSelectClause(string selectClause) 
		{
			this.selectClause = selectClause;
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
			Parameter[] parameters = Parameter.GenerateParameters(factory, tableAlias, columnNames, whereType);

			whereSqlStrings.Add(ToWhereString(tableAlias, columnNames, parameters));

			return this;

		}

		/// <summary>
		/// Adds a prebuilt SqlString to the Where clause
		/// </summary>
		/// <param name="whereSqlString">The SqlString that contains the sql and parameters to add to the WHERE</param>
		/// <returns>This SqlSelectBuilder</returns>
		public SqlSelectBuilder AddWhereClause(SqlString whereSqlString) 
		{
			whereSqlStrings.Add(whereSqlString);
			return this;
		}


		#region ISqlStringBuilder Members

		public SqlString ToSqlString()
		{

			//TODO: set a default capacity
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();

			sqlBuilder.Add("SELECT ")
				.Add(selectClause)
				.Add(" FROM ")
				.Add(fromClause)
				.Add(outerJoinsAfterFrom);

			sqlBuilder.Add(" WHERE ");

			if(whereSqlStrings.Count > 1) 
			{
				sqlBuilder.Add(
					(SqlString[])((ArrayList)whereSqlStrings).ToArray(typeof(SqlString)), 
					null, "AND", null, false);
			}
			else 
			{
				sqlBuilder.Add((SqlString)whereSqlStrings[0], null, null, null, false);
			}

			sqlBuilder.Add(outerJoinsAfterWhere);
			
			if (orderByClause != null && orderByClause.Trim().Length > 0) 
			{
				sqlBuilder.Add(" ORDER BY ")
					.Add(orderByClause);
			}

			return sqlBuilder.ToSqlString();

		}
		#endregion

	}
}
