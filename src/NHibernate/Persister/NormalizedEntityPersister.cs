using System;
using System.Text;
using System.Data;
using System.Collections;
using NHibernate.Util;
using NHibernate.Loader;
using NHibernate.Mapping;
using NHibernate.Hql;
using NHibernate.Sql;
using NHibernate.Type;
using NHibernate.Engine;
using NHibernate.Id;

namespace NHibernate.Persister {
	/// <summary>
	/// A <c>IClassPersister</c> implementing the normalized "table-per-subclass" mapping strategy
	/// </summary>
	public class NormalizedEntityPersister : AbstractEntityPersister {
		
		private readonly ISessionFactoryImplementor factory;
		
		// the class hierarchy structure
		private readonly string qualifiedTableName;
		private readonly string[] tableNames;
		private readonly string[][] tableKeyColumns;
		
		private readonly System.Type[] subclassClosure;
		private readonly string[] subclassTableNameClosure;
		private readonly string[][] subclassTableKeyColumns;
		private readonly bool[] isClassOrSuperclassTable;

		// SQL strings
		private readonly string[] sqlDeleteStrings;
		private readonly string[] sqlInsertStrings;
		private readonly string[] sqlIdentityInsertStrings;
		private readonly string[] sqlUpdateStrings;

		// properties of this class, including inherited properties
		private readonly int[] propertyColumnSpans;
		private readonly int[] propertyTables;
		private readonly bool[] propertyHasColumns;
		private readonly string[][] propertyColumnNames;
		private readonly string[][] propertyColumnNameAliases;

		// the closure of all properties in the entire hierarchy including
		// subclasses and superclasses of this class
		private readonly string[][] subclassPropertyColumnNameClosure;
		private readonly int[] subclassPropertyTableNumberClosure;
		private readonly IType[] subclassPropertyTypeClosure;
		private readonly OuterJoinLoaderType[] subclassPropertyEnableJoinedFetch;
		private readonly bool[] propertyDefinedOnSubclass;
		private readonly Hashtable tableNumberByPropertyPath = new Hashtable();

		// the closure of all columns used by the entire hierarchy including
		// subclasses and superclasses of this class
		private readonly string[] subclassColumnClosure;
		private readonly string[] subclassColumnClosureAliases;
		private readonly int[] subclassColumnTableNumberClosure;
		
		// subclass discrimination works by assigning particular
		// values to certain combinations of null primary key
		// values in the outer join using an SQL CASE
		private readonly Hashtable subclassesByDiscriminatorValue = new Hashtable();
		private readonly string[] discriminators;
		private readonly string[] notNullColumns;
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
		public override int CountSubclassProperties() {
			return subclassPropertyTypeClosure.Length;
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
		public System.Type[] SubclassClosure {
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

		//Access cached SQL

		/// <summary>
		/// The queries that delete rows by id (and version)
		/// </summary>
		protected string[] SqlDeleteStrings {
			get { return sqlDeleteStrings; }
		}

		/// <summary>
		/// The queries that insert rows with a given id
		/// </summary>
		protected string[] SqlInsertStrings {
			get { return sqlInsertStrings; }
		}

		/// <summary>
		/// The queries that insert rows, letting the database generate an id
		/// </summary>
		protected string[] SqlIdentityInsertStrings {
			get { return sqlIdentityInsertStrings; }
		}

		/// <summary>
		/// The queries that update rows by id (and version)
		/// </summary>
		protected string[] SqlUpdateStrings {
			get { return sqlUpdateStrings; }
		}

		// Generate all the SQL

		/// <summary>
		/// Generate the SQL that deletes rows by id (and version)
		/// </summary>
		/// <returns></returns>
		protected string[] GenerateDeleteStrings() {
			string[] result = new string[ tableNames.Length ];
			for (int i=0; i<tableNames.Length; i++ ) {
				Delete delete = new Delete()
					.SetTableName( tableNames[i] )
					.SetPrimaryKeyColumnNames( tableKeyColumns[i] );
				if (i==0) delete.SetVersionColumnName( VersionColumnName );
				result[i] = delete.ToStatementString();
			}
			return result;
		}

		/// <summary>
		/// Generate the SQL that inserts rows
		/// </summary>
		/// <param name="identityInsert"></param>
		/// <param name="includeProperty"></param>
		/// <returns></returns>
		protected string[] GenerateInsertStrings(bool identityInsert, bool[] includeProperty) {
			string[] result = new string[tableNames.Length];
			for (int j=0; j<tableNames.Length; j++) {
				Insert insert = new Insert(dialect)
					.SetTableName( tableNames[j] );

				for (int i=0; i<PropertyTypes.Length; i++) {
					if ( includeProperty[i] && propertyTables[i]==j ) {
						insert.AddColumns( propertyColumnNames[i] );
					}
				}

				if (identityInsert && j==0) {
					insert.AddIdentityColumn( tableKeyColumns[j][0] );
				} else {
					insert.AddColumns( tableKeyColumns[j] );
				}

				result[j] = insert.ToStatementString();
			}
			return result;
		}

		/// <summary>
		/// Generate the SQL that updates rows by id (and version)
		/// </summary>
		/// <param name="includeProperty"></param>
		/// <returns></returns>
		protected string[] GenerateUpdateStrings(bool[] includeProperty) {
			string[] result = new string[ tableNames.Length ];
			for (int j=0; j<tableNames.Length; j++ ) {
				Update update = new Update()
					.SetTableName( tableNames[j] )
					.SetPrimaryKeyColumnNames( tableKeyColumns[j] );

				if (j==0) update.SetVersionColumnName( VersionColumnName );

				bool hasColumns=false;
				for (int i=0; i<propertyColumnNames.Length; i++) {
					if ( includeProperty[i] && propertyTables[i]==j) {
						update.AddColumns( propertyColumnNames[i] );
						hasColumns = hasColumns || propertyColumnNames[i].Length > 0;
					}
				}

				result[j] = hasColumns ? update.ToStatementString() : null;
			}
			return result;
		}

		protected string GenerateLockString() {
			SimpleSelect select = new SimpleSelect()
				.SetTableName( TableName )
				.AddColumn( IdentifierColumnNames[0] )
				.AddCondition( IdentifierColumnNames, "=?" );
			if ( IsVersioned ) {
				select.AddWhereToken("and")
					.AddCondition( VersionColumnName, "=?" );
			}
			return select.ToStatementString();
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
		protected int Dehydrate(object id, object[] fields, bool[] includeProperty, IDbCommand[] statements, ISessionImplementor session) {
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

			int index = 1;
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

				IDbCommand st = session.Batcher.PrepareStatement( (string) lockers[lockMode] );
				try {
					IdentifierType.NullSafeSet(st, id, 1, session);
					if ( IsVersioned ) VersionType.NullSafeSet(st, version, 2, session);

					IDataReader rs = st.ExecuteReader();
					try {
						if ( !rs.Read() ) throw new StaleObjectStateException(MappedClass, id);
					} finally {
						rs.Close();
					}
				} catch (Exception e) {
					throw e;
				} finally {
					session.Batcher.CloseStatement(st);
				}
			}
		}

		/// <summary>
		/// Persist an object
		/// </summary>
		/// <param name="id"></param>
		/// <param name="fields"></param>
		/// <param name="obj"></param>
		/// <param name="session"></param>
		public override void Insert(object id, object[] fields, object obj, ISessionImplementor session) {
			if (log.IsDebugEnabled) {
				log.Debug("Inserting entity: " + ClassName + '#' + id);
				if ( IsVersioned ) log.Debug("Version: " + Versioning.GetVersion(fields, this) );
			}

			// render the SQL query
			IDbCommand[] statements = new IDbCommand[ tableNames.Length ];
			try {
				for (int i=0; i<tableNames.Length; i++ ) {
					statements[i] = session.Batcher.PrepareStatement( SqlInsertStrings[i] );
				}

				// write the value of fields onto the prepared statements - we MUST use the state at the time
				// the insert was issued (cos of foreign key constraints).

				Dehydrate(id, fields, PropertyInsertability, statements, session);

				for (int i=0; i<tableNames.Length; i++) statements[i].ExecuteNonQuery();
			} catch (Exception e) {
				throw e;
			} finally {
				for (int i=0; i<tableNames.Length; i++) {
					if (statements[i]!=null) session.Batcher.CloseStatement( statements[i] );
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

			string[] sql = SqlIdentityInsertStrings;

			IDbCommand statement = session.Batcher.PrepareStatement( sql[0] );
			try {
				Dehydrate(null, fields, allProperties, 0, statement, session);
				statement.ExecuteNonQuery();
			} catch (Exception e) {
				throw e;
			} finally {
				session.Batcher.CloseStatement(statement);
			}

			// fetch the generated id:
			IDbCommand idselect = session.Batcher.PrepareStatement( SqlIdentitySelect );
			object id;
			try {
				IDataReader rs = idselect.ExecuteReader();
				try {
					if ( !rs.Read() ) throw new HibernateException("The database returned no natively generated identity value");
					id = IdentifierGeneratorFactory.Get(rs, IdentifierType.ReturnedClass );
				} finally {
					rs.Close();
				}
				log.Debug("Natively generated identity: " + id);
			} catch (Exception e) {
				throw e;
			} finally {
				session.Batcher.CloseStatement(idselect);
			}

			for (int i=1; i<tableNames.Length; i++ ) {
				statement = session.Batcher.PrepareStatement( sql[i] );
				try {
					Dehydrate(id, fields, allProperties, i, statement, session);
					statement.ExecuteNonQuery();
				} catch ( Exception e) {
					throw e;
				} finally {
					session.Batcher.CloseStatement(statement);
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
					statements[i] = session.Batcher.PrepareStatement( SqlDeleteStrings[i] );
				} 

				if ( IsVersioned ) VersionType.NullSafeSet( statements[0], version, IdentifierColumnNames.Length + 1, session );

				for (int i=tableNames.Length-1; i>=0; i-- ) {

					// Do the key. The key is immutable so we can use the _current_ object state

					IdentifierType.NullSafeSet( statements[i], id, 1, session );
					Check( statements[i].ExecuteNonQuery(), id );
				}
			} catch (Exception e) {
				throw e;
			} finally {
				for (int i=0; i<tableNames.Length; i++) {
					if (statements[i]!=null ) session.Batcher.CloseStatement( statements[i] );
				}
			}
		}

		public override void Update(object id, object[] fields, int[] dirtyFields, object oldVersion, object obj, ISessionImplementor session) {
			bool[] tableUpdateNeeded;
			if (dirtyFields ==null) {
				tableUpdateNeeded = propertyHasColumns; //for object that came in via update()
			} else {
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
			} else {
				Update(id, fields, PropertyUpdateability, tableUpdateNeeded, oldVersion, obj, SqlUpdateStrings, session);
			}
		}

		protected void Update(object id, object[] fields, bool[] includeProperty, bool[] includeTable, object oldVersion, object obj, string[] sql, ISessionImplementor session) {
			if (log.IsDebugEnabled ) {
				log.Debug("Updating entity: " + ClassName + '#' + id);
				if ( IsVersioned ) log.Debug("Existing version: " + oldVersion + " -> New version: " + fields[ VersionProperty ] );
			}

			int tables = tableNames.Length;

			IDbCommand[] statements = new IDbCommand[tables];
			try {

				for (int i=0; i<tables; i++ ) {
					if ( includeTable[i] ) statements[i] = session.Batcher.PrepareStatement( sql[i] );
				}

				int versionParam = Dehydrate(id, fields, includeProperty, statements, session);

				if ( IsVersioned ) VersionType.NullSafeSet( statements[0], oldVersion, versionParam, session );

				for (int i=0; i<tables; i++ ) {
					if ( includeTable[i] ) Check(statements[i].ExecuteNonQuery(), id );
				}
			} catch (Exception e) {
				throw e;
			} finally {
				for (int i=0; i<tables; i++ ) {
					if ( statements[i]!=null ) session.Batcher.CloseStatement( statements[i] );
				}
			}
		}

		//INITIALIZATION:

		public NormalizedEntityPersister(PersistentClass model, ISessionFactoryImplementor factory) : base(model, factory) {

			// CLASS + TABLE

			this.factory = factory;
			Table table = model.RootTable;
			qualifiedTableName = table.GetQualifiedName( factory.DefaultSchema );

			// DISCRIMINATOR

			object discriminatorValue;
			if ( model.IsPolymorphic ) {
				discriminatorColumnName = "clazz_";
				try {
					discriminatorType = (IDiscriminatorType) NHibernate.Integer;
					discriminatorValue = 0;
					discriminatorSQLString = "0";
				} catch (Exception e) {
					throw new MappingException("could not format discriminator value to SQL string", e);
				}
			} else {
				discriminatorColumnName = null;
				discriminatorType = null;
				discriminatorValue = null;
				discriminatorSQLString = null;
			}

			//MULTITABLES

			ArrayList tables = new ArrayList();
			ArrayList keyColumns = new ArrayList();
			tables.Add( qualifiedTableName );
			keyColumns.Add( IdentifierColumnNames );

			int idColumnSpan = IdentifierType.GetColumnSpan(factory);
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
			tableNames = (string[]) tables.ToArray( typeof(string) );
			tableKeyColumns = (string[][]) keyColumns.ToArray( typeof(string[]) );

				ArrayList subtables = new ArrayList();
			keyColumns = new ArrayList();
			subtables.Add( qualifiedTableName );
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
			subclassTableNameClosure = (string[]) subtables.ToArray( typeof(string) );
			subclassTableKeyColumns = (string[][]) keyColumns.ToArray( typeof(string[]) );
				isClassOrSuperclassTable = new bool[ subclassTableNameClosure.Length ];
			for (int j=0; j<subclassTableNameClosure.Length; j++ ) {
				isClassOrSuperclassTable[j] = tables.Contains( subclassTableNameClosure[j] );
			}

			// PROPERTIES

			propertyTables = new int[hydrateSpan];
			propertyColumnNames = new string[hydrateSpan][];
			propertyColumnNameAliases = new string[hydrateSpan][];
			propertyColumnSpans = new int[hydrateSpan];

			ArrayList thisClassProperties = new ArrayList();

			int i=0;
			foreach(Property prop in model.PropertyClosureCollection) {
				thisClassProperties.Add(prop);
				Table tab = prop.Value.Table;
				string tabname = tab.GetQualifiedName( factory.DefaultSchema );
				propertyTables[i] = GetTableId(tabname, tableNames);
				propertyColumnSpans[i] = prop.ColumnSpan;

				string[] propCols = new string[ propertyColumnSpans[i] ];
				string[] propAliases = new string[ propertyColumnSpans[i] ];
				int j=0;
				foreach(Column col in prop.ColumnCollection ) {
					string colname = col.Name;
					propCols[j] = colname;
					propAliases[j] = col.Alias + tab.UniqueInteger + StringHelper.Underscore;
					j++;
				}
				propertyColumnNames[i] = propCols;
				propertyColumnNameAliases[i] = propAliases;

				i++;
			}

			ArrayList columns = new ArrayList();
			ArrayList types = new ArrayList();
			ArrayList propColumns = new ArrayList();
			ArrayList coltables = new ArrayList();
			ArrayList joinedFetchesList = new ArrayList();
			ArrayList aliases = new ArrayList();
			ArrayList propTables = new ArrayList();
			ArrayList definedBySubclass = new ArrayList();

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
			subclassColumnTableNumberClosure = (int[]) aliases.ToArray(typeof(int));
			subclassPropertyTypeClosure = (IType[]) types.ToArray(typeof(IType));
			subclassPropertyTableNumberClosure = (int[]) aliases.ToArray(typeof(int));

			subclassPropertyColumnNameClosure = (string[][]) propColumns.ToArray( typeof(string[]) );

			subclassPropertyEnableJoinedFetch  = (OuterJoinLoaderType[]) joinedFetchesList.ToArray( typeof(OuterJoinLoaderType) );
			propertyDefinedOnSubclass = (bool[]) definedBySubclass.ToArray( typeof(bool) );

			sqlDeleteStrings = GenerateDeleteStrings();
			sqlInsertStrings = GenerateInsertStrings(false, PropertyInsertability );
			sqlIdentityInsertStrings = IsIdentifierAssignedByInsert ?
				GenerateInsertStrings(true, PropertyInsertability) :
				null;
			sqlUpdateStrings = GenerateUpdateStrings( PropertyUpdateability );

			string lockString = GenerateLockString();
			lockers.Add( LockMode.Read, lockString);
			string lockExclusiveString = dialect.SupportsForUpdate ? lockString + " for update" : lockString;
			lockers.Add( LockMode.Upgrade, lockExclusiveString );
			string lockExclusiveNowaitString = dialect.SupportsForUpdateNoWait ? lockString + " for update nowait" : lockExclusiveString;
			lockers.Add( LockMode.UpgradeNoWait, lockExclusiveNowaitString);

			System.Type mappedClass = model.PersistentClazz;

			// SUBCLASSES

			int subclassSpan = model.SubclassSpan + 1;
			subclassClosure = new System.Type[subclassSpan];
			subclassClosure[subclassSpan-1] = mappedClass;
			if ( model.IsPolymorphic ) {
				subclassesByDiscriminatorValue.Add( discriminatorValue, mappedClass);
				discriminators = new string[subclassSpan];
				discriminators[subclassSpan-1] = discriminatorSQLString;
				tableNumbers = new int[subclassSpan];
				tableNumbers[subclassSpan-1] = GetTableId(
					model.Table.GetQualifiedName( factory.DefaultSchema ),
					subclassTableNameClosure);
				notNullColumns = new string[subclassSpan];
				foreach(Column col in model.Table.PrimaryKey.ColumnCollection) {
					notNullColumns[subclassSpan-1] = col.Name; //only once
				}
			} else {
				discriminators = null;
				tableNumbers = null;
				notNullColumns = null;
			}
			int p=0;
			foreach(Subclass sc in model.SubclassCollection) {
				subclassClosure[p] = sc.PersistentClazz;

				try {
					if (model.IsPolymorphic ) {
						object disc = new int[p+1];
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

			propertyHasColumns = new bool[sqlUpdateStrings.Length];
			for (int m=0; m<sqlUpdateStrings.Length; m++) {
				propertyHasColumns[m] = sqlUpdateStrings[m]!=null;
			}

			allProperties = new bool[hydrateSpan];
			for(int t=0; t<allProperties.Length; t++)
				allProperties[t] = true;
		}

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

		private int GetTableId(string tableName, string[] tables) {
			for (int tab=0; tab<tables.Length; tab++ ) {
				if ( tableName.Equals( tables[tab] ) ) return tab;
			}
			throw new AssertionFailure("table not found");
		}

		public override string[] ToColumns(string alias, string property) {
			if(PathExpressionParser.EntityClass.Equals(property) ) {
				// This doesn't actually seem to work but it *might* 
				// work on some dbs. Also it doesn't work if there 
				// are multiple columns of results because it 
				// is not accounting for the suffix: 
				// return new String[] { getDiscriminatorColumnName() }; 

				return new string[] { DiscriminatorFragment(alias).ToFragmentString() };
			}

			string[] cols = GetPropertyColumnNames(property);
			if (cols==null) throw new QueryException("unresolved property: " + property);

			int tab;
			if (cols.Length==0) {
				cols = IdentifierColumnNames;
				tab=0;
			} else {
				tab = (int) tableNumberByPropertyPath[property];
			}

			return StringHelper.Prefix( cols, Alias(alias,tab) + StringHelper.Dot );
		}

		public override string[] ToColumns(string alias, int i) {
			int tab = subclassPropertyTableNumberClosure[i];
			return StringHelper.Prefix(
				subclassPropertyColumnNameClosure[i],
				Alias(alias, tab) + StringHelper.Dot);
		}

		public override string PropertySelectFragment(string alias, string suffix) {
			string[] cols = subclassColumnClosure;
			SelectFragment frag = new SelectFragment()
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
					DiscriminatorFragment(alias) 
					.SetReturnColumnName( DiscriminatorColumnName, suffix ) 
					.ToFragmentString() + 
					frag.ToFragmentString(); 
			} 
			else { 
				return frag.ToFragmentString(); 
			} 
		} 

		private CaseFragment DiscriminatorFragment(string alias) {
			CaseFragment cases = dialect.CreateCaseFragment();
				
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
			return alias + ( (tab==0) ? StringHelper.EmptyString : StringHelper.Underscore + tab.ToString() );
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
