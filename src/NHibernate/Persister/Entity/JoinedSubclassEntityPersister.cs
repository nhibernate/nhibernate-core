using System;
using System.Collections;
using System.Data;

using log4net;

using NHibernate.Engine;
using NHibernate.Hql;
using NHibernate.Impl;
using NHibernate.Loader.Entity;
using NHibernate.Mapping;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

using Array = System.Array;

namespace NHibernate.Persister.Entity
{
	/// <summary>
	/// A <c>IEntityPersister</c> implementing the normalized "table-per-subclass" mapping strategy
	/// </summary>
	public class JoinedSubclassEntityPersister : AbstractEntityPersister
	{
		// the class hierarchy structure
		private readonly int tableSpan;
		private readonly string qualifiedTableName;

		// all of the table names that this Persister uses for just its data 
		// it is indexed as tableNames[tableIndex]
		// for both the superclass and subclass the index of 0=basetable
		// for the base class there is only 1 table
		private readonly string[] tableNames;
		private readonly string[] naturalOrderTableNames;

		// two dimensional array that is indexed as tableKeyColumns[tableIndex][columnIndex]
		private readonly string[][] tableKeyColumns;
		private readonly string[][] naturalOrderTableKeyColumns;

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

		// SQL strings
		private SqlString[] sqlDeleteStrings;
		private SqlString[] sqlInsertStrings;
		private SqlString[] sqlIdentityInsertStrings;
		private SqlString[] sqlUpdateStrings;

		// the index of the table that the property is coming from
		// the array is indexed as propertyTables[propertyIndex] = tableIndex 
		private readonly int[] propertyTables;
		private readonly int[] naturalOrderPropertyTables;
		private bool[] propertyHasColumns;

		private readonly int[] subclassPropertyTableNumberClosure;
		private readonly int[] subclassColumnTableNumberClosure;

		private readonly Hashtable tableNumberByPropertyPath = new Hashtable();

		private readonly int[] subclassFormulaTableNumberClosure;

		// subclass discrimination works by assigning particular values to certain 
		// combinations of null primary key values in the outer join using an SQL CASE

		// key = DiscrimatorValue, value = Subclass Type
		private readonly Hashtable subclassesByDiscriminatorValue = new Hashtable();
		private readonly string[] discriminatorValues;
		private readonly string[] notNullColumns;
		private readonly int[] tableNumbers;

		private readonly IDiscriminatorType discriminatorType;
		private readonly string discriminatorSQLString;
		private readonly string discriminatorColumnName;
		private SqlString sqlConcreteSelectString;
		private SqlString sqlVersionSelectString;

		private static readonly ILog log = LogManager.GetLogger( typeof( JoinedSubclassEntityPersister ) );

		public override void PostInstantiate()
		{
			base.PostInstantiate();
			InitLockers();

			// initialize the Statements - these are in the PostInstantiate method because we need
			// to have every other IEntityPersister loaded so we can resolve the IType for the 
			// relationships.  In Hibernate they are able to just use ? and not worry about Parameters until
			// the statement is actually called.  We need to worry about Parameters when we are building
			// the IEntityPersister...

			sqlDeleteStrings = GenerateDeleteStrings();
			sqlInsertStrings = GenerateInsertStrings( false, PropertyInsertability );
			sqlIdentityInsertStrings = IsIdentifierAssignedByInsert ?
				GenerateInsertStrings( true, PropertyInsertability ) :
				null;

			sqlUpdateStrings = GenerateUpdateStrings( PropertyUpdateability );

			sqlVersionSelectString = GenerateSelectVersionString();
			sqlConcreteSelectString = GenerateConcreteSelectString();

			// This is used to determine updates for objects that came in via update()
			propertyHasColumns = new bool[sqlUpdateStrings.Length];
			for( int m = 0; m < sqlUpdateStrings.Length; m++ )
			{
				propertyHasColumns[ m ] = ( sqlUpdateStrings[ m ] != null );
			}
		}

		public override string DiscriminatorColumnName
		{
			get { return discriminatorColumnName; }
		}

		protected override string DiscriminatorAlias
		{
			// Is always "clazz_", so just use columnName
			get { return DiscriminatorColumnName; }
		}

		public override string GetSubclassPropertyTableName( int i )
		{
			return subclassTableNameClosure[ subclassPropertyTableNumberClosure[ i ] ];
		}

		public override IType DiscriminatorType
		{
			get { return discriminatorType; }
		}

		public override string DiscriminatorSQLValue
		{
			get { return discriminatorSQLString; }
		}

		public override System.Type GetSubclassForDiscriminatorValue( object value )
		{
			return ( System.Type ) subclassesByDiscriminatorValue[ value ];
		}

		public override object[] PropertySpaces
		{
			get
			{
				// don't need subclass tables, because they can't appear in conditions
				return tableNames;
			}
		}

		//Access cached SQL

		/// <summary>
		/// The queries that delete rows by id (and version)
		/// </summary>
		protected SqlString[] SqlDeleteStrings
		{
			get { return sqlDeleteStrings; }
		}

		/// <summary>
		/// The queries that insert rows with a given id
		/// </summary>
		protected SqlString[] SqlInsertStrings
		{
			get { return sqlInsertStrings; }
		}

		/// <summary>
		/// The queries that insert rows, letting the database generate an id
		/// </summary>
		protected SqlString[] SqlIdentityInsertStrings
		{
			get { return sqlIdentityInsertStrings; }
		}

		/// <summary>
		/// The queries that update rows by id (and version)
		/// </summary>
		protected SqlString[] SqlUpdateStrings
		{
			get { return sqlUpdateStrings; }
		}

		protected override SqlString VersionSelectString
		{
			get { return sqlVersionSelectString; }
		}

		// Generate all the SQL

		/// <summary>
		/// Generate the SQL that deletes rows by id (and version)
		/// </summary>
		/// <returns>An array of SqlStrings</returns>
		protected virtual SqlString[] GenerateDeleteStrings()
		{
			SqlString[] deleteStrings = new SqlString[naturalOrderTableNames.Length];

			for( int i = 0; i < naturalOrderTableNames.Length; i++ )
			{
				SqlDeleteBuilder deleteBuilder = new SqlDeleteBuilder( Factory );

				// TODO: find out why this is using tableKeyColumns and when
				// they would ever be different between the two tables - I thought
				// a requirement of Hibernate is that joined/subclassed tables
				// had to have the same pk - otherwise you had an association.
				deleteBuilder.SetTableName( naturalOrderTableNames[ i ] )
					.SetIdentityColumn( naturalOrderTableKeyColumns[ i ], IdentifierType );

				if( i == 0 && IsVersioned )
				{
					deleteBuilder.SetVersionColumn( new string[] {VersionColumnName}, VersionType );
				}

				deleteStrings[ i ] = deleteBuilder.ToSqlString();
			}

			return deleteStrings;
		}

		/// <summary>
		/// Generate the SQL that inserts rows
		/// </summary>
		/// <param name="identityInsert"></param>
		/// <param name="includeProperty"></param>
		/// <returns>An array of SqlStrings</returns>
		protected virtual SqlString[] GenerateInsertStrings( bool identityInsert, bool[] includeProperty )
		{
			SqlString[] insertStrings = new SqlString[naturalOrderTableNames.Length];

			for( int j = 0; j < naturalOrderTableNames.Length; j++ )
			{
				SqlInsertBuilder builder = new SqlInsertBuilder( Factory );
				builder.SetTableName( naturalOrderTableNames[ j ] );

				for( int i = 0; i < PropertyTypes.Length; i++ )
				{
					if( includeProperty[ i ] && naturalOrderPropertyTables[ i ] == j )
					{
						builder.AddColumn( GetPropertyColumnNames( i ), PropertyTypes[ i ] );
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
		/// Generate the SQL that updates rows by id (and version)
		/// </summary>
		/// <param name="includeProperty"></param>
		/// <returns>An array of SqlStrings</returns>
		protected virtual SqlString[] GenerateUpdateStrings( bool[] includeProperty )
		{
			SqlString[] results = new SqlString[naturalOrderTableNames.Length];

			for( int j = 0; j < naturalOrderTableNames.Length; j++ )
			{
				SqlUpdateBuilder updateBuilder = new SqlUpdateBuilder( Factory )
					.SetTableName( naturalOrderTableNames[ j ] )
					.SetIdentityColumn( naturalOrderTableKeyColumns[ j ], IdentifierType );

				if( j == 0 && IsVersioned )
				{
					updateBuilder.SetVersionColumn( new string[] {VersionColumnName}, VersionType );
				}

				//TODO: figure out what the hasColumns variable comes into play for??
				bool hasColumns = false;
				for( int i = 0; i < PropertyNames.Length; i++ )
				{
					if( includeProperty[ i ] && naturalOrderPropertyTables[ i ] == j )
					{
						updateBuilder.AddColumns( GetPropertyColumnNames( i ), PropertyTypes[ i ] );
						hasColumns = hasColumns || GetPropertyColumnSpan( i ) > 0;
					}
				}
				results[ j ] = hasColumns ? updateBuilder.ToSqlString() : null;
			}

			return results;
		}

		/// <summary>
		/// Generate the SQL that pessimistic locks a row by id (and version)
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
				SqlSimpleSelectBuilder builder = new SqlSimpleSelectBuilder( Factory );

				// set the table name and add the columns to select
				builder.SetTableName( qualifiedTableName )
					.AddColumn( base.IdentifierColumnNames[ 0 ] );

				// add the parameters to use in the WHERE clause
				builder.SetIdentityColumn( base.IdentifierColumnNames, IdentifierType );
				if( IsVersioned )
				{
					builder.SetVersionColumn( new string[] {VersionColumnName}, VersionType );
				}

				sqlBuilder = new SqlStringBuilder( builder.ToSqlString() );
			}
			else
			{
				sqlBuilder = new SqlStringBuilder( sqlString );
			}

			// add any special text that is contained in the forUpdateFragment
			if( forUpdateFragment != null && forUpdateFragment != string.Empty )
			{
				sqlBuilder.Add( forUpdateFragment );
			}

			return sqlBuilder.ToSqlString();
		}

		protected override SqlString ConcreteSelectString
		{
			get { return sqlConcreteSelectString; }
		}

		private const string ConcreteAlias = "x";

		protected virtual SqlString GenerateConcreteSelectString()
		{
			SqlStringBuilder select = new SqlStringBuilder();

			select
				.Add( "select " )
				.Add( string.Join( StringHelper.CommaSpace,
				                   StringHelper.Qualify( ConcreteAlias, IdentifierColumnNames ) ) )
				.Add( ConcretePropertySelectFragment( ConcreteAlias, PropertyUpdateability ) )
				.Add( " from " )
				.Add( FromTableFragment( ConcreteAlias ) )
				.Add( FromJoinFragment( ConcreteAlias, true, false ) )
				.Add( " where " )
				.Add( WhereJoinFragment( ConcreteAlias, true, false ) );

			Parameter[] idParameters = Parameter.GenerateParameters( Factory, IdentifierType );

			for( int i = 0; i < idParameters.Length; i++ )
			{
				if( i > 0 )
				{
					select.Add( " and " );
				}

				select
					.Add( IdentifierColumnNames[ i ] )
					.Add( " = " )
					.Add( idParameters[ i ] );
			}

			if( IsVersioned )
			{
				Parameter[] versionParameters = Parameter.GenerateParameters(Factory, VersionType);
				select.Add( " and " )
					.Add( VersionColumnName )
					.Add( " = " )
					.Add( versionParameters[ 0 ] );
			}

			return select.ToSqlString();
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
		protected virtual int Dehydrate( object id, object[] fields, bool[] includeProperty, IDbCommand[] statements, ISessionImplementor session )
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

		protected override int Dehydrate( object id, object[] fields, bool[] includeProperty, int table, IDbCommand statement, ISessionImplementor session )
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
					index += GetPropertyColumnSpan( j );
				}
			}

			if( id != null )
			{
				IdentifierType.NullSafeSet( statement, id, index, session );
				index += IdentifierColumnNames.Length;
			}

			return index;
		}

		// Execute the SQL:

		public override object Insert( object[] fields, object obj, ISessionImplementor session )
		{
			if( UseDynamicInsert )
			{
				bool[] notNull = GetNotNullInsertableColumns( fields );
				return Insert( fields, notNull, GenerateInsertStrings( true, notNull ), obj, session );
			}
			else
			{
				return Insert( fields, PropertyInsertability, SqlIdentityInsertStrings, obj, session );
			}
		}

		public override void Insert( object id, object[] fields, object obj, ISessionImplementor session )
		{
			if( UseDynamicInsert )
			{
				bool[] notNull = GetNotNullInsertableColumns( fields );
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
		public void Insert( object id, object[] fields, bool[] notNull, SqlString[] sql, object obj, ISessionImplementor session )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Inserting entity: " + MessageHelper.InfoString( this, id ) );
				if( IsVersioned )
				{
					log.Debug( "Version: " + Versioning.GetVersion( fields, this ) );
				}
			}

			try
			{
				// render the SQL query
				IDbCommand[] insertCmds = new IDbCommand[tableNames.Length];

				try
				{
					for( int i = 0; i < tableNames.Length; i++ )
					{
						// TODO SP
						insertCmds[i] = session.Batcher.PrepareCommand( sql[ i ], CommandType.Text );
					}

					// write the value of fields onto the prepared statements - we MUST use the state at the time
					// the insert was issued (cos of foreign key constraints).
					Dehydrate( id, fields, notNull, insertCmds, session );

					for( int i = 0; i < tableNames.Length; i++ )
					{
						session.Batcher.ExecuteNonQuery( insertCmds[ i ], sql[i].GetParameterTypes() );
					}
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
			catch( HibernateException )
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch( Exception sqle )
			{
				throw Convert( sqle, "could not insert: " + MessageHelper.InfoString( this, id ) );
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
		public object Insert( object[] fields, bool[] notNull, SqlString[] sql, object obj, ISessionImplementor session )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Inserting entity: " + ClassName + " (native id)" );
				if( IsVersioned )
				{
					log.Debug( "Version: " + Versioning.GetVersion( fields, this ) );
				}
			}

			try
			{
				object id = InsertImpl( fields, notNull, sql[0], obj, session);

				for( int i = 1; i < naturalOrderTableNames.Length; i++ )
				{
					// TODO SP
					IDbCommand statement = session.Batcher.PrepareCommand( sql[ i ], CommandType.Text );

					try
					{
						Dehydrate( id, fields, notNull, i, statement, session );
						session.Batcher.ExecuteNonQuery( statement, sql[i].GetParameterTypes() );
					}
					finally
					{
						session.Batcher.CloseCommand( statement, null );
					}
				}

				return id;
			}
			catch( HibernateException )
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch( Exception sqle )
			{
				throw Convert( sqle, "could not insert: " + MessageHelper.InfoString( this ) );
			}
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
				log.Debug( "Deleting entity: " + MessageHelper.InfoString( this, id ) );
			}

			try
			{
				IDbCommand[] statements = new IDbCommand[naturalOrderTableNames.Length];
				SqlString[] sqls = SqlDeleteStrings;
				try
				{
					for( int i = 0; i < naturalOrderTableNames.Length; i++ )
					{
						// TODO SP
						statements[ i ] = session.Batcher.PrepareCommand( sqls[ i ], CommandType.Text );
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

						Check( session.Batcher.ExecuteNonQuery( statements[ i ], sqls[i].GetParameterTypes() ), id );
					}
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
			catch( HibernateException )
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch( Exception sqle )
			{
				throw Convert( sqle, "could not delete: " + MessageHelper.InfoString( this, id ) );
			}
		}

		/// <summary>
		/// Decide which tables need to be updated
		/// </summary>
		/// <param name="dirtyProperties"></param>
		/// <returns></returns>
		private bool[] GetTableUpdateNeeded( int[] dirtyProperties )
		{
			if( dirtyProperties == null )
			{
				return propertyHasColumns; //for object that came in via update()
			}
			else
			{
				bool[] updateability = PropertyUpdateability;
				bool[] tableUpdateNeeded = new bool[naturalOrderTableNames.Length];
				for( int i = 0; i < dirtyProperties.Length; i++ )
				{
					int property = dirtyProperties[ i ];
					int table = naturalOrderPropertyTables[ property ];
					tableUpdateNeeded[ table ] = tableUpdateNeeded[ table ] ||
						( GetPropertyColumnSpan( property ) > 0 && updateability[ property ] );
				}
				if( IsVersioned )
				{
					tableUpdateNeeded[ 0 ] = true;
					// = tableUpdateNeeded[ 0 ] || Versioning.IsVersionIncrementRequired(...);
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
		public override void Update( object id, object[] fields, int[] dirtyFields, object[] oldFields, object oldVersion, object obj, ISessionImplementor session )
		{
			bool[] tableUpdateNeeded = GetTableUpdateNeeded( dirtyFields );

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

		protected virtual void Update( object id, object[] fields, bool[] includeProperty, bool[] includeTable, object oldVersion, object obj, SqlString[] sql, ISessionImplementor session )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Updating entity: " + MessageHelper.InfoString( this, id ) );
				if( IsVersioned )
				{
					log.Debug( "Existing version: " + oldVersion + " -> New version: " + fields[ VersionProperty ] );
				}
			}

			int tables = naturalOrderTableNames.Length;

			try
			{
				IDbCommand[] statements = new IDbCommand[tables];

				try
				{
					for( int i = 0; i < tables; i++ )
					{
						if( includeTable[ i ] )
						{
							// TODO SP
							statements[ i ] = session.Batcher.PrepareCommand( sql[ i ], CommandType.Text );
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
							Check( session.Batcher.ExecuteNonQuery( statements[ i ], sql[i].GetParameterTypes() ), id );
						}
					}
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
			catch( HibernateException )
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch( Exception sqle )
			{
				throw Convert( sqle, "could not update: " + MessageHelper.InfoString( this, id ) );
			}
		}

		//INITIALIZATION:

		/// <summary>
		/// Constructs the NormalizedEntityPerister for the PersistentClass.
		/// </summary>
		/// <param name="model">The PeristentClass to create the EntityPersister for.</param>
		/// <param name="factory">The SessionFactory that this EntityPersister will be stored in.</param>
		public JoinedSubclassEntityPersister( PersistentClass model, ISessionFactoryImplementor factory, IMapping mapping )
			: base( model, factory )
		{
			// I am am making heavy use of the "this." just to help me with debugging what is a local variable to the 
			// constructor versus what is an class scoped variable.  I am only doing this when we are using fields 
			// instead of properties because it is easy to tell properties by the Case.

			// CLASS + TABLE

			Table table = model.RootTable;
			this.qualifiedTableName = table.GetQualifiedName( Dialect, Factory.DefaultSchema );

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

			if( OptimisticLockMode != OptimisticLockMode.Version )
			{
				throw new MappingException( "optimistic-lock attribute not supported for joined-subclass mappings: " + ClassName );
			}

			//MULTITABLES

			// these two will later be converted into arrays for the fields tableNames and tableKeyColumns
			ArrayList tables = new ArrayList();
			ArrayList keyColumns = new ArrayList();
			tables.Add( this.qualifiedTableName );
			keyColumns.Add( base.IdentifierColumnNames );

			// move through each table that contains the data for this entity.
			foreach( Table tab in model.TableClosureCollection )
			{
				string tabname = tab.GetQualifiedName( Dialect, factory.DefaultSchema );
				if( !tabname.Equals( qualifiedTableName ) )
				{
					tables.Add( tabname );
					string[] key = new string[tab.PrimaryKey.ColumnCollection.Count];
					int k = 0;
					foreach( Column col in tab.PrimaryKey.ColumnCollection )
					{
						key[ k++ ] = col.GetQuotedName( Dialect );
					}
					keyColumns.Add( key );
				}
			}

			this.naturalOrderTableNames = ( string[] ) tables.ToArray( typeof( string ) );
			this.naturalOrderTableKeyColumns = ( string[][] ) keyColumns.ToArray( typeof( string[] ) );

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
					string[] key = new string[tab.PrimaryKey.ColumnCollection.Count];
					int k = 0;
					foreach( Column col in tab.PrimaryKey.ColumnCollection )
					{
						key[ k++ ] = col.GetQuotedName( Dialect );
					}
					keyColumns.Add( key );
				}
			}

			// convert the local ArrayList variables into arrays for the fields in the class
			this.subclassTableNameClosure = ( string[] ) subtables.ToArray( typeof( string ) );
			this.subclassTableKeyColumns = ( string[][] ) keyColumns.ToArray( typeof( string[] ) );
			this.isClassOrSuperclassTable = new bool[this.subclassTableNameClosure.Length];
			for( int j = 0; j < subclassTableNameClosure.Length; j++ )
			{
				this.isClassOrSuperclassTable[ j ] = tables.Contains( this.subclassTableNameClosure[ j ] );
			}

			int len = naturalOrderTableNames.Length;
			tableSpan = naturalOrderTableNames.Length;
			tableNames = Reverse( naturalOrderTableNames );
			tableKeyColumns = Reverse( naturalOrderTableKeyColumns );
			Array.Reverse( subclassTableNameClosure, 0, len );
			Array.Reverse( subclassTableKeyColumns, 0, len );

			// PROPERTIES

			// initialize the lengths of all of the Property related fields in the class
			this.propertyTables = new int[HydrateSpan];
			this.naturalOrderPropertyTables = new int[HydrateSpan];

			int i = 0;
			foreach( Mapping.Property prop in model.PropertyClosureCollection )
			{
				Table tab = prop.Value.Table;
				string tabname = tab.GetQualifiedName( Dialect, factory.DefaultSchema );
				this.propertyTables[ i ] = GetTableId( tabname, this.tableNames );
				this.naturalOrderPropertyTables[ i ] = GetTableId( tabname, this.naturalOrderTableNames );
				i++;
			}

			// SUBCLASS CLOSURE PROPERTIES

			ArrayList columnTableNumbers = new ArrayList(); //this.subclassColumnTableNumberClosure
			ArrayList formulaTableNumbers = new ArrayList();
			ArrayList propTableNumbers = new ArrayList(); // this.subclassPropertyTableNameClosure

			foreach( Mapping.Property prop in model.SubclassPropertyClosureCollection )
			{
				Table tab = prop.Value.Table;
				String tabname = tab.GetQualifiedName(
					factory.Dialect,
					factory.DefaultSchema );
				int tabnum = GetTableId( tabname, subclassTableNameClosure );
				propTableNumbers.Add( tabnum );

				foreach( ISelectable thing in prop.ColumnCollection )
				{
					if( thing.IsFormula )
					{
						formulaTableNumbers.Add( tabnum );
					}
					else
					{
						columnTableNumbers.Add( tabnum );
					}
				}
			}

			subclassColumnTableNumberClosure = ArrayHelper.ToIntArray( columnTableNumbers );
			subclassPropertyTableNumberClosure = ArrayHelper.ToIntArray( propTableNumbers );
			subclassFormulaTableNumberClosure = ArrayHelper.ToIntArray( formulaTableNumbers );

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
			PostConstruct( mapping );
		}

		/// <summary>
		/// Create a new one dimensional array sorted in the Reverse order of the original array.
		/// </summary>
		/// <param name="objects">The original array.</param>
		/// <returns>A new array in the reverse order of the original array.</returns>
		private static string[] Reverse( string[] objects )
		{
			int len = objects.Length;
			string[] temp = new string[len];
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
		private static string[][] Reverse( string[][] objects )
		{
			int len = objects.Length;
			string[][] temp = new string[len][];
			for( int i = 0; i < len; i++ )
			{
				temp[ i ] = objects[ len - i - 1 ];
			}
			return temp;
		}

		protected int GetPropertyTableNumber( string propertyName )
		{
			string[] propertyNames = PropertyNames;

			for( int i = 0; i < propertyNames.Length; i++ )
			{
				if( propertyName.Equals( propertyNames[ i ] ) )
				{
					return propertyTables[ i ];
				}
			}
			return 0;
		}

		protected override void HandlePath( string path, IType type )
		{
			if( type.IsAssociationType && ( ( IAssociationType ) type ).UseLHSPrimaryKey )
			{
				tableNumberByPropertyPath[ path ] = 0;
			}
			else
			{
				string propertyName = StringHelper.Root( path );
				tableNumberByPropertyPath[ path ] = GetPropertyTableNumber( propertyName );
			}
		}

		public override SqlString FromTableFragment( string alias )
		{
			return new SqlString( subclassTableNameClosure[ 0 ] + ' ' + alias );
		}

		public override string TableName
		{
			get { return subclassTableNameClosure[ 0 ]; }
		}

		private JoinFragment Outerjoin( string name, bool innerJoin, bool includeSubclasses )
		{
			JoinFragment outerjoin = Factory.Dialect.CreateOuterJoinFragment();
			for( int i = 1; i < subclassTableNameClosure.Length; i++ )
			{
				if( includeSubclasses || isClassOrSuperclassTable[ i ] )
				{
					outerjoin.AddJoin(
						subclassTableNameClosure[ i ],
						Alias( name, i ),
						StringHelper.Qualify( name, IdentifierColumnNames ),
						subclassTableKeyColumns[ i ],
						innerJoin && isClassOrSuperclassTable[ i ] ?
							JoinType.InnerJoin :
							JoinType.LeftOuterJoin );
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
		private int GetTableId( string tableName, string[] tables )
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

		public override string[] ToColumns( string alias, string property )
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
				return new string[] {DiscriminatorFragment( alias ).ToSqlStringFragment().ToString()};
			}
			else
			{
				return base.ToColumns( alias, property );
			}
		}

		protected string ConcretePropertySelectFragment( string alias, bool[] includeProperty )
		{
			int propertyCount = PropertyNames.Length;
			SelectFragment frag = new SelectFragment( Dialect );

			for( int i = 0; i < propertyCount; i++ )
			{
				if( includeProperty[ i ] )
				{
					frag.AddColumns(
						Alias( alias, propertyTables[ i ] ),
						GetPropertyColumnNames( i ),
						GetPropertyColumnAliases( i ) );
				}
			}

			return frag.ToSqlStringFragment();
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

		public override SqlString FromJoinFragment( string alias, bool innerJoin, bool includeSubclasses )
		{
			return Outerjoin( alias, innerJoin, includeSubclasses ).ToFromFragmentString;
		}

		public override SqlString WhereJoinFragment( string alias, bool innerJoin, bool includeSubclasses )
		{
			return Outerjoin( alias, innerJoin, includeSubclasses ).ToWhereFragmentString;
		}

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

		public override string[] IdentifierColumnNames
		{
			get { return tableKeyColumns[ 0 ]; }
		}

		public override bool IsCacheInvalidationRequired
		{
			get { return HasFormulaProperties || ( !IsVersioned && UseDynamicUpdate ); }
		}

		protected override string VersionedTableName
		{
			get { return qualifiedTableName; }
		}

		protected override int GetSubclassPropertyTableNumber( int i )
		{
			return subclassPropertyTableNumberClosure[ i ];
		}

		protected override void AddDiscriminatorToSelect( SelectFragment select, string name, string suffix )
		{
			select.SetExtraSelectList( DiscriminatorFragment( name ), DiscriminatorAlias );
		}

		protected override int[] SubclassColumnTableNumberClosure
		{
			get { return subclassColumnTableNumberClosure; }
		}

		protected override int[] SubclassFormulaTableNumberClosure
		{
			get { return subclassFormulaTableNumberClosure; }
		}

		public override string GetPropertyTableName( string propertyName )
		{
			object index = EntityMetamodel.GetPropertyIndexOrNull( propertyName );
			if( index == null )
			{
				return null;
			}
			return tableNames[ propertyTables[ ( int ) index ] ];
		}

		public override string FilterFragment( string alias )
		{
			return HasWhere ?
			       " and " + GetSQLWhereString( GenerateFilterConditionAlias( alias ) ) :
			       "";
		}

		public override string GenerateFilterConditionAlias( string rootAlias ) 
		{
			return GenerateTableAlias( rootAlias, tableSpan - 1 );
		}

	}
}