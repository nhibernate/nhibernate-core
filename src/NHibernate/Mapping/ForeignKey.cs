using System.Collections;

namespace NHibernate.Mapping
{
	/// <summary></summary>
	public class ForeignKey : Constraint
	{
		private Table referencedTable;
		private System.Type referencedClass;

		/// <summary></summary>
		public ForeignKey()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="d"></param>
		/// <param name="constraintName"></param>
		/// <returns></returns>
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

		/// <summary></summary>
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

		/// <summary></summary>
		public System.Type ReferencedClass
		{
			get { return referencedClass; }
			set { referencedClass = value; }
		}
	}
}