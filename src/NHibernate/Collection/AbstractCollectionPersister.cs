using System;
using System.Collections;
using System.Data;
using Iesi.Collections;
using log4net;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Impl;
using NHibernate.Loader;
using NHibernate.Mapping;
using NHibernate.Metadata;
using NHibernate.Persister;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;
using Array = NHibernate.Mapping.Array;

namespace NHibernate.Collection
{
	/// <summary>
	/// Summary description for AbstractCollectionPersister.
	/// </summary>
	public abstract class AbstractCollectionPersister : ICollectionMetadata, IQueryableCollection
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( ICollectionPersister ) );

		private readonly SqlString sqlDeleteString;
		private readonly SqlString sqlInsertRowString;
		private readonly SqlString sqlUpdateRowString;
		private readonly SqlString sqlDeleteRowString;
		private readonly string sqlOrderByString;
		private readonly string sqlWhereString;
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
		private readonly string[ ] rowSelectColumnNames;
		private readonly string[ ] indexColumnAliases;
		private readonly string[ ] elementColumnAliases;
		private readonly string[ ] keyColumnAliases;
		private readonly IType rowSelectType;
		private readonly bool primitiveArray;
		private readonly bool array;
		//private readonly bool isOneToMany;
		private readonly string qualifiedTableName;
		private readonly bool hasIndex;
		private readonly bool isLazy;
		private readonly bool isInverse;
		private readonly int batchSize;
		private readonly System.Type elementClass;
		private readonly ICacheConcurrencyStrategy cache;
		private readonly PersistentCollectionType collectionType;
		private readonly OuterJoinFetchStrategy enableJoinedFetch;
		private readonly System.Type ownerClass;
		//private readonly bool isSorted;
		private readonly IIdentifierGenerator identifierGenerator;
		private readonly string unquotedIdentifierColumnName;
		private readonly IType identifierType;
		private readonly bool hasIdentifier;
		private readonly string identifierColumnName;
		private readonly string identifierColumnAlias;
		private readonly Dialect.Dialect dialect;
		private readonly IPropertyMapping elementPropertyMapping;
		private readonly IClassPersister elementPersister;

		private readonly ICollectionInitializer initializer;

		private readonly string role;

		/// <summary></summary>
		protected readonly ISessionFactoryImplementor factory;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="datastore"></param>
		/// <param name="factory"></param>
		public AbstractCollectionPersister( Mapping.Collection collection, Configuration datastore, ISessionFactoryImplementor factory )
		{
			this.factory = factory;
			this.dialect = factory.Dialect;
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
			keyColumnNames = new string[ keySpan ];
			string[ ] keyAliases = new string[ keySpan ];
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
			enableJoinedFetch = element.OuterJoinFetchSetting;
			elementType = element.Type;

			if( !collection.IsOneToMany )
			{
				CheckColumnDuplication( distinctColumns, element.ColumnCollection ) ;
			}
			
			if ( elementType.IsEntityType )
			{
				elementPersister = factory.GetPersister( ( (EntityType) elementType).AssociatedClass ) ;
			}
			else
			{
				elementPersister = null;
			}

			qualifiedTableName = table.GetQualifiedName( dialect, factory.DefaultSchema );
			string[ ] aliases = new string[ elementSpan ];
			elementColumnNames = new string[ elementSpan ];
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

			if ( elementType.IsComponentType )
			{
				elementPropertyMapping = new CompositeElementPropertyMapping( elementColumnNames, (IAbstractComponentType) elementType, factory );
			}
			else if ( !elementType.IsEntityType )
			{
				elementPropertyMapping = new ElementPropertyMapping( elementColumnNames, elementType );
			}
			else
			{
				IClassPersister persister = factory.GetPersister( ( (EntityType) elementType).AssociatedClass );
				// Not all classpersisters implement IPropertyMapping!
				if ( persister is IPropertyMapping )
				{
					elementPropertyMapping = (IPropertyMapping) persister;
				}
				else
				{
					elementPropertyMapping = new ElementPropertyMapping( elementColumnNames, elementType );
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		protected abstract ICollectionInitializer CreateCollectionInitializer( ISessionFactoryImplementor factory );

		/// <summary>
		/// 
		/// </summary>
		public int BatchSize
		{
			get { return batchSize; }
		}

		/// <summary>
		/// 
		/// </summary>
		public ICollectionInitializer Initializer
		{
			get { return initializer; }
		}

		/// <summary>
		/// 
		/// </summary>
		public ICacheConcurrencyStrategy Cache
		{
			get { return cache; }
		}

		/// <summary>
		/// 
		/// </summary>
		public bool HasCache
		{
			get { return cache != null; }
		}

		/// <summary>
		/// 
		/// </summary>
		public abstract bool IsManyToMany { get; }

		/*
		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		public void Softlock( object id )
		{
			if( cache != null )
			{
				cache.Lock( id );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		public void ReleaseSoftlock( object id )
		{
			if( cache != null )
			{
				cache.Release( id );
			}
		}
		*/

		/// <summary>
		/// 
		/// </summary>
		public ICollectionMetadata CollectionMetadata
		{
			get { return this; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string[] JoinKeyColumns
		{
			get { return KeyColumnNames; }
		}

		/// <summary>
		/// 
		/// </summary>
		public PersistentCollectionType CollectionType
		{
			get { return this.collectionType; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		public string GetSQLWhereString( string alias )
		{
			if( hasWhere )
			{
				return StringHelper.Replace( sqlWhereStringTemplate, Template.Placeholder, alias );
			}
			else
			{
				return null;
			}

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		public string GetSQLOrderByString( string alias )
		{
			if( sqlOrderByStringTemplate != null )
			{
				return StringHelper.Replace( sqlOrderByStringTemplate, Template.Placeholder, alias );
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public OuterJoinFetchStrategy EnableJoinFetch
		{
			get { return enableJoinedFetch; }
		}

		/// <summary>
		/// 
		/// </summary>
		public bool HasOrdering
		{
			get { return hasOrder; }
		}

		/// <summary>
		/// 
		/// </summary>
		public bool HasWhere
		{
			get { return hasWhere; }
		}

		/// <summary>
		/// 
		/// </summary>
		protected string Where
		{
			get { return sqlWhereString; }
		}

		/// <summary>
		/// 
		/// </summary>
		public SqlString SqlDeleteString
		{
			get { return sqlDeleteString; }
		}

		/// <summary>
		/// 
		/// </summary>
		public SqlString SqlInsertRowString
		{
			get { return sqlInsertRowString; }
		}

		/// <summary>
		/// 
		/// </summary>
		public SqlString SqlUpdateRowString
		{
			get { return sqlUpdateRowString; }
		}

		/// <summary>
		/// 
		/// </summary>
		public SqlString SqlDeleteRowString
		{
			get { return sqlDeleteRowString; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string Name
		{
			get { return Role; }
		}

		/// <summary>
		/// 
		/// </summary>
		public IClassPersister ElementPersister
		{
			get 
			{
				if ( elementPersister == null )
				{
					throw new AssertionFailure( "Not an association" );
				}

				return (ILoadable) elementPersister;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IsCollection
		{
			get { return true; }
		}

		/// <summary></summary>
		public IType KeyType
		{
			get { return keyType; }
		}

		/// <summary></summary>
		public IType IndexType
		{
			get { return indexType; }
		}

		/// <summary></summary>
		public IType ElementType
		{
			get { return elementType; }
		}

		/// <summary></summary>
		public System.Type ElementClass
		{
			// needed by arrays
			get { return elementClass; }
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public object ReadIndex( IDataReader rs, ISessionImplementor session )
		{
			object index = IndexType.NullSafeGet( rs, indexColumnAliases, session, null );
			if( index == null )
			{
				throw new HibernateException( "null index column for collection: " + role );
			}
			return index;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public object ReadIdentifier( IDataReader rs, ISessionImplementor session )
		{
			object id = IdentifierType.NullSafeGet( rs, unquotedIdentifierColumnName, session, null );
			if( id == null )
			{
				throw new HibernateException( "null identifier column for collection: " + role );
			}
			return id;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public object ReadKey( IDataReader dr, ISessionImplementor session )
		{
			return KeyType.NullSafeGet( dr, keyColumnAliases, session, null );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="st"></param>
		/// <param name="elt"></param>
		/// <param name="writeOrder"></param>
		/// <param name="session"></param>
		public void WriteElement( IDbCommand st, object elt, bool writeOrder, ISessionImplementor session )
		{
			ElementType.NullSafeSet( st, elt, ( writeOrder ? 0 : keyColumnNames.Length + ( hasIndex ? indexColumnNames.Length : 0 ) + ( hasIdentifier ? 1 : 0 ) ), session );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="st"></param>
		/// <param name="idx"></param>
		/// <param name="writeOrder"></param>
		/// <param name="session"></param>
		public void WriteIndex( IDbCommand st, object idx, bool writeOrder, ISessionImplementor session )
		{
			IndexType.NullSafeSet( st, idx, keyColumnNames.Length + ( writeOrder ? elementColumnNames.Length : 0 ), session );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="st"></param>
		/// <param name="idx"></param>
		/// <param name="writeOrder"></param>
		/// <param name="session"></param>
		public void WriteIdentifier( IDbCommand st, object idx, bool writeOrder, ISessionImplementor session )
		{
			IdentifierType.NullSafeSet( st, idx, ( writeOrder ? elementColumnNames.Length : keyColumnNames.Length ), session );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="st"></param>
		/// <param name="idx"></param>
		/// <param name="session"></param>
		private void WriteRowSelect( IDbCommand st, object idx, ISessionImplementor session )
		{
			rowSelectType.NullSafeSet( st, idx, ( HasIdentifier ? 0 : keyColumnNames.Length ), session );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="st"></param>
		/// <param name="id"></param>
		/// <param name="writeOrder"></param>
		/// <param name="session"></param>
		public void WriteKey( IDbCommand st, object id, bool writeOrder, ISessionImplementor session )
		{
			if( id == null )
			{
				throw new NullReferenceException( "Null key for collection: " + role );
			}
			KeyType.NullSafeSet( st, id, ( writeOrder ? elementColumnNames.Length : 0 ), session );
		}

		/// <summary></summary>
		public bool IsPrimitiveArray
		{
			get { return primitiveArray; }
		}

		/// <summary></summary>
		public bool IsArray
		{
			get { return array; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected abstract SqlString GenerateDeleteString();

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected abstract SqlString GenerateDeleteRowString();

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected abstract SqlString GenerateUpdateRowString();

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected abstract SqlString GenerateInsertRowString();

		/// <summary></summary>
		public string[ ] IndexColumnNames
		{
			get { return indexColumnNames; }
		}

		/// <summary></summary>
		public string[] RowSelectColumnNames
		{
			get { return rowSelectColumnNames; }
		}

		/// <summary></summary>
		public IType RowSelectType
		{
			get { return rowSelectType; }
		}

		/// <summary></summary>
		public string[ ] ElementColumnNames
		{
			get { return elementColumnNames; }
		}

		/// <summary></summary>
		public string[ ] KeyColumnNames
		{
			get { return keyColumnNames; }
		}

		/// <summary></summary>
		public OuterJoinFetchStrategy EnableJoinedFetch
		{
			get { return enableJoinedFetch; }
		}

		/// <summary></summary>
		public abstract bool IsOneToMany { get; }

		/// <summary></summary>
		public bool HasIndex
		{
			get { return hasIndex; }
		}

		/// <summary></summary>
		public bool IsLazy
		{
			get { return isLazy; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string TableName
		{
			get { return qualifiedTableName; }
		}

		/// <summary></summary>
		public bool IsInverse
		{
			get { return isInverse; }
		}

		/// <summary></summary>
		public string QualifiedTableName
		{
			get { return qualifiedTableName; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="session"></param>
		public void Remove( object id, ISessionImplementor session )
		{
			if( !isInverse )
			{
				if( log.IsDebugEnabled )
				{
					log.Debug( "Deleting collection: " + role + "#" + id );
				}

				IDbCommand st = session.Batcher.PrepareBatchCommand( SqlDeleteString );

				try
				{
					WriteKey( st, id, false, session );
					session.Batcher.AddToBatch( -1 );
				} 
					// TODO: change to SqlException
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
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="id"></param>
		/// <param name="session"></param>
		public void Recreate( PersistentCollection collection, object id, ISessionImplementor session )
		{
			if( !isInverse )
			{
				if( log.IsDebugEnabled )
				{
					log.Debug( "Inserting collection: " + role + "#" + id );
				}

				ICollection entries = collection.Entries();
				if( entries.Count > 0 )
				{
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
				}
				else
				{
					if( log.IsDebugEnabled )
					{
						log.Debug( "collection was empty" );
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="id"></param>
		/// <param name="session"></param>
		public void DeleteRows( PersistentCollection collection, object id, ISessionImplementor session )
		{
			if( !isInverse )
			{
				if( log.IsDebugEnabled )
				{
					log.Debug( "Deleting rows of collection: " + role + "#" + id );
				}

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
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="id"></param>
		/// <param name="session"></param>
		public void UpdateRows( PersistentCollection collection, object id, ISessionImplementor session )
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="collection"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		protected abstract int DoUpdateRows( object key, PersistentCollection collection, ISessionImplementor session );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="id"></param>
		/// <param name="session"></param>
		public void InsertRows( PersistentCollection collection, object id, ISessionImplementor session )
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

				// insert all the new entries
				ICollection entries = collection.Entries();
				try
				{
					// Moved the IDbCommand outside the loop otherwise, don't get the logic of this, it will be reinitialised each time round
					IDbCommand st = null;
					foreach( object entry in entries )
					{
						if( collection.NeedsInserting( entry, i, elementType ) )
						{
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
				}
					//TODO: change to SqlException
				catch( Exception e )
				{
					session.Batcher.AbortBatch( e );
					throw;
				}

				if( log.IsDebugEnabled )
				{
					log.Debug( string.Format( "done inserting rows: {0} inserted", count ) );
				}
			}
		}

		/// <summary>Get the name of this collection role (the fully qualified class name, extended by a "property path")</summary>
		public string Role
		{
			get { return role; }
		}

		/// <summary></summary>
		public System.Type OwnerClass
		{
			get { return ownerClass; }
		}

		/// <summary></summary>
		public IIdentifierGenerator IdentifierGenerator
		{
			get { return identifierGenerator; }
		}

		/// <summary></summary>
		public string IdentifierColumnName
		{
			get { return identifierColumnName; }
		}

		/// <summary></summary>
		public IType IdentifierType
		{
			get { return identifierType; }
		}

		/// <summary></summary>
		public bool HasIdentifier
		{
			get { return hasIdentifier; }
		}

		/// <summary></summary>
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

		/// <summary>
		/// 
		/// </summary>
		public object CollectionSpace
		{
			get { return TableName; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public IType ToType( string propertyName )
		{
			if ( "index".Equals( propertyName ) )
			{
				return indexType;
			}
			return elementPropertyMapping.ToType( propertyName );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public string[] ToColumns( string alias, string propertyName )
		{
			if ( "index".Equals( propertyName ) )
			{
				if ( IsManyToMany )
				{
					throw new QueryException( "index() function not supported for many-to-many association" );
				}
				return StringHelper.Qualify( alias, indexColumnNames );
			}
			return elementPropertyMapping.ToColumns( alias, propertyName );
		}

		/// <summary>
		/// 
		/// </summary>
		public IType Type
		{
			get { return elementPropertyMapping.Type; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="owner"></param>
		/// <param name="session"></param>
		public void Initialize( object key, object owner, ISessionImplementor session )
		{
			try
			{
				initializer.Initialize( key, session );
			}
			catch ( Exception e )
			{
				// TODO: Improve the exception info, original java shown below
				// throw convert( sqle, "could not initialize collection: " + MessageHelper.infoString(this, key) );
				throw new Exception( "could not initialize collection: ", e );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		public SqlString SelectFragment( string alias )
		{
			SelectFragment frag = new SelectFragment( factory.Dialect )
				.SetSuffix( String.Empty )		//always ignore suffix for collection columns
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="suffix"></param>
		/// <param name="includeCollectionColumns"></param>
		/// <returns></returns>
		public abstract SqlString SelectFragment( string alias, string suffix, bool includeCollectionColumns );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="innerJoin"></param>
		/// <param name="includeSubclasses"></param>
		/// <returns></returns>
		public abstract SqlString FromJoinFragment( string alias, bool innerJoin, bool includeSubclasses );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="innerJoin"></param>
		/// <param name="includeSubclasses"></param>
		/// <returns></returns>
		public abstract SqlString WhereJoinFragment( string alias, bool innerJoin, bool includeSubclasses );

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public abstract bool ConsumesAlias( );
	}
}
