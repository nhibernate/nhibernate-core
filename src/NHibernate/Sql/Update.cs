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

		private SortedList columns = new SortedList();
 
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
			return AddColumn(columnName, "?");
		}

		public Update AddColumn(string columnName, string value) {
			columns.Add(columnName, value);
			return this;
		}

		public Update AddColumn(string columnName, object value, ILiteralType type) {
			return AddColumn(columnName, type.ObjectToSQLString(value) );
		}

		public Update SetTableName(string tableName) {
			this.tableName = tableName;
			return this;
		}

		public string ToStatementString() {
			StringBuilder buf = new StringBuilder( columns.Count * 15 + tableName.Length + 10 );
			buf.Append("update ")
				.Append(tableName)
				.Append(" set ");
			int i=0;
			foreach(string key in columns.Keys) {
				i++;
				buf.Append( key )
					.Append('=')
					.Append(columns[key]);
				if (i<columns.Count-1) buf.Append(StringHelper.CommaSpace);
			}
			buf.Append(" where ")
				.Append( string.Join("=? and ", primaryKeyColumnNames) )
				.Append("=?");
			if (versionColumnName!=null) {
				buf.Append(" and ")
					.Append(versionColumnName)
					.Append("=?");
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
