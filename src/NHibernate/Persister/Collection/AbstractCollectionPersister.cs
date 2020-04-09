using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using NHibernate.AdoNet;
using NHibernate.Cache;
using NHibernate.Cache.Entry;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Id;
using NHibernate.Id.Insert;
using NHibernate.Impl;
using NHibernate.Loader.Collection;
using NHibernate.Mapping;
using NHibernate.Metadata;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.Util;
using Array = NHibernate.Mapping.Array;

namespace NHibernate.Persister.Collection
{
	/// <summary>
	/// Summary description for AbstractCollectionPersister.
	/// </summary>
	public abstract partial class AbstractCollectionPersister : ICollectionMetadata, ISqlLoadableCollection,
		IPostInsertIdentityPersister, ISupportSelectModeJoinable, ICompositeKeyPostInsertIdentityPersister, ISupportLazyPropsJoinable
	{
		protected static readonly object NotFoundPlaceHolder = new object();
		private readonly string role;

		#region SQL statements

		private readonly SqlCommandInfo sqlDeleteString;
		private readonly SqlCommandInfo sqlInsertRowString;
		private readonly SqlCommandInfo sqlUpdateRowString;
		private readonly SqlCommandInfo sqlDeleteRowString;
		private readonly ConcurrentDictionary<bool[], SqlCommandInfo> sqlDeleteRowStringByNullness =
			new ConcurrentDictionary<bool[], SqlCommandInfo>(ArrayHelper.ArrayComparer<bool>.Default);
		private readonly SqlString sqlSelectRowByIndexString;
		private readonly SqlString sqlDetectRowByIndexString;
		private readonly SqlString sqlDetectRowByElementString;
		private readonly string sqlOrderByString;
		protected readonly string sqlWhereString;
		private readonly string sqlOrderByStringTemplate;
		private readonly string sqlWhereStringTemplate;
		private readonly bool hasOrder;
		private readonly bool hasWhere;
		private readonly int baseIndex;

		protected internal bool indexContainsFormula;
		protected internal bool elementIsPureFormula;

		#endregion

		#region Types

		private readonly IType keyType;
		private readonly IType indexType;
		private readonly IType elementType;
		private readonly IType identifierType;

		#endregion

		#region Columns

		private readonly string[] keyColumnNames;
		private readonly string[] indexColumnNames;
		protected readonly string[] indexFormulaTemplates;
		private readonly string[] indexFormulas;
		protected readonly bool[] indexColumnIsSettable;
		private readonly string[] elementColumnNames;
		protected readonly string[] elementFormulaTemplates;
		protected readonly string[] elementFormulas;
		protected readonly bool[] elementColumnIsSettable;
		protected readonly bool[] elementColumnIsInPrimaryKey;
		private readonly string[] indexColumnAliases;
		protected readonly string[] elementColumnAliases;
		private readonly string[] keyColumnAliases;
		private readonly string identifierColumnName;
		private readonly string identifierColumnAlias;

		#endregion

		protected readonly string qualifiedTableName;
		private readonly string queryLoaderName;

		private readonly bool isPrimitiveArray;
		private readonly bool isArray;
		private readonly bool hasIndex;
		protected readonly bool hasIdentifier;

		// NH Specific: to manage identity for id-bag (NH-364)
		protected readonly IInsertGeneratedIdentifierDelegate identityDelegate;

		private readonly bool isLazy;
		private readonly bool isExtraLazy;
		private readonly bool isInverse;
		private readonly bool isMutable;
		private readonly bool isVersioned;
		protected readonly int batchSize;
		private readonly FetchMode fetchMode;
		private readonly bool hasOrphanDelete;
		private readonly bool subselectLoadable;

		#region Extra information about the element type

		private readonly System.Type elementClass;
		private readonly string entityName;

		#endregion

		private readonly Dialect.Dialect dialect;
		private readonly ISQLExceptionConverter sqlExceptionConverter;
		private readonly ISessionFactoryImplementor factory;
		private readonly IEntityPersister ownerPersister;
		private readonly IIdentifierGenerator identifierGenerator;
		private readonly IPropertyMapping elementPropertyMapping;
		private readonly IEntityPersister elementPersister;
		private readonly ICacheConcurrencyStrategy cache;
		private readonly CollectionType collectionType;
		private ICollectionInitializer initializer;

		private readonly ICacheEntryStructure cacheEntryStructure;

		// dynamic filters for the collection
		private readonly FilterHelper filterHelper;

		#region Dynamic filters specifically for many-to-many inside the collection

		private readonly FilterHelper manyToManyFilterHelper;
		private readonly string manyToManyWhereString;
		private readonly string manyToManyWhereTemplate;
		private readonly string manyToManyOrderByString;
		private readonly string manyToManyOrderByTemplate;

		#endregion

		#region Custom SQL

		private readonly bool insertCallable;
		private readonly bool updateCallable;
		private readonly bool deleteCallable;
		private readonly bool deleteAllCallable;
		private readonly ExecuteUpdateResultCheckStyle insertCheckStyle;
		private readonly ExecuteUpdateResultCheckStyle updateCheckStyle;
		private readonly ExecuteUpdateResultCheckStyle deleteCheckStyle;
		private readonly ExecuteUpdateResultCheckStyle deleteAllCheckStyle;

		#endregion

		private readonly string[] spaces;

		private readonly Dictionary<string, object> collectionPropertyColumnAliases = new Dictionary<string, object>();
		private readonly Dictionary<string, object> collectionPropertyColumnNames = new Dictionary<string, object>();

		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof (ICollectionPersister));

		public AbstractCollectionPersister(Mapping.Collection collection, ICacheConcurrencyStrategy cache, ISessionFactoryImplementor factory)
		{
			this.factory = factory;
			this.cache = cache;
			if (factory.Settings.IsStructuredCacheEntriesEnabled)
			{
				cacheEntryStructure = collection.IsMap
										? (ICacheEntryStructure) new StructuredMapCacheEntry()
										: (ICacheEntryStructure) new StructuredCollectionCacheEntry();
			}
			else
			{
				cacheEntryStructure = new UnstructuredCacheEntry();
			}

			dialect = factory.Dialect;
			sqlExceptionConverter = factory.SQLExceptionConverter;
			collectionType = collection.CollectionType;
			role = collection.Role;
			entityName = collection.OwnerEntityName;
			ownerPersister = factory.GetEntityPersister(entityName);
			queryLoaderName = collection.LoaderName;
			isMutable = collection.IsMutable;

			Table table = collection.CollectionTable;
			fetchMode = collection.Element.FetchMode;
			elementType = collection.Element.Type;
			isPrimitiveArray = collection.IsPrimitiveArray;
			isArray = collection.IsArray;
			subselectLoadable = collection.IsSubselectLoadable;
			qualifiedTableName = table.GetQualifiedName(dialect, factory.Settings.DefaultCatalogName, factory.Settings.DefaultSchemaName);

			int spacesSize = 1 + collection.SynchronizedTables.Count;
			spaces = new string[spacesSize];
			int ispa = 0;
			spaces[ispa++] = qualifiedTableName;
			foreach (string s in collection.SynchronizedTables)
			{
				spaces[ispa++] = s;
			}

			sqlOrderByString = collection.OrderBy;
			hasOrder = sqlOrderByString != null;
			sqlOrderByStringTemplate = hasOrder
										? Template.RenderOrderByStringTemplate(sqlOrderByString, dialect,
																			   factory.SQLFunctionRegistry)
										: null;
			sqlWhereString = !string.IsNullOrEmpty(collection.Where) ? '(' + collection.Where + ')' : null;
			hasWhere = sqlWhereString != null;
			sqlWhereStringTemplate = hasWhere
										? Template.RenderWhereStringTemplate(sqlWhereString, dialect, factory.SQLFunctionRegistry)
										: null;
			hasOrphanDelete = collection.HasOrphanDelete;
			int batch = collection.BatchSize;
			if (batch == -1)
			{
				batch = factory.Settings.DefaultBatchFetchSize;
			}
			batchSize = batch;

			isVersioned = collection.IsOptimisticLocked;

			keyType = collection.Key.Type;
			int keySpan = collection.Key.ColumnSpan;
			keyColumnNames = new string[keySpan];
			keyColumnAliases = new string[keySpan];
			int k = 0;
			foreach (Column col in collection.Key.ColumnIterator)
			{
				keyColumnNames[k] = col.GetQuotedName(dialect);
				keyColumnAliases[k] = col.GetAlias(dialect, table);
				k++;
			}

			HashSet<string> distinctColumns = new HashSet<string>();
			CheckColumnDuplication(distinctColumns, collection.Key.ColumnIterator);

			#region Element

			IValue element = collection.Element;
			if (!collection.IsOneToMany)
			{
				CheckColumnDuplication(distinctColumns, element.ColumnIterator);
			}

			if (elementType.IsEntityType)
			{
				string _entityName = ((EntityType) elementType).GetAssociatedEntityName();
				elementPersister = factory.GetEntityPersister(_entityName);
				// NativeSQL: collect element column and auto-aliases
			}
			else
			{
				elementPersister = null;
			}

			int elementSpan = element.ColumnSpan;
			elementColumnAliases = new string[elementSpan];
			elementColumnNames = new string[elementSpan];
			elementFormulaTemplates = new string[elementSpan];
			elementFormulas = new string[elementSpan];
			elementColumnIsSettable = new bool[elementSpan];
			elementColumnIsInPrimaryKey = new bool[elementSpan];
			bool isPureFormula = true;
			bool hasNotNullableColumns = false;
			int j = 0;
			foreach (ISelectable selectable in element.ColumnIterator)
			{
				elementColumnAliases[j] = selectable.GetAlias(dialect, table);
				if (selectable.IsFormula)
				{
					Formula form = (Formula) selectable;
					elementFormulaTemplates[j] = form.GetTemplate(dialect, factory.SQLFunctionRegistry);
					elementFormulas[j] = form.FormulaString;
				}
				else
				{
					Column col = (Column) selectable;
					elementColumnNames[j] = col.GetQuotedName(dialect);
					elementColumnIsSettable[j] = true;
					elementColumnIsInPrimaryKey[j] = !col.IsNullable;
					if (!col.IsNullable)
					{
						hasNotNullableColumns = true;
					}

					isPureFormula = false;
				}
				j++;
			}
			elementIsPureFormula = isPureFormula;

			//workaround, for backward compatibility of sets with no
			//not-null columns, assume all columns are used in the
			//row locator SQL
			if (!hasNotNullableColumns)
			{
				ArrayHelper.Fill(elementColumnIsInPrimaryKey, true);
			}

			#endregion

			#region INDEX AND ROW SELECT

			hasIndex = collection.IsIndexed;
			if (hasIndex)
			{
				// NativeSQL: collect index column and auto-aliases
				IndexedCollection indexedCollection = (IndexedCollection) collection;
				indexType = indexedCollection.Index.Type;
				int indexSpan = indexedCollection.Index.ColumnSpan;
				indexColumnNames = new string[indexSpan];
				indexFormulaTemplates = new string[indexSpan];
				indexFormulas = new string[indexSpan];
				indexColumnIsSettable = new bool[indexSpan];
				indexColumnAliases = new string[indexSpan];
				bool hasFormula = false;
				int i = 0;
				foreach (ISelectable selectable in indexedCollection.Index.ColumnIterator)
				{
					indexColumnAliases[i] = selectable.GetAlias(dialect);
					if (selectable.IsFormula)
					{
						Formula indexForm = (Formula) selectable;
						indexFormulaTemplates[i] = indexForm.GetTemplate(dialect, factory.SQLFunctionRegistry);
						indexFormulas[i] = indexForm.FormulaString;
						hasFormula = true;
					}
					else
					{
						Column indexCol = (Column) selectable;
						indexColumnNames[i] = indexCol.GetQuotedName(dialect);
						indexColumnIsSettable[i] = true;
					}
					i++;
				}
				indexContainsFormula = hasFormula;
				baseIndex = indexedCollection.IsList ? ((List) indexedCollection).BaseIndex : 0;

				CheckColumnDuplication(distinctColumns, indexedCollection.Index.ColumnIterator);
			}
			else
			{
				indexContainsFormula = false;
				indexColumnIsSettable = null;
				indexFormulaTemplates = null;
				indexFormulas = null;
				indexType = null;
				indexColumnNames = null;
				indexColumnAliases = null;
				baseIndex = 0;
			}

			hasIdentifier = collection.IsIdentified;
			if (hasIdentifier)
			{
				if (collection.IsOneToMany)
				{
					throw new MappingException("one-to-many collections with identifiers are not supported.");
				}
				IdentifierCollection idColl = (IdentifierCollection) collection;
				identifierType = idColl.Identifier.Type;

				Column col = null;
				foreach (Column column in idColl.Identifier.ColumnIterator)
				{
					col = column;
					break;
				}

				identifierColumnName = col.GetQuotedName(dialect);
				identifierColumnAlias = col.GetAlias(dialect);
				identifierGenerator =
					idColl.Identifier.CreateIdentifierGenerator(factory.Dialect, factory.Settings.DefaultCatalogName,
																factory.Settings.DefaultSchemaName, null);
				// NH see : identityDelegate declaration
				IPostInsertIdentifierGenerator pig = (identifierGenerator as IPostInsertIdentifierGenerator);
				if (pig != null)
				{
					identityDelegate = pig.GetInsertGeneratedIdentifierDelegate(this, Factory, UseGetGeneratedKeys());
				}
				else
				{
					identityDelegate = null;
				}

				CheckColumnDuplication(distinctColumns, idColl.Identifier.ColumnIterator);
			}
			else
			{
				identifierType = null;
				identifierColumnName = null;
				identifierColumnAlias = null;
				identifierGenerator = null;
				identityDelegate = null;
			}

			#endregion

			#region GENERATE THE SQL

			// NH Different behavior : for the Insert SQL we are managing isPostInsertIdentifier (not supported in H3.2.5) 
			if (collection.CustomSQLInsert == null)
			{
				if (!IsIdentifierAssignedByInsert)
				{
					sqlInsertRowString = GenerateInsertRowString();
				}
				else
				{
					sqlInsertRowString = GenerateIdentityInsertRowString();
				}
				insertCallable = false;
				insertCheckStyle = ExecuteUpdateResultCheckStyle.Count;
			}
			else
			{
				SqlType[] parmsTypes = GenerateInsertRowString().ParameterTypes;
				sqlInsertRowString = new SqlCommandInfo(collection.CustomSQLInsert, parmsTypes);
				insertCallable = collection.IsCustomInsertCallable;
				insertCheckStyle = collection.CustomSQLInsertCheckStyle
								   ?? ExecuteUpdateResultCheckStyle.DetermineDefault(collection.CustomSQLInsert, insertCallable);
			}

			sqlUpdateRowString = GenerateUpdateRowString();
			if (collection.CustomSQLUpdate == null)
			{
				updateCallable = false;
				updateCheckStyle = ExecuteUpdateResultCheckStyle.Count;
			}
			else
			{
				sqlUpdateRowString = new SqlCommandInfo(collection.CustomSQLUpdate, sqlUpdateRowString.ParameterTypes);
				updateCallable = collection.IsCustomUpdateCallable;
				updateCheckStyle = collection.CustomSQLUpdateCheckStyle
								   ?? ExecuteUpdateResultCheckStyle.DetermineDefault(collection.CustomSQLUpdate, updateCallable);
			}

			// 6.0 TODO: call GenerateDeleteRowString(null); instead.
#pragma warning disable 618
			sqlDeleteRowString = GenerateDeleteRowString();
#pragma warning restore 618
			if (collection.CustomSQLDelete == null)
			{
				deleteCallable = false;
				deleteCheckStyle = ExecuteUpdateResultCheckStyle.None;
			}
			else
			{
				sqlDeleteRowString = new SqlCommandInfo(collection.CustomSQLDelete, sqlDeleteRowString.ParameterTypes);
				deleteCallable = collection.IsCustomDeleteCallable;
				deleteCheckStyle = ExecuteUpdateResultCheckStyle.None;
			}

			sqlDeleteString = GenerateDeleteString();
			if (collection.CustomSQLDeleteAll == null)
			{
				deleteAllCallable = false;
				deleteAllCheckStyle = ExecuteUpdateResultCheckStyle.None;
			}
			else
			{
				sqlDeleteString = new SqlCommandInfo(collection.CustomSQLDeleteAll, sqlDeleteString.ParameterTypes);
				deleteAllCallable = collection.IsCustomDeleteAllCallable;
				deleteAllCheckStyle = ExecuteUpdateResultCheckStyle.None;
			}

			isCollectionIntegerIndex = collection.IsIndexed && !collection.IsMap;
			sqlDetectRowByIndexString = GenerateDetectRowByIndexString();
			sqlDetectRowByElementString = GenerateDetectRowByElementString();
			sqlSelectRowByIndexString = GenerateSelectRowByIndexString();

			LogStaticSQL();

			#endregion

			isLazy = collection.IsLazy;
			isExtraLazy = collection.ExtraLazy;
			isInverse = collection.IsInverse;

			if (collection.IsArray)
			{
				elementClass = ((Array) collection).ElementClass;
			}
			else
			{
				// for non-arrays, we don't need to know the element class
				elementClass = null;
			}

			if (elementType.IsComponentType)
			{
				elementPropertyMapping =
					new CompositeElementPropertyMapping(elementColumnNames, elementFormulaTemplates,
														(IAbstractComponentType) elementType, factory);
			}
			else if (!elementType.IsEntityType)
			{
				elementPropertyMapping = new ElementPropertyMapping(elementColumnNames, elementType);
			}
			else
			{
				elementPropertyMapping = elementPersister as IPropertyMapping;
				if (elementPropertyMapping == null)
				{
					elementPropertyMapping = new ElementPropertyMapping(elementColumnNames, elementType);
				}
			}

			// Handle any filters applied to this collection
			filterHelper = new FilterHelper(collection.FilterMap, dialect, factory.SQLFunctionRegistry);

			// Handle any filters applied to this collection for many-to-many
			manyToManyFilterHelper = new FilterHelper(collection.ManyToManyFilterMap, dialect, factory.SQLFunctionRegistry);
			manyToManyWhereString = !string.IsNullOrEmpty(collection.ManyToManyWhere)
										? "( " + collection.ManyToManyWhere + " )"
										: null;
			manyToManyWhereTemplate = manyToManyWhereString == null
										? null
										: Template.RenderWhereStringTemplate(manyToManyWhereString, factory.Dialect,
																			 factory.SQLFunctionRegistry);
			manyToManyOrderByString = collection.ManyToManyOrdering;
			manyToManyOrderByTemplate = manyToManyOrderByString == null
											? null
											: Template.RenderOrderByStringTemplate(manyToManyOrderByString, factory.Dialect,
																				   factory.SQLFunctionRegistry);
			InitCollectionPropertyMap();
		}

		public void PostInstantiate()
		{
			initializer = queryLoaderName == null
							? CreateCollectionInitializer(CollectionHelper.EmptyDictionary<string, IFilter>())
							: new NamedQueryCollectionInitializer(queryLoaderName, this);
		}

		protected void LogStaticSQL()
		{
			if (log.IsDebugEnabled())
			{
				log.Debug("Static SQL for collection: {0}", Role);
				if (SqlInsertRowString != null)
				{
					log.Debug(" Row insert: {0}", SqlInsertRowString.Text);
				}

				if (SqlUpdateRowString != null)
				{
					log.Debug(" Row update: {0}", SqlUpdateRowString.Text);
				}

				if (SqlDeleteRowString != null)
				{
					log.Debug(" Row delete: {0}", SqlDeleteRowString.Text);
				}

				if (SqlDeleteString != null)
				{
					log.Debug(" One-shot delete: {0}", SqlDeleteString.Text);
				}
			}
		}

		public void Initialize(object key, ISessionImplementor session)
		{
			GetAppropriateInitializer(key, session).Initialize(key, session);
		}

		protected ICollectionInitializer GetAppropriateInitializer(object key, ISessionImplementor session)
		{
			if (queryLoaderName != null)
			{
				//if there is a user-specified loader, return that
				//TODO: filters!?
				return initializer;
			}

			ICollectionInitializer subselectInitializer = GetSubselectInitializer(key, session);
			if (subselectInitializer != null)
			{
				return subselectInitializer;
			}
			else if (session.EnabledFilters.Count == 0)
			{
				return initializer;
			}
			else
			{
				return CreateCollectionInitializer(session.EnabledFilters);
			}
		}

		private ICollectionInitializer GetSubselectInitializer(object key, ISessionImplementor session)
		{
			if (!IsSubselectLoadable)
			{
				return null;
			}

			IPersistenceContext persistenceContext = session.PersistenceContext;

			SubselectFetch subselect =
				persistenceContext.BatchFetchQueue.GetSubselect(session.GenerateEntityKey(key, OwnerEntityPersister));

			if (subselect == null)
			{
				return null;
			}
			else
			{
				// Take care of any entities that might have
				// been evicted!
				List<EntityKey> keysToRemove = subselect.Result
					.Where(entityKey => !persistenceContext.ContainsEntity(entityKey)).ToList();

				foreach (var entityKey in keysToRemove)
					subselect.Result.Remove(entityKey);

				// Run a subquery loader
				return CreateSubselectInitializer(subselect, session);
			}
		}

		protected abstract ICollectionInitializer CreateSubselectInitializer(SubselectFetch subselect,
																			 ISessionImplementor session);

		protected abstract ICollectionInitializer CreateCollectionInitializer(IDictionary<string, IFilter> enabledFilters);

		public bool HasCache
		{
			get { return cache != null; }
		}

		public string GetSQLWhereString(string alias)
		{
			return Template.ReplacePlaceholder(sqlWhereStringTemplate, alias);
		}

		public string GetSQLOrderByString(string alias)
		{
			return HasOrdering ? Template.ReplacePlaceholder(sqlOrderByStringTemplate, alias) : string.Empty;
		}

		public string GetManyToManyOrderByString(string alias)
		{
			if (IsManyToMany && manyToManyOrderByString != null)
			{
				return Template.ReplacePlaceholder(manyToManyOrderByTemplate, alias);
			}
			else
			{
				return string.Empty;
			}
		}

		public bool HasOrdering
		{
			get { return hasOrder; }
		}

		public bool HasManyToManyOrdering
		{
			get { return IsManyToMany && manyToManyOrderByTemplate != null; }
		}

		public bool HasWhere
		{
			get { return hasWhere; }
		}

		/// <summary>
		/// Reads the Element from the DbDataReader.  The DbDataReader will probably only contain
		/// the id of the Element.
		/// </summary>
		/// <remarks>See ReadElementIdentifier for an explanation of why this method will be depreciated.</remarks>
		public object ReadElement(DbDataReader rs, object owner, string[] aliases, ISessionImplementor session)
		{
			return ElementType.NullSafeGet(rs, aliases, session, owner);
		}

		public object ReadIndex(DbDataReader rs, string[] aliases, ISessionImplementor session)
		{
			object index = IndexType.NullSafeGet(rs, aliases, session, null);
			if (index == null)
			{
				throw new HibernateException("null index column for collection: " + role);
			}
			index = DecrementIndexByBase(index);
			return index;
		}

		public object DecrementIndexByBase(object index)
		{
			if (baseIndex != 0)
			{
				index = (int) index - baseIndex;
			}

			return index;
		}

		public object ReadIdentifier(DbDataReader rs, string alias, ISessionImplementor session)
		{
			object id = IdentifierType.NullSafeGet(rs, alias, session, null);
			if (id == null)
			{
				throw new HibernateException("null identifier column for collection: " + role);
			}

			return id;
		}

		public object ReadKey(DbDataReader dr, string[] aliases, ISessionImplementor session)
		{
			return KeyType.NullSafeGet(dr, aliases, session, null);
		}

		protected int WriteKey(DbCommand st, object id, int i, ISessionImplementor session)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id", "Null key for collection: " + role);
			}

			KeyType.NullSafeSet(st, id, i, session);
			return i + keyColumnAliases.Length;
		}

		protected int WriteElement(DbCommand st, object elt, int i, ISessionImplementor session)
		{
			ElementType.NullSafeSet(st, elt, i, elementColumnIsSettable, session);
			return i + ArrayHelper.CountTrue(elementColumnIsSettable);
		}

		protected int WriteIndex(DbCommand st, object idx, int i, ISessionImplementor session)
		{
			IndexType.NullSafeSet(st, IncrementIndexByBase(idx), i, indexColumnIsSettable, session);
			return i + ArrayHelper.CountTrue(indexColumnIsSettable);
		}

		protected object IncrementIndexByBase(object index)
		{
			if (baseIndex != 0)
			{
				index = (int) index + baseIndex;
			}

			return index;
		}

		protected int WriteElementToWhere(DbCommand st, object elt, bool[] columnNullness, int i, ISessionImplementor session)
		{
			if (elementIsPureFormula)
			{
				throw new AssertionFailure("cannot use a formula-based element in the where condition");
			}

			var settable = Combine(elementColumnIsInPrimaryKey, columnNullness);

			ElementType.NullSafeSet(st, elt, i, settable, session);
			return i + settable.Count(s => s);
		}

		// Since v5.2
		[Obsolete("Use overload with columnNullness instead")]
		protected int WriteElementToWhere(DbCommand st, object elt, int i, ISessionImplementor session)
		{
			return WriteElementToWhere(st, elt, null, i, session);
		}

		/// <summary>
		/// Combine arrays indicating settability and nullness of columns into one, considering null columns as not
		/// settable.
		/// </summary>
		/// <param name="settable">Settable columns. <see langword="null"/> will consider them as all settable.</param>
		/// <param name="columnNullness">Nullness of columns.  <see langword="null"/> will consider them as all
		/// non-null. <see langword="true" /> indicates a non-null column, <see langword="false" /> indicates a null
		/// column.</param>
		/// <returns>The resulting settability of columns, or <see langword="null"/> if both argument are
		/// <see langword="null"/>.</returns>
		/// <exception cref="InvalidOperationException">thrown if <paramref name="settable"/> and
		/// <paramref name="columnNullness"/> have inconsistent lengthes.</exception>
		protected static bool[] Combine(bool[] settable, bool[] columnNullness)
		{
			if (columnNullness == null)
				return settable;
			if (settable == null)
				return columnNullness;

			if (columnNullness.Length != settable.Length)
				throw new InvalidOperationException("Inconsistent nullness and settable columns lengthes");

			var result = new bool[settable.Length];
			for (var idx = 0; idx < settable.Length; idx++)
			{
				result[idx] = columnNullness[idx] && settable[idx];
			}

			return result;
		}

		// No column nullness handling here: although a composite index could have null columns, the mapping
		// current implementation forbirds this by forcing not-null to true on all columns.
		protected int WriteIndexToWhere(DbCommand st, object index, int i, ISessionImplementor session)
		{
			if (indexContainsFormula)
			{
				throw new AssertionFailure("cannot use a formula-based index in the where condition");
			}

			IndexType.NullSafeSet(st, IncrementIndexByBase(index), i, session);
			return i + indexColumnAliases.Length;
		}

		protected int WriteIdentifier(DbCommand st, object idx, int i, ISessionImplementor session)
		{
			IdentifierType.NullSafeSet(st, idx, i, session);
			return i + 1;
		}

		public string[] GetKeyColumnAliases(string suffix)
		{
			return new Alias(suffix).ToAliasStrings(keyColumnAliases, dialect);
		}

		public string[] GetElementColumnAliases(string suffix)
		{
			return new Alias(suffix).ToAliasStrings(elementColumnAliases, dialect);
		}

		public string[] GetIndexColumnAliases(string suffix)
		{
			if (hasIndex)
			{
				return new Alias(suffix).ToAliasStrings(indexColumnAliases, dialect);
			}
			else
			{
				return null;
			}
		}

		public string GetIdentifierColumnAlias(string suffix)
		{
			if (hasIdentifier)
			{
				return new Alias(suffix).ToAliasString(identifierColumnAlias, dialect);
			}
			else
			{
				return null;
			}
		}

		public string SelectFragment(string alias, string columnSuffix)
		{
			SelectFragment frag = GenerateSelectFragment(alias, columnSuffix);
			AppendElementColumns(frag, alias);
			AppendIndexColumns(frag, alias);
			AppendIdentifierColumns(frag, alias);

			return frag.ToSqlStringFragment(false);
		}

		private void AddWhereFragment(SqlSimpleSelectBuilder sql)
		{
			if (!hasWhere)
				return;
			sql.AddWhereFragment(sqlWhereString);
		}

		private SqlString GenerateSelectSizeString(ISessionImplementor sessionImplementor)
		{
			var selectValue = GetCountSqlSelectClause();

			return
				new SqlSimpleSelectBuilder(dialect, factory)
					.SetTableName(TableName)
					.AddWhereFragment(KeyColumnNames, KeyType, "=")
					.AddWhereFragment(FilterFragment(TableName, sessionImplementor.EnabledFilters))
					.AddColumn(selectValue)
					.ToSqlString();
		}

		protected virtual string GetCountSqlSelectClause()
		{
			// NH: too many "if" when each collection can have its persister
			if (isCollectionIntegerIndex) return string.Format("coalesce(max({0}) + 1, 0)", IndexColumnNames[0]); // Do we need this "optimization"?
			return string.Format("count({0})", HasIndex ? GetIndexCountExpression() : ElementColumnNames[0]);
		}

		private string GetIndexCountExpression()
		{
			// when the index has at least one column then use that column to perform the count, otherwise it will use the formula.
			return IndexColumnNames[0] ?? IndexFormulas[0];
		}

		private SqlString GenerateDetectRowByIndexString()
		{
			if (!hasIndex)
			{
				return null;
			}

			// TODO NH: may be we need something else when Index is mixed with Formula
			var builder =
				new SqlSimpleSelectBuilder(dialect, factory)
					.SetTableName(TableName)
					.AddWhereFragment(KeyColumnNames, KeyType, "=")
					.AddWhereFragment(IndexColumnNames, IndexType, "=")
					.AddWhereFragment(indexFormulas, IndexType, "=")
					.AddColumn("1");
			AddWhereFragment(builder);
			return builder.ToSqlString();
		}

		private SqlString GenerateSelectRowByIndexString()
		{
			if (!hasIndex)
			{
				return null;
			}

			var builder =
				new SqlSimpleSelectBuilder(dialect, factory)
					.SetTableName(TableName)
					.AddWhereFragment(KeyColumnNames, KeyType, "=")
					.AddWhereFragment(IndexColumnNames, IndexType, "=")
					.AddWhereFragment(indexFormulas, IndexType, "=")
					.AddColumns(ElementColumnNames, elementColumnAliases)
					.AddColumns(indexFormulas, indexColumnAliases);
			AddWhereFragment(builder);
			return builder.ToSqlString();
		}

		private SqlString GenerateDetectRowByElementString()
		{
			var builder =
				new SqlSimpleSelectBuilder(dialect, factory)
					.SetTableName(TableName)
					.AddWhereFragment(KeyColumnNames, KeyType, "=")
					.AddWhereFragment(ElementColumnNames, ElementType, "=")
					.AddWhereFragment(elementFormulas, ElementType, "=")
					.AddColumn("1");
			AddWhereFragment(builder);
			return builder.ToSqlString();
		}

		protected virtual SelectFragment GenerateSelectFragment(string alias, string columnSuffix)
		{
			return new SelectFragment(dialect).SetSuffix(columnSuffix).AddColumns(alias, keyColumnNames, keyColumnAliases);
		}

		protected virtual void AppendElementColumns(SelectFragment frag, string elemAlias)
		{
			for (int i = 0; i < elementColumnIsSettable.Length; i++)
			{
				if (elementColumnIsSettable[i])
				{
					frag.AddColumn(elemAlias, elementColumnNames[i], elementColumnAliases[i]);
				}
				else
				{
					frag.AddFormula(elemAlias, elementFormulaTemplates[i], elementColumnAliases[i]);
				}
			}
		}

		protected virtual void AppendIndexColumns(SelectFragment frag, string alias)
		{
			if (hasIndex)
			{
				for (int i = 0; i < indexColumnIsSettable.Length; i++)
				{
					if (indexColumnIsSettable[i])
					{
						frag.AddColumn(alias, indexColumnNames[i], indexColumnAliases[i]);
					}
					else
					{
						frag.AddFormula(alias, indexFormulaTemplates[i], indexColumnAliases[i]);
					}
				}
			}
		}

		protected virtual void AppendIdentifierColumns(SelectFragment frag, string alias)
		{
			if (hasIdentifier)
			{
				frag.AddColumn(alias, identifierColumnName, identifierColumnAlias);
			}
		}

		public string[] IndexColumnNames
		{
			get { return indexColumnNames; }
		}

		public string[] GetIndexColumnNames(string alias)
		{
			return Qualify(alias, indexColumnNames, indexFormulaTemplates);
		}

		public string[] GetElementColumnNames(string alias)
		{
			return Qualify(alias, elementColumnNames, elementFormulaTemplates);
		}

		private static string[] Qualify(string alias, string[] columnNames, string[] formulaTemplates)
		{
			int span = columnNames.Length;
			string[] result = new string[span];
			for (int i = 0; i < span; i++)
			{
				if (columnNames[i] == null)
				{
					result[i] = Template.ReplacePlaceholder(formulaTemplates[i], alias);
				}
				else
				{
					result[i] = StringHelper.Qualify(alias, columnNames[i]);
				}
			}
			return result;
		}

		public string[] ElementColumnNames
		{
			get { return elementColumnNames; }
		}

		public bool HasIndex
		{
			get { return hasIndex; }
		}

		public virtual string TableName
		{
			get { return qualifiedTableName; }
		}

		public void Remove(object id, ISessionImplementor session)
		{
			if (!isInverse && RowDeleteEnabled)
			{
				if (log.IsDebugEnabled())
				{
					log.Debug("Deleting collection: {0}", MessageHelper.CollectionInfoString(this, id, Factory));
				}

				// Remove all the old entries
				try
				{
					int offset = 0;
					IExpectation expectation = Expectations.AppropriateExpectation(DeleteAllCheckStyle);
					//bool callable = DeleteAllCallable;
					bool useBatch = expectation.CanBeBatched;
					var st = useBatch
						? session.Batcher.PrepareBatchCommand(SqlDeleteString.CommandType, SqlDeleteString.Text, SqlDeleteString.ParameterTypes)
						: session.Batcher.PrepareCommand(SqlDeleteString.CommandType, SqlDeleteString.Text, SqlDeleteString.ParameterTypes);

					try
					{
						//offset += expectation.Prepare(st, factory.ConnectionProvider.Driver);
						WriteKey(st, id, offset, session);
						if (useBatch)
						{
							session.Batcher.AddToBatch(expectation);
						}
						else
						{
							expectation.VerifyOutcomeNonBatched(session.Batcher.ExecuteNonQuery(st), st);
						}
					}
					catch (Exception e)
					{
						if (useBatch)
						{
							session.Batcher.AbortBatch(e);
						}
						throw;
					}
					finally
					{
						if (!useBatch)
						{
							session.Batcher.CloseCommand(st, null);
						}
					}

					if (log.IsDebugEnabled())
					{
						log.Debug("done deleting collection");
					}
				}
				catch (DbException sqle)
				{
					throw ADOExceptionHelper.Convert(sqlExceptionConverter, sqle,
													 "could not delete collection: " + MessageHelper.CollectionInfoString(this, id));
				}
			}
		}

		public void Recreate(IPersistentCollection collection, object id, ISessionImplementor session)
		{
			if (!isInverse && RowInsertEnabled)
			{
				if (log.IsDebugEnabled())
				{
					log.Debug("Inserting collection: {0}", MessageHelper.CollectionInfoString(this, collection, id, session));
				}

				try
				{
					IExpectation expectation = null;
					bool useBatch = false;
					int i = 0;
					int count = 0;

					// create all the new entries
					foreach (var entry in collection.Entries(this))
					{
						// Init, if we're on the first element.
						if (count == 0)
						{
							expectation = Expectations.AppropriateExpectation(insertCheckStyle);
							collection.PreInsert(this);
							//bool callable = InsertCallable;
							useBatch = expectation.CanBeBatched;
						}
						
						if (collection.EntryExists(entry, i))
						{
							object entryId;
							if (!IsIdentifierAssignedByInsert)
							{
								// NH Different implementation: write once
								entryId = PerformInsert(id, collection, expectation, entry, i, useBatch, false, session);
							}
							else
							{
								entryId = PerformInsert(id, collection, entry, i, session);
							}
							collection.AfterRowInsert(this, entry, i, entryId);
							count++;
						}
						i++;
					}

					if (log.IsDebugEnabled())
					{
						if (count > 0)
							log.Debug("done inserting collection: {0} rows inserted", count);
						else
							log.Debug("collection was empty");
					}
				}
				catch (DbException sqle)
				{
					throw ADOExceptionHelper.Convert(sqlExceptionConverter, sqle,
													 "could not insert collection: " + MessageHelper.CollectionInfoString(this, collection, id, session));
				}
			}
		}

		public void DeleteRows(IPersistentCollection collection, object id, ISessionImplementor session)
		{
			if (!isInverse && RowDeleteEnabled)
			{
				if (log.IsDebugEnabled())
				{
					log.Debug("Deleting rows of collection: {0}", MessageHelper.CollectionInfoString(this, collection, id, session));
				}

				bool deleteByIndex = !IsOneToMany && hasIndex && !indexContainsFormula;

				try
				{
					// delete all the deleted entries
					var offset = 0;
					var count = 0;

					foreach (var entry in collection.GetDeletes(this, !deleteByIndex))
					{
						DbCommand st;
						var expectation = Expectations.AppropriateExpectation(deleteCheckStyle);
						//var callable = DeleteCallable;
						var commandInfo = GetDeleteCommand(deleteByIndex, entry, out var columnNullness);

						var useBatch = expectation.CanBeBatched;
						if (useBatch)
						{
							st = session.Batcher.PrepareBatchCommand(
								commandInfo.CommandType, commandInfo.Text, commandInfo.ParameterTypes);
						}
						else
						{
							st = session.Batcher.PrepareCommand(
								commandInfo.CommandType, commandInfo.Text, commandInfo.ParameterTypes);
						}
						try
						{
							var loc = offset;
							if (hasIdentifier)
							{
								WriteIdentifier(st, entry, loc, session);
							}
							else
							{
								loc = WriteKey(st, id, loc, session);

								if (deleteByIndex)
								{
									WriteIndexToWhere(st, entry, loc, session);
								}
								else
								{
									WriteElementToWhere(st, entry, columnNullness, loc, session);
								}
							}
							if (useBatch)
							{
								session.Batcher.AddToBatch(expectation);
							}
							else
							{
								expectation.VerifyOutcomeNonBatched(session.Batcher.ExecuteNonQuery(st), st);
							}
							count++;
						}
						catch (Exception e)
						{
							if (useBatch)
							{
								session.Batcher.AbortBatch(e);
							}
							throw;
						}
						finally
						{
							if (!useBatch)
							{
								session.Batcher.CloseCommand(st, null);
							}
						}
					}

					if (log.IsDebugEnabled())
					{
						if (count > 0)
							log.Debug("done deleting collection rows: {0} deleted", count);
						else
							log.Debug("no rows to delete");
					}
				}
				catch (DbException sqle)
				{
					throw ADOExceptionHelper.Convert(sqlExceptionConverter, sqle,
						"could not delete collection rows: " + MessageHelper.CollectionInfoString(this, collection, id, session));
				}
			}
		}

		private SqlCommandInfo GetDeleteCommand(bool deleteByIndex, object entry, out bool[] columnNullness)
		{
			var commandInfo = SqlDeleteRowString;
			columnNullness = null;
			// No column nullness handling if deleteByIndex: although a composite index could have null columns, the
			// mapping current implementation forbirds this by forcing not-null to true on all columns.
			if (!hasIdentifier && !deleteByIndex)
			{
				columnNullness = ElementType.ToColumnNullness(entry, Factory);
				if (columnNullness.Any(cn => !cn))
					commandInfo = sqlDeleteRowStringByNullness.GetOrAdd(
						columnNullness,
						GenerateDeleteRowString);
				else
					columnNullness = null;
			}

			return commandInfo;
		}

		public void InsertRows(IPersistentCollection collection, object id, ISessionImplementor session)
		{
			if (!isInverse && RowInsertEnabled)
			{
				if (log.IsDebugEnabled())
				{
					log.Debug("Inserting rows of collection: {0}", MessageHelper.CollectionInfoString(this, collection, id, session));
				}

				try
				{
					// insert all the new entries
					collection.PreInsert(this);
					IExpectation expectation = Expectations.AppropriateExpectation(insertCheckStyle);
					//bool callable = InsertCallable;
					bool useBatch = expectation.CanBeBatched;
					int i = 0;
					int count = 0;

					IEnumerable entries = collection.Entries(this);
					foreach (object entry in entries)
					{
						if (collection.NeedsInserting(entry, i, elementType))
						{
							object entryId;
							if (!IsIdentifierAssignedByInsert)
							{
								// NH Different implementation: write once
								entryId = PerformInsert(id, collection, expectation, entry, i, useBatch, false, session);
							}
							else
							{
								entryId = PerformInsert(id, collection, entry, i, session);
							}
							collection.AfterRowInsert(this, entry, i, entryId);
							count++;
						}
						i++;
					}

					if (log.IsDebugEnabled())
					{
						log.Debug("done inserting rows: {0} inserted", count);
					}
				}
				catch (DbException sqle)
				{
					throw ADOExceptionHelper.Convert(sqlExceptionConverter, sqle,
													 "could not insert collection rows: " + MessageHelper.CollectionInfoString(this, collection, id, session));
				}
			}
		}

		public bool HasOrphanDelete
		{
			get { return hasOrphanDelete; }
		}

		public IType ToType(string propertyName)
		{
			if ("index".Equals(propertyName))
			{
				return indexType;
			}

			return elementPropertyMapping.ToType(propertyName);
		}

		public bool TryToType(string propertyName, out IType type)
		{
			if ("index".Equals(propertyName))
			{
				type = indexType;
				return true;
			}
			else
			{
				return elementPropertyMapping.TryToType(propertyName, out type);
			}
		}

		public string GetManyToManyFilterFragment(string alias, IDictionary<string, IFilter> enabledFilters)
		{
			StringBuilder buffer = new StringBuilder();
			manyToManyFilterHelper.Render(buffer, alias, enabledFilters);

			if (manyToManyWhereString != null)
			{
				buffer.Append(" and ").Append(Template.ReplacePlaceholder(manyToManyWhereTemplate, alias));
			}

			return buffer.ToString();
		}
		
		public bool IsManyToManyFiltered(IDictionary<string, IFilter> enabledFilters)
		{
			return IsManyToMany && (manyToManyWhereString != null || manyToManyFilterHelper.IsAffectedBy(enabledFilters));
		}

		public string[] ToColumns(string alias, string propertyName, bool useLastIndex = false)
		{
			if ("index".Equals(propertyName))
			{
				if (IsManyToMany)
				{
					throw new QueryException("index() function not supported for many-to-many association");
				}

				return StringHelper.Qualify(alias, indexColumnNames);
			}
			return elementPropertyMapping.ToColumns(alias, propertyName, useLastIndex);
		}

		public string[] ToColumns(string propertyName, bool useLastIndex = false)
		{
			if ("index".Equals(propertyName))
			{
				if (IsManyToMany)
				{
					throw new QueryException("index() function not supported for many-to-many association");
				}

				return indexColumnNames;
			}

			return elementPropertyMapping.ToColumns(propertyName, useLastIndex);
		}

		protected abstract SqlCommandInfo GenerateDeleteString();
		// No column nullness handling here: updates currently only occur on cases not allowing null.
		protected abstract SqlCommandInfo GenerateUpdateRowString();
		protected abstract SqlCommandInfo GenerateInsertRowString();
		protected abstract SqlCommandInfo GenerateIdentityInsertRowString();

		/// <summary>
		/// Generate the SQL <c>delete</c> that deletes a particular row.
		/// </summary>
		/// <returns>A SQL <c>delete</c>.</returns>
		// Since v5.2
		[Obsolete("Use or override overload with columnNullness instead")]
		protected virtual SqlCommandInfo GenerateDeleteRowString()
		{
			return GenerateDeleteRowString(null);
		}

		/// <summary>
		/// Generate the SQL <c>delete</c> that deletes a particular row.
		/// </summary>
		/// <param name="columnNullness">If non-null, an array of boolean indicating which mapped columns of the index
		/// or element would be null. <see langword="true" /> indicates a non-null column, <see langword="false" />
		/// indicates a null column.</param>
		/// <returns>A SQL <c>delete</c>.</returns>
		// 6.0 TODO: make abstract
		protected virtual SqlCommandInfo GenerateDeleteRowString(bool[] columnNullness)
		{
			throw new NotSupportedException($"{GetType().FullName} does not support queries handling nullness.");
		}

		public void UpdateRows(IPersistentCollection collection, object id, ISessionImplementor session)
		{
			if (!isInverse && collection.RowUpdatePossible)
			{
				if (log.IsDebugEnabled())
				{
					log.Debug("Updating rows of collection: {0}#{1}", role, id);
				}

				// update all the modified entries
				int count = DoUpdateRows(id, collection, session);

				if (log.IsDebugEnabled())
				{
					log.Debug("done updating rows: {0} updated", count);
				}
			}
		}

		protected abstract int DoUpdateRows(object key, IPersistentCollection collection, ISessionImplementor session);

		protected virtual string FilterFragment(string alias)
		{
			return HasWhere ? " and " + GetSQLWhereString(alias) : "";
		}

		public virtual string FilterFragment(string alias, IDictionary<string, IFilter> enabledFilters)
		{
			StringBuilder sessionFilterFragment = new StringBuilder();
			filterHelper.Render(sessionFilterFragment, alias, enabledFilters);

			return sessionFilterFragment.Append(FilterFragment(alias)).ToString();
		}

		public string OneToManyFilterFragment(string alias)
		{
			return string.Empty;
		}

		public override string ToString()
		{
			// Java has StringHelper.root( getClass().getName() ) instead of GetType().Name,
			// but it doesn't make sense to me.
			return string.Format("{0}({1})", GetType().Name, role);
		}

		public bool IsAffectedByEnabledFilters(ISessionImplementor session)
		{
			return
				filterHelper.IsAffectedBy(session.EnabledFilters)
				|| (IsManyToMany && manyToManyFilterHelper.IsAffectedBy(session.EnabledFilters));
		}

		public string[] GetCollectionPropertyColumnAliases(string propertyName, string suffix)
		{
			object aliases;
			if (!collectionPropertyColumnAliases.TryGetValue(propertyName, out aliases))
			{
				return null;
			}
			var rawAliases = (string[]) aliases;

			var result = new string[rawAliases.Length];
			for (int i = 0; i < rawAliases.Length; i++)
			{
				result[i] = new Alias(suffix).ToUnquotedAliasString(rawAliases[i], dialect);
			}
			return result;
		}

		public void InitCollectionPropertyMap()
		{
			InitCollectionPropertyMap(CollectionPersister.PropKey, keyType, keyColumnAliases, keyColumnNames);
			InitCollectionPropertyMap(CollectionPersister.PropElement, elementType, elementColumnAliases, elementColumnNames);

			if (hasIndex)
			{
				InitCollectionPropertyMap(CollectionPersister.PropIndex, indexType, indexColumnAliases, indexColumnNames);
			}

			if (hasIdentifier)
			{
				InitCollectionPropertyMap(CollectionPersister.PropId, identifierType, new string[] {identifierColumnAlias},
										  new string[] {identifierColumnName});
			}
		}

		private void InitCollectionPropertyMap(string aliasName, IType type, string[] columnAliases, string[] columnNames)
		{
			collectionPropertyColumnAliases[aliasName] = columnAliases;
			collectionPropertyColumnNames[aliasName] = columnNames;

			if (type.IsComponentType)
			{
				// NH-1612+NH-1691: Recursively add column aliases for nested components to support the selection
				// of individual component properties in native SQL queries. This also seems to provide
				// a more complete solution to HHH-1019 (http://opensource.atlassian.com/projects/hibernate/browse/HHH-1019)
				// because it works for <load-collection> and <return-join>.
				int columnIndex = 0;

				var ct = (IAbstractComponentType) type;
				string[] propertyNames = ct.PropertyNames;
				for (int propertyIndex = 0; propertyIndex < propertyNames.Length; propertyIndex++)
				{
					string name = propertyNames[propertyIndex];
					IType propertyType = ct.Subtypes[propertyIndex];

					int propertyColSpan = 0;
					CalcPropertyColumnSpan(propertyType, ref propertyColSpan);

					var propertyColumnAliases = new string[propertyColSpan];
					var propertyColumnNames = new string[propertyColSpan];
					System.Array.Copy(columnAliases, columnIndex, propertyColumnAliases, 0, propertyColSpan);
					System.Array.Copy(columnNames, columnIndex, propertyColumnNames, 0, propertyColSpan);
					InitCollectionPropertyMap(aliasName + "." + name, propertyType, propertyColumnAliases, propertyColumnNames);

					columnIndex += propertyColSpan;
				}
			}
		}

		private static void CalcPropertyColumnSpan(IType propertyType, ref int count)
		{
			if (!propertyType.IsComponentType)
			{
				count++;
			}
			else
			{
				var componentType = (IAbstractComponentType) propertyType;
				foreach (var subtype in componentType.Subtypes)
				{
					CalcPropertyColumnSpan(subtype, ref count);
				}
			}
		}

		public int GetSize(object key, ISessionImplementor session)
		{
			using (session.BeginProcess())
			try
			{
				if(session.EnabledFilters.Count > 0)
				{
					// The above call validates the filters
				}

				var st = session.Batcher.PrepareCommand(CommandType.Text, GenerateSelectSizeString(session), KeyType.SqlTypes(factory));
				DbDataReader rs = null;
				try
				{
					KeyType.NullSafeSet(st, key, 0, session);
					rs = session.Batcher.ExecuteReader(st);
					return rs.Read() ? Convert.ToInt32(rs.GetValue(0)) - baseIndex : 0;
				}
				finally
				{
					session.Batcher.CloseCommand(st, rs);
				}
			}
			catch (DbException sqle)
			{
				throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, sqle,
												 "could not retrieve collection size: "
												 + MessageHelper.CollectionInfoString(this, key, Factory), GenerateSelectSizeString(session));
			}
		}

		public bool IndexExists(object key, object index, ISessionImplementor session)
		{
			return Exists(key, IncrementIndexByBase(index), IndexType, sqlDetectRowByIndexString, session);
		}

		public bool ElementExists(object key, object element, ISessionImplementor session)
		{
			return Exists(key, element, ElementType, sqlDetectRowByElementString, session);
		}

		private bool Exists(object key, object indexOrElement, IType indexOrElementType, SqlString sql,
							ISessionImplementor session)
		{
			using (session.BeginProcess())
			try
			{
				List<SqlType> sqlTl = new List<SqlType>(KeyType.SqlTypes(factory));
				sqlTl.AddRange(indexOrElementType.SqlTypes(factory));
				var st = session.Batcher.PrepareCommand(CommandType.Text, sql, sqlTl.ToArray());
				DbDataReader rs = null;
				try
				{
					KeyType.NullSafeSet(st, key, 0, session);
					indexOrElementType.NullSafeSet(st, indexOrElement, keyColumnNames.Length, session);
					rs = session.Batcher.ExecuteReader(st);
					try
					{
						return rs.Read();
					}
					finally
					{
						rs.Close();
					}
				}
				catch (TransientObjectException)
				{
					return false;
				}
				finally
				{
					session.Batcher.CloseCommand(st, rs);
				}
			}
			catch (DbException sqle)
			{
				throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, sqle,
												 "could not check row existence: " + MessageHelper.CollectionInfoString(this, key, Factory),
												 GenerateSelectSizeString(session));
			}
		}

		public virtual object GetElementByIndex(object key, object index, ISessionImplementor session, object owner)
		{
			using (session.BeginProcess())
			try
			{
				List<SqlType> sqlTl = new List<SqlType>(KeyType.SqlTypes(factory));
				sqlTl.AddRange(IndexType.SqlTypes(factory));
				var st = session.Batcher.PrepareCommand(CommandType.Text, sqlSelectRowByIndexString, sqlTl.ToArray());
				DbDataReader rs = null;
				try
				{
					KeyType.NullSafeSet(st, key, 0, session);
					IndexType.NullSafeSet(st, IncrementIndexByBase(index), keyColumnNames.Length, session);
					rs = session.Batcher.ExecuteReader(st);
					try
					{
						if (rs.Read())
						{
							return ElementType.NullSafeGet(rs, elementColumnAliases, session, owner);
						}
						else
						{
							return NotFoundObject;
						}
					}
					finally
					{
						rs.Close();
					}
				}
				finally
				{
					session.Batcher.CloseCommand(st, rs);
				}
			}
			catch (DbException sqle)
			{
				throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, sqle,
												 "could not read row: " + MessageHelper.CollectionInfoString(this, key, Factory),
												 GenerateSelectSizeString(session));
			}
		}

		public object NotFoundObject
		{
			get { return NotFoundPlaceHolder; }
		}

		public abstract bool ConsumesEntityAlias();

		public abstract SqlString FromJoinFragment(string alias, bool innerJoin, bool includeSubclasses);

		public abstract SqlString WhereJoinFragment(string alias, bool innerJoin, bool includeSubclasses);

		// 6.0 TODO: Remove
		// Since v5.2
		[Obsolete("Please use overload taking EntityLoadInfo")]
		public virtual string SelectFragment(
			IJoinable rhs,
			string rhsAlias,
			string lhsAlias,
			string currentEntitySuffix,
			string currentCollectionSuffix,
			bool includeCollectionColumns)
		{
			return SelectFragment(rhs, rhsAlias, lhsAlias, currentCollectionSuffix, includeCollectionColumns, new EntityLoadInfo(currentEntitySuffix));
		}

		// 6.0 TODO: Remove
		[Obsolete("Please use overload taking EntityLoadInfo")]
		public virtual string SelectFragment(
			IJoinable rhs, string rhsAlias, string lhsAlias, string entitySuffix, string collectionSuffix,
			bool includeCollectionColumns, bool includeLazyProperties)
		{
			return SelectFragment(rhs, rhsAlias, lhsAlias, collectionSuffix, includeCollectionColumns, new EntityLoadInfo(entitySuffix) {IncludeLazyProps = true});
		}

		//6.0 TODO: Make abstract
		public virtual string SelectFragment(IJoinable rhs, string rhsAlias, string lhsAlias, string currentCollectionSuffix, bool includeCollectionColumns, EntityLoadInfo entityInfo)
		{
			throw new NotImplementedException("SelectFragment with fetching lazy properties option is not implemented by " + GetType().FullName);
		}

		/// <summary>
		/// Given a query alias and an identifying suffix, render the identifier select fragment for collection element entity.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="suffix"></param>
		/// <returns></returns>
		public virtual string IdentifierSelectFragment(string name, string suffix)
		{
			var persister = ReflectHelper.CastOrThrow<ISupportSelectModeJoinable>(ElementPersister, "SelectMode.ChildFetch for collection");
			return persister.IdentifierSelectFragment(name, suffix);
		}

		public abstract bool ConsumesCollectionAlias();

		private void CheckColumnDuplication(HashSet<string> distinctColumns, IEnumerable<ISelectable> columns)
		{
			foreach (ISelectable sel in columns)
			{
				Column col = sel as Column;
				if (col == null)
				{
					// Ignore formulas
					continue;
				}
				if (distinctColumns.Contains(col.Name))
				{
					throw new MappingException("Repeated column in mapping for collection: " + role + " column: " + col.Name);
				}
				else
				{
					distinctColumns.Add(col.Name);
				}
			}
		}

		public ICacheConcurrencyStrategy Cache
		{
			get { return cache; }
		}

		public CollectionType CollectionType
		{
			get { return collectionType; }
		}

		public FetchMode FetchMode
		{
			get { return fetchMode; }
		}

		protected SqlCommandInfo SqlDeleteString
		{
			get { return sqlDeleteString; }
		}

		protected SqlCommandInfo SqlInsertRowString
		{
			get { return sqlInsertRowString; }
		}

		protected SqlCommandInfo SqlUpdateRowString
		{
			get { return sqlUpdateRowString; }
		}

		protected SqlCommandInfo SqlDeleteRowString
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

		public bool IsPrimitiveArray
		{
			get { return isPrimitiveArray; }
		}

		public bool IsArray
		{
			get { return isArray; }
		}

		public string IdentifierColumnName
		{
			get
			{
				if (hasIdentifier)
				{
					return identifierColumnName;
				}
				else
				{
					return null;
				}
			}
		}

		public string[] IndexFormulas
		{
			get { return indexFormulas; }
		}

		public string[] KeyColumnNames
		{
			get { return keyColumnNames; }
		}

		// Since v5.2
		[Obsolete("Use KeyColumnNames instead")]
		public string[] JoinColumnNames
		{
			get { return keyColumnNames; }
		}

		protected string[] KeyColumnAliases
		{
			get { return keyColumnAliases; }
		}

		public bool IsLazy
		{
			get { return isLazy; }
		}

		public bool IsInverse
		{
			get { return isInverse; }
		}

		protected virtual bool RowDeleteEnabled
		{
			get { return true; }
		}

		protected virtual bool RowInsertEnabled
		{
			get { return true; }
		}

		/// <summary>
		/// Get the name of this collection role (the fully qualified class name,
		/// extended by a "property path")
		/// </summary>
		public string Role
		{
			get { return role; }
		}

		public virtual string OwnerEntityName
		{
			get { return entityName; }
		}

		public IEntityPersister OwnerEntityPersister
		{
			get { return ownerPersister; }
		}

		public IIdentifierGenerator IdentifierGenerator
		{
			get { return identifierGenerator; }
		}

		public IType IdentifierType
		{
			get { return identifierType; }
		}

		public abstract bool IsManyToMany { get; }

		public IType Type
		{
			get { return elementPropertyMapping.Type; }
		}

		public string Name
		{
			get { return Role; }
		}

		public IEntityPersister ElementPersister
		{
			get
			{
				if (elementPersister == null)
				{
					throw new AssertionFailure("Not an association");
				}

				return elementPersister;
			}
		}

		public bool IsCollection
		{
			get { return true; }
		}

		public string[] CollectionSpaces
		{
			get { return spaces; }
		}

		public ICollectionMetadata CollectionMetadata
		{
			get { return this; }
		}

		public ISessionFactoryImplementor Factory
		{
			get { return factory; }
		}

		protected virtual bool InsertCallable
		{
			get { return insertCallable; }
		}

		protected ExecuteUpdateResultCheckStyle InsertCheckStyle
		{
			get { return insertCheckStyle; }
		}

		protected virtual bool UpdateCallable
		{
			get { return updateCallable; }
		}

		protected ExecuteUpdateResultCheckStyle UpdateCheckStyle
		{
			get { return updateCheckStyle; }
		}

		protected virtual bool DeleteCallable
		{
			get { return deleteCallable; }
		}

		protected ExecuteUpdateResultCheckStyle DeleteCheckStyle
		{
			get { return deleteCheckStyle; }
		}

		protected virtual bool DeleteAllCallable
		{
			get { return deleteAllCallable; }
		}

		protected ExecuteUpdateResultCheckStyle DeleteAllCheckStyle
		{
			get { return deleteAllCheckStyle; }
		}

		public bool IsVersioned
		{
			get { return isVersioned && OwnerEntityPersister.IsVersioned; }
		}

		protected virtual ISQLExceptionConverter SQLExceptionConverter
		{
			get { return sqlExceptionConverter; }
		}

		public ICacheEntryStructure CacheEntryStructure
		{
			get { return cacheEntryStructure; }
		}

		public bool IsSubselectLoadable
		{
			get { return subselectLoadable; }
		}

		public bool IsMutable
		{
			get { return isMutable; }
		}

		public bool IsExtraLazy
		{
			get { return isExtraLazy; }
		}

		protected Dialect.Dialect Dialect
		{
			get { return dialect; }
		}

		public abstract bool CascadeDeleteEnabled { get; }
		public abstract bool IsOneToMany { get; }

		// Since v5.2
		[Obsolete("Use directly the alias parameter value instead")]
		public virtual string GenerateTableAliasForKeyColumns(string alias)
		{
			return alias;
		}

		protected object PerformInsert(object ownerId, IPersistentCollection collection, IExpectation expectation,
									   object entry, int index, bool useBatch, bool callable, ISessionImplementor session)
		{
			object entryId = null;
			int offset = 0;
			var st = useBatch
				? session.Batcher.PrepareBatchCommand(SqlInsertRowString.CommandType, SqlInsertRowString.Text, SqlInsertRowString.ParameterTypes)
				: session.Batcher.PrepareCommand(SqlInsertRowString.CommandType, SqlInsertRowString.Text, SqlInsertRowString.ParameterTypes);
			try
			{
				//offset += expectation.Prepare(st, factory.ConnectionProvider.Driver);
				offset = WriteKey(st, ownerId, offset, session);
				if (hasIdentifier)
				{
					entryId = collection.GetIdentifier(entry, index);
					offset = WriteIdentifier(st, entryId, offset, session);
				}
				if (hasIndex)
				{
					offset = WriteIndex(st, collection.GetIndex(entry, index, this), offset, session);
				}
				WriteElement(st, collection.GetElement(entry), offset, session);
				if (useBatch)
				{
					session.Batcher.AddToBatch(expectation);
				}
				else
				{
					expectation.VerifyOutcomeNonBatched(session.Batcher.ExecuteNonQuery(st), st);
				}
			}
			catch (Exception e)
			{
				if (useBatch)
				{
					session.Batcher.AbortBatch(e);
				}
				throw;
			}
			finally
			{
				if (!useBatch)
				{
					session.Batcher.CloseCommand(st, null);
				}
			}
			return entryId;
		}

		#region NH specific

		public bool IsIdentifierAssignedByInsert
		{
			get { return identityDelegate != null; }
		}

		protected bool UseInsertSelectIdentity()
		{
			return !UseGetGeneratedKeys() && Factory.Dialect.SupportsInsertSelectIdentity;
		}

		protected bool UseGetGeneratedKeys()
		{
			return Factory.Settings.IsGetGeneratedKeysEnabled;
		}

		#region IPostInsertIdentityPersister Members

		private string identitySelectString;
		private bool isCollectionIntegerIndex;

		public string IdentitySelectString
		{
			get
			{
				if (identitySelectString == null)
				{
					identitySelectString =
						Factory.Dialect.GetIdentitySelectString(IdentifierColumnName, qualifiedTableName,
																IdentifierType.SqlTypes(Factory)[0].DbType);
				}
				return identitySelectString;
			}
		}

		public string[] RootTableKeyColumnNames
		{
			get { return new string[] {IdentifierColumnName}; }
		}

		/// <summary>
		/// Get the batch size of a collection persister.
		/// </summary>
		public int GetBatchSize()
		{
			return batchSize;
		}

		// Since 5.2
		[Obsolete("Use GetSelectByUniqueKeyString(string[] suppliedPropertyNames, out IType[] parameterTypes) instead.")]
		public SqlString GetSelectByUniqueKeyString(string propertyName)
		{
			return GetSelectByUniqueKeyString(null, out _);
		}

		public virtual SqlString GetSelectByUniqueKeyString(string[] suppliedPropertyNames, out IType[] parameterTypes)
		{
			if (suppliedPropertyNames != null)
				throw new NotSupportedException("Collections does not support custom property names for selecting by unique key");

			parameterTypes = HasIndex ?  new[] { KeyType, IndexType, ElementType } : new[] { KeyType, ElementType };
			var builder =
				new SqlSimpleSelectBuilder(Factory.Dialect, Factory)
					.SetTableName(qualifiedTableName)
					.AddColumns(new[] {identifierColumnName})
					.AddWhereFragment(KeyColumnNames, KeyType, " = ");

			if (HasIndex)
				builder.AddWhereFragment(IndexColumnNames, IndexType, " = ");

			return builder
				.AddWhereFragment(ElementColumnNames, ElementType, " = ")
				.ToSqlString();
		}

		public void BindSelectByUniqueKey(
			ISessionImplementor session,
			DbCommand selectCommand,
			IBinder binder,
			string[] suppliedPropertyNames)
		{
			binder.BindValues(selectCommand);
		}

		public string GetInfoString()
		{
			return MessageHelper.CollectionInfoString(this, null);
		}
		#endregion

		/// <summary>
		/// Perform an SQL INSERT, and then retrieve a generated identifier.
		/// </summary>
		/// <returns> the id of the collection entry </returns>
		/// <remarks>
		/// This form is used for PostInsertIdentifierGenerator-style ids (IDENTITY, select, etc).
		/// </remarks>
		protected object PerformInsert(object ownerId, IPersistentCollection collection, object entry, int index,
									   ISessionImplementor session)
		{
			IBinder binder = new GeneratedIdentifierBinder(ownerId, collection, entry, index, session, this);
			return identityDelegate.PerformInsert(SqlInsertRowString, session, binder);
		}

		protected partial class GeneratedIdentifierBinder : IBinder
		{
			private readonly object ownerId;
			private readonly IPersistentCollection collection;
			private readonly object entry;
			private readonly int index;
			private readonly ISessionImplementor session;
			private readonly AbstractCollectionPersister persister;

			public GeneratedIdentifierBinder(object ownerId, IPersistentCollection collection, object entry, int index,
											 ISessionImplementor session, AbstractCollectionPersister persister)
			{
				this.ownerId = ownerId;
				this.collection = collection;
				this.entry = entry;
				this.index = index;
				this.session = session;
				this.persister = persister;
			}

			public object Entity
			{
				get { return entry; }
			}

			public void BindValues(DbCommand cm)
			{
				int offset = 0;
				offset = persister.WriteKey(cm, ownerId, offset, session);
				if (persister.HasIndex)
				{
					offset = persister.WriteIndex(cm, collection.GetIndex(entry, index, persister), offset, session);
				}
				persister.WriteElement(cm, collection.GetElement(entry), offset, session);
			}
		}

		#endregion
	}
}
