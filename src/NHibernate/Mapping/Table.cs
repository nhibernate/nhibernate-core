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
		private Value idValue;
		private PrimaryKey primaryKey;
		private IDictionary indexes = new Hashtable();
		private IDictionary foreignKeys = new Hashtable();
		private IDictionary uniqueKeys = new Hashtable();
		private int uniqueInteger;
		private bool quoted;
		private static int tableCounter = 0;

		/// <summary></summary>
		public Table()
		{
			uniqueInteger = tableCounter++;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dialect"></param>
		/// <returns></returns>
		public string GetQualifiedName( Dialect.Dialect dialect )
		{
			string quotedName = GetQuotedName( dialect );
			return schema == null ? quotedName : schema + StringHelper.Dot + quotedName;
		}


		/// <summary>
		/// Returns the QualifiedName for the table using the specified Qualifier
		/// </summary>
		/// <param name="dialect"></param>
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

		/// <summary></summary>
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
		/// 
		/// </summary>
		/// <param name="dialect"></param>
		/// <returns></returns>
		public string GetQuotedName( Dialect.Dialect dialect )
		{
			return IsQuoted ?
				dialect.QuoteForTableName( name ) :
				name;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="n"></param>
		/// <returns></returns>
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
		/// 
		/// </summary>
		/// <param name="column"></param>
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

		/// <summary></summary>
		public int ColumnSpan
		{
			get { return columns.Count; }
		}

		/// <summary></summary>
		public ICollection ColumnCollection
		{
			get { return columns.Values; }
		}

		/// <summary></summary>
		public ICollection IndexCollection
		{
			get { return indexes.Values; }
		}

		/// <summary></summary>
		public ICollection ForeignKeyCollection
		{
			get { return foreignKeys.Values; }
		}

		/// <summary></summary>
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
		/// 
		/// </summary>
		/// <param name="dialect"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public string SqlCreateString( Dialect.Dialect dialect, IMapping p )
		{
			StringBuilder buf = new StringBuilder( "create table " )
				.Append( GetQualifiedName( dialect ) )
				.Append( " (" );

			bool identityColumn = idValue != null && idValue.CreateIdentifierGenerator( dialect ) is IdentityGenerator;

			// try to find out the name of the pk to create it as identity if the identitygenerator is used
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
					.Append( ' ' )
					.Append( col.GetSqlType( dialect, p ) );

				if( identityColumn && col.GetQuotedName( dialect ).Equals( pkname ) )
				{
					buf.Append( ' ' )
						.Append( dialect.IdentityColumnString );
				}
				else
				{
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
				buf.Append( ',' ).Append( primaryKey.SqlConstraintString( dialect ) );
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
		/// 
		/// </summary>
		/// <param name="dialect"></param>
		/// <returns></returns>
		public string SqlDropString( Dialect.Dialect dialect )
		{
			return "drop table " + GetQualifiedName( dialect ) + dialect.CascadeConstraintsString;
		}

		/// <summary></summary>
		public PrimaryKey PrimaryKey
		{
			get { return primaryKey; }
			set { primaryKey = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
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
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
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
		/// 
		/// </summary>
		/// <param name="columns"></param>
		/// <returns></returns>
		public ForeignKey CreateForeignKey( IList columns )
		{
			string name = "FK" + UniqueColumnString( columns );
			ForeignKey fk = ( ForeignKey ) foreignKeys[ name ];

			if( fk == null )
			{
				fk = new ForeignKey();
				fk.Name = name;
				fk.Table = this;
				foreignKeys.Add( name, fk );
			}

			foreach( Column col in columns )
			{
				fk.AddColumn( col );
			}

			return fk;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="col"></param>
		/// <returns></returns>
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

			return ( name.GetHashCode().ToString( "X" ) + result.GetHashCode().ToString( "X" ) );
		}

		/// <summary></summary>
		public string Schema
		{
			get { return schema; }
			set { schema = value; }
		}

		/// <summary></summary>
		public int UniqueInteger
		{
			get { return uniqueInteger; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="idValue"></param>
		public void SetIdentifierValue( Value idValue )
		{
			this.idValue = idValue;
		}

		/// <summary></summary>
		public bool IsQuoted
		{
			get { return quoted; }
			set { quoted = value; }
		}


	}
}