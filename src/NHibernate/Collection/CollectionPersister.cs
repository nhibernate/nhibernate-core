using System;
using System.Text;
using System.Data;
using System.Collections;

using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Loader;
using NHibernate.Mapping;
using NHibernate.Metadata;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Collection 
{
	/// <summary>
	/// Plugs into an instance of <c>PersistentCollection</c>, in order to implement
	/// persistence of that collection while in a particular role.
	/// </summary>
	/// <remarks>
	/// May be considered an immutable view of the mapping object
	/// </remarks>
	public sealed class CollectionPersister : ICollectionMetadata 
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(CollectionPersister));

		private readonly SqlString sqlDeleteString; 
		private readonly SqlString sqlInsertRowString;
		private readonly SqlString sqlUpdateRowString;
		private readonly SqlString sqlDeleteRowString;

		private readonly string sqlOrderByString;		
		private readonly string sqlOrderByStringTemplate;
		private readonly string sqlWhereString;
		private readonly string sqlWhereStringTemplate;

		private readonly bool hasOrder;
		private readonly bool hasWhere;
		private readonly bool hasOrphanDelete;
		private readonly IType keyType;
		private readonly IType indexType;
		private readonly IType elementType;
		private readonly string[] keyColumnNames;
		private readonly string[] indexColumnNames;
		private readonly string[] elementColumnNames;
		private readonly string[] rowSelectColumnNames;
		
		private readonly string[] indexColumnAliases;
		private readonly string[] elementColumnAliases;
		private readonly string[] keyColumnAliases;
		
		private readonly IType rowSelectType;
		private readonly bool primitiveArray;
		private readonly bool array;
		private readonly bool isOneToMany;
		private readonly string qualifiedTableName;
		private readonly bool hasIndex;
		private readonly bool isLazy;
		private readonly bool isInverse;
		private readonly System.Type elementClass;
		private readonly ICacheConcurrencyStrategy cache;
		private readonly PersistentCollectionType collectionType;
		private readonly OuterJoinLoaderType enableJoinedFetch;
		private readonly System.Type ownerClass;

		private readonly IIdentifierGenerator identifierGenerator;
		private readonly string unquotedIdentifierColumnName;
		private readonly IType identifierType;
		private readonly bool hasIdentifier;
		private readonly string identifierColumnName;
		private readonly string identifierColumnAlias;

		private readonly ICollectionInitializer loader;

		private readonly string role;

		private readonly Dialect.Dialect dialect;
		private readonly ISessionFactoryImplementor factory;

		public CollectionPersister(Mapping.Collection collection, Configuration datastore, ISessionFactoryImplementor factory) 
		{
			this.factory = factory;
			this.dialect = factory.Dialect;
			collectionType = collection.Type;
			role = collection.Role;
			ownerClass = collection.OwnerClass;
			Alias alias = new Alias("__");

			sqlOrderByString = collection.OrderBy;
			hasOrder = sqlOrderByString!=null;
			sqlOrderByStringTemplate = hasOrder ? Template.RenderOrderByStringTemplate(sqlOrderByString, dialect) : null;

			sqlWhereString = collection.Where;
			hasWhere = sqlWhereString!=null;
			sqlWhereStringTemplate = hasWhere ? Template.RenderWhereStringTemplate(sqlWhereString, dialect) : null;

			hasOrphanDelete = collection.OrphanDelete;
		
			cache = collection.Cache;

			keyType = collection.Key.Type;
			int span = collection.Key.ColumnSpan;
			keyColumnNames = new string[span];
			string[] keyAliases = new string[span];
			int k=0;
			foreach(Column col in collection.Key.ColumnCollection) 
			{
				keyColumnNames[k] = col.GetQuotedName(dialect);
				keyAliases[k] = col.Alias(dialect);
				k++;
			}
			keyColumnAliases = alias.ToAliasStrings(keyAliases, dialect);
			IList distinctColumns = new ArrayList();
			CheckColumnDuplication( distinctColumns, collection.Key.ColumnCollection );

			isOneToMany = collection.IsOneToMany;
			primitiveArray = collection.IsPrimitiveArray;
			array = collection.IsArray;

			Table table;
			ICollection iter;

			if (isOneToMany) 
			{
				EntityType type = collection.OneToMany.Type;
				elementType = type;
				PersistentClass associatedClass = datastore.GetClassMapping( type.PersistentClass );
				span = associatedClass.Identifier.ColumnSpan;
				iter = associatedClass.Key.ColumnCollection;
				table = associatedClass.Table;
				enableJoinedFetch = OuterJoinLoaderType.Eager;
			}
			else 
			{
				table = collection.Table;
				elementType = collection.Element.Type;
				span = collection.Element.ColumnSpan;
				enableJoinedFetch = collection.Element.OuterJoinFetchSetting;
				iter = collection.Element.ColumnCollection;
				CheckColumnDuplication( distinctColumns, collection.Element.ColumnCollection );

			}

			qualifiedTableName = table.GetQualifiedName( dialect, factory.DefaultSchema );
			string[] aliases = new string[span];
			elementColumnNames = new string[span];
			int j=0;
			foreach(Column col in iter) 
			{
				elementColumnNames[j] = col.GetQuotedName(dialect);
				aliases[j] = col.Alias(dialect);
				j++;
			}
			
			elementColumnAliases = alias.ToAliasStrings(aliases, dialect);

			IType selectColumns;
			string[] selectType;
			
			if ( hasIndex = collection.IsIndexed ) 
			{
				IndexedCollection indexedCollection = (IndexedCollection) collection;
				
				indexType = indexedCollection.Index.Type;
				int indexSpan = indexedCollection.Index.ColumnSpan;
				indexColumnNames = new string[indexSpan];
				
				string[] indexAliases = new string[indexSpan];
				int i=0;
				foreach(Column indexCol in indexedCollection.Index.ColumnCollection) 
				{
					indexAliases[i] = indexCol.Alias(dialect);
					indexColumnNames[i] = indexCol.GetQuotedName(dialect);
					i++;
				}
				selectType = indexColumnNames;
				selectColumns = indexType;
				indexColumnAliases = alias.ToAliasStrings(indexAliases, dialect);
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

			if (hasIdentifier) 
			{
				if ( isOneToMany) throw new MappingException("one-to-many collections with identifiers are not supported.");
				IdentifierCollection idColl = (IdentifierCollection) collection;
				identifierType = idColl.Identifier.Type;

				Column col = null;
				foreach(Column column in idColl.Identifier.ColumnCollection) 
				{
					col = column;
					break;
				}

				identifierColumnName = col.GetQuotedName(dialect);
				selectType = new string[] { identifierColumnName };
				selectColumns = identifierType;
				identifierColumnAlias = alias.ToAliasString( col.Alias(dialect), dialect );
				unquotedIdentifierColumnName = identifierColumnAlias;
				identifierGenerator = idColl.Identifier.CreateIdentifierGenerator(dialect);
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

			// TODO: refactor AddColumn method in insert to AddColumns
			sqlDeleteString = GenerateSqlDeleteString();
			sqlInsertRowString = GenerateSqlInsertRowString();
			sqlUpdateRowString = GenerateSqlUpdateRowString();
			sqlDeleteRowString = GenerateSqlDeleteRowString();

			isLazy = collection.IsLazy;

			isInverse = collection.IsInverse;

			if (collection.IsArray ) 
			{
				elementClass = ( (Mapping.Array) collection ).ElementClass;
			} 
			else 
			{
				// for non-arrays, we don't need to know the element class
				elementClass = null;
			}
			loader = CreateCollectionQuery(factory);
		}

		public ICollectionInitializer Initializer 
		{
			get { return loader; }
		}

		public ICollectionInitializer CreateCollectionQuery(ISessionFactoryImplementor factory) 
		{
			return isOneToMany ?
				(ICollectionInitializer) new OneToManyLoader(this, factory) :
				(ICollectionInitializer) new CollectionLoader(this, factory);
		}

		public void Cache(object id, PersistentCollection coll, ISessionImplementor s) 
		{
			if (cache!=null) 
			{
				if (log.IsDebugEnabled) log.Debug("Caching collection: " + role + "#" + id);
				cache.Put(id, coll.Disassemble(this), s.Timestamp);
			}
		}

		public ICacheConcurrencyStrategy CacheConcurrencyStrategy 
		{
			get { return cache; }
		}

		public bool HasCache 
		{
			get { return cache!=null; }
		}

		public PersistentCollection GetCachedCollection(object id, object owner, ISessionImplementor s) 
		{
			if (cache==null) 
			{
				return null;
			} 
			else 
			{
				if(log.IsDebugEnabled) log.Debug("Searching for collection in cache: " + role + "#" + id);
				object cached = cache.Get( id, s.Timestamp );
				if (cached==null) 
				{
					return null;
				} else 
				{
					return collectionType.AssembleCachedCollection(s, this, cached, owner);
				}
			}
		}

		public void Softlock(object id) 
		{
			if (cache!=null) cache.Lock(id);
		}

		public void ReleaseSoftlock(object id) 
		{
			if (cache!=null) cache.Release(id);
		}

		public PersistentCollectionType CollectionType 
		{
			get { return this.collectionType; }
		}

		public string GetSQLWhereString(string alias) 
		{
			if(sqlWhereStringTemplate!=null) 
			{
				return StringHelper.Replace(sqlWhereStringTemplate, Template.PlaceHolder, alias);
			}
			else 
			{
				return null;
			}

		}


		public string GetSQLOrderByString(string alias) 
		{
			if(sqlOrderByStringTemplate!=null) 
			{
				return StringHelper.Replace(sqlOrderByStringTemplate, Template.PlaceHolder, alias);
			}
			else 
			{
				return null;
			}
		}

		public OuterJoinLoaderType EnableJoinFetch 
		{
			get { return enableJoinedFetch; }
		}

		public bool HasOrdering 
		{
			get { return hasOrder; }
		}

		public bool HasWhere 
		{
			get { return hasWhere; }
		}
		
		public SqlString SqlDeleteString 
		{
			get { return sqlDeleteString; }
		}

		public SqlString SqlInsertRowString 
		{
			get { return sqlInsertRowString; }
		}

		public SqlString SqlUpdateRowString 
		{
			get { return sqlUpdateRowString; }
		}

		public SqlString SqlDeleteRowString 
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
		public object ReadElementIdentifier(IDataReader rs, object owner, ISessionImplementor session) 
		{
			return ElementType.Hydrate(rs, elementColumnAliases, session, owner);
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
		public object ReadElement(IDataReader rs, object owner, ISessionImplementor session) 
		{
			object element = ElementType.NullSafeGet(rs, elementColumnAliases, session, owner);
			return element;
		}

		public object ReadIndex(IDataReader rs, ISessionImplementor session) 
		{
			object index = IndexType.NullSafeGet(rs, indexColumnAliases, session, null);
			if(index==null) throw new HibernateException("null index column for collection: " + role);
			return index;
		}

		public object ReadIdentifier(IDataReader rs, ISessionImplementor session) 
		{
			object id = IdentifierType.NullSafeGet(rs, unquotedIdentifierColumnName, session ,null);
			if(id==null) throw new HibernateException("null identifier column for collection: " + role);
			return id;
		}

		public object ReadKey(IDataReader dr, ISessionImplementor session) 
		{
			return KeyType.NullSafeGet(dr, keyColumnAliases, session, null);
		}

		public void WriteElement(IDbCommand st, object elt, bool writeOrder, ISessionImplementor session) 
		{
			ElementType.NullSafeSet(st, elt, (writeOrder?0:keyColumnNames.Length+(hasIndex?indexColumnNames.Length:0)+(hasIdentifier?1:0)), session);
		}

		public void WriteIndex(IDbCommand st, object idx, bool writeOrder, ISessionImplementor session) 
		{
			IndexType.NullSafeSet(st, idx, keyColumnNames.Length + (writeOrder?elementColumnNames.Length:0), session);
		}

		public void WriteIdentifier(IDbCommand st, object idx, bool writeOrder, ISessionImplementor session) 
		{
			IdentifierType.NullSafeSet(st, idx, (writeOrder?elementColumnNames.Length:keyColumnNames.Length), session);
		}

		private void WriteRowSelect(IDbCommand st, object idx, ISessionImplementor session) 
		{
			rowSelectType.NullSafeSet(st, idx, (HasIdentifier ? 0 : keyColumnNames.Length), session);
		}

		public void WriteKey(IDbCommand st, object id, bool writeOrder, ISessionImplementor session) 
		{
			if ( id==null ) throw new NullReferenceException("Null key for collection: " + role);
			KeyType.NullSafeSet(st, id, (writeOrder?elementColumnNames.Length:0), session);
		}

		public bool IsPrimitiveArray 
		{
			get { return primitiveArray; }
		}

		public bool IsArray 
		{
			get { return array; }
		}

		public string SelectClauseFragment(string alias) 
		{
			SelectFragment frag = new SelectFragment(factory.Dialect)
				.SetSuffix(String.Empty)
				.AddColumns(alias, elementColumnNames, elementColumnAliases);
			if (hasIndex) frag.AddColumns(alias, indexColumnNames, indexColumnAliases);
			if (hasIdentifier) frag.AddColumn(alias, identifierColumnName, identifierColumnAlias);
			// TODO: fix this once the interface is changed from a String to a SqlString
			// this works for now because there are no parameters in the select string.
			return frag.ToSqlStringFragment(false)
				.ToString();
		}

		public string MultiselectClauseFragment(string alias) 
		{
			SelectFragment frag = new SelectFragment(dialect)
				.SetSuffix(String.Empty)
				.AddColumns(alias, elementColumnNames, elementColumnAliases)
				.AddColumns(alias, keyColumnNames, keyColumnAliases);
			if (hasIndex) frag.AddColumns(alias, indexColumnNames, indexColumnAliases);
			if (hasIdentifier) frag.AddColumn(alias, identifierColumnName, identifierColumnAlias);
			
			return frag.ToSqlStringFragment(false).ToString();	
		}

		private SqlString GenerateSqlDeleteString() 
		{
			if(isOneToMany) 
			{
				SqlUpdateBuilder update = new SqlUpdateBuilder(factory);
				update.SetTableName(qualifiedTableName)
					.AddColumns(keyColumnNames, "null");
				if(hasIndex) update.AddColumns(indexColumnNames, "null");
				if(hasWhere) update.AddWhereFragment(sqlWhereString);
				update.SetIdentityColumn(keyColumnNames, keyType);
				
				return update.ToSqlString();
			}
			else 
			{
				SqlDeleteBuilder delete = new SqlDeleteBuilder(factory);
				delete.SetTableName(qualifiedTableName)
					.SetIdentityColumn(keyColumnNames, keyType);
				if (hasWhere) delete.AddWhereFragment(sqlWhereString);

				return delete.ToSqlString();
			}

		}
		
		private SqlString GenerateSqlInsertRowString() 
		{
			if(isOneToMany) 
			{
				SqlUpdateBuilder update = new SqlUpdateBuilder(factory);
				update.SetTableName(qualifiedTableName)
					.AddColumns(keyColumnNames, keyType);
				if(hasIndex) update.AddColumns(indexColumnNames, indexType);
				update.SetIdentityColumn(elementColumnNames, elementType);
				return update.ToSqlString();
				
			}
			else 
			{
				SqlInsertBuilder insert = new SqlInsertBuilder(factory);
				insert.SetTableName(qualifiedTableName)
					.AddColumn(keyColumnNames, keyType);
				if(hasIndex) insert.AddColumn(indexColumnNames, indexType);
				if(hasIdentifier) insert.AddColumn(new string[] {identifierColumnName}, identifierType);
				insert.AddColumn(elementColumnNames, elementType);

				return insert.ToSqlString();
			}

		}

		private SqlString GenerateSqlUpdateRowString() 
		{
			if (isOneToMany) 
			{
				return null;
			} 
			else 
			{
				SqlUpdateBuilder update = new SqlUpdateBuilder(factory);
				update.SetTableName(qualifiedTableName)
					.AddColumns(elementColumnNames, elementType);
				if(hasIdentifier) 
				{
					update.AddWhereFragment(rowSelectColumnNames, rowSelectType, " = ");
				}
				else 
				{
					update.AddWhereFragment(keyColumnNames, keyType, " = ")
						.AddWhereFragment(rowSelectColumnNames, rowSelectType, " = ");
				}
					
				return update.ToSqlString();
				
			}
		}

		private SqlString GenerateSqlDeleteRowString() 
		{
			if (isOneToMany) 
			{
				SqlUpdateBuilder update = new SqlUpdateBuilder(factory);
				update.SetTableName(qualifiedTableName)
					.AddColumns(keyColumnNames, "null");

				if(hasIndex) update.AddColumns(indexColumnNames, "null");
				
				if(hasIdentifier) 
				{
					update.AddWhereFragment(rowSelectColumnNames, rowSelectType, " = ");
				}
				else 
				{
					update.AddWhereFragment(keyColumnNames, keyType, " = ");
					update.AddWhereFragment(rowSelectColumnNames, rowSelectType, " = ");
				}

				return update.ToSqlString();
				
			}
			else 
			{
				SqlDeleteBuilder delete = new SqlDeleteBuilder(factory);
				delete.SetTableName(qualifiedTableName);
				if(hasIdentifier) 
				{
					delete.AddWhereFragment(rowSelectColumnNames, rowSelectType, " = ");
				}
				else 
				{
					delete.AddWhereFragment(keyColumnNames, keyType, " = ")
						.AddWhereFragment(rowSelectColumnNames, rowSelectType, " = ");
				}

				return delete.ToSqlString();
				
			}
			
		}

		public string[] IndexColumnNames 
		{
			get { return indexColumnNames; }
		}

		public string[] ElementColumnNames 
		{
			get { return elementColumnNames; }
		}

		public string[] KeyColumnNames 
		{
			get { return keyColumnNames; }
		}

		public bool IsOneToMany 
		{
			get { return isOneToMany; }
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

		public string QualifiedTableName 
		{
			get { return qualifiedTableName; }
		}

		public void Remove(object id, ISessionImplementor session) 
		{
			if (!isInverse) 
			{
				if (log.IsDebugEnabled ) log.Debug("Deleting collection: " + role + "#" + id);

				//IDbCommand st = session.Batcher.PrepareBatchStatement( SQLDeleteString );
				IDbCommand st = session.Preparer.PrepareCommand(SqlDeleteString);

				try 
				{
					WriteKey(st, id, false, session);
					//TODO: this is hackish for expected row count
					int expectedRowCount = -1;
					int rowCount = st.ExecuteNonQuery();

					//negative expected row count means we don't know how many rows to expect
					if ( expectedRowCount>0 && expectedRowCount!=rowCount )
						throw new HibernateException("SQL update or deletion failed (row not found)");
					//session.Batcher.AddToBatch(-1);
				} 
				catch (Exception e) 
				{
					throw e;
				}
				if (log.IsDebugEnabled ) log.Debug("done deleting collection");

			}
		}
		

		public void Recreate(PersistentCollection collection, object id, ISessionImplementor session) 
		{
			if (!isInverse) 
			{
				if (log.IsDebugEnabled) log.Debug("Inserting collection: " + role + "#" + id);
				
				ICollection entries = collection.Entries();
				if (entries.Count > 0) 
				{
					//IDbCommand st = session.Batcher.PrepareBatchStatement( SQLInsertRowString );
					IDbCommand st = session.Preparer.PrepareCommand(SqlInsertRowString);

					int i=0;
					try 
					{
						foreach(object entry in entries) 
						{
							if (collection.EntryExists(entry, i)) 
							{
								collection.PreInsert(this, entry, i); //TODO: (Big): this here screws up batch - H2.0.3 comment
								WriteKey(st, id, false, session);
								collection.WriteTo(st, this, entry, i, false);
								//TODO: this is hackish for expected row count
								int expectedRowCount = 1;
								int rowCount = st.ExecuteNonQuery();

								//negative expected row count means we don't know how many rows to expect
								if ( expectedRowCount>0 && expectedRowCount!=rowCount )
									throw new HibernateException("SQL update or deletion failed (row not found)");
								//session.Batcher.AddToBatch(1);
							}
							i++;
						}
					} 
					catch (Exception e) 
					{
						throw e;
					}
					if( log.IsDebugEnabled) log.Debug("done inserting collection");
				}
				else 
				{
					if( log.IsDebugEnabled) log.Debug("collection was empty");
				}
			}
		}

		public void DeleteRows(PersistentCollection collection, object id, ISessionImplementor session) 
		{		
			if (!isInverse) 
			{				
				if (log.IsDebugEnabled) log.Debug("Deleting rows of collection: " + role + "#" + id);

				ICollection entries = collection.GetDeletes(elementType);
				if (entries.Count > 0) 
				{
					IDbCommand st = session.Preparer.PrepareCommand(SqlDeleteRowString);
					try 
					{
						foreach(object entry in entries) 
						{
							if(!hasIdentifier) WriteKey(st, id, false, session);
							WriteRowSelect(st, entry, session);
							//TODO: this is hackish for expected row count
							int expectedRowCount = -1;
							int rowCount = st.ExecuteNonQuery();

							//negative expected row count means we don't know how many rows to expect
							if ( expectedRowCount>0 && expectedRowCount!=rowCount )
								throw new HibernateException("SQL update or deletion failed (row not found)");
							//session.Batcher.AddToBatch(-1);
						}
					} 
					catch (Exception e) 
					{
						throw e;
					}
					
					if( log.IsDebugEnabled ) log.Debug("done deleting collection rows");
				}
				else 
				{
					if( log.IsDebugEnabled ) log.Debug("no rows to delete");
				}
			}
		}

		public void Update(object id, PersistentCollection collection, ISessionImplementor session) 
		{
			IDbCommand st = null;
			ICollection entries = collection.Entries();
			int i=0;
			try 
			{
				foreach(object entry in entries) 
				{
					if (collection.NeedsUpdating(entry, i, elementType)) 
					{
						if (st==null) st = session.Preparer.PrepareCommand(SqlUpdateRowString); //st = session.Batcher.PrepareBatchStatement( SQLUpdateRowString );
						if(!hasIdentifier) WriteKey(st, id, true, session);
						collection.WriteTo(st, this, entry, i, true);
						//TODO: this is hackish for expected row count
						int expectedRowCount = 1;
						int rowCount = st.ExecuteNonQuery();

						//negative expected row count means we don't know how many rows to expect
						if ( expectedRowCount>0 && expectedRowCount!=rowCount )
							throw new HibernateException("SQL update or deletion failed (row not found)");
						//session.Batcher.AddToBatch(1);
					}
					i++;
				}
			} 
			catch (Exception e) 
			{
				throw e;
			}
		}

		public void UpdateOneToMany(object id, PersistentCollection collection, ISessionImplementor session) 
		{
			IDbCommand rmvst = null;
			int i=0;
			ICollection entries = collection.Entries();
			try 
			{
				foreach(object entry in entries) 
				{
					if (collection.NeedsUpdating(entry, i, elementType) ) 
					{
						if (rmvst==null)  rmvst = session.Preparer.PrepareCommand(SqlDeleteRowString);//rmvst = session.Batcher.PrepareBatchStatement( SQLDeleteRowString );
						WriteKey(rmvst, id, false, session);
						WriteIndex(rmvst, collection.GetIndex(entry, i), false, session);
						//TODO: this is hackish for expected row count
						int expectedRowCount = -1;
						int rowCount = rmvst.ExecuteNonQuery();

						//negative expected row count means we don't know how many rows to expect
						if ( expectedRowCount>0 && expectedRowCount!=rowCount )
							throw new HibernateException("SQL update or deletion failed (row not found)");
						//session.Batcher.AddToBatch(-1);
					}
					i++;
				}
			} 
			catch(Exception e) 
			{
				throw e;
			}

			// finish all the removes first to take care of possible unique constraints
			// and so that we can take advantage of batching
			IDbCommand insst = null;
			i=0;
			entries = collection.Entries();
			try 
			{
				foreach( object entry in entries) 
				{
					if (collection.NeedsUpdating(entry, i, elementType) ) 
					{
						if (insst==null) insst = session.Preparer.PrepareCommand(SqlInsertRowString); //session.Batcher.PrepareBatchStatement( SQLInsertRowString );
						WriteKey(insst, id, false, session);
						collection.WriteTo(insst, this, entry, i, false);
						//TODO: this is hackish for expected row count
						int expectedRowCount = 1;
						int rowCount = insst.ExecuteNonQuery();

						//negative expected row count means we don't know how many rows to expect
						if ( expectedRowCount>0 && expectedRowCount!=rowCount )
							throw new HibernateException("SQL update or deletion failed (row not found)");
						//session.Batcher.AddToBatch(1);
					}
					i++;
				}
			} 
			catch (Exception e) 
			{
				throw e;
			}
		}

		public void UpdateRows(PersistentCollection collection, object id, ISessionImplementor session) 
		{
			if (!isInverse) 
			{
				if( log.IsDebugEnabled ) log.Debug("Updating rows of collection: " + role + "#" + id);

				// update all the modified entries
				if (isOneToMany) 
				{
					UpdateOneToMany(id, collection, session);
				} else 
				{
					Update(id, collection, session);
				}

				if( log.IsDebugEnabled ) log.Debug("done updating rows");
			}
		}

		public void InsertRows(PersistentCollection collection, object id, ISessionImplementor session) 
		{		
			if (!isInverse) 
			{
				if (log.IsDebugEnabled) log.Debug("Inserting rows of collection: " + role + "#" + id);

				// insert all the new entries
				ICollection entries = collection.Entries();
				IDbCommand st = null;
				int i=0;
				try 
				{
					foreach(object entry in entries) 
					{
						if (collection.NeedsInserting(entry, i, elementType)) 
						{
							collection.PreInsert(this, entry, i); //TODO: (Big): this here screws up batching! H2.0.3 comment
							if (st==null) st = session.Preparer.PrepareCommand(SqlInsertRowString); //st = session.Batcher.PrepareBatchStatement(SQLInsertRowString);
							WriteKey(st, id, false, session);
							collection.WriteTo(st, this, entry, i, false);
							//TODO: this is hackish for expected row count
							int expectedRowCount = 1;
							int rowCount = st.ExecuteNonQuery();

							//negative expected row count means we don't know how many rows to expect
							if ( expectedRowCount>0 && expectedRowCount!=rowCount )
								throw new HibernateException("SQL update or deletion failed (row not found)");
							//session.Batcher.AddToBatch(1);
						}
						i++;
					}
				} 
				catch (Exception e) 
				{
					throw e;
				}

				if( log.IsDebugEnabled ) log.Debug("done inserting rows");
			}
		}

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

		public bool HasIdentifier 
		{
			get { return hasIdentifier; }
		}

		public bool HasOrphanDelete 
		{
			get { return hasOrphanDelete; }
		}

		private void CheckColumnDuplication(IList distinctColumns, ICollection columns) 
		{
			foreach(Column col in columns) 
			{
				if( distinctColumns.Contains(col.Name) ) 
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
					distinctColumns.Add(col.Name);
				}
			}
		}

	}
}
