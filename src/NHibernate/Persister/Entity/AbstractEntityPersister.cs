using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

using NHibernate.AdoNet;
using NHibernate.Cache;
using NHibernate.Cache.Entry;
using NHibernate.Dialect.Lock;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Id;
using NHibernate.Id.Insert;
using NHibernate.Impl;
using NHibernate.Intercept;
using NHibernate.Loader.Entity;
using NHibernate.Mapping;
using NHibernate.Metadata;
using NHibernate.Properties;
using NHibernate.SqlCommand;
using NHibernate.Tuple;
using NHibernate.Tuple.Entity;
using NHibernate.Type;
using NHibernate.Util;
using Array=System.Array;
using Property=NHibernate.Mapping.Property;
using NHibernate.SqlTypes;
using System.Linq;

namespace NHibernate.Persister.Entity
{
	/// <summary>
	/// Superclass for built-in mapping strategies. Implements functionalty common to both mapping
	/// strategies
	/// </summary>
	/// <remarks>
	/// May be considered an immutable view of the mapping object
	/// </remarks>
	public abstract partial class AbstractEntityPersister : IOuterJoinLoadable, IQueryable, IClassMetadata, IUniqueKeyLoadable, ISqlLoadable, ILazyPropertyInitializer, IPostInsertIdentityPersister, ILockable
	{
		#region InclusionChecker

		protected internal interface IInclusionChecker
		{
			bool IncludeProperty(int propertyNumber);
		}

		private class NoneInclusionChecker : IInclusionChecker
		{
			private readonly ValueInclusion[] inclusions;

			public NoneInclusionChecker(ValueInclusion[] inclusions)
			{
				this.inclusions = inclusions;
			}

			// TODO : currently we really do not handle ValueInclusion.PARTIAL...
			// ValueInclusion.PARTIAL would indicate parts of a component need to
			// be included in the select; currently we then just render the entire
			// component into the select clause in that case.
			public bool IncludeProperty(int propertyNumber)
			{
				return inclusions[propertyNumber] != ValueInclusion.None;
			}
		}

		private class FullInclusionChecker : IInclusionChecker
		{
			private readonly bool[] includeProperty;

			public FullInclusionChecker(bool[] includeProperty)
			{
				this.includeProperty = includeProperty;
			}
			public bool IncludeProperty(int propertyNumber)
			{
				return includeProperty[propertyNumber];
			}
		}

		#endregion

		private class GeneratedIdentifierBinder : IBinder
		{
			private readonly object[] fields;
			private readonly bool[] notNull;
			private readonly ISessionImplementor session;
			private readonly object entity;
			private readonly AbstractEntityPersister entityPersister;

			public GeneratedIdentifierBinder(object[] fields, bool[] notNull, ISessionImplementor session, object entity, AbstractEntityPersister entityPersister)
			{
				this.fields = fields;
				this.notNull = notNull;
				this.session = session;
				this.entity = entity;
				this.entityPersister = entityPersister;
			}

			public object Entity
			{
				get { return entity; }
			}

			public virtual void BindValues(DbCommand ps)
			{
				entityPersister.Dehydrate(null, fields, notNull, entityPersister.propertyColumnInsertable, 0, ps, session);
			}
		}

		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(AbstractEntityPersister));
		public const string EntityClass = "class";
		protected const string Discriminator_Alias = "clazz_";

		private readonly ISessionFactoryImplementor factory;

		private readonly ICacheConcurrencyStrategy cache;
		private readonly bool isLazyPropertiesCacheable;
		private readonly ICacheEntryStructure cacheEntryStructure;
		private readonly EntityMetamodel entityMetamodel;
		private readonly Dictionary<System.Type, string> entityNameBySubclass = new Dictionary<System.Type, string>();

		private readonly string[] rootTableKeyColumnNames;
		private readonly string[] identifierAliases;
		private readonly int identifierColumnSpan;
		private readonly string versionColumnName;
		private readonly bool hasFormulaProperties;
		private readonly int batchSize;
		private readonly bool hasSubselectLoadableCollections;
		protected internal string rowIdName;

		private readonly ISet<string> lazyProperties;

		private readonly string sqlWhereString;
		private readonly string sqlWhereStringTemplate;

		#region Information about properties of this class
		//including inherited properties
		//(only really needed for updatable/insertable properties)
		private readonly int[] propertyColumnSpans;
		// the names of the columns for the property
		// the array is indexed as propertyColumnNames[propertyIndex][columnIndex] = "columnName"
		private readonly string[] propertySubclassNames;
		private readonly string[][] propertyColumnAliases;
		private readonly string[][] propertyColumnNames;
		// the alias names for the columns of the property.  This is used in the AS portion for 
		// selecting a column.  It is indexed the same as propertyColumnNames
		// private readonly string[ ] propertyFormulaTemplates;
		private readonly string[][] propertyColumnFormulaTemplates;

		private readonly bool[][] propertyColumnUpdateable;
		private readonly bool[][] propertyColumnInsertable;
		private readonly bool[] propertyUniqueness;
		private readonly bool[] propertySelectable;

		#endregion

		#region Information about lazy properties of this class

		private readonly string[] lazyPropertyNames;
		private readonly int[] lazyPropertyNumbers;
		private readonly IType[] lazyPropertyTypes;
		private readonly string[][] lazyPropertyColumnAliases;

		#endregion

		#region Information about all properties in class hierarchy

		private readonly string[] subclassPropertyNameClosure;
		private readonly string[] subclassPropertySubclassNameClosure;
		private readonly IType[] subclassPropertyTypeClosure;
		private readonly string[][] subclassPropertyFormulaTemplateClosure;
		private readonly string[][] subclassPropertyColumnNameClosure;
		private readonly FetchMode[] subclassPropertyFetchModeClosure;
		private readonly bool[] subclassPropertyNullabilityClosure;
		protected bool[] propertyDefinedOnSubclass;
		private readonly int[][] subclassPropertyColumnNumberClosure;
		private readonly int[][] subclassPropertyFormulaNumberClosure;
		private readonly CascadeStyle[] subclassPropertyCascadeStyleClosure;

		#endregion

		#region Information about all columns/formulas in class hierarchy

		private readonly string[] subclassColumnClosure;
		private readonly bool[] subclassColumnLazyClosure;
		private readonly string[] subclassColumnAliasClosure;
		private readonly bool[] subclassColumnSelectableClosure;
		private readonly string[] subclassFormulaClosure;
		private readonly string[] subclassFormulaTemplateClosure;
		private readonly string[] subclassFormulaAliasClosure;
		private readonly bool[] subclassFormulaLazyClosure;

		#endregion

		#region Dynamic filters attached to the class-level

		private readonly FilterHelper filterHelper;

		#endregion

		private readonly Dictionary<string, EntityLoader> uniqueKeyLoaders = new Dictionary<string, EntityLoader>();
		private readonly Dictionary<LockMode, ILockingStrategy> lockers = new Dictionary<LockMode, ILockingStrategy>();
		private readonly Dictionary<string, IUniqueEntityLoader> loaders = new Dictionary<string, IUniqueEntityLoader>();

		#region SQL strings

		private SqlString sqlVersionSelectString;
		private SqlString sqlSnapshotSelectString;
		private SqlString sqlLazySelectString;

		private SqlCommandInfo sqlIdentityInsertString;
		private SqlCommandInfo sqlUpdateByRowIdString;
		private SqlCommandInfo sqlLazyUpdateByRowIdString;

		private SqlCommandInfo[] sqlDeleteStrings;
		private SqlCommandInfo[] sqlInsertStrings;
		private SqlCommandInfo[] sqlUpdateStrings;
		private SqlCommandInfo[] sqlLazyUpdateStrings;

		private SqlString sqlInsertGeneratedValuesSelectString;
		private SqlString sqlUpdateGeneratedValuesSelectString;
		private string identitySelectString;
		#endregion

		#region Custom SQL

		protected internal bool[] insertCallable;
		protected internal bool[] updateCallable;
		protected internal bool[] deleteCallable;
		protected internal SqlString[] customSQLInsert;
		protected internal SqlString[] customSQLUpdate;
		protected internal SqlString[] customSQLDelete;
		protected internal ExecuteUpdateResultCheckStyle[] insertResultCheckStyles;
		protected internal ExecuteUpdateResultCheckStyle[] updateResultCheckStyles;
		protected internal ExecuteUpdateResultCheckStyle[] deleteResultCheckStyles;

		#endregion

		private IInsertGeneratedIdentifierDelegate identityDelegate;

		private bool[] tableHasColumns;

		private readonly string loaderName;

		private IUniqueEntityLoader queryLoader;

		private readonly string temporaryIdTableName;
		private readonly string temporaryIdTableDDL;

		private readonly Dictionary<string, string[]> subclassPropertyAliases = new Dictionary<string, string[]>();
		private readonly Dictionary<string, string[]> subclassPropertyColumnNames = new Dictionary<string, string[]>();

		protected readonly BasicEntityPropertyMapping propertyMapping;

		protected AbstractEntityPersister(PersistentClass persistentClass, ICacheConcurrencyStrategy cache,
																			ISessionFactoryImplementor factory)
		{
			this.factory = factory;
			this.cache = cache;
			isLazyPropertiesCacheable = persistentClass.IsLazyPropertiesCacheable;
			cacheEntryStructure = factory.Settings.IsStructuredCacheEntriesEnabled
															? (ICacheEntryStructure)new StructuredCacheEntry(this)
															: (ICacheEntryStructure)new UnstructuredCacheEntry();

			entityMetamodel = new EntityMetamodel(persistentClass, factory);

			if (persistentClass.HasPocoRepresentation)
			{
				//TODO: this is currently specific to pojos, but need to be available for all entity-modes
				foreach (Subclass subclass in persistentClass.SubclassIterator)
				{
					entityNameBySubclass[subclass.MappedClass] = subclass.EntityName;
				}
			}

			batchSize = persistentClass.BatchSize ?? factory.Settings.DefaultBatchFetchSize;
			hasSubselectLoadableCollections = persistentClass.HasSubselectLoadableCollections;

			propertyMapping = new BasicEntityPropertyMapping(this);

			#region IDENTIFIER

			identifierColumnSpan = persistentClass.Identifier.ColumnSpan;
			rootTableKeyColumnNames = new string[identifierColumnSpan];
			identifierAliases = new string[identifierColumnSpan];

			rowIdName = persistentClass.RootTable.RowId;

			loaderName = persistentClass.LoaderName;

			// TODO NH: Not safe cast to Column
			int i = 0;
			foreach (Column col in persistentClass.Identifier.ColumnIterator)
			{
				rootTableKeyColumnNames[i] = col.GetQuotedName(factory.Dialect);
				identifierAliases[i] = col.GetAlias(factory.Dialect, persistentClass.RootTable);
				i++;
			}

			#endregion

			#region VERSION

			if (persistentClass.IsVersioned)
			{
				foreach (Column col in persistentClass.Version.ColumnIterator)
				{
					versionColumnName = col.GetQuotedName(factory.Dialect);
					break; //only happens once
				}
			}
			else
			{
				versionColumnName = null;
			}

			#endregion

			#region WHERE STRING

			sqlWhereString = !string.IsNullOrEmpty(persistentClass.Where) ? "( " + persistentClass.Where + ") " : null;
			sqlWhereStringTemplate = sqlWhereString == null
																? null
																: Template.RenderWhereStringTemplate(sqlWhereString, factory.Dialect,
																																		 factory.SQLFunctionRegistry);

			#endregion

			#region PROPERTIES

			// NH: see consistence with the implementation on EntityMetamodel where we are disabling lazy-properties for no lazy entities
			bool lazyAvailable = IsInstrumented && entityMetamodel.IsLazy;

			int hydrateSpan = entityMetamodel.PropertySpan;
			propertyColumnSpans = new int[hydrateSpan];
			propertySubclassNames = new string[hydrateSpan];
			propertyColumnAliases = new string[hydrateSpan][];
			propertyColumnNames = new string[hydrateSpan][];
			propertyColumnFormulaTemplates = new string[hydrateSpan][];
			propertyUniqueness = new bool[hydrateSpan];
			propertySelectable = new bool[hydrateSpan];
			propertyColumnUpdateable = new bool[hydrateSpan][];
			propertyColumnInsertable = new bool[hydrateSpan][];
			var thisClassProperties = new HashSet<Property>();

			lazyProperties = new HashSet<string>();
			List<string> lazyNames = new List<string>();
			List<int> lazyNumbers = new List<int>();
			List<IType> lazyTypes = new List<IType>();
			List<string[]> lazyColAliases = new List<string[]>();

			i = 0;
			bool foundFormula = false;
			foreach (Property prop in persistentClass.PropertyClosureIterator)
			{
				thisClassProperties.Add(prop);

				int span = prop.ColumnSpan;
				propertyColumnSpans[i] = span;
				propertySubclassNames[i] = prop.PersistentClass.EntityName;
				string[] colNames = new string[span];
				string[] colAliases = new string[span];
				string[] templates = new string[span];
				int k = 0;
				foreach (ISelectable thing in prop.ColumnIterator)
				{
					colAliases[k] = thing.GetAlias(factory.Dialect, prop.Value.Table);
					if (thing.IsFormula)
					{
						foundFormula = true;
						templates[k] = thing.GetTemplate(factory.Dialect, factory.SQLFunctionRegistry);
					}
					else
					{
						colNames[k] = thing.GetTemplate(factory.Dialect, factory.SQLFunctionRegistry);
					}
					k++;
				}
				propertyColumnNames[i] = colNames;
				propertyColumnFormulaTemplates[i] = templates;
				propertyColumnAliases[i] = colAliases;

				if (lazyAvailable && prop.IsLazy)
				{
					lazyProperties.Add(prop.Name);
					lazyNames.Add(prop.Name);
					lazyNumbers.Add(i);
					lazyTypes.Add(prop.Value.Type);
					lazyColAliases.Add(colAliases);
				}

				propertyColumnUpdateable[i] = prop.Value.ColumnUpdateability;
				propertyColumnInsertable[i] = prop.Value.ColumnInsertability;

				propertySelectable[i] = prop.IsSelectable;

				propertyUniqueness[i] = prop.Value.IsAlternateUniqueKey;

				i++;
			}
			hasFormulaProperties = foundFormula;
			lazyPropertyColumnAliases = lazyColAliases.ToArray();
			lazyPropertyNames = lazyNames.ToArray();
			lazyPropertyNumbers = lazyNumbers.ToArray();
			lazyPropertyTypes = lazyTypes.ToArray();

			#endregion

			#region SUBCLASS PROPERTY CLOSURE

			List<string> columns = new List<string>();
			List<bool> columnsLazy = new List<bool>();
			List<string> aliases = new List<string>();
			List<string> formulas = new List<string>();
			List<string> formulaAliases = new List<string>();
			List<string> formulaTemplates = new List<string>();
			List<bool> formulasLazy = new List<bool>();
			List<IType> types = new List<IType>();
			List<string> names = new List<string>();
			List<string> classes = new List<string>();
			List<string[]> templates2 = new List<string[]>();
			List<string[]> propColumns = new List<string[]>();
			List<FetchMode> joinedFetchesList = new List<FetchMode>();
			List<CascadeStyle> cascades = new List<CascadeStyle>();
			List<bool> definedBySubclass = new List<bool>();
			List<int[]> propColumnNumbers = new List<int[]>();
			List<int[]> propFormulaNumbers = new List<int[]>();
			List<bool> columnSelectables = new List<bool>();
			List<bool> propNullables = new List<bool>();

			foreach (Property prop in persistentClass.SubclassPropertyClosureIterator)
			{
				names.Add(prop.Name);
				classes.Add(prop.PersistentClass.EntityName);
				bool isDefinedBySubclass = !thisClassProperties.Contains(prop);
				definedBySubclass.Add(isDefinedBySubclass);
				propNullables.Add(prop.IsOptional || isDefinedBySubclass); //TODO: is this completely correct?
				types.Add(prop.Type);

				string[] cols = new string[prop.ColumnSpan];
				string[] forms = new string[prop.ColumnSpan];
				int[] colnos = new int[prop.ColumnSpan];
				int[] formnos = new int[prop.ColumnSpan];
				int l = 0;
				bool lazy = prop.IsLazy && lazyAvailable;
				foreach (ISelectable thing in prop.ColumnIterator)
				{
					if (thing.IsFormula)
					{
						string template = thing.GetTemplate(factory.Dialect, factory.SQLFunctionRegistry);
						formnos[l] = formulaTemplates.Count;
						colnos[l] = -1;
						formulaTemplates.Add(template);
						forms[l] = template;
						formulas.Add(thing.GetText(factory.Dialect));
						formulaAliases.Add(thing.GetAlias(factory.Dialect));
						formulasLazy.Add(lazy);
					}
					else
					{
						string colName = thing.GetTemplate(factory.Dialect, factory.SQLFunctionRegistry);
						colnos[l] = columns.Count; //before add :-)
						formnos[l] = -1;
						columns.Add(colName);
						cols[l] = colName;
						aliases.Add(thing.GetAlias(factory.Dialect, prop.Value.Table));
						columnsLazy.Add(lazy);
						columnSelectables.Add(prop.IsSelectable);
					}
					l++;
				}
				propColumns.Add(cols);
				templates2.Add(forms);
				propColumnNumbers.Add(colnos);
				propFormulaNumbers.Add(formnos);

				joinedFetchesList.Add(prop.Value.FetchMode);
				cascades.Add(prop.CascadeStyle);
			}
			subclassColumnClosure = columns.ToArray();
			subclassColumnAliasClosure = aliases.ToArray();
			subclassColumnLazyClosure = columnsLazy.ToArray();
			subclassColumnSelectableClosure = columnSelectables.ToArray();

			subclassFormulaClosure = formulas.ToArray();
			subclassFormulaTemplateClosure = formulaTemplates.ToArray();
			subclassFormulaAliasClosure = formulaAliases.ToArray();
			subclassFormulaLazyClosure = formulasLazy.ToArray();

			subclassPropertyNameClosure = names.ToArray();
			subclassPropertySubclassNameClosure = classes.ToArray();
			subclassPropertyTypeClosure = types.ToArray();
			subclassPropertyNullabilityClosure = propNullables.ToArray();
			subclassPropertyFormulaTemplateClosure = templates2.ToArray();
			subclassPropertyColumnNameClosure = propColumns.ToArray();
			subclassPropertyColumnNumberClosure = propColumnNumbers.ToArray();
			subclassPropertyFormulaNumberClosure = propFormulaNumbers.ToArray();

			subclassPropertyCascadeStyleClosure = cascades.ToArray();
			subclassPropertyFetchModeClosure = joinedFetchesList.ToArray();

			propertyDefinedOnSubclass = definedBySubclass.ToArray();

			#endregion

			// Handle any filters applied to the class level
			filterHelper = new FilterHelper(persistentClass.FilterMap, factory.Dialect, factory.SQLFunctionRegistry);

			temporaryIdTableName = persistentClass.TemporaryIdTableName;
			temporaryIdTableDDL = persistentClass.TemporaryIdTableDDL;
		}

		protected abstract int[] SubclassColumnTableNumberClosure { get; }
		protected abstract int[] SubclassFormulaTableNumberClosure { get; }
		protected internal abstract int[] PropertyTableNumbersInSelect { get;}
		protected internal abstract int[] PropertyTableNumbers { get;}

		public virtual string DiscriminatorColumnName
		{
			get { return Discriminator_Alias; }
		}

		protected virtual string DiscriminatorFormulaTemplate
		{
			get { return null; }
		}

		public string[] RootTableKeyColumnNames
		{
			get { return rootTableKeyColumnNames; }
		}

		protected internal SqlCommandInfo[] SQLUpdateByRowIdStrings
		{
			get
			{
				if (sqlUpdateByRowIdString == null)
					throw new AssertionFailure("no update by row id");

				SqlCommandInfo[] result = new SqlCommandInfo[TableSpan + 1];
				result[0] = sqlUpdateByRowIdString;
				Array.Copy(sqlUpdateStrings, 0, result, 1, TableSpan);
				return result;
			}
		}

		protected internal SqlCommandInfo[] SQLLazyUpdateByRowIdStrings
		{
			get
			{
				if (sqlLazyUpdateByRowIdString == null)
					throw new AssertionFailure("no update by row id");

				SqlCommandInfo[] result = new SqlCommandInfo[TableSpan];
				result[0] = sqlLazyUpdateByRowIdString;
				for (int i = 1; i < TableSpan; i++)
					result[i] = sqlLazyUpdateStrings[i];

				return result;
			}
		}

		protected SqlString SQLSnapshotSelectString
		{
			get { return sqlSnapshotSelectString; }
		}

		protected SqlString SQLLazySelectString
		{
			get { return sqlLazySelectString; }
		}

		/// <summary>
		/// The queries that delete rows by id (and version)
		/// </summary>
		protected SqlCommandInfo[] SqlDeleteStrings
		{
			get { return sqlDeleteStrings; }
		}

		/// <summary>
		/// The queries that insert rows with a given id
		/// </summary>
		protected SqlCommandInfo[] SqlInsertStrings
		{
			get { return sqlInsertStrings; }
		}

		/// <summary>
		/// The queries that update rows by id (and version)
		/// </summary>
		protected SqlCommandInfo[] SqlUpdateStrings
		{
			get { return sqlUpdateStrings; }
		}

		protected internal SqlCommandInfo[] SQLLazyUpdateStrings
		{
			get { return sqlLazyUpdateStrings; }
		}

		/// <summary> 
		/// The query that inserts a row, letting the database generate an id 
		/// </summary>
		/// <returns> The IDENTITY-based insertion query. </returns>
		protected internal SqlCommandInfo SQLIdentityInsertString
		{
			get { return sqlIdentityInsertString; }
		}

		protected SqlString VersionSelectString
		{
			get { return sqlVersionSelectString; }
		}

		public bool IsBatchable => OptimisticLockMode == Versioning.OptimisticLock.None ||
		                           (!IsVersioned && OptimisticLockMode == Versioning.OptimisticLock.Version) ||
		                           Factory.Settings.IsBatchVersionedDataEnabled;

		public virtual string[] QuerySpaces
		{
			get { return PropertySpaces; }
		}

		protected internal ISet<string> LazyProperties
		{
			get { return lazyProperties; }
		}

		public bool IsBatchLoadable
		{
			get { return batchSize > 1; }
		}

		public virtual string[] IdentifierColumnNames
		{
			get { return rootTableKeyColumnNames; }
		}

		protected int IdentifierColumnSpan
		{
			get { return identifierColumnSpan; }
		}

		public virtual string VersionColumnName
		{
			get { return versionColumnName; }
		}

		protected internal string VersionedTableName
		{
			get { return GetTableName(0); }
		}

		protected internal bool[] SubclassColumnLaziness
		{
			get { return subclassColumnLazyClosure; }
		}

		protected internal bool[] SubclassFormulaLaziness
		{
			get { return subclassFormulaLazyClosure; }
		}

		/// <summary> 
		/// We can't immediately add to the cache if we have formulas
		/// which must be evaluated, or if we have the possibility of
		/// two concurrent updates to the same item being merged on
		/// the database. This can happen if (a) the item is not
		/// versioned and either (b) we have dynamic update enabled
		/// or (c) we have multiple tables holding the state of the
		/// item.
		/// </summary>
		public bool IsCacheInvalidationRequired
		{
			get { return HasFormulaProperties || (!IsVersioned && (entityMetamodel.IsDynamicUpdate || TableSpan > 1)); }
		}

		public bool IsLazyPropertiesCacheable
		{
			get { return isLazyPropertiesCacheable; }
		}

		public virtual string RootTableName
		{
			get { return GetSubclassTableName(0); }
		}

		public virtual string[] RootTableIdentifierColumnNames
		{
			get { return RootTableKeyColumnNames; }
		}

		protected internal string[] PropertySubclassNames
		{
			get { return propertySubclassNames; }
		}

		protected string[][] SubclassPropertyFormulaTemplateClosure
		{
			get { return subclassPropertyFormulaTemplateClosure; }
		}

		protected IType[] SubclassPropertyTypeClosure
		{
			get { return subclassPropertyTypeClosure; }
		}

		protected string[][] SubclassPropertyColumnNameClosure
		{
			get { return subclassPropertyColumnNameClosure; }
		}

		protected string[] SubclassPropertyNameClosure
		{
			get { return subclassPropertyNameClosure; }
		}

		protected string[] SubclassPropertySubclassNameClosure
		{
			get { return subclassPropertySubclassNameClosure; }
		}

		protected string[] SubclassColumnClosure
		{
			get { return subclassColumnClosure; }
		}

		protected string[] SubclassColumnAliasClosure
		{
			get { return subclassColumnAliasClosure; }
		}

		protected string[] SubclassFormulaClosure
		{
			get { return subclassFormulaClosure; }
		}

		protected string[] SubclassFormulaTemplateClosure
		{
			get { return subclassFormulaTemplateClosure; }
		}

		protected string[] SubclassFormulaAliasClosure
		{
			get { return subclassFormulaAliasClosure; }
		}

		public string IdentitySelectString
		{
			get
			{
				if (identitySelectString == null)
					identitySelectString =
						Factory.Dialect.GetIdentitySelectString(GetTableName(0), GetKeyColumns(0)[0],
																										IdentifierType.SqlTypes(Factory)[0].DbType);
				return identitySelectString;
			}
		}

		private string RootAlias
		{
			get { return StringHelper.GenerateAlias(EntityName); }
		}

		public ISessionFactoryImplementor Factory
		{
			get { return factory; }
		}

		public EntityMetamodel EntityMetamodel
		{
			get { return entityMetamodel; }
		}

		public ICacheConcurrencyStrategy Cache
		{
			get { return cache; }
		}

		public ICacheEntryStructure CacheEntryStructure
		{
			get { return cacheEntryStructure; }
		}

		public IComparer VersionComparator
		{
			get { return IsVersioned ? VersionType.Comparator : null; }
		}

		public string EntityName
		{
			get { return entityMetamodel.Name; }
		}

		public EntityType EntityType
		{
			get { return entityMetamodel.EntityType; }
		}

		public virtual bool IsPolymorphic
		{
			get { return entityMetamodel.IsPolymorphic; }
		}

		public virtual bool IsInherited
		{
			get { return entityMetamodel.IsInherited; }
		}

		public virtual IVersionType VersionType
		{
			get { return LocateVersionType(); }
		}

		public virtual int VersionProperty
		{
			get { return entityMetamodel.VersionPropertyIndex; }
		}

		public virtual bool IsVersioned
		{
			get { return entityMetamodel.IsVersioned; }
		}

		public virtual bool IsIdentifierAssignedByInsert
		{
			get { return entityMetamodel.IdentifierProperty.IsIdentifierAssignedByInsert; }
		}

		public virtual bool IsMutable
		{
			get { return entityMetamodel.IsMutable; }
		}

		public virtual bool IsAbstract
		{
			get { return entityMetamodel.IsAbstract; }
		}

		public virtual IIdentifierGenerator IdentifierGenerator
		{
			get { return entityMetamodel.IdentifierProperty.IdentifierGenerator; }
		}

		public virtual string RootEntityName
		{
			get { return entityMetamodel.RootName; }
		}

		public virtual IClassMetadata ClassMetadata
		{
			get { return this; }
		}

		public virtual string MappedSuperclass
		{
			get { return entityMetamodel.Superclass; }
		}

		public virtual bool IsExplicitPolymorphism
		{
			get { return entityMetamodel.IsExplicitPolymorphism; }
		}

		public string[] KeyColumnNames
		{
			get { return IdentifierColumnNames; }
		}

		public string[] JoinColumnNames
		{
			get { return KeyColumnNames; }
		}

		public string Name
		{
			get { return EntityName; }
		}

		public bool IsCollection
		{
			get { return false; }
		}

		public IType Type
		{
			get { return entityMetamodel.EntityType; }
		}

		public bool IsSelectBeforeUpdateRequired
		{
			get { return entityMetamodel.IsSelectBeforeUpdate; }
		}

		public bool IsVersionPropertyGenerated
		{
			get { return IsVersioned && PropertyUpdateGenerationInclusions[VersionProperty] != ValueInclusion.None; }
		}

		public bool VersionPropertyInsertable
		{
			get { return IsVersioned && PropertyInsertability[VersionProperty]; }
		}

		public virtual string[] PropertyNames
		{
			get { return entityMetamodel.PropertyNames; }
		}

		public virtual IType[] PropertyTypes
		{
			get { return entityMetamodel.PropertyTypes; }
		}

		public bool[] PropertyLaziness
		{
			get { return entityMetamodel.PropertyLaziness; }
		}

		public virtual bool[] PropertyCheckability
		{
			get { return entityMetamodel.PropertyCheckability; }
		}

		public bool[] NonLazyPropertyUpdateability
		{
			get { return entityMetamodel.NonlazyPropertyUpdateability; }
		}

		public virtual bool[] PropertyInsertability
		{
			get { return entityMetamodel.PropertyInsertability; }
		}

		public ValueInclusion[] PropertyInsertGenerationInclusions
		{
			get { return entityMetamodel.PropertyInsertGenerationInclusions; }
		}

		public ValueInclusion[] PropertyUpdateGenerationInclusions
		{
			get { return entityMetamodel.PropertyUpdateGenerationInclusions; }
		}

		public virtual bool[] PropertyNullability
		{
			get { return entityMetamodel.PropertyNullability; }
		}

		public virtual bool[] PropertyVersionability
		{
			get { return entityMetamodel.PropertyVersionability; }
		}

		public virtual CascadeStyle[] PropertyCascadeStyles
		{
			get { return entityMetamodel.CascadeStyles; }
		}

		public virtual bool IsMultiTable
		{
			get { return false; }
		}

		public string TemporaryIdTableName
		{
			get { return temporaryIdTableName; }
		}

		public string TemporaryIdTableDDL
		{
			get { return temporaryIdTableDDL; }
		}

		protected int PropertySpan
		{
			get { return entityMetamodel.PropertySpan; }
		}

		public virtual string IdentifierPropertyName
		{
			get { return entityMetamodel.IdentifierProperty.Name; }
		}

		public virtual IType GetIdentifierType(int j)
		{
			return IdentifierType;
		}

		public virtual IType IdentifierType
		{
			get { return entityMetamodel.IdentifierProperty.Type; }
		}

		public int[] NaturalIdentifierProperties
		{
			get { return entityMetamodel.NaturalIdentifierProperties; }
		}

		public abstract string[][] ConstraintOrderedTableKeyColumnClosure { get;}
		public abstract IType DiscriminatorType { get;}
		public abstract string[] ConstraintOrderedTableNameClosure { get;}
		public abstract string DiscriminatorSQLValue { get;}
		public abstract object DiscriminatorValue { get;}
		public abstract string[] SubclassClosure { get; }
		public abstract string[] PropertySpaces { get;}

		protected virtual void AddDiscriminatorToInsert(SqlInsertBuilder insert) { }

		protected virtual void AddDiscriminatorToSelect(SelectFragment select, string name, string suffix) { }

		public abstract string GetSubclassTableName(int j);

		//gets the identifier for a join table if other than pk
		protected virtual object GetJoinTableId(int j, object[] fields)
		{
			return null;
		}

		protected virtual object GetJoinTableId(int table, object obj)
		{
			return null;
		}

		//for joining to other keys than pk
		protected virtual string[] GetJoinIdKeyColumns(int j)
		{
			return IdentifierColumnNames;
		}

		protected abstract string[] GetSubclassTableKeyColumns(int j);

		protected abstract bool IsClassOrSuperclassTable(int j);

		protected abstract int SubclassTableSpan { get; }

		protected abstract int TableSpan { get; }

		protected abstract bool IsTableCascadeDeleteEnabled(int j);

		protected abstract string GetTableName(int table);

		protected abstract string[] GetKeyColumns(int table);

		protected abstract bool IsPropertyOfTable(int property, int table);

		protected virtual int? GetRefIdColumnOfTable(int table)
		{
			return null;
		}

		protected virtual Tuple.Property GetIdentiferProperty(int table)
		{
			var refId = GetRefIdColumnOfTable(table);
			if (refId == null)
				return entityMetamodel.IdentifierProperty;

			return entityMetamodel.Properties[refId.Value];
		}

		protected virtual bool IsIdOfTable(int property, int table)
		{
			return false;
		}

		protected abstract int GetSubclassPropertyTableNumber(int i);

		public abstract string FilterFragment(string alias);

		protected internal virtual string DiscriminatorAlias
		{
			get { return Discriminator_Alias; }
		}

		protected virtual bool IsInverseTable(int j)
		{
			return false;
		}

		protected virtual bool IsNullableTable(int j)
		{
			return false;
		}

		protected virtual bool IsNullableSubclassTable(int j)
		{
			return false;
		}

		protected virtual bool IsInverseSubclassTable(int j)
		{
			return false;
		}

		public virtual bool IsSubclassEntityName(string entityName)
		{
			return entityMetamodel.SubclassEntityNames.Contains(entityName);
		}

		protected bool[] TableHasColumns
		{
			get { return tableHasColumns; }
		}

		protected bool IsInsertCallable(int j)
		{
			return insertCallable[j];
		}

		protected bool IsUpdateCallable(int j)
		{
			return updateCallable[j];
		}

		protected bool IsDeleteCallable(int j)
		{
			return deleteCallable[j];
		}

		protected virtual bool IsSubclassPropertyDeferred(string propertyName, string entityName)
		{
			return false;
		}

		protected virtual bool IsSubclassTableSequentialSelect(int table)
		{
			return false;
		}

		public virtual bool HasSequentialSelect
		{
			get { return false; }
		}

		/// <summary>
		/// Decide which tables need to be updated
		/// </summary>
		/// <param name="dirtyProperties">The indices of all the entity properties considered dirty.</param>
		/// <param name="hasDirtyCollection">Whether any collections owned by the entity which were considered dirty. </param>
		/// <returns> Array of booleans indicating which table require updating. </returns>
		/// <remarks>
		/// The return here is an array of boolean values with each index corresponding
		/// to a given table in the scope of this persister.
		/// </remarks>
		protected virtual bool[] GetTableUpdateNeeded(int[] dirtyProperties, bool hasDirtyCollection)
		{
			if (dirtyProperties == null)
			{
				return TableHasColumns; //for object that came in via update()
			}
			else
			{
				bool[] updateability = PropertyUpdateability;
				int[] propertyTableNumbers = PropertyTableNumbers;
				bool[] tableUpdateNeeded = new bool[TableSpan];
				for (int i = 0; i < dirtyProperties.Length; i++)
				{
					int property = dirtyProperties[i];
					int table = propertyTableNumbers[property];
					tableUpdateNeeded[table] = tableUpdateNeeded[table] ||
																		 (GetPropertyColumnSpan(property) > 0 && updateability[property]);
				}
				if (IsVersioned)
				{
					// NH-2386 when there isn't dirty-properties and the version is generated even in UPDATE
					// we can't execute an UPDATE because there isn't something to UPDATE
					if(!entityMetamodel.VersionProperty.IsUpdateGenerated)
					{
						tableUpdateNeeded[0] = tableUpdateNeeded[0] ||
																	 Versioning.IsVersionIncrementRequired(dirtyProperties, hasDirtyCollection,
																																				 PropertyVersionability);
					}
				}
				return tableUpdateNeeded;
			}
		}

		public virtual bool HasRowId
		{
			get { return rowIdName != null; }
		}

		protected internal virtual SqlString GenerateLazySelectString()
		{
			if (!entityMetamodel.HasLazyProperties)
				return null;

			HashSet<int> tableNumbers = new HashSet<int>();
			List<int> columnNumbers = new List<int>();
			List<int> formulaNumbers = new List<int>();
			for (int i = 0; i < lazyPropertyNames.Length; i++)
			{
				// all this only really needs to consider properties
				// of this class, not its subclasses, but since we
				// are reusing code used for sequential selects, we
				// use the subclass closure
				int propertyNumber = GetSubclassPropertyIndex(lazyPropertyNames[i]);

				int tableNumber = GetSubclassPropertyTableNumber(propertyNumber);
				tableNumbers.Add(tableNumber);

				int[] colNumbers = subclassPropertyColumnNumberClosure[propertyNumber];
				for (int j = 0; j < colNumbers.Length; j++)
				{
					if (colNumbers[j] != -1)
					{
						columnNumbers.Add(colNumbers[j]);
					}
				}
				int[] formNumbers = subclassPropertyFormulaNumberClosure[propertyNumber];
				for (int j = 0; j < formNumbers.Length; j++)
				{
					if (formNumbers[j] != -1)
					{
						formulaNumbers.Add(formNumbers[j]);
					}
				}
			}

			if (columnNumbers.Count == 0 && formulaNumbers.Count == 0)
			{
				// only one-to-one is lazy fetched
				return null;
			}

			return RenderSelect(tableNumbers.ToArray(), columnNumbers.ToArray(), formulaNumbers.ToArray());
		}

		public virtual object InitializeLazyProperty(string fieldName, object entity, ISessionImplementor session)
		{
			object id = session.GetContextEntityIdentifier(entity);

			EntityEntry entry = session.PersistenceContext.GetEntry(entity);
			if (entry == null)
				throw new HibernateException("entity is not associated with the session: " + id);

			if (log.IsDebugEnabled)
			{
				log.Debug(
					string.Format("initializing lazy properties of: {0}, field access: {1}",
												MessageHelper.InfoString(this, id, Factory), fieldName));
			}

			if (HasCache)
			{
				CacheKey cacheKey = session.GenerateCacheKey(id, IdentifierType, EntityName);
				object ce = Cache.Get(cacheKey, session.Timestamp);
				if (ce != null)
				{
					CacheEntry cacheEntry = (CacheEntry)CacheEntryStructure.Destructure(ce, factory);
					if (!cacheEntry.AreLazyPropertiesUnfetched)
					{
						//note early exit here:
						return InitializeLazyPropertiesFromCache(fieldName, entity, session, entry, cacheEntry);
					}
				}
			}

			return InitializeLazyPropertiesFromDatastore(fieldName, entity, session, id, entry);
		}

		private object InitializeLazyPropertiesFromDatastore(string fieldName, object entity, ISessionImplementor session, object id, EntityEntry entry)
		{
			if (!HasLazyProperties)
				throw new AssertionFailure("no lazy properties");

			log.Debug("initializing lazy properties from datastore");

			using (new SessionIdLoggingContext(session.SessionId)) 
			try
			{
				object result = null;
				DbCommand ps = null;
				DbDataReader rs = null;
				try
				{
					SqlString lazySelect = SQLLazySelectString;
					if (lazySelect != null)
					{
						// null sql means that the only lazy properties
						// are shared PK one-to-one associations which are
						// handled differently in the Type#nullSafeGet code...
						ps = session.Batcher.PrepareCommand(CommandType.Text, lazySelect, IdentifierType.SqlTypes(Factory));
						IdentifierType.NullSafeSet(ps, id, 0, session);
						rs = session.Batcher.ExecuteReader(ps);
						rs.Read();
					}
					object[] snapshot = entry.LoadedState;
					for (int j = 0; j < lazyPropertyNames.Length; j++)
					{
						object propValue = lazyPropertyTypes[j].NullSafeGet(rs, lazyPropertyColumnAliases[j], session, entity);
						if (InitializeLazyProperty(fieldName, entity, session, snapshot, j, propValue))
						{
							result = propValue;
						}
					}
				}
				finally
				{
					session.Batcher.CloseCommand(ps, rs);
				}

				log.Debug("done initializing lazy properties");

				return result;
			}
			catch (DbException sqle)
			{
				var exceptionContext = new AdoExceptionContextInfo
										{
											SqlException = sqle,
											Message =
												"could not initialize lazy properties: " + MessageHelper.InfoString(this, id, Factory),
											Sql = SQLLazySelectString.ToString(),
											EntityName = EntityName,
											EntityId = id
										};
				throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, exceptionContext);
			}
		}

		private object InitializeLazyPropertiesFromCache(string fieldName, object entity, ISessionImplementor session, EntityEntry entry, CacheEntry cacheEntry)
		{
			log.Debug("initializing lazy properties from second-level cache");

			object result = null;
			object[] disassembledValues = cacheEntry.DisassembledState;
			object[] snapshot = entry.LoadedState;
			for (int j = 0; j < lazyPropertyNames.Length; j++)
			{
				object propValue = lazyPropertyTypes[j].Assemble(disassembledValues[lazyPropertyNumbers[j]], session, entity);
				if (InitializeLazyProperty(fieldName, entity, session, snapshot, j, propValue))
				{
					result = propValue;
				}
			}

			log.Debug("done initializing lazy properties");

			return result;
		}

		private bool InitializeLazyProperty(string fieldName, object entity, ISessionImplementor session, object[] snapshot, int j, object propValue)
		{
			SetPropertyValue(entity, lazyPropertyNumbers[j], propValue);
			if (snapshot != null)
			{
				// object have been loaded with setReadOnly(true); HHH-2236
				snapshot[lazyPropertyNumbers[j]] = lazyPropertyTypes[j].DeepCopy(propValue, factory);
			}
			return fieldName.Equals(lazyPropertyNames[j]);
		}

		public string[] IdentifierAliases
		{
			get { return identifierAliases; }
		}

		public string SelectFragment(string alias, string suffix)
		{
			return IdentifierSelectFragment(alias, suffix) + PropertySelectFragment(alias, suffix, false);
		}

		public string[] GetIdentifierAliases(string suffix)
		{
			// NOTE: this assumes something about how PropertySelectFragment is implemented by the subclass!
			// was toUnqotedAliasStrings( getIdentiferColumnNames() ) before - now tried
			// to remove that unquoting and missing aliases..
			return new Alias(suffix).ToAliasStrings(IdentifierAliases, factory.Dialect);
		}

		public string[] GetPropertyAliases(string suffix, int i)
		{
			// NOTE: this assumes something about how pPropertySelectFragment is implemented by the subclass!
			return new Alias(suffix).ToUnquotedAliasStrings(propertyColumnAliases[i], factory.Dialect);
		}

		public string GetDiscriminatorAlias(string suffix)
		{
			// NOTE: this assumes something about how PropertySelectFragment is implemented by the subclass!
			// was toUnqotedAliasStrings( getdiscriminatorColumnName() ) before - now tried
			// to remove that unquoting and missing aliases..		
			return entityMetamodel.HasSubclasses ? new Alias(suffix).ToAliasString(DiscriminatorAlias, factory.Dialect) : null;
		}

		public virtual string IdentifierSelectFragment(string name, string suffix)
		{
			return new SelectFragment(factory.Dialect)
				.SetSuffix(suffix)
				.AddColumns(name, IdentifierColumnNames, IdentifierAliases)
				.ToSqlStringFragment(false);
		}

		public string PropertySelectFragment(string name, string suffix, bool allProperties)
		{
			SelectFragment select = new SelectFragment(Factory.Dialect)
				.SetSuffix(suffix)
				.SetUsedAliases(IdentifierAliases);

			int[] columnTableNumbers = SubclassColumnTableNumberClosure;
			string[] columnAliases = SubclassColumnAliasClosure;
			string[] columns = SubclassColumnClosure;

			for (int i = 0; i < columns.Length; i++)
			{
				bool selectable = (allProperties || !subclassColumnLazyClosure[i]) &&
					!IsSubclassTableSequentialSelect(columnTableNumbers[i]) &&
					subclassColumnSelectableClosure[i];
				if (selectable)
				{
					string subalias = GenerateTableAlias(name, columnTableNumbers[i]);
					select.AddColumn(subalias, columns[i], columnAliases[i]);
				}
			}

			int[] formulaTableNumbers = SubclassFormulaTableNumberClosure;
			string[] formulaTemplates = SubclassFormulaTemplateClosure;
			string[] formulaAliases = SubclassFormulaAliasClosure;
			for (int i = 0; i < formulaTemplates.Length; i++)
			{
				bool selectable = (allProperties || !subclassFormulaLazyClosure[i]) &&
					!IsSubclassTableSequentialSelect(formulaTableNumbers[i]);
				if (selectable)
				{
					string subalias = GenerateTableAlias(name, formulaTableNumbers[i]);
					select.AddFormula(subalias, formulaTemplates[i], formulaAliases[i]);
				}
			}

			if (entityMetamodel.HasSubclasses)
				AddDiscriminatorToSelect(select, name, suffix);

			if (HasRowId)
				select.AddColumn(name, rowIdName, Loadable.RowIdAlias);

			return select.ToSqlStringFragment();
		}

		public object[] GetDatabaseSnapshot(object id, ISessionImplementor session)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("Getting current persistent state for: " + MessageHelper.InfoString(this, id, Factory));
			}

			using (new SessionIdLoggingContext(session.SessionId))
			try
			{
				var st = session.Batcher.PrepareCommand(CommandType.Text, SQLSnapshotSelectString, IdentifierType.SqlTypes(factory));
				DbDataReader rs = null;
				try
				{
					IdentifierType.NullSafeSet(st, id, 0, session);
					rs = session.Batcher.ExecuteReader(st);

					if (!rs.Read())
					{
						//if there is no resulting row, return null
						return null;
					}

					//otherwise return the "hydrated" state (ie. associations are not resolved)
					IType[] types = PropertyTypes;
					object[] values = new object[types.Length];
					bool[] includeProperty = PropertyUpdateability;
					for (int i = 0; i < types.Length; i++)
					{
						if (includeProperty[i])
						{
							values[i] = types[i].Hydrate(rs, GetPropertyAliases(string.Empty, i), session, null); //null owner ok??
						}
					}
					return values;
				}
				finally
				{
					session.Batcher.CloseCommand(st, rs);
				}
			}
			catch (DbException sqle)
			{
				var exceptionContext = new AdoExceptionContextInfo
										{
											SqlException = sqle,
											Message = "could not retrieve snapshot: " + MessageHelper.InfoString(this, id, Factory),
											Sql = SQLSnapshotSelectString.ToString(),
											EntityName = EntityName,
											EntityId = id
										};
				throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, exceptionContext);
			}
		}

		/// <summary>
		/// Generate the SQL that selects the version number by id
		/// </summary>
		protected SqlString GenerateSelectVersionString()
		{
			SqlSimpleSelectBuilder builder = new SqlSimpleSelectBuilder(Factory.Dialect, factory)
				.SetTableName(VersionedTableName);

			if (IsVersioned)
				builder.AddColumn(versionColumnName);
			else
				builder.AddColumns(rootTableKeyColumnNames);

			if (Factory.Settings.IsCommentsEnabled)
			{
				builder.SetComment("get version " + EntityName);
			}

			return builder.AddWhereFragment(rootTableKeyColumnNames, IdentifierType, " = ").ToSqlString();
		}

		protected SqlString GenerateInsertGeneratedValuesSelectString()
		{
			return GenerateGeneratedValuesSelectString(PropertyInsertGenerationInclusions);
		}

		protected SqlString GenerateUpdateGeneratedValuesSelectString()
		{
			return GenerateGeneratedValuesSelectString(PropertyUpdateGenerationInclusions);
		}

		private SqlString GenerateGeneratedValuesSelectString(ValueInclusion[] inclusions)
		{
			SqlSelectBuilder select = new SqlSelectBuilder(Factory);

			if (Factory.Settings.IsCommentsEnabled)
			{
				select.SetComment("get generated state " + EntityName);
			}

			string[] aliasedIdColumns = StringHelper.Qualify(RootAlias, IdentifierColumnNames);

			// Here we render the select column list based on the properties defined as being generated.
			// For partial component generation, we currently just re-select the whole component
			// rather than trying to handle the individual generated portions.
			string selectClause = ConcretePropertySelectFragment(RootAlias, inclusions);
			selectClause = selectClause.Substring(2);

			string fromClause = FromTableFragment(RootAlias) + FromJoinFragment(RootAlias, true, false);

			SqlString whereClause = new SqlString(
				SqlStringHelper.Join(new SqlString("=", Parameter.Placeholder, " and "), aliasedIdColumns),
				"=", Parameter.Placeholder,
				WhereJoinFragment(RootAlias, true, false));

			return select.SetSelectClause(selectClause)
				.SetFromClause(fromClause)
				.SetOuterJoins(SqlString.Empty, SqlString.Empty)
				.SetWhereClause(whereClause)
				.ToSqlString();
		}

		protected string ConcretePropertySelectFragment(string alias, ValueInclusion[] inclusions)
		{
			return ConcretePropertySelectFragment(alias, new NoneInclusionChecker(inclusions));
		}

		protected string ConcretePropertySelectFragment(string alias, bool[] includeProperty)
		{
			return ConcretePropertySelectFragment(alias, new FullInclusionChecker(includeProperty));
		}

		protected string ConcretePropertySelectFragment(string alias, IInclusionChecker inclusionChecker)
		{
			int propertyCount = PropertyNames.Length;
			int[] propertyTableNumbers = PropertyTableNumbersInSelect;
			SelectFragment frag = new SelectFragment(Factory.Dialect);
			for (int i = 0; i < propertyCount; i++)
			{
				if (inclusionChecker.IncludeProperty(i))
				{
					frag.AddColumns(GenerateTableAlias(alias, propertyTableNumbers[i]), propertyColumnNames[i], propertyColumnAliases[i]);
					frag.AddFormulas(GenerateTableAlias(alias, propertyTableNumbers[i]), propertyColumnFormulaTemplates[i], propertyColumnAliases[i]);
				}
			}
			return frag.ToFragmentString();
		}


		protected virtual SqlString GenerateSnapshotSelectString()
		{
			//TODO: should we use SELECT .. FOR UPDATE?

			SqlSelectBuilder select = new SqlSelectBuilder(Factory);

			if (Factory.Settings.IsCommentsEnabled)
			{
				select.SetComment("get current state " + EntityName);
			}

			string[] aliasedIdColumns = StringHelper.Qualify(RootAlias, IdentifierColumnNames);
			string selectClause = StringHelper.Join(StringHelper.CommaSpace, aliasedIdColumns)
														+ ConcretePropertySelectFragment(RootAlias, PropertyUpdateability);

			SqlString fromClause = new SqlString(
				FromTableFragment(RootAlias), 
				FromJoinFragment(RootAlias, true, false));

			SqlString joiner = new SqlString("=", Parameter.Placeholder, " and ");
			SqlString whereClause = new SqlString(
				SqlStringHelper.Join(joiner, aliasedIdColumns),
				"=", Parameter.Placeholder,
				WhereJoinFragment(RootAlias, true, false));

			// H3.2 the Snapshot is what we found in DB without take care on version
			//if (IsVersioned)
			//{
			//  whereClauseBuilder.Add(" and ")
			//    .Add(VersionColumnName)
			//    .Add("=")
			//    .AddParameter();
			//}

			return select.SetSelectClause(selectClause)
				.SetFromClause(fromClause)
				.SetOuterJoins(SqlString.Empty, SqlString.Empty)
				.SetWhereClause(whereClause)
				.ToSqlString();
		}

		public object ForceVersionIncrement(object id, object currentVersion, ISessionImplementor session)
		{
			if (!IsVersioned)
				throw new AssertionFailure("cannot force version increment on non-versioned entity");

			if (IsVersionPropertyGenerated)
			{
				// the difficulty here is exactly what do we update in order to
				// force the version to be incremented in the db...
				throw new HibernateException("LockMode.Force is currently not supported for generated version properties");
			}

			object nextVersion = VersionType.Next(currentVersion, session);
			if (log.IsDebugEnabled)
			{
				log.Debug("Forcing version increment [" +
					MessageHelper.InfoString(this, id, Factory) + "; " +
					VersionType.ToLoggableString(currentVersion, Factory) + " -> " +
					VersionType.ToLoggableString(nextVersion, Factory) + "]");
			}

			IExpectation expectation = Expectations.AppropriateExpectation(updateResultCheckStyles[0]);
			// todo : cache this sql...
			SqlCommandInfo versionIncrementCommand = GenerateVersionIncrementUpdateString();
			try
			{
				var st = session.Batcher.PrepareCommand(versionIncrementCommand.CommandType, versionIncrementCommand.Text, versionIncrementCommand.ParameterTypes);
				try
				{
					VersionType.NullSafeSet(st, nextVersion, 0, session);
					IdentifierType.NullSafeSet(st, id, 1, session);
					VersionType.NullSafeSet(st, currentVersion, 1 + IdentifierColumnSpan, session);
					Check(session.Batcher.ExecuteNonQuery(st), id, 0, expectation, st);
				}
				finally
				{
					session.Batcher.CloseCommand(st, null);
				}
			}
			catch (DbException sqle)
			{
				var exceptionContext = new AdoExceptionContextInfo
										{
											SqlException = sqle,
											Message = "could not retrieve version: " + MessageHelper.InfoString(this, id, Factory),
											Sql = VersionSelectString.ToString(),
											EntityName = EntityName,
											EntityId = id
										};
				throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, exceptionContext);
			}
			return nextVersion;
		}

		private SqlCommandInfo GenerateVersionIncrementUpdateString()
		{
			SqlUpdateBuilder update = new SqlUpdateBuilder(Factory.Dialect, Factory);
			update.SetTableName(GetTableName(0));
			if (Factory.Settings.IsCommentsEnabled)
			{
				update.SetComment("forced version increment");
			}
			update.AddColumn(VersionColumnName, VersionType);
			update.SetIdentityColumn(IdentifierColumnNames, IdentifierType);
			update.SetVersionColumn(new string[] { VersionColumnName }, VersionType);
			return update.ToSqlCommandInfo();
		}

		/// <summary>
		/// Retrieve the version number
		/// </summary>
		public object GetCurrentVersion(object id, ISessionImplementor session)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("Getting version: " + MessageHelper.InfoString(this, id, Factory));
			}
			using(new SessionIdLoggingContext(session.SessionId))
			try
			{
				var st = session.Batcher.PrepareQueryCommand(CommandType.Text, VersionSelectString, IdentifierType.SqlTypes(Factory));
				DbDataReader rs = null;
				try
				{
					IdentifierType.NullSafeSet(st, id, 0, session);
					rs = session.Batcher.ExecuteReader(st);
					if (!rs.Read())
					{
						return null;
					}
					if (!IsVersioned)
					{
						return this;
					}
					return VersionType.NullSafeGet(rs, VersionColumnName, session, null);
				}
				finally
				{
					session.Batcher.CloseCommand(st, rs);
				}
			}
			catch (DbException sqle)
			{
				var exceptionContext = new AdoExceptionContextInfo
										{
											SqlException = sqle,
											Message = "could not retrieve version: " + MessageHelper.InfoString(this, id, Factory),
											Sql = VersionSelectString.ToString(),
											EntityName = EntityName,
											EntityId = id
										};
				throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, exceptionContext);
			}
		}

		protected internal virtual void InitLockers()
		{
			lockers[LockMode.Read] = GenerateLocker(LockMode.Read);
			lockers[LockMode.Upgrade] = GenerateLocker(LockMode.Upgrade);
			lockers[LockMode.UpgradeNoWait] = GenerateLocker(LockMode.UpgradeNoWait);
			lockers[LockMode.Force] = GenerateLocker(LockMode.Force);
		}

		protected internal virtual ILockingStrategy GenerateLocker(LockMode lockMode)
		{
			return factory.Dialect.GetLockingStrategy(this, lockMode);
		}

		private ILockingStrategy GetLocker(LockMode lockMode)
		{
			try
			{
				return lockers[lockMode];
			}
			catch (KeyNotFoundException)
			{
				throw new HibernateException(string.Format("LockMode {0} not supported by {1}", lockMode, GetType().FullName));
			}
		}

		public virtual void Lock(object id, object version, object obj, LockMode lockMode, ISessionImplementor session)
		{
			GetLocker(lockMode).Lock(id, version, obj, session);
		}

		public virtual string GetRootTableAlias(string drivingAlias)
		{
			return drivingAlias;
		}

		public virtual string[] ToColumns(string alias, string propertyName)
		{
			return propertyMapping.ToColumns(alias, propertyName);
		}

		public string[] ToColumns(string propertyName)
		{
			return propertyMapping.GetColumnNames(propertyName);
		}

		public IType ToType(string propertyName)
		{
			return propertyMapping.ToType(propertyName);
		}

		public bool TryToType(string propertyName, out IType type)
		{
			return propertyMapping.TryToType(propertyName, out type);
		}

		public string[] GetPropertyColumnNames(string propertyName)
		{
			return propertyMapping.GetColumnNames(propertyName);
		}

		/// <remarks>
		/// Warning:
		/// When there are duplicated property names in the subclasses
		/// of the class, this method may return the wrong table
		/// number for the duplicated subclass property (note that
		/// SingleTableEntityPersister defines an overloaded form
		/// which takes the entity name.
		/// </remarks>
		public virtual int GetSubclassPropertyTableNumber(string propertyPath)
		{
			string rootPropertyName = StringHelper.Root(propertyPath);
			IType type = propertyMapping.ToType(rootPropertyName);
			if (type.IsAssociationType)
			{
				IAssociationType assocType = (IAssociationType)type;
				if (assocType.UseLHSPrimaryKey)
				{
					// performance op to avoid the array search
					return 0;
				}
				else if (type.IsCollectionType)
				{
					// properly handle property-ref-based associations
					rootPropertyName = assocType.LHSPropertyName;
				}
			}

			//Enable for HHH-440, which we don't like:
			/*if ( type.isComponentType() && !propertyName.equals(rootPropertyName) ) {
				String unrooted = StringHelper.unroot(propertyName);
				int idx = ArrayHelper.indexOf( getSubclassColumnClosure(), unrooted );
				if ( idx != -1 ) {
					return getSubclassColumnTableNumberClosure()[idx];
				}
			}*/
			int index = Array.IndexOf(SubclassPropertyNameClosure, rootPropertyName); //TODO: optimize this better!
			return index == -1 ? 0 : GetSubclassPropertyTableNumber(index);
		}

		public virtual Declarer GetSubclassPropertyDeclarer(string propertyPath)
		{
			int tableIndex = GetSubclassPropertyTableNumber(propertyPath);
			if (tableIndex == 0)
			{
				return Declarer.Class;
			}
			else if (IsClassOrSuperclassTable(tableIndex))
			{
				return Declarer.SuperClass;
			}
			else
			{
				return Declarer.SubClass;
			}
		}

		public virtual string GenerateTableAliasForColumn(string rootAlias, string column)
		{
			int propertyIndex = Array.IndexOf(SubclassColumnClosure, column);

			// The check for KeyColumnNames was added to fix NH-2491
			if (propertyIndex < 0 || Array.IndexOf(KeyColumnNames, column) >= 0)
			{
				return rootAlias;
			}
			return GenerateTableAlias(rootAlias, SubclassColumnTableNumberClosure[propertyIndex]);
		}

		public string GenerateTableAlias(string rootAlias, int tableNumber)
		{
			if (tableNumber == 0)
				return rootAlias;

			StringBuilder buf = new StringBuilder().Append(rootAlias);
			if (!rootAlias.EndsWith("_"))
			{
				buf.Append('_');
			}

			return buf.Append(tableNumber).Append('_').ToString();
		}

		public string[] ToColumns(string name, int i)
		{
			string alias = GenerateTableAlias(name, GetSubclassPropertyTableNumber(i));
			string[] cols = GetSubclassPropertyColumnNames(i);
			string[] templates = SubclassPropertyFormulaTemplateClosure[i];
			string[] result = new string[cols.Length];

			for (int j = 0; j < cols.Length; j++)
			{
				if (cols[j] == null)
				{
					result[j] = StringHelper.Replace(templates[j], Template.Placeholder, alias);
				}
				else
				{
					result[j] = StringHelper.Qualify(alias, cols[j]);
				}
			}
			return result;
		}

		public string[] ToIdentifierColumns(string name)
		{
			string alias = GenerateTableAlias(name, 0);
			string[] cols = IdentifierColumnNames;
			var result = new string[cols.Length];
			for (int j = 0; j < cols.Length; j++)
			{
				result[j] = StringHelper.Qualify(alias, cols[j]);
			}
			return result;
		}

		private int GetSubclassPropertyIndex(string propertyName)
		{
			return Array.IndexOf(subclassPropertyNameClosure, propertyName);
		}

		/// <summary>
		/// Get the column names for the numbered property of <em>this</em> class
		/// </summary>
		public string[] GetPropertyColumnNames(int i)
		{
			return propertyColumnNames[i];
		}

		protected int GetPropertyColumnSpan(int i)
		{
			return propertyColumnSpans[i];
		}

		protected bool HasFormulaProperties
		{
			get { return hasFormulaProperties; }
		}

		public FetchMode GetFetchMode(int i)
		{
			return subclassPropertyFetchModeClosure[i];
		}

		public CascadeStyle GetCascadeStyle(int i)
		{
			return subclassPropertyCascadeStyleClosure[i];
		}

		public IType GetSubclassPropertyType(int i)
		{
			return subclassPropertyTypeClosure[i];
		}

		public string GetSubclassPropertyName(int i)
		{
			return subclassPropertyNameClosure[i];
		}

		public int CountSubclassProperties()
		{
			return subclassPropertyTypeClosure.Length;
		}

		public string[] GetSubclassPropertyColumnNames(int i)
		{
			return subclassPropertyColumnNameClosure[i];
		}

		public bool IsDefinedOnSubclass(int i)
		{
			return propertyDefinedOnSubclass[i];
		}

		public string[] GetSubclassPropertyColumnAliases(string propertyName, string suffix)
		{
			string[] rawAliases;

			if (!subclassPropertyAliases.TryGetValue(propertyName,out rawAliases))
				return null;

			string[] result = new string[rawAliases.Length];
			for (int i = 0; i < rawAliases.Length; i++)
				result[i] = new Alias(suffix).ToUnquotedAliasString(rawAliases[i], Factory.Dialect);

			return result;
		}

		public string[] GetSubclassPropertyColumnNames(string propertyName)
		{
			//TODO: should we allow suffixes on these ?
			string[] result;
			subclassPropertyColumnNames.TryGetValue(propertyName, out result);
			return result;
		}

		/// <summary>
		/// Must be called by subclasses, at the end of their constructors
		/// </summary>
		protected void InitSubclassPropertyAliasesMap(PersistentClass model)
		{
			// ALIASES
			InternalInitSubclassPropertyAliasesMap(null, model.SubclassPropertyClosureIterator);

			// aliases for identifier ( alias.id ); skip if the entity defines a non-id property named 'id'
			if (!entityMetamodel.HasNonIdentifierPropertyNamedId)
			{
				subclassPropertyAliases[EntityPersister.EntityID] = identifierAliases;
				subclassPropertyColumnNames[EntityPersister.EntityID] = IdentifierColumnNames;
			}

			// aliases named identifier ( alias.idname )
			if (HasIdentifierProperty)
			{
				subclassPropertyAliases[IdentifierPropertyName] = IdentifierAliases;
				subclassPropertyColumnNames[IdentifierPropertyName] = IdentifierColumnNames;
			}

			// aliases for composite-id's
			if (IdentifierType.IsComponentType)
			{
				// Fetch embedded identifiers propertynames from the "virtual" identifier component
				IAbstractComponentType componentId = (IAbstractComponentType)IdentifierType;
				string[] idPropertyNames = componentId.PropertyNames;
				string[] idAliases = IdentifierAliases;
				string[] idColumnNames = IdentifierColumnNames;

				for (int i = 0; i < idPropertyNames.Length; i++)
				{
					if (entityMetamodel.HasNonIdentifierPropertyNamedId)
					{
						subclassPropertyAliases[EntityPersister.EntityID + "." + idPropertyNames[i]] = new string[] { idAliases[i] };
						subclassPropertyColumnNames[EntityPersister.EntityID + "." + IdentifierPropertyName + "." + idPropertyNames[i]] = new string[] { idColumnNames[i] };
					}
					//				if (hasIdentifierProperty() && !ENTITY_ID.equals( getIdentifierPropertyName() ) ) {
					if (HasIdentifierProperty)
					{
						subclassPropertyAliases[IdentifierPropertyName + "." + idPropertyNames[i]] = new string[] { idAliases[i] };
						subclassPropertyColumnNames[IdentifierPropertyName + "." + idPropertyNames[i]] = new string[] { idColumnNames[i] };
					}
					else
					{
						// embedded composite ids ( alias.idname1, alias.idname2 )
						subclassPropertyAliases[idPropertyNames[i]] = new string[] { idAliases[i] };
						subclassPropertyColumnNames[idPropertyNames[i]] = new string[] { idColumnNames[i] };
					}
				}
			}

			if (entityMetamodel.IsPolymorphic)
			{
				subclassPropertyAliases[EntityClass] = new string[] { DiscriminatorAlias };
				subclassPropertyColumnNames[EntityClass] = new string[] { DiscriminatorColumnName };
			}
		}

		private void InternalInitSubclassPropertyAliasesMap(string path, IEnumerable<Property> col)
		{
			foreach (Property prop in col)
			{
				string propName = path == null ? prop.Name : path + "." + prop.Name;
				if (prop.IsComposite)
				{
					Component component = (Component)prop.Value;
					InternalInitSubclassPropertyAliasesMap(propName, component.PropertyIterator);
				}
				else
				{
					string[] aliases = new string[prop.ColumnSpan];
					string[] cols = new string[prop.ColumnSpan];
					int l = 0;
					foreach (ISelectable thing in prop.ColumnIterator)
					{
						aliases[l] = thing.GetAlias(Factory.Dialect, prop.Value.Table);
						cols[l] = thing.GetText(Factory.Dialect);
						l++;
					}

					subclassPropertyAliases[propName] = aliases;
					subclassPropertyColumnNames[propName] = cols;
				}
			}
		}

		public object LoadByUniqueKey(string propertyName, object uniqueKey, ISessionImplementor session)
		{
			return GetAppropriateUniqueKeyLoader(propertyName, session.EnabledFilters).LoadByUniqueKey(session, uniqueKey);
		}

		private EntityLoader GetAppropriateUniqueKeyLoader(string propertyName, IDictionary<string, IFilter> enabledFilters)
		{
			//ugly little workaround for fact that createUniqueKeyLoaders() does not handle component properties
			bool useStaticLoader = (enabledFilters == null || (enabledFilters.Count == 0)) && propertyName.IndexOf('.') < 0;

			if (useStaticLoader)
			{
				return uniqueKeyLoaders[propertyName];
			}

			return CreateUniqueKeyLoader(propertyMapping.ToType(propertyName), propertyMapping.ToColumns(propertyName), enabledFilters);
		}

		public int GetPropertyIndex(string propertyName)
		{
			return entityMetamodel.GetPropertyIndex(propertyName);
		}

		protected void CreateUniqueKeyLoaders()
		{
			IType[] propertyTypes = PropertyTypes;
			string[] propertyNames = PropertyNames;

			for (int i = 0; i < entityMetamodel.PropertySpan; i++)
			{
				if (propertyUniqueness[i])
				{
					//don't need filters for the static loaders
					uniqueKeyLoaders[propertyNames[i]] =
						CreateUniqueKeyLoader(propertyTypes[i], GetPropertyColumnNames(i),
																	new CollectionHelper.EmptyMapClass<string, IFilter>());
				}
			}
		}

		private EntityLoader CreateUniqueKeyLoader(IType uniqueKeyType, string[] columns, IDictionary<string, IFilter> enabledFilters)
		{
			if (uniqueKeyType.IsEntityType)
			{
				string className = ((EntityType)uniqueKeyType).GetAssociatedEntityName();
				uniqueKeyType = Factory.GetEntityPersister(className).IdentifierType;
			}

			return new EntityLoader(this, columns, uniqueKeyType, 1, LockMode.None, Factory, enabledFilters);
		}

		protected string GetSQLWhereString(string alias)
		{
			return StringHelper.Replace(sqlWhereStringTemplate, Template.Placeholder, alias);
		}

		protected bool HasWhere
		{
			get { return !string.IsNullOrEmpty(sqlWhereString); }
		}

		private void InitOrdinaryPropertyPaths(IMapping mapping)
		{
			for (int i = 0; i < SubclassPropertyNameClosure.Length; i++)
			{
				propertyMapping.InitPropertyPaths(
					SubclassPropertyNameClosure[i],
					SubclassPropertyTypeClosure[i],
					SubclassPropertyColumnNameClosure[i],
					SubclassPropertyFormulaTemplateClosure[i],
					mapping);
			}
		}

		private void InitIdentifierPropertyPaths(IMapping mapping)
		{
			string idProp = IdentifierPropertyName;
			if (idProp != null)
			{
				propertyMapping.InitPropertyPaths(idProp, IdentifierType, IdentifierColumnNames, null, mapping);
			}
			if (entityMetamodel.IdentifierProperty.IsEmbedded)
			{
				propertyMapping.InitPropertyPaths(null, IdentifierType, IdentifierColumnNames, null, mapping);
			}
			if (!entityMetamodel.HasNonIdentifierPropertyNamedId)
			{
				propertyMapping.InitPropertyPaths(EntityPersister.EntityID, IdentifierType, IdentifierColumnNames, null, mapping);
			}
		}

		private void InitDiscriminatorPropertyPath()
		{
			propertyMapping.InitPropertyPaths(EntityClass, DiscriminatorType, new string[] {DiscriminatorColumnName},
											  new string[] {DiscriminatorFormulaTemplate}, Factory);
		}

		private void InitPropertyPaths(IMapping mapping)
		{
			InitOrdinaryPropertyPaths(mapping);
			InitOrdinaryPropertyPaths(mapping); // do two passes, for collection property-ref
			InitIdentifierPropertyPaths(mapping);
			if (entityMetamodel.IsPolymorphic)
				InitDiscriminatorPropertyPath();
		}

		protected IUniqueEntityLoader CreateEntityLoader(LockMode lockMode, IDictionary<string, IFilter> enabledFilters)
		{
			//TODO: disable batch loading if lockMode > READ?
			return BatchingEntityLoader.CreateBatchingEntityLoader(this, batchSize, lockMode, Factory, enabledFilters);
		}

		protected IUniqueEntityLoader CreateEntityLoader(LockMode lockMode)
		{
			return CreateEntityLoader(lockMode, new CollectionHelper.EmptyMapClass<string, IFilter>());
		}

		protected bool Check(int rows, object id, int tableNumber, IExpectation expectation, DbCommand statement)
		{
			try
			{
				expectation.VerifyOutcomeNonBatched(rows, statement);
			}
			catch (StaleStateException)
			{
				if (!IsNullableTable(tableNumber))
				{
					if (Factory.Statistics.IsStatisticsEnabled)
						Factory.StatisticsImplementor.OptimisticFailure(EntityName);

					throw new StaleObjectStateException(EntityName, id);
				}
			}
			catch (TooManyRowsAffectedException ex)
			{
				throw new HibernateException("Duplicate identifier in table for: " + MessageHelper.InfoString(this, id, Factory), ex);
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		protected virtual SqlCommandInfo GenerateUpdateString(bool[] includeProperty, int j, bool useRowId)
		{
			return GenerateUpdateString(includeProperty, j, null, useRowId);
		}

		/// <summary> Generate the SQL that updates a row by id (and version)</summary>
		protected internal SqlCommandInfo GenerateUpdateString(bool[] includeProperty, int j, object[] oldFields, bool useRowId)
		{
			SqlUpdateBuilder updateBuilder = new SqlUpdateBuilder(Factory.Dialect, Factory)
				.SetTableName(GetTableName(j));

			// select the correct row by either pk or rowid
			if (useRowId)
				updateBuilder.SetIdentityColumn(new[] {rowIdName}, NHibernateUtil.Int32); //TODO: eventually, rowIdName[j]
			else
				updateBuilder.SetIdentityColumn(GetKeyColumns(j), GetIdentifierType(j));

			bool hasColumns = false;
			for (int i = 0; i < entityMetamodel.PropertySpan; i++)
			{
				if (includeProperty[i] && IsPropertyOfTable(i, j))
				{
					// this is a property of the table, which we are updating
					updateBuilder.AddColumns(GetPropertyColumnNames(i), propertyColumnUpdateable[i], PropertyTypes[i]);
					hasColumns = hasColumns || GetPropertyColumnSpan(i) > 0;
				}
			}

			if (j == 0 && IsVersioned && entityMetamodel.OptimisticLockMode == Versioning.OptimisticLock.Version)
			{
				// this is the root (versioned) table, and we are using version-based
				// optimistic locking;  if we are not updating the version, also don't
				// check it (unless this is a "generated" version column)!
				if (CheckVersion(includeProperty))
				{
					updateBuilder.SetVersionColumn(new string[] { VersionColumnName }, VersionType);
					hasColumns = true;
				}
			}
			else if (entityMetamodel.OptimisticLockMode > Versioning.OptimisticLock.Version && oldFields != null)
			{
				// we are using "all" or "dirty" property-based optimistic locking
				bool[] includeInWhere =
					OptimisticLockMode == Versioning.OptimisticLock.All
						? PropertyUpdateability
						: includeProperty; //optimistic-lock="dirty", include all properties we are updating this time

				bool[] versionability = PropertyVersionability;
				IType[] types = PropertyTypes;

				for (int i = 0; i < entityMetamodel.PropertySpan; i++)
				{
					bool include = includeInWhere[i] && IsPropertyOfTable(i, j) && versionability[i];
					if (include)
					{
						// this property belongs to the table, and it is not specifically
						// excluded from optimistic locking by optimistic-lock="false"
						string[] _propertyColumnNames = GetPropertyColumnNames(i);
						bool[] propertyNullness = types[i].ToColumnNullness(oldFields[i], Factory);
						SqlType[] sqlt = types[i].SqlTypes(Factory);
						for (int k = 0; k < propertyNullness.Length; k++)
						{
							if (propertyNullness[k])
							{
								updateBuilder.AddWhereFragment(_propertyColumnNames[k], sqlt[k], " = ");
							}
							else
							{
								updateBuilder.AddWhereFragment(_propertyColumnNames[k] + " is null");
							}
						}
					}
				}
			}

			if (Factory.Settings.IsCommentsEnabled)
			{
				updateBuilder.SetComment("update " + EntityName);
			}

			return hasColumns ? updateBuilder.ToSqlCommandInfo() : null;
		}

		private bool CheckVersion(bool[] includeProperty)
		{
			return includeProperty[VersionProperty] || entityMetamodel.PropertyUpdateGenerationInclusions[VersionProperty] != ValueInclusion.None;
		}

		protected SqlCommandInfo GenerateInsertString(bool[] includeProperty, int j)
		{
			return GenerateInsertString(false, includeProperty, j);
		}

		protected SqlCommandInfo GenerateInsertString(bool identityInsert, bool[] includeProperty)
		{
			return GenerateInsertString(identityInsert, includeProperty, 0);
		}

		/// <summary> Generate the SQL that inserts a row</summary>
		protected virtual SqlCommandInfo GenerateInsertString(bool identityInsert, bool[] includeProperty, int j)
		{
			// todo : remove the identityInsert param and variations;
			//   identity-insert strings are now generated from generateIdentityInsertString()

			SqlInsertBuilder builder = new SqlInsertBuilder(Factory).SetTableName(GetTableName(j));

			// add normal properties
			for (int i = 0; i < entityMetamodel.PropertySpan; i++)
			{
				if (includeProperty[i] && IsPropertyOfTable(i, j))
				{
					// this property belongs on the table and is to be inserted
					builder.AddColumns(GetPropertyColumnNames(i), propertyColumnInsertable[i], PropertyTypes[i]);
				}
			}

			// add the discriminator
			if (j == 0)
			{
				AddDiscriminatorToInsert(builder);
			}

			// add the primary key
			if (j == 0 && identityInsert)
			{
				builder.AddIdentityColumn(GetKeyColumns(0)[0]);
			}
			else
			{
				builder.AddColumns(GetKeyColumns(j), null, GetIdentifierType(j));
			}

			if (Factory.Settings.IsCommentsEnabled)
			{
				builder.SetComment("insert " + EntityName);
			}

			// append the SQL to return the generated identifier
			if (j == 0 && identityInsert && UseInsertSelectIdentity())
			{
				SqlString sql = builder.ToSqlString();
				return new SqlCommandInfo(Factory.Dialect.AppendIdentitySelectToInsert(sql), builder.GetParametersTypeArray());
			}

			return builder.ToSqlCommandInfo();
		}

		protected virtual SqlCommandInfo GenerateIdentityInsertString(bool[] includeProperty)
		{
			SqlInsertBuilder insert = identityDelegate.PrepareIdentifierGeneratingInsert();
			insert.SetTableName(GetTableName(0));

			// add normal properties
			for (int i = 0; i < entityMetamodel.PropertySpan; i++)
			{
				if (includeProperty[i] && IsPropertyOfTable(i, 0))
				{
					// this property belongs on the table and is to be inserted
					insert.AddColumns(GetPropertyColumnNames(i), propertyColumnInsertable[i], PropertyTypes[i]);
				}
			}

			// add the discriminator
			AddDiscriminatorToInsert(insert);

			// delegate already handles PK columns
			if (Factory.Settings.IsCommentsEnabled)
			{
				insert.SetComment("insert " + EntityName);
			}

			return insert.ToSqlCommandInfo();
		}

		protected virtual SqlCommandInfo GenerateDeleteString(int j)
		{
			var deleteBuilder = new SqlDeleteBuilder(Factory.Dialect, Factory);
			deleteBuilder
				.SetTableName(GetTableName(j))
				.SetIdentityColumn(GetKeyColumns(j), GetIdentifierType(j));

			// NH: Only add version to where clause if optimistic lock mode is Version
			if (j == 0 && IsVersioned && entityMetamodel.OptimisticLockMode == Versioning.OptimisticLock.Version)
			{
				deleteBuilder.SetVersionColumn(new[] { VersionColumnName }, VersionType);
			}

			if (Factory.Settings.IsCommentsEnabled)
			{
				deleteBuilder.SetComment("delete " + EntityName);
			}
			return deleteBuilder.ToSqlCommandInfo();
		}

		protected int Dehydrate(object id, object[] fields, bool[] includeProperty, bool[][] includeColumns, int j, DbCommand st, ISessionImplementor session)
		{
			return Dehydrate(id, fields, null, includeProperty, includeColumns, j, st, session, 0);
		}

		/// <summary> Marshall the fields of a persistent instance to a prepared statement</summary>
		protected int Dehydrate(object id, object[] fields, object rowId, bool[] includeProperty, bool[][] includeColumns, int table,
			DbCommand statement, ISessionImplementor session, int index)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("Dehydrating entity: " + MessageHelper.InfoString(this, id, Factory));
			}

			// there's a pretty strong coupling between the order of the SQL parameter 
			// construction and the actual order of the parameter collection. 

			for (int i = 0; i < entityMetamodel.PropertySpan; i++)
			{
				if (includeProperty[i] && IsPropertyOfTable(i, table))
				{
					try
					{
						PropertyTypes[i].NullSafeSet(statement, fields[i], index, includeColumns[i], session);
						index += ArrayHelper.CountTrue(includeColumns[i]); //TODO:  this is kinda slow...
					}
					catch (Exception ex)
					{
						throw new PropertyValueException("Error dehydrating property value for", EntityName, entityMetamodel.PropertyNames[i], ex);
					}
				}
			}

			if (rowId != null)
			{
				// TODO H3.2 : support to set the rowId
				// TransactionManager.manager.SetObject(ps, index, rowId);
				// index += 1;
				throw new NotImplementedException("support to set the rowId");
			}
			else if (id != null)
			{
				var property = GetIdentiferProperty(table);
				property.Type.NullSafeSet(statement, id, index, session);
				index += property.Type.GetColumnSpan(factory);
			}

			return index;
		}

		/// <summary>
		/// Unmarshall the fields of a persistent instance from a result set,
		/// without resolving associations or collections
		/// </summary>
		public object[] Hydrate(DbDataReader rs, object id, object obj, ILoadable rootLoadable,
			string[][] suffixedPropertyColumns, bool allProperties, ISessionImplementor session)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("Hydrating entity: " + MessageHelper.InfoString(this, id, Factory));
			}

			AbstractEntityPersister rootPersister = (AbstractEntityPersister)rootLoadable;

			bool hasDeferred = rootPersister.HasSequentialSelect;
			DbCommand sequentialSelect = null;
			DbDataReader sequentialResultSet = null;
			bool sequentialSelectEmpty = false;
			using (new SessionIdLoggingContext(session.SessionId)) 
			try
			{
				if (hasDeferred)
				{
					SqlString sql = rootPersister.GetSequentialSelect(EntityName);
					if (sql != null)
					{
						//TODO: I am not so sure about the exception handling in this bit!
						sequentialSelect = session.Batcher.PrepareCommand(CommandType.Text, sql, IdentifierType.SqlTypes(factory));
						rootPersister.IdentifierType.NullSafeSet(sequentialSelect, id, 0, session);
						sequentialResultSet = session.Batcher.ExecuteReader(sequentialSelect);
						if (!sequentialResultSet.Read())
						{
							// TODO: Deal with the "optional" attribute in the <join> mapping;
							// this code assumes that optional defaults to "true" because it
							// doesn't actually seem to work in the fetch="join" code
							//
							// Note that actual proper handling of optional-ality here is actually
							// more involved than this patch assumes.  Remember that we might have
							// multiple <join/> mappings associated with a single entity.  Really
							// a couple of things need to happen to properly handle optional here:
							//  1) First and foremost, when handling multiple <join/>s, we really
							//      should be using the entity root table as the driving table;
							//      another option here would be to choose some non-optional joined
							//      table to use as the driving table.  In all likelihood, just using
							//      the root table is much simplier
							//  2) Need to add the FK columns corresponding to each joined table
							//      to the generated select list; these would then be used when
							//      iterating the result set to determine whether all non-optional
							//      data is present
							// My initial thoughts on the best way to deal with this would be
							// to introduce a new SequentialSelect abstraction that actually gets
							// generated in the persisters (ok, SingleTable...) and utilized here.
							// It would encapsulated all this required optional-ality checking...
							sequentialSelectEmpty = true;
						}
					}
				}

				string[] propNames = PropertyNames;
				IType[] types = PropertyTypes;
				object[] values = new object[types.Length];
				bool[] laziness = PropertyLaziness;
				string[] propSubclassNames = SubclassPropertySubclassNameClosure;

				for (int i = 0; i < types.Length; i++)
				{
					if (!propertySelectable[i])
					{
						values[i] = BackrefPropertyAccessor.Unknown;
					}
					else if (allProperties || !laziness[i])
					{
						//decide which ResultSet to get the property value from:
						bool propertyIsDeferred = hasDeferred
																			&& rootPersister.IsSubclassPropertyDeferred(propNames[i], propSubclassNames[i]);
						if (propertyIsDeferred && sequentialSelectEmpty)
						{
							values[i] = null;
						}
						else
						{
							var propertyResultSet = propertyIsDeferred ? sequentialResultSet : rs;
							string[] cols = propertyIsDeferred ? propertyColumnAliases[i] : suffixedPropertyColumns[i];
							values[i] = types[i].Hydrate(propertyResultSet, cols, session, obj);
						}
					}
					else
					{
						values[i] = LazyPropertyInitializer.UnfetchedProperty;
					}
				}

				if (sequentialResultSet != null)
				{
					sequentialResultSet.Close();
				}

				return values;
			}
			finally
			{
				if (sequentialSelect != null)
				{
					session.Batcher.CloseCommand(sequentialSelect, sequentialResultSet);
				}
			}
		}

		protected bool UseInsertSelectIdentity()
		{
			return !UseGetGeneratedKeys() && Factory.Dialect.SupportsInsertSelectIdentity;
		}

		protected bool UseGetGeneratedKeys()
		{
			return Factory.Settings.IsGetGeneratedKeysEnabled;
		}

		protected virtual SqlString GetSequentialSelect(string entityName)
		{
			throw new NotSupportedException("no sequential selects");
		}

		/// <summary>
		/// Perform an SQL INSERT, and then retrieve a generated identifier.
		/// </summary>
		/// <remarks>
		/// This form is used for PostInsertIdentifierGenerator-style ids (IDENTITY, select, etc).
		/// </remarks>
		protected object Insert(object[] fields, bool[] notNull, SqlCommandInfo sql, object obj, ISessionImplementor session)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("Inserting entity: " + EntityName + " (native id)");
				if (IsVersioned)
				{
					log.Debug("Version: " + Versioning.GetVersion(fields, this));
				}
			}
			IBinder binder = new GeneratedIdentifierBinder(fields, notNull, session, obj, this);
			return identityDelegate.PerformInsert(sql, session, binder);
		}

		public virtual SqlString GetSelectByUniqueKeyString(string propertyName)
		{
			return new SqlSimpleSelectBuilder(Factory.Dialect, Factory)
				.SetTableName(GetTableName(0))
				.AddColumns(GetKeyColumns(0))
				.AddWhereFragment(GetPropertyColumnNames(propertyName), GetPropertyType(propertyName), " = ").ToSqlString();
		}

		/// <summary>
		/// Perform an SQL INSERT.
		/// </summary>
		/// <remarks>
		/// This for is used for all non-root tables as well as the root table
		/// in cases where the identifier value is known before the insert occurs.
		/// </remarks>
		protected void Insert(object id, object[] fields, bool[] notNull, int j,
			SqlCommandInfo sql, object obj, ISessionImplementor session)
		{
			//check if the id comes from an alternate column
			object tableId = GetJoinTableId(j, fields) ?? id;

			if (IsInverseTable(j))
			{
				return;
			}

			//note: it is conceptually possible that a UserType could map null to
			//	  a non-null value, so the following is arguable:
			if (IsNullableTable(j) && IsAllNull(fields, j))
			{
				return;
			}

			if (log.IsDebugEnabled)
			{
				log.Debug("Inserting entity: " + MessageHelper.InfoString(this, tableId, Factory));
				if (j == 0 && IsVersioned)
				{
					log.Debug("Version: " + Versioning.GetVersion(fields, this));
				}
			}

			IExpectation expectation = Expectations.AppropriateExpectation(insertResultCheckStyles[j]);
			//bool callable = IsInsertCallable(j);
			// we can't batch joined inserts, *especially* not if it is an identity insert;
			// nor can we batch statements where the expectation is based on an output param
			bool useBatch = j == 0 && expectation.CanBeBatched;

			try
			{
				// Render the SQL query
				var insertCmd = useBatch
					? session.Batcher.PrepareBatchCommand(sql.CommandType, sql.Text, sql.ParameterTypes)
					: session.Batcher.PrepareCommand(sql.CommandType, sql.Text, sql.ParameterTypes);

				try
				{
					int index = 0;
					//index += expectation.Prepare(insertCmd, factory.ConnectionProvider.Driver);

					// Write the values of the field onto the prepared statement - we MUST use the
					// state at the time the insert was issued (cos of foreign key constraints).
					// Not necessarily the obect's current state

					Dehydrate(tableId, fields, null, notNull, propertyColumnInsertable, j, insertCmd, session, index);

					if (useBatch)
					{
						session.Batcher.AddToBatch(expectation);
					}
					else
					{
						expectation.VerifyOutcomeNonBatched(session.Batcher.ExecuteNonQuery(insertCmd), insertCmd);
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
						session.Batcher.CloseCommand(insertCmd, null);
					}
				}
			}
			catch (DbException sqle)
			{
				var exceptionContext = new AdoExceptionContextInfo
										{
											SqlException = sqle,
											Message = "could not insert: " + MessageHelper.InfoString(this, tableId),
											Sql = sql.ToString(),
											EntityName = EntityName,
											EntityId = tableId
										};
				throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, exceptionContext);
			}
		}

		/// <summary> Perform an SQL UPDATE or SQL INSERT</summary>
		protected internal virtual void UpdateOrInsert(object id, object[] fields, object[] oldFields, object rowId,
			bool[] includeProperty, int j, object oldVersion, object obj, SqlCommandInfo sql, ISessionImplementor session)
		{
			if (!IsInverseTable(j))
			{
				//check if the id comes from an alternate column
				object tableId = GetJoinTableId(j, fields) ?? id;

				bool isRowToUpdate;
				if (IsNullableTable(j) && oldFields != null && IsAllNull(oldFields, j))
				{
					//don't bother trying to update, we know there is no row there yet
					isRowToUpdate = false;
				}
				else if (IsNullableTable(j) && IsAllNull(fields, j))
				{
					//if all fields are null, we might need to delete existing row
					isRowToUpdate = true;
					Delete(tableId, oldVersion, j, obj, SqlDeleteStrings[j], session, null);
				}
				else
				{
					//there is probably a row there, so try to update
					//if no rows were updated, we will find out
					isRowToUpdate = Update(tableId, fields, oldFields, rowId, includeProperty, j, oldVersion, obj, sql, session);
				}

				if (!isRowToUpdate && !IsAllNull(fields, j))
				{
					// assume that the row was not there since it previously had only null
					// values, so do an INSERT instead
					//TODO: does not respect dynamic-insert
					Insert(tableId, fields, PropertyInsertability, j, SqlInsertStrings[j], obj, session);
				}
			}
		}

		protected bool Update(object id, object[] fields, object[] oldFields, object rowId, bool[] includeProperty, int j,
			object oldVersion, object obj, SqlCommandInfo sql, ISessionImplementor session)
		{
			bool useVersion = j == 0 && IsVersioned;
			IExpectation expectation = Expectations.AppropriateExpectation(updateResultCheckStyles[j]);
			//bool callable = IsUpdateCallable(j);
			bool useBatch = j == 0 && expectation.CanBeBatched && IsBatchable; //note: updates to joined tables can't be batched...

			if (log.IsDebugEnabled)
			{
				log.Debug("Updating entity: " + MessageHelper.InfoString(this, id, Factory));
				if (useVersion)
				{
					log.Debug("Existing version: " + oldVersion + " -> New Version: " + fields[VersionProperty]);
				}
			}

			try
			{
				int index = 0;
				var statement = useBatch
					? session.Batcher.PrepareBatchCommand(sql.CommandType, sql.Text, sql.ParameterTypes)
					: session.Batcher.PrepareCommand(sql.CommandType, sql.Text, sql.ParameterTypes);
				try
				{
					//index += expectation.Prepare(statement, factory.ConnectionProvider.Driver);

					//Now write the values of fields onto the prepared statement
					index = Dehydrate(id, fields, rowId, includeProperty, propertyColumnUpdateable, j, statement, session, index);

					// Write any appropriate versioning conditional parameters
					if (useVersion && Versioning.OptimisticLock.Version == entityMetamodel.OptimisticLockMode)
					{
						if (CheckVersion(includeProperty))
							VersionType.NullSafeSet(statement, oldVersion, index, session);
					}
					else if (entityMetamodel.OptimisticLockMode > Versioning.OptimisticLock.Version && oldFields != null)
					{
						bool[] versionability = PropertyVersionability;
						bool[] includeOldField = OptimisticLockMode == Versioning.OptimisticLock.All
																			? PropertyUpdateability
																			: includeProperty;
						IType[] types = PropertyTypes;

						for (int i = 0; i < entityMetamodel.PropertySpan; i++)
						{
							bool include = includeOldField[i] &&
														 IsPropertyOfTable(i, j) &&
														 versionability[i];
							if (include)
							{
								bool[] settable = types[i].ToColumnNullness(oldFields[i], Factory);
								types[i].NullSafeSet(statement, oldFields[i], index, settable, session);
								index += ArrayHelper.CountTrue(settable);
							}
						}
					}

					if (useBatch)
					{
						session.Batcher.AddToBatch(expectation);
						return true;
					}
					else
					{
						return Check(session.Batcher.ExecuteNonQuery(statement), id, j, expectation, statement);
					}
				}
				catch (StaleStateException e)
				{
					if (useBatch)
					{
						session.Batcher.AbortBatch(e);
					}

					throw new StaleObjectStateException(EntityName, id);
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
						session.Batcher.CloseCommand(statement, null);
					}
				}
			}
			catch (DbException sqle)
			{
				var exceptionContext = new AdoExceptionContextInfo
										{
											SqlException = sqle,
											Message = "could not update: " + MessageHelper.InfoString(this, id, Factory),
											Sql = sql.Text.ToString(),
																	EntityName = EntityName,
																	EntityId = id
										};
				throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, exceptionContext);
			}
		}

		/// <summary>
		/// Perform an SQL DELETE
		/// </summary>
		public void Delete(object id, object version, int j, object obj, SqlCommandInfo sql, ISessionImplementor session,
											 object[] loadedState)
		{
			//check if the id should come from another column
			object tableId = GetJoinTableId(j, obj) ?? id;

			if (IsInverseTable(j))
			{
				return;
			}

			// NH : Only use version if lock mode is Version
			bool useVersion = j == 0 && IsVersioned && Versioning.OptimisticLock.Version == entityMetamodel.OptimisticLockMode;

			//bool callable = IsDeleteCallable(j);
			IExpectation expectation = Expectations.AppropriateExpectation(deleteResultCheckStyles[j]);
			bool useBatch = j == 0 && expectation.CanBeBatched && IsBatchable;

			if (log.IsDebugEnabled)
			{
				log.Debug("Deleting entity: " + MessageHelper.InfoString(this, tableId, Factory));
				if (useVersion)
				{
					log.Debug("Version: " + version);
				}
			}

			if (IsTableCascadeDeleteEnabled(j))
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("delete handled by foreign key constraint: " + GetTableName(j));
				}
				return; //EARLY EXIT!
			}

			try
			{
				int index = 0;
				var statement = useBatch 
					? session.Batcher.PrepareBatchCommand(sql.CommandType, sql.Text, sql.ParameterTypes) 
					: session.Batcher.PrepareCommand(sql.CommandType, sql.Text, sql.ParameterTypes);

				try
				{
					//index += expectation.Prepare(statement, factory.ConnectionProvider.Driver);

					// Do the key. The key is immutable so we can use the _current_ object state - not necessarily
					// the state at the time the delete was issued
					var property = GetIdentiferProperty(j);
					property.Type.NullSafeSet(statement, tableId, index, session);
					index += property.Type.GetColumnSpan(factory);

					// We should use the _current_ object state (ie. after any updates that occurred during flush)
					if (useVersion)
					{
						VersionType.NullSafeSet(statement, version, index, session);
					}
					else if (entityMetamodel.OptimisticLockMode > Versioning.OptimisticLock.Version && loadedState != null)
					{
						bool[] versionability = PropertyVersionability;
						IType[] types = PropertyTypes;
						for (int i = 0; i < entityMetamodel.PropertySpan; i++)
						{
							if (IsPropertyOfTable(i, j) && versionability[i])
							{
								// this property belongs to the table and it is not specifically
								// excluded from optimistic locking by optimistic-lock="false"
								bool[] settable = types[i].ToColumnNullness(loadedState[i], Factory);

								types[i].NullSafeSet(statement, loadedState[i], index, settable, session);
								index += ArrayHelper.CountTrue(settable);
							}
						}
					}

					if (useBatch)
					{
						session.Batcher.AddToBatch(expectation);
					}
					else
					{
						Check(session.Batcher.ExecuteNonQuery(statement), tableId, j, expectation, statement);
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
						session.Batcher.CloseCommand(statement, null);
					}
				}
			}
			catch (DbException sqle)
			{
				var exceptionContext = new AdoExceptionContextInfo
										{
											SqlException = sqle,
											Message = "could not delete: " + MessageHelper.InfoString(this, tableId, Factory),
											Sql = sql.Text.ToString(),
											EntityName = EntityName,
											EntityId = tableId
										};
				throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, exceptionContext);
			}
		}

		private SqlCommandInfo[] GetUpdateStrings(bool byRowId, bool lazy)
		{
			if (byRowId)
				return lazy ? SQLLazyUpdateByRowIdStrings : SQLUpdateByRowIdStrings;
			else
				return lazy ? SQLLazyUpdateStrings : SqlUpdateStrings;
		}

		public void Update(object id, object[] fields, int[] dirtyFields, bool hasDirtyCollection,
			object[] oldFields, object oldVersion, object obj, object rowId, ISessionImplementor session)
		{
			//note: dirtyFields==null means we had no snapshot, and we couldn't get one using select-before-update
			//	  oldFields==null just means we had no snapshot to begin with (we might have used select-before-update to get the dirtyFields)

			bool[] tableUpdateNeeded = GetTableUpdateNeeded(dirtyFields, hasDirtyCollection);
			int span = TableSpan;
			bool[] propsToUpdate;
			SqlCommandInfo[] updateStrings;
			EntityEntry entry = session.PersistenceContext.GetEntry(obj);

			// Ensure that an immutable or non-modifiable entity is not being updated unless it is
			// in the process of being deleted.
			if (entry == null && !IsMutable)
				throw new InvalidOperationException("Updating immutable entity that is not in session yet!");
			
			if (entityMetamodel.IsDynamicUpdate && dirtyFields != null)
			{
				// For the case of dynamic-update="true", we need to generate the UPDATE SQL
				propsToUpdate = GetPropertiesToUpdate(dirtyFields, hasDirtyCollection);
				// don't need to check laziness (dirty checking algorithm handles that)
				updateStrings = new SqlCommandInfo[span];
				for (int j = 0; j < span; j++)
				{
					updateStrings[j] = tableUpdateNeeded[j]
															? GenerateUpdateString(propsToUpdate, j, oldFields, j == 0 && rowId != null)
															: null;
				}
			}
			else if (!IsModifiableEntity(entry))
			{
				// We need to generate UPDATE SQL when a non-modifiable entity (e.g., read-only or immutable)
				// needs:
				// - to have references to transient entities set to null before being deleted
				// - to have version incremented do to a "dirty" association
				// If dirtyFields == null, then that means that there are no dirty properties to
				// to be updated; an empty array for the dirty fields needs to be passed to
				// getPropertiesToUpdate() instead of null.
				propsToUpdate = this.GetPropertiesToUpdate(
					(dirtyFields == null ? ArrayHelper.EmptyIntArray : dirtyFields), hasDirtyCollection);
				
				// don't need to check laziness (dirty checking algorithm handles that)
				updateStrings = new SqlCommandInfo[span];
				for (int j = 0; j < span; j++)
				{
					updateStrings[j] = tableUpdateNeeded[j] ? GenerateUpdateString(propsToUpdate, j, oldFields, j == 0 && rowId != null) : null;
				}
			}
			else
			{
				// For the case of dynamic-update="false", or no snapshot, we use the static SQL
				updateStrings = GetUpdateStrings(rowId != null, HasUninitializedLazyProperties(obj));
				propsToUpdate = GetPropertyUpdateability(obj);
			}

			for (int j = 0; j < span; j++)
			{
				// Now update only the tables with dirty properties (and the table with the version number)
				if (tableUpdateNeeded[j])
				{
					UpdateOrInsert(id, fields, oldFields, j == 0 ? rowId : null, propsToUpdate, j, oldVersion, obj, updateStrings[j], session);
				}
			}
		}

		public object Insert(object[] fields, object obj, ISessionImplementor session)
		{
			int span = TableSpan;
			object id;

			if (entityMetamodel.IsDynamicInsert)
			{
				// For the case of dynamic-insert="true", we need to generate the INSERT SQL
				bool[] notNull = GetPropertiesToInsert(fields);
				id = Insert(fields, notNull, GenerateInsertString(true, notNull), obj, session);
				for (int j = 1; j < span; j++)
				{
					Insert(id, fields, notNull, j, GenerateInsertString(notNull, j), obj, session);
				}
			}
			else
			{
				// For the case of dynamic-insert="false", use the static SQL
				id = Insert(fields, PropertyInsertability, SQLIdentityInsertString, obj, session);
				for (int j = 1; j < span; j++)
				{
					Insert(id, fields, PropertyInsertability, j, SqlInsertStrings[j], obj, session);
				}
			}

			return id;
		}

		public void Insert(object id, object[] fields, object obj, ISessionImplementor session)
		{
			int span = TableSpan;
			if (entityMetamodel.IsDynamicInsert)
			{
				bool[] notNull = GetPropertiesToInsert(fields);
				// For the case of dynamic-insert="true", we need to generate the INSERT SQL
				for (int j = 0; j < span; j++)
				{
					Insert(id, fields, notNull, j, GenerateInsertString(notNull, j), obj, session);
				}
			}
			else
			{
				// For the case of dynamic-insert="false", use the static SQL
				for (int j = 0; j < span; j++)
				{
					Insert(id, fields, PropertyInsertability, j, SqlInsertStrings[j], obj, session);
				}
			}
		}

		public void Delete(object id, object version, object obj, ISessionImplementor session)
		{
			int span = TableSpan;
			bool isImpliedOptimisticLocking = !entityMetamodel.IsVersioned &&
																				entityMetamodel.OptimisticLockMode > Versioning.OptimisticLock.Version;
			object[] loadedState = null;
			if (isImpliedOptimisticLocking)
			{
				// need to treat this as if it where optimistic-lock="all" (dirty does *not* make sense);
				// first we need to locate the "loaded" state
				//
				// Note, it potentially could be a proxy, so perform the location the safe way...
				EntityKey key = session.GenerateEntityKey(id, this);
				object entity = session.PersistenceContext.GetEntity(key);
				if (entity != null)
				{
					EntityEntry entry = session.PersistenceContext.GetEntry(entity);
					loadedState = entry.LoadedState;
				}
			}

			SqlCommandInfo[] deleteStrings;
			if (isImpliedOptimisticLocking && loadedState != null)
			{
				// we need to utilize dynamic delete statements
				deleteStrings = GenerateSQLDeleteStrings(loadedState);
			}
			else
			{
				// otherwise, utilize the static delete statements
				deleteStrings = SqlDeleteStrings;
			}

			for (int j = span - 1; j >= 0; j--)
			{
				Delete(id, version, j, obj, deleteStrings[j], session, loadedState);
			}
		}

		protected SqlCommandInfo[] GenerateSQLDeleteStrings(object[] loadedState)
		{
			int span = TableSpan;
			SqlCommandInfo[] deleteStrings = new SqlCommandInfo[span];

			for (int j = span - 1; j >= 0; j--)
			{
				SqlDeleteBuilder delete = new SqlDeleteBuilder(Factory.Dialect, Factory)
					.SetTableName(GetTableName(j))
					.SetIdentityColumn(GetKeyColumns(j), GetIdentifierType(j));

				if (Factory.Settings.IsCommentsEnabled)
				{
					delete.SetComment("delete " + EntityName + " [" + j + "]");
				}

				bool[] versionability = PropertyVersionability;
				IType[] types = PropertyTypes;
				for (int i = 0; i < entityMetamodel.PropertySpan; i++)
				{
					bool include = versionability[i] &&
												 IsPropertyOfTable(i, j);

					if (include)
					{
						// this property belongs to the table and it is not specifically
						// excluded from optimistic locking by optimistic-lock="false"
						string[] _propertyColumnNames = GetPropertyColumnNames(i);
						bool[] propertyNullness = types[i].ToColumnNullness(loadedState[i], Factory);
						SqlType[] sqlt = types[i].SqlTypes(Factory);
						for (int k = 0; k < propertyNullness.Length; k++)
						{
							if (propertyNullness[k])
							{
								delete.AddWhereFragment(_propertyColumnNames[k], sqlt[k], " = ");
							}
							else
							{
								delete.AddWhereFragment(_propertyColumnNames[k] + " is null");
							}
						}
					}
				}
				deleteStrings[j] = delete.ToSqlCommandInfo();
			}

			return deleteStrings;
		}

		protected void LogStaticSQL()
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("Static SQL for entity: " + EntityName);
				if (sqlLazySelectString != null)
				{
					log.Debug(" Lazy select: " + sqlLazySelectString);
				}
				if (sqlVersionSelectString != null)
				{
					log.Debug(" Version select: " + sqlVersionSelectString);
				}
				if (sqlSnapshotSelectString != null)
				{
					log.Debug(" Snapshot select: " + sqlSnapshotSelectString);
				}
				for (int j = 0; j < TableSpan; j++)
				{
					log.Debug(" Insert " + j + ": " + SqlInsertStrings[j]);
					log.Debug(" Update " + j + ": " + SqlUpdateStrings[j]);
					log.Debug(" Delete " + j + ": " + SqlDeleteStrings[j]);
				}
				if (sqlIdentityInsertString != null)
				{
					log.Debug(" Identity insert: " + sqlIdentityInsertString);
				}
				if (sqlUpdateByRowIdString != null)
				{
					log.Debug(" Update by row id (all fields): " + sqlUpdateByRowIdString);
				}
				if (sqlLazyUpdateByRowIdString != null)
				{
					log.Debug(" Update by row id (non-lazy fields): " + sqlLazyUpdateByRowIdString);
				}
				if (sqlInsertGeneratedValuesSelectString != null)
				{
					log.Debug("Insert-generated property select: " + sqlInsertGeneratedValuesSelectString);
				}
				if (sqlUpdateGeneratedValuesSelectString != null)
				{
					log.Debug("Update-generated property select: " + sqlUpdateGeneratedValuesSelectString);
				}
			}
		}

		public virtual string FilterFragment(string alias, IDictionary<string, IFilter> enabledFilters)
		{
			StringBuilder sessionFilterFragment = new StringBuilder();

			filterHelper.Render(sessionFilterFragment, GenerateFilterConditionAlias(alias), GetColumnsToTableAliasMap(alias), enabledFilters);

			return sessionFilterFragment.Append(FilterFragment(alias)).ToString();
		}

		private IDictionary<string, string> GetColumnsToTableAliasMap(string rootAlias)
		{
			IDictionary<PropertyKey, string> propDictionary = new Dictionary<PropertyKey, string>();
			for (int i =0; i < SubclassPropertyNameClosure.Length; i++)
			{
				string property = SubclassPropertyNameClosure[i];
                string[] cols = GetSubclassPropertyColumnNames(property);

				if (cols != null && cols.Length > 0)
				{
					PropertyKey key = new PropertyKey(cols[0], GetSubclassPropertyTableNumber(i));
					propDictionary[key] = property;
				}
			}

			IDictionary<string, string> dict = new Dictionary<string, string>();
			for (int i = 0; i < SubclassColumnTableNumberClosure.Length; i++ )
			{
				string col = SubclassColumnClosure[i];
				string alias = GenerateTableAlias(rootAlias, SubclassColumnTableNumberClosure[i]);

				string fullColumn = string.Format("{0}.{1}", alias, col);

				PropertyKey key = new PropertyKey(col, SubclassColumnTableNumberClosure[i]);
				if (propDictionary.ContainsKey(key))
				{
					dict[propDictionary[key]] = fullColumn;
				}

				if (!dict.ContainsKey(col))
				{
					dict[col] = fullColumn;	
				}
			}

			return dict;
		}

		private class PropertyKey
		{
			public string Column { get; set; }
			public int TableNumber { get; set; }

			public PropertyKey(string column, int tableNumber)
			{
				Column = column;
				TableNumber = tableNumber;
			}

			public override int GetHashCode()
			{
				return Column.GetHashCode() ^ TableNumber.GetHashCode();
			}

			public override bool Equals(object other)
			{
				PropertyKey otherTuple = other as PropertyKey;
				return otherTuple == null ? false : Column.Equals(otherTuple.Column) && TableNumber.Equals(otherTuple.TableNumber);
			}
		}

		public virtual string GenerateFilterConditionAlias(string rootAlias)
		{
			return rootAlias;
		}

		public virtual string OneToManyFilterFragment(string alias)
		{
			return string.Empty;
		}

		public virtual SqlString FromJoinFragment(string alias, bool innerJoin, bool includeSubclasses)
		{
			return SubclassTableSpan == 1
				? new SqlString(string.Empty) // just a performance opt!
				: CreateJoin(alias, innerJoin, includeSubclasses).ToFromFragmentString;
		}

		public virtual SqlString WhereJoinFragment(string alias, bool innerJoin, bool includeSubclasses)
		{
			return
				SubclassTableSpan == 1 ? SqlString.Empty : CreateJoin(alias, innerJoin, includeSubclasses).ToWhereFragmentString;
		}

		protected internal virtual bool IsSubclassTableLazy(int j)
		{
			return false;
		}

		private JoinFragment CreateJoin(string name, bool innerjoin, bool includeSubclasses)
		{
			JoinFragment join = Factory.Dialect.CreateOuterJoinFragment();
			int tableSpan = SubclassTableSpan;
			for (int j = 1; j < tableSpan; j++) //notice that we skip the first table; it is the driving table!
			{
				string[] idCols = StringHelper.Qualify(name, GetJoinIdKeyColumns(j)); //some joins may be to non primary keys

				bool joinIsIncluded = IsClassOrSuperclassTable(j) ||
					(includeSubclasses && !IsSubclassTableSequentialSelect(j) && !IsSubclassTableLazy(j));
				if (joinIsIncluded)
				{
					join.AddJoin(GetSubclassTableName(j),
						GenerateTableAlias(name, j),
						idCols,
						GetSubclassTableKeyColumns(j),
						innerjoin && IsClassOrSuperclassTable(j) && !IsInverseTable(j) && !IsNullableTable(j)
							? JoinType.InnerJoin //we can inner join to superclass tables (the row MUST be there)
							: JoinType.LeftOuterJoin //we can never inner join to subclass tables
					);
				}
			}
			return join;
		}

		private JoinFragment CreateJoin(int[] tableNumbers, string drivingAlias)
		{
			string[] keyCols = StringHelper.Qualify(drivingAlias, GetSubclassTableKeyColumns(tableNumbers[0]));
			JoinFragment jf = Factory.Dialect.CreateOuterJoinFragment();
			for (int i = 1; i < tableNumbers.Length; i++)
			{ //skip the driving table
				int j = tableNumbers[i];
				jf.AddJoin(GetSubclassTableName(j),
						GenerateTableAlias(RootAlias, j),
						keyCols,
						GetSubclassTableKeyColumns(j),
						IsInverseSubclassTable(j) || IsNullableSubclassTable(j)
							? JoinType.LeftOuterJoin
							: JoinType.InnerJoin);
			}
			return jf;
		}

		protected SelectFragment CreateSelect(int[] subclassColumnNumbers, int[] subclassFormulaNumbers)
		{
			SelectFragment selectFragment = new SelectFragment(Factory.Dialect);

			int[] columnTableNumbers = SubclassColumnTableNumberClosure;
			string[] columnAliases = SubclassColumnAliasClosure;
			string[] columns = SubclassColumnClosure;
			for (int i = 0; i < subclassColumnNumbers.Length; i++)
			{
				if (subclassColumnSelectableClosure[i])
				{
					int columnNumber = subclassColumnNumbers[i];
					string subalias = GenerateTableAlias(RootAlias, columnTableNumbers[columnNumber]);
					selectFragment.AddColumn(subalias, columns[columnNumber], columnAliases[columnNumber]);
				}
			}

			int[] formulaTableNumbers = SubclassFormulaTableNumberClosure;
			String[] formulaTemplates = SubclassFormulaTemplateClosure;
			String[] formulaAliases = SubclassFormulaAliasClosure;
			for (int i = 0; i < subclassFormulaNumbers.Length; i++)
			{
				int formulaNumber = subclassFormulaNumbers[i];
				String subalias = GenerateTableAlias(RootAlias, formulaTableNumbers[formulaNumber]);
				selectFragment.AddFormula(subalias, formulaTemplates[formulaNumber], formulaAliases[formulaNumber]);
			}

			return selectFragment;
		}

		protected string CreateFrom(int tableNumber, String alias)
		{
			return GetSubclassTableName(tableNumber) + ' ' + alias;
		}

		protected SqlString CreateWhereByKey(int tableNumber, string alias)
		{
			//TODO: move to .sql package, and refactor with similar things!
			//return new SqlString(StringHelper.Join("= ? and ",
			//        StringHelper.Qualify(alias, GetSubclassTableKeyColumns(tableNumber))) + "= ?");
			string[] subclauses = StringHelper.Qualify(alias, GetSubclassTableKeyColumns(tableNumber));
			SqlStringBuilder builder = new SqlStringBuilder(subclauses.Length * 4);
			for (int i = 0; i < subclauses.Length; i++)
			{
				string subclause = subclauses[i];
				if (i > 0)
					builder.Add(" and ");

				builder.Add(subclause + "=").AddParameter();
			}
			return builder.ToSqlString();
		}

		protected SqlString RenderSelect(int[] tableNumbers, int[] columnNumbers, int[] formulaNumbers)
		{
			Array.Sort(tableNumbers); //get 'em in the right order (not that it really matters)

			//render the where and from parts
			int drivingTable = tableNumbers[0];
			string drivingAlias = GenerateTableAlias(RootAlias, drivingTable); //we *could* regenerate this inside each called method!
			SqlString where = CreateWhereByKey(drivingTable, drivingAlias);
			string from = CreateFrom(drivingTable, drivingAlias);

			//now render the joins
			JoinFragment jf = CreateJoin(tableNumbers, drivingAlias);

			//now render the select clause
			SelectFragment selectFragment = CreateSelect(columnNumbers, formulaNumbers);

			//now tie it all together
			SqlSelectBuilder select = new SqlSelectBuilder(Factory);
			select.SetSelectClause(selectFragment.ToFragmentString().Substring(2));
			select.SetFromClause(from);
			select.SetWhereClause(where);
			select.SetOuterJoins(jf.ToFromFragmentString, jf.ToWhereFragmentString);
			if (Factory.Settings.IsCommentsEnabled)
			{
				select.SetComment("sequential select " + EntityName);
			}
			return select.ToSqlString();
		}

		protected void PostConstruct(IMapping mapping)
		{
			InitPropertyPaths(mapping);

			// NH Different implementation
			// The creation of queries was moved from PostConstruct to PostInstantiate
			// because we need that the persister exist in the SessionFactory before investigate it
			// to know ParameterTypes of each query.
		}

		public virtual void PostInstantiate()
		{
			#region insert/update/delete SQLs

			// this section was moved from PostConstruct() method (know difference in NH)
			int joinSpan = TableSpan;
			sqlDeleteStrings = new SqlCommandInfo[joinSpan];
			sqlInsertStrings = new SqlCommandInfo[joinSpan];
			sqlUpdateStrings = new SqlCommandInfo[joinSpan];
			sqlLazyUpdateStrings = new SqlCommandInfo[joinSpan];

			sqlUpdateByRowIdString = rowIdName == null ? null : GenerateUpdateString(PropertyUpdateability, 0, true);
			sqlLazyUpdateByRowIdString = rowIdName == null ? null : GenerateUpdateString(NonLazyPropertyUpdateability, 0, true);

			for (int j = 0; j < joinSpan; j++)
			{
				SqlCommandInfo defaultInsert = GenerateInsertString(PropertyInsertability, j);
				SqlCommandInfo defaultUpdate = GenerateUpdateString(PropertyUpdateability, j, false);
				SqlCommandInfo defaultDelete = GenerateDeleteString(j);

				sqlInsertStrings[j] = customSQLInsert[j] != null
																? new SqlCommandInfo(customSQLInsert[j], defaultInsert.ParameterTypes)
																: defaultInsert;

				sqlUpdateStrings[j] = customSQLUpdate[j] != null
																? new SqlCommandInfo(customSQLUpdate[j], defaultUpdate.ParameterTypes)
																: defaultUpdate;

				// NH: in practice for lazy update de update sql is the same any way.
				sqlLazyUpdateStrings[j] = customSQLUpdate[j] != null
																		? new SqlCommandInfo(customSQLUpdate[j], defaultUpdate.ParameterTypes)
																		: GenerateUpdateString(NonLazyPropertyUpdateability, j, false);

				sqlDeleteStrings[j] = customSQLDelete[j] != null
																? new SqlCommandInfo(customSQLDelete[j], defaultDelete.ParameterTypes)
																: defaultDelete;
			}

			tableHasColumns = new bool[joinSpan];
			for (int j = 0; j < joinSpan; j++)
			{
				tableHasColumns[j] = sqlUpdateStrings[j] != null;
			}

			//select SQL
			sqlSnapshotSelectString = GenerateSnapshotSelectString();
			sqlLazySelectString = GenerateLazySelectString();
			sqlVersionSelectString = GenerateSelectVersionString();
			if (HasInsertGeneratedProperties)
			{
				sqlInsertGeneratedValuesSelectString = GenerateInsertGeneratedValuesSelectString();
			}
			if (HasUpdateGeneratedProperties)
			{
				sqlUpdateGeneratedValuesSelectString = GenerateUpdateGeneratedValuesSelectString();
			}
			if (IsIdentifierAssignedByInsert)
			{
				identityDelegate = ((IPostInsertIdentifierGenerator)IdentifierGenerator).GetInsertGeneratedIdentifierDelegate(this, Factory, UseGetGeneratedKeys());
				SqlCommandInfo defaultInsert = GenerateIdentityInsertString(PropertyInsertability);
				sqlIdentityInsertString = customSQLInsert[0] != null
																		? new SqlCommandInfo(customSQLInsert[0], defaultInsert.ParameterTypes)
																		: defaultInsert;
			}
			else
			{
				sqlIdentityInsertString = null;
			}

			LogStaticSQL();

			#endregion

			CreateLoaders();
			CreateUniqueKeyLoaders();
			CreateQueryLoader();
		}

		private void CreateLoaders()
		{
			loaders[LockMode.None.ToString()] = CreateEntityLoader(LockMode.None);
			IUniqueEntityLoader readLoader = CreateEntityLoader(LockMode.Read);
			loaders[LockMode.Read.ToString()] = readLoader;

			//TODO: inexact, what we really need to know is: are any outer joins used?
			bool disableForUpdate = SubclassTableSpan > 1 && HasSubclasses && !Factory.Dialect.SupportsOuterJoinForUpdate;

			loaders[LockMode.Upgrade.ToString()] = disableForUpdate ? readLoader : CreateEntityLoader(LockMode.Upgrade);
			loaders[LockMode.UpgradeNoWait.ToString()] = disableForUpdate ? readLoader : CreateEntityLoader(LockMode.UpgradeNoWait);
			loaders[LockMode.Force.ToString()] = disableForUpdate ? readLoader : CreateEntityLoader(LockMode.Force);

			loaders["merge"] = new CascadeEntityLoader(this, CascadingAction.Merge, Factory);
			loaders["refresh"] = new CascadeEntityLoader(this, CascadingAction.Refresh, Factory);
		}

		protected void CreateQueryLoader()
		{
			if (loaderName != null)
				queryLoader = new NamedQueryLoader(loaderName, this);
		}

		/// <summary>
		/// Load an instance using the appropriate loader (as determined by <see cref="GetAppropriateLoader" />
		/// </summary>
		public object Load(object id, object optionalObject, LockMode lockMode, ISessionImplementor session)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("Fetching entity: " + MessageHelper.InfoString(this, id, Factory));
			}

			IUniqueEntityLoader loader = GetAppropriateLoader(lockMode, session);
			return loader.Load(id, optionalObject, session);
		}

		private IUniqueEntityLoader GetAppropriateLoader(LockMode lockMode, ISessionImplementor session)
		{
			IDictionary<string, IFilter> enabledFilters = session.EnabledFilters;
			if (queryLoader != null)
			{
				return queryLoader;
			}
			else if (enabledFilters == null || enabledFilters.Count == 0)
			{
				if (!string.IsNullOrEmpty(session.FetchProfile) && LockMode.Upgrade.GreaterThan(lockMode))
				{
					return loaders[session.FetchProfile];
				}
				else
				{
					return loaders[lockMode.ToString()];
				}
			}
			else
			{
				return CreateEntityLoader(lockMode, enabledFilters);
			}
		}

		private bool IsAllNull(object[] array, int tableNumber)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (IsPropertyOfTable(i, tableNumber) && array[i] != null)
					return false;
			}
			return true;
		}

		public bool IsSubclassPropertyNullable(int i)
		{
			return subclassPropertyNullabilityClosure[i];
		}

		/// <summary> 
		/// Transform the array of property indexes to an array of booleans, true when the property is dirty
		/// </summary>
		protected virtual bool[] GetPropertiesToUpdate(int[] dirtyProperties, bool hasDirtyCollection)
		{
			bool[] propsToUpdate = new bool[entityMetamodel.PropertySpan];
			bool[] updateability = PropertyUpdateability; //no need to check laziness, dirty checking handles that
			for (int j = 0; j < dirtyProperties.Length; j++)
			{
				int property = dirtyProperties[j];
				if (updateability[property])
				{
					propsToUpdate[property] = true;
				}
			}
			if (IsVersioned)
			{
				propsToUpdate[VersionProperty] =
					PropertyUpdateability[VersionProperty] && 
					Versioning.IsVersionIncrementRequired(dirtyProperties, hasDirtyCollection, PropertyVersionability);
			}

			return propsToUpdate;
		}

		protected bool[] GetPropertiesToInsert(object[] fields)
		{
			bool[] notNull = new bool[fields.Length];
			bool[] insertable = PropertyInsertability;

			for (int i = 0; i < fields.Length; i++)
			{
				notNull[i] = insertable[i] && fields[i] != null;
			}

			return notNull;
		}

		public virtual int[] FindDirty(object[] currentState, object[] previousState, object entity, ISessionImplementor session)
		{
			int[] props = TypeHelper.FindDirty(
				entityMetamodel.Properties, currentState, previousState, propertyColumnUpdateable, HasUninitializedLazyProperties(entity), session);

			if (props == null)
			{
				return null;
			}
			else
			{
				LogDirtyProperties(props);
				return props;
			}
		}

		public virtual int[] FindModified(object[] old, object[] current, object entity, ISessionImplementor session)
		{
			int[] props = TypeHelper.FindModified(
				entityMetamodel.Properties, current, old, propertyColumnUpdateable, HasUninitializedLazyProperties(entity), session);
			if (props == null)
			{
				return null;
			}
			else
			{
				LogDirtyProperties(props);
				return props;
			}
		}

		/// <summary> Which properties appear in the SQL update? (Initialized, updateable ones!) </summary>
		protected bool[] GetPropertyUpdateability(object entity)
		{
			return HasUninitializedLazyProperties(entity) ? NonLazyPropertyUpdateability : PropertyUpdateability;
		}

		private void LogDirtyProperties(int[] props)
		{
			if (log.IsDebugEnabled)
			{
				for (int i = 0; i < props.Length; i++)
				{
					string propertyName = entityMetamodel.Properties[props[i]].Name;
					log.Debug(StringHelper.Qualify(EntityName, propertyName) + " is dirty");
				}
			}
		}

		protected internal IEntityTuplizer GetTuplizer(ISessionImplementor session)
		{
			return EntityTuplizer;
		}


		public virtual bool HasCache
		{
			get { return cache != null; }
		}

		private string GetSubclassEntityName(System.Type clazz)
		{
			string result;
			entityNameBySubclass.TryGetValue(clazz, out result);
			return result;
		}

		public virtual bool HasCascades
		{
			get { return entityMetamodel.HasCascades; }
		}

		public virtual bool HasIdentifierProperty
		{
			get { return !entityMetamodel.IdentifierProperty.IsVirtual; }
		}

		private IVersionType LocateVersionType()
		{
			return entityMetamodel.VersionProperty == null ? null : (IVersionType)entityMetamodel.VersionProperty.Type;
		}

		public virtual bool HasLazyProperties
		{
			get { return entityMetamodel.HasLazyProperties; }
		}

		public virtual void AfterReassociate(object entity, ISessionImplementor session)
		{
			if (FieldInterceptionHelper.IsInstrumented(entity))
			{
				IFieldInterceptor interceptor = FieldInterceptionHelper.ExtractFieldInterceptor(entity);
				if (interceptor != null)
				{
					interceptor.Session = session;
				}
				else
				{
					IFieldInterceptor fieldInterceptor = FieldInterceptionHelper.InjectFieldInterceptor(entity, EntityName, MappedClass, null, null, session);
					fieldInterceptor.MarkDirty();
				}
			}
		}

		public virtual bool? IsTransient(object entity, ISessionImplementor session)
		{
			object id;
			if (CanExtractIdOutOfEntity)
			{
				id = GetIdentifier(entity);
			}
			else
			{
				id = null;
			}
			// we *always* assume an instance with a null
			// identifier or no identifier property is unsaved!
			if (id == null)
			{
				return true;
			}

            // check the id unsaved-value
            // We do this first so we don't have to hydrate the version property if the id property already gives us the info we need (NH-3505).
            bool? result2 = entityMetamodel.IdentifierProperty.UnsavedValue.IsUnsaved(id);
            if (result2.HasValue)
            {
                if (IdentifierGenerator is Assigned)
                {
                    // if using assigned identifier, we can only make assumptions
                    // if the value is a known unsaved-value
                    if (result2.Value)
                        return true;
                }
                else
                {
                    return result2;
                }
            }

            // check the version unsaved-value, if appropriate
            if (IsVersioned)
            {
                object version = GetVersion(entity);
                bool? result = entityMetamodel.VersionProperty.UnsavedValue.IsUnsaved(version);
                if (result.HasValue)
                {
                    return result;
                }
            }

			// check to see if it is in the second-level cache
			if (HasCache)
			{
				CacheKey ck = session.GenerateCacheKey(id, IdentifierType, RootEntityName);
				if (Cache.Get(ck, session.Timestamp) != null)
					return false;
			}

			return null;
		}
		
		public virtual bool IsModifiableEntity(EntityEntry entry)
		{
			return (entry == null ? IsMutable : entry.IsModifiableEntity());
		}

		public virtual bool HasCollections
		{
			get { return entityMetamodel.HasCollections; }
		}

		public virtual bool HasMutableProperties
		{
			get { return entityMetamodel.HasMutableProperties; }
		}

		public virtual bool HasSubclasses
		{
			get { return entityMetamodel.HasSubclasses; }
		}

		public virtual bool HasProxy
		{
			get { return entityMetamodel.IsLazy; }
		}

		protected virtual bool UseDynamicUpdate
		{
			get { return entityMetamodel.IsDynamicUpdate; }
		}

		protected virtual bool UseDynamicInsert
		{
			get { return entityMetamodel.IsDynamicInsert; }
		}

		protected virtual bool HasEmbeddedCompositeIdentifier
		{
			get { return entityMetamodel.IdentifierProperty.IsEmbedded; }
		}

		public virtual bool CanExtractIdOutOfEntity
		{
			get { return HasIdentifierProperty || HasEmbeddedCompositeIdentifier || HasIdentifierMapper(); }
		}

		private bool HasIdentifierMapper()
		{
			return entityMetamodel.IdentifierProperty.HasIdentifierMapper;
		}

		public bool ConsumesEntityAlias()
		{
			return true;
		}

		public bool ConsumesCollectionAlias()
		{
			return false;
		}

		public virtual IType GetPropertyType(string path)
		{
			return propertyMapping.ToType(path);
		}

		protected Versioning.OptimisticLock OptimisticLockMode
		{
			get { return entityMetamodel.OptimisticLockMode; }
		}

		public object CreateProxy(object id, ISessionImplementor session)
		{
			return entityMetamodel.Tuplizer.CreateProxy(id, session);
		}

		public override string ToString()
		{
			return StringHelper.Unqualify(GetType().FullName) + '(' + entityMetamodel.Name + ')';
		}

		public string SelectFragment(IJoinable rhs, string rhsAlias, string lhsAlias,
			string entitySuffix, string collectionSuffix, bool includeCollectionColumns)
		{
			return SelectFragment(lhsAlias, entitySuffix);
		}

		public bool IsInstrumented
		{
			get { return EntityTuplizer.IsInstrumented; }
		}

		public bool HasInsertGeneratedProperties
		{
			get { return entityMetamodel.HasInsertGeneratedValues; }
		}

		public bool HasUpdateGeneratedProperties
		{
			get { return entityMetamodel.HasUpdateGeneratedValues; }
		}

		public void AfterInitialize(object entity, bool lazyPropertiesAreUnfetched, ISessionImplementor session)
		{
			EntityTuplizer.AfterInitialize(entity, lazyPropertiesAreUnfetched, session);
		}

		public virtual bool[] PropertyUpdateability
		{
			get { return entityMetamodel.PropertyUpdateability; }
		}

		public System.Type MappedClass
		{
			get { return EntityTuplizer.MappedClass; }
		}

		public bool ImplementsLifecycle
		{
			get { return EntityTuplizer.IsLifecycleImplementor; }
		}

		public bool ImplementsValidatable
		{
			get { return EntityTuplizer.IsValidatableImplementor; }
		}

		public System.Type ConcreteProxyClass
		{
			get { return EntityTuplizer.ConcreteProxyClass; }
		}

		public void SetPropertyValues(object obj, object[] values)
		{
			EntityTuplizer.SetPropertyValues(obj, values);
		}

		public void SetPropertyValue(object obj, int i, object value)
		{
			EntityTuplizer.SetPropertyValue(obj, i, value);
		}

		public object[] GetPropertyValues(object obj)
		{
			return EntityTuplizer.GetPropertyValues(obj);
		}

		public object GetPropertyValue(object obj, int i)
		{
			return EntityTuplizer.GetPropertyValue(obj, i);
		}

		public object GetPropertyValue(object obj, string propertyName)
		{
			return EntityTuplizer.GetPropertyValue(obj, propertyName);
		}

		public virtual object GetIdentifier(object obj)
		{
			return EntityTuplizer.GetIdentifier(obj);
		}

		public virtual void SetIdentifier(object obj, object id)
		{
			EntityTuplizer.SetIdentifier(obj, id);
		}

		public virtual object GetVersion(object obj)
		{
			return EntityTuplizer.GetVersion(obj);
		}

		public virtual object Instantiate(object id)
		{
			return EntityTuplizer.Instantiate(id);
		}

		/// <summary>
		/// Determines whether the specified entity is an instance of the class
		/// managed by this persister.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>
		/// 	<see langword="true"/> if the specified entity is an instance; otherwise, <see langword="false"/>.
		/// </returns>
		public bool IsInstance(object entity)
		{
			return EntityTuplizer.IsInstance(entity);
		}

		public virtual bool HasUninitializedLazyProperties(object obj)
		{
			return EntityTuplizer.HasUninitializedLazyProperties(obj);
		}

		public virtual void ResetIdentifier(object entity, object currentId, object currentVersion)
		{
			EntityTuplizer.ResetIdentifier(entity, currentId, currentVersion);
		}

		public IEntityPersister GetSubclassEntityPersister(object instance, ISessionFactoryImplementor factory)
		{
			if (!HasSubclasses)
			{
				return this;
			}
			// TODO : really need a way to do something like :
			//      getTuplizer(entityMode).determineConcreteSubclassEntityName(instance)
			var clazz = instance.GetType();
			if (clazz == MappedClass)
			{
				return this;
			}
			var subclassEntityName = GetSubclassEntityName(clazz);
			if (subclassEntityName == null || EntityName.Equals(subclassEntityName))
			{
				return this;
			}
			return factory.GetEntityPersister(subclassEntityName);
		}

		public virtual object[] GetPropertyValuesToInsert(object obj, IDictionary mergeMap, ISessionImplementor session)
		{
			return EntityTuplizer.GetPropertyValuesToInsert(obj, mergeMap, session);
		}

		public void ProcessInsertGeneratedProperties(object id, object entity, object[] state, ISessionImplementor session)
		{
			if (!HasInsertGeneratedProperties)
			{
				throw new AssertionFailure("no insert-generated properties");
			}

			session.Batcher.ExecuteBatch(); //force immediate execution of the insert

			if (loaderName == null)
			{
				ProcessGeneratedPropertiesWithGeneratedSql(id, entity, state, session, sqlInsertGeneratedValuesSelectString, PropertyInsertGenerationInclusions);
		}
			else
			{
				ProcessGeneratedPropertiesWithLoader(id, entity, session);

				// The loader has added the entity to the first-level cache. We must remove
				// the entity from the first-level cache to avoid problems in the Save or SaveOrUpdate
				// event listeners, which don't expect the entity to already be present in the 
				// first-level cache.
				session.PersistenceContext.RemoveEntity(session.GenerateEntityKey(id, this));
			}
		}

		public void ProcessUpdateGeneratedProperties(object id, object entity, object[] state, ISessionImplementor session)
		{
			if (!HasUpdateGeneratedProperties)
			{
				throw new AssertionFailure("no update-generated properties");
			}

			session.Batcher.ExecuteBatch(); //force immediate execution of the update

			if (loaderName == null)
			{
				ProcessGeneratedPropertiesWithGeneratedSql(id, entity, state, session, sqlUpdateGeneratedValuesSelectString, PropertyUpdateGenerationInclusions);
			}
			else
			{
				// Remove entity from first-level cache to ensure that loader fetches fresh data from database.
				// The loader will ensure that the same entity is added back to the first-level cache.
				session.PersistenceContext.RemoveEntity(session.GenerateEntityKey(id, this));
				ProcessGeneratedPropertiesWithLoader(id, entity, session);
			}
		}

		private void ProcessGeneratedPropertiesWithGeneratedSql(object id, object entity, object[] state,
			ISessionImplementor session, SqlString selectionSQL, ValueInclusion[] generationInclusions)
		{
			using (new SessionIdLoggingContext(session.SessionId)) 
			try
			{
				var cmd = session.Batcher.PrepareQueryCommand(CommandType.Text, selectionSQL, IdentifierType.SqlTypes(Factory));
				DbDataReader rs = null;
				try
				{
					IdentifierType.NullSafeSet(cmd, id, 0, session);
					rs = session.Batcher.ExecuteReader(cmd);
					if (!rs.Read())
					{
						throw new HibernateException("Unable to locate row for retrieval of generated properties: "
																				 + MessageHelper.InfoString(this, id, Factory));
					}
					for (int i = 0; i < PropertySpan; i++)
					{
						if (generationInclusions[i] != ValueInclusion.None)
						{
							object hydratedState = PropertyTypes[i].Hydrate(rs, GetPropertyAliases(string.Empty, i), session, entity);
							state[i] = PropertyTypes[i].ResolveIdentifier(hydratedState, session, entity);
							SetPropertyValue(entity, i, state[i]);
						}
					}
				}
				finally
				{
					session.Batcher.CloseCommand(cmd, rs);
				}
			}
			catch (DbException sqle)
			{
				var exceptionContext = new AdoExceptionContextInfo
										{
											SqlException = sqle,
											Message = "unable to select generated column values",
											Sql = selectionSQL.ToString(),
											EntityName = EntityName,
											EntityId = id
										};
				throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, exceptionContext);
			}
		}

		private void ProcessGeneratedPropertiesWithLoader(object id, object entity, ISessionImplementor session)
		{
			var query = (AbstractQueryImpl)session.GetNamedQuery(loaderName);
			if (query.HasNamedParameters)
			{
				query.SetParameter(query.NamedParameters[0], id, this.IdentifierType);
			}
			else
			{
				query.SetParameter(0, id, this.IdentifierType);
			}
			query.SetOptionalId(id);
			query.SetOptionalEntityName(this.EntityName);
			query.SetOptionalObject(entity);
			query.SetFlushMode(FlushMode.Manual);
			query.List();
		}

		public bool HasSubselectLoadableCollections
		{
			get { return hasSubselectLoadableCollections; }
		}

		public virtual object[] GetNaturalIdentifierSnapshot(object id, ISessionImplementor session)
		{
			if (!HasNaturalIdentifier)
			{
				throw new MappingException("persistent class did not define a natural-id : " + MessageHelper.InfoString(this));
			}
			if (log.IsDebugEnabled)
			{
				log.Debug("Getting current natural-id snapshot state for: " + MessageHelper.InfoString(this, id, Factory));
			}

			int[] naturalIdPropertyIndexes = NaturalIdentifierProperties;
			int naturalIdPropertyCount = naturalIdPropertyIndexes.Length;
			bool[] naturalIdMarkers = new bool[PropertySpan];
			IType[] extractionTypes = new IType[naturalIdPropertyCount];
			for (int i = 0; i < naturalIdPropertyCount; i++)
			{
				extractionTypes[i] = PropertyTypes[naturalIdPropertyIndexes[i]];
				naturalIdMarkers[naturalIdPropertyIndexes[i]] = true;
			}

			///////////////////////////////////////////////////////////////////////
			// TODO : look at perhaps caching this...
			SqlSelectBuilder select = new SqlSelectBuilder(Factory);
			if (Factory.Settings.IsCommentsEnabled)
			{
				select.SetComment("get current natural-id state " + EntityName);
			}
			select.SetSelectClause(ConcretePropertySelectFragmentSansLeadingComma(RootAlias, naturalIdMarkers));
			select.SetFromClause(FromTableFragment(RootAlias) + FromJoinFragment(RootAlias, true, false));

			string[] aliasedIdColumns = StringHelper.Qualify(RootAlias, IdentifierColumnNames);
			SqlString whereClause = new SqlString(
				SqlStringHelper.Join(new SqlString("=", Parameter.Placeholder, " and "), aliasedIdColumns),
				"=", Parameter.Placeholder,
				WhereJoinFragment(RootAlias, true, false));

			SqlString sql = select.SetOuterJoins(SqlString.Empty, SqlString.Empty).SetWhereClause(whereClause).ToStatementString();
			///////////////////////////////////////////////////////////////////////

			object[] snapshot = new object[naturalIdPropertyCount];
			using (new SessionIdLoggingContext(session.SessionId)) 
			try
			{
				var ps = session.Batcher.PrepareCommand(CommandType.Text, sql, IdentifierType.SqlTypes(factory));
				DbDataReader rs = null;
				try
				{
					IdentifierType.NullSafeSet(ps, id, 0, session);
					rs = session.Batcher.ExecuteReader(ps);
					//if there is no resulting row, return null
					if (!rs.Read())
					{
						return null;
					}

					for (int i = 0; i < naturalIdPropertyCount; i++)
					{
						snapshot[i] =
							extractionTypes[i].Hydrate(rs, GetPropertyAliases(string.Empty, naturalIdPropertyIndexes[i]), session, null);
						if (extractionTypes[i].IsEntityType)
						{
							snapshot[i] = extractionTypes[i].ResolveIdentifier(snapshot[i], session, null);
						}
					}
					return snapshot;
				}
				finally
				{
					session.Batcher.CloseCommand(ps, rs);
				}
			}
			catch (DbException sqle)
			{
				var exceptionContext = new AdoExceptionContextInfo
										{
											SqlException = sqle,
											Message = "could not retrieve snapshot: " + MessageHelper.InfoString(this, id, Factory),
											Sql = sql.ToString(),
											EntityName = EntityName,
											EntityId = id
										};
				throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, exceptionContext);
			}
		}

		protected string ConcretePropertySelectFragmentSansLeadingComma(string alias, bool[] include)
		{
			string concretePropertySelectFragment = ConcretePropertySelectFragment(alias, include);
			int firstComma = concretePropertySelectFragment.IndexOf(StringHelper.CommaSpace);
			if (firstComma == 0)
			{
				concretePropertySelectFragment = concretePropertySelectFragment.Substring(2);
			}
			return concretePropertySelectFragment;
		}

		public virtual bool HasNaturalIdentifier
		{
			get { return entityMetamodel.HasNaturalIdentifier; }
		}

		public virtual void SetPropertyValue(object obj, string propertyName, object value)
		{
			EntityTuplizer.SetPropertyValue(obj, propertyName, value);
		}

		public EntityMode EntityMode => entityMetamodel.EntityMode;

		public IEntityTuplizer EntityTuplizer => entityMetamodel.Tuplizer;

		public abstract string GetPropertyTableName(string propertyName);
		public abstract string FromTableFragment(string alias);
		public abstract string GetSubclassForDiscriminatorValue(object value);
		public abstract string GetSubclassPropertyTableName(int i);
		public abstract string TableName { get; }

		#region NH specific

		public bool? IsUnsavedVersion(object version)
		{
			if (!IsVersioned)
				return false;

			return entityMetamodel.VersionProperty.UnsavedValue.IsUnsaved(version);
		}

		private SqlType[] idAndVersionSqlTypes;
		public virtual SqlType[] IdAndVersionSqlTypes
		{
			get
			{
				if(idAndVersionSqlTypes == null)
					idAndVersionSqlTypes = IsVersioned
												? ArrayHelper.Join(IdentifierType.SqlTypes(factory), VersionType.SqlTypes(factory))
												: IdentifierType.SqlTypes(factory);
				return idAndVersionSqlTypes;
			}
		}

		public string GetInfoString()
		{
			return MessageHelper.InfoString(this);
		}
		#endregion
	}
}
