using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Text;

using NHibernate.Connection;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.SqlCommand {

	/// <summary>
	/// A class that builds an <c>INSERT</c> sql statement.
	/// </summary>
	public class SqlInsertBuilder: ISqlStringBuilder	{

		ISessionFactoryImplementor factory;
		string tableName;
		IList columnNames = new ArrayList();
		IList columnValues = new ArrayList();

		//SortedList columnValues = new SortedList(); //key=columName, value=string/IParameter

		public SqlInsertBuilder(ISessionFactoryImplementor factory){
			this.factory = factory;
		}

		public SqlInsertBuilder SetTableName(string tableName) {
			this.tableName = tableName;
			return this;
		}

		/// <summary>
		/// Adds the Property's columns to the INSERT sql
		/// </summary>
		/// <param name="columnNames">An array of the column names for the Property</param>
		/// <param name="propertyType">The IType of the property.</param>
		/// <returns>The SqlInsertBuilder.</returns>
		public SqlInsertBuilder AddColumn(string[] columnNames, IType propertyType) {
			Parameter[] parameters = Parameter.GenerateParameters(factory, columnNames, propertyType);

			for(int i = 0; i < columnNames.Length; i++) {
				this.columnNames.Add(columnNames[i]);
				columnValues.Add(parameters[i]);
			}
			
			return this;
		}

		/// <summary>
		/// Add a column with a specific value to the INSERT sql
		/// </summary>
		/// <param name="columnName">The name of the Column to add.</param>
		/// <param name="val">The value to set for the column.</param>
		/// <param name="literalType">The NHibernateType to use to convert the value to a sql string.</param>
		/// <returns>The SqlInsertBuilder.</returns>
		public SqlInsertBuilder AddColumn(string columnName, object val, ILiteralType literalType) {
			return AddColumn(columnName, literalType.ObjectToSQLString(val));
		}


		/// <summary>
		/// Add a column with a specific value to the INSERT sql
		/// </summary>
		/// <param name="columnName">The name of the Column to add.</param>
		/// <param name="val">A valid sql string to set as the value of the column.</param>
		/// <returns>The SqlInsertBuilder.</returns>
		public SqlInsertBuilder AddColumn(string columnName, string val) {
			
			columnNames.Add(columnName);
			columnValues.Add(val);
			
			return this;
		}
		
		

		#region ISqlStringBuilder Members

		public SqlString ToSqlString() {
			//TODO: add a default capacity
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();
			
			sqlBuilder.Add("INSERT INTO ")
				.Add(tableName);

			
			if(columnNames.Count == 0 ) {
				sqlBuilder.Add(" ").Add( factory.Dialect.NoColumnsInsertString );
			} 
			else {
				sqlBuilder.Add(" (");

				// do we need a comma before we add the column to the INSERT list
				// when we get started the first column doesn't need one.
				bool commaNeeded = false;
					
				foreach(string columnName in columnNames){

					// build up the column list
					if(commaNeeded) sqlBuilder.Add(StringHelper.CommaSpace);
					sqlBuilder.Add(columnName);
					commaNeeded = true;
					
				}

				sqlBuilder.Add( ") VALUES (");

				commaNeeded = false;
				
				foreach(object obj in columnValues) {

					if(commaNeeded) sqlBuilder.Add(StringHelper.CommaSpace);
					commaNeeded = true;

					Parameter param = obj as Parameter;
					if(param!=null) {
						sqlBuilder.Add(param);
					}
					else {
						sqlBuilder.Add((string)obj);
					}

					
	
				}

				sqlBuilder.Add(")");
	

			}

			return sqlBuilder.ToSqlString();


		}

		#endregion
	}
}
