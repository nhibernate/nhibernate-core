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
		
		private ISessionFactoryImplementor factory;

		private string qualifiedTableName;
		private string[] tableNames;
		private string[][] tableKeyColumns;
		private string[] subclassTableNameClosure;
		private string[][] subclassTableKeyColumns;
		private bool[] isClassOrSuperclassTable;

		private string[] deleteStrings;
		private string[] insertStrings;
		private string[] identityInsertStrings;
		private string[] updateStrings;

		private OuterJoinLoaderType[] joinedFetch;
		private int[] propertyColumnSpans;
		private int[] propertyTables;
		private bool[] hasColumns;
		private string[][] propertyColumnNames;
		private string[][] propertyColumnNameAliases;
		private bool[] definedOnSubclass;
		private string[][] subclassPropertyColumnNameClosure;
		private int[] subclassPropertyTableNumberClosure;

		private string[] subclassColumnClosure;
		private string[] subclassColumnClosureAliases;
		private int[] subclassColumnTableNumberClosure;
		private IType[] subclassPropertyTypeClosure;
		private System.Type[] subclassClosure;

		private Hashtable tableNumberByPropertyPath = new Hashtable();

		private Hashtable subclassesByDiscriminatorValue = new Hashtable();
		private string[] discriminators;
		private string[] notNullColumns;
		private int[] tableNumbers;

		private IDiscriminatorType discriminatorType;
		private string discriminatorSQLString;
		private string discriminatorColumnName;

		protected IUniqueEntityLoader loader;
		protected IDictionary lockers = new Hashtable();


		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(NormalizedEntityPersister)); 

		public override void PostInstantiate(ISessionFactoryImplementor factory) {

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
					if ( idType.IsComponentType ) {
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
			return definedOnSubclass[i];
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
			return joinedFetch[i];
		}
		public override object IdentifierSpace {
			get { return qualifiedTableName; }
		}
		public override object[] PropertySpaces {
			get { return tableNames; }
		}

		//Access cached SQL

		protected string[] SqlDelete {
			get { return deleteStrings; }
		}
		protected string[] SqlInsert {
			get { return insertStrings; }
		}
		protected string[] SqlIdentityInsert {
			get { return identityInsertStrings; }
		}
		protected string[] SqlUpdate {
			get { return updateStrings; }
		}

		// Generate all the SQL
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

		private bool[] AllProperties {
			get {
				bool[] result = new bool[hydrateSpan];
				for (int i=0; i< result.Length; i++) result[i] = true;
				return result;
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
					statements[i] = session.Batcher.PrepareStatement( SqlInsert[i] );
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

			string[] sql = SqlIdentityInsert;

			IDbCommand statement = session.Batcher.PrepareStatement( sql[0] );
			try {
				Dehydrate(null, fields, AllProperties, 0, statement, session);
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
					Dehydrate(id, fields, AllProperties, i, statement, session);
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
					statements[i] = session.Batcher.PrepareStatement( SqlDelete[i] );
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
				tableUpdateNeeded = hasColumns; //for object that came in via update()
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
				Update(id, fields, PropertyUpdateability, tableUpdateNeeded, oldVersion, obj, SqlUpdate, session);
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

				InitPropertyPaths(prop, StringHelper.EmptyString);
				i++;
			}

			if (model.HasIdentifierProperty && model.IdentifierProperty.IsComposite) {
				InitPropertyPaths( model.IdentifierProperty, StringHelper.EmptyString );
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

			joinedFetch = (OuterJoinLoaderType[]) joinedFetchesList.ToArray( typeof(OuterJoinLoaderType) );
			definedOnSubclass = (bool[]) definedBySubclass.ToArray( typeof(bool) );

			deleteStrings = GenerateDeleteStrings();
			insertStrings = GenerateInsertStrings(false, PropertyInsertability );
			identityInsertStrings = IsIdentifierAssignedByInsert ?
				GenerateInsertStrings(true, PropertyInsertability) :
				null;
			updateStrings = GenerateUpdateStrings( PropertyUpdateability );

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

			hasColumns = new bool[updateStrings.Length];
			for (int m=0; m<updateStrings.Length; m++) {
				hasColumns[m] = updateStrings[m]!=null;
			}
		}

		private void InitPropertyPaths(Property prop, string path) {
			path += prop.Name;
			if (prop.IsComposite ) {
				foreach(Property subProp in ((Component)prop.Value).PropertyCollection) {
					typesByPropertyPath.Add( path, prop.Type);
					InitPropertyPaths( subProp, path + "." );
				}
			}

			string[] names = new string[ prop.ColumnSpan ];
			int k=0;
			foreach( Column col in prop.ColumnCollection ) {
				names[k] = col.Name;
				k++;
			}

			tableNumberByPropertyPath.Add(
				path,
				GetTableId( prop.Value.Table.GetQualifiedName( factory.DefaultSchema ), subclassTableNameClosure) );

			IType type = prop.Type;
			typesByPropertyPath.Add(path, type);

			columnNamesByPropertyPath.Add(path, names);
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
			string[] idcols = base.ToColumns(alias, property);
			if (idcols!=null) return idcols;

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
				CaseFragment cases = dialect.CreateCaseFragment()
					.SetReturnColumnName( DiscriminatorColumnName, suffix );

				for (int i=0; i<discriminators.Length; i++) {
					cases.AddWhenColumnNotNull(
						Alias( alias, tableNumbers[i] ),
						notNullColumns[i],
						discriminators[i]
						);
				}

				return ", " + cases.ToFragmentString() + frag.ToFragmentString();
			} else {
				return frag.ToFragmentString();
			}
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
		
		public override string QueryWhereFragment(string name, bool includeSubclasses) 
		{
#warning    QueryWhereFragment not yet ported
			throw new NotImplementedException("Not ported yet");
		}	

		public override string QueryWhereFragment(string alias, bool innerJoin, bool includeSubclasses) 
		{
			return WhereJoinFragment(alias, innerJoin, includeSubclasses);
		}
				

					
	}
}
