using System;
using System.Data;
using System.Collections;

namespace NHibernate.Tool.hbm2ddl {
	
	public class AdoTable {
		private string name;
		private IDictionary columns = new Hashtable();
		private IDictionary foreignKeys = new Hashtable();
		private IDictionary indexes = new Hashtable();
		
		public AdoTable(DataTable table) {
			name = table.TableName;
			foreach(DataColumn column in table.Columns) {
				columns.Add( column.ColumnName, new AdoColumn(column) );
			}
		}

		public ICollection Columns {
			get { return columns.Values; }
		}


	}
}
