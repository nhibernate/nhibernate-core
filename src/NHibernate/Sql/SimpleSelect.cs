using System;
using System.Text;
using System.Collections;
using NHibernate.Util;

namespace NHibernate.Sql {

	/// <summary>
	/// An SQL <c>SELECT</c> statement
	/// </summary>
	public class SimpleSelect {
		private string tableName;
		private string orderBy;

		private IList columns = new ArrayList();
		private IDictionary aliases = new Hashtable();
		private IList whereTokens = new ArrayList();

		public SimpleSelect AddColumns(string[] columnNames, string[] aliases) {
			for (int i=0; i<columnNames.Length; i++) {
				AddColumn( columnNames[i], aliases[i] );
			}
			return this;
		}

		public SimpleSelect AddColumns(string[] columnNames) {
			for (int i=0; i<columnNames.Length; i++) {
				AddColumn( columnNames[i] );
			}
			return this;
		}

		public SimpleSelect AddColumn(string columnName) {
			columns.Add(columnName);
			return this;
		}

		public SimpleSelect AddColumn(string columnName, string alias) {
			columns.Add(columnName);
			aliases[columnName] = alias;
			return this;
		}

		public SimpleSelect SetTableName(string tableName) {
			this.tableName = tableName;
			return this;
		}

		public SimpleSelect AddWhereToken(string token) {
			whereTokens.Add(token);
			return this;
		}

		public SimpleSelect AddCondition(string lhs, string op, string rhs) {
			whereTokens.Add( lhs + ' ' + op + ' ' + rhs );
			return this;
		}

		public SimpleSelect AddCondition(string lhs, string condition) {
			whereTokens.Add( lhs + ' ' + condition );
			return this;
		}

		public SimpleSelect AddCondition(string[] lhs, string op, string[] rhs) {
			for (int i=0; i<lhs.Length; i++) {
				AddCondition(lhs[i], op, rhs[i]);
				if (i!=lhs.Length-1) whereTokens.Add("and");
			}
			return this;
		}
		public SimpleSelect AddCondition(string[] lhs, string condition) {
			for (int i=0; i<lhs.Length; i++) {
				AddCondition(lhs[i], condition);
				if (i!=lhs.Length-1) whereTokens.Add("and");
			}
			return this;
		}

		public string ToStatementString() {
			StringBuilder buf = new StringBuilder( columns.Count * 10 + tableName.Length + whereTokens.Count * 10 + 10 );
			buf.Append("select ");
			for (int i=0; i<columns.Count; i++) {
				string col = (string) columns[i];
				buf.Append(col);
				string alias = (string) aliases[col];
				if (alias!=null && !alias.Equals(col) ) {
					buf.Append(" as ")
						.Append(alias);
				}
				if (i<columns.Count-1) buf.Append(StringHelper.CommaSpace);
			}
			buf.Append(" from ")
				.Append(tableName);
			if ( whereTokens.Count > 0 ) {
				buf.Append(" where ")
					.Append( ToWhereClause() );
			}
			if (orderBy!=null) buf.Append(orderBy);
			return buf.ToString();
		}

		public string ToWhereClause() {
			StringBuilder buf = new StringBuilder( whereTokens.Count * 5 );
			for(int i=0; i<whereTokens.Count; i++) {
				buf.Append( whereTokens[i] );
				if (i<whereTokens.Count-1) buf.Append(' ');
			}
			return buf.ToString();
		}

		public void SetOrderBy(string orderBy) {
			this.orderBy = orderBy;
		}
	}
}
