using System;
using System.Collections;
using NHibernate.Dialect;

namespace NHibernate.Mapping {
	
	public class ForeignKey : Constraint {
		private Table referencedTable;
		private System.Type referencedClass;

		public override string SqlConstraintString(Dialect.Dialect d, string constraintName) {
			string[] cols = new string[ ColumnSpan ];
			string[] refcols = new string[ ColumnSpan ];
			int i=0;
			foreach(Column col in referencedTable.PrimaryKey.ColumnCollection) {
				refcols[i] = col.Name;
				i++;
			}
			i=0;
			foreach(Column col in ColumnCollection) {
				cols[i] = col.Name;
				i++;
			}
			return d.GetAddForeignKeyConstraintString(constraintName, cols, referencedTable.QualifiedName, refcols);
		}

		public Table ReferencedTable {
			get { return referencedTable; }
			set {
				if (referencedTable.PrimaryKey.ColumnSpan != ColumnSpan)
					throw new MappingException("Foreign key must have same number of columns as referenced primary key");

				IEnumerator fkCols = ColumnCollection.GetEnumerator();
				IEnumerator pkCols = referencedTable.PrimaryKey.ColumnCollection.GetEnumerator();

				while( fkCols.MoveNext() && pkCols.MoveNext() ) {
					((Column)fkCols.Current).Length = ((Column)pkCols.Current).Length;
				}
				this.referencedTable = referencedTable;
			}
		}

		public System.Type ReferencedClass {
			get { return referencedClass; }
			set { referencedClass = value; }
		}

		public ForeignKey() { }

	}
}
