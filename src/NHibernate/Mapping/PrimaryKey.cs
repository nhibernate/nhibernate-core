using System;
using System.Text;
using System.Collections;
using NHibernate.Dialect;
using NHibernate.Util;

namespace NHibernate.Mapping {

	public class PrimaryKey : Constraint {
		
		public string SqlConstraintString(Dialect.Dialect d) {
			StringBuilder buf = new StringBuilder(" primary key (");
			int i=0;
			foreach(Column col in ColumnCollection) {
				buf.Append(col.Name);
				if (i < ColumnCollection.Count-1) buf.Append(StringHelper.CommaSpace);
				i++;
			}
			return buf.Append(StringHelper.ClosedParen).ToString();
		}

		public override string SqlConstraintString(Dialect.Dialect d, string constraintName) {
			StringBuilder buf = new StringBuilder(
				d.GetAddPrimaryKeyConstraintString(constraintName))
				.Append('(');
			int i=0;
			foreach(Column col in ColumnCollection) {
				buf.Append( col.Name);
				if (i < ColumnCollection.Count - 1) buf.Append(StringHelper.CommaSpace);
				i++;
			}
			return buf.Append(StringHelper.ClosedParen).ToString();
		}
	}
}
