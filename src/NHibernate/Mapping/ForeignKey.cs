using System.Collections;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A Foreign Key constraint in the database.
	/// </summary>
	public class ForeignKey : Constraint
	{
		private Table referencedTable;
		private System.Type referencedClass;

		/// <summary>
		/// Initializes a new instance of the <see cref="ForeignKey"/> class.
		/// </summary>
		public ForeignKey()
		{
		}

		/// <summary>
		/// Generates the SQL string to create the named Foreign Key Constraint in the database.
		/// </summary>
		/// <param name="d">The <see cref="Dialect.Dialect"/> to use for SQL rules.</param>
		/// <param name="constraintName">The name to use as the identifier of the constraint in the database.</param>
		/// <param name="defaultSchema"></param>
		/// <returns>
		/// A string that contains the SQL to create the named Foreign Key Constraint.
		/// </returns>
		public override string SqlConstraintString(Dialect.Dialect d, string constraintName, string defaultSchema)
		{
			string[] cols = new string[ColumnSpan];
			string[] refcols = new string[ColumnSpan];
			int i = 0;

			foreach (Column col in referencedTable.PrimaryKey.ColumnIterator)
			{
				refcols[i] = col.GetQuotedName(d);
				i++;
			}

			i = 0;
			foreach (Column col in ColumnIterator)
			{
				cols[i] = col.GetQuotedName(d);
				i++;
			}

			return
				d.GetAddForeignKeyConstraintString(constraintName, cols, referencedTable.GetQualifiedName(d, defaultSchema), refcols, false);
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
			set
			{
				if (value.PrimaryKey.ColumnSpan != ColumnSpan)
				{
					string message = "Foreign key in table {0} must have same number of columns as referenced primary key in table {1}";

					throw new MappingException(string.Format(message, this.Table.Name, value.Name));
				}

				IEnumerator fkCols = ColumnIterator.GetEnumerator();
				IEnumerator pkCols = value.PrimaryKey.ColumnIterator.GetEnumerator();

				while (fkCols.MoveNext() && pkCols.MoveNext())
				{
					((Column) fkCols.Current).Length = ((Column) pkCols.Current).Length;
				}

				this.referencedTable = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="System.Type"/> that this Foreign Key is referencing.
		/// </summary>
		/// <value>
		/// The <see cref="System.Type"/> that this Foreign Key is referencing.
		/// </value>
		public System.Type ReferencedClass
		{
			get { return referencedClass; }
			set { referencedClass = value; }
		}

		#region IRelationalModel Memebers

		/// <summary>
		/// Get the SQL string to drop this Constraint in the database.
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to use for SQL rules.</param>
		/// <param name="defaultCatalog"></param>
		/// <param name="defaultSchema"></param>
		/// <returns>
		/// A string that contains the SQL to drop this Constraint.
		/// </returns>
		public override string SqlDropString(Dialect.Dialect dialect, string defaultCatalog, string defaultSchema)
		{
			string ifExists = dialect.GetIfExistsDropConstraint(Table, Name);
			string drop = string.Format("alter table {0} {1}", Table.GetQualifiedName(dialect, defaultSchema),
			                            dialect.GetDropForeignKeyConstraintString(Name));
			string end = dialect.GetIfExistsDropConstraintEnd(Table, Name);
			return ifExists + System.Environment.NewLine + drop + System.Environment.NewLine + end;
		}

		#endregion

		public bool IsPhysicalConstraint
		{
			get
			{
				return referencedTable.IsPhysicalTable && Table.IsPhysicalTable && 
					!referencedTable.HasDenormalizedTables;
			}
		}
	}
}
