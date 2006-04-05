using System;
using System.Collections;
using System.Data;
using System.Text;

using Iesi.Collections;

using log4net;

using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Mapping;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Persister.Entity
{
	/// <summary>
	/// Default implementation of the <c>ClassPersister</c> interface. Implements the
	/// "table-per-class hierarchy" mapping strategy for an entity class.
	/// </summary>
	public class SingleTableEntityPersister : AbstractEntityPersister, IQueryable
	{
		// the class hierarchy structure
		private readonly string qualifiedTableName;
		private readonly string[] tableNames;
		private readonly System.Type[] subclassClosure;

		// SQL strings
		private SqlString sqlDeleteString;
		private SqlString sqlInsertString;
		private SqlString sqlUpdateString;
		private SqlString sqlIdentityInsertString;
		private SqlString sqlConcreteSelectString;
		private SqlString sqlVersionSelectString;

		// discriminator column
		private readonly Hashtable subclassesByDiscriminatorValue = new Hashtable();
		private readonly bool forceDiscriminator;
		private readonly string discriminatorColumnName;
		private readonly string discriminatorAlias;
		private readonly IType discriminatorType;
		private readonly string discriminatorSQLValue;
		private readonly bool discriminatorInsertable;

		private static readonly object NullDiscriminator = new object();
		private static readonly object NotNullDiscriminator = new object();

		private static readonly ILog log = LogManager.GetLogger( typeof( SingleTableEntityPersister ) );

		public override void PostInstantiate()
		{
			base.PostInstantiate();
			InitLockers();

			// initialize the SqlStrings - these are in the PostInstantiate method because we need
			// to have every other IEntityPersister loaded so we can resolve the IType for the 
			// relationships.  In Hibernate they are able to just use ? and not worry about Parameters until
			// the statement is actually called.  We need to worry about Parameters when we are building
			// the IEntityPersister...
			sqlDeleteString = GenerateDeleteString();
			sqlInsertString = GenerateInsertString( false, PropertyInsertability );
			sqlIdentityInsertString = IsIdentifierAssignedByInsert ?
				GenerateInsertString( true, PropertyInsertability ) :
				null;

			sqlUpdateString = GenerateUpdateString( PropertyUpdateability );
			sqlConcreteSelectString = GenerateConcreteSelectString( PropertyUpdateability );
			sqlVersionSelectString = GenerateSelectVersionString();
		}

		public override string DiscriminatorColumnName
		{
			get { return discriminatorColumnName; }
		}

		protected override string DiscriminatorAlias
		{
			get { return discriminatorAlias; }
		}

		public override string TableName
		{
			get { return qualifiedTableName; }
		}

		public override IType DiscriminatorType
		{
			get { return discriminatorType; }
		}

		public override string DiscriminatorSQLValue
		{
			get { return discriminatorSQLValue; }
		}

		public virtual System.Type[] SubclassClosure
		{
			get { return subclassClosure; }
		}

		public override System.Type GetSubclassForDiscriminatorValue( object value )
		{
			if( value == null )
			{
				return ( System.Type ) subclassesByDiscriminatorValue[ NullDiscriminator ];
			}
			else
			{
				System.Type result = ( System.Type ) subclassesByDiscriminatorValue[ value ];
				if( result == null )
				{
					result = ( System.Type ) subclassesByDiscriminatorValue[ NotNullDiscriminator ];
				}
				return result;
			}
		}

		public override object[] PropertySpaces
		{
			get { return tableNames; }
		}

		/// <summary>
		/// The query that deletes a row by id (and version)
		/// </summary>
		protected SqlString SqlDeleteString
		{
			get { return sqlDeleteString; }
		}

		/// <summary>
		/// The query that inserts a row with a given id
		/// </summary>
		protected SqlString SqlInsertString
		{
			get { return sqlInsertString; }
		}

		/// <summary>
		/// The query that inserts a row, letting the database generate an id
		/// </summary>
		protected SqlString SqlIdentityInsertString
		{
			get { return sqlIdentityInsertString; }
		}

		/// <summary>
		/// The query that updates a row by id (and version)
		/// </summary>
		protected SqlString SqlUpdateString
		{
			get { return sqlUpdateString; }
		}

		protected override SqlString VersionSelectString
		{
			get { return sqlVersionSelectString; }
		}

		// Generate all the SQL

		/// <summary>
		/// Generate the SQL that deletes a row by id (and version)
		/// </summary>
		/// <returns>A SqlString for a Delete</returns>
		protected virtual SqlString GenerateDeleteString()
		{
			SqlDeleteBuilder deleteBuilder = new SqlDeleteBuilder( Factory );
			deleteBuilder.SetTableName( TableName )
				.SetIdentityColumn( IdentifierColumnNames, IdentifierType );

			if( IsVersioned )
			{
				deleteBuilder.SetVersionColumn( new string[] {VersionColumnName}, VersionType );
			}

			return deleteBuilder.ToSqlString();

		}

		/// <summary>
		/// Generate the SQL that inserts a row
		/// </summary>
		/// <param name="identityInsert"></param>
		/// <param name="includeProperty"></param>
		/// <returns>A SqlString for an Insert</returns>
		protected virtual SqlString GenerateInsertString( bool identityInsert, bool[] includeProperty )
		{
			SqlInsertBuilder builder = new SqlInsertBuilder( Factory )
				.SetTableName( TableName );

			for( int i = 0; i < HydrateSpan; i++ )
			{
				if( includeProperty[ i ] )
				{
					builder.AddColumn( GetPropertyColumnNames( i ), PropertyTypes[ i ] );
				}
			}

			if( discriminatorInsertable )
			{
				builder.AddColumn( DiscriminatorColumnName, DiscriminatorSQLValue );
			}

			if( !identityInsert )
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
		/// Generate the SQL that selects a row by id using <c>FOR UPDATE</c>
		/// </summary>
		/// <returns></returns>
		protected SqlString GenerateSelectForUpdateString()
		{
			return GenerateSelectString( " for update" );
		}

		/// <summary>
		/// Generate the SQL that selects a row by id using <c>FOR UPDATE NOWAIT</c>
		/// </summary>
		/// <returns></returns>
		protected SqlString GenerateSelectForUpdateNoWaitString()
		{
			return GenerateSelectString( " for update nowait" );
		}

		/// <summary>
		/// Generates an SqlString that selects a row by id
		/// </summary>
		/// <param name="forUpdateFragment">SQL containing <c>FOR UPDATE</c> clauses
		/// to append at the end of the query (optional)</param>
		/// <returns></returns>
		protected virtual SqlString GenerateSelectString( string forUpdateFragment )
		{
			SqlSimpleSelectBuilder builder = new SqlSimpleSelectBuilder( Factory );

			// set the table name and add the columns to select
			builder.SetTableName( TableName )
				.AddColumns( IdentifierColumnNames )
				.AddColumns( SubclassColumnClosure, SubclassColumnAliasClosure )
				.AddColumns( SubclassFormulaClosure, SubclassFormulaAliasClosure );

			if( HasSubclasses )
			{
				builder.AddColumn( DiscriminatorColumnName, DiscriminatorAlias );
			}

			// add the parameters to use in the WHERE clause
			builder.SetIdentityColumn( IdentifierColumnNames, IdentifierType );

			// Ok, render the SELECT statement
			SqlString selectSqlString = builder.ToSqlString();

			// add any special text that is contained in the forUpdateFragment
			if( forUpdateFragment != null && forUpdateFragment.Length > 0 )
			{
				selectSqlString = selectSqlString.Append( forUpdateFragment );
			}

			return selectSqlString;
		}

		/// <summary>
		/// Generate the SQL that selects a row by id, excluding subclasses
		/// </summary>
		/// <param name="includeProperty"></param>
		/// <returns></returns>
		protected virtual SqlString GenerateConcreteSelectString( bool[] includeProperty )
		{
			SqlSimpleSelectBuilder builder = new SqlSimpleSelectBuilder( Factory );

			// set the table and the identity columns
			builder.SetTableName( TableName )
				.AddColumns( IdentifierColumnNames );

			for( int i = 0; i < PropertyNames.Length; i++ )
			{
				if( includeProperty[ i ] )
				{
					builder.AddColumns( GetPropertyColumnNames( i ), GetPropertyColumnAliases( i ) );
				}
			}

			builder.SetIdentityColumn( IdentifierColumnNames, IdentifierType );
			if( IsVersioned )
			{
				builder.SetVersionColumn( new string[] {VersionColumnName}, VersionType );
			}

			return builder.ToSqlString();
		}

		/// <summary>
		/// Generate the SQL that updates a row by id, excluding subclasses
		/// </summary>
		/// <param name="includeProperty"></param>
		/// <returns></returns>
		protected SqlString GenerateUpdateString( bool[] includeProperty )
		{
			SqlUpdateBuilder builder = GenerateUpdate( includeProperty );
			return builder != null ? builder.ToSqlString() : null;
		}

		protected SqlString GenerateUpdateString( bool[] includeProperty, object[] oldFields )
		{
			SqlUpdateBuilder builder = GenerateUpdate( includeProperty );

			if( builder == null )
			{
				return null;
			}

			if( OptimisticLockMode > OptimisticLockMode.Version && oldFields != null )
			{
				bool[] includeInWhere = OptimisticLockMode == OptimisticLockMode.All ?
				PropertyUpdateability :
					includeProperty;

				for( int i = 0; i < HydrateSpan; i++ )
				{
					if( includeInWhere[ i ] )
					{
						if( oldFields[ i ] == null )
						{
							foreach( string column in GetPropertyColumnNames( i ) )
							{
								builder.AddWhereFragment( column + " is null" );
							}
						}
						else
						{
							builder.AddWhereFragment( GetPropertyColumnNames( i ), PropertyTypes[ i ], "=" );
						}
					}
				}
			}

			return builder.ToSqlString();
		}

		protected virtual SqlUpdateBuilder GenerateUpdate( bool[] includeProperty )
		{
			SqlUpdateBuilder builder = new SqlUpdateBuilder( Factory );
			bool hasColumns = false;

			builder.SetTableName( TableName );

			for( int i = 0; i < HydrateSpan; i++ )
			{
				if( includeProperty[ i ] )
				{
					hasColumns = hasColumns || GetPropertyColumnSpan( i ) > 0;
					builder.AddColumns( GetPropertyColumnNames( i ), PropertyTypes[ i ] );
				}
			}

			builder.SetIdentityColumn( IdentifierColumnNames, IdentifierType );
			if( IsVersioned )
			{
				if( OptimisticLockMode == OptimisticLockMode.Version )
				{
					builder.SetVersionColumn( new string[] {VersionColumnName}, VersionType );
					hasColumns = true;
				}
			}

			return hasColumns ? builder : null;
		}

		/// <summary>
		/// Generates the SQL that pessimistically locks a row by id (and version)
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
			SqlStringBuilder sqlBuilder;

			if( sqlString == null )
			{
				SqlSimpleSelectBuilder builder = new SqlSimpleSelectBuilder( Factory );

				// set the table name and add the columns to select
				builder.SetTableName( TableName )
					.AddColumns( IdentifierColumnNames );

				// add the parameters to use in the WHERE clause
				builder.SetIdentityColumn( IdentifierColumnNames, IdentifierType );
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
			if( forUpdateFragment != null )
			{
				sqlBuilder.Add( forUpdateFragment );
			}

			return sqlBuilder.ToSqlString();
		}

		protected override int Dehydrate( object id, object[] fields, bool[] includeProperty, int table, IDbCommand statement, ISessionImplementor session )
		{
			return Dehydrate( id, fields, includeProperty, statement, session);
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
		protected virtual int Dehydrate( object id, object[] fields, bool[] includeProperty, IDbCommand st, ISessionImplementor session )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Dehydrating entity: " + MessageHelper.InfoString( this, id ) );
			}

			int index = 0;

			// there's a pretty strong coupling between the order of the SQL parameter 
			// construction and the actual order of the parameter collection. 

			for( int j = 0; j < HydrateSpan; j++ )
			{
				if( includeProperty[ j ] )
				{
					PropertyTypes[ j ].NullSafeSet( st, fields[ j ], index, session );
					index += GetPropertyColumnSpan( j );
				}
			}

			if( id != null )
			{
				IdentifierType.NullSafeSet( st, id, index, session );
				index += IdentifierColumnNames.Length;
			}

			return index;
		}

		public override object Insert( object[] fields, object obj, ISessionImplementor session )
		{
			if( UseDynamicInsert )
			{
				bool[] notNull = GetNotNullInsertableColumns( fields );
				return Insert( fields, notNull, GenerateInsertString( true, notNull ), obj, session );
			}
			else
			{
				return Insert( fields, PropertyInsertability, SqlIdentityInsertString, obj, session );
			}
		}

		public override void Insert( object id, object[] fields, object obj, ISessionImplementor session )
		{
			if( UseDynamicInsert )
			{
				bool[] notNull = GetNotNullInsertableColumns( fields );
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
		public void Insert( object id, object[] fields, bool[] notNull, SqlString sql, object obj, ISessionImplementor session )
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
				catch( Exception e )
				{
					session.Batcher.AbortBatch( e );
					throw;
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
		public object Insert( object[] fields, bool[] notNull, SqlString sql, object obj, ISessionImplementor session )
		{
			return InsertImpl( fields, notNull, sql, obj, session);
		}

		/// <summary>
		/// Delete an object
		/// </summary>
		/// <param name="id">The id of the object to delete.</param>
		/// <param name="version">The version of the object to delete.</param>
		/// <param name="obj">The object to delete.</param>
		/// <param name="session">The session to perform the deletion in.</param>
		public override void Delete( object id, object version, object obj, ISessionImplementor session )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Deleting entity: " + MessageHelper.InfoString( this, id ) );
				if( IsVersioned )
				{
					log.Debug( "Version: " + version );
				}
			}

			try
			{
				IDbCommand deleteCmd;

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
				catch( Exception e )
				{
					if( !IsVersioned )
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
		/// Update an object
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
			//note: dirtyFields==null means we had no snapshot, and we couldn't get one using select-before-update
			//      oldFields==null just means we had no snapshot to begin with (we might have used select-before-update to get the dirtyFields)
			bool[] propsToUpdate;
			SqlString updateString;

			if( UseDynamicUpdate && dirtyFields != null )
			{
				propsToUpdate = GetPropertiesToUpdate( dirtyFields );
				updateString = GenerateUpdateString( propsToUpdate, oldFields );
				//don't need to check property updatability (dirty checking algorithm handles that)
			}
			else
			{
				propsToUpdate = PropertyUpdateability;
				updateString = SqlUpdateString;
			}

			if( updateString == null )
			{
				// Nothing to update
				return;
			}

			Update( id, fields, oldFields, propsToUpdate, oldVersion, obj, updateString, session );
		}

		protected virtual void Update( object id, object[] fields, object[] oldFields, bool[] includeProperty, object oldVersion, object obj, SqlString sqlUpdateString, ISessionImplementor session )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Updating entity: " + MessageHelper.InfoString( this, id ) );
				if( IsVersioned )
				{
					log.Debug( "Existing version: " + oldVersion + " -> New Version: " + fields[ VersionProperty ] );
				}
			}

			/*
			if( !hasUpdateableColumns )
			{
				return;
			}
			*/

			try
			{
				IDbCommand statement = IsBatchable ?
					session.Batcher.PrepareBatchCommand( sqlUpdateString ) :
					session.Batcher.PrepareCommand( sqlUpdateString );

				try
				{
					// now write the values of fields onto the command

					int index = Dehydrate( id, fields, includeProperty, statement, session );

					// Write any appropriate versioning conditional parameters
					if( IsVersioned && OptimisticLockMode == OptimisticLockMode.Version )
					{
						VersionType.NullSafeSet( statement, oldVersion, index, session );
					}
					else if( OptimisticLockMode.Version < OptimisticLockMode && null != oldFields )
					{
						bool[] includeOldField = OptimisticLockMode == OptimisticLockMode.All ?
						PropertyUpdateability :
							includeProperty;

						for( int j = 0; j < HydrateSpan; j++ )
						{
							if( includeOldField[ j ] && oldFields[ j ] != null )
							{
								PropertyTypes[ j ].NullSafeSet( statement, oldFields[ j ], index, session );
								index += GetPropertyColumnSpan( j );
							}
						}
					}

					if( IsBatchable )
					{
						session.Batcher.AddToBatch( 1 );
					}
					else
					{
						Check( session.Batcher.ExecuteNonQuery( statement ), id );
					}
				}
				catch( Exception e )
				{
					if( IsBatchable )
					{
						session.Batcher.AbortBatch( e );
					}

					throw;
				}
				finally
				{
					if( !IsBatchable )
					{
						session.Batcher.CloseCommand( statement, null );
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

		public SingleTableEntityPersister( PersistentClass model, ISessionFactoryImplementor factory, IMapping mapping )
			: base( model, factory )
		{
			// CLASS + TABLE

			System.Type mappedClass = model.MappedClass;
			Table table = model.RootTable;
			qualifiedTableName = table.GetQualifiedName( Dialect, factory.DefaultSchema );
			tableNames = new string[] {qualifiedTableName};

			// detect mapping errors
			HashedSet distinctColumns = new HashedSet();

			// DISCRIMINATOR
			object discriminatorValue;
			if( model.IsPolymorphic )
			{
				IValue d = model.Discriminator;
				if( d == null )
				{
					throw new MappingException( "discriminator mapping required for polymorphic persistence" );
				}
				forceDiscriminator = model.IsForceDiscriminator;

				// the discriminator will have only one column 
				foreach( Column discColumn in d.ColumnCollection )
				{
					discriminatorColumnName = discColumn.GetQuotedName( Dialect );
					discriminatorAlias = discColumn.GetAlias( Dialect );
				}
				discriminatorType = model.Discriminator.Type;

				if( model.IsDiscriminatorValueNull )
				{
					discriminatorValue = NullDiscriminator;
					discriminatorSQLValue = InFragment.Null;
					discriminatorInsertable = false;
				}
				else if( model.IsDiscriminatorValueNotNull )
				{
					discriminatorValue = NotNullDiscriminator;
					discriminatorSQLValue = InFragment.NotNull;
					discriminatorInsertable = false;
				}
				else
				{
					discriminatorInsertable = model.IsDiscriminatorInsertable;
					try
					{
						IDiscriminatorType dtype = ( IDiscriminatorType ) discriminatorType;
						discriminatorValue = dtype.StringToObject( model.DiscriminatorValue );
						discriminatorSQLValue = dtype.ObjectToSQLString( discriminatorValue );
					}
					catch( InvalidCastException )
					{
						throw new MappingException( string.Format( "Illegal discriminator type: {0}", discriminatorType.Name ) );
					}
					catch( Exception e )
					{
						string msg = String.Format( "Could not format discriminator value '{0}' to sql string using the IType {1}",
							model.DiscriminatorValue,
							model.Discriminator.Type.ToString() );

						throw new MappingException( msg, e );
					}

					if( discriminatorInsertable )
					{
						distinctColumns.Add( discriminatorColumnName );
					}
				}
			}
			else
			{
				forceDiscriminator = false;
				discriminatorInsertable = false;
				discriminatorColumnName = null;
				discriminatorAlias = null;
				discriminatorType = null;
				discriminatorValue = null;
				discriminatorSQLValue = null;
			}

			// PROPERTIES
			HashedSet thisClassProperties = new HashedSet();

			foreach( Mapping.Property prop in model.PropertyClosureCollection )
			{
				thisClassProperties.Add( prop );
			}

			// SQL string generation moved to PostInstantiate

			int subclassSpan = model.SubclassSpan + 1;
			subclassClosure = new System.Type[subclassSpan];
			subclassClosure[ 0 ] = mappedClass;
			if( model.IsPolymorphic )
			{
				subclassesByDiscriminatorValue.Add( discriminatorValue, mappedClass );
			}

			// SUBCLASSES
			if( model.IsPolymorphic )
			{
				int k = 1;
				foreach( Subclass sc in model.SubclassCollection )
				{
					subclassClosure[ k++ ] = sc.MappedClass;
					if( sc.IsDiscriminatorValueNull )
					{
						subclassesByDiscriminatorValue.Add( NullDiscriminator, sc.MappedClass );
					}
					else if( sc.IsDiscriminatorValueNotNull )
					{
						subclassesByDiscriminatorValue.Add( NotNullDiscriminator, sc.MappedClass );
					}
					else
					{
						try
						{
							IDiscriminatorType dtype = discriminatorType as IDiscriminatorType;
							subclassesByDiscriminatorValue.Add(
								dtype.StringToObject( sc.DiscriminatorValue ),
								sc.MappedClass );
						}
						catch( InvalidCastException )
						{
							throw new MappingException( string.Format( "Illegal discriminator type: {0}", discriminatorType.Name ) );
						}
						catch( Exception e )
						{
							throw new MappingException( string.Format( "Error parsing discriminator value: '{0}'", sc.DiscriminatorValue ), e );
						}
					}
				}
			}

			// This is in PostInstatiate as it needs identifier info
			//InitLockers();

			InitSubclassPropertyAliasesMap( model );
			PostConstruct( mapping );
		}

		public override SqlString FromTableFragment( string alias )
		{
			return new SqlString( TableName + ' ' + alias );
		}

		public override SqlString QueryWhereFragment( string name, bool innerJoin, bool includeSubclasses )
		{
			if( innerJoin && NeedsDiscriminator )
			{
				SqlStringBuilder builder = new SqlStringBuilder();
				builder.Add( " and " + DiscriminatorWhereCondition( name ) );

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
					return SqlString.Empty;
				}

			}
		}

		private SqlString DiscriminatorWhereCondition( string alias )
		{
			InFragment frag = new InFragment()
				.SetColumn( alias, DiscriminatorColumnName );
			System.Type[] subclasses = SubclassClosure;
			for( int i = 0; i < subclasses.Length; i++ )
			{
				frag.AddValue(
					( ( IQueryable ) Factory.GetPersister( subclasses[ i ] ) ).DiscriminatorSQLValue
					);
			}

			return frag.ToFragmentString();
		}

		public override string GetSubclassPropertyTableName( int i )
		{
			return qualifiedTableName;
		}

		public override SqlString FromJoinFragment( string alias, bool innerJoin, bool includeSubclasses )
		{
			return SqlString.Empty;
		}

		public override SqlString WhereJoinFragment( string alias, bool innerJoin, bool includeSubclasses )
		{
			return SqlString.Empty;
		}

		protected override SqlString ConcreteSelectString
		{
			get { return sqlConcreteSelectString; }
		}

		/// <summary></summary>
		public override bool IsCacheInvalidationRequired
		{
			get { return HasFormulaProperties || ( !IsVersioned && UseDynamicUpdate ); }
		}

		/// <summary></summary>
		protected override string VersionedTableName
		{
			get { return qualifiedTableName; }
		}

		protected override int GetSubclassPropertyTableNumber( int i )
		{
			return 0;
		}

		protected override void AddDiscriminatorToSelect( SelectFragment select, string name, string suffix )
		{
			// TODO H3:
			//if( IsDiscriminatorFormula ) 
			//{
			//	select.AddFormula( name, DiscriminatorFormulaTemplate, DiscriminatorAlias );
			//}
			//else 
			//{
			select.AddColumn( name, DiscriminatorColumnName, DiscriminatorAlias );
			//}
		}

		protected override int[] SubclassColumnTableNumberClosure
		{
			get { return new int[SubclassColumnClosure.Length]; }
		}

		protected override int[] SubclassFormulaTableNumberClosure
		{
			get { return new int[SubclassFormulaClosure.Length]; }
		}

		public override string GetPropertyTableName( string propertyName )
		{
			return TableName;
		}

		public override string FilterFragment(string alias)
		{
			string result = DiscriminatorFilterFragment( alias );
			if ( HasWhere )
			{
				result += " and " + GetSQLWhereString( alias );
			}
			return result;
		}

		private string DiscriminatorFilterFragment( string alias )
		{
			if ( NeedsDiscriminator ) 
			{
				InFragment frag = new InFragment();

				// TODO H3:
//				if ( IsDiscriminatorFormula ) 
//				{
//					frag.SetFormula( alias, DiscriminatorFormulaTemplate );
//				}
//				else 
//				{
					frag.SetColumn( alias, DiscriminatorColumnName );
//				}

				System.Type[] subclasses = SubclassClosure;
				for ( int i=0; i<subclasses.Length; i++ ) 
				{
					IQueryable queryable = (IQueryable) Factory.GetPersister( subclasses[i] );
					
					// TODO H3:
//					if ( !queryable.IsAbstract )
//					{
						frag.AddValue( queryable.DiscriminatorSQLValue );
//					}
				}

				StringBuilder buf = new StringBuilder(50)
					.Append(" and ")
					.Append( frag.ToFragmentString().ToString() );

				return buf.ToString();
			}
			else 
			{
				return "";
			}
		}

		private bool NeedsDiscriminator
		{
			get { return forceDiscriminator || IsInherited; }
		}

		public override string OneToManyFilterFragment( string alias )
		{
			return forceDiscriminator ?
				DiscriminatorFilterFragment( alias ) :
				string.Empty;
		}

	}
}