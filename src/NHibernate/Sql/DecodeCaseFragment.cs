using System;
using System.Text;
using System.Collections;
using NHibernate.Util;

namespace NHibernate.Sql {
	/// <summary>
	/// Represents an SQL decode(pkvalue, key1, 1, key2, 2, ..., 0)
	/// </summary>
	public class DecodeCaseFragment : CaseFragment {
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
			cases.Add( alias + StringHelper.Dot + columnName, value );
			return this;
		}

		public override string ToFragmentString() {
			StringBuilder buf = new StringBuilder( cases.Count * 15 + 10 );

			foreach(string key in cases.Values) {
				if (cases[key].Equals("0")) {
					buf.Insert(0, key);
				} else {
					buf.Append(", ")
						.Append( key )
						.Append(", ")
						.Append( cases[key] );
				}
			}
			return buf.Insert(0, "decode (").Append(",0 ) as ")
				.Append(returnColumnName)
				.ToString();
		}
	}
}
