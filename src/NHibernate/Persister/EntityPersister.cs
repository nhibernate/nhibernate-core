using System;
using System.Text;
using System.Data;
using System.Collections;

using NHibernate.Util;
using NHibernate.Loader;
using NHibernate.Mapping;
using NHibernate.Hql;
using NHibernate.Sql;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Engine;
using NHibernate.Id;


namespace NHibernate.Persister {
	/// <summary>
	/// Default implementation of the <c>ClassPersister</c> interface. Implements the
	/// "table-per-class hierarchy" mapping strategy for an entity class
	/// </summary>
	public class EntityPersister : AbstractEntityPersister, IQueryable {
		private readonly ISessionFactoryImplementor factory;

		// the class hierarchy structure
		private readonly string qualifiedTableName;
		private readonly string[] tableNames;
		private readonly bool hasUpdateableColumns;
		private readonly System.Type[] subclassClosure;

		// SQL strings
		private SqlString sqlDeleteString;
		private SqlString sqlInsertString;
		private SqlString sqlUpdateString;
		private SqlString sqlIdentityInsertString;

		// properties of this class, including inherited properties
		private readonly int[] propertyColumnSpans;
		private readonly bool[] propertyDefinedOnSubclass;
		private readonly string[][] propertyColumnNames;
		private readonly string[][] propertyColumnAliases;
		
		// the closure of all columns used by the entire hierarchy including
		// subclasses and superclasses of this class
		private readonly string[] subclassColumnClosure;
		private readonly string[] subclassColumnAliasClosure;

		// the closure of all properties in the entire hierarchy including
		// subclasses and superclasses of this class
		private readonly string[][] subclassPropertyColumnNameClosure;
		private readonly string[] subclassPropertyNameClosure;
		private readonly IType[] subclassPropertyTypeClosure;
		private readonly OuterJoinLoaderType[] subclassPropertyEnableJoinedFetch;

		// discriminator column
		private readonly Hashtable subclassesByDiscriminatorValue = new Hashtable();
		private readonly bool forceDiscriminator;
		private readonly string discriminatorColumnName;
		private readonly IDiscriminatorType discriminatorType;
		private readonly string discriminatorSQLString;
		
		private readonly IDictionary loaders = new Hashtable();
		private readonly IDictionary lockers = new Hashtable();

		private readonly string[] StringArray = {};
		private readonly IType[] TypeArray = {};

		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(EntityPersister));

		public EntityPersister(PersistentClass model, ISessionFactoryImplementor factory) : base(model, factory) {

			// CLASS + TABLE

			System.Type mappedClass = model.PersistentClazz;
			this.factory = factory;
			Table table = model.RootTable;
			qualifiedTableName = table.GetQualifiedName( dialect, factory.DefaultSchema );
			tableNames = new string[] { qualifiedTableName };

			// DISCRIMINATOR

			object discriminatorValue;
			if ( model.IsPolymorphic ) {
				Value d = model.Discriminator;
				if (d==null) throw new MappingException("discriminator mapping required for polymorphic persistence");
				forceDiscriminator = model.IsForceDiscriminator;
				
				// the discriminator will have only one column 
				foreach(Column discColumn in d.ColumnCollection) {
					discriminatorColumnName = discColumn.GetQuotedName(dialect);
				}
				try {
					discriminatorType = (IDiscriminatorType) model.Discriminator.Type;
					if ( "null".Equals( model.DiscriminatorValue)) {
						discriminatorValue = null;
						discriminatorSQLString = "null";
					} 
					else {
						discriminatorValue = discriminatorType.StringToObject( model.DiscriminatorValue );
						discriminatorSQLString = discriminatorType.ObjectToSQLString(discriminatorValue);
					}
				} 
				catch (Exception e) {
					throw new MappingException("Could not format discriminator value to sql string", e);
				}
			} 
			else {
				forceDiscriminator = false;
				discriminatorColumnName = null;
				discriminatorValue = null;
				discriminatorType = null;
				discriminatorSQLString = null;
			}

			// PROPERTIES

			propertyColumnNames = new string[hydrateSpan][];
			propertyColumnAliases = new string[hydrateSpan][];
			propertyColumnSpans = new int[hydrateSpan];
			ArrayList thisClassProperties = new ArrayList();

			int i=0;
			bool foundColumn = false;
			foreach(Property prop in model.PropertyClosureCollection) {
				int span = prop.ColumnSpan;
				propertyColumnSpans[i] = span;
				thisClassProperties.Add(prop);

				string[] colNames = new string[span];
				string[] colAliases = new string[span];
				int j=0;
				foreach(Column col in prop.ColumnCollection) {
					colAliases[j] = col.Alias(dialect);
					colNames[j] = col.GetQuotedName(dialect);
					j++;
					if( prop.IsUpdateable ) foundColumn=true;
				}
				propertyColumnNames[i] = colNames;
				propertyColumnAliases[i] = colAliases;

				i++;
			}

			hasUpdateableColumns = foundColumn;

			ArrayList columns = new ArrayList();
			ArrayList types = new ArrayList();
			ArrayList names = new ArrayList();
			ArrayList propColumns = new ArrayList();
			ArrayList aliases = new ArrayList();
			ArrayList joinedFetchesList = new ArrayList();
			ArrayList definedBySubclass = new ArrayList();

			foreach(Property prop in model.SubclassPropertyClosureCollection) {
				names.Add( prop.Name );
				definedBySubclass.Add( !thisClassProperties.Contains(prop) );
				string[] cols = new string[ prop.ColumnSpan ];
				types.Add( prop.Type );
				int l=0;
				foreach( Column col in prop.ColumnCollection ) {
					columns.Add( col.GetQuotedName(dialect) );
					aliases.Add( col.Alias(dialect) );
					cols[l++] = col.GetQuotedName(dialect);
				}
				propColumns.Add(cols);
				joinedFetchesList.Add( prop.Value.OuterJoinFetchSetting );
			}
			subclassColumnClosure = (string[]) columns.ToArray(typeof(string));
			subclassPropertyTypeClosure = (IType[]) types.ToArray(typeof(IType));
			subclassColumnAliasClosure = (string[]) aliases.ToArray(typeof(string));
			subclassPropertyNameClosure = (string[]) names.ToArray(typeof(string));
			subclassPropertyColumnNameClosure = (string[][]) propColumns.ToArray( typeof(string[]));

			subclassPropertyEnableJoinedFetch = new OuterJoinLoaderType[ joinedFetchesList.Count ];
			int m=0;
			foreach(OuterJoinLoaderType qq in joinedFetchesList) {
				subclassPropertyEnableJoinedFetch[m++] = qq;
			}
			propertyDefinedOnSubclass = new bool[definedBySubclass.Count];
			m=0;
			foreach( bool val in definedBySubclass) {
				propertyDefinedOnSubclass[m++] = val;
			}

			int subclassSpan = model.SubclassSpan + 1;
			subclassClosure = new System.Type[subclassSpan];
			subclassClosure[0] = mappedClass;
			if ( model.IsPolymorphic ) {
				if (discriminatorValue==null) {
					subclassesByDiscriminatorValue.Add( ObjectUtils.Null, mappedClass);
				} else {
					subclassesByDiscriminatorValue.Add( discriminatorValue, mappedClass);
				}

			}

			// SUBCLASSES
			if ( model.IsPolymorphic ) {
				int k=1;
				foreach(Subclass sc in model.SubclassCollection) {
					subclassClosure[k++] = sc.PersistentClazz;
					if ("null".Equals( sc.DiscriminatorValue ) ) {
						subclassesByDiscriminatorValue.Add ( ObjectUtils.Null, sc.PersistentClazz );
					} else {
						try {
							subclassesByDiscriminatorValue.Add (
								discriminatorType.StringToObject( sc.DiscriminatorValue ),
								sc.PersistentClazz);
						} catch (Exception e) {
							throw new MappingException("Error parsing discriminator value", e);
						}
					}
				}
			}
		}

		
		public override void PostInstantiate(ISessionFactoryImplementor factory) {

			InitPropertyPaths(factory);

			//TODO: move into InitPropertyPaths
			Hashtable mods = new Hashtable();
			foreach(DictionaryEntry e in typesByPropertyPath) {
				IType type = (IType) e.Value;
				if ( type.IsEntityType ) {
					string path = (string) e.Key;
					string[] columns = (string[]) columnNamesByPropertyPath[path];
					if ( columns.Length==0 ) columns = IdentifierColumnNames; //1-to-1 assoc
					EntityType etype = (EntityType) type;
					IType idType = factory.GetIdentifierType( etype.PersistentClass );

					string idpath = path + StringHelper.Dot + PathExpressionParser.EntityID;
					mods.Add(idpath, idType);
					columnNamesByPropertyPath.Add(idpath, columns);
					if ( idType.IsComponentType || idType.IsObjectType ) {
						IAbstractComponentType actype = (IAbstractComponentType) idType;
						string[] props = actype.PropertyNames;
						IType[] subtypes = actype.Subtypes;
						if ( actype.GetColumnSpan(factory) != columns.Length )
							throw new MappingException("broken mapping for: " + ClassName + StringHelper.Dot + path);
						int j=0;
						for (int i=0; i<props.Length; i++) {
							string subidpath = idpath + StringHelper.Dot + props[i];
							string[] componentColumns = new string[ subtypes[i].GetColumnSpan(factory) ];
							for (int k=0; k<componentColumns.Length; k++) {
								componentColumns[k] = columns[j++];
							}
							columnNamesByPropertyPath.Add(subidpath, componentColumns);
							mods.Add( subidpath, actype.Subtypes[i] );
						}
					}
				}
			}
			foreach(DictionaryEntry de in mods) {
				typesByPropertyPath.Add(de.Key, de.Value);
			}

			IUniqueEntityLoader loader = new EntityLoader(this, factory);

			// initialize the SqlStrings - these are in the PostInstantiate method because we need
			// to have every other IClassPersister loaded so we can resolve the IType for the 
			// relationships.  In Hibernate they are able to just use ? and not worry about Parameters until
			// the statement is actually called.  We need to worry about Parameters when we are building
			// the IClassPersister...
			sqlDeleteString = GenerateDeleteString();
			sqlInsertString = GenerateInsertString(false, PropertyInsertability);
			sqlIdentityInsertString = IsIdentifierAssignedByInsert ?
				GenerateInsertString(true, PropertyInsertability):
				null;

			sqlUpdateString = GenerateUpdateString(PropertyInsertability);

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

			SqlString selectForUpdateString = factory.Dialect.SupportsForUpdate ?
				GenerateSelectForUpdateString(" FOR UPDATE") : GenerateSelectForUpdateString(null);

			SqlString selectForUpdateNoWaitString = factory.Dialect.SupportsForUpdateNoWait ?
				GenerateSelectForUpdateString(" FOR UPDATE NO WAIT") : 
				selectForUpdateString.Clone();

			loaders.Add( LockMode.None, loader );
			loaders.Add( LockMode.Read, loader );
			loaders.Add( LockMode.Upgrade, new SimpleEntityLoader(this, selectForUpdateString, LockMode.Upgrade, factory.Dialect));
			loaders.Add( LockMode.UpgradeNoWait, new SimpleEntityLoader(this, selectForUpdateNoWaitString, LockMode.UpgradeNoWait, factory.Dialect));

		}

		public override bool IsDefinedOnSubclass(int i) {
			return propertyDefinedOnSubclass[i];
		}

		public override string DiscriminatorColumnName {
			get { return discriminatorColumnName; }
		}

		public override OuterJoinLoaderType EnableJoinedFetch(int i) {
			return subclassPropertyEnableJoinedFetch[i];
		}

		public override IType GetSubclassPropertyType(int i) {
			return subclassPropertyTypeClosure[i];
		}

		public override int CountSubclassProperties() {
			return subclassPropertyTypeClosure.Length;
		}

		public override string TableName {
			get { return qualifiedTableName; }
		}

		public override string[] GetSubclassPropertyColumnNames(int i) {
			return subclassPropertyColumnNameClosure[i];
		}

		public override string[] GetPropertyColumnNames(int i) {
			return propertyColumnAliases[i];
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
			if (value==null) {
				return (System.Type) subclassesByDiscriminatorValue[ ObjectUtils.Null ];
			} else {
				return (System.Type) subclassesByDiscriminatorValue[ value ];
			}
		}

		public override object IdentifierSpace {
			get { return qualifiedTableName; }
		}

		public override object[] PropertySpaces {
			get { return tableNames; }
		}

		/// <summary>
		/// The SqlString do Delete this Entity.
		/// </summary>
		protected SqlString SqlDeleteString {
			get { return sqlDeleteString;}
		}
		/// <summary>
		/// The SqlString to Insert this Entity
		/// </summary>
		protected SqlString SqlInsertString {
			get { return sqlInsertString;}
		}

		/// <summary>
		/// The SqlString to Insert this Entity using a natively generated Id.
		/// </summary>
		protected SqlString SqlIdentityInsertString {
			get {return sqlIdentityInsertString;}
		}

		/// <summary>
		/// The SqlString to Update this Entity.
		/// </summary>
		protected SqlString SqlUpdateString {
			get {return sqlUpdateString;}
		}

		/// <summary>
		/// Generates an SqlString that encapsulates what later will be translated
		/// to an ADO.NET IDbCommand to Delete this Entity.
		/// </summary>
		/// <returns>A SqlString for a Delete</returns>
		protected virtual SqlString GenerateDeleteString() {
			SqlDeleteBuilder deleteBuilder = new SqlDeleteBuilder(factory);
			deleteBuilder.SetTableName(TableName)
				.SetIdentityColumn(IdentifierColumnNames, IdentifierType);

			if(IsVersioned) deleteBuilder.SetVersionColumn(new string[] {VersionColumnName}, VersionType);

			return deleteBuilder.ToSqlString();

		}

		/// <summary>
		/// Generates an SqlString that encapsulates what later will be translated
		/// to an ADO.NET IDbCommand to Insert this Entity.
		/// </summary>
		/// <returns>A SqlString for an Insert</returns>
		protected virtual SqlString GenerateInsertString(bool identityInsert, bool[] includeProperty) {
			
			SqlInsertBuilder builder = new SqlInsertBuilder(factory);
			builder.SetTableName(TableName);

			for(int i = 0 ; i < hydrateSpan; i++) {
				if(includeProperty[i]) builder.AddColumn(propertyColumnNames[i], PropertyTypes[i]);
			}		  

			if (IsPolymorphic) builder.AddColumn(DiscriminatorColumnName, DiscriminatorSQLString);

			if(identityInsert==false) {
				builder.AddColumn(IdentifierColumnNames, IdentifierType);
			}
			else {
				// make sure the Dialect has an identity insert string because we don't want
				// to add the column when there is no value to supply the SqlBuilder
				if(dialect.IdentityInsertString!=null) {
					// only 1 column if there is IdentityInsert enabled.
					builder.AddColumn(IdentifierColumnNames[0], dialect.IdentityInsertString);
				}
			}

			return builder.ToSqlString();

		}

		
		/// <summary>
		/// Generates an SqlString that encapsulates what later will be translated
		/// to an ADO.NET IDbCommand to Update this Entity.
		/// </summary>
		/// <returns>A SqlString for an Update</returns>
		protected virtual SqlString GenerateUpdateString(bool[] includeProperty) {
			SqlUpdateBuilder updateBuilder = new SqlUpdateBuilder(factory);

			updateBuilder.SetTableName(TableName);

			for (int i = 0; i < hydrateSpan; i++) {
				if (includeProperty[i]) updateBuilder.AddColumns(propertyColumnNames[i], PropertyTypes[i]);
			}

			updateBuilder.SetIdentityColumn(IdentifierColumnNames, IdentifierType);
			if(IsVersioned) updateBuilder.SetVersionColumn(new string[] {VersionColumnName}, VersionType);
		
			return updateBuilder.ToSqlString();

		}


		/// <summary>
		/// Generates a SqlString that will append the forUpdateFragment to the sql.
		/// </summary>
		/// <param name="forUpdateFragment"></param>
		/// <returns>A new SqlString</returns>
		protected virtual SqlString GenerateSelectForUpdateString(string forUpdateFragment) {
			
			SqlString forUpdateSqlString = null;
			
			SqlSimpleSelectBuilder builder = new SqlSimpleSelectBuilder(factory);
				
			// set the table name and add the columns to select
			builder.SetTableName(TableName)
				.AddColumns(IdentifierColumnNames)
				.AddColumns(subclassColumnClosure, subclassColumnAliasClosure);

			if (HasSubclasses) builder.AddColumn(DiscriminatorColumnName);

			// add the parameters to use in the WHERE clause
			// TODO: find out why version isn't used here
			builder.SetIdentityColumn(IdentifierColumnNames, IdentifierType);

			forUpdateSqlString = builder.ToSqlString();
			

			// add any special text that is contained in the forUpdateFragment
			if(forUpdateFragment!=null && forUpdateFragment!=String.Empty) {
				SqlStringBuilder sqlBuilder = new SqlStringBuilder(forUpdateSqlString);	
				sqlBuilder.Add(forUpdateFragment);
				forUpdateSqlString = sqlBuilder.ToSqlString();
			}

			return forUpdateSqlString;
		
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
		/// Marshall the fields of a persistent instance to a prepared statement
		/// </summary>
		/// <param name="id"></param>
		/// <param name="fields"></param>
		/// <param name="includeProperty"></param>
		/// <param name="st"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		protected virtual int Dehydrate(object id, object[] fields, bool[] includeProperty, IDbCommand st, ISessionImplementor session) {
			if (log.IsDebugEnabled ) log.Debug("Dehydrating entity: " + ClassName + '#' + id);

			int index = 1;

			// there's a pretty strong coupling between the order of the SQL parameter 
			// construction and the actual order of the parameter collection. 
			index = 0;
			
			for (int j=0; j<hydrateSpan; j++) 
			{
				if ( includeProperty[j] ) {
					PropertyTypes[j].NullSafeSet( st, fields[j], index, session );
					index += propertyColumnSpans[j];
				}
			}

			if ( id!=null ) {
				IdentifierType.NullSafeSet( st, id, index, session );
				index += IdentifierColumnNames.Length;
			}

			return index;
		}

		/// <summary>
		/// Load an instance uing either the <c>forUpdateLoader</c> or the other joining <c>loader</c>,
		/// depending upon the value of the <c>lock</c> parameter
		/// </summary>
		/// <param name="id"></param>
		/// <param name="optionalObject"></param>
		/// <param name="lockMode"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public override object Load(object id, object optionalObject, LockMode lockMode, ISessionImplementor session) {
			if ( log.IsDebugEnabled ) log.Debug( "Materializing entity: " + ClassName + '#' + id );

			return ( (IUniqueEntityLoader)loaders[lockMode]).Load(session, id, optionalObject);
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
			if (lockMode.GreaterThan(LockMode.None) ) {

				if (log.IsDebugEnabled ) {
					log.Debug("Locking entity: " + ClassName + '#' + id);
					if ( IsVersioned ) log.Debug("Version: " + version);
				}

				IDbCommand st = session.Preparer.PrepareCommand((SqlString)lockers[lockMode]);

				try {
					IdentifierType.NullSafeSet(st, id, 0, session);
					
					if ( IsVersioned ) VersionType.NullSafeSet(st, version, IdentifierColumnNames.Length, session);

					IDataReader rs = st.ExecuteReader();
					try {
						if ( rs.Read()==false ) throw new StaleObjectStateException( MappedClass, id);
					} finally {
						rs.Close();
					}
				} finally {
					//session.Batcher.CloseStatement(st);
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
				if ( IsVersioned ) log.Debug("Version: " + Versioning.GetVersion(fields, this));
			}

			// Render the SQL query
			IDbCommand insertCmd = session.Preparer.PrepareCommand(SqlInsertString);

			try {

				// Write the values of the field onto the prepared statement - we MUST use the
				// state at the time the insert was issued (cos of foreign key constraints)
				// not necessarily the obect's current state

				Dehydrate(id, fields, PropertyInsertability, insertCmd, session);
				
				// IDbCommand REFACTOR 
				// session.Batcher.AddToBatch(1);
				int rowCount = insertCmd.ExecuteNonQuery();

				//negative expected row count means we don't know how many rows to expect
				if ( rowCount > 0 && rowCount != 1)
					throw new HibernateException("SQL update or deletion failed (row not found)");

			} 
			catch (Exception e ) {
				throw e;
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

			IDbCommand statement = null;
			IDbCommand idSelect = null;

			if(dialect.SupportsIdentitySelectInInsert) 
			{
				statement = session.Preparer.PrepareCommand( dialect.AddIdentitySelectToInsert(SqlIdentityInsertString) );
				idSelect = statement;
			}
			else 
			{
				statement = session.Preparer.PrepareCommand(SqlIdentityInsertString);
				idSelect = session.Preparer.PrepareCommand(SqlIdentitySelect);
			}


			try 
			{
				Dehydrate(null, fields, PropertyInsertability, statement, session);
			} 
			catch (Exception e) 
			{
				throw new HibernateException("EntityPersister had a problem Dehydrating for an Insert", e);
			} 

			try 
			{
				// if it doesn't support identity select in insert then we have to issue the Insert
				// as a seperate command here
				if(dialect.SupportsIdentitySelectInInsert==false) 
				{
					statement.ExecuteNonQuery();
				}

				IDataReader rs = idSelect.ExecuteReader();
				object id;
				try 
				{
					if ( !rs.Read() ) throw new HibernateException("The database returned no natively generated identity value");
					id = IdentifierGeneratorFactory.Get( rs, IdentifierType.ReturnedClass );
				} 
				finally 
				{
					rs.Close();
				}

				log.Debug("Natively generated identity: " + id);

				return id;
			} 
			catch (Exception e) 
			{
				throw e;
			} 
			finally 
			{
				// session.Batcher.CloseStatement(statement);
				// session.Batcher.CloseStatement(idselect);
			}
		}

		/// <summary>
		/// Perform a Deletion of the Entity from the Database.
		/// </summary>
		/// <param name="id">The id of the Object to Delete.</param>
		/// <param name="version">The version of the Object to Delete.</param>
		/// <param name="obj">The Object to Delete.</param>
		/// <param name="session">The Session to perform the Deletion in.</param>
		public override void Delete(object id, object version, object obj, ISessionImplementor session) {
			
			if ( log.IsDebugEnabled ) {
				log.Debug("Deleting entity: " + ClassName + '#' + id);
				if ( IsVersioned ) log.Debug( "Version: " + version );
			}

			
			int actualCount = 0;
			IDbCommand deleteCmd = session.Preparer.PrepareCommand(SqlDeleteString);
			
			try {
				IdentifierType.NullSafeSet(deleteCmd, id, 0, session);
 
				if(IsVersioned) {
					//VersionType.NullSafeSet(deleteCmd, version, IdentifierColumnNames.Length + 1, session);
					VersionType.NullSafeSet(deleteCmd, version, IdentifierColumnNames.Length, session);
				}

				actualCount = deleteCmd.ExecuteNonQuery();
			}
			catch (Exception e) {
				throw e;
			}
			
			
			if (actualCount < 1) {
				throw new StaleObjectStateException( MappedClass, id );
			} 
			else if (actualCount > 1) {
				throw new HibernateException("Duplicate identifier in table for " + ClassName + ": " + id);
			}

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="fields"></param>
		/// <param name="dirtyFields"></param>
		/// <param name="oldVersion"></param>
		/// <param name="obj"></param>
		/// <param name="session"></param>
		public override void Update(object id, object[] fields, int[] dirtyFields, object oldVersion, object obj, ISessionImplementor session) {
			if (UseDynamicUpdate && dirtyFields!=null ) {
				bool[] propsToUpdate = new bool[hydrateSpan];
				for (int i=0; i<hydrateSpan; i++) {
					bool dirty = false;
					for (int j=0; j<dirtyFields.Length; j++) {
						if ( dirtyFields[j]==i ) dirty=true;
					}
					propsToUpdate[i] = dirty || VersionProperty==i;
				}
				Update(id, fields, propsToUpdate, oldVersion, obj, GenerateUpdateString(propsToUpdate), session);
			} 
			else {
				Update(id, fields, PropertyUpdateability, oldVersion, obj, SqlUpdateString, session);
			}
		}

		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="fields"></param>
		/// <param name="includeProperty"></param>
		/// <param name="oldVersion"></param>
		/// <param name="obj"></param>
		/// <param name="sqlUpdateString"></param>
		/// <param name="session"></param>
		protected virtual void Update(object id, object[] fields, bool[] includeProperty, object oldVersion, object obj, SqlString sqlUpdateString, ISessionImplementor session) {
			if (log.IsDebugEnabled ) {
				log.Debug("Updating entity: " + ClassName + '#' + id);
				if ( IsVersioned ) log.Debug( "Existing version: " + oldVersion + " -> New Version: " + fields[ VersionProperty ] );
			}

			if (!hasUpdateableColumns) return;

			
			int actualCount = 0;
			
			IDbCommand statement = session.Preparer.PrepareCommand(sqlUpdateString);

			try {

				// now write the values of fields onto the command

				int versionParamIndex = Dehydrate(id, fields, includeProperty, statement, session);

				if ( IsVersioned ) {
					VersionType.NullSafeSet( statement, oldVersion, versionParamIndex, session);
					// I have moved the contents of the Check() out to inside this statement...
					//Check( statement.ExecuteNonQuery(), id );
				} 
				
				actualCount = statement.ExecuteNonQuery();

			} 
			catch (Exception e) {
				throw e;
			} 
			
			if (actualCount < 1) {
				throw new StaleObjectStateException( MappedClass, id );
			} 
			else if (actualCount > 1) {
				throw new HibernateException("Duplicate identifier in table for " + ClassName + ": " + id);
			}
		}
		

		private void InitPropertyPaths(IMapping mapping) {
			IType[] propertyTypes = PropertyTypes;
			string[] propertyNames = PropertyNames;
			for ( int i=0; i<propertyNames.Length; i++ ) {
				InitPropertyPaths( propertyNames[i], propertyTypes[i], propertyColumnNames[i], mapping );
			}
		
			string idProp = IdentifierPropertyName;
			if (idProp!=null) InitPropertyPaths( idProp, IdentifierType, IdentifierColumnNames, mapping );
			if ( hasEmbeddedIdentifier ) InitPropertyPaths( null, IdentifierType, IdentifierColumnNames, mapping );
			if (PathExpressionParser.EntityID != idProp)
				InitPropertyPaths( PathExpressionParser.EntityID, IdentifierType, IdentifierColumnNames, mapping );
		
			if ( IsPolymorphic ) {
				typesByPropertyPath.Add( PathExpressionParser.EntityClass, DiscriminatorType );
				columnNamesByPropertyPath.Add( PathExpressionParser.EntityClass, new string[] { DiscriminatorColumnName } );
			}
		}

		private void InitPropertyPaths(string propertyName, IType propertyType, string[] columns, IMapping mapping) {
			if (propertyName!=null) {
				typesByPropertyPath.Add(propertyName, propertyType);
				columnNamesByPropertyPath.Add(propertyName, columns);
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
					InitPropertyPaths(path, types[k], slice, mapping);
				}
			}
		}

		public virtual string[] TableNames {
			get { return tableNames; }
		}

		public override string FromTableFragment(string name) {
			return TableName + ' ' + name;
		}

		public override string QueryWhereFragment(string name, bool innerJoin, bool includeSubclasses) {
			if (innerJoin && (forceDiscriminator || IsInherited )) {
				InFragment frag = new InFragment()
					.SetColumn ( name, DiscriminatorColumnName );
				System.Type[] subclasses = SubclassClosure;
				for ( int i=0; i<subclasses.Length; i++) {
					frag.AddValue(
						((IQueryable) factory.GetPersister(subclasses[i])).DiscriminatorSQLString
						);
				}
				return " and " + frag.ToFragmentString();
			} else {
				return String.Empty;
			}
		}

		public override string[] ToColumns(string name, string property) {
			
			string[] cols = GetPropertyColumnNames(property);

			if (cols==null) throw new QueryException("unresolved property: " + property);

			if (cols.Length==0) 
			{
				// ie a nested collection or a one-to-one
				cols = IdentifierColumnNames;
			}

			// make sure an Alias was actually passed into the statement
			if(name!=null && name!=String.Empty) 
			{
				return StringHelper.Prefix(cols, name + StringHelper.Dot);
			}
			else 
			{
				return cols;
			}
		}

		public override string[] ToColumns(string name, int i) {
			return StringHelper.Prefix( subclassPropertyColumnNameClosure[i], name + StringHelper.Dot );
		}

		public override string GetSubclassPropertyTableName(int i) {
			return qualifiedTableName;
		}

		public override string GetSubclassPropertyName(int i) {
			return this.subclassPropertyNameClosure[i];
		}

		//public abstract string GetSubclassPropertyTableName(int i) ;
		//public abstract string GetSubclassPropertyName(int i);

		public override string PropertySelectFragment(string name, string suffix) {
			SqlCommand.SelectFragment frag = new SqlCommand.SelectFragment(factory.Dialect)
				.SetSuffix(suffix);
			if ( HasSubclasses ) frag.AddColumn( name, DiscriminatorColumnName );
			
			// TODO: fix this once the interface is changed from a string to SqlString
			// this works now because there are no parameters in the select string
			return frag.AddColumns(name, subclassColumnClosure, subclassColumnAliasClosure)
				.ToSqlStringFragment().ToString();
		}

		public override string FromJoinFragment(string alias, bool innerJoin, bool includeSubclasses) {
			return String.Empty;
		}

		public override string WhereJoinFragment(string alias, bool innerJoin, bool includeSublasses) {
			return String.Empty;
		}


	}
}
