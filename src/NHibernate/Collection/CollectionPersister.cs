using System;
using System.Text;
using System.Data;
using System.Collections;

using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Loader;
using NHibernate.Mapping;
using NHibernate.Metadata;
using NHibernate.Sql;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Collection {

	/// <summary>
	/// Plugs into an instance of <c>PersistentCollection</c>, in order to implement
	/// persistence of that collection while in a particular role.
	/// </summary>
	/// <remarks>
	/// May be considered an immutable view of the mapping object
	/// </remarks>
	public sealed class CollectionPersister : ICollectionMetadata {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(CollectionPersister));

		private SqlString sqlSelectString;
		private SqlString sqlDeleteString; 
		private SqlString sqlInsertRowString;
		private SqlString sqlUpdateRowString;
		private SqlString sqlDeleteRowString;

		private string sqlOrderByString;		
		private string sqlOrderByStringTemplate;
		private string sqlWhereString;
		private string sqlWhereStringTemplate;

		private bool hasOrder;
		private bool hasWhere;
		private bool isSet;
		private IType keyType;
		private IType indexType;
		private IType elementType;
		private string[] keyColumnNames;
		private string[] keyColumnAliases;
		private string[] indexColumnNames;
		private string[] unquotedIndexColumnNames;
		private string[] elementColumnNames;
		private string[] unquotedElementColumnNames;
		private string[] rowSelectColumnNames;
		private IType rowSelectType;
		private bool primitiveArray;
		private bool array;
		private bool isOneToMany;
		private string qualifiedTableName;
		private bool hasIndex;
		private bool isLazy;
		private bool isInverse;
		private System.Type elementClass;
		private ICacheConcurrencyStrategy cache;
		private PersistentCollectionType collectionType;
		private OuterJoinLoaderType enableJoinedFetch;
		private System.Type ownerClass;

		private ICollectionInitializer loader;

		private string role;

		private Dialect.Dialect dialect;
		private ISessionFactoryImplementor factory;

		public CollectionPersister(Mapping.Collection collection, Configuration datastore, ISessionFactoryImplementor factory) {
			
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

			//hasOrphanDelete = collection.hasOrphanDelete();
		
			cache = collection.Cache;

			keyType = collection.Key.Type;
			int span = collection.Key.ColumnSpan;
			keyColumnNames = new string[span];
			string[] keyAliases = new string[span];
			int k=0;
			foreach(Column col in collection.Key.ColumnCollection) {
				keyColumnNames[k] = col.GetQuotedName(dialect);
				keyAliases[k] = col.Alias(dialect);
				k++;
			}
			keyColumnAliases = alias.ToAliasStrings(keyAliases, dialect);

			isSet = collection.IsSet;
			isOneToMany = collection.IsOneToMany;
			primitiveArray = collection.IsPrimitiveArray;
			array = collection.IsArray;

			if (isOneToMany) {
				EntityType type = collection.OneToMany.Type;
				elementType = type;
				PersistentClass associatedClass = datastore.GetClassMapping( type.PersistentClass );
				span = associatedClass.Identifier.ColumnSpan;
				elementColumnNames = new string[span];
				int j=0;
				foreach(Column col in associatedClass.Key.ColumnCollection) {
					elementColumnNames[j] = col.GetQuotedName(dialect);
					j++;
				}
				Table table = associatedClass.Table;
				qualifiedTableName = table.GetQualifiedName( dialect, factory.DefaultSchema );
				enableJoinedFetch = OuterJoinLoaderType.Eager;
			} else {
				Table table = collection.Table;
				qualifiedTableName = table.GetQualifiedName( dialect, factory.DefaultSchema );
				elementType = collection.Element.Type;
				span = collection.Element.ColumnSpan;
				elementColumnNames = new string[span];
				enableJoinedFetch = collection.Element.OuterJoinFetchSetting;

				int i=0;
				foreach(Column col in collection.Element.ColumnCollection) {
					elementColumnNames[i] = col.GetQuotedName(dialect);
					i++;
				}
			}
			unquotedElementColumnNames = dialect.UnQuote(elementColumnNames); //StringHelper.UnQuote(elementColumnNames);

			if ( hasIndex = collection.IsIndexed ) {
				IndexedCollection indexedMap = (IndexedCollection) collection;
				
				indexType = indexedMap.Index.Type;
				int indexSpan = indexedMap.Index.ColumnSpan;
				indexColumnNames = new string[indexSpan];
				int i=0;
				foreach(Column indexCol in indexedMap.Index.ColumnCollection) {
					indexColumnNames[i++] = indexCol.GetQuotedName(dialect);
				}
				rowSelectColumnNames = indexColumnNames;
				rowSelectType = indexType;
				unquotedIndexColumnNames = dialect.UnQuote(indexColumnNames); // StringHelper.UnQuote(indexColumnNames);
			} else {
				indexType = null;
				indexColumnNames = null;
				unquotedIndexColumnNames = null;
				rowSelectColumnNames = elementColumnNames;
				rowSelectType = elementType;
			}

			// TODO: refactor AddColumn method in insert to AddColumns
			sqlSelectString = GenerateSqlSelectString();
			sqlDeleteString = GenerateSqlDeleteString();
			sqlInsertRowString = GenerateSqlInsertRowString();
			sqlUpdateRowString = GenerateSqlUpdateRowString();
			sqlDeleteRowString = GenerateSqlDeleteRowString();

			isLazy = collection.IsLazy;

			isInverse = collection.IsInverse;

			if (collection.IsArray ) {
				elementClass = ( (Mapping.Array) collection ).ElementClass;
			} 
			else {
				// for non-arrays, we don't need to know the element class
				elementClass = null;
			}
			loader = CreateCollectionQuery(factory);
		}

		public ICollectionInitializer Initializer {
			get { return loader; }
		}
		public ICollectionInitializer CreateCollectionQuery(ISessionFactoryImplementor factory) {
			return isOneToMany ?
				(ICollectionInitializer) new OneToManyLoader(this, factory) :
				(ICollectionInitializer) new CollectionLoader(this, factory);
	
		}
		public void Cache(object id, PersistentCollection coll, ISessionImplementor s) {
			if (cache!=null) {
				cache.Put(id, coll.Disassemble(this), s.Timestamp);
			}
		}
		public PersistentCollection GetCachedCollection(object id, object owner, ISessionImplementor s) {
			if (cache==null) {
				return null;
			} else {
				object cached = cache.Get( id, s.Timestamp );
				if (cached==null) {
					return null;
				} else {
					return collectionType.AssembleCachedCollection(s, this, cached, owner);
				}
			}
		}

		public void Softlock(object id) {
			if (cache!=null) cache.Lock(id);
		}
		public void ReleaseSoftlock(object id) {
			if (cache!=null) cache.Release(id);
		}

		public PersistentCollectionType CollectionType {
			get {
				return this.collectionType;
			}
		}

		public string GetSQLWhereString(string alias) 
		{
			if(sqlWhereStringTemplate!=null)
				return StringHelper.Replace(sqlWhereStringTemplate, Template.PlaceHolder, alias);
			else
				return null;


//			string[] tokens = sqlWhereString.Split( ' ', '=', '>', '<', '!' );
//			StringBuilder result = new StringBuilder();
//			foreach(string token in tokens) {
//				if (token.Length == 0)
//					continue;
//				if (char.IsLetter(token[0]) && !keywords.Contains(token) ) {
//					//todo: handle and, or, not
//					result.Append(alias).Append(StringHelper.Dot).Append(token);
//				} else {
//					result.Append(token);
//				}
//			}
//			return result.ToString();
		}

//		private static readonly IList keywords = new ArrayList();
//
//		static CollectionPersister() {
//			keywords.Add("and");
//			keywords.Add("or");
//			keywords.Add("not");
//			keywords.Add("like");
//			keywords.Add("is");
//			keywords.Add("null");
//		}

		public string GetSQLOrderByString(string alias) 
		{
			if(sqlOrderByStringTemplate!=null)
				return StringHelper.Replace(sqlOrderByStringTemplate, Template.PlaceHolder, alias);
			else
				return null;

//			string[] tokens = sqlOrderByString.Split(',');
//			StringBuilder result = new StringBuilder();
//			int i=0;
//			foreach(string token in tokens) {
//				i++;
//				result.Append(alias).Append(StringHelper.Dot).Append( token.Trim() );
//				if (i<tokens.Length) result.Append(StringHelper.CommaSpace);
//			}
//			return result.ToString();
		}

		public OuterJoinLoaderType EnableJoinFetch {
			get { return enableJoinedFetch; }
		}

		public bool HasOrdering {
			get { return hasOrder; }
		}

		public bool HasWhere {
			get { return hasWhere; }
		}

		public SqlString SqlSelectString {
			get { return sqlSelectString; }
		}
		
		public SqlString SqlDeleteString {
			get { return sqlDeleteString; }
		}

		public SqlString SqlInsertRowString {
			get { return sqlInsertRowString; }
		}

		public SqlString SqlUpdateRowString {
			get { return sqlUpdateRowString; }
		}

		public SqlString SqlDeleteRowString {
			get { return sqlDeleteRowString; }
		}

		public IType KeyType {
			get { return keyType; }
		}

		public IType IndexType {
			get { return indexType; }
		}

		public IType ElementType {
			get { return elementType; }
		}

		public System.Type ElementClass {
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
		public object ReadElementIdentifier(IDataReader rs, object owner, ISessionImplementor session) {
			return ElementType.Hydrate(rs, unquotedElementColumnNames, session, owner);
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
		public object ReadElement(IDataReader rs, object owner, ISessionImplementor session) {
			object element = ElementType.NullSafeGet(rs, unquotedElementColumnNames, session, owner);
			return element;
		}

		public object ReadIndex(IDataReader rs, ISessionImplementor session) {
			return IndexType.NullSafeGet(rs, unquotedIndexColumnNames, session, null);
		}

		public object ReadKey(IDataReader dr, ISessionImplementor session) {
			//TODO: h2.0.3 = use keyColumnAliases instead of keyColumnNames
			return KeyType.NullSafeGet(dr, keyColumnAliases, session, null);
		}

		public void WriteElement(IDbCommand st, object elt, bool writeOrder, ISessionImplementor session) {
			//ElementType.NullSafeSet(st, elt, 1+(writeOrder?0:keyColumnNames.Length+(hasIndex?indexColumnNames.Length:0)), session);
			ElementType.NullSafeSet(st, elt, (writeOrder?0:keyColumnNames.Length+(hasIndex?indexColumnNames.Length:0)), session);
		}

		public void WriteIndex(IDbCommand st, object idx, bool writeOrder, ISessionImplementor session) {
			//IndexType.NullSafeSet(st, idx, 1+keyColumnNames.Length + (writeOrder?elementColumnNames.Length:0), session);
			IndexType.NullSafeSet(st, idx, keyColumnNames.Length + (writeOrder?elementColumnNames.Length:0), session);
		}

		private void WriteRowSelect(IDbCommand st, object idx, ISessionImplementor session) {
			//rowSelectType.NullSafeSet(st, idx, 1+keyColumnNames.Length, session);
			rowSelectType.NullSafeSet(st, idx, keyColumnNames.Length, session);
		}

		public void WriteKey(IDbCommand st, object id, bool writeOrder, ISessionImplementor session) {
			if ( id==null ) throw new NullReferenceException("Null collection key");
			//KeyType.NullSafeSet(st, id, 1+(writeOrder?elementColumnNames.Length:0), session);
			KeyType.NullSafeSet(st, id, (writeOrder?elementColumnNames.Length:0), session);
		}

		public bool IsPrimitiveArray {
			get { return primitiveArray; }
		}

		public bool IsArray {
			get { return array; }
		}

		public string SelectClauseFragment(string alias) {
			SqlCommand.SelectFragment frag = new SqlCommand.SelectFragment(factory.Dialect)
				.SetSuffix(String.Empty)
				.AddColumns(alias, elementColumnNames);
			if (hasIndex) frag.AddColumns(alias, indexColumnNames);
			// TODO: fix this once the interface is changed from a String to a SqlString
			// this works for now because there are no parameters in the select string.
			return frag.ToSqlStringFragment(false)
				.ToString();
				//.Substring(2); //strip leading ',' - commented out because a parameter was added ToSqlStringFragment
		}

		private SqlString GenerateSqlDeleteString() {
			if(isOneToMany) {
				SqlUpdateBuilder update = new SqlUpdateBuilder(factory);
				update.SetTableName(qualifiedTableName)
					.AddColumns(keyColumnNames, "null");
				if(hasIndex) update.AddColumns(indexColumnNames, "null");

				update.SetIdentityColumn(keyColumnNames, keyType);
				return update.ToSqlString();
				
			}
			else {
				SqlDeleteBuilder delete = new SqlDeleteBuilder(factory);
				delete.SetTableName(qualifiedTableName)
					.SetIdentityColumn(keyColumnNames, keyType);
				
				return delete.ToSqlString();
			}

		}

		private SqlString GenerateSqlSelectString(){
			SqlSimpleSelectBuilder builder = new SqlSimpleSelectBuilder(factory);
			builder.SetTableName(qualifiedTableName)
				.AddColumns(elementColumnNames);

			if (hasIndex) builder.AddColumns(indexColumnNames);
			
			builder.AddWhereFragment(keyColumnNames, keyType, " = ");

			if(hasOrder) builder.SetOrderBy(sqlOrderByString);

			return builder.ToSqlString();
			
		}

		
		private SqlString GenerateSqlInsertRowString() {
			if(isOneToMany) {
				SqlUpdateBuilder update = new SqlUpdateBuilder(factory);
				update.SetTableName(qualifiedTableName)
					.AddColumns(keyColumnNames, keyType);
				if(hasIndex) update.AddColumns(indexColumnNames, indexType);
				update.SetIdentityColumn(elementColumnNames, elementType);
				return update.ToSqlString();
				
			}
			else {
				SqlInsertBuilder insert = new SqlInsertBuilder(factory);
				insert.SetTableName(qualifiedTableName)
					.AddColumn(keyColumnNames, keyType);
				if(hasIndex) insert.AddColumn(indexColumnNames, indexType);
				insert.AddColumn(elementColumnNames, elementType);

				return insert.ToSqlString();
			}

		}

		private SqlString GenerateSqlUpdateRowString() {
			if (isOneToMany) {
				return null;
			} 
			else {
				SqlUpdateBuilder update = new SqlUpdateBuilder(factory);
				update.SetTableName(qualifiedTableName)
					.AddColumns(elementColumnNames, elementType)
					.AddWhereFragment(keyColumnNames, keyType, " = ")
					.AddWhereFragment(rowSelectColumnNames, rowSelectType, " = ");
					
				return update.ToSqlString();
				
			}
		}

		


		private SqlString GenerateSqlDeleteRowString() {
			
			if (isOneToMany) {
				SqlUpdateBuilder update = new SqlUpdateBuilder(factory);
				update.SetTableName(qualifiedTableName)
					.AddColumns(keyColumnNames, "null");

				if(hasIndex) update.AddColumns(indexColumnNames, "null");
				
				update.AddWhereFragment(keyColumnNames, keyType, " = ");
				update.AddWhereFragment(rowSelectColumnNames, rowSelectType, " = ");

				return update.ToSqlString();
				
			}
			else {
				SqlDeleteBuilder delete = new SqlDeleteBuilder(factory);
				delete.SetTableName(qualifiedTableName)
					.AddWhereFragment(keyColumnNames, keyType, " = ")
					.AddWhereFragment(rowSelectColumnNames, rowSelectType, " = ");

				return delete.ToSqlString();
				

			}
			
		}


		public string[] IndexColumnNames {
			get { return indexColumnNames; }
		}

		public string[] ElementColumnNames {
			get { return elementColumnNames; }
		}

		public string[] KeyColumnNames {
			get { return keyColumnNames; }
		}

		public bool IsOneToMany {
			get { return isOneToMany; }
		}

		public bool HasIndex {
			get { return hasIndex; }
		}

		public bool IsLazy {
			get { return isLazy; }
		}

		public bool IsInverse {
			get { return isInverse; }
		}

		public string QualifiedTableName {
			get { return qualifiedTableName; }
		}

		public void Remove(object id, ISessionImplementor session) {
			
			if (!isInverse) {
				if (log.IsDebugEnabled ) log.Debug("Deleting collection: " + role + "#" + id);

				//IDbCommand st = session.Batcher.PrepareBatchStatement( SQLDeleteString );
				IDbCommand st = session.Preparer.PrepareCommand(SqlDeleteString);

				try {
					WriteKey(st, id, false, session);
					//TODO: this is hackish for expected row count
					int expectedRowCount = -1;
					int rowCount = st.ExecuteNonQuery();

					//negative expected row count means we don't know how many rows to expect
					if ( expectedRowCount>0 && expectedRowCount!=rowCount )
						throw new HibernateException("SQL update or deletion failed (row not found)");
					//session.Batcher.AddToBatch(-1);
				} 
				catch (Exception e) {
					throw e;
				}
			}
		}
		

		public void Recreate(PersistentCollection collection, object id, ISessionImplementor session) {
			
			if (!isInverse) {
				if (log.IsDebugEnabled) log.Debug("Inserting collection: " + role + "#" + id);

				
				ICollection entries = collection.Entries();
				if (entries.Count > 0) {
					//IDbCommand st = session.Batcher.PrepareBatchStatement( SQLInsertRowString );
					IDbCommand st = session.Preparer.PrepareCommand(SqlInsertRowString);

					int i=0;
					try {
						foreach(object entry in entries) {
							if (collection.EntryExists(entry, i)) {
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
					} catch (Exception e) {
						throw e;
					}
				}
			}


		}

		public void DeleteRows(PersistentCollection collection, object id, ISessionImplementor session) {
		
			if (!isInverse) {
				
				if (log.IsDebugEnabled) log.Debug("Deleting rows of collection: " + role + "#" + id);

				ICollection entries = collection.GetDeletes(elementType);
				if (entries.Count > 0) {
					IDbCommand st = session.Preparer.PrepareCommand(SqlDeleteRowString);
					try {
						foreach(object entry in entries) {
							WriteKey(st, id, false, session);
							WriteRowSelect(st, entry, session);
							//TODO: this is hackish for expected row count
							int expectedRowCount = -1;
							int rowCount = st.ExecuteNonQuery();

							//negative expected row count means we don't know how many rows to expect
							if ( expectedRowCount>0 && expectedRowCount!=rowCount )
								throw new HibernateException("SQL update or deletion failed (row not found)");
							//session.Batcher.AddToBatch(-1);
						}
					} catch (Exception e) {
						throw e;
					}
				}
			}
		}

		public void Update(object id, PersistentCollection collection, ISessionImplementor session) {

			IDbCommand st = null;
			ICollection entries = collection.Entries();
			int i=0;
			try {
				foreach(object entry in entries) {
					if (collection.NeedsUpdating(entry, i, elementType)) {
						if (st==null) st = session.Preparer.PrepareCommand(SqlUpdateRowString); //st = session.Batcher.PrepareBatchStatement( SQLUpdateRowString );
						WriteKey(st, id, true, session);
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
			catch (Exception e) {
				throw e;
			}
		}

		public void UpdateOneToMany(object id, PersistentCollection collection, ISessionImplementor session) {

			IDbCommand rmvst = null;
			int i=0;
			ICollection entries = collection.Entries();
			try {
				foreach(object entry in entries) {
					if (collection.NeedsUpdating(entry, i, elementType) ) {
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
			} catch(Exception e) {
				throw e;
			}
			// finish all the removes first to take care of possible unique constraints
			// and so that we can take advantage of batching
			IDbCommand insst = null;
			i=0;
			entries = collection.Entries();
			try {
				foreach( object entry in entries) {
					if (collection.NeedsUpdating(entry, i, elementType) ) {
						if (insst==null) insst = session.Preparer.PrepareCommand(SqlInsertRowString); //session.Batcher.PrepareBatchStatement( SQLInsertRowString );
						WriteKey(insst, id, false, session);
						collection.WriteTo(insst, this, entry, i, false);
						//TODO: this is hackish for expected row count
						int expectedRowCount = 1;
						int rowCount = rmvst.ExecuteNonQuery();

						//negative expected row count means we don't know how many rows to expect
						if ( expectedRowCount>0 && expectedRowCount!=rowCount )
							throw new HibernateException("SQL update or deletion failed (row not found)");
						//session.Batcher.AddToBatch(1);
					}
					i++;
				}
			} catch (Exception e) {
				throw e;
			}
		}

		public void UpdateRows(PersistentCollection collection, object id, ISessionImplementor session) {
			if (!isInverse) {
				if (log.IsDebugEnabled) log.Debug("Updating rows of collection: " + role + "#" + id);

				if (isOneToMany) {
					UpdateOneToMany(id, collection, session);
				} else {
					Update(id, collection, session);
				}
			}
		}

		public void InsertRows(PersistentCollection collection, object id, ISessionImplementor session) {
			
			if (!isInverse) {
				if (log.IsDebugEnabled) log.Debug("Inserting rows of collection: " + role + "#" + id);

				ICollection entries = collection.Entries();
				IDbCommand st = null;
				int i=0;
				try {
					foreach(object entry in entries) {
						if (collection.NeedsInserting(entry, i, elementType)) {
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
				} catch (Exception e) {
					throw e;
				}
			}
		}

		public string Role {
			get { return role; }
		}

		public bool IsSet {
			get { return isSet; }
		}

		public System.Type OwnerClass {
			get { return ownerClass; }
		}
	}
}
