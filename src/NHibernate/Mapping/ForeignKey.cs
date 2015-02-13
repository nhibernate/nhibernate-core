using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.DdlGen.Model;
using NHibernate.DdlGen.Operations;
using NHibernate.Engine;
using NHibernate.Util;
using System;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A Foreign Key constraint in the database.
	/// </summary>
	[Serializable]
	public class ForeignKey : Constraint
	{
		private Table referencedTable;
		private string referencedEntityName;
		private bool cascadeDeleteEnabled;
		private readonly List<Column> referencedColumns = new List<Column>();

		

		/// <summary>
		/// Gets or sets the <see cref="Table"/> that the Foreign Key is referencing.
		/// </summary>
		/// <value>The <see cref="Table"/> the Foreign Key is referencing.</value>
		/// <exception cref="MappingException">
		/// Thrown when the number of columns in this Foreign Key is not the same
		/// amount of columns as the Primary Key in the ReferencedTable.
		/// </exception>
		public Table ReferencedTable
		{
			get { return referencedTable; }
			set { referencedTable = value; }
		}

		public bool CascadeDeleteEnabled
		{
			get { return cascadeDeleteEnabled; }
			set { cascadeDeleteEnabled = value; }
		}



		/// <summary> 
		/// Validates that columnspan of the foreignkey and the primarykey is the same.
		///  Furthermore it aligns the length of the underlying tables columns.
		/// </summary>
		public void AlignColumns()
		{
			if (IsReferenceToPrimaryKey)
				AlignColumns(referencedTable);
		}

		private void AlignColumns(Table referencedTable)
		{
			if (referencedTable.PrimaryKey.ColumnSpan != ColumnSpan)
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("Foreign key (")
					.Append(Name + ":")
					.Append(Table.Name)
					.Append(" [");
				AppendColumns(sb, ColumnIterator);
				sb.Append("])")
					.Append(") must have same number of columns as the referenced primary key (")
					.Append(referencedTable.Name).Append(" [");
				AppendColumns(sb, referencedTable.PrimaryKey.ColumnIterator);
				sb.Append("])");
				throw new FKUnmatchingColumnsException(sb.ToString());
			}
			IEnumerator<Column> fkCols = ColumnIterator.GetEnumerator();
			IEnumerator<Column> pkCols = referencedTable.PrimaryKey.ColumnIterator.GetEnumerator();

			while (fkCols.MoveNext() && pkCols.MoveNext())
			{
				fkCols.Current.Length = pkCols.Current.Length;
			}
		}

		private static void AppendColumns(StringBuilder buf, IEnumerable<Column> columns)
		{
			bool commaNeeded = false;
			foreach (Column column in columns)
			{
				if (commaNeeded)
					buf.Append(StringHelper.CommaSpace);
				commaNeeded = true;
				buf.Append(column.Name);
			}
		}

		public virtual void AddReferencedColumns(IEnumerable<Column> referencedColumnsIterator)
		{
			foreach (Column col in referencedColumnsIterator)
			{
				if (!col.IsFormula)
					AddReferencedColumn(col);
			}
		}

		private void AddReferencedColumn(Column column)
		{
			if (!referencedColumns.Contains(column))
				referencedColumns.Add(column);
		}

		internal void AddReferencedTable(PersistentClass referencedClass)
		{
			if (referencedColumns.Count > 0)
			{
				referencedTable = referencedColumns[0].Value.Table;
			}
			else
			{
				referencedTable = referencedClass.Table;
			}
		}

		public override string ToString()
		{
			if (!IsReferenceToPrimaryKey)
			{
				var result = new StringBuilder();
				result.Append(GetType().FullName)
					.Append('(')
					.Append(Table.Name)
					.Append(StringHelper.Join(", " , Columns))
					.Append(" ref-columns:")
					.Append('(')
					.Append(StringHelper.Join(", ", ReferencedColumns))
					.Append(") as ")
					.Append(Name);
				return result.ToString();
			}

			return base.ToString();
		}

		public bool HasPhysicalConstraint
		{
			get
			{
				return referencedTable.IsPhysicalTable && Table.IsPhysicalTable && !referencedTable.HasDenormalizedTables;
			}
		}

		public IList<Column> ReferencedColumns
		{
			get { return referencedColumns; }
		}

		public string ReferencedEntityName
		{
			get { return referencedEntityName; }
			set { referencedEntityName = value; }
		}

		/// <summary>Does this foreignkey reference the primary key of the reference table </summary>
		public bool IsReferenceToPrimaryKey
		{
			get { return referencedColumns.Count == 0; }
		}

        #region DdlOperations Generation

        public IDdlOperation GetCreateOperation(Dialect.Dialect dialect, IMapping mapping, string defaultSchema)
        {
            var constratintName = dialect.Qualify("", Table.Schema ?? defaultSchema, Name);
            var model = GetForeignKeyModel(dialect, constratintName, defaultSchema);
            return new CreateForeignKeyOperation(model);
        }

        public IDdlOperation GetDropOperation(Dialect.Dialect dialect, string defaultSchema)
        {
            var constratintName = dialect.Qualify("", Table.Schema ?? defaultSchema, Name);
            var model = GetForeignKeyModel(dialect, constratintName, defaultSchema);
            return new DropForeignKeyDdlOperation(model);
        }



        private ForeignKeyModel GetForeignKeyModel(Dialect.Dialect dialect, string constraintName, string defaultSchema)
        {
            var referencedTableName = referencedTable.GetThreePartName(dialect, "", defaultSchema);

            //var referencedTableName = referencedTable.GetQualifiedName(d, defaultCatalog, defaultSchema);

            var dependentTableName = this.Table.GetThreePartName(dialect, "", defaultSchema);
            var model = new ForeignKeyModel
            {
                Name = constraintName,
                DependentTable = dependentTableName,
                ReferencedTable = referencedTableName,
                CascadeDelete = CascadeDeleteEnabled,
                ForeignKeyColumns = ColumnIterator.Select(c => c.GetQuotedName()).ToList(),
                IsReferenceToPrimaryKey = IsReferenceToPrimaryKey,
                PrimaryKeyColumns = (IsReferenceToPrimaryKey ? referencedTable.PrimaryKey.ColumnIterator : referencedColumns)
                    .Select(c => c.GetQuotedName()).ToList()
            };
            //string result = d.GetAddForeignKeyConstraintString(constraintName, cols, referencedTable.GetQualifiedName(d, defaultCatalog, defaultSchema), refcols, IsReferenceToPrimaryKey);
            return model;
        }

        #endregion
    }
}
