using System;
using System.Collections;
using System.Text;

using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Util;
using NHibernate.Type;


namespace NHibernate.SqlCommand
{
	/// <summary>
	/// A class that builds an <c>UPDATE</c> sql statement.
	/// </summary>
	public class SqlUpdateBuilder: SqlBaseBuilder, ISqlStringBuilder {
		
		string tableName;

		IList columnNames = new ArrayList(); // name of the column
		IList columnValues = new ArrayList();  //string or a Parameter

		int identityFragmentIndex = -1;
		int versionFragmentIndex = -1;

		IList whereStrings = new ArrayList();

		public SqlUpdateBuilder(ISessionFactoryImplementor factory) : base(factory) {
			
		}

		public SqlUpdateBuilder SetTableName(string tableName) {
			this.tableName = tableName;
			return this;
		}

		

		/// <summary>
		/// Add a column with a specific value to the INSERT sql
		/// </summary>
		/// <param name="columnName">The name of the Column to add.</param>
		/// <param name="val">The value to set for the column.</param>
		/// <param name="literalType">The NHibernateType to use to convert the value to a sql string.</param>
		/// <returns>The SqlUpdateBuilder.</returns>
		public SqlUpdateBuilder AddColumn(string columnName, object val, ILiteralType literalType) {
			return AddColumn(columnName, literalType.ObjectToSQLString(val));
		}


		/// <summary>
		/// Add a column with a specific value to the INSERT sql
		/// </summary>
		/// <param name="columnName">The name of the Column to add.</param>
		/// <param name="val">A valid sql string to set as the value of the column.</param>
		/// <returns>The SqlUpdateBuilder.</returns>
		public SqlUpdateBuilder AddColumn(string columnName, string val) {
			
			columnNames.Add(columnName);
			columnValues.Add(val);
			
			return this;
		}

		/// <summary>
		/// Adds columns with a specific value to the INSERT sql
		/// </summary>
		/// <param name="columnName">The names of the Column sto add.</param>
		/// <param name="val">A valid sql string to set as the value of the column.</param>
		/// <returns>The SqlUpdateBuilder.</returns>
		public SqlUpdateBuilder AddColumns(string[] columnName, string val) {
			
			for(int i = 0; i < columnName.Length; i++) {
				columnNames.Add(columnName[i]);
				columnValues.Add(val);
			}
			
			return this;
		}

		/// <summary>
		/// Adds the Property's columns to the UPDATE sql
		/// </summary>
		/// <param name="columnNames">An array of the column names for the Property</param>
		/// <param name="propertyType">The IType of the property.</param>
		/// <returns>The SqlUpdateBuilder.</returns>
		public SqlUpdateBuilder AddColumns(string[] columnNames, IType propertyType) {
			Parameter[] parameters = Parameter.GenerateParameters(factory, columnNames, propertyType);

			for(int i = 0; i < columnNames.Length; i++) {
				this.columnNames.Add(columnNames[i]);
				columnValues.Add(parameters[i]);
			}
			
			return this;
		}
		
		/// <summary>
		/// Sets the IdentityColumn for the <c>UPDATE</c> sql to use.
		/// </summary>
		/// <param name="columnNames">An array of the column names for the Property</param>
		/// <param name="identityType">The IType of the Identity Property.</param>
		/// <returns>The SqlUpdateBuilder.</returns>
		public SqlUpdateBuilder SetIdentityColumn(string[] columnNames, IType identityType) {
			Parameter[] parameters = Parameter.GenerateParameters(factory, columnNames, identityType);

			identityFragmentIndex = whereStrings.Add(ToWhereString(columnNames, parameters));

			return this;
		}

		/// <summary>
		/// Sets the VersionColumn for the <c>UPDATE</c> sql to use.
		/// </summary>
		/// <param name="columnNames">An array of the column names for the Property</param>
		/// <param name="versionType">The IVersionType of the Version Property.</param>
		/// <returns>The SqlUpdateBuilder.</returns>
		public SqlUpdateBuilder SetVersionColumn(string[] columnNames, IVersionType versionType) {
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
		/// <returns>The SqlUpdateBuilder</returns>
		public SqlUpdateBuilder AddWhereFragment(string[] columnNames, IType type, string op) {
			Parameter[] parameters = Parameter.GenerateParameters(factory, columnNames, type);
			whereStrings.Add(ToWhereString(columnNames, parameters, op));

			return this;
		}
		

		#region ISqlStringBuilder Members

		public SqlString ToSqlString() {
			// TODO:  Add default capacity
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();

			bool commaNeeded = false;
			bool andNeeded = false;

			
			sqlBuilder.Add("UPDATE ")
				.Add(tableName)
				.Add(" SET ");

			for(int i = 0; i < columnNames.Count; i++){
				
				if(commaNeeded) sqlBuilder.Add(StringHelper.CommaSpace);
				commaNeeded = true;

				string columnName = (string)columnNames[i];
				object columnValue = columnValues[i];

				sqlBuilder.Add(columnName)
					.Add(" = ");
				
				Parameter param = columnValue as Parameter;
				if(param!=null) {
					sqlBuilder.Add(param);
				}
				else {
					sqlBuilder.Add((string)columnValue);
				}

			}
			
			sqlBuilder.Add(" WHERE ");

			foreach(SqlString whereString in whereStrings) {
				if(andNeeded) sqlBuilder.Add(" AND ");
				andNeeded = true;

				sqlBuilder.Add(whereString, null, null, null, false);

			}

			return sqlBuilder.ToSqlString();
		}

		#endregion
	}
}
