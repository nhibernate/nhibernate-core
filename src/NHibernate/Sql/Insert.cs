using System;
using System.Text;
using System.Collections;
using NHibernate.Util;
using NHibernate.Dialect;
using NHibernate.Type;

namespace NHibernate.Sql {
	/// <summary>
	/// A SQL <c>INSERT</c> statement
	/// </summary>
	public class Insert {
		private Dialect.Dialect dialect;
		private string tableName;
		private SortedList columns = new SortedList();

		public Insert(Dialect.Dialect dialect) {
			this.dialect = dialect;
		}

		public Insert AddColumn(string columnName) {
			return AddColumn(columnName, "?");
		}

		public Insert AddColumns(string[] columnNames) {
			for (int i=0; i<columnNames.Length; i++) {
				AddColumn(columnNames[i]);
			}
			return this;
		}

		public Insert AddColumn(string columnName, string value) {
			columns.Add(columnName, value);
			return this;
		}

		public Insert AddColumn(string columnName, object value, ILiteralType type) {
			return AddColumn(columnName, type.ObjectToSQLString(value) );
		}

		public Insert AddIdentityColumn(string columnName) {
			string value = dialect.IdentityInsertString;
			if (value!=null) AddColumn(columnName, value);
			return this;
		}

		public Insert SetTableName(string tableName) {
			this.tableName = tableName;
			return this;
		}

		public string ToStatementString() {
			StringBuilder buf = new StringBuilder( columns.Count*15 + tableName.Length + 10 );
			buf.Append("insert into ")
				.Append(tableName);
			if ( columns.Count == 0 ) {
				buf.Append(' ').Append( dialect.NoColumnsInsertString );
			} else {
				buf.Append(" (");
				int i=0;
				foreach(string key in columns.Keys) {
					i++;
					buf.Append( key );
					if (i<columns.Count-1) buf.Append(StringHelper.CommaSpace);
				}
				buf.Append(") values (");
				i = 0;
				foreach(string value in columns.Values) {
					i++;
					buf.Append( value );
					if (i<columns.Count-1) buf.Append(StringHelper.CommaSpace);
				}
				buf.Append(')');
			}
			return buf.ToString();
		}

		
	}
}
