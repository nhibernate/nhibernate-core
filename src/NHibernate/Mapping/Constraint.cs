using System;
using System.Text;
using System.Collections;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Mapping {
	
	public abstract class Constraint : IRelationalModel {
		private string name;
		private ArrayList columns = new ArrayList();
		private Table table;

		public string Name {
			get { return name; }
			set { name = value; }
		}

		public ICollection ColumnCollection {
			get { return columns; }
		}

		public void AddColumn(Column column) {
			if ( !columns.Contains(column) ) columns.Add(column);
		}

		public int ColumnSpan {
			get { return columns.Count; }
		}

		public Table Table {
			get { return table; }
			set { table = value; }
		}

		public string SqlDropString(Dialect.Dialect dialect) {
			return "alter table " + Table.QualifiedName + " drop constraint " + Name;
		}

		public string SqlCreateString(Dialect.Dialect dialect, IMapping p) {
			StringBuilder buf = new StringBuilder("alter table ")
				.Append( Table.QualifiedName )
				.Append( SqlConstraintString( dialect, Name ) );
			return buf.ToString();
		}

		public abstract string SqlConstraintString(Dialect.Dialect d, string constraintName);

	}
}
