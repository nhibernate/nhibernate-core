using System;
using System.Text;
using System.Collections;
using NHibernate.Util;

namespace NHibernate.Sql {

	/// <summary>
	/// Represents an SQL <c>case when ... then ... end as ...</c>
	/// </summary>
	public class ANSICaseFragment : CaseFragment {
		private string returnColumnName;

		private SortedList cases = new SortedList();

		public override CaseFragment SetReturnColumnName(string returnColumnName) {
			this.returnColumnName = returnColumnName;
			return this;
		}

		public override CaseFragment SetReturnColumnName(string returnColumnName, string suffix) {
			return SetReturnColumnName( StringHelper.Suffix(returnColumnName, suffix) );
		}

		public override CaseFragment AddWhenColumnNotNull(string alias, string columnName, string value) {
			cases.Add( alias + StringHelper.Dot + columnName + " is not null", value );
			return this;
		}

		public override string ToFragmentString() {
			StringBuilder buf = new StringBuilder( cases.Count * 15 + 10 );

			buf.Append("case");

			foreach( string key in cases.Keys) {
				buf.Append(" when ")
					.Append( key )
					.Append(" then ")
					.Append(cases[key]);
			}

			return buf.Append(" end as ")
				.Append(returnColumnName)
				.ToString();
		}

	}
}
