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

namespace NHibernate.Persister
{
	/// <summary>
	/// Default implementation of the <c>ClassPersister</c> interface. Implements the
	/// "table-per-class hierarchy" mapping strategy for an entity class.
	/// </summary>
	public class EntityPersister : AbstractEntityPersister, IQueryable
	{
		private readonly ISessionFactoryImplementor factory;

		// the class hierarchy structure
		private readonly string qualifiedTableName;
		private readonly string[ ] tableNames;
		private readonly bool hasUpdateableColumns;
		private readonly System.Type[ ] subclassClosure;

		// SQL strings
		private SqlString sqlDeleteString;
		private SqlString sqlInsertString;
		private SqlString sqlUpdateString;
		private SqlString sqlIdentityInsertString;

		// properties of this class, including inherited properties
		private readonly int[ ] propertyColumnSpans;
		private readonly bool[ ] propertyDefinedOnSubclass;
		private readonly string[ ][ ] propertyColumnNames;
		private readonly string[ ][ ] propertyColumnAliases;

		// the closure of all columns used by the entire hierarchy including
		// subclasses and superclasses of this class
		private readonly string[ ] subclassColumnClosure;
		private readonly string[ ] subclassFormulaTemplateClosure;
		private readonly string[ ] subclassFormulaClosure;
		private readonly string[ ] subclassColumnAliasClosure;
		private readonly string[ ] subclassFormulaAliasClosure;

		// the closure of all properties in the entire hierarchy including
		// subclasses and superclasses of this class
		private readonly string[ ][ ] subclassPropertyColumnNameClosure;
		private readonly string[ ] subclassPropertyNameClosure;
		private readonly IType[ ] subclassPropertyTypeClosure;
		private readonly OuterJoinLoaderType[ ] subclassPropertyEnableJoinedFetch;

		// discriminator column
		private readonly Hashtable subclassesByDiscriminatorValue = new Hashtable();
		private readonly bool forceDiscriminator;
		private readonly string discriminatorColumnName;
		private readonly IDiscriminatorType discriminatorType;
		private readonly string discriminatorSQLString;

		private readonly IDictionary loaders = new Hashtable();
		private readonly IDictionary lockers = new Hashtable();

//		private readonly string[ ] StringArray = {};
//		private readonly IType[ ] TypeArray = {};

		private static readonly ILog log = LogManager.GetLogger( typeof( EntityPersister ) );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="model"></param>
		/// <param name="factory"></param>
		public EntityPersister( PersistentClass model, ISessionFactoryImplementor factory )
			: base( model, factory )
		{
			// CLASS + TABLE

			System.Type mappedClass = model.PersistentClazz;
			this.factory = factory;
			Table table = model.RootTable;
			qualifiedTableName = table.GetQualifiedName( Dialect, factory.DefaultSchema );
			tableNames = new string[ ] {qualifiedTableName};

			// detect mapping errors
			HashedSet distinctColumns = new HashedSet();

			// DISCRIMINATOR

			object discriminatorValue;
			if( model.IsPolymorphic )
			{
				Value d = model.Discriminator;
				if( d == null )
				{
					throw new MappingException( "discriminator mapping required for polymorphic persistence" );
				}
				forceDiscriminator = model.IsForceDiscriminator;

				// the discriminator will have only one column 
				foreach( Column discColumn in d.ColumnCollection )
				{
					discriminatorColumnName = discColumn.GetQuotedName( Dialect );
				}

				try
				{
					discriminatorType = ( IDiscriminatorType ) model.Discriminator.Type;
					if( "null".Equals( model.DiscriminatorValue ) )
					{
						discriminatorValue = null;
						discriminatorSQLString = "null";
					}
					else
					{
						discriminatorValue = discriminatorType.StringToObject( model.DiscriminatorValue );
						discriminatorSQLString = discriminatorType.ObjectToSQLString( discriminatorValue );
					}
				} 
					// TODO: add a ClassCastException here to catch illegal disc types
				catch( Exception e )
				{
					throw new MappingException( "Could not format discriminator value to sql string", e );
				}

				distinctColumns.Add( discriminatorColumnName );

			}
			else
			{
				forceDiscriminator = false;
				discriminatorColumnName = null;
				discriminatorValue = null;
				discriminatorType = null;
				discriminatorSQLString = null;
			}

			// PROPERTIES
			CheckColumnDuplication( distinctColumns, model.Key.ColumnCollection );

			propertyColumnNames = new string[hydrateSpan][ ];
			propertyColumnAliases = new string[hydrateSpan][ ];
			propertyColumnSpans = new int[hydrateSpan];
			ArrayList thisClassProperties = new ArrayList();

			int i = 0;
			bool foundColumn = false;
			foreach( Mapping.Property prop in model.PropertyClosureCollection )
			{
				thisClassProperties.Add( prop );

				if( prop.IsFormula )
				{
					propertyColumnAliases[ i ] = new string[ ] {prop.Formula.Alias};
					propertyColumnSpans[ i ] = 1;
				}
				else
				{
					int span = prop.ColumnSpan;
					propertyColumnSpans[ i ] = span;

					string[ ] colNames = new string[span];
					string[ ] colAliases = new string[span];
					int j = 0;
					foreach( Column col in prop.ColumnCollection )
					{
						colAliases[ j ] = col.Alias( Dialect );
						colNames[ j ] = col.GetQuotedName( Dialect );
						j++;
						if( prop.IsUpdateable )
						{
							foundColumn = true;
						}
					}
					propertyColumnNames[ i ] = colNames;
					propertyColumnAliases[ i ] = colAliases;
				}
				i++;

				// columns must be unique across all subclasses
				if( prop.IsUpdateable || prop.IsInsertable )
				{
					CheckColumnDuplication( distinctColumns, prop.ColumnCollection );
				}
			}

			hasUpdateableColumns = foundColumn;

			ArrayList columns = new ArrayList();
			ArrayList formulas = new ArrayList();
			ArrayList formulaTemplates = new ArrayList();
			ArrayList types = new ArrayList();
			ArrayList names = new ArrayList();
			ArrayList propColumns = new ArrayList();
			ArrayList aliases = new ArrayList();
			ArrayList formulaAliases = new ArrayList();
			ArrayList joinedFetchesList = new ArrayList();
			ArrayList definedBySubclass = new ArrayList();

			foreach( Mapping.Property prop in model.SubclassPropertyClosureCollection )
			{
				names.Add( prop.Name );
				definedBySubclass.Add( !thisClassProperties.Contains( prop ) );

				if( prop.IsFormula )
				{
					formulas.Add( prop.Formula.FormulaString );
					formulaTemplates.Add( prop.Formula.GetTemplate( Dialect ) );
					propColumns.Add( new string[0] );
					formulaAliases.Add( prop.Formula.Alias );
					types.Add( prop.Type );
				}
				else
				{
					string[ ] cols = new string[prop.ColumnSpan];
					types.Add( prop.Type );
					int l = 0;
					foreach( Column col in prop.ColumnCollection )
					{
						columns.Add( col.GetQuotedName( Dialect ) );
						aliases.Add( col.Alias( Dialect ) );
						cols[ l++ ] = col.GetQuotedName( Dialect );
					}
					propColumns.Add( cols );
				}
				joinedFetchesList.Add( prop.Value.OuterJoinFetchSetting );
			}

			subclassColumnClosure = ( string[ ] ) columns.ToArray( typeof( string ) );
			subclassFormulaClosure = ( string[ ] ) formulas.ToArray( typeof( string ) );
			subclassFormulaTemplateClosure = ( string[ ] ) formulaTemplates.ToArray( typeof( string ) );
			subclassPropertyTypeClosure = ( IType[ ] ) types.ToArray( typeof( IType ) );
			subclassColumnAliasClosure = ( string[ ] ) aliases.ToArray( typeof( string ) );
			subclassFormulaAliasClosure = ( string[ ] ) formulaAliases.ToArray( typeof( string ) );
			subclassPropertyNameClosure = ( string[ ] ) names.ToArray( typeof( string ) );
			subclassPropertyColumnNameClosure = ( string[ ][ ] ) propColumns.ToArray( typeof( string[ ] ) );

			subclassPropertyEnableJoinedFetch = new OuterJoinLoaderType[joinedFetchesList.Count];
			int m = 0;
			foreach( OuterJoinLoaderType qq in joinedFetchesList )
			{
				subclassPropertyEnableJoinedFetch[ m++ ] = qq;
			}

			propertyDefinedOnSubclass = new bool[definedBySubclass.Count];
			m = 0;
			foreach( bool val in definedBySubclass )
			{
				propertyDefinedOnSubclass[ m++ ] = val;
			}

			int subclassSpan = model.SubclassSpan + 1;
			subclassClosure = new System.Type[subclassSpan];
			subclassClosure[ 0 ] = mappedClass;
			if( model.IsPolymorphic )
			{
				if( discriminatorValue == null )
				{
					subclassesByDiscriminatorValue.Add( ObjectUtils.Null, mappedClass );
				}
				else
				{
					subclassesByDiscriminatorValue.Add( discriminatorValue, mappedClass );
				}

			}

			// SUBCLASSES
			if( model.IsPolymorphic )
			{
				int k = 1;
				foreach( Subclass sc in model.SubclassCollection )
				{
					subclassClosure[ k++ ] = sc.PersistentClazz;
					if( "null".Equals( sc.DiscriminatorValue ) )
					{
						subclassesByDiscriminatorValue.Add( ObjectUtils.Null, sc.PersistentClazz );
					}
					else
					{
						try
						{
							subclassesByDiscriminatorValue.Add(
								discriminatorType.StringToObject( sc.DiscriminatorValue ),
								sc.PersistentClazz );
						}
						catch( Exception e )
						{
							throw new MappingException( "Error parsing discriminator value", e );
						}
					}
				}
			}
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
					string[ ] columns = ( string[ ] ) columnNamesByPropertyPath[ path ];
					if( columns.Length == 0 )
					{
						columns = IdentifierColumnNames;
					} //1-to-1 assoc
					EntityType etype = ( EntityType ) type;
					IType idType = factory.GetIdentifierType( etype.PersistentClass );

					string idpath = path + StringHelper.Dot + PathExpressionParser.EntityID;
					mods.Add( idpath, idType );
					columnNamesByPropertyPath.Add( idpath, columns );
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
							mods.Add( subidpath, actype.Subtypes[ i ] );
						}
					}
				}
			}

			foreach( DictionaryEntry de in mods )
			{
				typesByPropertyPath.Add( de.Key, de.Value );
			}

			IUniqueEntityLoader loader = new EntityLoader( this, factory );

			// initialize the SqlStrings - these are in the PostInstantiate method because we need
			// to have every other IClassPersister loaded so we can resolve the IType for the 
			// relationships.  In Hibernate they are able to just use ? and not worry about Parameters until
			// the statement is actually called.  We need to worry about Parameters when we are building
			// the IClassPersister...
			sqlDeleteString = GenerateDeleteString();
			sqlInsertString = GenerateInsertString( false, PropertyInsertability );
			sqlIdentityInsertString = IsIdentifierAssignedByInsert ?
				GenerateInsertString( true, PropertyInsertability ) :
				null;

			sqlUpdateString = GenerateUpdateString( PropertyUpdateability );

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


			loaders.Add( LockMode.None, loader );
			loaders.Add( LockMode.Read, loader );

			SqlString selectForUpdateString = factory.Dialect.SupportsForUpdate ?
				GenerateSelectForUpdateString( " FOR UPDATE" ) : GenerateSelectForUpdateString( null );

			loaders.Add( LockMode.Upgrade, new SimpleEntityLoader( this, selectForUpdateString, LockMode.Upgrade, factory.Dialect ) );

			SqlString selectForUpdateNoWaitString = factory.Dialect.SupportsForUpdateNoWait ?
				GenerateSelectForUpdateString( " FOR UPDATE NOWAIT" ) :
				selectForUpdateString.Clone();

			loaders.Add( LockMode.UpgradeNoWait, new SimpleEntityLoader( this, selectForUpdateNoWaitString, LockMode.UpgradeNoWait, factory.Dialect ) );

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
		public override OuterJoinLoaderType EnableJoinedFetch( int i )
		{
			return subclassPropertyEnableJoinedFetch[ i ];
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

		/// <summary></summary>
		public override int CountSubclassProperties()
		{
			return subclassPropertyTypeClosure.Length;
		}

		/// <summary></summary>
		public override string TableName
		{
			get { return qualifiedTableName; }
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
			return propertyColumnAliases[ i ];
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
			if( value == null )
			{
				return ( System.Type ) subclassesByDiscriminatorValue[ ObjectUtils.Null ];
			}
			else
			{
				return ( System.Type ) subclassesByDiscriminatorValue[ value ];
			}
		}

		/// <summary></summary>
		public override object IdentifierSpace
		{
			get { return qualifiedTableName; }
		}

		/// <summary></summary>
		public override object[ ] PropertySpaces
		{
			get { return tableNames; }
		}

		/// <summary>
		/// The SqlString do Delete this Entity.
		/// </summary>
		protected SqlString SqlDeleteString
		{
			get { return sqlDeleteString; }
		}

		/// <summary>
		/// The SqlString to Insert this Entity
		/// </summary>
		protected SqlString SqlInsertString
		{
			get { return sqlInsertString; }
		}

		/// <summary>
		/// The SqlString to Insert this Entity using a natively generated Id.
		/// </summary>
		protected SqlString SqlIdentityInsertString
		{
			get { return sqlIdentityInsertString; }
		}

		/// <summary>
		/// The SqlString to Update this Entity.
		/// </summary>
		protected SqlString SqlUpdateString
		{
			get { return sqlUpdateString; }
		}

		/// <summary>
		/// Generates an SqlString that encapsulates what later will be translated
		/// to an ADO.NET IDbCommand to Delete this Entity.
		/// </summary>
		/// <returns>A SqlString for a Delete</returns>
		protected virtual SqlString GenerateDeleteString()
		{
			SqlDeleteBuilder deleteBuilder = new SqlDeleteBuilder( factory );
			deleteBuilder.SetTableName( TableName )
				.SetIdentityColumn( IdentifierColumnNames, IdentifierType );

			if( IsVersioned )
			{
				deleteBuilder.SetVersionColumn( new string[ ] {VersionColumnName}, VersionType );
			}

			return deleteBuilder.ToSqlString();

		}

		/// <summary>
		/// Generates an SqlString that encapsulates what later will be translated
		/// to an ADO.NET IDbCommand to Insert this Entity.
		/// </summary>
		/// <param name="identityInsert"></param>
		/// <param name="includeProperty"></param>
		/// <returns>A SqlString for an Insert</returns>
		protected virtual SqlString GenerateInsertString( bool identityInsert, bool[ ] includeProperty )
		{
			SqlInsertBuilder builder = new SqlInsertBuilder( factory );
			builder.SetTableName( TableName );

			for( int i = 0; i < hydrateSpan; i++ )
			{
				if( includeProperty[ i ] )
				{
					builder.AddColumn( propertyColumnNames[ i ], PropertyTypes[ i ] );
				}
			}

			if( IsPolymorphic )
			{
				builder.AddColumn( DiscriminatorColumnName, DiscriminatorSQLString );
			}

			if( identityInsert == false )
			{
				builder.AddColumn( IdentifierColumnNames, IdentifierType );
			}
			else
			{
				// make sure the Dialect has an identity insert string because we don't want
				// to add the column when there is no value to supply the SqlBuilder
				if( Dialect.IdentityInsertString != null )
				{
					// only 1 column if there is IdentityInsert enabled.
					builder.AddColumn( IdentifierColumnNames[ 0 ], Dialect.IdentityInsertString );
				}
			}

			return builder.ToSqlString();
		}


		/// <summary>
		/// Generates an SqlString that encapsulates what later will be translated
		/// to an ADO.NET IDbCommand to Update this Entity.
		/// </summary>
		/// <param name="includeProperty"></param>
		/// <returns>A SqlString for an Update</returns>
		protected virtual SqlString GenerateUpdateString( bool[ ] includeProperty )
		{
			SqlUpdateBuilder updateBuilder = new SqlUpdateBuilder( factory );

			updateBuilder.SetTableName( TableName );

			for( int i = 0; i < hydrateSpan; i++ )
			{
				if( includeProperty[ i ] )
				{
					updateBuilder.AddColumns( propertyColumnNames[ i ], PropertyTypes[ i ] );
				}
			}

			updateBuilder.SetIdentityColumn( IdentifierColumnNames, IdentifierType );
			if( IsVersioned )
			{
				updateBuilder.SetVersionColumn( new string[ ] {VersionColumnName}, VersionType );
			}

			return updateBuilder.ToSqlString();

		}


		/// <summary>
		/// Generates a SqlString that will append the forUpdateFragment to the sql.
		/// </summary>
		/// <param name="forUpdateFragment"></param>
		/// <returns>A new SqlString</returns>
		protected virtual SqlString GenerateSelectForUpdateString( string forUpdateFragment )
		{
			SqlString forUpdateSqlString = null;

			SqlSimpleSelectBuilder builder = new SqlSimpleSelectBuilder( factory );

			// set the table name and add the columns to select
			builder.SetTableName( TableName )
				.AddColumns( IdentifierColumnNames )
				.AddColumns( subclassColumnClosure, subclassColumnAliasClosure )
				.AddColumns( subclassFormulaClosure, subclassFormulaAliasClosure );

			if( HasSubclasses )
			{
				builder.AddColumn( DiscriminatorColumnName );
			}

			// add the parameters to use in the WHERE clause
			// TODO: find out why version isn't used here
			builder.SetIdentityColumn( IdentifierColumnNames, IdentifierType );

			forUpdateSqlString = builder.ToSqlString();


			// add any special text that is contained in the forUpdateFragment
			if( forUpdateFragment != null && forUpdateFragment.Length > 0 )
			{
				SqlStringBuilder sqlBuilder = new SqlStringBuilder( forUpdateSqlString );
				sqlBuilder.Add( forUpdateFragment );
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
		protected virtual SqlString GenerateLockString( SqlString sqlString, string forUpdateFragment )
		{
			SqlStringBuilder sqlBuilder = null;

			if( sqlString == null )
			{
				SqlSimpleSelectBuilder builder = new SqlSimpleSelectBuilder( factory );

				// set the table name and add the columns to select
				builder.SetTableName( TableName )
					.AddColumns( IdentifierColumnNames );

				// add the parameters to use in the WHERE clause
				builder.SetIdentityColumn( IdentifierColumnNames, IdentifierType );
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
		/// Marshall the fields of a persistent instance to a prepared statement
		/// </summary>
		/// <param name="id"></param>
		/// <param name="fields">The fields to write to the command.</param>
		/// <param name="includeProperty">A bool indicating if the Property should be written to the Command</param>
		/// <param name="st"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		protected virtual int Dehydrate( object id, object[ ] fields, bool[ ] includeProperty, IDbCommand st, ISessionImplementor session )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Dehydrating entity: " + ClassName + '#' + id );
			}

			int index = 1;

			// there's a pretty strong coupling between the order of the SQL parameter 
			// construction and the actual order of the parameter collection. 
			index = 0;

			for( int j = 0; j < hydrateSpan; j++ )
			{
				if( includeProperty[ j ] )
				{
					PropertyTypes[ j ].NullSafeSet( st, fields[ j ], index, session );
					index += propertyColumnSpans[ j ];
				}
			}

			if( id != null )
			{
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
		public override object Load( object id, object optionalObject, LockMode lockMode, ISessionImplementor session )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Materializing entity: " + ClassName + '#' + id );
			}

			return ( ( IUniqueEntityLoader ) loaders[ lockMode ] ).Load( session, id, optionalObject );
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
				IDataReader reader = null;

				try
				{
					IdentifierType.NullSafeSet( st, id, 0, session );

					if( IsVersioned )
					{
						VersionType.NullSafeSet( st, version, IdentifierColumnNames.Length, session );
					}

					reader = session.Batcher.ExecuteReader( st );

					if( reader.Read() == false )
					{
						throw new StaleObjectStateException( MappedClass, id );
					}
				} 
					//TODO: add something to catch a sql exception and log it here
				finally
				{
					session.Batcher.CloseCommand( st, reader );
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
				return Insert( fields, notNull, GenerateInsertString( true, notNull ), obj, session );
			}
			else
			{
				return Insert( fields, PropertyInsertability, SqlIdentityInsertString, obj, session );
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
				Insert( id, fields, notNull, GenerateInsertString( false, notNull ), obj, session );
			}
			else
			{
				Insert( id, fields, PropertyInsertability, SqlInsertString, obj, session );
			}
		}

		/// <summary>
		/// Persist an object
		/// </summary>
		/// <param name="id"></param>
		/// <param name="fields"></param>
		/// <param name="notNull"></param>
		/// <param name="sql"></param>
		/// <param name="obj"></param>
		/// <param name="session"></param>
		public void Insert( object id, object[ ] fields, bool[ ] notNull, SqlString sql, object obj, ISessionImplementor session )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Inserting entity: " + ClassName + '#' + id );
				if( IsVersioned )
				{
					log.Debug( "Version: " + Versioning.GetVersion( fields, this ) );
				}
			}

			// Render the SQL query
			IDbCommand insertCmd = session.Batcher.PrepareBatchCommand( sql );

			try
			{
				// Write the values of the field onto the prepared statement - we MUST use the
				// state at the time the insert was issued (cos of foreign key constraints)
				// not necessarily the obect's current state

				Dehydrate( id, fields, notNull, insertCmd, session );

				session.Batcher.AddToBatch( 1 );

			}
				//TODO: add SQLException catching here...
			catch( Exception e )
			{
				session.Batcher.AbortBatch( e );
				throw;
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
		public object Insert( object[ ] fields, bool[ ] notNull, SqlString sql, object obj, ISessionImplementor session )
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

			if( Dialect.SupportsIdentitySelectInInsert )
			{
				statement = session.Batcher.PrepareCommand( Dialect.AddIdentitySelectToInsert( sql ) );
				idSelect = statement;
			}
			else
			{
				// do not Prepare the Command to be part of a batch.  When the second command
				// is Prepared for the Batch that would cause the first one to be executed and
				// we don't want that yet.  
				statement = session.Batcher.Generate( sql );
				idSelect = session.Batcher.PrepareCommand( new SqlString( SqlIdentitySelect ) );
			}

			try
			{
				Dehydrate( null, fields, notNull, statement, session );
			}
			catch( Exception e )
			{
				throw new HibernateException( "EntityPersister had a problem Dehydrating for an Insert", e );
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
				object id;

				if( !rs.Read() )
				{
					throw new HibernateException( "The database returned no natively generated identity value" );
				}
				id = IdentifierGeneratorFactory.Get( rs, IdentifierType.ReturnedClass );

				log.Debug( "Natively generated identity: " + id );

				return id;
			} 
				//TODO: add SQLException logging here
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
		}

		/// <summary>
		/// Perform a Deletion of the Entity from the Database.
		/// </summary>
		/// <param name="id">The id of the Object to Delete.</param>
		/// <param name="version">The version of the Object to Delete.</param>
		/// <param name="obj">The Object to Delete.</param>
		/// <param name="session">The Session to perform the Deletion in.</param>
		public override void Delete( object id, object version, object obj, ISessionImplementor session )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Deleting entity: " + ClassName + '#' + id );
				if( IsVersioned )
				{
					log.Debug( "Version: " + version );
				}
			}

			IDbCommand deleteCmd = null;

			if( IsVersioned )
			{
				deleteCmd = session.Batcher.PrepareCommand( SqlDeleteString );
			}
			else
			{
				deleteCmd = session.Batcher.PrepareBatchCommand( SqlDeleteString );
			}


			try
			{
				// Do the key.  The key is immutable so we can use the _current_ object state - not necessarily
				// the state at the time the delete was issued.
				IdentifierType.NullSafeSet( deleteCmd, id, 0, session );

				// we should use the _current_ object state (ie. after any updates that occurred during flush)
				if( IsVersioned )
				{
					VersionType.NullSafeSet( deleteCmd, version, IdentifierColumnNames.Length, session );
					Check( session.Batcher.ExecuteNonQuery( deleteCmd ), id );
				}
				else
				{
					session.Batcher.AddToBatch( 1 );
				}
			}
				// TODO: h2.0.3 - add some Sql Exception logging here
			catch( Exception e )
			{
				if( IsVersioned )
				{
					//TODO: add some logging.
				}
				else
				{
					session.Batcher.AbortBatch( e );
				}
				throw;
			}
			finally
			{
				if( IsVersioned )
				{
					session.Batcher.CloseCommand( deleteCmd, null );
				}
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
		public override void Update( object id, object[ ] fields, int[ ] dirtyFields, object oldVersion, object obj, ISessionImplementor session )
		{
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
					//don't need to check property mutability (dirty checking algorithm handles that)
				}
				Update( id, fields, propsToUpdate, oldVersion, obj, GenerateUpdateString( propsToUpdate ), session );
			}
			else
			{
				Update( id, fields, PropertyUpdateability, oldVersion, obj, SqlUpdateString, session );
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
		protected virtual void Update( object id, object[ ] fields, bool[ ] includeProperty, object oldVersion, object obj, SqlString sqlUpdateString, ISessionImplementor session )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Updating entity: " + ClassName + '#' + id );
				if( IsVersioned )
				{
					log.Debug( "Existing version: " + oldVersion + " -> New Version: " + fields[ VersionProperty ] );
				}
			}

			if( !hasUpdateableColumns )
			{
				return;
			}

			IDbCommand statement = null;

			if( IsVersioned )
			{
				statement = session.Batcher.PrepareCommand( sqlUpdateString );
			}
			else
			{
				statement = session.Batcher.PrepareBatchCommand( sqlUpdateString );
			}

			try
			{
				// now write the values of fields onto the command

				int versionParamIndex = Dehydrate( id, fields, includeProperty, statement, session );

				if( IsVersioned )
				{
					VersionType.NullSafeSet( statement, oldVersion, versionParamIndex, session );
					Check( session.Batcher.ExecuteNonQuery( statement ), id );
				}
				else
				{
					session.Batcher.AddToBatch( 1 );
				}


			} 
				// TODO: h2.0.3 - add some sql exception logging here
			catch( Exception e )
			{
				if( IsVersioned )
				{
					// log an exception here
				}
				else
				{
					session.Batcher.AbortBatch( e );
				}

				throw;
			}
			finally
			{
				if( IsVersioned )
				{
					session.Batcher.CloseCommand( statement, null );
				}
			}
		}


		private void InitPropertyPaths( IMapping mapping )
		{
			IType[ ] propertyTypes = PropertyTypes;
			string[ ] propertyNames = PropertyNames;

			for( int i = 0; i < propertyNames.Length; i++ )
			{
				InitPropertyPaths( propertyNames[ i ], propertyTypes[ i ], propertyColumnNames[ i ], mapping );
			}

			string idProp = IdentifierPropertyName;
			if( idProp != null )
			{
				InitPropertyPaths( idProp, IdentifierType, IdentifierColumnNames, mapping );
			}
			if( HasEmbeddedIdentifier )
			{
				InitPropertyPaths( null, IdentifierType, IdentifierColumnNames, mapping );
			}
			InitPropertyPaths( PathExpressionParser.EntityID, IdentifierType, IdentifierColumnNames, mapping );

			if( IsPolymorphic )
			{
				typesByPropertyPath[ PathExpressionParser.EntityClass ] = DiscriminatorType;
				columnNamesByPropertyPath[ PathExpressionParser.EntityClass ] = new string[ ] {DiscriminatorColumnName};
			}
		}

		private void InitPropertyPaths( string propertyName, IType propertyType, string[ ] columns, IMapping mapping )
		{
			if( propertyName != null )
			{
				typesByPropertyPath[ propertyName ] = propertyType;
				columnNamesByPropertyPath[ propertyName ] = columns;
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
					InitPropertyPaths( path, types[ k ], slice, mapping );
				}
			}
		}

		/// <summary></summary>
		public virtual string[ ] TableNames
		{
			get { return tableNames; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		public override SqlString FromTableFragment( string alias )
		{
			return new SqlString( TableName + ' ' + alias );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="innerJoin"></param>
		/// <param name="includeSubclasses"></param>
		/// <returns></returns>
		public override SqlString QueryWhereFragment( string name, bool innerJoin, bool includeSubclasses )
		{
			if( innerJoin && ( forceDiscriminator || IsInherited ) )
			{
				InFragment frag = new InFragment()
					.SetColumn( name, DiscriminatorColumnName );
				System.Type[ ] subclasses = SubclassClosure;
				for( int i = 0; i < subclasses.Length; i++ )
				{
					frag.AddValue(
						( ( IQueryable ) factory.GetPersister( subclasses[ i ] ) ).DiscriminatorSQLString
						);
				}

				SqlStringBuilder builder = new SqlStringBuilder();
				builder.Add( " and " + frag.ToFragmentString() );

				if( HasWhere )
				{
					builder.Add( " and " + GetSQLWhereString( name ) );
				}

				return builder.ToSqlString();
			}
			else
			{
				if( HasWhere )
				{
					return new SqlString( " and " + GetSQLWhereString( name ) );
				}
				else
				{
					return new SqlString( String.Empty );
				}

			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="property"></param>
		/// <returns></returns>
		public override string[ ] ToColumns( string name, string property )
		{
			string[ ] cols = GetPropertyColumnNames( property );

			if( cols == null )
			{
				throw new QueryException( "unresolved property: " + property );
			}

			if( cols.Length == 0 )
			{
				// ie a nested collection or a one-to-one
				cols = IdentifierColumnNames;
			}

			// make sure an Alias was actually passed into the statement
			if( name != null && name.Length > 0 )
			{
				return StringHelper.Prefix( cols, name + StringHelper.Dot );
			}
			else
			{
				return cols;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		public override string[ ] ToColumns( string name, int i )
		{
			return StringHelper.Prefix( subclassPropertyColumnNameClosure[ i ], name + StringHelper.Dot );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override string GetSubclassPropertyTableName( int i )
		{
			return qualifiedTableName;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public override string GetSubclassPropertyName( int i )
		{
			return this.subclassPropertyNameClosure[ i ];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="suffix"></param>
		/// <returns></returns>
		public override SqlString PropertySelectFragment( string alias, string suffix )
		{
			SelectFragment frag = new SelectFragment( factory.Dialect );

			frag.SetSuffix( suffix );
			if( HasSubclasses )
			{
				frag.AddColumn( alias, DiscriminatorColumnName );
			}

			return frag.AddColumns( alias, subclassColumnClosure, subclassColumnAliasClosure )
				.AddFormulas( alias, subclassFormulaTemplateClosure, subclassFormulaAliasClosure )
				.ToSqlStringFragment();
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
			return new SqlString( String.Empty );
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
			return new SqlString( String.Empty );
		}


	}
}