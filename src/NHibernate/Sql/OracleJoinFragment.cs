using System;
using System.Text;
using NHibernate.Util;

namespace NHibernate.Sql {
	
	public class OracleJoinFragment : JoinFragment {
		private StringBuilder afterFrom = new StringBuilder();
		private StringBuilder afterWhere = new StringBuilder();

		public override void AddJoin(string tableName, string alias, string[] fkColumns, string[] pkColumns, JoinType joinType) {
			AddCrossJoin(tableName, alias);

			for (int j=0; j<fkColumns.Length; j++) {
				afterWhere.Append(" and ")
					.Append( fkColumns[j] );
				if (joinType == JoinType.RightOuterJoin || joinType == JoinType.FullJoin) afterWhere.Append("(+)");
				afterWhere.Append('=')
					.Append(alias)
					.Append(StringHelper.Dot)
					.Append( pkColumns[j] );
				if (joinType == JoinType.LeftOuterJoin || joinType == JoinType.FullJoin) afterWhere.Append("(+)");
			}
		}

		public override string ToFromFragmentString {
			get { return afterFrom.ToString(); }
		}

		public override string ToWhereFragmentString {
			get { return afterWhere.ToString(); }
		}

		public override void AddJoins(string fromFragment, string whereFragment) {
			afterFrom.Append(fromFragment);
			afterWhere.Append(whereFragment);
		}

		public override JoinFragment Copy() {
			OracleJoinFragment copy = new OracleJoinFragment();
			copy.afterFrom = new StringBuilder( afterFrom.ToString() );
			copy.afterWhere = new StringBuilder( afterWhere.ToString() );
			return copy;
		}

		public override void AddCondition(string alias, string[] columns, string condition) {
			for (int i=0; i<columns.Length; i++) {
				afterWhere.Append(" and ")
					.Append(alias)
					.Append(StringHelper.Dot)
					.Append( columns[i] )
					.Append(condition);
			}
		}

		public override void AddCrossJoin(string tableName, string alias) {
			afterFrom.Append(StringHelper.CommaSpace)
				.Append(tableName)
				.Append(' ')
				.Append(alias);
		}

		public override void AddCondition(string alias, string[] fkColumns, string[] pkColumns) {
			throw new NotSupportedException();
		}

		public override void AddCondition(string condition) {
			throw new NotSupportedException();
		}
	}
}
