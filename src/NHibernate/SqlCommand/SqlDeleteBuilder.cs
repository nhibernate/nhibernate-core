using System;
using System.Collections;
using System.Text;

using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Util;
using NHibernate.Type;

namespace NHibernate.SqlCommand {
	/// <summary>
	/// A class that builds an <c>DELETE</c> sql statement.
	/// </summary>
	public class SqlDeleteBuilder: SqlBaseBuilder, ISqlStringBuilder {
		
		string tableName;

		int versionFragmentIndex = -1;
		int identityFragmentIndex = -1;

		IList whereStrings = new ArrayList();

		public SqlDeleteBuilder(ISessionFactoryImplementor factory): base(factory){
			
		}

		public SqlDeleteBuilder SetTableName(string tableName) {
			this.tableName = tableName;
			return this;
		}


		/// <summary>
		/// Sets the IdentityColumn for the <c>DELETE</c> sql to use.
		/// </summary>
		/// <param name="columnNames">An array of the column names for the Property</param>
		/// <param name="identityType">The IType of the Identity Property.</param>
		/// <returns>The SqlDeleteBuilder.</returns>
		public SqlDeleteBuilder SetIdentityColumn(string[] columnNames, IType identityType) {
			Parameter[] parameters = Parameter.GenerateParameters(factory, columnNames, identityType);

			identityFragmentIndex = whereStrings.Add(ToWhereString(columnNames, parameters));

			return this;
		}

		/// <summary>
		/// Sets the VersionColumn for the <c>DELETE</c> sql to use.
		/// </summary>
		/// <param name="columnNames">An array of the column names for the Property</param>
		/// <param name="versionType">The IVersionType of the Version Property.</param>
		/// <returns>The SqlDeleteBuilder.</returns>
		public SqlDeleteBuilder SetVersionColumn(string[] columnNames, IVersionType versionType) {
			Parameter[] parameters = Parameter.GenerateParameters(factory, columnNames, versionType);

			versionFragmentIndex = whereStrings.Add(ToWhereString(columnNames, parameters));

			return this;
		}

		/// <summary>
		/// Adds the columns for the Type to the WhereFragment
		/// </summary>
		/// <param name="columnNames">The names of the columns to add.</param>
		/// <param name="type">The IType of the property.</param>
		/// <param name="op">The operator to put between the column name and value.</param>
		/// <returns>The SqlDeleteBuilder</returns>
		public SqlDeleteBuilder AddWhereFragment(string[] columnNames, IType type, string op) {
			Parameter[] parameters = Parameter.GenerateParameters(factory, columnNames, type);
			whereStrings.Add(ToWhereString(columnNames, parameters, op));

			return this;
		}


		#region ISqlStringBuilder Members

		public SqlString ToSqlString() {

			// TODO: add default capacity
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();
			
			sqlBuilder.Add("DELETE FROM ")
				.Add(tableName)
				.Add(" WHERE ");	
			
			if(whereStrings.Count > 1) {
				sqlBuilder.Add(
					(SqlString[])((ArrayList)whereStrings).ToArray(typeof(SqlString)), 
					null, "AND", null, false);
			}
			else {
				sqlBuilder.Add((SqlString)whereStrings[0], null, null, null, false) ;
			}

			return sqlBuilder.ToSqlString();
		}

		#endregion
	}
}
