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

		// TODO: understand this variable and its context/contents
		//private readonly bool[] propertyHasColumns;
		private bool[ ] propertyHasColumns;

		// the names of the columns for the property
		// the array is indexed as propertyColumnNames[propertyIndex][columnIndex] = "columnName"
		private readonly string[ ][ ] propertyColumnNames;

		// the alias names for the columns of the property.  This is used in the AS portion for 
		// selecting a column.  It is indexed the same as propertyColumnNames
		private readonly string[ ][ ] propertyColumnNameAliases;

		// the closure of all properties in the entire hierarchy including
		// subclasses and superclasses of this class
		private readonly string[ ][ ] subclassPropertyColumnNameClosure;
		private readonly int[ ] subclassPropertyTableNumberClosure;
		private readonly IType[ ] subclassPropertyTypeClosure;
		private readonly string[ ] subclassPropertyNameClosure;
		private readonly OuterJoinLoaderType[ ] subclassPropertyEnableJoinedFetch;
		private readonly bool[ ] propertyDefinedOnSubclass;

		private readonly Hashtable tableNumberByPropertyPath = new Hashtable();

		// the closure of all columns used by the entire hierarchy including
		// subclasses and superclasses of this class
		private readonly string[ ] subclassColumnClosure;
		private readonly string[ ] subclassColumnClosureAliases;
		private readonly int[ ] subclassColumnTableNumberClosure;

		// subclass discrimination works by assigning particular values to certain 
		// combinations of null primary key values in the outer join using an SQL CASE

		// key = DiscrimatorValue, value = Subclass Type
		private readonly Hashtable subclassesByDiscriminatorValue = new Hashtable();

		// the value the discrimator column will contain to indicate the type of object
		// the row contains.
		// the array is indexed as discriminatorValues[subclassIndex].  The subclassIndex comes
		// from the field subclassClosure
		private readonly string[ ] discriminatorValues;

		private readonly string[ ] notNullColumns;

		// TODO: figure out how this is used
		private readonly int[ ] tableNumbers;

		private readonly IDiscriminatorType discriminatorType;
		private readonly string discriminatorSQLString;
		private readonly string discriminatorColumnName;

		/// <summary></summary>
		protected IUniqueEntityLoader loader;
		/// <summary></summary>
		protected readonly IDictionary lockers = new Hashtable();

//		private static readonly string[ ] StringArray = {};
//		private static readonly IType[ ] TypeArray = {};

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
					throw new MappingException( "could not format discriminator value to SQL string", e );
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

			// convert the local ArrayList variables into arrays for the fields in the class
			this.tableNames = ( string[ ] ) tables.ToArray( typeof( string ) );
			this.tableKeyColumns = ( string[ ][ ] ) keyColumns.ToArray( typeof( string[ ] ) );

			// the description of these variables is the same as before
			ArrayList subtables = new ArrayList();
			keyColumns = new ArrayList();

			subtables.Add( this.qualifiedTableName );
			keyColumns.Add( IdentifierColumnNames );
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
			this.propertyTables = new int[this.hydrateSpan];
			this.naturalOrderPropertyTables = new int[this.hydrateSpan];
			this.propertyColumnNames = new string[this.hydrateSpan][ ];
			this.propertyColumnNameAliases = new string[this.hydrateSpan][ ];
			this.propertyColumnSpans = new int[this.hydrateSpan];

			Hashtable thisClassProperties = new Hashtable();
			// just a dummy object for the value so I can treat Hashtable like a Set
			object thisClassPropertiesObject = new object();

			int propertyIndex = 0;
			foreach( Mapping.Property prop in model.PropertyClosureCollection )
			{
				thisClassProperties.Add( prop, thisClassPropertiesObject );
				Table tab = prop.Value.Table;
				string tabname = tab.GetQualifiedName( Dialect, factory.DefaultSchema );

				this.propertyTables[ propertyIndex ] = GetTableId( tabname, this.tableNames );
				this.naturalOrderPropertyTables[ propertyIndex ] = GetTableId( tabname, this.naturalOrderTableNames );
				this.propertyColumnSpans[ propertyIndex ] = prop.ColumnSpan;

				string[ ] propCols = new string[propertyColumnSpans[ propertyIndex ]];
				string[ ] propAliases = new string[propertyColumnSpans[ propertyIndex ]];

				int columnIndex = 0;
				foreach( Column col in prop.ColumnCollection )
				{
					string colname = col.GetQuotedName( Dialect );
					propCols[ columnIndex ] = colname;
					propAliases[ columnIndex ] = col.Alias( Dialect, tab.UniqueInteger.ToString() + StringHelper.Underscore );
					columnIndex++;
				}

				this.propertyColumnNames[ propertyIndex ] = propCols;
				this.propertyColumnNameAliases[ propertyIndex ] = propAliases;

				propertyIndex++;
			}

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
			ArrayList types = new ArrayList(); //this.subclassPropertyTypeClosure
			ArrayList names = new ArrayList(); //this.subclassPropertyNameClosure
			ArrayList propColumns = new ArrayList(); //this.subclassPropertyColumnNameClosure
			ArrayList coltables = new ArrayList(); //this.subclassColumnTableNumberClosure
			ArrayList joinedFetchesList = new ArrayList(); //this.subclassPropertyEnableJoinedFetch
			ArrayList aliases = new ArrayList(); //this.subclassColumnClosureAlias
			ArrayList propTables = new ArrayList(); // this.subclassPropertyTableNameClosure
			ArrayList definedBySubclass = new ArrayList(); // this.propertyDefinedOnSubclass

			foreach( Mapping.Property prop in model.SubclassPropertyClosureCollection )
			{
				names.Add( prop.Name );
				definedBySubclass.Add( !thisClassProperties.Contains( prop ) );
				Table tab = prop.Value.Table;
				string tabname = tab.GetQualifiedName( Dialect, factory.DefaultSchema );
				string[ ] cols = new string[prop.ColumnSpan];
				types.Add( prop.Type );
				int tabnum = GetTableId( tabname, subclassTableNameClosure );
				propTables.Add( tabnum );

				int l = 0;
				foreach( Column col in prop.ColumnCollection )
				{
					columns.Add( col.GetQuotedName( Dialect ) );
					coltables.Add( tabnum );
					cols[ l++ ] = col.GetQuotedName( Dialect );
					aliases.Add( col.Alias( Dialect, tab.UniqueInteger.ToString() + StringHelper.Underscore ) );
				}
				propColumns.Add( cols );
				joinedFetchesList.Add( prop.Value.OuterJoinFetchSetting );
			}

			subclassColumnClosure = ( string[ ] ) columns.ToArray( typeof( string ) );
			subclassColumnClosureAliases = ( string[ ] ) aliases.ToArray( typeof( string ) );
			subclassColumnTableNumberClosure = ( int[ ] ) coltables.ToArray( typeof( int ) );
			subclassPropertyTypeClosure = ( IType[ ] ) types.ToArray( typeof( IType ) );
			subclassPropertyNameClosure = ( string[ ] ) names.ToArray( typeof( string ) );
			subclassPropertyTableNumberClosure = ( int[ ] ) propTables.ToArray( typeof( int ) );

			subclassPropertyColumnNameClosure = ( string[ ][ ] ) propColumns.ToArray( typeof( string[ ] ) );

			subclassPropertyEnableJoinedFetch = new OuterJoinLoaderType[joinedFetchesList.Count];
			int n = 0;
			foreach( OuterJoinLoaderType ojlType in joinedFetchesList )
			{
				subclassPropertyEnableJoinedFetch[ n++ ] = ojlType;
			}

			propertyDefinedOnSubclass = new bool[definedBySubclass.Count];
			n = 0;
			foreach( bool pdos in definedBySubclass )
			{
				propertyDefinedOnSubclass[ n++ ] = pdos;
			}

			// moved the sql generation to PostIntantiate

			System.Type mappedClass = model.PersistentClazz;

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
				this.tableNumbers[ subclassSpan - 1 ] = GetTableId(
					model.Table.GetQualifiedName( Dialect, factory.DefaultSchema ),
					this.subclassTableNameClosure );

				this.notNullColumns = new string[subclassSpan];
				foreach( Column col in model.Table.PrimaryKey.ColumnCollection )
				{
					notNullColumns[ subclassSpan - 1 ] = col.GetQuotedName( Dialect ); //only once
				}
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
				subclassClosure[ p ] = sc.PersistentClazz;
				try
				{
					if( model.IsPolymorphic )
					{
						int disc = p + 1;
						subclassesByDiscriminatorValue.Add( disc, sc.PersistentClazz );
						discriminatorValues[ p ] = disc.ToString();
						tableNumbers[ p ] = GetTableId(
							sc.Table.GetQualifiedName( Dialect, factory.DefaultSchema ),
							subclassTableNameClosure
							);
						foreach( Column col in sc.Table.PrimaryKey.ColumnCollection )
						{
							notNullColumns[ p ] = col.GetQuotedName( Dialect ); //only once;
						}
					}
				}
				catch( Exception e )
				{
					throw new MappingException( "Error parsing discriminator value", e );
				}
				p++;
			}

			// moved the propertyHasColumns into PostInstantiate

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		public override void PostInstantiate( ISessionFactoryImplementor factory )
		{
			InitPropertyPaths( factory );

			//TODO: move into InitPropertyPaths 
			Hashtable mods = new Hashtable();
			foreach( DictionaryEntry e in typesByPropertyPath )
			{
				IType type = ( IType ) e.Value;
				if( type.IsEntityType )
				{
					string path = ( string ) e.Key;
					object table = tableNumberByPropertyPath[ path ];
					string[ ] columns = ( string[ ] ) columnNamesByPropertyPath[ path ];
					if( columns.Length == 0 )
					{
						//ie a one-to-one association
						columns = IdentifierColumnNames;
						table = new int[0];
					}
					EntityType etype = ( EntityType ) type;
					IType idType = factory.GetIdentifierType( etype.PersistentClass );

					string idpath = path + StringHelper.Dot + PathExpressionParser.EntityID;
					mods.Add( idpath, idType );
					columnNamesByPropertyPath.Add( idpath, columns );
					tableNumberByPropertyPath.Add( idpath, table );
					if( idType.IsComponentType || idType.IsObjectType )
					{
						IAbstractComponentType actype = ( IAbstractComponentType ) idType;
						string[ ] props = actype.PropertyNames;
						IType[ ] subtypes = actype.Subtypes;
						if( actype.GetColumnSpan( factory ) != columns.Length )
						{
							throw new MappingException( "broken mapping for: " + ClassName + StringHelper.Dot + path );
						}

						int j = 0;
						for( int i = 0; i < props.Length; i++ )
						{
							string subidpath = idpath + StringHelper.Dot + props[ i ];
							string[ ] componentColumns = new string[subtypes[ i ].GetColumnSpan( factory )];
							for( int k = 0; k < componentColumns.Length; k++ )
							{
								componentColumns[ k ] = columns[ j++ ];
							}
							columnNamesByPropertyPath.Add( subidpath, componentColumns );
							tableNumberByPropertyPath.Add( subidpath, table );
							mods.Add( subidpath, actype.Subtypes[ i ] );
						}
					}
				}
			}

			foreach( DictionaryEntry de in mods )
			{
				typesByPropertyPath.Add( de.Key, de.Value );
			}

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

			SqlString lockString = GenerateLockString( null, null );
			SqlString lockExclusiveString = Dialect.SupportsForUpdate ?
				GenerateLockString( lockString, " FOR UPDATE" ) :
				GenerateLockString( lockString, null );
			SqlString lockExclusiveNowaitString = Dialect.SupportsForUpdateNoWait ?
				GenerateLockString( lockString, " FOR UPDATE NOWAIT" ) :
				GenerateLockString( lockString, null );

			lockers.Add( LockMode.Read, lockString );
			lockers.Add( LockMode.Upgrade, lockExclusiveString );
			lockers.Add( LockMode.UpgradeNoWait, lockExclusiveNowaitString );


			//TODO: find out why this was in the constructor in the spot it was...
			propertyHasColumns = new Boolean[sqlUpdateStrings.Length];
			for( int m = 0; m < sqlUpdateStrings.Length; m++ )
			{
				propertyHasColumns[ m ] = ( sqlUpdateStrings[ m ] != null );
			}

			loader = new EntityLoader( this, factory );
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

		/// <summary></summary>
		public override IDiscriminatorType DiscriminatorType
		{
			get { return discriminatorType; }
		}

		/// <summary></summary>
		public override string DiscriminatorSQLString
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
		public override OuterJoinLoaderType EnableJoinedFetch( int i )
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
			SqlString[ ] updateStrings = new SqlString[naturalOrderTableNames.Length];

			for( int j = 0; j < naturalOrderTableNames.Length; j++ )
			{
				SqlUpdateBuilder updateBuilder = new SqlUpdateBuilder( factory );

				updateBuilder.SetTableName( naturalOrderTableNames[ j ] );

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

				updateBuilder.SetIdentityColumn( naturalOrderTableKeyColumns[ j ], IdentifierType );

				if( j == 0 && IsVersioned )
				{
					updateBuilder.SetVersionColumn( new string[ ] {VersionColumnName}, VersionType );
				}

				updateStrings[ j ] = hasColumns ?
					updateBuilder.ToSqlString() :
					null;

			}

			return updateStrings;
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
		protected virtual SqlString GenerateLockString( SqlString sqlString, string forUpdateFragment )
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
			for( int j = 0; j < hydrateSpan; j++ )
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
		/// Do a version check
		/// </summary>
		/// <param name="id"></param>
		/// <param name="version"></param>
		/// <param name="obj"></param>
		/// <param name="lockMode"></param>
		/// <param name="session"></param>
		public override void Lock( object id, object version, object obj, LockMode lockMode, ISessionImplementor session )
		{
			if( lockMode != LockMode.None )
			{
				if( log.IsDebugEnabled )
				{
					log.Debug( "Locking entity: " + ClassName + '#' + id );
					if( IsVersioned )
					{
						log.Debug( "Version: " + version );
					}
				}

				IDbCommand st = session.Batcher.PrepareCommand( ( SqlString ) lockers[ lockMode ] );
				IDataReader rs = null;

				try
				{
					IdentifierType.NullSafeSet( st, id, 0, session );
					if( IsVersioned )
					{
						VersionType.NullSafeSet( st, version, IdentifierColumnNames.Length, session );
					}

					//					IDataReader rs = st.ExecuteReader();
					rs = session.Batcher.ExecuteReader( st );
					//					try 
					//					{
					if( !rs.Read() )
					{
						throw new StaleObjectStateException( MappedClass, id );
					}
					//					} 
					//					finally 
					//					{
					//						rs.Close();
					//					}
				} 
					//TODO: change this to catch a Sql Exception and log it
				catch( Exception e )
				{
					log.Error( e.Message, e );
					throw;
				}
				finally
				{
					session.Batcher.CloseCommand( st, rs );
				}
			}
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
		/// Update an object.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="fields"></param>
		/// <param name="dirtyFields"></param>
		/// <param name="oldVersion"></param>
		/// <param name="obj"></param>
		/// <param name="session"></param>
		public override void Update( object id, object[ ] fields, int[ ] dirtyFields, object oldVersion, object obj, ISessionImplementor session )
		{
			bool[ ] tableUpdateNeeded;
			if( dirtyFields == null )
			{
				tableUpdateNeeded = propertyHasColumns; //for object that came in via update()
			}
			else
			{
				tableUpdateNeeded = new bool[naturalOrderTableNames.Length];
				for( int i = 0; i < dirtyFields.Length; i++ )
				{
					tableUpdateNeeded[ naturalOrderPropertyTables[ dirtyFields[ i ] ] ] = true;
				}
				if( IsVersioned )
				{
					tableUpdateNeeded[ 0 ] = true;
				}
			}

			if( UseDynamicUpdate && dirtyFields != null )
			{
				bool[ ] propsToUpdate = new bool[hydrateSpan];
				for( int i = 0; i < hydrateSpan; i++ )
				{
					bool dirty = false;
					for( int j = 0; j < dirtyFields.Length; j++ )
					{
						if( dirtyFields[ j ] == i )
						{
							dirty = true;
						}
					}
					propsToUpdate[ i ] = dirty || VersionProperty == i;
				}

				Update( id, fields, propsToUpdate, tableUpdateNeeded, oldVersion, obj, GenerateUpdateStrings( propsToUpdate ), session );
			}
			else
			{
				Update( id, fields, PropertyUpdateability, tableUpdateNeeded, oldVersion, obj, SqlUpdateStrings, session );
			}
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


		//INITIALIZATION:


		private void InitPropertyPaths( IMapping mapping )
		{
			IType[ ] propertyTypes = PropertyTypes;
			string[ ] propertyNames = PropertyNames;

			for( int i = 0; i < propertyNames.Length; i++ )
			{
				InitPropertyPaths( propertyNames[ i ], propertyTypes[ i ], propertyColumnNames[ i ], propertyTables[ i ], mapping );
			}

			string idProp = IdentifierPropertyName;
			if( idProp != null )
			{
				InitPropertyPaths( idProp, IdentifierType, IdentifierColumnNames, 0, mapping );
			}
			if( HasEmbeddedIdentifier )
			{
				InitPropertyPaths( null, IdentifierType, IdentifierColumnNames, 0, mapping );
			}
			InitPropertyPaths( PathExpressionParser.EntityID, IdentifierType, IdentifierColumnNames, 0, mapping );

			typesByPropertyPath[ PathExpressionParser.EntityClass ] = DiscriminatorType;
			columnNamesByPropertyPath[ PathExpressionParser.EntityClass ] = new string[ ] {DiscriminatorColumnName};
			tableNumberByPropertyPath[ PathExpressionParser.EntityClass ] = 0;
		}

		private void InitPropertyPaths( string propertyName, IType propertyType, string[ ] columns, int table, IMapping mapping )
		{
			if( propertyName != null )
			{
				typesByPropertyPath[ propertyName ] = propertyType;
				columnNamesByPropertyPath[ propertyName ] = columns;
				tableNumberByPropertyPath[ propertyName ] = table;
			}

			if( propertyType.IsComponentType )
			{
				IAbstractComponentType compType = ( IAbstractComponentType ) propertyType;
				string[ ] props = compType.PropertyNames;
				IType[ ] types = compType.Subtypes;
				int count = 0;
				for( int k = 0; k < props.Length; k++ )
				{
					int len = types[ k ].GetColumnSpan( mapping );
					string[ ] slice = new string[len];
					for( int j = 0; j < len; j++ )
					{
						slice[ j ] = columns[ count++ ];
					}
					string path = ( propertyName == null ) ? props[ k ] : propertyName + '.' + props[ k ];
					InitPropertyPaths( path, types[ k ], slice, table, mapping );
				}
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
						StringHelper.Prefix( IdentifierColumnNames, name + StringHelper.Dot ),
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

			throw new AssertionFailure( "table [" + tableName + "] not found" );
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

			string[ ] cols = GetPropertyColumnNames( property );
			if( cols == null )
			{
				throw new QueryException( "unresolved property: " + property );
			}

			int tableIndex;
			if( cols.Length == 0 )
			{
				cols = IdentifierColumnNames;
				tableIndex = 0;
			}
			else
			{
				tableIndex = ( int ) tableNumberByPropertyPath[ property ];
			}

			//TODO: H2.0.3 synch - figure out what is different here and why it is different
			// make sure an Alias was actually passed into the statement
			if( alias != null && alias.Length > 0 )
			{
				return StringHelper.Prefix( cols, Alias( alias, tableIndex ) + StringHelper.Dot );
			}
			else
			{
				return cols;
			}

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
			string[ ] cols = subclassColumnClosure;
			SelectFragment frag = new SelectFragment( factory.Dialect )
				.SetSuffix( suffix );

			for( int i = 0; i < cols.Length; i++ )
			{
				frag.AddColumn(
					Alias( alias, subclassColumnTableNumberClosure[ i ] ),
					cols[ i ],
					subclassColumnClosureAliases[ i ]
					);
			}

			if( HasSubclasses )
			{
				SqlStringBuilder builder = new SqlStringBuilder( 3 );

				builder.Add( ", " );
				builder.Add(
					DiscriminatorFragment( alias )
						.SetReturnColumnName( DiscriminatorColumnName, suffix )
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
			return Dialect.QuoteForAliasName( Dialect.UnQuote( name ) + StringHelper.Underscore + tableNumber );
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

			if( HasWhere )
			{
				result = result.Append( " and " + GetSQLWhereString( alias ) );
			}

			return result;

		}

		/// <summary></summary>
		public override string[ ] IdentifierColumnNames
		{
			get { return tableKeyColumns[ 0 ]; }
		}


	}
}