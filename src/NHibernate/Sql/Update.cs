using System;
using System.Text;
using System.Collections;
using NHibernate.Util;
using NHibernate.Type;


namespace NHibernate.Sql {
	/// <summary>
	/// An SQL <c>UPDATE</c> statement
	/// </summary>
	public class Update {
		private string tableName;
		private string[] primaryKeyColumnNames;
		private string versionColumnName;
		private string where;
		private Dialect.Dialect dialect;

		private SortedList columns = new SortedList();

		#region Hack parameters: need original order, refactor the entire approach
		private ArrayList columnOrder = new ArrayList(); 
		#endregion

		public Update(Dialect.Dialect dialect) 
		{
			this.dialect = dialect;
		}
 
		public Update AddColumns(string[] columnNames) {
			for (int i=0; i<columnNames.Length; i++ ) {
				AddColumn( columnNames[i] );
			}
			return this;
		}

		public Update AddColumns(string[] columnNames, string value) {
			for (int i=0; i<columnNames.Length; i++) {
				AddColumn(columnNames[i], value);
			}
			return this;
		}

		public Update AddColumn(string columnName) {
			if(dialect.UseNamedParameters)
				return AddColumn(columnName, dialect.NamedParametersPrefix + columnName);
			else 
				return AddColumn(columnName, "?");
		}

		public Update AddColumn(string columnName, string value) {
			columns.Add(columnName, value);
			columnOrder.Add(columnName);
			return this;
		}

		public Update AddColumn(string columnName, object value, ILiteralType type) {
			return AddColumn(columnName, type.ObjectToSQLString(value) );
		}

		public Update SetTableName(string tableName) {
			this.tableName = tableName;
			return this;
		}

		public Update SetWhere(string where) {
			this.where = where;
			return this;
		}

		public string ToStatementString() {
			bool useNamedPars = dialect.UseNamedParameters;
			StringBuilder buf = new StringBuilder( columns.Count * 15 + tableName.Length + 10 );
			buf.Append("update ")
				.Append(tableName)
				.Append(" set ");
			int i=0;
			foreach(string key in columnOrder) {
				i++;
				buf.Append( key )
					.Append('=')
					.Append(columns[key]);
				if (i<=columns.Count-1) buf.Append(StringHelper.CommaSpace);
			}
			buf.Append(" where ");
			if(useNamedPars) {
				for(int j=0; j<primaryKeyColumnNames.Length; j++) {
					buf.Append(primaryKeyColumnNames[j]);
					buf.Append('=');
					buf.Append(dialect.NamedParametersPrefix);
					buf.Append(primaryKeyColumnNames[j]);
					if( j==primaryKeyColumnNames.Length-1) break;
					buf.Append(" and ");
				}
			}
			else {
				buf.Append( string.Join("=? and ", primaryKeyColumnNames) )
					.Append("=?");
			}
				
			if (where!=null) {
				buf.Append(" and ")
					.Append(where);
			}
			if (versionColumnName!=null) {
				buf.Append(" and ")
					.Append(versionColumnName);
				if(useNamedPars) {
					buf.Append("=")
						.Append(dialect.NamedParametersPrefix)
						.Append(versionColumnName);
				}
				else {
					buf.Append("=?");
				}
			}
			return buf.ToString();
		}

		public Update SetPrimaryKeyColumnNames(string[] primaryKeyColumnNames) {
			this.primaryKeyColumnNames = primaryKeyColumnNames;
			return this;
		}

		public Update SetVersionColumnName(string versionColumnName) {
			this.versionColumnName = versionColumnName;
			return this;
		}
	}
}
