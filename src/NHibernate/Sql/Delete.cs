using System;
using System.Text;
using NHibernate.Util;

namespace NHibernate.Sql {

	/// <summary>
	/// An SQL <c>DELETE</c> statement
	/// </summary>
	public class Delete {
		private string tableName;
		private string[] primaryKeyColumnNames;
		private string versionColumnName;
		private string where;

		public Delete SetTableName(string tableName) {
			this.tableName = tableName;
			return this;
		}

		public string ToStatementString() {
			StringBuilder buf = new StringBuilder( tableName.Length + 10 );
			buf.Append("delete from ")
				.Append(tableName)
				.Append(" where ")
				.Append( string.Join("=? and ", primaryKeyColumnNames) )
				.Append("=?");
			if (versionColumnName != null) {
				buf.Append(" and ")
					.Append(versionColumnName)
					.Append("=?");
				if(where!=null) {
					buf.Append(" and ")
						.Append(where);
				}
			}
			return buf.ToString();
		}

		public Delete SetWhere(string where) {
			this.where = where;
			return this;
		}

		public Delete SetPrimaryKeyColumnNames(string[] primaryKeyColumnNames) {
			this.primaryKeyColumnNames = primaryKeyColumnNames;
			return this;
		}

		public Delete SetVersionColumnName(string versionColumnName) {
			this.versionColumnName = versionColumnName;
			return this;
		}
	}
}
