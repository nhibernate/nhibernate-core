using System;
using System.Text;
using NHibernate.Util;

namespace NHibernate.Sql {
	
	public class ConditionalFragment {
		private string tableAlias;
		private string[] lhs;
		private string[] rhs;
		private string op = "=";

		/// <summary>
		/// Sets the op
		/// </summary>
		/// <param name="op">The op to set</param>
		public ConditionalFragment SetOp(string op) {
			this.op = op;
			return this;
		}

		public ConditionalFragment SetTableAlias(string tableAlias) {
			this.tableAlias = tableAlias;
			return this;
		}

		public ConditionalFragment SetCondition(string[] lhs, string[] rhs) {
			this.lhs = lhs;
			this.rhs = rhs;
			return this;
		}

		public ConditionalFragment SetCondition(string[] lhs, string rhs) {
			this.lhs = lhs;
			this.rhs = ArrayHelper.FillArray(rhs, lhs.Length);
			return this;
		}

		public string ToFragmentString() {
			StringBuilder buf = new StringBuilder( lhs.Length * 10 );
			for ( int i=0; i<lhs.Length; i++ ) {
				buf.Append(tableAlias)
					.Append(StringHelper.Dot)
					.Append( lhs[i] )
					.Append(op)
					.Append( rhs[i] );
				if (i<lhs.Length-1) buf.Append(" and ");
			}
			return buf.ToString();
		}
	}
}
