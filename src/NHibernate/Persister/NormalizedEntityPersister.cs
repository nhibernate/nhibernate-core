using System;
using System.Text;
using System.Data;
using System.Collections;

using NHibernate.Engine;
using NHibernate.Hql;
using NHibernate.Id;
using NHibernate.Loader;
using NHibernate.Mapping;
using NHibernate.Sql;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;


namespace NHibernate.Persister {
	/// <summary>
	/// A <c>IClassPersister</c> implementing the normalized "table-per-subclass" mapping strategy
	/// </summary>
	public class NormalizedEntityPersister : AbstractEntityPersister {
		
		private readonly ISessionFactoryImplementor factory;
		
		// SYNCHISSUE: there are some extra fields added in Hibernate that start
		// with naturalOrder**.  My guess is that might be because we are checking
		// to see if the column is distinct or not????

		// the class hierarchy structure
		private readonly string qualifiedTableName;
		
		// all of the table names that this Persister uses for just its data 
		// it is indexed as tableNames[tableIndex]
		// for both the superclass and subclass the index of 0=basetable
		// for the base class there is only 1 table
		private readonly string[] tableNames;

		// two dimensional array that is indexed as tableKeyColumns[tableIndex][columnIndex]
		private readonly string[][] tableKeyColumns;
		
		// the Type of objects for the subclass
		// the array is indexed as subclassClosure[subclassIndex].  
		// The length of the array is the number of subclasses + 1 for the Base Class.
		// The last index of the array contains the Type for the Base Class.
		// in the example of JoinedSubclassBase/One the values are :
		// subclassClosure[0] = JoinedSubclassOne
		// subclassClosure[1] = JoinedSubclassBase
		private readonly System.Type[] subclassClosure;

		// the names of the tables for the subclasses
		// the array is indexed as subclassTableNameColsure[tableIndex] = "tableName"
		// for the RootClass the index 0 is the base table
		// for the subclass the index 0 is also the base table
		private readonly string[] subclassTableNameClosure;

		// the names of the columns that are the Keys for the table - I don't know why they would
		// be different - I thought the subclasses didn't have their own PK, but used the one generated
		// by the base class??
		// the array is indexed as subclassTableKeyColumns[tableIndex][columnIndex] = "columnName"
		private readonly string[][] subclassTableKeyColumns;

		// TODO: figure out what this is being used for - when initializing the base class the values
		// are isClassOrSuperclassTable[0] = true, isClassOrSuperclassTable[1] = false
		// when initialized the subclass the values are [0]=true, [1]=true.  I believe this is telling
		// us that the table is used to populate this class or the superclass.
		// I would guess this is telling us specifically which tables this Persister will write information to.
		private readonly bool[] isClassOrSuperclassTable;

		private SqlString[] sqlDeleteStrings;
		private SqlString[] sqlInsertStrings;
		private SqlString[] sqlIdentityInsertStrings;
		private SqlString[] sqlUpdateStrings;
		
		/* 
		 * properties of this class, including inherited properties
		 */

		// the number of columns that the property spans
		// the array is indexed as propertyColumnSpans[propertyIndex] = ##
		private readonly int[] propertyColumnSpans;

		// the index of the table that the property is coming from
		// the array is indexed as propertyTables[propertyIndex] = tableIndex 
		private readonly int[] propertyTables;

		// TODO: understand this variable and its context/contents
		//private readonly bool[] propertyHasColumns;
		private bool[] propertyHasColumns;

		// the names of the columns for the property
		// the array is indexed as propertyColumnNames[propertyIndex][columnIndex] = "columnName"
		private readonly string[][] propertyColumnNames;

		// the alias names for the columns of the property.  This is used in the AS portion for 
		// selecting a column.  It is indexed the same as propertyColumnNames
		private readonly string[][] propertyColumnNameAliases;

		// the closure of all properties in the entire hierarchy including
		// subclasses and superclasses of this class
		private readonly string[][] subclassPropertyColumnNameClosure;
		private readonly int[] subclassPropertyTableNumberClosure;
		private readonly IType[] subclassPropertyTypeClosure;
		private readonly string[] subclassPropertyNameClosure;
		private readonly OuterJoinLoaderType[] subclassPropertyEnableJoinedFetch;
		private readonly bool[] propertyDefinedOnSubclass;
		private readonly Hashtable tableNumberByPropertyPath = new Hashtable();

		// the closure of all columns used by the entire hierarchy including
		// subclasses and superclasses of this class
		private readonly string[] subclassColumnClosure;
		private readonly string[] subclassColumnClosureAliases;
		private readonly int[] subclassColumnTableNumberClosure;
		
		// subclass discrimination works by assigning particular values to certain 
		// combinations of null primary key values in the outer join using an SQL CASE

		// key = DiscrimatorValue, value = Subclass Type
		private readonly Hashtable subclassesByDiscriminatorValue = new Hashtable();
		
		// the value the discrimator column will contain to indicate the type of object
		// the row contains.
		// the array is indexed as discriminators[subclassIndex].  The subclassIndex comes
		// from the field subclassClosure
		private readonly string[] discriminators;

		private readonly string[] notNullColumns;
		
		// TODO: figure out how this is used
		private readonly int[] tableNumbers;

		private readonly IDiscriminatorType discriminatorType;
		private readonly string discriminatorSQLString;
		private readonly string discriminatorColumnName;

		protected IUniqueEntityLoader loader;
		protected readonly IDictionary lockers = new Hashtable();
		
		private readonly bool[] allProperties;

		private static readonly string[] StringArray = {};
		private static readonly IType[] TypeArray = {};

		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(NormalizedEntityPersister)); 

		/// <summary>
		/// Constructs the NormalizedEntityPerister for the PersistentClass.
		/// </summary>
		/// <param name="model">The PeristentClass to create the EntityPersister for.</param>
		/// <param name="factory">The SessionFactory that this EntityPersister will be stored in.</param>
		public NormalizedEntityPersister(PersistentClass model, ISessionFactoryImplementor factory) : base(model, factory) {

			// I am am making heavy use of the "this." just to help me with debugging what is a local variable to the 
			// constructor versus what is an class scoped variable.  I am only doing this when we are using fields 
			// instead of properties because it is easy to tell properties by the Case.

			// CLASS + TABLE

			this.factory = factory;
			Table table = model.RootTable;
			this.qualifiedTableName = table.GetQualifiedName( factory.DefaultSchema );

			// DISCRIMINATOR

			object discriminatorValue;
			if ( model.IsPolymorphic ) {
				
				// when we have a Polymorphic model then we are going to add a column "clazz_" to 
				// the sql statement that will be a large CASE statement where we will use the 
				// integer value to tell us which class to instantiate for the record.
				this.discriminatorColumnName = "clazz_";

				try {
					this.discriminatorType = (IDiscriminatorType) NHibernate.Int32;
					discriminatorValue = 0;
					this.discriminatorSQLString = "0";
				} 
				catch (Exception e) {
					throw new MappingException("could not format discriminator value to SQL string", e);
				}
			} 
			else {
				this.discriminatorColumnName = null;
				this.discriminatorType = null;
				discriminatorValue = null;
				this.discriminatorSQLString = null;
			}

			//MULTITABLES
			
			// TODO - find out when multitables are used??  I have only come at this through the debug
			// for the base class (RootClass).  This might become a little bit more clear when I come at this through
			// the subclass (Subclass).

			// these two will later be converted into arrays for the fields tableNames and tableKeyColumns
			ArrayList tables = new ArrayList();
			ArrayList keyColumns = new ArrayList();

			tables.Add(this.qualifiedTableName);
			keyColumns.Add( IdentifierColumnNames );

			int idColumnSpan = IdentifierType.GetColumnSpan(factory);
			
			// move through each table that contains the data for this entity.
			foreach(Table tab in model.TableClosureCollection) {
				string tabname = tab.GetQualifiedName( factory.DefaultSchema );
				if ( !tabname.Equals(qualifiedTableName) ) {
					tables.Add( tabname );
					string[] key = new string[idColumnSpan];
					int k=0;
					foreach(Column col in tab.PrimaryKey.ColumnCollection ) key[k++] = col.Name;
					keyColumns.Add(key);
				}
			}
			
			// convert the local ArrayList variables into arrays for the fields in the class
			this.tableNames = (string[]) tables.ToArray( typeof(string) );
			this.tableKeyColumns = (string[][]) keyColumns.ToArray( typeof(string[]) );

			// the description of these variables is the same as before
			ArrayList subtables = new ArrayList();
			keyColumns = new ArrayList();

			subtables.Add( this.qualifiedTableName );
			keyColumns.Add( IdentifierColumnNames );
			foreach(Table tab in model.SubclassTableClosureCollection ) {
				string tabname = tab.GetQualifiedName(factory.DefaultSchema);
				if ( !tabname.Equals(qualifiedTableName) ) {
					subtables.Add(tabname);
					string[] key = new string[idColumnSpan];
					int k=0;
					foreach(Column col in tab.PrimaryKey.ColumnCollection ) key[k++] = col.Name;
					keyColumns.Add(key);
				}
			}

			// convert the local ArrayList variables into arrays for the fields in the class
			this.subclassTableNameClosure = (string[]) subtables.ToArray( typeof(string) );
			this.subclassTableKeyColumns = (string[][]) keyColumns.ToArray( typeof(string[]) );
			
			//TODO: figure out exactly how this field is being used.  I know that for our Simple example
			// the values for the base class are isClassOrSuperclassTable[0] = true; [1] = false;
			this.isClassOrSuperclassTable = new bool[ this.subclassTableNameClosure.Length ];
			for (int j=0; j<subclassTableNameClosure.Length; j++ ) {
				this.isClassOrSuperclassTable[j] = tables.Contains( this.subclassTableNameClosure[j] );
			}

			// PROPERTIES

			// initialize the lengths of all of the Property related fields in the class
			this.propertyTables = new int[this.hydrateSpan];
			this.propertyColumnNames = new string[this.hydrateSpan][];
			this.propertyColumnNameAliases = new string[this.hydrateSpan][];
			this.propertyColumnSpans = new int[this.hydrateSpan];

			// TODO: SYNCHISSUE: in latest Hibernate version this is a Hashtable not an array list.  There are 
			// some local variables to do with naturalOrderPropertyTables and naturalOrderTableNames
			ArrayList thisClassProperties = new ArrayList();

			int propertyIndex = 0;
			foreach(Property prop in model.PropertyClosureCollection) {
				
				thisClassProperties.Add(prop);
				Table tab = prop.Value.Table;
				string tabname = tab.GetQualifiedName( factory.DefaultSchema );
				
				this.propertyTables[propertyIndex] = GetTableId(tabname, this.tableNames);
				this.propertyColumnSpans[propertyIndex] = prop.ColumnSpan;

				string[] propCols = new string[ propertyColumnSpans[propertyIndex] ];
				string[] propAliases = new string[ propertyColumnSpans[propertyIndex] ];
				
				int columnIndex = 0;
				foreach(Column col in prop.ColumnCollection ) {
					propCols[columnIndex] = col.Name;
					propAliases[columnIndex] = col.Alias + tab.UniqueInteger + StringHelper.Underscore;
					columnIndex++;
				}
				
				this.propertyColumnNames[propertyIndex] = propCols;
				this.propertyColumnNameAliases[propertyIndex] = propAliases;

				propertyIndex++;
			}


			// subclass closure properties
			
			// SYNCHISSUE: in Hibernate there is a field names = new ArrayList that appears
			// after types.  It is used to populate a class scoped field called subclassPropertyNameClosure

			ArrayList columns = new ArrayList();//this.subclassColumnClosure
			ArrayList types = new ArrayList();//this.subclassPropertyTypeClosure
			ArrayList names = new ArrayList(); //this.subclassPropertyNameClosure
			ArrayList propColumns = new ArrayList();//this.subclassPropertyColumnNameClosure
			ArrayList coltables = new ArrayList();//this.subclassColumnTableNumberClosure
			ArrayList joinedFetchesList = new ArrayList();//this.subclassPropertyEnableJoinedFetch
			ArrayList aliases = new ArrayList();//this.subclassColumnClosureAlias
			ArrayList propTables = new ArrayList(); // this.subclassPropertyTableNameClosure
			ArrayList definedBySubclass = new ArrayList(); // this.propertyDefinedOnSubclass

			foreach(Property prop in model.SubclassPropertyClosureCollection) {
				definedBySubclass.Add( !thisClassProperties.Contains(prop) );
				Table tab = prop.Value.Table;
				string tabname = tab.GetQualifiedName( factory.DefaultSchema );
				string[] cols = new string[ prop.ColumnSpan ];
				types.Add( prop.Type );
				int tabnum = GetTableId(tabname, subclassTableNameClosure);
				propTables.Add(tabnum);
				int l=0;
				foreach(Column col in prop.ColumnCollection) {
					columns.Add( col.Name );
					coltables.Add(tabnum);
					cols[l++] = col.Name;
					aliases.Add( col.Alias + tab.UniqueInteger + StringHelper.Underscore );
				}
				propColumns.Add(cols);
				joinedFetchesList.Add( prop.Value.OuterJoinFetchSetting );
			}
			subclassColumnClosure = (string[]) columns.ToArray(typeof(string));
			subclassColumnClosureAliases = (string[]) aliases.ToArray(typeof(string));
			subclassColumnTableNumberClosure = (int[]) coltables.ToArray(typeof(int));
			subclassPropertyTypeClosure = (IType[]) types.ToArray(typeof(IType));
			subclassPropertyNameClosure = (string[]) names.ToArray(typeof(string));
			subclassPropertyTableNumberClosure = (int[]) propTables.ToArray(typeof(int));
			subclassPropertyColumnNameClosure = (string[][]) propColumns.ToArray( typeof(string[]) );
			subclassPropertyEnableJoinedFetch  = (OuterJoinLoaderType[]) joinedFetchesList.ToArray( typeof(OuterJoinLoaderType) );
			propertyDefinedOnSubclass = (bool[]) definedBySubclass.ToArray( typeof(bool) );

			System.Type mappedClass = model.PersistentClazz;

			// SUBCLASSES

			// all of the classes spanned, so even though there might be 2 subclasses we need to 
			// add in the baseclass - so we add 1 to the Closure
			int subclassSpan = model.SubclassSpan + 1;
			this.subclassClosure = new System.Type[subclassSpan];
			
			// start with the mapped class as the last element in the subclassClosure
			this.subclassClosure[subclassSpan-1] = mappedClass;

			if ( model.IsPolymorphic ) {
				this.subclassesByDiscriminatorValue.Add( discriminatorValue, mappedClass);
				this.discriminators = new string[subclassSpan];
				this.discriminators[subclassSpan-1] = discriminatorSQLString;
				
				this.tableNumbers = new int[subclassSpan];
				this.tableNumbers[subclassSpan-1] = GetTableId(
					model.Table.GetQualifiedName( factory.DefaultSchema ), 
					this.subclassTableNameClosure);
				
				this.notNullColumns = new string[subclassSpan];
				foreach(Column col in model.Table.PrimaryKey.ColumnCollection) {
					notNullColumns[subclassSpan-1] = col.Name; //only once
				}

			} 
			else {
				discriminators = null;
				tableNumbers = null;
				notNullColumns = null;
			}

			int p=0;
			foreach(Subclass sc in model.SubclassCollection) {
				subclassClosure[p] = sc.PersistentClazz;

				try {
					if (model.IsPolymorphic ) {
						int disc = p + 1;
						subclassesByDiscriminatorValue.Add( disc, sc.PersistentClazz );
						discriminators[p] = disc.ToString();
						tableNumbers[p] = GetTableId(
							sc.Table.GetQualifiedName( factory.DefaultSchema ),
							subclassTableNameClosure);
						foreach(Column col in sc.Table.PrimaryKey.ColumnCollection) {
							notNullColumns[p] = col.Name; //only once;
						}
					}
				} catch (Exception e) {
					throw new MappingException("Error parsing discriminator value", e);
				}
				p++;
			}

//			propertyHasColumns = new bool[sqlUpdateStrings.Length];
//			for (int m=0; m<sqlUpdateStrings.Length; m++) {
//				propertyHasColumns[m] = sqlUpdateStrings[m]!=null;
//			}

			

			allProperties = new bool[hydrateSpan];
			for(int t=0; t<allProperties.Length; t++)
				allProperties[t] = true;
		}

		

		public override void PostInstantiate(ISessionFactoryImplementor factory) {

			InitPropertyPaths(factory); 
    
			//TODO: move into InitPropertyPaths 
			Hashtable mods = new Hashtable();
			foreach( DictionaryEntry e in typesByPropertyPath ) {
				IType type = (IType) e.Value;
				if ( type.IsEntityType ) {
					string path = (string) e.Key;
					object table = tableNumberByPropertyPath[path];
					string[] columns = (string[]) columnNamesByPropertyPath[path];
					if ( columns.Length==0) {
						//ie a one-to-one association
						columns = IdentifierColumnNames;
						table = new int[0];
					}
					EntityType etype = (EntityType) type;
					IType idType = factory.GetIdentifierType( etype.PersistentClass );

					string idpath = path + StringHelper.Dot + PathExpressionParser.EntityID;
					mods.Add( idpath, idType);
					columnNamesByPropertyPath.Add( idpath, columns );
					tableNumberByPropertyPath.Add( idpath, table );
					if ( idType.IsComponentType || idType.IsObjectType ) {
						IAbstractComponentType actype = (IAbstractComponentType) idType;
						string[] props = actype.PropertyNames;
						if (props.Length!=columns.Length) throw new MappingException("broken mapping for: " + ClassName + StringHelper.Dot + path);
						for (int i=0; i<props.Length; i++) {
							string subidpath = idpath + StringHelper.Dot + props[i];
							columnNamesByPropertyPath.Add( subidpath, new string[] { columns[i] } );
							tableNumberByPropertyPath.Add( subidpath, table);
							mods.Add( subidpath, actype.Subtypes[i] );
						}
					}
				}
			}
			foreach(DictionaryEntry de in mods) {
				typesByPropertyPath.Add(de.Key, de.Value);
			}

			// initialize the Statements - these are in the PostInstantiate method because we need
			// to have every other IClassPersister loaded so we can resolve the IType for the 
			// relationships.  In Hibernate they are able to just use ? and not worry about Parameters until
			// the statement is actually called.  We need to worry about Parameters when we are building
			// the IClassPersister...

			sqlDeleteStrings = GenerateDeleteStrings();
			sqlInsertStrings = GenerateInsertStrings(false, PropertyInsertability);
			sqlIdentityInsertStrings = IsIdentifierAssignedByInsert ?
				GenerateInsertStrings(true, PropertyInsertability):
				null;

			sqlUpdateStrings = GenerateUpdateStrings(PropertyInsertability);

			SqlString lockString = GenerateLockString(null, null);
			SqlString lockExclusiveString = dialect.SupportsForUpdate ? 
				GenerateLockString(lockString, " FOR UPDATE") :
				GenerateLockString(lockString, null);
			SqlString lockExclusiveNowaitString = dialect.SupportsForUpdate ? 
				GenerateLockString(lockString, " FOR UPDATE NOWAIT") :
				GenerateLockString(lockString, null);

			lockers.Add(LockMode.Read, lockString);
			lockers.Add(LockMode.Upgrade, lockExclusiveString);
			lockers.Add(LockMode.UpgradeNoWait, lockExclusiveNowaitString);


			//TODO: find out why this was in the constructor in the spot it was...
			propertyHasColumns = new Boolean[sqlUpdateStrings.Length];
			for (int m = 0; m < sqlUpdateStrings.Length; m++) {
				propertyHasColumns[m] = (sqlUpdateStrings[m]!=null);
			}

			loader = new EntityLoader(this, factory);
		}

		public override bool IsDefinedOnSubclass(int i) {
			return propertyDefinedOnSubclass[i];
		}
		public override string TableName {
			get { return qualifiedTableName; }
		}
		public override string DiscriminatorColumnName {
			get { return discriminatorColumnName; }
		}
		public override IType GetSubclassPropertyType(int i) {
			return subclassPropertyTypeClosure[i];
		}
		
		public override string GetSubclassPropertyName(int i) {
			return subclassPropertyNameClosure[i];
		}

		public override int CountSubclassProperties() {
			return subclassPropertyTypeClosure.Length;
		}
		
		public override string GetSubclassPropertyTableName(int i) {
			return subclassTableNameClosure[ subclassPropertyTableNumberClosure[i] ];
		}

		public override string[] GetSubclassPropertyColumnNames(int i) {
			return subclassPropertyColumnNameClosure[i];
		}
		public override string[] GetPropertyColumnNames(int i) {
			return propertyColumnNameAliases[i];
		}
		public override IDiscriminatorType DiscriminatorType {
			get { return discriminatorType; }
		}
		public override string DiscriminatorSQLString {
			get { return discriminatorSQLString; }
		}
		public virtual System.Type[] SubclassClosure {
			get { return subclassClosure; }
		}
		public override System.Type GetSubclassForDiscriminatorValue(object value) {
			return (System.Type) subclassesByDiscriminatorValue[value];
		}
		public override OuterJoinLoaderType EnableJoinedFetch(int i) {
			return subclassPropertyEnableJoinedFetch[i];
		}
		public override object IdentifierSpace {
			get { return qualifiedTableName; }
		}
		public override object[] PropertySpaces {
			get {
				return tableNames; // don't need subclass tables, because they can't appear in conditions
			}
		}

		/// <summary>
		/// The SqlStrings to Delete this Entity
		/// </summary>
		protected SqlString[] SqlDeleteStrings{
			get {return sqlDeleteStrings;}
		}

		/// <summary>
		/// The SqlStrings to Insert this Entity
		/// </summary>
		protected SqlString[] SqlInsertStrings{
			get {return sqlInsertStrings;}
		}

		/// <summary>
		/// The SqlStrings to Insert this Entity when the DB handles
		/// generating the Identity.
		/// </summary>
		protected SqlString[] SqlIdentityInsertStrings {
			get {return sqlIdentityInsertStrings;}
		}

		/// <summary>
		/// The SqlStrings to Update this Entity
		/// </summary>
		protected SqlString[] SqlUpdateStrings {
			get {return sqlUpdateStrings;}
		}

		/// <summary>
		/// Generates an array of SqlStrings that encapsulates what later will be translated
		/// to ADO.NET IDbCommands to Delete this Entity.
		/// </summary>
		/// <returns>An array of SqlStrings </returns>
		protected virtual SqlString[] GenerateDeleteStrings() {
			SqlString[] deleteStrings = new SqlString[tableNames.Length];

			for( int i = 0; i < tableNames.Length; i++ ){
				SqlDeleteBuilder deleteBuilder = new SqlDeleteBuilder(factory);


				// TODO: find out why this is using tableKeyColumns and when
				// they would ever be different between the two tables - I thought
				// a requirement of Hibernate is that joined/subclassed tables
				// had to have the same pk - otherwise you had an association.
				deleteBuilder.SetTableName(tableNames[i])
					.SetIdentityColumn(tableKeyColumns[i], IdentifierType);

				if(i==0 && IsVersioned) deleteBuilder.SetVersionColumn(new string[] {VersionColumnName}, VersionType);

				
				deleteStrings[i] = deleteBuilder.ToSqlString();

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
		protected virtual SqlString[] GenerateInsertStrings(bool identityInsert, bool[] includeProperty) {
			SqlString[] insertStrings = new SqlString[tableNames.Length];
			
			for(int j = 0; j < tableNames.Length; j++) {
				SqlInsertBuilder builder = new SqlInsertBuilder(factory);
				builder.SetTableName(tableNames[j]);

				for(int i = 0 ; i < hydrateSpan; i++) {
					if(includeProperty[i] && propertyTables[i]==j) 
						builder.AddColumn(propertyColumnNames[i], PropertyTypes[i]);
				}		  

				//if (IsPolymorphic) builder.AddColumn(DiscriminatorColumnName, DiscriminatorSQLString);

				if(identityInsert && j==0) {
					builder.AddColumn(tableKeyColumns[j][0], dialect.IdentityInsertString);
					
				}
				else {
					builder.AddColumn(tableKeyColumns[j], IdentifierType);
				}

				insertStrings[j] = builder.ToSqlString();
			}

			return insertStrings;
			
		}

		
		/// <summary>
		/// Generates SqlStrings that encapsulates what later will be translated
		/// to ADO.NET IDbCommands to Update this Entity.
		/// </summary>
		/// <param name="includeProperty"></param>
		/// <returns>An array of SqlStrings</returns>
		protected virtual SqlString[] GenerateUpdateStrings(bool[] includeProperty) {
			SqlString[] updateStrings = new SqlString[tableNames.Length];
			
			
			for(int j = 0; j < tableNames.Length; j++) {
				SqlUpdateBuilder updateBuilder = new SqlUpdateBuilder(factory);

				updateBuilder.SetTableName(tableNames[j]);
				
				//TODO: figure out what the hasColumns variable comes into play for??
				bool hasColumns = false;

				for (int i = 0; i < propertyColumnNames.Length; i++) {
					if (includeProperty[i] && propertyTables[i]==j) {
						updateBuilder.AddColumns(propertyColumnNames[i], PropertyTypes[i]);
						hasColumns = hasColumns || propertyColumnNames[i].Length > 0;
					}
				}

				updateBuilder.SetIdentityColumn(tableKeyColumns[j], IdentifierType);

				if(j==0 && IsVersioned) updateBuilder.SetVersionColumn(new string[] {VersionColumnName}, VersionType);

				updateStrings[j] = hasColumns ? 
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
		protected virtual SqlString GenerateLockString(SqlString sqlString, string forUpdateFragment) {
			SqlStringBuilder sqlBuilder = null;

			if(sqlString==null) {
				SqlSimpleSelectBuilder builder = new SqlSimpleSelectBuilder(factory);
				
				// set the table name and add the columns to select
				builder.SetTableName(TableName)
					.AddColumns(IdentifierColumnNames);

				// add the parameters to use in the WHERE clause
				builder.SetIdentityColumn(IdentifierColumnNames, IdentifierType);
				if(IsVersioned) builder.SetVersionColumn(new string[]{VersionColumnName}, VersionType);
				
				sqlBuilder = new SqlStringBuilder(builder.ToSqlString());

			}
			else {
				sqlBuilder = new SqlStringBuilder(sqlString);
			}

			// add any special text that is contained in the forUpdateFragment
			if(forUpdateFragment!=null) sqlBuilder.Add(forUpdateFragment);

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
		protected virtual int Dehydrate(object id, object[] fields, bool[] includeProperty, IDbCommand[] statements, ISessionImplementor session) {
			if (log.IsDebugEnabled ) log.Debug("Dehydrating entity: " + ClassName + '#' + id);

			int versionParm = 0;

			for (int i=0; i<tableNames.Length; i++) {
				int index = Dehydrate(id, fields, includeProperty, i, statements[i], session);
				if (i==0) versionParm = index;
			}
			return versionParm;
		}

		private int Dehydrate(object id, object[] fields, bool[] includeProperty, int table, IDbCommand statement, ISessionImplementor session) {
			if (statement==null) return -1;

			int index = 0;
			for (int j=0; j<hydrateSpan; j++) {
				if ( includeProperty[j] && propertyTables[j]==table ) {
					PropertyTypes[j].NullSafeSet( statement, fields[j], index, session );
					index += propertyColumnSpans[j];
				}
			}

			if ( id!=null ) {
				IdentifierType.NullSafeSet( statement, id, index, session );
				index+= IdentifierColumnNames.Length;
			}

			return index;
		}

		/// <summary>
		/// Load an instance using either the <c>ForUpdateLoader</c> or the outer joining <c>loader</c>,
		/// depending on the value of the <c>lock</c> parameter
		/// </summary>
		/// <param name="id"></param>
		/// <param name="optionalObject"></param>
		/// <param name="lockMode"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public override object Load(object id, object optionalObject, LockMode lockMode, ISessionImplementor session) {
			if (log.IsDebugEnabled) log.Debug( "Materializing entity: " + ClassName + '#' + id );
			object result = loader.Load(session, id, optionalObject);

			if (result!=null) Lock(id, GetVersion(result), result, lockMode, session);
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
		public override void Lock(object id, object version, object obj, LockMode lockMode, ISessionImplementor session) {
			
			if ( lockMode.GreaterThan(LockMode.None) ) {

				if (log.IsDebugEnabled) {
					log.Debug("Locking entity: " + ClassName + '#' + id);
					if (IsVersioned) log.Debug("Version: " + version);
				}

				IDbCommand st = session.Preparer.PrepareCommand((SqlString)lockers[lockMode]);

				try {
					IdentifierType.NullSafeSet(st, id, 1, session);
					if ( IsVersioned ) VersionType.NullSafeSet(st, version, 2, session);

					IDataReader rs = st.ExecuteReader();
					try {
						if ( !rs.Read() ) throw new StaleObjectStateException(MappedClass, id);
					} 
					finally {
						rs.Close();
					}
				} 
				catch (Exception e) {
					throw e;
				} 
				finally {
					session.Batcher.CloseStatement(st);
				}
			}
		}

		/// <summary>
		/// Persist an object
		/// </summary>
		/// <param name="id">The Id to give the new object/</param>
		/// <param name="fields">The fields to transfer to the Command</param>
		/// <param name="obj">The object to Insert into the database.  I don't see where this is used???</param>
		/// <param name="session">The Session to use when Inserting the object.</param>
		public override void Insert(object id, object[] fields, object obj, ISessionImplementor session) {
			
			if (log.IsDebugEnabled) {
				log.Debug("Inserting entity: " + ClassName + '#' + id);
				if ( IsVersioned ) log.Debug("Version: " + Versioning.GetVersion(fields, this) );
			}

			// render the SQL query
			IDbCommand[] insertCmds = new IDbCommand[tableNames.Length];
			


			try {
				for (int i=0; i<tableNames.Length; i++ ) {
					
					insertCmds[i] = session.Preparer.PrepareCommand(SqlInsertStrings[i]);
				}

				// write the value of fields onto the prepared statements - we MUST use the state at the time
				// the insert was issued (cos of foreign key constraints).
				Dehydrate(id, fields, PropertyInsertability, insertCmds, session);

				for (int i = 0; i < tableNames.Length; i++) insertCmds[i].ExecuteNonQuery();

			} 
			catch (Exception e) {
				throw e;
			} 
			finally {
				for (int i=0; i<tableNames.Length; i++) {
					//if (statements[i]!=null) session.Batcher.CloseStatement( statements[i] );
				}
			}
		}

		/// <summary>
		/// Persist an object, using a natively generated identifier
		/// </summary>
		/// <param name="fields"></param>
		/// <param name="obj"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public override object Insert(object[] fields, object obj, ISessionImplementor session) {
			if (log.IsDebugEnabled) {
				log.Debug("Inserting entity: " + ClassName + " (native id)");
				if ( IsVersioned ) log.Debug( "Version: " + Versioning.GetVersion(fields, this) );
			}

			SqlString[] sqlStrings = SqlIdentityInsertStrings;

			IDbCommand statement = session.Preparer.PrepareCommand(sqlStrings[0]);

			try {
				Dehydrate(null, fields, allProperties, 0, statement, session);
				statement.ExecuteNonQuery();
			} 
			catch (Exception e) {
				throw e;
			} 
			finally {
				//session.Batcher.CloseStatement(statement);
			}

			// fetch the generated id:
			IDbCommand idSelect = session.Preparer.PrepareCommand(SqlIdentitySelect);

			object id;
			try {
				IDataReader rs = idSelect.ExecuteReader();
				try {
					if ( !rs.Read() ) throw new HibernateException("The database returned no natively generated identity value");
					id = IdentifierGeneratorFactory.Get(rs, IdentifierType.ReturnedClass );
				} 
				finally {
					rs.Close();
				}
				log.Debug("Natively generated identity: " + id);
			} 
			catch (Exception e) {
				throw e;
			} 
			finally {
				//session.Batcher.CloseStatement(idselect);
			}

			for (int i=1; i<tableNames.Length; i++ ) {
				statement = session.Preparer.PrepareCommand(sqlStrings[i]);

				try {
					Dehydrate(id, fields, allProperties, i, statement, session);
					statement.ExecuteNonQuery();
				} 
				catch ( Exception e) {
					throw e;
				} 
				finally {
					//session.Batcher.CloseStatement(statement);
				}
			}
			return id;
		}

		public override void Delete(object id, object version, object obj, ISessionImplementor session) {
			if ( log.IsDebugEnabled ) {
				log.Debug("Deleting entity: " + ClassName + '#' + id);
			}

			IDbCommand[] statements = new IDbCommand[tableNames.Length];
			try {

				for (int i=0; i<tableNames.Length; i++) {
					statements[i] = session.Preparer.PrepareCommand(SqlDeleteStrings[i]);
				} 

				if ( IsVersioned ) VersionType.NullSafeSet( statements[0], version, IdentifierColumnNames.Length + 1, session );

				for (int i=tableNames.Length-1; i>=0; i-- ) {

					// Do the key. The key is immutable so we can use the _current_ object state
					IdentifierType.NullSafeSet( statements[i], id, 0, session );

					// TODO: see if we want to synch this design up with EntityPersister, inconsistent right now...
					Check( statements[i].ExecuteNonQuery(), id );
				}
			} 
			catch (Exception e) {
				throw e;
			} 
			finally {
//				for (int i=0; i<tableNames.Length; i++) {
//					if (statements[i]!=null ) session.Batcher.CloseStatement( statements[i] );
//				}
			}
		}

		public override void Update(object id, object[] fields, int[] dirtyFields, object oldVersion, object obj, ISessionImplementor session) {
			bool[] tableUpdateNeeded;
			if (dirtyFields ==null) {
				tableUpdateNeeded = propertyHasColumns; //for object that came in via update()
			} 
			else {
				tableUpdateNeeded = new bool[tableNames.Length];
				for (int i=0; i<dirtyFields.Length; i++ ) {
					tableUpdateNeeded[ propertyTables[ dirtyFields[i] ] ] = true;
				}
				if ( IsVersioned ) tableUpdateNeeded[0] = true;
			}

			if ( UseDynamicUpdate && dirtyFields!=null ) {
				bool[] propsToUpdate = new bool[hydrateSpan];
				for (int i=0; i<hydrateSpan; i++ ) {
					bool dirty = false;
					for (int j=0; j<dirtyFields.Length; j++) {
						if ( dirtyFields[j]==i ) dirty=true;
					}
					propsToUpdate[i] = dirty || VersionProperty == i;
				}
				Update(id, fields, propsToUpdate, tableUpdateNeeded, oldVersion, obj, GenerateUpdateStrings(propsToUpdate), session);
			} 
			else {
				Update(id, fields, PropertyUpdateability, tableUpdateNeeded, oldVersion, obj, SqlUpdateStrings, session);
			}
		}

		protected virtual void Update(object id, object[] fields, bool[] includeProperty, bool[] includeTable, object oldVersion, object obj, SqlString[] sqlUpdateStrings, ISessionImplementor session) {
			if (log.IsDebugEnabled ) {
				log.Debug("Updating entity: " + ClassName + '#' + id);
				if ( IsVersioned ) log.Debug("Existing version: " + oldVersion + " -> New version: " + fields[ VersionProperty ] );
			}

			int tables = tableNames.Length;

			IDbCommand[] statements = new IDbCommand[tables];
			try {

				for (int i=0; i<tables; i++ ) {
					if ( includeTable[i] ) {
						statements[i] = session.Preparer.PrepareCommand(sqlUpdateStrings[i]);
					}
				}

				int versionParam = Dehydrate(id, fields, includeProperty, statements, session);

				if ( IsVersioned ) VersionType.NullSafeSet( statements[0], oldVersion, versionParam, session );

				for (int i=0; i<tables; i++ ) {
					if ( includeTable[i] ) Check(statements[i].ExecuteNonQuery(), id );
				}
			} 
			catch (Exception e) {
				throw e;
			} 
			finally {
//				for (int i=0; i<tables; i++ ) {
//					if ( statements[i]!=null ) session.Batcher.CloseStatement( statements[i] );
//				}
			}
		}

		


		//INITIALIZATION:

		
		
		
		private void InitPropertyPaths(IMapping mapping) {
		
			IType[] propertyTypes = PropertyTypes;
			string[] propertyNames = PropertyNames;
			for ( int i=0; i<propertyNames.Length; i++ ) {
				InitPropertyPaths( propertyNames[i], propertyTypes[i], propertyColumnNames[i], propertyTables[i], mapping );
			}
		
			string idProp = IdentifierPropertyName;
			if (idProp!=null) InitPropertyPaths( idProp, IdentifierType, IdentifierColumnNames, 0, mapping );
			if ( hasEmbeddedIdentifier ) InitPropertyPaths( null, IdentifierType, IdentifierColumnNames, 0, mapping );
			InitPropertyPaths( PathExpressionParser.EntityID, IdentifierType, IdentifierColumnNames, 0, mapping );

			typesByPropertyPath.Add( PathExpressionParser.EntityClass, DiscriminatorType );
			columnNamesByPropertyPath.Add( PathExpressionParser.EntityClass, new string[] { DiscriminatorColumnName } );
			tableNumberByPropertyPath.Add( PathExpressionParser.EntityClass, (int)0 );
		}
	
		private void InitPropertyPaths(string propertyName, IType propertyType, string[] columns, int table, IMapping mapping) {
		
			if ( propertyName!=null ) {
				typesByPropertyPath.Add(propertyName, propertyType);
				columnNamesByPropertyPath.Add(propertyName, columns);
				tableNumberByPropertyPath.Add( propertyName, (int)table );
			}
	
			if ( propertyType.IsComponentType ) {
				IAbstractComponentType compType = (IAbstractComponentType) propertyType; 
				string[] props = compType.PropertyNames;
				IType[] types = compType.Subtypes;
				int count=0;
				for ( int k=0; k<props.Length; k++ ) {
					int len = types[k].GetColumnSpan(mapping);
					string[] slice = new string[len];
					for ( int j=0; j<len; j++ ) {
						slice[j] = columns[count++];
					}
					string path = (propertyName==null) ? props[k] : propertyName + '.' + props[k];
					InitPropertyPaths(path, types[k], slice, table, mapping);
				}
			}
		}

		public override string FromTableFragment(string alias) {
			return subclassTableNameClosure[0] + ' ' + alias;
		}

		private JoinFragment Outerjoin(string name, bool innerJoin, bool includeSubclasses) {
			JoinFragment outerjoin = factory.Dialect.CreateOuterJoinFragment();
			for (int i=1; i<subclassTableNameClosure.Length; i++ ) {
				if (includeSubclasses || isClassOrSuperclassTable[i]) {
					outerjoin.AddJoin(
						subclassTableNameClosure[i],
						Alias(name, i),
						StringHelper.Prefix( IdentifierColumnNames, name + StringHelper.Dot ),
						subclassTableKeyColumns[i],
						innerJoin && isClassOrSuperclassTable[i] ? JoinType.InnerJoin : JoinType.LeftOuterJoin);
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
		/// <exception cref="AssertionFaliure">Thrown when the tableName specified can't be found</exception>
		private int GetTableId(string tableName, string[] tables) {
			
			for (int tableIndex = 0; tableIndex < tables.Length; tableIndex++ ) {
				if ( tableName.Equals( tables[tableIndex] ) ) return tableIndex;
			}
			
			throw new AssertionFailure("table [" + tableName + "] not found");
		}

		public override string[] ToColumns(string alias, string property) {
			if(PathExpressionParser.EntityClass.Equals(property) ) {
				// This doesn't actually seem to work but it *might* 
				// work on some dbs. Also it doesn't work if there 
				// are multiple columns of results because it 
				// is not accounting for the suffix: 
				// return new String[] { getDiscriminatorColumnName() }; 

				//TODO: this will need to be changed to return a SqlString but for now the SqlString
				// is being converted to a string for existing interfaces to work.
				return new string[] { DiscriminatorFragment(alias).ToSqlStringFragment().ToString() };
			}

			string[] cols = GetPropertyColumnNames(property);
			if (cols==null) throw new QueryException("unresolved property: " + property);

			int tableIndex;
			if (cols.Length==0) 
			{
				cols = IdentifierColumnNames;
				tableIndex = 0;
			} 
			else 
			{
				tableIndex = (int) tableNumberByPropertyPath[property];
			}

			// make sure an Alias was actually passed into the statement
			if(alias!=null && alias!=String.Empty) 
				return StringHelper.Prefix(cols, alias + StringHelper.Dot);
			else 
				return cols;

		}

		public override string[] ToColumns(string alias, int i) {
			int tab = subclassPropertyTableNumberClosure[i];
			return StringHelper.Prefix(
				subclassPropertyColumnNameClosure[i],
				Alias(alias, tab) + StringHelper.Dot);
		}

		public override string PropertySelectFragment(string alias, string suffix) {
			string[] cols = subclassColumnClosure;
			SqlCommand.SelectFragment frag = new SqlCommand.SelectFragment()
				.SetSuffix(suffix);
			for (int i=0; i<cols.Length; i++) {
				frag.AddColumn(
					Alias( alias, subclassColumnTableNumberClosure[i] ),
					cols[i],
					subclassColumnClosureAliases[i]
					);
			}

			if (HasSubclasses) {
				return ", " + 
					//TODO: this will need to be changed to return a SqlString but for now the SqlString
					// is being converted to a string for existing interfaces to work.
					DiscriminatorFragment(alias) 
						.SetReturnColumnName( DiscriminatorColumnName, suffix ) 
						.ToSqlStringFragment().ToString() 
					+
					frag.ToSqlStringFragment().ToString();
					// TODO: fix this once the interface has changed from a string to SqlString
			} 
			else { 
				// TODO: fix this once the interface has changed from a string to SqlString
				return frag.ToSqlStringFragment().ToString(); 
			} 
		} 

		private SqlCommand.CaseFragment DiscriminatorFragment(string alias) {
			SqlCommand.CaseFragment cases = dialect.CreateCaseFragment();
			
			for (int i=0; i<discriminators.Length; i++) {
				cases.AddWhenColumnNotNull(
					Alias( alias, tableNumbers[i] ),
					notNullColumns[i],
					discriminators[i]
					);
			}
			return cases;
		}

		private string Alias(string name, int tableNumber) {
			if (tableNumber==0) return name;
			return name + StringHelper.Underscore + tableNumber;
		}

		public override string GetConcreteClassAlias(string alias) {
			int tab = tableNumbers[ tableNumbers.Length - 1 ];
			return alias + ( (tab==0) ? String.Empty : StringHelper.Underscore + tab.ToString() );
		}

		public override string FromJoinFragment(string alias, bool innerJoin, bool includeSubclasses) {
			return Outerjoin(alias, innerJoin, includeSubclasses).ToFromFragmentString;
		}
		public override string WhereJoinFragment(string alias, bool innerJoin, bool includeSubclasses) {
			return Outerjoin(alias, innerJoin, includeSubclasses).ToWhereFragmentString;
		}

		public override string QueryWhereFragment(string alias, bool innerJoin, bool includeSubclasses)  {
			return WhereJoinFragment(alias, innerJoin, includeSubclasses);
		}
				

					
	}
}
