using System;
using System.Data;

namespace NHibernate.Tool.hbm2ddl {
	
	public class AdoColumn {
		private string name;
		private System.Type type;
		private int columnSize;
		private bool isNullable;
		

		public AdoColumn(DataColumn column) {
			name = column.ColumnName;
			type = column.DataType;
			columnSize = column.MaxLength;
			isNullable = column.AllowDBNull;
		}

		public string Name {
			get { return name; }
		}
		public System.Type Type {
			get { return type; }
		}
		public int ColumnSize {
			get { return columnSize; }
		}
		public bool IsNullable {
			get { return isNullable; }
		}

		public override string ToString() {
			return name;
		}

		public override int GetHashCode() {
			return name.GetHashCode();
		}
	}
}
