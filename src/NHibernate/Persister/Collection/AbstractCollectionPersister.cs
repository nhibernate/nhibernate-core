using System;
using System.Collections;
using System.Data;

using Iesi.Collections;

using log4net;

using NHibernate.Cache;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Id;
using NHibernate.Impl;
using NHibernate.Loader;
using NHibernate.Mapping;
using NHibernate.Metadata;
using NHibernate.Persister;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

using Array = NHibernate.Mapping.Array;

namespace NHibernate.Persister.Collection
{
	/// <summary>
	/// Summary description for AbstractCollectionPersister.
	/// </summary>
	public abstract class AbstractCollectionPersister : ICollectionMetadata, IQueryableCollection
	{
		// Added this, because SQL builders require it
		protected readonly ISessionFactoryImplementor factory;

		private readonly SqlString sqlDeleteString;
		private readonly SqlString sqlInsertRowString;
		private readonly SqlString sqlUpdateRowString;
		private readonly SqlString sqlDeleteRowString;
		private readonly string sqlOrderByString;
		protected readonly string sqlWhereString;
		private readonly string sqlOrderByStringTemplate;
		private readonly string sqlWhereStringTemplate;
		private readonly bool hasOrder;
		private readonly bool hasWhere;
		private readonly bool hasOrphanDelete;
		private readonly IType keyType;
		private readonly IType indexType;
		private readonly IType elementType;
		private readonly string[ ] keyColumnNames;
		private readonly string[ ] indexColumnNames;
		private readonly string[ ] elementColumnNames;
		protected readonly string[ ] rowSelectColumnNames;
		private readonly string[ ] indexColumnAliases;
		private readonly string[ ] elementColumnAliases;
		private readonly string[ ] keyColumnAliases;
		protected readonly IType rowSelectType;
		private readonly bool primitiveArray;
		private readonly bool array;
		//private readonly bool isOneToMany;
		protected readonly string qualifiedTableName;
		private readonly bool hasIndex;
		private readonly bool isLazy;
		private readonly bool isInverse;
		protected readonly int batchSize;
		private readonly System.Type elementClass;
		private readonly ICacheConcurrencyStrategy cache;
		private readonly PersistentCollectionType collectionType;
		private readonly FetchMode fetchMode;
		private readonly System.Type ownerClass;
		//private readonly bool isSorted;
		private readonly IIdentifierGenerator identifierGenerator;
		private readonly string unquotedIdentifierColumnName;
		private readonly IType identifierType;
		protected readonly bool hasIdentifier;
		protected readonly string identifierColumnName;
		private readonly string identifierColumnAlias;
		private readonly Dialect.Dialect dialect;
		//private readonly SQLExceptionConverter sqlExceptionConverter;
		private readonly IPropertyMapping elementPropertyMapping;
		private readonly IClassPersister elementPersister;

		private readonly ICollectionInitializer initializer;

		private readonly string role;

		private static readonly ILog log = LogManager.GetLogger( typeof( ICollectionPersister ) );

		public AbstractCollectionPersister( Mapping.Collection collection, ISessionFactoryImplementor factory )
		{
			this.factory = factory;
			dialect = factory.Dialect;
			//sqlExceptionConverter = factory.SQLExceptionConverter;
			collectionType = collection.CollectionType;
			role = collection.Role;
			ownerClass = collection.OwnerClass;
			Alias alias = new Alias( "__" );

			sqlOrderByString = collection.OrderBy;
			hasOrder = sqlOrderByString != null;
			sqlOrderByStringTemplate = hasOrder ? Template.RenderOrderByStringTemplate( sqlOrderByString, dialect ) : null;

			sqlWhereString = collection.Where;
			hasWhere = sqlWhereString != null;
			sqlWhereStringTemplate = hasWhere ? Template.RenderWhereStringTemplate( sqlWhereString, dialect ) : null;

			hasOrphanDelete = collection.OrphanDelete;

			batchSize = collection.BatchSize;

			cache = collection.Cache;

			keyType = collection.Key.Type;
			int keySpan = collection.Key.ColumnSpan;
			keyColumnNames = new string[keySpan];
			string[ ] keyAliases = new string[keySpan];
			int k = 0;
			foreach( Column col in collection.Key.ColumnCollection )
			{
				keyColumnNames[ k ] = col.GetQuotedName( dialect );
				keyAliases[ k ] = col.Alias( dialect );
				k++;
			}
			keyColumnAliases = alias.ToAliasStrings( keyAliases, dialect );
			//unquotedKeyColumnNames = StringHelper.Unquote( keyColumnAliases );
			ISet distinctColumns = new HashedSet();
			CheckColumnDuplication( distinctColumns, collection.Key.ColumnCollection );

			//isSet = collection.IsSet;
			//isSorted = collection.IsSorted;
			primitiveArray = collection.IsPrimitiveArray;
			array = collection.IsArray;

			IValue element = collection.Element;
			int elementSpan = element.ColumnSpan;
			ICollection iter = element.ColumnCollection;
			Table table = collection.CollectionTable;
			fetchMode = element.FetchMode;
			elementType = element.Type;

			if( !collection.IsOneToMany )
			{
				CheckColumnDuplication( distinctColumns, element.ColumnCollection );
			}

			if( elementType.IsEntityType )
			{
				elementPersister = factory.GetPersister( ( ( EntityType ) elementType ).AssociatedClass );
			}
			else
			{
				elementPersister = null;
			}

			qualifiedTableName = table.GetQualifiedName( dialect, factory.DefaultSchema );
			string[ ] aliases = new string[elementSpan];
			elementColumnNames = new string[elementSpan];
			int j = 0;
			foreach( Column col in iter )
			{
				elementColumnNames[ j ] = col.GetQuotedName( dialect );
				aliases[ j ] = col.Alias( dialect );
				j++;
			}

			elementColumnAliases = alias.ToAliasStrings( aliases, dialect );

			IType selectColumns;
			string[ ] selectType;

			hasIndex = collection.IsIndexed;

			if( hasIndex )
			{
				IndexedCollection indexedCollection = ( IndexedCollection ) collection;

				indexType = indexedCollection.Index.Type;
				int indexSpan = indexedCollection.Index.ColumnSpan;
				indexColumnNames = new string[indexSpan];

				string[ ] indexAliases = new string[indexSpan];
				int i = 0;
				foreach( Column indexCol in indexedCollection.Index.ColumnCollection )
				{
					indexAliases[ i ] = indexCol.Alias( dialect );
					indexColumnNames[ i ] = indexCol.GetQuotedName( dialect );
					i++;
				}
				selectType = indexColumnNames;
				selectColumns = indexType;
				indexColumnAliases = alias.ToAliasStrings( indexAliases, dialect );
				CheckColumnDuplication( distinctColumns, indexedCollection.Index.ColumnCollection );
			}
			else
			{
				indexType = null;
				indexColumnNames = null;
				indexColumnAliases = null;
				selectType = elementColumnNames;
				selectColumns = elementType;
			}

			hasIdentifier = collection.IsIdentified;

			if( hasIdentifier )
			{
				if( collection.IsOneToMany )
				{
					throw new MappingException( "one-to-many collections with identifiers are not supported." );
				}
				IdentifierCollection idColl = ( IdentifierCollection ) collection;
				identifierType = idColl.Identifier.Type;

				Column col = null;
				foreach( Column column in idColl.Identifier.ColumnCollection )
				{
					col = column;
					break;
				}

				identifierColumnName = col.GetQuotedName( dialect );
				selectType = new string[ ] {identifierColumnName};
				selectColumns = identifierType;
				identifierColumnAlias = alias.ToAliasString( col.Alias( dialect ), dialect );
				unquotedIdentifierColumnName = identifierColumnAlias;
				identifierGenerator = idColl.Identifier.CreateIdentifierGenerator( dialect );
				CheckColumnDuplication( distinctColumns, idColl.Identifier.ColumnCollection );
			}
			else
			{
				identifierType = null;
				identifierColumnName = null;
				identifierColumnAlias = null;
				unquotedIdentifierColumnName = null;
				identifierGenerator = null;
			}

			rowSelectColumnNames = selectType;
			rowSelectType = selectColumns;

			sqlDeleteString = GenerateDeleteString();
			sqlInsertRowString = GenerateInsertRowString();
			sqlUpdateRowString = GenerateUpdateRowString();
			sqlDeleteRowString = GenerateDeleteRowString();
			isLazy = collection.IsLazy;

			isInverse = collection.IsInverse;

			if( collection.IsArray )
			{
				elementClass = ( ( Array ) collection ).ElementClass;
			}
			else
			{
				// for non-arrays, we don't need to know the element class
				elementClass = null;
			}

			initializer = CreateCollectionInitializer( factory );

			if( elementType.IsComponentType )
			{
				elementPropertyMapping = new CompositeElementPropertyMapping( elementColumnNames, ( IAbstractComponentType ) elementType, factory );
			}
			else if( !elementType.IsEntityType )
			{
				elementPropertyMapping = new ElementPropertyMapping( elementColumnNames, elementType );
			}
			else
			{
				IClassPersister persister = factory.GetPersister( ( ( EntityType ) elementType ).AssociatedClass );
				// Not all classpersisters implement IPropertyMapping!
				if( persister is IPropertyMapping )
				{
					elementPropertyMapping = ( IPropertyMapping ) persister;
				}
				else
				{
					elementPropertyMapping = new ElementPropertyMapping( elementColumnNames, elementType );
				}
			}
		}

		public void Initialize( object key, ISessionImplementor session )
		{
			try
			{
				initializer.Initialize( key, session );
			}
			catch( HibernateException )
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch( Exception sqle )
			{
				throw Convert( sqle, "could not initialize collection: " + MessageHelper.InfoString( this, key ) );
			}
		}

		protected abstract ICollectionInitializer CreateCollectionInitializer( ISessionFactoryImplementor factory );

		public ICacheConcurrencyStrategy Cache
		{
			get { return cache; }
		}

		public bool HasCache
		{
			get { return cache != null; }
		}

		public PersistentCollectionType CollectionType
		{
			get { return this.collectionType; }
		}

		public string GetSQLWhereString( string alias )
		{
			return StringHelper.Replace( sqlWhereStringTemplate, Template.Placeholder, alias );
		}

		public string GetSQLOrderByString( string alias )
		{
			return StringHelper.Replace( sqlOrderByStringTemplate, Template.Placeholder, alias );
		}

		public FetchMode FetchMode
		{
			get { return fetchMode; }
		}

		public bool HasOrdering
		{
			get { return hasOrder; }
		}

		public bool HasWhere
		{
			get { return hasWhere; }
		}

		protected SqlString SqlDeleteString
		{
			get { return sqlDeleteString; }
		}

		protected SqlString SqlInsertRowString
		{
			get { return sqlInsertRowString; }
		}

		protected SqlString SqlUpdateRowString
		{
			get { return sqlUpdateRowString; }
		}

		protected SqlString SqlDeleteRowString
		{
			get { return sqlDeleteRowString; }
		}

		public IType KeyType
		{
			get { return keyType; }
		}

		public IType IndexType
		{
			get { return indexType; }
		}

		public IType ElementType
		{
			get { return elementType; }
		}

		/// <summary>
		/// Return the element class of an array, or null otherwise
		/// </summary>
		public System.Type ElementClass
		{
			// needed by arrays
			get { return elementClass; }
		}

		/// <summary>
		/// Reads the Element from the IDataReader.  The IDataReader will probably only contain
		/// the id of the Element.
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="owner"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		/// <remarks>See ReadElementIdentifier for an explanation of why this method will be depreciated.</remarks>
		public object ReadElement( IDataReader rs, object owner, ISessionImplementor session )
		{
			object element = ElementType.NullSafeGet( rs, elementColumnAliases, session, owner );
			return element;
		}

		public object ReadIndex( IDataReader rs, ISessionImplementor session )
		{
			object index = IndexType.NullSafeGet( rs, indexColumnAliases, session, null );
			if( index == null )
			{
				throw new HibernateException( "null index column for collection: " + role );
			}
			return index;
		}

		public object ReadIdentifier( IDataReader rs, ISessionImplementor session )
		{
			object id = IdentifierType.NullSafeGet( rs, unquotedIdentifierColumnName, session, null );
			if( id == null )
			{
				throw new HibernateException( "null identifier column for collection: " + role );
			}
			return id;
		}

		public object ReadKey( IDataReader dr, ISessionImplementor session )
		{
			return KeyType.NullSafeGet( dr, keyColumnAliases, session, null );
		}

		public void WriteElement( IDbCommand st, object elt, bool writeOrder, ISessionImplementor session )
		{
			ElementType.NullSafeSet(
				st,
				elt,
				( writeOrder ? 0 : keyColumnNames.Length + ( hasIndex ? indexColumnNames.Length : 0 ) + ( hasIdentifier ? 1 : 0 ) ),
				session );
		}

		public void WriteIndex( IDbCommand st, object idx, bool writeOrder, ISessionImplementor session )
		{
			IndexType.NullSafeSet( st, idx, keyColumnNames.Length + ( writeOrder ? elementColumnNames.Length : 0 ), session );
		}

		public void WriteIdentifier( IDbCommand st, object idx, bool writeOrder, ISessionImplementor session )
		{
			IdentifierType.NullSafeSet( st, idx, ( writeOrder ? elementColumnNames.Length : keyColumnNames.Length ), session );
		}

		private void WriteRowSelect( IDbCommand st, object idx, ISessionImplementor session )
		{
			rowSelectType.NullSafeSet( st, idx, ( hasIdentifier ? 0 : keyColumnNames.Length ), session );
		}

		public void WriteKey( IDbCommand st, object id, bool writeOrder, ISessionImplementor session )
		{
			if( id == null )
			{
				throw new NullReferenceException( "Null key for collection: " + role );
			}
			KeyType.NullSafeSet( st, id, ( writeOrder ? elementColumnNames.Length : 0 ), session );
		}

		public bool IsPrimitiveArray
		{
			get { return primitiveArray; }
		}

		public bool IsArray
		{
			get { return array; }
		}

		public SqlString SelectFragment( string alias )
		{
			SelectFragment frag = new SelectFragment( dialect )
				.SetSuffix( String.Empty ) //always ignore suffix for collection columns
				.AddColumns( alias, keyColumnNames, keyColumnAliases )
				.AddColumns( alias, elementColumnNames, elementColumnAliases );
			if( hasIndex )
			{
				frag.AddColumns( alias, indexColumnNames, indexColumnAliases );
			}
			if( hasIdentifier )
			{
				frag.AddColumn( alias, identifierColumnName, identifierColumnAlias );
			}
			return frag.ToSqlStringFragment( false );
		}

		public string[ ] IndexColumnNames
		{
			get { return indexColumnNames; }
		}

		public string[ ] ElementColumnNames
		{
			get { return elementColumnNames; }
		}

		public string[ ] KeyColumnNames
		{
			get { return keyColumnNames; }
		}

		public bool HasIndex
		{
			get { return hasIndex; }
		}

		public bool IsLazy
		{
			get { return isLazy; }
		}

		public bool IsInverse
		{
			get { return isInverse; }
		}

		public string TableName
		{
			get { return qualifiedTableName; }
		}

		public void Remove( object id, ISessionImplementor session )
		{
			if( !isInverse )
			{
				if( log.IsDebugEnabled )
				{
					log.Debug( "Deleting collection: " + MessageHelper.InfoString( this, id ) );
				}

				// Remove all the old entries
				try
				{
					IDbCommand st = session.Batcher.PrepareBatchCommand( SqlDeleteString );

					try
					{
						WriteKey( st, id, false, session );
						session.Batcher.AddToBatch( -1 );
					}
					catch( Exception e )
					{
						session.Batcher.AbortBatch( e );
						throw;
					}

					if( log.IsDebugEnabled )
					{
						log.Debug( "done deleting collection" );
					}
				}
				catch( HibernateException )
				{
					// Do not call Convert on HibernateExceptions
					throw;
				}
				catch( Exception sqle )
				{
					throw Convert( sqle, "could not delete collection: " + MessageHelper.InfoString( this, id ) );
				}
			}
		}

		public void Recreate( IPersistentCollection collection, object id, ISessionImplementor session )
		{
			if( !isInverse )
			{
				if( log.IsDebugEnabled )
				{
					log.Debug( "Inserting collection: " + MessageHelper.InfoString( this, id ) );
				}

				try
				{
					// create all the new entries
					IEnumerable entries = collection.Entries();
					
					//if( entries.Count > 0 )
					//{
						int i = 0;
						int count = 0;
						try
						{
							collection.PreInsert( this );

							foreach( object entry in entries )
							{
								if( collection.EntryExists( entry, i ) )
								{
									IDbCommand st = session.Batcher.PrepareBatchCommand( SqlInsertRowString );
									WriteKey( st, id, false, session );
									collection.WriteTo( st, this, entry, i, false );
									session.Batcher.AddToBatch( 1 );
									collection.AfterRowInsert( this, entry, i );
									count++;
								}
								i++;
							}
						} 
							//TODO: change to SqlException
						catch( Exception e )
						{
							session.Batcher.AbortBatch( e );
							throw;
						}
						if( log.IsDebugEnabled )
						{
							log.Debug( string.Format( "done inserting collection: {0} rows inserted", count ) );
						}
					//}
					//else
					//{
						if( count==0 && log.IsDebugEnabled )
						{
							log.Debug( "collection was empty" );
						}
					//}
				}
				catch( HibernateException )
				{
					// Do not call Convert on HibernateExceptions
					throw;
				}
				catch( Exception sqle )
				{
					throw Convert( sqle, "could not insert collection: " + MessageHelper.InfoString( this, id ) );
				}
			}
		}

		public void DeleteRows( IPersistentCollection collection, object id, ISessionImplementor session )
		{
			if( !isInverse )
			{
				if( log.IsDebugEnabled )
				{
					log.Debug( "Deleting rows of collection: " + MessageHelper.InfoString( this, id ) );
				}

				try
				{
					// delete all the deleted entries
					ICollection entries = collection.GetDeletes( elementType );
					if( entries.Count > 0 )
					{
						IDbCommand st = session.Batcher.PrepareBatchCommand( SqlDeleteRowString );
						try
						{
							foreach( object entry in entries )
							{
								if( !hasIdentifier )
								{
									WriteKey( st, id, false, session );
								}
								WriteRowSelect( st, entry, session );
								session.Batcher.AddToBatch( -1 );
							}
						} 
							// TODO: change to SqlException
						catch( Exception e )
						{
							session.Batcher.AbortBatch( e );
							throw;
						}

						if( log.IsDebugEnabled )
						{
							log.Debug( "done deleting collection rows" );
						}
					}
					else
					{
						if( log.IsDebugEnabled )
						{
							log.Debug( "no rows to delete" );
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
					throw Convert( sqle, "could not delete collection rows: " + MessageHelper.InfoString( this, id ) );
				}
			}
		}

		public void InsertRows( IPersistentCollection collection, object id, ISessionImplementor session )
		{
			if( !isInverse )
			{
				if( log.IsDebugEnabled )
				{
					log.Debug( "Inserting rows of collection: " + role + "#" + id );
				}

				collection.PreInsert( this );
				int i = 0;
				int count = 0;

				try
				{
					// insert all the new entries
					IEnumerable entries = collection.Entries();
					try
					{
						// Moved the IDbCommand outside the loop, because ADO.NET doesn't do batch commands,
						// so it's more efficient. But it
						foreach( object entry in entries )
						{
							if( collection.NeedsInserting( entry, i, elementType ) )
							{
								IDbCommand st = null;
								if( st == null )
								{
									st = session.Batcher.PrepareBatchCommand( SqlInsertRowString );
								}
								WriteKey( st, id, false, session );
								collection.WriteTo( st, this, entry, i, false );
								session.Batcher.AddToBatch( 1 );
								collection.AfterRowInsert( this, entry, i );
								count++;
							}
							i++;
						}

						if( log.IsDebugEnabled )
						{
							log.Debug( string.Format( "done inserting rows: {0} inserted", count ) );
						}
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
					throw Convert( sqle, "could not insert collection rows: " + MessageHelper.InfoString( this, id ) );
				}

			}
		}

		/// <summary>
		/// Get the name of this collection role (the fully qualified class name,
		/// extended by a "property path")
		/// </summary>
		public string Role
		{
			get { return role; }
		}

		public System.Type OwnerClass
		{
			get { return ownerClass; }
		}

		public IIdentifierGenerator IdentifierGenerator
		{
			get { return identifierGenerator; }
		}

		public IType IdentifierType
		{
			get { return identifierType; }
		}

		public bool HasOrphanDelete
		{
			get { return hasOrphanDelete; }
		}

		private void CheckColumnDuplication( ISet distinctColumns, ICollection columns )
		{
			foreach( Column col in columns )
			{
				if( distinctColumns.Contains( col.Name ) )
				{
					throw new MappingException(
						"Repeated column in mapping for collection: " +
							role +
							" column: " +
							col.Name
						);
				}
				else
				{
					distinctColumns.Add( col.Name );
				}
			}
		}

		public IType ToType( string propertyName )
		{
			if( "index".Equals( propertyName ) )
			{
				return indexType;
			}
			return elementPropertyMapping.ToType( propertyName );
		}

		public string[ ] ToColumns( string alias, string propertyName )
		{
			if( "index".Equals( propertyName ) )
			{
				if( IsManyToMany )
				{
					throw new QueryException( "index() function not supported for many-to-many association" );
				}
				return StringHelper.Qualify( alias, indexColumnNames );
			}
			return elementPropertyMapping.ToColumns( alias, propertyName );
		}

		public IType Type
		{
			get { return elementPropertyMapping.Type; }
		}

		public string Name
		{
			get { return Role; }
		}

		public IClassPersister ElementPersister
		{
			get
			{
				if( elementPersister == null )
				{
					throw new AssertionFailure( "Not an association" );
				}

				return ( ILoadable ) elementPersister;
			}
		}

		public bool IsCollection
		{
			get { return true; }
		}

		public object CollectionSpace
		{
			get { return TableName; }
		}

		protected abstract SqlString GenerateDeleteString();
		protected abstract SqlString GenerateDeleteRowString();
		protected abstract SqlString GenerateUpdateRowString();
		protected abstract SqlString GenerateInsertRowString();

		public void UpdateRows( IPersistentCollection collection, object id, ISessionImplementor session )
		{
			if( !isInverse )
			{
				if( log.IsDebugEnabled )
				{
					log.Debug( string.Format( "Updating rows of collection: {0}#{1}", role, id ) );
				}

				// update all the modified entries
				int count = DoUpdateRows( id, collection, session );

				if( log.IsDebugEnabled )
				{
					log.Debug( string.Format( "done updating rows: {0} updated", count ) );
				}
			}
		}

		protected abstract int DoUpdateRows( object key, IPersistentCollection collection, ISessionImplementor session );

		public ICollectionMetadata CollectionMetadata
		{
			get { return this; }
		}

		protected ADOException Convert( Exception sqlException, string message )
		{
			return ADOExceptionHelper.Convert( sqlException, message );
		}

		public override string ToString()
		{
			// Java has StringHelper.root( getClass().getName() ) instead of GetType().Name,
			// but it doesn't make sense to me.
			return GetType().Name + '(' + role + ')';
		}

		/// <summary>
		/// Gets just the Identifier of the Element for the Collection.
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="owner"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		/// <remarks>
		/// This was created in addition to ReadElement because ADO.NET does not allow
		/// for 2 IDataReaders to be open against a single IDbConnection at one time.  
		/// 
		/// When a Collection is loaded it was recursively opening IDbDataReaders to resolve
		/// the Element for the Collection while the IDbDataReader was open that contained the
		/// record for the Collection.
		/// </remarks>		
		public object ReadElementIdentifier( IDataReader rs, object owner, ISessionImplementor session )
		{
			return ElementType.Hydrate( rs, elementColumnAliases, session, owner );
		}

		public abstract SqlString SelectFragment( string alias, string suffix, bool includeCollectionColumns );
		public abstract SqlString FromJoinFragment( string alias, bool innerJoin, bool includeSubclasses );
		public abstract SqlString WhereJoinFragment( string alias, bool innerJoin, bool includeSubclasses );

		public abstract bool ConsumesAlias();
		public abstract bool IsManyToMany { get; }
		public abstract bool IsOneToMany { get; }
	}
}