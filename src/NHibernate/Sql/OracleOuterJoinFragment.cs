using System;
using System.Text;
using NHibernate.Util;

namespace NHibernate.Sql {
	
	public class OracleOuterJoinFragment : OuterJoinFragment {
		private StringBuilder afterFrom = new StringBuilder();
		private StringBuilder afterWhere = new StringBuilder();

		public override void AddJoin(string tableName, string alias, string[] fkColumns, string[] pkColumns, bool innerJoin) {
			afterFrom.Append(StringHelper.CommaSpace)
				.Append(tableName)
				.Append(' ')
				.Append(alias);

			for (int j=0; j<fkColumns.Length; j++) {
				afterWhere.Append(" and ")
					.Append( fkColumns[j] )
					.Append('=')
					.Append(alias)
					.Append(StringHelper.Dot)
					.Append( pkColumns[j] );
				if (!innerJoin) afterWhere.Append("(+)");
			}
		}

		public override string ToFromFragmentString() {
			return afterFrom.ToString();
		}

		public override string ToWhereFragmentString() {
			return afterWhere.ToString();
		}

		public override void AddJoins(string fromFragment, string whereFragment) {
			afterFrom.Append(fromFragment);
			afterWhere.Append(whereFragment);
		}
	}
}
