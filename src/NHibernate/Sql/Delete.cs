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
		private Dialect.Dialect dialect;

		public Delete(Dialect.Dialect dialect) {
			this.dialect = dialect;
		}

		public Delete SetTableName(string tableName) {
			this.tableName = tableName;
			return this;
		}

		public string ToStatementString() {
			bool useNamedPars = dialect.UseNamedParameters;
			StringBuilder buf = new StringBuilder( tableName.Length + 10 );
			buf.Append("delete from ")
				.Append(tableName)
				.Append(" where ");
			if(useNamedPars) {
				for(int j=0; j<primaryKeyColumnNames.Length; j++) {
					buf.Append(primaryKeyColumnNames[j]);
					buf.Append('"');
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
			if(where!=null) {
				buf.Append(" and ")
					.Append(where);
			}
			if (versionColumnName != null) {
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

		public Delete SetWhere(string where) {
			this.where = where;
			return this;
		}

		public Delete SetPrimaryKeyColumnNames(params string[] primaryKeyColumnNames) {
			this.primaryKeyColumnNames = primaryKeyColumnNames;
			return this;
		}

		public Delete SetVersionColumnName(string versionColumnName) {
			this.versionColumnName = versionColumnName;
			return this;
		}
	}
}
