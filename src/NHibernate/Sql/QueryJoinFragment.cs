using System;
using System.Text;

using NHibernate.Dialect;
using NHibernate.Util;

namespace NHibernate.Sql
{
	/// <summary>
	/// Summary description for QueryJoinFragment.
	/// </summary>
	public class QueryJoinFragment : JoinFragment {

		private StringBuilder afterFrom = new StringBuilder();
		private StringBuilder afterWhere = new StringBuilder();
		private Dialect.Dialect dialect;
	
		public QueryJoinFragment(Dialect.Dialect dialect) {
			this.dialect = dialect;
		}

		public override void AddJoin(string tableName, string alias, string[] fkColumns, string[] pkColumns, JoinType joinType) {
			if (joinType!=JoinType.InnerJoin) {
				//TODO: get right impl for dialect
				JoinFragment jf = dialect.CreateOuterJoinFragment();
				jf.AddJoin(tableName, alias, fkColumns, pkColumns, joinType);
				AddFragment(jf);
			}
			else {
				AddCrossJoin(tableName, alias);
				AddCondition(alias, fkColumns, pkColumns);
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
			QueryJoinFragment copy = new QueryJoinFragment(dialect);
			copy.afterFrom = new StringBuilder( afterFrom.ToString() );
			copy.afterWhere = new StringBuilder( afterWhere.ToString() );
			return copy;
		}

		public override void AddCondition(string alias, string[] columns, string condition) {
			for ( int i=0; i<columns.Length; i++ ) {
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
			for ( int j=0; j<fkColumns.Length; j++) {
				afterWhere.Append(" and ")
					.Append( fkColumns[j] )
					.Append('=')
					.Append(alias)
					.Append(StringHelper.Dot)
					.Append( pkColumns[j] );
			}
		}

		public override void AddCondition(string condition) {
			if ( 
				afterFrom.ToString().IndexOf( condition.Trim() ) < 0 &&
				afterWhere.ToString().IndexOf( condition.Trim() ) < 0 )
			{

			
				if ( !condition.StartsWith(" and ") ) afterWhere.Append(" and ");
				afterWhere.Append(condition);
			}
		}

	}
}