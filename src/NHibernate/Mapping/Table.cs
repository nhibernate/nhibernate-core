using System;
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

	}
}
