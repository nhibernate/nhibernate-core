using System;
using System.Text;
using System.Collections;
using NHibernate.Util;
using NHibernate.Dialect;


namespace NHibernate.Mapping {
	
	public class UniqueKey : Constraint {
		
		public string SqlConstraintString(Dialect.Dialect d) {
			StringBuilder buf = new StringBuilder(" unique (");
			int i=0;
			foreach(Column col in ColumnCollection) {
				buf.Append( col.Name);
				if (i < ColumnCollection.Count-1) buf.Append(StringHelper.CommaSpace);
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
			}
			return StringHelper.Replace( buf.Append(StringHelper.ClosedParen).ToString(), "primary key", "unique" );
		}
	}
}
