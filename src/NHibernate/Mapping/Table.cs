using System;
using System.Text;
using System.Data;
using System.Collections;
using NHibernate.Util;
using NHibernate.Dialect;
using NHibernate.Id;
using NHibernate.Engine;

namespace NHibernate.Mapping {
	
	public class Table {//: IRelationalModel {
		
		private string name;
		private string schema;
		private SequencedHashMap columns = new SequencedHashMap();
		private Value idValue;
		private PrimaryKey primaryKey;
		private IDictionary indexes = new Hashtable();
		private IDictionary foreignKeys = new Hashtable();
		private IDictionary uniqueKeys = new Hashtable();
		private int uniqueInteger;
		private static int tableCounter = 0;

		public Table() {
			uniqueInteger = tableCounter++;
		}

		public string QualifiedName {
			get { return (schema == null) ? name : schema + StringHelper.Dot + name; }
		}

		public string GetQualifiedName(string defaultQualifier) {
			return (schema==null) ? ( (defaultQualifier==null) ? name : defaultQualifier + StringHelper.Dot + name ) : QualifiedName;
		}

		public string Name {
			get { return name; }
			set { name = value; }
		}

		public Column GetColumn(int n) {
			IEnumerator iter = columns.Values.GetEnumerator();
			for (int i=0; i<n; i++) iter.MoveNext();
			return (Column) iter.Current;
		}
		
		public void AddColumn(Column column) {
			Column old = (Column) columns[ column.Name ];
			if (old == null) {
				columns[column.Name] = column;
				column.uniqueInteger = columns.Count;
			} else {
				column.uniqueInteger = old.uniqueInteger;
			}
		}
		public int ColumnSpan {
			get { return columns.Count; }
		}
		public IEnumerator GetColumnEnumerator() {
			return columns.Values.GetEnumerator();
		}
		public IEnumerator GetIndexEnumerator() {
			return indexes.Values.GetEnumerator();
		}
		public IEnumerator GetForeignKeyEnumerator() {
			return foreignKeys.Values.GetEnumerator();
		}
		public IEnumerator GetUniqueKeyEnumerator() {
			return uniqueKeys.Values.GetEnumerator();
		}

		public string SqlAlterString(Dialect.Dialect dialect, IMapping p, DataTable tableInfo) {
			IEnumerator iter = GetColumnEnumerator();
			StringBuilder buf = new StringBuilder(50);
			while(iter.MoveNext()) {
				Column col = (Column) iter.Current;
				DataColumn columnInfo = tableInfo.Columns[ col.Name ];

				if (columnInfo == null) {
					// the column doesnt exist at all
					if (buf.Length != 0)
						buf.Append(StringHelper.CommaSpace);
					buf.Append(col.Name).Append(' ').Append(col.GetSqlType(dialect, p));
					if (col.IsUnique && dialect.SupportsUnique) {
						buf.Append(" unique");
					}
				}
			}

			if (buf.Length == 0)
				return null;

			return new StringBuilder("alter table ").Append(QualifiedName).Append(" add ").Append(buf).ToString();

		}

		public string SqlCreateString(Dialect.Dialect dialect, IMapping p) {
			//TODO: finish
			return null;
		}

	

	}
}
