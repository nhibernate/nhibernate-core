using System;
using System.Collections;
using System.Text;
using NHibernate.Engine;
using NHibernate.Util;
using NHibernate.Dialect;

namespace NHibernate.Mapping {
	
	public class Index : IRelationalModel {
		private Table table;
		private ArrayList columns = new ArrayList();
		private string name;

		public string SqlCreateString(Dialect.Dialect dialect, IMapping p) {
			StringBuilder buf = new StringBuilder("create index ")
				.Append( dialect.QualifyIndexName ? name : StringHelper.Unqualify(name) )
				.Append(" on ")
				.Append( table.QualifiedName )
				.Append(" (");
			for(int i=0; i<columns.Count; i++) {
				buf.Append( ((Column)columns[i]).Name);
				if (i<columns.Count-1) buf.Append(StringHelper.CommaSpace);
			}
			buf.Append(StringHelper.ClosedParen);
			return buf.ToString();
		}

		public string SqlDropString(Dialect.Dialect dialect) {
			return "drop index " + table.QualifiedName + StringHelper.Dot + name;
		}

		public Table Table {
			get { return table; }
			set { table = value; }
		}

		public ICollection ColumnCollection {
			get { return columns; }
		}

		public void AddColumn(Column column) {
			if (!columns.Contains(column)) columns.Add(column);
		}

		public string Name {
			get { return name; }
			set { name = value; }
		}
	}
}
