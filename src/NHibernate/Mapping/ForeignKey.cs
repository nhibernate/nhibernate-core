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
		/// <returns>
		/// A string that contains the SQL to create the named Foreign Key Constraint.
		/// </returns>
		public override string SqlConstraintString( Dialect.Dialect d, string constraintName )
		{
			string[ ] cols = new string[ColumnSpan];
			string[ ] refcols = new string[ColumnSpan];
			int i = 0;

			foreach( Column col in referencedTable.PrimaryKey.ColumnCollection )
			{
				refcols[ i ] = col.GetQuotedName( d );
				i++;
			}

			i = 0;
			foreach( Column col in ColumnCollection )
			{
				cols[ i ] = col.GetQuotedName( d );
				i++;
			}

			return d.GetAddForeignKeyConstraintString( constraintName, cols, referencedTable.GetQualifiedName( d ), refcols );
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
				if( value.PrimaryKey.ColumnSpan != ColumnSpan )
				{
					throw new MappingException( "Foreign key must have same number of columns as referenced primary key" );
				}

				IEnumerator fkCols = ColumnCollection.GetEnumerator();
				IEnumerator pkCols = value.PrimaryKey.ColumnCollection.GetEnumerator();

				while( fkCols.MoveNext() && pkCols.MoveNext() )
				{
					( ( Column ) fkCols.Current ).Length = ( ( Column ) pkCols.Current ).Length;
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
	}
}