using System;
using System.Text;

using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.SqlCommand{
	/// <summary>
	/// The base class for all of the SqlBuilders.
	/// </summary>
	public abstract class SqlBaseBuilder {

		protected ISessionFactoryImplementor factory;

		public SqlBaseBuilder(ISessionFactoryImplementor factory){
			this.factory = factory;
		}

		/// <summary>
		/// Converts the ColumnNames and ColumnValues to a WhereFragment
		/// </summary>
		/// <param name="columnNames">The names of the Columns to Add to the WhereFragment</param>
		/// <param name="columnValues">The Values for the Columns in the WhereFragment</param>
		/// <returns>A SqlString that contains the WhereFragment</returns>
		/// <remarks>This just calls the overloaded ToWhereFragment() with the operator as " = " and the tableAlias null.</remarks>
		protected SqlString ToWhereString(string[] columnNames, object[] columnValues) {
			return ToWhereString(columnNames, columnValues, " = ");
		}

		/// <summary>
		/// Converts the ColumnNames and ColumnValues to a WhereFragment
		/// </summary>
		/// <param name="tableAlias">The Alias for the Table.</param>
		/// <param name="columnNames">The names of the Columns to Add to the WhereFragment</param>
		/// <param name="columnValues">The Values for the Columns in the WhereFragment</param>
		/// <returns>A SqlString that contains the WhereFragment</returns>
		/// <remarks>This defaults the op to " = "</remarks>
		protected SqlString ToWhereString(string tableAlias, string[] columnNames, object[] columnValues) {
			return ToWhereString(tableAlias, columnNames, columnValues, " = ");
		}

		/// <summary>
		/// Converts the ColumnNames and ColumnValues to a WhereFragment
		/// </summary>
		/// <param name="columnNames">The names of the Columns to Add to the WhereFragment</param>
		/// <param name="columnValues">The Values for the Columns in the WhereFragment</param>
		/// <param name="op">The operator to use between the names & values.  For example " = " or "!="</param>
		/// <returns>A SqlString that contains the WhereFragment</returns>
		protected SqlString ToWhereString(string[] columnNames, object[] columnValues, string op) {
			return ToWhereString(null, columnNames, columnValues, op);
		}

		/// <summary>
		/// Converts the ColumnNames and ColumnValues to a WhereFragment
		/// </summary>
		/// <param name="tableAlias">The Alias for the Table.</param>
		/// <param name="columnNames">The names of the Columns to Add to the WhereFragment</param>
		/// <param name="columnValues">The Values for the Columns in the WhereFragment</param>
		/// <param name="op">The operator to use between the names & values.  For example " = " or "!="</param>
		/// <returns>A SqlString that contains the WhereFragment</returns>
		protected SqlString ToWhereString(string tableAlias, string[] columnNames, object[] columnValues, string op) {
			SqlStringBuilder sqlBuilder = new SqlStringBuilder((columnNames.Length * 2) + 5);

			bool andNeeded = false;
			
			for(int i = 0; i < columnNames.Length; i++){
				if(andNeeded) sqlBuilder.Add(" AND ");
				andNeeded = true;

				string columnName;
				if(tableAlias!=null && tableAlias!=String.Empty) {
					columnName = tableAlias + StringHelper.Dot + columnNames[i];
				}
				else {
					columnName = columnNames[i];
				}

				sqlBuilder.Add(columnName)
					.Add(op);
				if(columnValues[i] is Parameter) {
					sqlBuilder.Add((Parameter)columnValues[i]);
				}
				else {
					sqlBuilder.Add((string)columnValues[i]);
				}
			}

			return sqlBuilder.ToSqlString();

		}

	}
}
