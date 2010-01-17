using System.Collections;
using System.Collections.Generic;
using System.Text;
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
		/// Generates the SQL string to create the named Foreign Key Constraint in the database.
		/// </summary>
		/// <param name="d">The <see cref="Dialect.Dialect"/> to use for SQL rules.</param>
		/// <param name="constraintName">The name to use as the identifier of the constraint in the database.</param>
		/// <param name="defaultSchema"></param>
		/// <param name="defaultCatalog"></param>
		/// <returns>
		/// A string that contains the SQL to create the named Foreign Key Constraint.
		/// </returns>
		public override string SqlConstraintString(Dialect.Dialect d, string constraintName, string defaultCatalog, string defaultSchema)
		{
			string[] cols = new string[ColumnSpan];
			string[] refcols = new string[ColumnSpan];
			int i = 0;
			IEnumerable<Column> refiter;
			if (IsReferenceToPrimaryKey)
				refiter = referencedTable.PrimaryKey.ColumnIterator;
			else
				refiter = referencedColumns;
			foreach (Column column in ColumnIterator)
			{
				cols[i] = column.GetQuotedName(d);
				i++;
			}

			i = 0;
			foreach (Column column in refiter)
			{
				refcols[i] = column.GetQuotedName(d);
				i++;
			}
			string result = d.GetAddForeignKeyConstraintString(constraintName, cols, referencedTable.GetQualifiedName(d, defaultCatalog, defaultSchema), refcols, IsReferenceToPrimaryKey);
			return cascadeDeleteEnabled && d.SupportsCascadeDelete ? result + " on delete cascade" : result;
		}

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

		#region IRelationalModel Memebers

		/// <summary>
		/// Get the SQL string to drop this Constraint in the database.
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to use for SQL rules.</param>
		/// <param name="defaultSchema"></param>
		/// <param name="defaultCatalog"></param>
		/// <returns>
		/// A string that contains the SQL to drop this Constraint.
		/// </returns>
		public override string SqlDropString(Dialect.Dialect dialect, string defaultCatalog, string defaultSchema)
		{
			string ifExists = dialect.GetIfExistsDropConstraint(Table, Name);
			string drop = string.Format("alter table {0} {1}", Table.GetQualifiedName(dialect, defaultCatalog, defaultSchema),
																	dialect.GetDropForeignKeyConstraintString(Name));
			string end = dialect.GetIfExistsDropConstraintEnd(Table, Name);
			return ifExists + System.Environment.NewLine + drop + System.Environment.NewLine + end;
		}

		#endregion

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
				StringBuilder result = new StringBuilder();
				result.Append(GetType().FullName)
					.Append('(')
					.Append(Table.Name)
					.Append(ArrayHelper.ToStringArray((ICollection)Columns))
					.Append(" ref-columns:")
					.Append('(')
					.Append(ArrayHelper.ToStringArray((ICollection)ReferencedColumns))
					.Append(") as ")
					.Append(Name);
				return result.ToString();
			}
			else
			{
				return base.ToString();
			}
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
	}
}
