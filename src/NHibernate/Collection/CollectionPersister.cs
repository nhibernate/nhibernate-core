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
	public sealed class CollectionPersister { //: ICollectionMetadata {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(CollectionPersister));

		private string sqlSelectString;
		private string sqlDeleteString;
		private string sqlInsertRowString;
		private string sqlUpdateRowString;
		private string sqlDeleteRowString;
		private string sqlOrderByString;
		private string sqlWhereString;
		private bool hasOrder;
		private bool hasWhere;
		private bool isSet;
		private IType keyType;
		private IType indexType;
		private IType elementType;
		private string[] keyColumnNames;
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
		private int enableJoinedFetch;
		private System.Type ownerClass;

		private ICollectionInitializer loader;

		private string role;

		public CollectionPersister(Mapping.Collection  collection, Configuration datastore, ISessionFactoryImplementor factory) {
			//TODO: finish
		}

		public ICollectionInitializer Initializer {
			get { return loader; }
		}
		public ICollectionInitializer CreateCollectionQuery(ISessionFactoryImplementor factory) {
			/*return isOneToMany ?
				(ICollectionInitializer) new OneToManyLoader(this, factory) :
				(ICollectionInitializer) new CollectionLoader(this, factory);
				
			*/ 
			//TODO: uncomment
			return null;
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

		public string GetSQLWhereString(string alias) {
			string[] tokens = sqlWhereString.Split( ' ', '=', '>', '<', '!' );
			StringBuilder result = new StringBuilder();
			foreach(string token in tokens) {
				if (char.IsLetter(token[0]) && !keywords.Contains(token) ) {
					//TODO: handle and, or, not
					result.Append(alias).Append(StringHelper.Dot).Append(token);
				} else {
					result.Append(token);
				}
			}
			return result.ToString();
		}

		private static readonly IList keywords = new ArrayList();

		static CollectionPersister() {
			keywords.Add("and");
			keywords.Add("or");
			keywords.Add("not");
			keywords.Add("like");
			keywords.Add("is");
			keywords.Add("null");
		}

		public string GetSQLOrderByString(string alias) {
			string[] tokens = sqlOrderByString.Split(',');
			StringBuilder result = new StringBuilder();
			int i=0;
			foreach(string token in tokens) {
				i++;
				result.Append(alias).Append(StringHelper.Dot).Append( token.Trim() );
				if (i<tokens.Length) result.Append(StringHelper.CommaSpace);
			}
			return result.ToString();
		}

		public int EnableJoinFetch {
			get { return enableJoinedFetch; }
		}

		public bool HasOrdering {
			get { return hasOrder; }
		}

		public bool HasWhere {
			get { return hasWhere; }
		}

		public string SQLSelectString {
			get { return sqlSelectString; }
		}
		
		public string SQLDeleteString {
			get { return sqlDeleteString; }
		}

		public string SQLInsertRowString {
			get { return sqlInsertRowString; }
		}

		public string SQLUpdateRowString {
			get { return sqlUpdateRowString; }
		}

		public string SQLDeleteRowString {
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

		public object ReadElement(IDataReader rs, object owner, ISessionImplementor session) {
			object element = ElementType.NullSafeGet(rs, unquotedElementColumnNames, session, owner);
			return element;
		}
		public object ReadIndex(IDataReader rs, ISessionImplementor session) {
			return IndexType.NullSafeGet(rs, unquotedIndexColumnNames, session, null);
		}

		public void WriteElement(IDbCommand st, object elt, bool writeOrder, ISessionImplementor session) {
			ElementType.NullSafeSet(st, elt, 1+(writeOrder?0:keyColumnNames.Length+(hasIndex?indexColumnNames.Length:0)), session);
		}

		public void WriteIndex(IDbCommand st, object idx, bool writeOrder, ISessionImplementor session) {
			IndexType.NullSafeSet(st, idx, 1+keyColumnNames.Length + (writeOrder?elementColumnNames.Length:0), session);
		}

		private void WriteRowSelect(IDbCommand st, object idx, ISessionImplementor session) {
			rowSelectType.NullSafeSet(st, idx, 1+keyColumnNames.Length, session);
		}

		public void WriteKey(IDbCommand st, object id, bool writeOrder, ISessionImplementor session) {
			if ( id==null ) throw new NullReferenceException("Null collection key");
			KeyType.NullSafeSet(st, id, 1+(writeOrder?elementColumnNames.Length:0), session);
		}

		public bool IsPrimitiveArray {
			get { return primitiveArray; }
		}

		public bool IsArray {
			get { return array; }
		}

		public string SelectClauseFragment(string alias) {
			SelectFragment frag = new SelectFragment()
				.SetSuffix(StringHelper.EmptyString)
				.AddColumns(alias, elementColumnNames);
			if (hasIndex) frag.AddColumns(alias, indexColumnNames);
			return frag.ToFragmentString()
				.Substring(2); //string leading ','
		}

		private string SqlSelectString() {
			SimpleSelect select = new SimpleSelect()
				.SetTableName(qualifiedTableName)
				.AddColumns(elementColumnNames);
			if (hasIndex) select.AddColumns(indexColumnNames);
			select.AddCondition( keyColumnNames, "=?" );
			if (hasOrder) select.SetOrderBy(sqlOrderByString);
			return select.ToStatementString();
		}

		private string SqlDeleteString() {
			if (isOneToMany) {
				Update update = new Update()
					.SetTableName(qualifiedTableName)
					.AddColumns(keyColumnNames, "null");
				if (hasIndex) update.AddColumns(indexColumnNames, "null");
				return update.SetPrimaryKeyColumnNames(keyColumnNames)
					.ToStatementString();
			} else {
				return new Delete()
					.SetTableName(qualifiedTableName)
					.SetPrimaryKeyColumnNames(keyColumnNames)
					.ToStatementString();
			}
		}

		private string SqlInsertRowString() {
			if (isOneToMany) {
				Update update = new Update()
					.SetTableName(qualifiedTableName)
					.AddColumns(keyColumnNames);
				if (hasIndex) update.AddColumns(indexColumnNames);
				return update.SetPrimaryKeyColumnNames(elementColumnNames)
					.ToStatementString();
			} else {
				Insert insert = new Insert(null)
					.SetTableName(qualifiedTableName)
					.AddColumns(keyColumnNames);
				if (hasIndex) insert.AddColumns(indexColumnNames);
				return insert.AddColumns(elementColumnNames)
					.ToStatementString();
			}
		}

		private string SqlUpdateRowString() {
			if (isOneToMany) {
				return null;
			} else {
				return new Update()
					.SetTableName(qualifiedTableName)
					.AddColumns(elementColumnNames)
					.SetPrimaryKeyColumnNames( ArrayHelper.Join(keyColumnNames, rowSelectColumnNames) )
					.ToStatementString();
			}
		}

		private string SqlDeleteRowString() {
			string[] pkColumns = ArrayHelper.Join(keyColumnNames, rowSelectColumnNames);
			if (isOneToMany) {
				Update update = new Update()
					.SetTableName(qualifiedTableName)
					.AddColumns(keyColumnNames, "null");
				if (hasIndex) update.AddColumns(indexColumnNames, "null");
				return update.SetPrimaryKeyColumnNames(pkColumns).ToStatementString();
			} else {
				return new Delete()
					.SetTableName(qualifiedTableName)
					.SetPrimaryKeyColumnNames(pkColumns)
					.ToStatementString();
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
		
		}

		public void Recreate(PersistentCollection collection, object id, ISessionImplementor session) {
		
		}

		public void DeleteRows(PersistentCollection collection, object id, ISessionImplementor session) {
		
		}

		public void Update(object id, PersistentCollection collection, ISessionImplementor session) {
		
		}

		public void UpdateOneToMany(object id, PersistentCollection collection, ISessionImplementor session) {

		}

		public void UpdateRows(PersistentCollection collection, object id, ISessionImplementor session) {

		}

		public void InsertRows(PersistentCollection collection, object id, ISessionImplementor session) {
			
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
