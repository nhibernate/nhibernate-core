using System;
using System.Text;
using System.Collections;
using NHibernate.Util;

namespace NHibernate.Sql {

	/// <summary>
	/// Represents an <c>... in (...)</c> expression
	/// </summary>
	public class InFragment {
		private string columnName;
		private ArrayList values = new ArrayList();

		public InFragment AddValue(string value) {
			values.Add(value);
			return this;
		}

		public InFragment SetColumn(string columnName) {
			this.columnName = columnName;
			return this;
		}

		public InFragment SetColumn(string alias, string columnName) {
			return SetColumn( this.columnName = alias + StringHelper.Dot + columnName );
		}

		public string ToFragmentString() {
			StringBuilder buf = new StringBuilder(values.Count * 5);
			buf.Append(columnName);
			if (values.Count > 1) {
				bool allowNull = false;
				buf.Append(" in (");
				for(int i=0; i<values.Count; i++) {
					if("null".Equals(values[i]))
						allowNull = true;
					else {
						buf.Append( values[i] );
						if ( i<values.Count-1) buf.Append(StringHelper.CommaSpace);
					}
				}
				buf.Append(StringHelper.ClosedParen);
				if(allowNull)
					buf.Insert(0, " is null or ")
						.Insert(0, columnName)
						.Insert(0, StringHelper.OpenParen)
						.Append(StringHelper.OpenParen);
			} else {
				string value = values[0] as string;
				if ( "null".Equals(value) ) {
					buf.Append(" is null");
				} else {
					buf.Append("=").Append(values[0]);
				}
			}
			return buf.ToString();
		}				
	}
}
