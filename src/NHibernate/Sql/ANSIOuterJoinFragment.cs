using System;
using System.Text;
using NHibernate.Util;

namespace NHibernate.Sql {
	
	public class ANSIOuterJoinFragment : OuterJoinFragment {
		
		private StringBuilder buffer = new StringBuilder();

		public override void AddJoin(string tableName, string alias, string[] fkColumns, string[] pkColumns, bool innerJoin) {
			buffer.Append( innerJoin ? " inner join " : " left outer join ")
				.Append(' ')
				.Append(alias)
				.Append(" on ");

			for (int j=0; j<fkColumns.Length; j++) {
				if (fkColumns[j].IndexOf('.')<1) throw new AssertionFailure("missing alias");
				buffer.Append( fkColumns[j] )
					.Append('=')
					.Append(alias)
					.Append(StringHelper.Dot)
					.Append(pkColumns[j]);
				if (j<fkColumns.Length-1) buffer.Append(" and ");
			}
		}

		public override string ToFromFragmentString() {
			return buffer.ToString();
		}

		public override string ToWhereFragmentString() {
			return StringHelper.EmptyString;
		}

		public override void AddJoins(string fromFragment, string whereFragment) {
			buffer.Append(fromFragment);
			if (whereFragment != null && whereFragment != "")
				throw new AssertionFailure("where fragment must be empty");
		}
	}
}
