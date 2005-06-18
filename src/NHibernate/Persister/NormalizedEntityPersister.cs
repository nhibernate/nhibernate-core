using System;
using System.Collections;
using System.Data;
using Iesi.Collections;
using log4net;
using NHibernate.Engine;
using NHibernate.Hql;
using NHibernate.Id;
using NHibernate.Loader;
using NHibernate.Mapping;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;
using Array = System.Array;

namespace NHibernate.Persister
{
	/// <summary>
	/// A <c>IClassPersister</c> implementing the normalized "table-per-subclass" mapping strategy
	/// </summary>
	public class NormalizedEntityPersister : AbstractEntityPersister
	{
		private readonly ISessionFactoryImplementor factory;

		// the class hierarchy structure
		private readonly string qualifiedTableName;

		// all of the table names that this Persister uses for just its data 
		// it is indexed as tableNames[tableIndex]
		// for both the superclass and subclass the index of 0=basetable
		// for the base class there is only 1 table
		private readonly string[ ] tableNames;
		private readonly string[ ] naturalOrderTableNames;

		// two dimensional array that is indexed as tableKeyColumns[tableIndex][columnIndex]
		private readonly string[ ][ ] tableKeyColumns;
		private readonly string[ ][ ] naturalOrderTableKeyColumns;
		private readonly bool hasFormulaProperties;

		// the Type of objects for the subclass
		// the array is indexed as subclassClosure[subclassIndex].  
		// The length of the array is the number of subclasses + 1 for the Base Class.
		// The last index of the array contains the Type for the Base Class.
		// in the example of JoinedSubclassBase/One the values are :
		// subclassClosure[0] = JoinedSubclassOne
		// subclassClosure[1] = JoinedSubclassBase
		private readonly System.Type[ ] subclassClosure;

		// the names of the tables for the subclasses
		// the array is indexed as subclassTableNameColsure[tableIndex] = "tableName"
		// for the RootClass the index 0 is the base table
		// for the subclass the index 0 is also the base table
		private readonly string[ ] subclassTableNameClosure;

		// the names of the columns that are the Keys for the table - I don't know why they would
		// be different - I thought the subclasses didn't have their own PK, but used the one generated
		// by the base class??
		// the array is indexed as subclassTableKeyColumns[tableIndex][columnIndex] = "columnName"
		private readonly string[ ][ ] subclassTableKeyColumns;

		// TODO: figure out what this is being used for - when initializing the base class the values
		// are isClassOrSuperclassTable[0] = true, isClassOrSuperclassTable[1] = false
		// when initialized the subclass the values are [0]=true, [1]=true.  I believe this is telling
		// us that the table is used to populate this class or the superclass.
		// I would guess this is telling us specifically which tables this Persister will write information to.
		private readonly bool[ ] isClassOrSuperclassTable;

		// SQL strings
		private SqlString[ ] sqlDeleteStrings;
		private SqlString[ ] sqlInsertStrings;
		private SqlString[ ] sqlIdentityInsertStrings;
		private SqlString[ ] sqlUpdateStrings;

		/* 
		 * properties of this class, including inherited properties
		 */

		// the number of columns that the property spans
		// the array is indexed as propertyColumnSpans[propertyIndex] = ##
		private readonly int[ ] propertyColumnSpans;

		// the index of the table that the property is coming from
		// the array is indexed as propertyTables[propertyIndex] = tableIndex 
		private readonly int[ ] propertyTables;
		private readonly int[ ] naturalOrderPropertyTables;
		private bool[ ] propertyHasColumns;

		// the names of the columns for the property
		// the array is indexed as propertyColumnNames[propertyIndex][columnIndex] = "columnName"
		private readonly string[ ][ ] propertyColumnNames;

		// the alias names for the columns of the property.  This is used in the AS portion for 
		// selecting a column.  It is indexed the same as propertyColumnNames
		private readonly string[ ][ ] propertyColumnNameAliases;
		private readonly string[ ] propertyFormulaTemplates;

		// the closure of all properties in the entire hierarchy including
		// subclasses and superclasses of this class
		private readonly string[ ][ ] subclassPropertyColumnNameClosure;
		private readonly int[ ] subclassPropertyTableNumberClosure;
		private readonly IType[ ] subclassPropertyTypeClosure;
		private readonly string[ ] subclassPropertyNameClosure;
		private readonly OuterJoinFetchStrategy[ ] subclassPropertyEnableJoinedFetch;
		private readonly bool[ ] propertyDefinedOnSubclass;

		private readonly Hashtable tableNumberByPropertyPath = new Hashtable();

		// the closure of all columns used by the entire hierarchy including
		// subclasses and superclasses of this class
		private readonly int[ ] subclassColumnTableNumberClosure;
		private readonly string[ ] subclassColumnClosure;
		private readonly string[ ] subclassColumnClosureAliases;
		private readonly int[ ] subclassFormulaTableNumberClosure;
		private readonly string[ ] subclassFormulaTemplateClosure;
		private readonly string[ ] subclassFormulaAliasClosure;

		// subclass discrimination works by assigning particular values to certain 
		// combinations of null primary key values in the outer join using an SQL CASE

		// key = DiscrimatorValue, value = Subclass Type
		private readonly Hashtable subclassesByDiscriminatorValue = new Hashtable();
		private readonly string[ ] discriminatorValues;
		private readonly string[ ] notNullColumns;
		private readonly int[ ] tableNumbers;

		private readonly IDiscriminatorType discriminatorType;
		private readonly string discriminatorSQLString;
		private readonly string discriminatorColumnName;
		private SqlString sqlConcreteSelectString;
		private SqlString sqlVersionSelectString;

		/// <summary></summary>
		protected IUniqueEntityLoader loader;

		private static readonly ILog log = LogManager.GetLogger( typeof( NormalizedEntityPersister ) );

		/// <summary>
		/// Constructs the NormalizedEntityPerister for the PersistentClass.
		/// </summary>
		/// <param name="model">The PeristentClass to create the EntityPersister for.</param>
		/// <param name="factory">The SessionFactory that this EntityPersister will be stored in.</param>
		public NormalizedEntityPersister( PersistentClass model, ISessionFactoryImplementor factory )
			: base( model, factory )
		{
			// I am am making heavy use of the "this." just to help me with debugging what is a local variable to the 
			// constructor versus what is an class scoped variable.  I am only doing this when we are using fields 
			// instead of properties because it is easy to tell properties by the Case.

			// CLASS + TABLE

			this.factory = factory;
			Table table = model.RootTable;
			this.qualifiedTableName = table.GetQualifiedName( Dialect, factory.DefaultSchema );

			// DISCRIMINATOR

			object discriminatorValue;
			if( model.IsPolymorphic )
			{
				// when we have a Polymorphic model then we are going to add a column "clazz_" to 
				// the sql statement that will be a large CASE statement where we will use the 
				// integer value to tell us which class to instantiate for the record.
				this.discriminatorColumnName = "clazz_";

				try
				{
					this.discriminatorType = ( IDiscriminatorType ) NHibernateUtil.Int32;
					discriminatorValue = 0;
					this.discriminatorSQLString = "0";
				}
				catch( Exception e )
				{
					throw new MappingException( "Could not format discriminator value '0' to sql string using the IType NHibernate.Types.Int32Type", e );
				}
			}
			else
			{
				this.discriminatorColumnName = null;
				this.discriminatorType = null;
				discriminatorValue = null;
				this.discriminatorSQLString = null;
			}

			//MULTITABLES

			// these two will later be converted into arrays for the fields tableNames and tableKeyColumns
			ArrayList tables = new ArrayList();
			ArrayList keyColumns = new ArrayList();
			tables.Add( this.qualifiedTableName );
			keyColumns.Add( base.IdentifierColumnNames );

			int idColumnSpan = IdentifierType.GetColumnSpan( factory );

			// move through each table that contains the data for this entity.
			foreach( Table tab in model.TableClosureCollection )
			{
				string tabname = tab.GetQualifiedName( Dialect, factory.DefaultSchema );
				if( !tabname.Equals( qualifiedTableName ) )
				{
					tables.Add( tabname );
					string[ ] key = new string[idColumnSpan];
					int k = 0;
					foreach( Column col in tab.PrimaryKey.ColumnCollection )
					{
						key[ k++ ] = col.GetQuotedName( Dialect );
					}
					keyColumns.Add( key );
				}
			}

			this.naturalOrderTableNames = ( string[ ] ) tables.ToArray( typeof( string ) );
			this.naturalOrderTableKeyColumns = ( string[ ][ ] ) keyColumns.ToArray( typeof( string[ ] ) );

			// the description of these variables is the same as before
			ArrayList subtables = new ArrayList();
			keyColumns = new ArrayList();
			subtables.Add( this.qualifiedTableName );
			keyColumns.Add( base.IdentifierColumnNames );
			foreach( Table tab in model.SubclassTableClosureCollection )
			{
				string tabname = tab.GetQualifiedName( Dialect, factory.DefaultSchema );
				if( !tabname.Equals( qualifiedTableName ) )
				{
					subtables.Add( tabname );
					string[ ] key = new string[idColumnSpan];
					int k = 0;
					foreach( Column col in tab.PrimaryKey.ColumnCollection )
					{
						key[ k++ ] = col.GetQuotedName( Dialect );
					}
					keyColumns.Add( key );
				}
			}

			// convert the local ArrayList variables into arrays for the fields in the class
			this.subclassTableNameClosure = ( string[ ] ) subtables.ToArray( typeof( string ) );
			this.subclassTableKeyColumns = ( string[ ][ ] ) keyColumns.ToArray( typeof( string[ ] ) );
			this.isClassOrSuperclassTable = new bool[this.subclassTableNameClosure.Length];
			for( int j = 0; j < subclassTableNameClosure.Length; j++ )
			{
				this.isClassOrSuperclassTable[ j ] = tables.Contains( this.subclassTableNameClosure[ j ] );
			}

			int len = naturalOrderTableNames.Length;
			tableNames = Reverse( naturalOrderTableNames );
			tableKeyColumns = Reverse( naturalOrderTableKeyColumns );
			Array.Reverse( subclassTableNameClosure, 0, len );
			Array.Reverse( subclassTableKeyColumns, 0, len );

			// PROPERTIES

			// initialize the lengths of all of the Property related fields in the class
			this.propertyTables = new int[HydrateSpan];
			this.naturalOrderPropertyTables = new int[HydrateSpan];
			this.propertyColumnNames = new string[HydrateSpan][ ];
			this.propertyColumnNameAliases = new string[HydrateSpan][ ];
			this.propertyColumnSpans = new int[HydrateSpan];
			this.propertyFormulaTemplates = new string[ HydrateSpan ];

			HashedSet thisClassProperties = new HashedSet();

			int i = 0;
			bool foundFormula = false;
			foreach( Mapping.Property prop in model.PropertyClosureCollection )
			{
				thisClassProperties.Add( prop );
				Table tab = prop.Value.Table;
				string tabname = tab.GetQualifiedName( Dialect, factory.DefaultSchema );
				this.propertyTables[ i ] = GetTableId( tabname, this.tableNames );
				this.naturalOrderPropertyTables[ i ] = GetTableId( tabname, this.naturalOrderTableNames );

				if ( prop.IsFormula )
				{
					this.propertyColumnNameAliases[ i ] = new string[] { prop.Formula.Alias };
					this.propertyColumnSpans[ i ] = 1;
					this.propertyFormulaTemplates[ i ] = prop.Formula.GetTemplate( Dialect );
					foundFormula = true;
				}
				else
				{
					this.propertyColumnSpans[ i ] = prop.ColumnSpan;

					string[ ] propCols = new string[propertyColumnSpans[ i ]];
					string[ ] propAliases = new string[propertyColumnSpans[ i ]];

					int j = 0;
					foreach( Column col in prop.ColumnCollection )
					{
						string colname = col.GetQuotedName( Dialect );
						propCols[ j ] = colname;
						propAliases[ j ] = col.Alias( Dialect, tab.UniqueInteger.ToString() + StringHelper.Underscore );
						j++;
					}
					this.propertyColumnNames[ i ] = propCols;
					this.propertyColumnNameAliases[ i ] = propAliases;
				}

				i++;
			}

			this.hasFormulaProperties = foundFormula;

			// check distinctness of columns for this specific subclass only
			HashedSet distinctColumns = new HashedSet();
			CheckColumnDuplication( distinctColumns, model.Key.ColumnCollection );
			foreach( Mapping.Property prop in model.PropertyCollection )
			{
				if( prop.IsUpdateable || prop.IsInsertable )
				{
					CheckColumnDuplication( distinctColumns, prop.ColumnCollection );
				}
			}

			// subclass closure properties

			ArrayList columns = new ArrayList(); //this.subclassColumnClosure
			ArrayList aliases = new ArrayList();
			ArrayList formulaAliases = new ArrayList();
			ArrayList formulaTemplates = new ArrayList();
			ArrayList types = new ArrayList(); //this.subclassPropertyTypeClosure
			ArrayList names = new ArrayList(); //this.subclassPropertyNameClosure
			ArrayList propColumns = new ArrayList(); //this.subclassPropertyColumnNameClosure
			ArrayList coltables = new ArrayList(); //this.subclassColumnTableNumberClosure
			ArrayList formtables = new ArrayList();
			ArrayList joinedFetchesList = new ArrayList(); //this.subclassPropertyEnableJoinedFetch
			ArrayList propTables = new ArrayList(); // this.subclassPropertyTableNameClosure
			ArrayList definedBySubclass = new ArrayList(); // this.propertyDefinedOnSubclass

			foreach( Mapping.Property prop in model.SubclassPropertyClosureCollection )
			{
				names.Add( prop.Name );
				definedBySubclass.Add( !thisClassProperties.Contains( prop ) );
				Table tab = prop.Value.Table;
				string tabname = tab.GetQualifiedName( Dialect, factory.DefaultSchema );
				int tabnum = GetTableId( tabname, subclassTableNameClosure );
				propTables.Add( tabnum );
				types.Add( prop.Type );

				if ( prop.IsFormula )
				{
					formulaTemplates.Add( prop.Formula.GetTemplate( Dialect ) );
					propColumns.Add( new string [] { } ) ;
					formulaAliases.Add( prop.Formula.Alias );
					formtables.Add( tabnum );
				}
				else
				{
					string[ ] cols = new string[prop.ColumnSpan];
					int l = 0;
					foreach( Column col in prop.ColumnCollection )
					{
						columns.Add( col.GetQuotedName( Dialect ) );
						coltables.Add( tabnum );
						cols[ l++ ] = col.GetQuotedName( Dialect );
						aliases.Add( col.Alias( Dialect, tab.UniqueInteger.ToString() + StringHelper.Underscore ) );
					}
					propColumns.Add( cols );
				}

				joinedFetchesList.Add( prop.Value.OuterJoinFetchSetting );
			}

			subclassColumnClosure = ( string[ ] ) columns.ToArray( typeof( string ) );
			subclassColumnClosureAliases = ( string[ ] ) aliases.ToArray( typeof( string ) );
			subclassColumnTableNumberClosure = ( int[ ] ) coltables.ToArray( typeof( int ) );
			subclassPropertyTypeClosure = ( IType[ ] ) types.ToArray( typeof( IType ) );
			subclassPropertyNameClosure = ( string[ ] ) names.ToArray( typeof( string ) );
			subclassPropertyTableNumberClosure = ( int[ ] ) propTables.ToArray( typeof( int ) );
			subclassFormulaAliasClosure = ( string[ ] ) formulaAliases.ToArray( typeof( string ) );
			subclassFormulaTemplateClosure = ( string[ ] ) formulaTemplates.ToArray( typeof( string ) );
			subclassFormulaTableNumberClosure = ( int[ ] ) formtables.ToArray( typeof( int ) );
			subclassPropertyColumnNameClosure = ( string[ ][ ] ) propColumns.ToArray( typeof( string[ ] ) );

			subclassPropertyEnableJoinedFetch = new OuterJoinFetchStrategy[ joinedFetchesList.Count ];
			int n = 0;
			foreach( OuterJoinFetchStrategy ojlType in joinedFetchesList )
			{
				subclassPropertyEnableJoinedFetch[ n++ ] = ojlType;
			}

			propertyDefinedOnSubclass = new bool[ definedBySubclass.Count ];
			n = 0;
			foreach( bool pdos in definedBySubclass )
			{
				propertyDefinedOnSubclass[ n++ ] = pdos;
			}

			// ****** Moved the sql generation to PostIntantiate *****

			System.Type mappedClass = model.MappedClass;

			// SUBCLASSES

			// all of the classes spanned, so even though there might be 2 subclasses we need to 
			// add in the baseclass - so we add 1 to the Closure
			int subclassSpan = model.SubclassSpan + 1;
			this.subclassClosure = new System.Type[subclassSpan];

			// start with the mapped class as the last element in the subclassClosure
			this.subclassClosure[ subclassSpan - 1 ] = mappedClass;

			if( model.IsPolymorphic )
			{
				this.subclassesByDiscriminatorValue.Add( discriminatorValue, mappedClass );
				this.discriminatorValues = new string[subclassSpan];
				this.discriminatorValues[ subclassSpan - 1 ] = discriminatorSQLString;

				this.tableNumbers = new int[subclassSpan];
				int id = GetTableId(
					model.Table.GetQualifiedName( Dialect, factory.DefaultSchema ),
					this.subclassTableNameClosure );

				this.tableNumbers[ subclassSpan - 1 ] = id;
				this.notNullColumns = new string[subclassSpan];
				this.notNullColumns[ subclassSpan - 1 ] = subclassTableKeyColumns[ id ][ 0 ];
				/*
				foreach( Column col in model.Table.PrimaryKey.ColumnCollection )
				{
					notNullColumns[ subclassSpan - 1 ] = col.GetQuotedName( Dialect ); //only once
				}
				*/
			}
			else
			{
				discriminatorValues = null;
				tableNumbers = null;
				notNullColumns = null;
			}

			int p = 0;
			foreach( Subclass sc in model.SubclassCollection )
			{
				subclassClosure[ p ] = sc.MappedClass;
				try
				{
					if( model.IsPolymorphic )
					{
						int disc = p + 1;
						subclassesByDiscriminatorValue.Add( disc, sc.MappedClass );
						discriminatorValues[ p ] = disc.ToString();
						int id = GetTableId(
							sc.Table.GetQualifiedName( Dialect, factory.DefaultSchema ),
							this.subclassTableNameClosure );
						tableNumbers[ p ] = id;
						notNullColumns[ p ] = subclassTableKeyColumns[ id ][ 0 ];
						/*
						foreach( Column col in sc.Table.PrimaryKey.ColumnCollection )
						{
							notNullColumns[ p ] = col.GetQuotedName( Dialect ); //only once;
						}
						*/
					}
				}
				catch( Exception e )
				{
					throw new MappingException( "Error parsing discriminator value", e );
				}
				p++;
			}

			// moved the propertyHasColumns into PostInstantiate as it needs the SQL strings

			// needs identifier info so moved to PostInstatiate
			//InitLockers( );

			InitSubclassPropertyAliasesMap( model );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		public override void PostInstantiate( ISessionFactoryImplementor factory )
		{
			InitPropertyPaths( factory );

			loader = CreateEntityLoader( factory );

			CreateUniqueKeyLoaders( factory );

			InitLockers( );

			// initialize the Statements - these are in the PostInstantiate method because we need
			// to have every other IClassPersister loaded so we can resolve the IType for the 
			// relationships.  In Hibernate they are able to just use ? and not worry about Parameters until
			// the statement is actually called.  We need to worry about Parameters when we are building
			// the IClassPersister...

			sqlDeleteStrings = GenerateDeleteStrings();
			sqlInsertStrings = GenerateInsertStrings( false, PropertyInsertability );
			sqlIdentityInsertStrings = IsIdentifierAssignedByInsert ?
				GenerateInsertStrings( true, PropertyInsertability ) :
				null;

			sqlUpdateStrings = GenerateUpdateStrings( PropertyUpdateability );

			sqlVersionSelectString = GenerateSelectVersionString( factory );
			sqlConcreteSelectString = GenerateConcreteSelectString();

			// This is used to determine updates for objects that came in via update()
			propertyHasColumns = new bool[sqlUpdateStrings.Length];
			for( int m = 0; m < sqlUpdateStrings.Length; m++ )
			{
				propertyHasColumns[ m ] = ( sqlUpdateStrings[ m ] != null );
			}
		}

		/// <summary>
		/// Create a new one dimensional array sorted in the Reverse order of the original array.
		/// </summary>
		/// <param name="objects">The original array.</param>
		/// <returns>A new array in the reverse order of the original array.</returns>
		private static string[ ] Reverse( string[ ] objects )
		{
			int len = objects.Length;
			string[ ] temp = new string[len];
			for( int i = 0; i < len; i++ )
			{
				temp[ i ] = objects[ len - i - 1 ];
			}
			return temp;
		}

		/// <summary>
		/// Create a new two dimensional array sorted in the Reverse order of the original array. The 
		/// second dimension is not reversed.
		/// </summary>
		/// <param name="objects">The original array.</param>
		/// <returns>A new array in the reverse order of the original array.</returns>
		private static string[ ][ ] Reverse( string[ ][ ] objects )
		{
			int len = objects.Length;
			string[ ][ ] temp = new string[len][ ];
			for( int i = 0; i < len; i++ )
			{
				temp[ i ] = objects[ len - i - 1 ];
			}
			return temp;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override bool IsDefinedOnSubclass( int i )
		{
			return propertyDefinedOnSubclass[ i ];
		}

		/// <summary></summary>
		public override string DiscriminatorColumnName
		{
			get { return discriminatorColumnName; }
		}

		/// <summary></summary>
		public override string DiscriminatorAlias
		{
			// Is always "clazz_", so just use columnName
			get { return DiscriminatorColumnName; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override IType GetSubclassPropertyType( int i )
		{
			return subclassPropertyTypeClosure[ i ];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override string GetSubclassPropertyName( int i )
		{
			return subclassPropertyNameClosure[ i ];
		}

		/// <summary></summary>
		public override int CountSubclassProperties()
		{
			return subclassPropertyTypeClosure.Length;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override string GetSubclassPropertyTableName( int i )
		{
			return subclassTableNameClosure[ subclassPropertyTableNumberClosure[ i ] ];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override string[ ] GetSubclassPropertyColumnNames( int i )
		{
			return subclassPropertyColumnNameClosure[ i ];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override string[ ] GetPropertyColumnNames( int i )
		{
			return propertyColumnNameAliases[ i ];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		protected override string[ ] GetActualPropertyColumnNames( int i )
		{
			return propertyColumnNames[ i ];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		protected override string GetFormulaTemplate( int i )
		{
			return propertyFormulaTemplates[ i ];
		}

		/// <summary></summary>
		public override IType DiscriminatorType
		{
			get { return discriminatorType; }
		}

		/*
		/// <summary></summary>
		public override string DiscriminatorSQLString
		{
			get { return discriminatorSQLString; }
		}
		*/

		/// <summary></summary>
		public override object DiscriminatorSQLValue
		{
			get { return discriminatorSQLString; }
		}

		/// <summary></summary>
		public virtual System.Type[ ] SubclassClosure
		{
			get { return subclassClosure; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override System.Type GetSubclassForDiscriminatorValue( object value )
		{
			return ( System.Type ) subclassesByDiscriminatorValue[ value ];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override OuterJoinFetchStrategy EnableJoinedFetch( int i )
		{
			return subclassPropertyEnableJoinedFetch[ i ];
		}

		/// <summary></summary>
		public override object IdentifierSpace
		{
			get { return qualifiedTableName; }
		}

		/// <summary></summary>
		public override object[ ] PropertySpaces
		{
			get
			{
				// don't need subclass tables, because they can't appear in conditions
				return tableNames;
			}
		}

		/// <summary>
		/// The SqlStrings to Delete this Entity
		/// </summary>
		protected SqlString[ ] SqlDeleteStrings
		{
			get { return sqlDeleteStrings; }
		}

		/// <summary>
		/// The SqlStrings to Insert this Entity
		/// </summary>
		protected SqlString[ ] SqlInsertStrings
		{
			get { return sqlInsertStrings; }
		}

		/// <summary>
		/// The SqlStrings to Insert this Entity when the DB handles
		/// generating the Identity.
		/// </summary>
		protected SqlString[ ] SqlIdentityInsertStrings
		{
			get { return sqlIdentityInsertStrings; }
		}

		/// <summary>
		/// The SqlStrings to Update this Entity
		/// </summary>
		protected SqlString[ ] SqlUpdateStrings
		{
			get { return sqlUpdateStrings; }
		}

		/// <summary>
		/// Generates an array of SqlStrings that encapsulates what later will be translated
		/// to ADO.NET IDbCommands to Delete this Entity.
		/// </summary>
		/// <returns>An array of SqlStrings </returns>
		protected virtual SqlString[ ] GenerateDeleteStrings()
		{
			SqlString[ ] deleteStrings = new SqlString[tableNames.Length];

			for( int i = 0; i < tableNames.Length; i++ )
			{
				SqlDeleteBuilder deleteBuilder = new SqlDeleteBuilder( factory );

				// TODO: find out why this is using tableKeyColumns and when
				// they would ever be different between the two tables - I thought
				// a requirement of Hibernate is that joined/subclassed tables
				// had to have the same pk - otherwise you had an association.
				deleteBuilder.SetTableName( naturalOrderTableNames[ i ] )
					.SetIdentityColumn( naturalOrderTableKeyColumns[ i ], IdentifierType );

				if( i == 0 && IsVersioned )
				{
					deleteBuilder.SetVersionColumn( new string[ ] {VersionColumnName}, VersionType );
				}

				deleteStrings[ i ] = deleteBuilder.ToSqlString();
			}

			return deleteStrings;
		}

		/// <summary>
		/// Generates SqlStrings that encapsulates what later will be translated
		/// to ADO.NET IDbCommands to Insert this Entity.
		/// </summary>
		/// <param name="identityInsert"></param>
		/// <param name="includeProperty"></param>
		/// <returns>An array of SqlStrings</returns>
		protected virtual SqlString[ ] GenerateInsertStrings( bool identityInsert, bool[ ] includeProperty )
		{
			SqlString[ ] insertStrings = new SqlString[tableNames.Length];

			for( int j = 0; j < naturalOrderTableNames.Length; j++ )
			{
				SqlInsertBuilder builder = new SqlInsertBuilder( factory );
				builder.SetTableName( naturalOrderTableNames[ j ] );

				for( int i = 0; i < PropertyTypes.Length; i++ )
				{
					if( includeProperty[ i ] && naturalOrderPropertyTables[ i ] == j )
					{
						builder.AddColumn( propertyColumnNames[ i ], PropertyTypes[ i ] );
					}
				}

				if( identityInsert && j == 0 )
				{
					// make sure the Dialect has an identity insert string because we don't want
					// to add the column when there is no value to supply the SqlBuilder
					if( Dialect.IdentityInsertString != null )
					{
						// only 1 column if there is IdentityInsert enabled.
						builder.AddColumn( naturalOrderTableKeyColumns[ j ][ 0 ], Dialect.IdentityInsertString );
					}
				}
				else
				{
					builder.AddColumn( naturalOrderTableKeyColumns[ j ], IdentifierType );
				}

				insertStrings[ j ] = builder.ToSqlString();
			}

			return insertStrings;
		}


		/// <summary>
		/// Generates SqlStrings that encapsulates what later will be translated
		/// to ADO.NET IDbCommands to Update this Entity.
		/// </summary>
		/// <param name="includeProperty"></param>
		/// <returns>An array of SqlStrings</returns>
		protected virtual SqlString[ ] GenerateUpdateStrings( bool[ ] includeProperty )
		{
			SqlString[ ] results = new SqlString[naturalOrderTableNames.Length];

			for( int j = 0; j < naturalOrderTableNames.Length; j++ )
			{
				SqlUpdateBuilder updateBuilder = new SqlUpdateBuilder( factory )
					.SetTableName( naturalOrderTableNames[ j ] )
					.SetIdentityColumn( naturalOrderTableKeyColumns[ j ], IdentifierType );

				if( j == 0 && IsVersioned )
				{
					updateBuilder.SetVersionColumn( new string[ ] {VersionColumnName}, VersionType );
				}

				//TODO: figure out what the hasColumns variable comes into play for??
				bool hasColumns = false;
				for( int i = 0; i < propertyColumnNames.Length; i++ )
				{
					if( includeProperty[ i ] && naturalOrderPropertyTables[ i ] == j )
					{
						updateBuilder.AddColumns( propertyColumnNames[ i ], PropertyTypes[ i ] );
						hasColumns = hasColumns || propertyColumnNames[ i ].Length > 0;
					}
				}
				results[ j ] = hasColumns ?	updateBuilder.ToSqlString() : null;
			}

			return results;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected virtual SqlString GenerateConcreteSelectString( )
		{
			// TODO: 2.1 This needs work - java version shown below
			/*
			String select = "select " + 
				StringHelper.join( 
					StringHelper.COMMA_SPACE, 
					StringHelper.qualify( CONCRETE_ALIAS, getIdentifierColumnNames() ) 
				) +
				concretePropertySelectFragment( CONCRETE_ALIAS, getPropertyUpdateability() ) + 
				" from " +
				fromTableFragment(CONCRETE_ALIAS) + 
				fromJoinFragment(CONCRETE_ALIAS, true, false) +
				" where " +
				StringHelper.join(
					"=? and ", 
					StringHelper.qualify( CONCRETE_ALIAS, getIdentifierColumnNames() ) 
				) +
				"=?" +
				whereJoinFragment(CONCRETE_ALIAS, true, false);
			if ( isVersioned() ) {
				select += 
					" and " + 
					getVersionColumnName() + 
					"=?";
			} 
			return select;
			*/
			const string ConcreteAlias = "x";

			SqlSimpleSelectBuilder builder = new SqlSimpleSelectBuilder( factory );

			// set the table and the identity columns
			builder.SetTableName( TableName )
				.AddColumns( StringHelper.Qualify( ConcreteAlias, IdentifierColumnNames ) );

			ConcretePropertySelectFragment( builder, ConcreteAlias, PropertyUpdateability );
			//FromTableFragment( ConcreteAlias );
			//FromJoinFragment( ConcreteAlias, true, false );
			//" where "
			builder.SetIdentityColumn( StringHelper.Qualify( ConcreteAlias, IdentifierColumnNames ), IdentifierType );
			//WhereJoinFragment( alias, true, false );
			if( IsVersioned )
			{
				builder.SetVersionColumn( new string[ ] { VersionColumnName }, VersionType );
			}

			return builder.ToSqlString();
		}

		private SqlString ConcretePropertySelectFragment( SqlSimpleSelectBuilder builder, string alias, bool[] includeProperty)
		{
			int propertyCount = propertyColumnNames.Length;
			SelectFragment frag = new SelectFragment( Dialect );

			for( int i = 0; i < HydrateSpan; i++ )
			{
				if( includeProperty[ i ] )
				{
					frag.AddColumns( Alias( alias, propertyTables[ i ] ), propertyColumnNames[ i ], propertyColumnNameAliases[ i ] );
				}
			}

			return frag.ToSqlStringFragment( );
		}

		/// <summary>
		/// Generates a SqlString that will append the forUpdateFragment to the sql.
		/// </summary>
		/// <param name="sqlString">An existing SqlString to copy for then new SqlString.</param>
		/// <param name="forUpdateFragment"></param>
		/// <returns>A new SqlString</returns>
		/// <remarks>
		/// The parameter <c>sqlString</c> does not get modified.  It is Cloned to make a new SqlString.
		/// If the parameter<c>sqlString</c> is null a new one will be created.
		/// </remarks>
		protected override SqlString GenerateLockString( SqlString sqlString, string forUpdateFragment )
		{
			SqlStringBuilder sqlBuilder = null;

			if( sqlString == null )
			{
				SqlSimpleSelectBuilder builder = new SqlSimpleSelectBuilder( factory );

				// set the table name and add the columns to select
				builder.SetTableName( qualifiedTableName )
					.AddColumn( base.IdentifierColumnNames[ 0 ] );

				// add the parameters to use in the WHERE clause
				builder.SetIdentityColumn( base.IdentifierColumnNames, IdentifierType );
				if( IsVersioned )
				{
					builder.SetVersionColumn( new string[ ] {VersionColumnName}, VersionType );
				}

				sqlBuilder = new SqlStringBuilder( builder.ToSqlString() );
			}
			else
			{
				sqlBuilder = new SqlStringBuilder( sqlString );
			}

			// add any special text that is contained in the forUpdateFragment
			if( forUpdateFragment != null )
			{
				sqlBuilder.Add( forUpdateFragment );
			}

			return sqlBuilder.ToSqlString();
		}


		/// <summary>
		/// Marshall the fields of a persistent instance to a properared statement
		/// </summary>
		/// <param name="id"></param>
		/// <param name="fields"></param>
		/// <param name="includeProperty"></param>
		/// <param name="statements"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		protected virtual int Dehydrate( object id, object[ ] fields, bool[ ] includeProperty, IDbCommand[ ] statements, ISessionImplementor session )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Dehydrating entity: " + ClassName + '#' + id );
			}

			int versionParm = 0;

			for( int i = 0; i < tableNames.Length; i++ )
			{
				int index = Dehydrate( id, fields, includeProperty, i, statements[ i ], session );
				if( i == 0 )
				{
					versionParm = index;
				}
			}

			return versionParm;
		}

		private int Dehydrate( object id, object[ ] fields, bool[ ] includeProperty, int table, IDbCommand statement, ISessionImplementor session )
		{
			if( statement == null )
			{
				return -1;
			}

			int index = 0;
			for( int j = 0; j < HydrateSpan; j++ )
			{
				if( includeProperty[ j ] && naturalOrderPropertyTables[ j ] == table )
				{
					PropertyTypes[ j ].NullSafeSet( statement, fields[ j ], index, session );
					index += propertyColumnSpans[ j ];
				}
			}

			if( id != null )
			{
				IdentifierType.NullSafeSet( statement, id, index, session );
				index += IdentifierColumnNames.Length;
			}

			return index;
		}

		//Execute the SQL:

		/// <summary>
		/// Load an instance using either the <c>ForUpdateLoader</c> or the outer joining <c>loader</c>,
		/// depending on the value of the <c>lock</c> parameter
		/// </summary>
		/// <param name="id"></param>
		/// <param name="optionalObject"></param>
		/// <param name="lockMode"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public override object Load( object id, object optionalObject, LockMode lockMode, ISessionImplementor session )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Materializing entity: " + ClassName + '#' + id );
			}
			object result = loader.Load( session, id, optionalObject );

			if( result != null )
			{
				Lock( id, GetVersion( result ), result, lockMode, session );
			}
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fields"></param>
		/// <param name="obj"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public override object Insert( object[ ] fields, object obj, ISessionImplementor session )
		{
			if( UseDynamicInsert )
			{
				bool[ ] notNull = GetNotNullInsertableColumns( fields );
				return Insert( fields, notNull, GenerateInsertStrings( true, notNull ), obj, session );
			}
			else
			{
				return Insert( fields, PropertyInsertability, SqlIdentityInsertStrings, obj, session );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="fields"></param>
		/// <param name="obj"></param>
		/// <param name="session"></param>
		public override void Insert( object id, object[ ] fields, object obj, ISessionImplementor session )
		{
			if( UseDynamicInsert )
			{
				bool[ ] notNull = GetNotNullInsertableColumns( fields );
				Insert( id, fields, notNull, GenerateInsertStrings( false, notNull ), obj, session );
			}
			else
			{
				Insert( id, fields, PropertyInsertability, SqlInsertStrings, obj, session );
			}
		}

		/// <summary>
		/// Persist an object
		/// </summary>
		/// <param name="id">The Id to give the new object/</param>
		/// <param name="fields">The fields to transfer to the Command</param>
		/// <param name="notNull"></param>
		/// <param name="sql"></param>
		/// <param name="obj">The object to Insert into the database.  I don't see where this is used???</param>
		/// <param name="session">The Session to use when Inserting the object.</param>
		public void Insert( object id, object[ ] fields, bool[ ] notNull, SqlString[ ] sql, object obj, ISessionImplementor session )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Inserting entity: " + ClassName + '#' + id );
				if( IsVersioned )
				{
					log.Debug( "Version: " + Versioning.GetVersion( fields, this ) );
				}
			}

			// render the SQL query
			IDbCommand[ ] insertCmds = new IDbCommand[tableNames.Length];

			try
			{
				for( int i = 0; i < tableNames.Length; i++ )
				{
					insertCmds[ i ] = session.Batcher.PrepareCommand( sql[ i ] );
				}

				// write the value of fields onto the prepared statements - we MUST use the state at the time
				// the insert was issued (cos of foreign key constraints).
				Dehydrate( id, fields, notNull, insertCmds, session );

				for( int i = 0; i < tableNames.Length; i++ )
				{
					session.Batcher.ExecuteNonQuery( insertCmds[ i ] );
				}

			} 
				//TODO: change this to SQLException catching and log it
			catch( Exception e )
			{
				log.Error( e.Message, e );
				throw;
			}
			finally
			{
				for( int i = 0; i < tableNames.Length; i++ )
				{
					if( insertCmds[ i ] != null )
					{
						session.Batcher.CloseCommand( insertCmds[ i ], null );
					}
				}
			}
		}

		/// <summary>
		/// Persist an object, using a natively generated identifier
		/// </summary>
		/// <param name="fields"></param>
		/// <param name="notNull"></param>
		/// <param name="sql"></param>
		/// <param name="obj"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public object Insert( object[ ] fields, bool[ ] notNull, SqlString[ ] sql, object obj, ISessionImplementor session )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Inserting entity: " + ClassName + " (native id)" );
				if( IsVersioned )
				{
					log.Debug( "Version: " + Versioning.GetVersion( fields, this ) );
				}
			}

			IDbCommand statement = null;
			IDbCommand idSelect = null;
			IDataReader rs = null;

			object id;

			if( Dialect.SupportsIdentitySelectInInsert )
			{
				statement = session.Batcher.PrepareCommand( Dialect.AddIdentitySelectToInsert( sql[ 0 ] ) );
				idSelect = statement;
			}
			else
			{
				// do not Prepare the Command to be part of a batch.  When the second command
				// is Prepared for the Batch that would cause the first one to be executed and
				// we don't want that yet.  
				statement = session.Batcher.Generate( sql[ 0 ] );
				idSelect = session.Batcher.PrepareCommand( new SqlString( SqlIdentitySelect ) );
			}

			try
			{
				Dehydrate( null, fields, notNull, 0, statement, session );
			}
			catch( Exception e )
			{
				throw new HibernateException( "NormalizedEntityPersister had a problem Dehydrating for an Insert", e );
			}

			try
			{
				// if it doesn't support identity select in insert then we have to issue the Insert
				// as a seperate command here
				if( Dialect.SupportsIdentitySelectInInsert == false )
				{
					session.Batcher.ExecuteNonQuery( statement );
				}

				rs = session.Batcher.ExecuteReader( idSelect );
				if( !rs.Read() )
				{
					throw new HibernateException( "The database returned no natively generated identity value" );
				}
				id = IdentifierGeneratorFactory.Get( rs, IdentifierType.ReturnedClass );

				log.Debug( "Natively generated identity: " + id );

			} 
				//TODO: change this to SQLException and log it
			catch( Exception e )
			{
				log.Error( e.Message, e );
				throw;
			}
			finally
			{
				if( Dialect.SupportsIdentitySelectInInsert == false )
				{
					session.Batcher.CloseCommand( statement, null );
				}
				session.Batcher.CloseCommand( idSelect, rs );
			}

			for( int i = 1; i < naturalOrderTableNames.Length; i++ )
			{
				statement = session.Batcher.PrepareCommand( sql[ i ] );
				try
				{
					Dehydrate( id, fields, notNull, i, statement, session );
					session.Batcher.ExecuteNonQuery( statement );
				} 
					//TODO: change this to SQLException and log it
				catch( Exception e )
				{
					log.Error( e.Message, e );
					throw;
				}
				finally
				{
					session.Batcher.CloseCommand( statement, null );
				}
			}
			return id;
		}

		/// <summary>
		/// Delete an object.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="version"></param>
		/// <param name="obj"></param>
		/// <param name="session"></param>
		public override void Delete( object id, object version, object obj, ISessionImplementor session )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Deleting entity: " + ClassName + '#' + id );
			}

			IDbCommand[ ] statements = new IDbCommand[naturalOrderTableNames.Length];
			try
			{
				for( int i = 0; i < naturalOrderTableNames.Length; i++ )
				{
					statements[ i ] = session.Batcher.PrepareCommand( SqlDeleteStrings[ i ] );
				}

				if( IsVersioned )
				{
					// don't need to add the 1 because the parameter indexes begin at 0, unlike jdbc's which begin at 1
					VersionType.NullSafeSet( statements[ 0 ], version, IdentifierColumnNames.Length, session );
				}

				for( int i = naturalOrderTableNames.Length - 1; i >= 0; i-- )
				{
					// Do the key. The key is immutable so we can use the _current_ object state
					IdentifierType.NullSafeSet( statements[ i ], id, 0, session );

					Check( session.Batcher.ExecuteNonQuery( statements[ i ] ), id );
				}
			} 
				//TODO: change to SqlException
			catch( Exception e )
			{
				log.Error( e.Message, e );
				throw;
			}
			finally
			{
				for( int i = 0; i < naturalOrderTableNames.Length; i++ )
				{
					if( statements[ i ] != null )
					{
						session.Batcher.CloseCommand( statements[ i ], null );
					}
				}
			}
		}

		/// <summary>
		/// Decide which tables need to be updated
		/// </summary>
		/// <param name="dirtyFields"></param>
		/// <returns></returns>
		private bool[] GetTableUpdateNeeded( int[] dirtyFields )
		{
			if( dirtyFields == null )
			{
				return propertyHasColumns; //for object that came in via update()
			}
			else
			{
				bool[ ] tableUpdateNeeded = new bool[naturalOrderTableNames.Length];
				for( int i = 0; i < dirtyFields.Length; i++ )
				{
					tableUpdateNeeded[ naturalOrderPropertyTables[ dirtyFields[ i ] ] ] = true;
				}
				if( IsVersioned )
				{
					tableUpdateNeeded[ 0 ] = true;
				}
				return tableUpdateNeeded;
			}
		}

		/// <summary>
		/// Update an object.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="fields"></param>
		/// <param name="dirtyFields"></param>
		/// <param name="oldFields"></param>
		/// <param name="oldVersion"></param>
		/// <param name="obj"></param>
		/// <param name="session"></param>
		public override void Update( object id, object[ ] fields, int[ ] dirtyFields, object[] oldFields, object oldVersion, object obj, ISessionImplementor session )
		{
			bool[ ] tableUpdateNeeded = GetTableUpdateNeeded( dirtyFields );

			SqlString[] updateStrings;
			bool[] propsToUpdate;
			if( UseDynamicUpdate && dirtyFields != null )
			{
				// decide which columns we really need to update
				propsToUpdate = GetPropertiesToUpdate( dirtyFields );
				updateStrings = GenerateUpdateStrings( propsToUpdate );
			}
			else
			{
				// just update them all
				propsToUpdate = PropertyUpdateability;
				updateStrings = SqlUpdateStrings;
			}
			Update( id, fields, propsToUpdate, tableUpdateNeeded, oldVersion, obj, updateStrings, session );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="fields"></param>
		/// <param name="includeProperty"></param>
		/// <param name="includeTable"></param>
		/// <param name="oldVersion"></param>
		/// <param name="obj"></param>
		/// <param name="sql"></param>
		/// <param name="session"></param>
		protected virtual void Update( object id, object[ ] fields, bool[ ] includeProperty, bool[ ] includeTable, object oldVersion, object obj, SqlString[ ] sql, ISessionImplementor session )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Updating entity: " + ClassName + '#' + id );
				if( IsVersioned )
				{
					log.Debug( "Existing version: " + oldVersion + " -> New version: " + fields[ VersionProperty ] );
				}
			}

			int tables = naturalOrderTableNames.Length;

			IDbCommand[ ] statements = new IDbCommand[tables];

			try
			{
				for( int i = 0; i < tables; i++ )
				{
					if( includeTable[ i ] )
					{
						statements[ i ] = session.Batcher.PrepareCommand( sql[ i ] );
					}
				}

				int versionParam = Dehydrate( id, fields, includeProperty, statements, session );

				if( IsVersioned )
				{
					VersionType.NullSafeSet( statements[ 0 ], oldVersion, versionParam, session );
				}

				for( int i = 0; i < tables; i++ )
				{
					if( includeTable[ i ] )
					{
						Check( session.Batcher.ExecuteNonQuery( statements[ i ] ), id );
					}
				}
			} 
				// TODO: change to SQLException and log
			catch( Exception e )
			{
				log.Error( e.Message, e );
				throw;
			}
			finally
			{
				for( int i = 0; i < tables; i++ )
				{
					if( statements[ i ] != null )
					{
						session.Batcher.CloseCommand( statements[ i ], null );
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		protected int GetPropertyTableNumber( string propertyName )
		{
			string[] propertyNames = PropertyNames;

			for ( int i = 0; i < propertyNames.Length; i++ )
			{
				if ( propertyName.Equals( propertyNames[ i ] ) )
				{
					return propertyTables[ i ];
				}
			}
			return 0;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="type"></param>
		protected override void HandlePath( string path, IType type )
		{
			if ( type.IsAssociationType && ( (IAssociationType) type ).UsePrimaryKeyAsForeignKey )
			{
				tableNumberByPropertyPath[ path ] = 0 ;
			}
			else
			{
				string propertyName = StringHelper.Root( path );
				tableNumberByPropertyPath[ path ] = GetPropertyTableNumber( propertyName );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		public override SqlString FromTableFragment( string alias )
		{
			return new SqlString( subclassTableNameClosure[ 0 ] + ' ' + alias );
		}

		/// <summary></summary>
		public override string TableName
		{
			get { return subclassTableNameClosure[ 0 ]; }
		}


		private JoinFragment Outerjoin( string name, bool innerJoin, bool includeSubclasses )
		{
			JoinFragment outerjoin = factory.Dialect.CreateOuterJoinFragment();
			for( int i = 1; i < subclassTableNameClosure.Length; i++ )
			{
				if( includeSubclasses || isClassOrSuperclassTable[ i ] )
				{
					outerjoin.AddJoin(
						subclassTableNameClosure[ i ],
						Alias( name, i ),
						StringHelper.Qualify( name, IdentifierColumnNames ),
						subclassTableKeyColumns[ i ],
						innerJoin && isClassOrSuperclassTable[ i ] ? JoinType.InnerJoin : JoinType.LeftOuterJoin );
				}
			}
			return outerjoin;
		}

		/// <summary>
		/// Find the Index of the table name from a list of table names.
		/// </summary>
		/// <param name="tableName">The name of the table to find.</param>
		/// <param name="tables">The array of table names</param>
		/// <returns>The Index of the table in the array.</returns>
		/// <exception cref="AssertionFailure">Thrown when the tableName specified can't be found</exception>
		private int GetTableId( string tableName, string[ ] tables )
		{
			for( int tableIndex = 0; tableIndex < tables.Length; tableIndex++ )
			{
				if( tableName.Equals( tables[ tableIndex ] ) )
				{
					return tableIndex;
				}
			}

			throw new AssertionFailure( string.Format( "table [{0}] not found", tableName ) );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="property"></param>
		/// <returns></returns>
		public override string[ ] ToColumns( string alias, string property )
		{
			if( PathExpressionParser.EntityClass.Equals( property ) )
			{
				// This doesn't actually seem to work but it *might* 
				// work on some dbs. Also it doesn't work if there 
				// are multiple columns of results because it 
				// is not accounting for the suffix: 
				// return new String[] { getDiscriminatorColumnName() }; 

				//TODO: this will need to be changed to return a SqlString but for now the SqlString
				// is being converted to a string for existing interfaces to work.
				return new string[ ] {DiscriminatorFragment( alias ).ToSqlStringFragment().ToString()};
			}

			int tab = (int) tableNumberByPropertyPath[ property ];

			return base.ToColumns( Alias( alias, tab ), property );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		public override string[ ] ToColumns( string alias, int i )
		{
			int tab = subclassPropertyTableNumberClosure[ i ];
			return StringHelper.Prefix(
				subclassPropertyColumnNameClosure[ i ],
				Alias( alias, tab ) + StringHelper.Dot );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="suffix"></param>
		/// <returns></returns>
		public override SqlString PropertySelectFragment( string alias, string suffix )
		{
			SelectFragment frag = new SelectFragment( factory.Dialect )
				.SetSuffix( suffix )
				.SetUsedAliases( IdentifierAliases );

			for( int i = 0; i < subclassColumnClosure.Length; i++ )
			{
				string subalias = Alias( alias, subclassColumnTableNumberClosure[ i ] );
				frag.AddColumn( subalias, subclassColumnClosure[ i ], subclassColumnClosureAliases[ i ]	);
			}

			for( int i = 0; i < subclassFormulaTemplateClosure.Length; i++ )
			{
				string subalias = Alias( alias, subclassFormulaTableNumberClosure[ i ] );
				frag.AddFormula( subalias, subclassFormulaTemplateClosure[ i ], subclassFormulaAliasClosure[ i ]	);
			}

			if( HasSubclasses )
			{
				SqlStringBuilder builder = new SqlStringBuilder( 3 );

				builder.Add( StringHelper.CommaSpace );
				builder.Add(
					DiscriminatorFragment( alias )
						.SetReturnColumnName( DiscriminatorAlias, suffix )
						.ToSqlStringFragment()
					);

				builder.Add( frag.ToSqlStringFragment() );

				return builder.ToSqlString();
			}
			else
			{
				return frag.ToSqlStringFragment();
			}
		}


		private CaseFragment DiscriminatorFragment( string alias )
		{
			CaseFragment cases = Dialect.CreateCaseFragment();

			for( int i = 0; i < discriminatorValues.Length; i++ )
			{
				cases.AddWhenColumnNotNull(
					Alias( alias, tableNumbers[ i ] ),
					notNullColumns[ i ],
					discriminatorValues[ i ]
					);
			}
			return cases;
		}

		private string Alias( string name, int tableNumber )
		{
			if( tableNumber == 0 )
			{
				return name;
			}
			return Dialect.QuoteForAliasName( Dialect.UnQuote( name ) + StringHelper.Underscore + tableNumber + StringHelper.Underscore );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="innerJoin"></param>
		/// <param name="includeSubclasses"></param>
		/// <returns></returns>
		public override SqlString FromJoinFragment( string alias, bool innerJoin, bool includeSubclasses )
		{
			return Outerjoin( alias, innerJoin, includeSubclasses ).ToFromFragmentString;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="innerJoin"></param>
		/// <param name="includeSubclasses"></param>
		/// <returns></returns>
		public override SqlString WhereJoinFragment( string alias, bool innerJoin, bool includeSubclasses )
		{
			return Outerjoin( alias, innerJoin, includeSubclasses ).ToWhereFragmentString;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="innerJoin"></param>
		/// <param name="includeSubclasses"></param>
		/// <returns></returns>
		public override SqlString QueryWhereFragment( string alias, bool innerJoin, bool includeSubclasses )
		{
			SqlString result = WhereJoinFragment( alias, innerJoin, includeSubclasses );
			string rootAlias = Alias( alias, naturalOrderTableNames.Length - 1 ); // urgh, ugly!!
			if( HasWhere )
			{
				result = result.Append( " and " + GetSQLWhereString( rootAlias ) );
			}

			return result;
		}

		/// <summary></summary>
		public override string[ ] IdentifierColumnNames
		{
			get { return tableKeyColumns[ 0 ]; }
		}

		/// <summary></summary>
		protected override SqlString ConcreteSelectString
		{
			get { return sqlConcreteSelectString; }
		}

		/// <summary></summary>
		public override bool IsCacheInvalidationRequired
		{
			get { return hasFormulaProperties || ( !IsVersioned && UseDynamicUpdate ); }
		}

		/// <summary></summary>
		protected override string VersionedTableName
		{
			get	{ return qualifiedTableName; }
		}

		/// <summary></summary>
		protected override SqlString VersionSelectString
		{
			get { return sqlVersionSelectString; }
		}
	}
}