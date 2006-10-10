using System;
using System.Collections;
using System.Data;
using System.Text;

using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary>
	/// Represents a Table in a database that an object gets mapped against.
	/// </summary>
	public class Table : IRelationalModel
	{
		private string name;
		private string schema;
		private SequencedHashMap columns = new SequencedHashMap();
		private SimpleValue idValue;
		private PrimaryKey primaryKey;
		private IDictionary indexes = new Hashtable();
		private IDictionary foreignKeys = new Hashtable();
		private IDictionary uniqueKeys = new Hashtable();
		private int uniqueInteger;
		private bool quoted;
		private static int tableCounter = 0;
		private IList checkConstraints = new ArrayList();

		/// <summary>
		/// Initializes a new instance of <see cref="Table"/>.
		/// </summary>
		public Table()
		{
			uniqueInteger = tableCounter++;
		}

		/// <summary>
		/// Gets the schema qualified name of the Table.
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> that knows how to Quote the Table name.</param>
		/// <returns>The name of the table qualified with the schema if one is specified.</returns>
		public string GetQualifiedName( Dialect.Dialect dialect )
		{
			string quotedName = GetQuotedName( dialect );
			return schema == null ? quotedName :
				GetQuotedSchemaName( dialect ) + StringHelper.Dot + quotedName;
		}


		/// <summary>
		/// Gets the schema qualified name of the Table using the specified qualifier
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> that knows how to Quote the Table name.</param>
		/// <param name="defaultQualifier">The Qualifier to use when accessing the table.</param>
		/// <returns>A String representing the Qualified name.</returns>
		/// <remarks>If this were used with MSSQL it would return a dbo.table_name.</remarks>
		public string GetQualifiedName( Dialect.Dialect dialect, string defaultQualifier )
		{
			string quotedName = GetQuotedName( dialect );
			return schema == null ?
				( ( defaultQualifier == null ) ? quotedName : defaultQualifier + StringHelper.Dot + quotedName ) :
				GetQualifiedName( dialect );
		}

		/// <summary>
		/// Gets or sets the name of the Table in the database.
		/// </summary>
		/// <value>
		/// The name of the Table in the database.  The get does 
		/// not return a Quoted Table name.
		/// </value>
		/// <remarks>
		/// <p>
		/// If a value is passed in that is wrapped by <c>`</c> then 
		/// NHibernate will Quote the Table whenever SQL is generated
		/// for it.  How the Table is quoted depends on the Dialect.
		/// </p>
		/// <p>
		/// The value returned by the getter is not Quoted.  To get the
		/// column name in quoted form use <see cref="GetQuotedName(Dialect.Dialect)"/>.
		/// </p>
		/// </remarks>
		public string Name
		{
			get { return name; }
			set
			{
				if( value[ 0 ] == '`' )
				{
					quoted = true;
					name = value.Substring( 1, value.Length - 2 );
				}
				else
				{
					name = value;
				}
			}
		}

		/// <summary>
		/// Gets the name of this Table in quoted form if it is necessary.
		/// </summary>
		/// <param name="dialect">
		/// The <see cref="Dialect.Dialect"/> that knows how to quote the Table name.
		/// </param>
		/// <returns>
		/// The Table name in a form that is safe to use inside of a SQL statement.
		/// Quoted if it needs to be, not quoted if it does not need to be.
		/// </returns>
		public string GetQuotedName( Dialect.Dialect dialect )
		{
			return IsQuoted ?
				dialect.QuoteForTableName( name ) :
				name;
		}

		/// <summary>
		/// Gets the schema for this table in quoted form if it is necessary.
		/// </summary>
		/// <param name="dialect">
		/// The <see cref="Dialect.Dialect" /> that knows how to quote the table name.
		/// </param>
		/// <returns>
		/// The schema name for this table in a form that is safe to use inside
		/// of a SQL statement. Quoted if it needs to be, not quoted if it does not need to be.
		/// </returns>
		private string GetQuotedSchemaName( Dialect.Dialect dialect )
		{
			if( schema == null )
			{
				return null;
			}

			if( schema.StartsWith( "`" ) )
			{
				return dialect.QuoteForSchemaName( schema.Substring( 1, schema.Length - 2 ) );
			}

			return schema;
		}

		/// <summary>
		/// Gets the <see cref="Column"/> at the specified index.
		/// </summary>
		/// <param name="n">The index of the Column to get.</param>
		/// <returns> 
		/// The <see cref="Column"/> at the specified index.
		/// </returns>
		public Column GetColumn( int n )
		{
			IEnumerator iter = columns.Values.GetEnumerator();
			for( int i = 0; i < n; i++ )
			{
				iter.MoveNext();
			}
			return ( Column ) iter.Current;
		}

		/// <summary>
		/// Adds the <see cref="Column"/> to the <see cref="ICollection"/> of 
		/// Columns that are part of the Table.
		/// </summary>
		/// <param name="column">The <see cref="Column"/> to include in the Table.</param>
		public void AddColumn( Column column )
		{
			Column old = ( Column ) columns[ column.Name ];
			if( old == null )
			{
				columns[ column.Name ] = column;
				column.uniqueInteger = columns.Count;
			}
			else
			{
				column.uniqueInteger = old.uniqueInteger;
			}
		}

		/// <summary>
		/// Gets the number of columns that this Table contains.
		/// </summary>
		/// <value>
		/// The number of columns that this Table contains.
		/// </value>
		public int ColumnSpan
		{
			get { return columns.Count; }
		}

		/// <summary>
		/// Gets an <see cref="ICollection"/> of <see cref="Column"/> objects that 
		/// are part of the Table.
		/// </summary>
		/// <value>
		/// An <see cref="ICollection"/> of <see cref="Column"/> objects that are 
		/// part of the Table.
		/// </value>
		public ICollection ColumnCollection
		{
			get { return columns.Values; }
		}

		/// <summary>
		/// Gets an <see cref="ICollection"/> of <see cref="Index"/> objects that 
		/// are part of the Table.
		/// </summary>
		/// <value>
		/// An <see cref="ICollection"/> of <see cref="Index"/> objects that are 
		/// part of the Table.
		/// </value>
		public ICollection IndexCollection
		{
			get { return indexes.Values; }
		}

		/// <summary>
		/// Gets an <see cref="ICollection"/> of <see cref="ForeignKey"/> objects that 
		/// are part of the Table.
		/// </summary>
		/// <value>
		/// An <see cref="ICollection"/> of <see cref="ForeignKey"/> objects that are 
		/// part of the Table.
		/// </value>
		public ICollection ForeignKeyCollection
		{
			get { return foreignKeys.Values; }
		}

		/// <summary>
		/// Gets an <see cref="ICollection"/> of <see cref="UniqueKey"/> objects that 
		/// are part of the Table.
		/// </summary>
		/// <value>
		/// An <see cref="ICollection"/> of <see cref="UniqueKey"/> objects that are 
		/// part of the Table.
		/// </value>
		public ICollection UniqueKeyCollection
		{
			get { return uniqueKeys.Values; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dialect"></param>
		/// <param name="p"></param>
		/// <param name="tableInfo"></param>
		/// <returns></returns>
		public IList SqlAlterStrings( Dialect.Dialect dialect, IMapping p, DataTable tableInfo )
		{
			StringBuilder root = new StringBuilder( "alter table " )
				.Append( GetQualifiedName( dialect ) )
				.Append( " " )
				.Append( dialect.AddColumnString );

			IList results = new ArrayList( ColumnCollection.Count );

			foreach( Column col in ColumnCollection )
			{
				DataColumn columnInfo = tableInfo.Columns[ col.Name ];

				if( columnInfo == null )
				{
					StringBuilder alter = new StringBuilder( root.ToString() )
						.Append( " " )
						.Append( col.GetQuotedName( dialect ) )
						.Append( " " )
						.Append( col.GetSqlType( dialect, p ) );

					if( col.IsUnique && dialect.SupportsUnique )
					{
						alter.Append( " unique" );
					}

					results.Add( alter.ToString() );
				}
			}

			return results;
		}

		/// <summary>
		/// Generates the SQL string to create this Table in the database.
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to use for SQL rules.</param>
		/// <param name="p"></param>
		/// <param name="defaultSchema"></param>
		/// <returns>
		/// A string that contains the SQL to create this Table, Primary Key Constraints
		/// , and Unique Key Constraints.
		/// </returns>
		public string SqlCreateString( Dialect.Dialect dialect, IMapping p, string defaultSchema )
		{
			StringBuilder buf = new StringBuilder( "create table " )
				.Append( GetQualifiedName( dialect, defaultSchema ) )
				.Append( " (" );

			bool identityColumn = idValue != null && idValue.CreateIdentifierGenerator( dialect ) is IdentityGenerator;

			// try to find out the name of the pk to create it as identity if the 
			// identitygenerator is used
			string pkname = null;
			if( primaryKey != null && identityColumn )
			{
				foreach( Column col in primaryKey.ColumnCollection )
				{
					pkname = col.GetQuotedName( dialect ); //should only go through this loop once
				}
			}

			int i = 0;
			foreach( Column col in ColumnCollection )
			{
				i++;
				buf.Append( col.GetQuotedName( dialect ) )
					.Append( ' ' );

				if( identityColumn && col.GetQuotedName( dialect ).Equals( pkname ) )
				{
 					// to support dialects that have their own identity data type
 					if ( dialect.hasDataTypeInIdentityColumn() ) {
 						buf.Append( col.GetSqlType( dialect, p ) );
 					}
					buf.Append( ' ' )
						.Append( dialect.IdentityColumnString );
				}
				else
				{
					buf.Append( col.GetSqlType( dialect, p ) );
					if( col.IsNullable )
					{
						buf.Append( dialect.NullColumnString );
					}
					else
					{
						buf.Append( " not null" );
					}
				}

				if( col.IsUnique )
				{
					if( dialect.SupportsUnique )
					{
						buf.Append( " unique" );
					}
					else
					{
						UniqueKey uk = GetUniqueKey( col.GetQuotedName( dialect ) + "_" );
						uk.AddColumn( col );
					}
				}
				if( i < ColumnCollection.Count )
				{
					buf.Append( StringHelper.CommaSpace );
				}
			}

			if( primaryKey != null )
			{
				//if ( dialect is HSQLDialect && identityColumn ) {
				// // skip the primary key definition	
				// //ugly hack...
				//} else {
				buf.Append( ',' ).Append( primaryKey.SqlConstraintString( dialect, defaultSchema ) );
				//}
			}

			foreach( UniqueKey uk in UniqueKeyCollection )
			{
				buf.Append( ',' ).Append( uk.SqlConstraintString( dialect ) );
			}

			buf.Append( StringHelper.ClosedParen );

			return buf.ToString();
		}

		/// <summary>
		/// Generates the SQL string to drop this Table in the database.
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to use for SQL rules.</param>
		/// <param name="defaultSchema"></param>
		/// <returns>
		/// A string that contains the SQL to drop this Table and to cascade the drop to 
		/// the constraints if the database supports it.
		/// </returns>
		public string SqlDropString( Dialect.Dialect dialect, string defaultSchema )
		{
			return dialect.GetDropTableString( GetQualifiedName( dialect, defaultSchema ) );
		}

		/// <summary>
		/// Gets or sets the <see cref="PrimaryKey"/> of the Table.
		/// </summary>
		/// <value>The <see cref="PrimaryKey"/> of the Table.</value>
		public PrimaryKey PrimaryKey
		{
			get { return primaryKey; }
			set { primaryKey = value; }
		}

		/// <summary>
		/// Gets the <see cref="Index"/> identified by the name.
		/// </summary>
		/// <param name="name">The name of the <see cref="Index"/> to get.</param>
		/// <returns>
		/// The <see cref="Index"/> identified by the name.  If the <see cref="Index"/>
		/// identified by the name does not exist then it is created.
		/// </returns>
		public Index GetIndex( string name )
		{
			Index index = ( Index ) indexes[ name ];

			if( index == null )
			{
				index = new Index();
				index.Name = name;
				index.Table = this;
				indexes.Add( name, index );
			}

			return index;
		}

		/// <summary>
		/// Gets the <see cref="UniqueKey"/> identified by the name.
		/// </summary>
		/// <param name="name">The name of the <see cref="UniqueKey"/> to get.</param>
		/// <returns>
		/// The <see cref="UniqueKey"/> identified by the name.  If the <see cref="UniqueKey"/>
		/// identified by the name does not exist then it is created.
		/// </returns>
		public UniqueKey GetUniqueKey( string name )
		{
			UniqueKey uk = ( UniqueKey ) uniqueKeys[ name ];

			if( uk == null )
			{
				uk = new UniqueKey();
				uk.Name = name;
				uk.Table = this;
				uniqueKeys.Add( name, uk );
			}

			return uk;
		}

		/// <summary>
		/// Create a <see cref="ForeignKey"/> for the columns in the Table.
		/// </summary>
		/// <param name="keyName"></param>
		/// <param name="columns">An <see cref="IList"/> of <see cref="Column"/> objects.</param>
		/// <param name="referencedClass"></param>
		/// <returns>
		/// A <see cref="ForeignKey"/> for the columns in the Table.  
		/// </returns>
		/// <remarks>
		/// This does not necessarily create a <see cref="ForeignKey"/>, if
		/// one already exists for the columns then it will return an 
		/// existing <see cref="ForeignKey"/>.
		/// </remarks>
		public ForeignKey CreateForeignKey( string keyName, IList columns, System.Type referencedClass )
		{
			if( keyName == null )
			{
				keyName = "FK" + UniqueColumnString( columns );
			}
			ForeignKey fk = ( ForeignKey ) foreignKeys[ keyName ];

			if( fk == null )
			{
				fk = new ForeignKey();
				fk.Name = keyName;
				fk.Table = this;
				fk.ReferencedClass = referencedClass;
				foreignKeys.Add( keyName, fk );
				foreach( Column col in columns )
				{
					fk.AddColumn( col );
				}
			}
			else
			{
				// "X"= hexadecimal format
				if( fk.ReferencedClass != referencedClass )
				{
					foreignKeys.Remove( keyName );
					keyName += referencedClass.Name.GetHashCode().ToString( "X" ).ToUpper( System.Globalization.CultureInfo.InvariantCulture );
					return CreateForeignKey( keyName, columns, referencedClass );
				}
			}
			return fk;
		}

		/// <summary>
		/// Generates a unique string for an <see cref="ICollection"/> of 
		/// <see cref="Column"/> objects.
		/// </summary>
		/// <param name="col">An <see cref="ICollection"/> of <see cref="Column"/> objects.</param>
		/// <returns>
		/// An unique string for the <see cref="Column"/> objects.
		/// </returns>
		public string UniqueColumnString( ICollection col )
		{
			int result = 0;

			foreach( object obj in col )
			{
				// this is marked as unchecked because the GetHashCode could potentially
				// cause an integer overflow.  This way if there is an overflow it will
				// just roll back over - since we are not doing any computations based
				// on this number then a rollover is no big deal.
				unchecked
				{
					result += obj.GetHashCode();
				}
			}
			// "X"= hexadecimal format
			return ( name.GetHashCode().ToString( "X" ) + result.GetHashCode().ToString( "X" ) );
		}

		/// <summary>
		/// Gets or sets the schema the table is in.
		/// </summary>
		/// <value>
		/// The schema the table is in or <c>null</c> if no schema is specified.
		/// </value>
		public string Schema
		{
			get { return schema; }
			set { schema = value; }
		}

		/// <summary>
		/// Gets the unique number of the Table.
		/// </summary>
		/// <value>The unique number of the Table.</value>
		public int UniqueInteger
		{
			get { return uniqueInteger; }
		}

		/// <summary>
		/// Sets the Identifier of the Table.
		/// </summary>
		/// <param name="idValue">The <see cref="SimpleValue"/> that represents the Identifier.</param>
		public void SetIdentifierValue( SimpleValue idValue )
		{
			this.idValue = idValue;
		}

		/// <summary>
		/// Gets or sets if the column needs to be quoted in SQL statements.
		/// </summary>
		/// <value><c>true</c> if the column is quoted.</value>
		public bool IsQuoted
		{
			get { return quoted; }
			set { quoted = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="constraint"></param>
		public void AddCheckConstraint( string constraint )
		{
			checkConstraints.Add( constraint );
		}
	}
}