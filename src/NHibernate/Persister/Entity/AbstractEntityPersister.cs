using System;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Text;
using Iesi.Collections;
using log4net;
using NHibernate.AdoNet;
using NHibernate.Bytecode;
using NHibernate.Cache;
using NHibernate.Classic;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Id;
using NHibernate.Impl;
using NHibernate.Loader.Entity;
using NHibernate.Mapping;
using NHibernate.Metadata;
using NHibernate.Property;
using NHibernate.Proxy;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Tuple.Entity;
using NHibernate.Type;
using NHibernate.Util;
using Array=System.Array;
using Environment=NHibernate.Cfg.Environment;
using System.Collections.Generic;
using NHibernate.Dialect.Lock;
using Iesi.Collections.Generic;

namespace NHibernate.Persister.Entity
{
	/// <summary>
	/// Superclass for built-in mapping strategies. Implements functionalty common to both mapping
	/// strategies
	/// </summary>
	/// <remarks>
	/// May be considered an immutable view of the mapping object
	/// </remarks>
	public abstract class AbstractEntityPersister : IOuterJoinLoadable, IQueryable, IClassMetadata,
	                                                IUniqueKeyLoadable, ISqlLoadable, ILockable
	{
		private readonly ISessionFactoryImplementor factory;

		private static readonly ILog log = LogManager.GetLogger(typeof(AbstractEntityPersister));

		public const string EntityClass = "class";

		private readonly Dialect.Dialect dialect;
		//private readonly SQLExceptionConverter sqlExceptionConverter;

		// The class itself
		private readonly System.Type mappedClass;
		private readonly bool implementsLifecycle;
		private readonly bool implementsValidatable;
		private readonly int batchSize;
		private readonly ConstructorInfo constructor;

		// The optional SQL string defined in the where attribute
		private readonly string sqlWhereString;
		private readonly string sqlWhereStringTemplate;

		// proxies (if the proxies are interfaces, we use an array of interfaces of all subclasses)
		private readonly System.Type concreteProxyClass;
		private readonly IProxyFactory proxyFactory;

		// the identifier property
		private readonly bool hasEmbeddedIdentifier;

		private readonly string[] rootTableKeyColumnNames;
		private readonly string[] identifierAliases;
		private readonly int identifierColumnSpan;

		private readonly ISetter identifierSetter;
		private readonly IGetter identifierGetter;

		// version property
		private readonly string versionColumnName;
		private readonly IVersionType versionType;
		private readonly IGetter versionGetter;
		//private readonly bool batchVersionData;

		// other properties (for this concrete class only, not the subclass closure)
		private readonly int hydrateSpan;

		private readonly IGetter[] getters;
		private readonly ISetter[] setters;

		private readonly Hashtable gettersByPropertyName = new Hashtable();
		private readonly Hashtable settersByPropertyName = new Hashtable();
		//private readonly Hashtable typesByPropertyName = new Hashtable();

		// the cache
		private readonly ICacheConcurrencyStrategy cache;
		private readonly bool isLazyPropertiesCacheable;

		private readonly Hashtable uniqueKeyLoaders = new Hashtable();
		//private readonly Hashtable uniqueKeyColumns = new Hashtable();

		private readonly Hashtable subclassPropertyAliases = new Hashtable();
		private readonly Hashtable subclassPropertyColumnNames = new Hashtable();

		private readonly Dictionary<LockMode, ILockingStrategy> lockers = new Dictionary<LockMode, ILockingStrategy>();

		private readonly IReflectionOptimizer optimizer = null;

		private readonly EntityMetamodel entityMetamodel;

		//information about properties of this class,
		//including inherited properties
		//(only really needed for updatable/insertable properties)

		// the number of columns that the property spans
		// the array is indexed as propertyColumnSpans[propertyIndex] = ##
		private readonly int[] propertyColumnSpans;
		// the names of the columns for the property
		// the array is indexed as propertyColumnNames[propertyIndex][columnIndex] = "columnName"
		private readonly string[][] propertyColumnNames;
		// the alias names for the columns of the property.  This is used in the AS portion for 
		// selecting a column.  It is indexed the same as propertyColumnNames
		private readonly string[][] propertyColumnAliases;
		//private readonly string[ ] propertyFormulaTemplates;
		private readonly string[][] propertyColumnFormulaTemplates;

		private readonly bool[][] propertyColumnInsertable;
		private readonly bool[][] propertyColumnUpdateable;
		private readonly bool[] propertyUniqueness;
		private readonly System.Type[] propertySubclassNames;

		private readonly bool hasFormulaProperties;

		// the closure of all columns used by the entire hierarchy including
		// subclasses and superclasses of this class
		private readonly string[] subclassColumnClosure;
		private readonly string[] subclassColumnLazyClosure;
		private readonly string[] subclassColumnAliasClosure;
		private readonly bool[] subclassColumnSelectableClosure;
		private readonly string[] subclassFormulaTemplateClosure;
		private readonly string[] subclassFormulaClosure;
		private readonly string[] subclassFormulaAliasClosure;

		// the closure of all properties in the entire hierarchy including
		// subclasses and superclasses of this class
		private readonly string[] subclassPropertyNameClosure;
		private readonly System.Type[] subclassPropertySubclassNameClosure;
		private readonly IType[] subclassPropertyTypeClosure;
		private readonly string[][] subclassPropertyFormulaTemplateClosure;
		private readonly string[][] subclassPropertyColumnNameClosure;
		private readonly FetchMode[] subclassPropertyFetchModeClosure;
		private readonly bool[] subclassPropertyNullabilityClosure;

		private readonly FilterHelper filterHelper;

		// temporarily 'protected' instead of 'private readonly'
		protected bool[] propertyDefinedOnSubclass;

		private readonly Cascades.CascadeStyle[] subclassPropertyCascadeStyleClosure;
		private IDictionary loaders = new Hashtable();
		private SqlType[] idAndVersionSqlTypes;
		private SqlType[] idSqlTypes;

		// SQL strings
		private SqlCommandInfo[] sqlDeleteStrings;
		private SqlCommandInfo[] sqlInsertStrings;
		private SqlCommandInfo sqlIdentityInsertString;
		private SqlCommandInfo[] sqlUpdateStrings;
		private SqlString sqlSnapshotSelectString;
		private SqlString sqlVersionSelectString;

		private SqlString sqlInsertGeneratedValuesSelectString;
		private SqlString sqlUpdateGeneratedValuesSelectString;

		private bool[] tableHasColumns;

		protected SqlString[] customSQLInsert;
		protected SqlString[] customSQLUpdate;
		protected SqlString[] customSQLDelete;
		protected bool[] insertCallable;
		protected bool[] updateCallable;
		protected bool[] deleteCallable;
		protected ExecuteUpdateResultCheckStyle[] insertResultCheckStyles;
		protected ExecuteUpdateResultCheckStyle[] updateResultCheckStyles;
		protected ExecuteUpdateResultCheckStyle[] deleteResultCheckStyles;

		private readonly string loaderName;
		private IUniqueEntityLoader queryLoader;
		private bool hasSubselectLoadableCollections;

		protected readonly BasicEntityPropertyMapping propertyMapping;
		
		public abstract string GetSubclassTableName(int j);

		protected abstract string[] GetSubclassTableKeyColumns(int j);

		private ILockingStrategy GetLocker(LockMode lockMode)
		{
			try
			{
				return lockers[lockMode];
			}
			catch(KeyNotFoundException)
			{
				throw	new HibernateException(string.Format("LockMode {0} not supported by {1}", lockMode, GetType().FullName));
			}
		}

		public System.Type MappedClass
		{
			get { return mappedClass; }
		}

		// TODO: override
		public string ClassName
		{
			get { return entityMetamodel.Type.FullName; }
		}

		public string EntityName
		{
			get { return entityMetamodel.Name; }
		}

		public virtual object IdentifierSpace
		{
			get { return entityMetamodel.RootTypeAssemblyQualifiedName; }
		}

		public virtual string IdentifierSelectFragment(string name, string suffix)
		{
			return new SelectFragment(dialect)
				.SetSuffix(suffix)
				.AddColumns(name, IdentifierColumnNames, IdentifierAliases)
				.ToSqlStringFragment(false);
		}

		public virtual Cascades.CascadeStyle[] PropertyCascadeStyles
		{
			get { return entityMetamodel.CascadeStyles; }
		}

		/// <summary>
		/// Set the given values to the mapped properties of the given object
		/// </summary>
		/// <remarks>
		/// Use the access optimizer if available
		/// </remarks>
		public virtual void SetPropertyValues(object obj, object[] values)
		{
			if (optimizer != null && optimizer.AccessOptimizer != null)
			{
				try
				{
					optimizer.AccessOptimizer.SetPropertyValues(obj, values);
				}
				catch (InvalidCastException e)
				{
					throw new MappingException(
						"Invalid mapping information specified for type " + obj.GetType()
						+ ", check your mapping file for property type mismatches",
						e);
				}
			}
			else
			{
				for (int j = 0; j < HydrateSpan; j++)
				{
					Setters[j].Set(obj, values[j]);
				}
			}
		}

		/// <summary>
		/// Return the values of the mapped properties of the object
		/// </summary>
		/// <remarks>
		/// Uses the access optimizer, if available.
		/// </remarks>
		public virtual object[] GetPropertyValues(object obj)
		{
			if (optimizer != null && optimizer.AccessOptimizer != null)
			{
				return optimizer.AccessOptimizer.GetPropertyValues(obj);
			}
			else
			{
				int span = HydrateSpan;
				object[] result = new object[span];
				for (int j = 0; j < span; j++)
				{
					result[j] = Getters[j].Get(obj);
				}
				return result;
			}
		}

		/// <summary>
		/// Get the value of the numbered property
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		public virtual object GetPropertyValue(object obj, int i)
		{
			return Getters[i].Get(obj);
		}

		/// <summary>
		/// Set the value of the numbered property
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="i"></param>
		/// <param name="value"></param>
		public virtual void SetPropertyValue(object obj, int i, object value)
		{
			Setters[i].Set(obj, value);
		}

		private void LogDirtyProperties(int[] props)
		{
			if (log.IsDebugEnabled)
			{
				for (int i = 0; i < props.Length; i++)
				{
					string propertyName = entityMetamodel.Properties[props[i]].Name;
					log.Debug(StringHelper.Qualify(ClassName, propertyName) + " is dirty");
				}
			}
		}

		/// <summary>
		/// Determine if the given field values are dirty.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="obj"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public virtual int[] FindDirty(object[] x, object[] y, object obj, ISessionImplementor session)
		{
			int[] props = TypeFactory.FindDirty(
				entityMetamodel.Properties, x, y, propertyColumnUpdateable, false, session);

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

		/// <summary>
		/// Determine if the given field values are dirty.
		/// </summary>
		/// <param name="old"></param>
		/// <param name="current"></param>
		/// <param name="obj"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public virtual int[] FindModified(object[] old, object[] current, object obj, ISessionImplementor session)
		{
			int[] props = TypeFactory.FindModified(
				entityMetamodel.Properties, current, old, propertyColumnUpdateable, false, session);
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

		public virtual object GetIdentifier(object obj)
		{
			object id;
			if (HasEmbeddedIdentifier)
			{
				id = obj;
			}
			else
			{
				if (identifierGetter == null)
				{
					throw new HibernateException("The class has no identifier property: " + ClassName);
				}
				id = identifierGetter.Get(obj);
			}
			return id;
		}

		public virtual object GetVersion(object obj)
		{
			if (!IsVersioned)
			{
				return null;
			}
			return versionGetter.Get(obj);
		}

		public virtual void SetIdentifier(object obj, object id)
		{
			if (HasEmbeddedIdentifier)
			{
				ComponentType copier = (ComponentType) entityMetamodel.IdentifierProperty.Type;
				copier.SetPropertyValues(obj, copier.GetPropertyValues(id, EntityMode.Poco), EntityMode.Poco);
			}
			else if (identifierSetter != null)
			{
				identifierSetter.Set(obj, id);
			}
		}

		/// <summary>
		/// Return a new instance initialized with the given identifier.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public virtual object Instantiate(object id)
		{
			if (HasEmbeddedIdentifier && id.GetType() == mappedClass)
			{
				return id;
			}
			else
			{
				if (mappedClass.IsAbstract)
				{
					throw new HibernateException("Cannot instantiate abstract class or interface: " + ClassName);
				}

				object result;
				try
				{
					if (optimizer != null && optimizer.InstantiationOptimizer != null)
					{
						result = optimizer.InstantiationOptimizer.CreateInstance();
					}
					else
					{
						result = constructor.Invoke(null);
					}
				}
				catch (Exception e)
				{
					throw new InstantiationException("Could not instantiate entity: ", e, mappedClass);
				}

				SetIdentifier(result, id);
				return result;
			}
		}

		protected virtual ISetter[] Setters
		{
			get { return setters; }
		}

		protected virtual IGetter[] Getters
		{
			get { return getters; }
		}

		public virtual IType[] PropertyTypes
		{
			get { return entityMetamodel.PropertyTypes; }
		}

		public virtual IType IdentifierType
		{
			get { return entityMetamodel.IdentifierProperty.Type; }
		}

		public virtual string[] IdentifierColumnNames
		{
			get { return rootTableKeyColumnNames; }
		}

		public string[] IdentifierAliases
		{
			get { return identifierAliases; }
		}

		public virtual bool IsPolymorphic
		{
			get { return entityMetamodel.IsPolymorphic; }
		}

		public virtual bool IsInherited
		{
			get { return entityMetamodel.IsInherited; }
		}

		public virtual bool HasCascades
		{
			get { return entityMetamodel.HasCascades; }
		}

		public virtual ICacheConcurrencyStrategy Cache
		{
			get { return cache; }
		}

		public virtual bool HasIdentifierProperty
		{
			get { return identifierGetter != null; }
		}

		public virtual IVersionType VersionType
		{
			get { return versionType; }
		}

		public virtual int VersionProperty
		{
			get { return entityMetamodel.VersionPropertyIndex; }
		}

		public virtual bool IsVersioned
		{
			get { return entityMetamodel.IsVersioned; }
		}
		
		public bool IsBatchable
		{
			get { return /* jdbcBatchVersionedData || */ !IsVersioned; }
		}

		public virtual bool IsIdentifierAssignedByInsert
		{
			get { return entityMetamodel.IdentifierProperty.IsIdentifierAssignedByInsert; }
		}

		public bool IsUnsaved(object obj)
		{
			object id;
			if (CanExtractIdOutOfEntity)
			{
				id = GetIdentifier(obj);
			}
			else
			{
				id = null;
			}
			// we always assume a transient instance with a null
			// identifier or no identifier property is unsaved!
			if (id == null)
			{
				return true;
			}

			if (IsVersioned)
			{
				VersionValue unsavedVersionValue = entityMetamodel.VersionProperty.UnsavedValue;
				// let this take precedence if defined, since it works for
				// assigned identifiers
				object result = unsavedVersionValue.IsUnsaved(GetVersion(obj));
				if (result != null)
				{
					return (bool) result;
				}
			}
			return entityMetamodel.IdentifierProperty.UnsavedValue.IsUnsaved(id);
		}

		/// <summary></summary>
		public virtual string[] PropertyNames
		{
			get { return entityMetamodel.PropertyNames; }
		}

		/// <summary></summary>
		public virtual string IdentifierPropertyName
		{
			get { return entityMetamodel.IdentifierProperty.Name; }
		}

		#region ILockable Members
		public virtual string RootTableName
		{
			get { return GetSubclassTableName(0); }
		}

		public virtual string[] RootTableIdentifierColumnNames
		{
			get { return rootTableKeyColumnNames; }
		}

		/// <summary></summary>
		public virtual string VersionColumnName
		{
			get { return versionColumnName; }
		}

		public virtual string GetRootTableAlias(string drivingAlias)
		{
			return drivingAlias;
		}


		public virtual SqlType[] IdAndVersionSqlTypes
		{
			get { return idAndVersionSqlTypes; }
		}

		#endregion

		/// <summary></summary>
		public virtual bool ImplementsLifecycle
		{
			get { return implementsLifecycle; }
		}

		/// <summary></summary>
		public virtual bool ImplementsValidatable
		{
			get { return implementsValidatable; }
		}

		/// <summary></summary>
		public virtual bool HasCollections
		{
			get { return entityMetamodel.HasCollections; }
		}

		/// <summary></summary>
		public virtual bool IsMutable
		{
			get { return entityMetamodel.IsMutable; }
		}

		public virtual bool IsAbstract
		{
			get { return entityMetamodel.IsAbstract; }
		}

		/// <summary></summary>
		public virtual bool HasCache
		{
			get { return cache != null; }
		}

		public virtual bool HasSubclasses
		{
			get { return entityMetamodel.HasSubclasses; }
		}

		public virtual bool HasProxy
		{
			get { return entityMetamodel.IsLazy; }
		}

		/// <summary>
		/// Returns the SQL used to get the Identity value from the last insert.
		/// </summary>
		/// <remarks>This is not a NHibernate Command because the SQL contains no parameters.</remarks>
		public string SqlIdentitySelect(string identityColumn, string tableName)
		{
			return factory.Dialect.IdentitySelectString;
		}

		public virtual IIdentifierGenerator IdentifierGenerator
		{
			get { return entityMetamodel.IdentifierProperty.IdentifierGenerator; }
		}

		protected void Check(int rows, object id, int tableNumber, IExpectation expectation, IDbCommand statement)
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
					{
						Factory.StatisticsImplementor.OptimisticFailure(EntityName);
					}

					throw new StaleObjectStateException(MappedClass, id);
				}
			}
			catch (TooManyRowsAffectedException ex)
			{
				throw new HibernateException(
					"Duplicate identifier in table for: " +
					MessageHelper.InfoString(this, id, Factory),
					ex);
			}
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
			propertyMapping.InitPropertyPaths(EntityPersister.EntityID, IdentifierType, IdentifierColumnNames, null, mapping);
		}

		private void InitDiscriminatorPropertyPath(IMapping mapping)
		{
			propertyMapping.InitPropertyPaths(EntityClass,
				DiscriminatorType,
				new string[] { DiscriminatorColumnName },
				new string[] { DiscriminatorFormulaTemplate }, 
				Factory);
		}

		private void InitPropertyPaths(IMapping mapping)
		{
			InitOrdinaryPropertyPaths(mapping);
			// TODO: is calling InitOrdinaryPropertyPaths() twice needed here?
			InitOrdinaryPropertyPaths(mapping); // do two passes, for collection property-ref
			InitIdentifierPropertyPaths(mapping);
			if (entityMetamodel.IsPolymorphic)
			{
				InitDiscriminatorPropertyPath(mapping);
			}
		}

		protected AbstractEntityPersister(PersistentClass persistentClass, ICacheConcurrencyStrategy cache,
		                                  ISessionFactoryImplementor factory)
		{
			this.factory = factory;
			dialect = factory.Dialect;
			this.cache = cache;
			isLazyPropertiesCacheable = persistentClass.IsLazyPropertiesCacheable;
			//sqlExceptionConverter = factory.SQLExceptionConverter;

			entityMetamodel = new EntityMetamodel(persistentClass, factory);

			// CLASS
			mappedClass = persistentClass.MappedClass;

			sqlWhereString = persistentClass.Where;
			sqlWhereStringTemplate = sqlWhereString == null
			                         	?
			                         null
			                         	:
			                         Template.RenderWhereStringTemplate(sqlWhereString, Dialect, factory.SQLFunctionRegistry);

			batchSize = persistentClass.BatchSize;
			hasSubselectLoadableCollections = persistentClass.HasSubselectLoadableCollections;
			constructor = ReflectHelper.GetDefaultConstructor(mappedClass);

			// verify that the class has a default constructor if it is not abstract - it is considered
			// a mapping exception if the default ctor is missing.
			if (!entityMetamodel.IsAbstract && constructor == null)
			{
				throw new InstantiationException(
					"Object class " + mappedClass + " must declare a default (no-argument) constructor", mappedClass);
			}

			propertyMapping = new BasicEntityPropertyMapping(this);

			// IDENTIFIER
			hasEmbeddedIdentifier = persistentClass.HasEmbeddedIdentifier;
			IValue idValue = persistentClass.Identifier;

			if (persistentClass.HasIdentifierProperty)
			{
				Mapping.Property idProperty = persistentClass.IdentifierProperty;
				identifierSetter = idProperty.GetSetter(mappedClass);
				identifierGetter = idProperty.GetGetter(mappedClass);
			}
			else
			{
				identifierGetter = null;
				identifierSetter = null;
			}

			System.Type prox = persistentClass.ProxyInterface;
			MethodInfo proxySetIdentifierMethod = null;
			MethodInfo proxyGetIdentifierMethod = null;

			if (persistentClass.HasIdentifierProperty && prox != null)
			{
				Mapping.Property idProperty = persistentClass.IdentifierProperty;
				proxyGetIdentifierMethod = idProperty.GetGetter(prox).Method;
				proxySetIdentifierMethod = idProperty.GetSetter(prox).Method;
			}

			// HYDRATE SPAN
			hydrateSpan = persistentClass.PropertyClosureSpan;

			// IDENTIFIER 

			identifierColumnSpan = persistentClass.Identifier.ColumnSpan;
			rootTableKeyColumnNames = new string[identifierColumnSpan];
			identifierAliases = new string[identifierColumnSpan];

			loaderName = persistentClass.LoaderName;

			int i = 0;
			foreach (Column col in idValue.ColumnIterator)
			{
				rootTableKeyColumnNames[i] = col.GetQuotedName(factory.Dialect);
				identifierAliases[i] = col.GetAlias(Dialect, persistentClass.RootTable);
				i++;
			}

			// VERSION:

			if (persistentClass.IsVersioned)
			{
				foreach (Column col in persistentClass.Version.ColumnIterator)
				{
					versionColumnName = col.GetQuotedName(Dialect);
					break; //only happens once
				}
			}
			else
			{
				versionColumnName = null;
			}

			if (persistentClass.IsVersioned)
			{
				versionGetter = persistentClass.Version.GetGetter(mappedClass);
				versionType = (IVersionType) persistentClass.Version.Type;
			}
			else
			{
				versionGetter = null;
				versionType = null;
			}

			// PROPERTIES 

			getters = new IGetter[hydrateSpan];
			setters = new ISetter[hydrateSpan];
			string[] setterNames = new string[hydrateSpan];
			string[] getterNames = new string[hydrateSpan];

			i = 0;

			// NH: reflection optimizer works with custom accessors
			//bool foundCustomAccessor = false;

			foreach (Mapping.Property prop in persistentClass.PropertyClosureIterator)
			{
				//if( !prop.IsBasicPropertyAccessor )
				//{
				//	foundCustomAccessor = true;
				//}

				getters[i] = prop.GetGetter(mappedClass);
				setters[i] = prop.GetSetter(mappedClass);
				getterNames[i] = getters[i].PropertyName;
				setterNames[i] = setters[i].PropertyName;

				string propertyName = prop.Name;
				gettersByPropertyName[propertyName] = getters[i];
				settersByPropertyName[propertyName] = setters[i];

				i++;
			}

			// PROPERTIES (FROM ABSTRACTENTITYPERSISTER SUBCLASSES)
			propertyColumnNames = new string[HydrateSpan][];
			propertyColumnAliases = new string[HydrateSpan][];
			propertyColumnSpans = new int[HydrateSpan];
			propertyColumnFormulaTemplates = new string[HydrateSpan][];
			propertyColumnUpdateable = new bool[HydrateSpan][];
			propertyColumnInsertable = new bool[HydrateSpan][];
			propertyUniqueness = new bool[HydrateSpan];
			propertySubclassNames = new System.Type[HydrateSpan];

			HashedSet thisClassProperties = new HashedSet();
			i = 0;
			bool foundFormula = false;

			foreach (Mapping.Property prop in persistentClass.PropertyClosureIterator)
			{
				thisClassProperties.Add(prop);

				int span = prop.ColumnSpan;
				propertyColumnSpans[i] = span;
				propertySubclassNames[i] = prop.PersistentClass.MappedClass;
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
				propertyColumnInsertable[i] = prop.Value.ColumnInsertability;
				propertyColumnUpdateable[i] = prop.Value.ColumnUpdateability;
				propertyUniqueness[i] = prop.Value.IsAlternateUniqueKey;

				i++;
			}

			hasFormulaProperties = foundFormula;

			// NH: reflection optimizer works with custom accessors
			if (Environment.UseReflectionOptimizer)
			{
				optimizer = Environment.BytecodeProvider.GetReflectionOptimizer(MappedClass, Getters, Setters);
			}

			// SUBCLASS PROPERTY CLOSURE

			ArrayList columns = new ArrayList(); //this.subclassColumnClosure
			ArrayList columnsLazy = new ArrayList(); //this.subclassColumnClosure
			ArrayList aliases = new ArrayList();
			ArrayList formulaAliases = new ArrayList();
			ArrayList formulaTemplates = new ArrayList();
			ArrayList types = new ArrayList(); //this.subclassPropertyTypeClosure
			ArrayList names = new ArrayList(); //this.subclassPropertyNameClosure
			ArrayList classes = new ArrayList();
			ArrayList subclassTemplates = new ArrayList();
			ArrayList propColumns = new ArrayList(); //this.subclassPropertyColumnNameClosure
			ArrayList joinedFetchesList = new ArrayList(); //this.subclassPropertyEnableJoinedFetch
			ArrayList cascades = new ArrayList();
			ArrayList definedBySubclass = new ArrayList(); // this.propertyDefinedOnSubclass
			ArrayList formulas = new ArrayList();
			ArrayList columnSelectables = new ArrayList();
			ArrayList propNullables = new ArrayList();

			foreach (Mapping.Property prop in persistentClass.SubclassPropertyClosureIterator)
			{
				names.Add(prop.Name);
				classes.Add(prop.PersistentClass.MappedClass);
				bool isDefinedBySubclass = !thisClassProperties.Contains(prop);
				definedBySubclass.Add(isDefinedBySubclass);
				propNullables.Add(prop.IsOptional || isDefinedBySubclass); //TODO: is this completely correct?
				types.Add(prop.Type);

				string[] cols = new string[prop.ColumnSpan];
				string[] forms = new string[prop.ColumnSpan];
				int[] colnos = new int[prop.ColumnSpan];
				int[] formnos = new int[prop.ColumnSpan];
				int l = 0;
				// TODO H3: bool lazy = prop.IsLazy;

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
						// TODO H3: formulasLazy.add( lazy );
					}
					else
					{
						String colName = thing.GetTemplate(factory.Dialect, factory.SQLFunctionRegistry);
						colnos[l] = columns.Count; //before add :-)
						formnos[l] = -1;
						columns.Add(colName);
						cols[l] = colName;
						aliases.Add(thing.GetAlias(factory.Dialect, prop.Value.Table));
						// TODO H3: columnsLazy.add( lazy );
						columnSelectables.Add( prop.IsSelectable );
					}
					l++;
				}

				propColumns.Add(cols);
				subclassTemplates.Add(forms);
				//propColumnNumbers.Add( colnos );
				//propFormulaNumbers.Add( formnos );

				joinedFetchesList.Add(prop.Value.FetchMode);
				cascades.Add(prop.CascadeStyle);
			}

			subclassColumnClosure = (string[]) columns.ToArray(typeof (string));
			subclassColumnAliasClosure = (string[])aliases.ToArray(typeof(string));
			// TODO H3: subclassColumnLazyClosure = ArrayHelper.ToBooleanArray(columnsLazy);
			subclassColumnSelectableClosure = ArrayHelper.ToBooleanArray(columnSelectables);

			subclassFormulaClosure = (string[])formulas.ToArray(typeof(string));
			subclassFormulaTemplateClosure = (string[])formulaTemplates.ToArray(typeof(string));
			subclassFormulaAliasClosure = (string[])formulaAliases.ToArray(typeof(string));
			//subclassFormulaLazyClosure = ArrayHelper.ToBooleanArray(formulasLazy);

			subclassPropertyNameClosure = (string[])names.ToArray(typeof(string));
			subclassPropertySubclassNameClosure = (System.Type[])classes.ToArray(typeof(System.Type));
			subclassPropertyTypeClosure = (IType[])types.ToArray(typeof(IType));
			subclassPropertyNullabilityClosure = (bool[]) propNullables.ToArray(typeof (bool));
			subclassPropertyFormulaTemplateClosure = ArrayHelper.To2DStringArray(subclassTemplates);
			subclassPropertyColumnNameClosure = (string[][]) propColumns.ToArray(typeof(string[]));
			//subclassPropertyColumnNumberClosure = ArrayHelper.To2DIntArray(propColumnNumbers);
			//subclassPropertyFormulaNumberClosure = ArrayHelper.To2DIntArray(propFormulaNumbers);

			subclassPropertyCascadeStyleClosure = new Cascades.CascadeStyle[cascades.Count];
			int m = 0;
			foreach (Cascades.CascadeStyle cs in cascades)
			{
				subclassPropertyCascadeStyleClosure[m++] = cs;
			}

			subclassPropertyFetchModeClosure = new FetchMode[joinedFetchesList.Count];
			m = 0;
			foreach (FetchMode qq in joinedFetchesList)
			{
				subclassPropertyFetchModeClosure[m++] = qq;
			}

			propertyDefinedOnSubclass = new bool[definedBySubclass.Count];
			m = 0;
			foreach (bool val in definedBySubclass)
			{
				propertyDefinedOnSubclass[m++] = val;
			}

			// CALLBACK INTERFACES
			implementsLifecycle = typeof(ILifecycle).IsAssignableFrom(mappedClass);
			implementsValidatable = typeof(IValidatable).IsAssignableFrom(mappedClass);

			// PROXIES
			concreteProxyClass = persistentClass.ProxyInterface;
			bool hasProxy = concreteProxyClass != null;

			if (hasProxy)
			{
				HashedSet<System.Type> proxyInterfaces = new HashedSet<System.Type>();
				proxyInterfaces.Add(typeof(INHibernateProxy));

				if (!mappedClass.Equals(concreteProxyClass))
				{
					if (!concreteProxyClass.IsInterface)
					{
						throw new MappingException(
							"proxy must be either an interface, or the class itself: " +
							mappedClass.FullName);
					}

					proxyInterfaces.Add(concreteProxyClass);
				}

				if (mappedClass.IsInterface)
				{
					proxyInterfaces.Add(mappedClass);
				}

				if (HasProxy)
				{
					foreach (Subclass subclass in persistentClass.SubclassIterator)
					{
						System.Type subclassProxy = subclass.ProxyInterface;
						if (subclassProxy == null)
						{
							throw new MappingException("All subclasses must also have proxies: "
							                           + mappedClass.Name);
						}

						if (!subclass.MappedClass.Equals(subclassProxy))
						{
							proxyInterfaces.Add(subclassProxy);
						}
					}
				}

				if (HasProxy)
				{
					proxyFactory = CreateProxyFactory();
					proxyFactory.PostInstantiate(EntityName, mappedClass, proxyInterfaces, proxyGetIdentifierMethod,
					                             proxySetIdentifierMethod,
					                             persistentClass.HasEmbeddedIdentifier ? (IAbstractComponentType) persistentClass.Identifier.Type: null);
				}
				else
				{
					proxyFactory = null;
				}
			}
			else
			{
				proxyFactory = null;
			}

			// Handle any filters applied to the class level
			filterHelper = new FilterHelper(persistentClass.FilterMap, factory.Dialect, factory.SQLFunctionRegistry);
		}

		protected virtual IProxyFactory CreateProxyFactory()
		{
			return Environment.BytecodeProvider.ProxyFactoryFactory.BuildProxyFactory();
		}

		/// <summary>
		/// Must be called by subclasses, at the end of their constructors
		/// </summary>
		/// <param name="model"></param>
		protected void InitSubclassPropertyAliasesMap(PersistentClass model)
		{
			// ALIASES
			InternalInitSubclassPropertyAliasesMap(null, model.SubclassPropertyClosureIterator);

			// aliases for identifier
			if (HasIdentifierProperty)
			{
				subclassPropertyAliases[IdentifierPropertyName] = identifierAliases;
				subclassPropertyAliases[EntityPersister.EntityID] = identifierAliases;
			}

			if (HasEmbeddedIdentifier)
			{
				// Fetch embedded identifier property names from the "virtual" identifier component
				IAbstractComponentType componentId = (IAbstractComponentType) IdentifierType;
				string[] idPropertyNames = componentId.PropertyNames;
				string[] idAliases = IdentifierAliases;

				for (int i = 0; i < idPropertyNames.Length; i++)
				{
					subclassPropertyAliases[idPropertyNames[i]] = new string[] {idAliases[i]};
				}
			}

			if (IsPolymorphic)
			{
				subclassPropertyAliases[EntityClass] = new string[] {DiscriminatorAlias};
			}
		}

		private void InternalInitSubclassPropertyAliasesMap(string path, IEnumerable<Mapping.Property> col)
		{
			foreach (Mapping.Property prop in col)
			{
				string propName = path == null ? prop.Name : path + "." + prop.Name;
				if (prop.IsComposite)
				{
					Component component = (Component) prop.Value;
					InternalInitSubclassPropertyAliasesMap(propName, component.PropertyIterator);
				}
				else
				{
					string[] aliases = new string[prop.ColumnSpan];
					string[] cols = new string[prop.ColumnSpan];
					int l = 0;
					foreach (ISelectable thing in prop.ColumnIterator)
					{
						aliases[l] = thing.GetAlias(dialect, prop.Value.Table);
						cols[l] = thing.GetText(dialect);
						l++;
					}

					subclassPropertyAliases[propName] = aliases;
					subclassPropertyColumnNames[propName] = cols;
				}
			}
		}

		protected internal virtual void InitLockers()
		{
			lockers[LockMode.Read] = GenerateLocker(LockMode.Read);
			lockers[LockMode.Upgrade] = GenerateLocker(LockMode.Upgrade);
			lockers[LockMode.UpgradeNoWait] = GenerateLocker(LockMode.UpgradeNoWait);
			lockers[LockMode.Force] = GenerateLocker(LockMode.Force);
		}

		public virtual string RootEntityName
		{
			get { return entityMetamodel.RootName; }
		}

		public virtual IClassMetadata ClassMetadata
		{
			get { return this; }
		}

		public virtual System.Type ConcreteProxyClass
		{
			get { return concreteProxyClass; }
		}

		public virtual System.Type MappedSuperclass
		{
			get { return entityMetamodel.SuperclassType; }
		}

		public virtual bool IsExplicitPolymorphism
		{
			get { return entityMetamodel.IsExplicitPolymorphism; }
		}

		public virtual bool[] PropertyUpdateability
		{
			get { return entityMetamodel.PropertyUpdateability; }
		}

		public virtual bool[] PropertyCheckability
		{
			get { return entityMetamodel.PropertyCheckability; }
		}

		public virtual bool[] PropertyNullability
		{
			get { return entityMetamodel.PropertyNullability; }
		}

		protected virtual bool UseDynamicUpdate
		{
			get { return entityMetamodel.IsDynamicUpdate; }
		}

		public virtual bool[] PropertyInsertability
		{
			get { return entityMetamodel.PropertyInsertability; }
		}

		public virtual bool[] PropertyVersionability
		{
			get { return entityMetamodel.PropertyVersionability; }
		}

		public virtual bool[][] PropertyColumnInsertable
		{
			get { return this.propertyColumnInsertable; }
		}

		public virtual bool[][] PropertyColumnUpdateable
		{
			get { return this.propertyColumnUpdateable; }
		}

		public virtual object GetPropertyValue(object obj, string propertyName)
		{
			IGetter getter = (IGetter) gettersByPropertyName[propertyName];
			if (getter == null)
			{
				throw new HibernateException("unmapped property: " + mappedClass + "." + propertyName);
			}
			return getter.Get(obj);
		}

		public virtual void SetPropertyValue(object obj, string propertyName, object value)
		{
			ISetter setter = (ISetter) settersByPropertyName[propertyName];
			if (setter == null)
			{
				throw new HibernateException("unmapped property: " + mappedClass + "." + propertyName);
			}
			setter.Set(obj, value);
		}

		protected virtual bool HasEmbeddedIdentifier
		{
			get { return hasEmbeddedIdentifier; }
		}

		protected bool[] GetPropertiesToInsert(object[] fields)
		{
			bool[] notNull = new bool[fields.Length];
			bool[] insertable = PropertyInsertability;

			for (int i = 0; i < fields.Length; i++)
			{
				notNull[i] = insertable[i] && !PropertyTypes[i].IsDatabaseNull(fields[i]);
			}

			return notNull;
		}

		protected Dialect.Dialect Dialect
		{
			get { return dialect; }
		}

		protected string GetSQLWhereString(string alias)
		{
			return StringHelper.Replace(sqlWhereStringTemplate, Template.Placeholder, alias);
		}

		protected bool HasWhere
		{
			get { return (sqlWhereString != null && sqlWhereString.Length > 0); }
		}

		public virtual bool CanExtractIdOutOfEntity
		{
			get { return HasIdentifierProperty || HasEmbeddedIdentifier; }
		}

		protected void CheckColumnDuplication(ISet distinctColumns, ICollection columns)
		{
			foreach (Column col in columns)
			{
				if (!distinctColumns.Add(col.Name))
				{
					throw new MappingException(
						"Repeated column in mapping for class " +
						ClassName +
						" should be mapped with insert=\"false\" update=\"false\": " +
						col.Name);
				}
			}
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

		protected void CreateUniqueKeyLoaders()
		{
			IType[] propertyTypes = PropertyTypes;
			string[] propertyNames = PropertyNames;

			// TODO: Does not handle components, or properties of a joined subclass
			for (int i = 0; i < entityMetamodel.PropertySpan; i++)
			{
				if (propertyUniqueness[i])
				{
					//don't need filters for the static loaders
					uniqueKeyLoaders[propertyNames[i]] =
						CreateUniqueKeyLoader(
							propertyTypes[i],
							GetPropertyColumnNames(i),
							new CollectionHelper.EmptyMapClass<string, IFilter>()
							);
				}
			}
		}

		private EntityLoader CreateUniqueKeyLoader(IType uniqueKeyType, string[] columns, IDictionary<string, IFilter> enabledFilters)
		{
			if (uniqueKeyType.IsEntityType)
			{
				string type = ((EntityType) uniqueKeyType).GetAssociatedEntityName();
				uniqueKeyType = Factory.GetEntityPersister(type).IdentifierType;
			}

			return new EntityLoader(this, columns, uniqueKeyType, 1, LockMode.None, Factory, enabledFilters);
		}

		public IType Type
		{
			get { return entityMetamodel.EntityType; }
		}

		protected int HydrateSpan
		{
			get { return hydrateSpan; }
		}

		public bool IsBatchLoadable
		{
			get { return batchSize > 1; }
		}

		public string[] GetSubclassPropertyColumnAliases(string propertyName, string suffix)
		{
			string[] rawAliases = (string[]) subclassPropertyAliases[propertyName];

			if (rawAliases == null)
			{
				return null;
			}

			string[] result = new string[rawAliases.Length];
			for (int i = 0; i < rawAliases.Length; i++)
			{
				result[i] = new Alias(suffix).ToUnquotedAliasString(rawAliases[i], Dialect);
			}
			return result;
		}

		public string[] KeyColumnNames
		{
			get { return IdentifierColumnNames; }
		}

		public string Name
		{
			get { return ClassName; }
		}

		public string SelectFragment(string alias, string suffix)
		{
			return IdentifierSelectFragment(alias, suffix) +
			       PropertySelectFragment(alias, suffix, true);
		}

		public string[] GetIdentifierAliases(string suffix)
		{
			// NOTE: this assumes something about how PropertySelectFragment is implemented by the subclass!
			// was toUnqotedAliasStrings( getIdentiferColumnNames() ) before - now tried
			// to remove that unquoting and missing aliases..
			return new Alias(suffix).ToAliasStrings(IdentifierAliases, dialect);
		}

		public string[] GetPropertyAliases(string suffix, int i)
		{
			// NOTE: this assumes something about how pPropertySelectFragment is implemented by the subclass!
			return new Alias(suffix).ToUnquotedAliasStrings(propertyColumnAliases[i], dialect);
		}

		public string GetDiscriminatorAlias(string suffix)
		{
			// NOTE: this assumes something about how PropertySelectFragment is implemented by the subclass!
			// was toUnqotedAliasStrings( getdiscriminatorColumnName() ) before - now tried
			// to remove that unquoting and missing aliases..		
			return HasSubclasses
			       	?
			       new Alias(suffix).ToAliasString(DiscriminatorAlias, dialect)
			       	:
			       null;
		}

		protected abstract string DiscriminatorAlias { get; }

		public object LoadByUniqueKey(string propertyName, object uniqueKey, ISessionImplementor session)
		{
			return ((EntityLoader) uniqueKeyLoaders[propertyName]).LoadByUniqueKey(session, uniqueKey);
		}

		public bool IsCollection
		{
			get { return false; }
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

		protected bool HasSelectBeforeUpdate
		{
			get { return entityMetamodel.IsSelectBeforeUpdate; }
		}

		/// <summary>
		/// Retrieve the version number
		/// </summary>
		/// <param name="id"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public object GetCurrentVersion(object id, ISessionImplementor session)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("Getting version: " + MessageHelper.InfoString(this, id));
			}

			SqlString sql = VersionSelectString;
			try
			{
				IDbCommand st = session.Batcher.PrepareQueryCommand(CommandType.Text, sql, idSqlTypes);
				IDataReader rs = null;
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
			catch (HibernateException)
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch (Exception sqle)
			{
				throw Convert(sqle, "could not retrieve version: " + MessageHelper.InfoString(this, id), sql);
			}
		}

		public object ForceVersionIncrement(object id, object currentVersion, ISessionImplementor session)
		{
			if (!IsVersioned)
			{
				throw new AssertionFailure("cannot force version increment on non-versioned entity");
			}

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
				IDbCommand st = session.Batcher.PrepareCommand(versionIncrementCommand.CommandType, versionIncrementCommand.Text, versionIncrementCommand.ParameterTypes);
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
			catch (HibernateException)
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch (Exception sqle)
			{
				throw Convert(sqle, "could not retrieve version: " + MessageHelper.InfoString(this, id), VersionSelectString);
			}
			return nextVersion;
		}

		private SqlCommandInfo GenerateVersionIncrementUpdateString()
		{
			SqlUpdateBuilder update = new SqlUpdateBuilder(Factory.Dialect, Factory);
			update.SetTableName(GetTableName(0));
			//if (Factory.Settings.IsCommentsEnabled)
			//{
			//  update.SetComment("forced version increment");
			//}
			update.AddColumns(new string[] { VersionColumnName }, VersionType);
			update.SetIdentityColumn(GetKeyColumns(0), IdentifierType);
			update.SetVersionColumn(new string[] { VersionColumnName }, VersionType);
			return update.ToSqlCommandInfo();
		}

		/// <summary>
		/// Do a version check
		/// </summary>
		public virtual void Lock(object id, object version, object obj, LockMode lockMode, ISessionImplementor session)
		{
			GetLocker(lockMode).Lock(id, version, obj, session);
		}

		protected object GetGeneratedIdentity(object obj, ISessionImplementor session, IDataReader rs)
		{
			object id;

			try
			{
				if (!rs.Read())
				{
					throw new HibernateException("The database returned no natively generated identity value");
				}
				id = IdentifierGeneratorFactory.Get(rs, IdentifierType, session);
			}
			finally
			{
				rs.Close();
			}

			if (log.IsDebugEnabled)
			{
				log.Debug("Natively generated identity: " + id);
			}

			return id;
		}

		public object[] GetDatabaseSnapshot(object id, ISessionImplementor session)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("Getting current persistent state for: " + MessageHelper.InfoString(this, id));
			}

			IType[] types = PropertyTypes;
			object[] values = new object[types.Length];
			bool[] includeProperty = PropertyUpdateability;
			SqlString sql = SQLSnapshotSelectString;
			try
			{
				IDbCommand st = session.Batcher.PrepareCommand(CommandType.Text, sql, idSqlTypes);
				IDataReader rs = null;
				try
				{
					IdentifierType.NullSafeSet(st, id, 0, session);
					// H3.2 don't use the version for snapshot
					//if (IsVersioned)
					//{
					//  VersionType.NullSafeSet(st, version, IdentifierColumnNames.Length, session);
					//}
					rs = session.Batcher.ExecuteReader(st);
					if (!rs.Read())
					{
						throw new StaleObjectStateException(MappedClass, id);
					}
					for (int i = 0; i < types.Length; i++)
					{
						if (includeProperty[i])
						{
							values[i] = types[i].Hydrate(rs, GetPropertyAliases(string.Empty, i), session, null); //null owner ok??
						}
					}
				}
				finally
				{
					session.Batcher.CloseCommand(st, rs);
				}
			}
			catch (HibernateException)
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch (Exception sqle)
			{
				throw Convert(sqle, "error retrieving current persistent state", sql);
			}

			return values;
		}

		protected abstract string VersionedTableName { get; }

		/// <summary>
		/// Generate the SQL that selects the version number by id
		/// </summary>
		/// <returns></returns>
		protected SqlString GenerateSelectVersionString()
		{
			SqlSimpleSelectBuilder builder = new SqlSimpleSelectBuilder(factory.Dialect, factory);
			builder.SetTableName(VersionedTableName);

			if (IsVersioned)
			{
				builder.AddColumn(versionColumnName);
			}
			else
			{
				builder.AddColumns(rootTableKeyColumnNames);
			}

			builder.AddWhereFragment(rootTableKeyColumnNames, IdentifierType, " = ");

			return builder.ToSqlString();
		}

		protected OptimisticLockMode OptimisticLockMode
		{
			get { return entityMetamodel.OptimisticLockMode; }
		}

		public bool IsManyToMany
		{
			get { return false; }
		}

		public object CreateProxy(object id, ISessionImplementor session)
		{
			return proxyFactory.GetProxy(id, session);
		}

		/// <summary>
		/// Transform the array of property indexes to an array of booleans
		/// </summary>
		protected bool[] GetPropertiesToUpdate(int[] dirtyProperties, bool hasDirtyCollection)
		{
			bool[] propsToUpdate = new bool[HydrateSpan];
			bool[] updateability = PropertyUpdateability;
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
					Versioning.IsVersionIncrementRequired(dirtyProperties, hasDirtyCollection, PropertyVersionability);
			}

			return propsToUpdate;
		}

		public override string ToString()
		{
			return GetType().Name + '(' + ClassName + ')';
		}

		public System.Type[] PropertySubclassNames
		{
			get { return propertySubclassNames; }
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

		protected string[] GetPropertyColumnFormulaTemplates(int i)
		{
			return propertyColumnFormulaTemplates[i];
		}

		public string SelectFragment(string alias, string suffix, bool includeCollectionColumns)
		{
			return SelectFragment(alias, suffix);
		}

		public string SelectFragment(
			IJoinable rhs,
			string rhsAlias,
			string lhsAlias,
			string entitySuffix,
			string collectionSuffix,
			bool includeCollectionColumns)
		{
			return SelectFragment(lhsAlias, entitySuffix);
		}

		protected ADOException Convert(Exception sqlException, string message)
		{
			return ADOExceptionHelper.Convert( /* sqlExceptionConverter, */ sqlException, message);
		}

		protected ADOException Convert(Exception sqlException, string message, SqlString sql)
		{
			return ADOExceptionHelper.Convert( /* sqlExceptionConverter, */ sqlException, message, sql);
		}

		public abstract SqlString QueryWhereFragment(string alias, bool innerJoin, bool includeSubclasses);

		public abstract string DiscriminatorSQLValue { get; }

		public abstract object DiscriminatorValue { get; }

		public abstract object[] PropertySpaces { get; }

		/// <summary>
		/// Decide which tables need to be updated
		/// </summary>
		private bool[] GetTableUpdateNeeded(int[] dirtyProperties, bool hasDirtyCollection)
		{
			if (dirtyProperties == null)
			{
				return TableHasColumns; //for object that came in via update()
			}
			else
			{
				bool[] updateability = PropertyUpdateability;
				bool[] tableUpdateNeeded = new bool[TableSpan];
				int[] propertyTableNumbers = PropertyTableNumbers;
				for (int i = 0; i < dirtyProperties.Length; i++)
				{
					int property = dirtyProperties[i];
					int table = propertyTableNumbers[property];
					tableUpdateNeeded[table] = tableUpdateNeeded[table] ||
					                           (GetPropertyColumnSpan(property) > 0 && updateability[property]);
				}
				if (IsVersioned)
				{
					tableUpdateNeeded[0] = tableUpdateNeeded[0] ||
					                       Versioning.IsVersionIncrementRequired(dirtyProperties, hasDirtyCollection,
					                                                             PropertyVersionability);
				}
				return tableUpdateNeeded;
			}
		}

		public void Update(
			object id,
			object[] fields,
			int[] dirtyFields,
			bool hasDirtyCollection,
			object[] oldFields,
			object oldVersion,
			object obj,
			ISessionImplementor session)
		{
			bool[] tableUpdateNeeded = GetTableUpdateNeeded(dirtyFields, hasDirtyCollection);
			int span = TableSpan;
			bool[] propsToUpdate;
			SqlCommandInfo[] updateStrings;

			if (entityMetamodel.IsDynamicUpdate && dirtyFields != null)
			{
				// For the case of dynamic-update="true", we need to generate the UPDATE SQL
				propsToUpdate = GetPropertiesToUpdate(dirtyFields, hasDirtyCollection);
				updateStrings = new SqlCommandInfo[span];
				for (int j = 0; j < span; j++)
				{
					updateStrings[j] = tableUpdateNeeded[j]
					                   	? GenerateUpdateString(propsToUpdate, j, oldFields)
					                   	: null;
				}
			}
			else
			{
				// For the case of dynamic-update="false", or no snapshot, we use the static SQL
				updateStrings = SqlUpdateStrings;
				propsToUpdate = PropertyUpdateability;
			}

			for (int j = 0; j < span; j++)
			{
				// Now update only the tables with dirty properties (and the table with the version number)
				if (tableUpdateNeeded[j])
				{
					Update(id, fields, oldFields, propsToUpdate, j, oldVersion, obj, updateStrings[j], session);
				}
			}
		}

		protected void Update(
			object id,
			object[] fields,
			object[] oldFields,
			bool[] includeProperty,
			int j,
			object oldVersion,
			object obj,
			SqlCommandInfo sql,
			ISessionImplementor session)
		{
			if (IsInverseTable(j))
			{
				return;
			}

			bool useVersion = j == 0 && IsVersioned;
			IExpectation expectation = Expectations.AppropriateExpectation(updateResultCheckStyles[j]);
			bool useBatch = j == 0 && expectation.CanBeBatched && IsBatchable;
				//note: updates to joined tables can't be batched...

			if (log.IsDebugEnabled)
			{
				log.Debug("Updating entity: " + MessageHelper.InfoString(this, id));
				if (useVersion)
				{
					log.Debug("Existing version: " + oldVersion + " -> New Version: " + fields[VersionProperty]);
				}
			}

			try
			{
				IDbCommand statement = useBatch
				                       	? session.Batcher.PrepareBatchCommand(sql.CommandType, sql.Text, sql.ParameterTypes)
				                       	: session.Batcher.PrepareCommand(sql.CommandType, sql.Text, sql.ParameterTypes);
				try
				{
					int index = 0;

					//index += expectation.Prepare(statement, factory.ConnectionProvider.Driver);
					index = Dehydrate(id, fields, includeProperty, propertyColumnUpdateable, j, statement, session, index);

					// Write any appropriate versioning conditional parameters
					if (useVersion && OptimisticLockMode == OptimisticLockMode.Version)
					{
						VersionType.NullSafeSet(statement, oldVersion, index, session);
					}
					else if (OptimisticLockMode.Version < OptimisticLockMode && null != oldFields)
					{
						bool[] versionability = PropertyVersionability;
						bool[] includeOldField = OptimisticLockMode == OptimisticLockMode.All
						                         	? PropertyUpdateability
						                         	: includeProperty;

						for (int i = 0; i < entityMetamodel.PropertySpan; i++)
						{
							bool include = includeOldField[i] &&
							               IsPropertyOfTable(i, j) &&
							               versionability[i];
							if (include)
							{
								if (!PropertyTypes[i].IsDatabaseNull(oldFields[i]))
								{
									PropertyTypes[i].NullSafeSet(statement, oldFields[i], index, session);
									index += GetPropertyColumnSpan(i);
								}
							}
						}
					}

					if (useBatch)
					{
						session.Batcher.AddToBatch(expectation);
					}
					else
					{
						Check(session.Batcher.ExecuteNonQuery(statement), id, j, expectation, statement);
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
			catch (HibernateException)
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch (Exception sqle)
			{
				throw Convert(sqle, "could not update: " + MessageHelper.InfoString(this, id), sql.Text);
			}
		}


		public abstract IType DiscriminatorType { get; }

		public virtual SqlString FromJoinFragment(string alias, bool innerJoin, bool includeSubclasses)
		{
			return TableSpan == 1
				? new SqlString(string.Empty) // just a performance opt!
				: CreateJoin(alias, innerJoin, includeSubclasses).ToFromFragmentString;
		}

		public virtual string FromTableFragment(string alias)
		{
			return TableName + ' ' + alias;
		}

		public abstract System.Type GetSubclassForDiscriminatorValue(object value);

		public bool IsDefinedOnSubclass(int i)
		{
			return propertyDefinedOnSubclass[i];
		}

		[Obsolete("Added parameter allProperties to follow H3's API")]
		public string PropertySelectFragment(string name, string suffix)
		{
			return PropertySelectFragment(name, suffix, true);
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
				bool selectable = (allProperties) &&
					!IsSubclassTableSequentialSelect(columnTableNumbers[i]) &&
					subclassColumnSelectableClosure[i];
				if (selectable)
				{
					string subalias = Alias(name, columnTableNumbers[i]);
					select.AddColumn(subalias, columns[i], columnAliases[i]);
				}
			}

			int[] formulaTableNumbers = SubclassFormulaTableNumberClosure;
			string[] formulaTemplates = SubclassFormulaTemplateClosure;
			string[] formulaAliases = SubclassFormulaAliasClosure;

			for (int i = 0; i < formulaTemplates.Length; i++)
			{
				bool selectable = (allProperties) &&
					!IsSubclassTableSequentialSelect(formulaTableNumbers[i]);
				if (selectable)
				{
					string subalias = Alias(name, formulaTableNumbers[i]);
					select.AddFormula(subalias, formulaTemplates[i], formulaAliases[i]);
				}
			}

			if (HasSubclasses)
			{
				AddDiscriminatorToSelect(select, name, suffix);
			}

			return select.ToSqlStringFragment();
		}

		protected abstract int[] SubclassColumnTableNumberClosure { get; }
		protected abstract int[] SubclassFormulaTableNumberClosure { get; }

		protected string[] SubclassColumnClosure
		{
			get { return subclassColumnClosure; }
		}

		protected string[] SubclassColumnAliasClosure
		{
			get { return subclassColumnAliasClosure; }
		}

		protected string[] SubclassFormulaTemplateClosure
		{
			get { return subclassFormulaTemplateClosure; }
		}

		protected string[] SubclassFormulaAliasClosure
		{
			get { return subclassFormulaAliasClosure; }
		}

		protected string[] SubclassFormulaClosure
		{
			get { return subclassFormulaClosure; }
		}

		protected string[] SubclassPropertyNameClosure
		{
			get { return subclassPropertyNameClosure; }
		}

		protected System.Type[] SubclassPropertySubclassNameClosure
		{
			get { return this.subclassPropertySubclassNameClosure; }
		}

		protected IType[] SubclassPropertyTypeClosure
		{
			get { return subclassPropertyTypeClosure; }
		}

		protected string[][] SubclassPropertyColumnNameClosure
		{
			get { return subclassPropertyColumnNameClosure; }
		}

		protected string[][] SubclassPropertyFormulaTemplateClosure
		{
			get { return subclassPropertyFormulaTemplateClosure; }
		}

		protected int Dehydrate(
			object id,
			object[] fields,
			bool[] includeProperty,
			bool[][] includeColumns,
			int table,
			IDbCommand statement,
			ISessionImplementor session,
			int index)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("Dehydrating entity: " + MessageHelper.InfoString(this, id));
			}

			// there's a pretty strong coupling between the order of the SQL parameter 
			// construction and the actual order of the parameter collection. 

			for (int j = 0; j < entityMetamodel.PropertySpan; j++)
			{
				if (includeProperty[j] && IsPropertyOfTable(j, table))
				{
					PropertyTypes[j].NullSafeSet(statement, fields[j], index, includeColumns[j], session);
					index += ArrayHelper.CountTrue(includeColumns[j]);
				}
			}

			if (id != null)
			{
				IdentifierType.NullSafeSet(statement, id, index, session);
				index += IdentifierColumnSpan;
			}

			return index;
		}

		/// <summary>
		/// Unmarshall the fields of a persistent instance from a result set,
		/// without resolving associations or collections
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="id"></param>
		/// <param name="obj"></param>
		/// <param name="rootLoadable"></param>
		/// <param name="suffixedPropertyColumns"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public object[] Hydrate(
			IDataReader rs,
			object id,
			object obj,
			ILoadable rootLoadable,
			string[][] suffixedPropertyColumns,
			ISessionImplementor session)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("Hydrating entity: " + rootLoadable.ClassName + '#' + id);
			}

			AbstractEntityPersister rootPersister = (AbstractEntityPersister)rootLoadable;

			bool hasDeferred = rootPersister.HasSequentialSelect;
			IDbCommand sequentialSelect = null;
			IDataReader sequentialResultSet = null;
			bool sequentialSelectEmpty = false;
			try
			{
				if (hasDeferred)
				{
					SqlString sql = rootPersister.GetSequentialSelect(MappedClass);
					if (sql != null)
					{
						//TODO: I am not so sure about the exception handling in this bit!
						sequentialSelect = session.Batcher.PrepareCommand(CommandType.Text, sql, idSqlTypes);
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
				// TODO: H3 - Property Laziness
				// bool[] laziness = PropertyLaziness();
				System.Type[] propSubclassNames = SubclassPropertySubclassNameClosure;

				for (int i = 0; i < types.Length; i++)
				{
					bool propertyIsDeferred = hasDeferred
						&& rootPersister.IsSubclassPropertyDeferred(propNames[i], propSubclassNames[i]);

					IDataReader propertyResultSet = propertyIsDeferred ? sequentialResultSet : rs;
					string[] cols = propertyIsDeferred ? propertyColumnAliases[i] : suffixedPropertyColumns[i];
					values[i] = types[i].Hydrate(propertyResultSet, cols, session, obj);
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

		protected virtual SqlString GetSequentialSelect(System.Type entityName)
		{
			throw new NotSupportedException("no sequential selects");
		}

		/// <summary>
		/// Persist an object, using a natively generated identifier
		/// </summary>
		protected object Insert(object[] fields, bool[] notNull, SqlCommandInfo sql, object obj, ISessionImplementor session)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("Inserting entity: " + ClassName + " (native id)");
				if (IsVersioned)
				{
					log.Debug("Version: " + Versioning.GetVersion(fields, this));
				}
			}

			try
			{
				if (Dialect.SupportsInsertSelectIdentity && sql.CommandType == CommandType.Text)
				{
					SqlString insertSelectSQL = Dialect.AppendIdentitySelectToInsert(sql.Text);
					// Use one statement to insert the row and get the generated id
					IDbCommand insertSelect = session.Batcher.PrepareCommand(CommandType.Text, insertSelectSQL, sql.ParameterTypes);
					IDataReader rs = null;
					try
					{
						// Well, it's always the first table to dehydrate, so pass 0 as the position
						Dehydrate(null, fields, notNull, propertyColumnInsertable, 0, insertSelect, session, 0);
						rs = session.Batcher.ExecuteReader(insertSelect);
						return GetGeneratedIdentity(obj, session, rs);
					}
					finally
					{
						session.Batcher.CloseCommand(insertSelect, rs);
					}
				}
				else
				{
					// Do the insert
					IDbCommand statement = session.Batcher.PrepareCommand(sql.CommandType, sql.Text, sql.ParameterTypes);
					try
					{
						// Well, it's always the first table to dehydrate, so pass 0 as the position
						Dehydrate(null, fields, notNull, propertyColumnInsertable, 0, statement, session, 0);
						session.Batcher.ExecuteNonQuery(statement);

						// Fetch the generated id in a separate query. This is done inside the first try/finally block
						// to keep the insert command open, so that the batcher does not close the connection.
						//
						// It's possible that some ADO.NET provider will not allow two open IDbCommands for the same connection,
						// in that case we'll have to rewrite the code to use some sort of lock on IBatcher.
						SqlString idselectSql = new SqlString(SqlIdentitySelect(IdentifierColumnNames[0], GetTableName(0)));
						IDbCommand idselect = session.Batcher.PrepareCommand(CommandType.Text, idselectSql, SqlTypeFactory.NoTypes);
						IDataReader rs = null;

						try
						{
							rs = session.Batcher.ExecuteReader(idselect);
							return GetGeneratedIdentity(obj, session, rs);
						}
						finally
						{
							session.Batcher.CloseCommand(idselect, rs);
						}
					}
					finally
					{
						session.Batcher.CloseCommand(statement, null);
					}
				}
			}
			catch (HibernateException)
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch (Exception sqle)
			{
				throw Convert(sqle, "could not insert: " + MessageHelper.InfoString(this), sql.Text);
			}
		}

		public abstract string TableName { get; }

		public string[] ToColumns(string name, int i)
		{
			string alias = Alias(name, GetSubclassPropertyTableNumber(i));
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

		/// <remarks>
		/// Warning:
		/// When there are duplicated property names in the subclasses
		/// of the class, this method may return the wrong table
		/// number for the duplicated subclass property (note that
		/// SingleTableEntityPersister defines an overloaded form
		/// which takes the entity name.
		/// </remarks>
		public int GetSubclassPropertyTableNumber(string propertyPath)
		{
			string rootPropertyName = StringHelper.Root(propertyPath);
			IType type = propertyMapping.ToType(rootPropertyName);
			if (type.IsAssociationType && ((IAssociationType) type).UseLHSPrimaryKey)
			{
				return 0;
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

		// TODO: override
		public virtual string[] ToColumns(string alias, string propertyName)
		{
			return propertyMapping.ToColumns(alias, propertyName);
		}

		public IType ToType(string propertyName)
		{
			return propertyMapping.ToType(propertyName);
		}

		public abstract SqlString WhereJoinFragment(string alias, bool innerJoin, bool includeSubclasses);

		public abstract string DiscriminatorColumnName { get; }

		public abstract string GetSubclassPropertyTableName(int i);

		public abstract bool IsCacheInvalidationRequired { get; }

		protected abstract int GetSubclassPropertyTableNumber(int i);

		public bool IsLazyPropertiesCacheable
		{
			get { return isLazyPropertiesCacheable; }
		}
	

		public bool IsUnsavedVersion(object[] values)
		{
			if (!IsVersioned)
			{
				return false;
			}

			object result = entityMetamodel.VersionProperty.UnsavedValue.IsUnsaved(values[VersionProperty]);
			if (result != null)
			{
				return (bool) result;
			}

			return true;
		}

		protected bool HasFormulaProperties
		{
			get { return hasFormulaProperties; }
		}

		protected string[] GetPropertyColumnAliases(int i)
		{
			return propertyColumnAliases[i];
		}

		public FetchMode GetFetchMode(int i)
		{
			return subclassPropertyFetchModeClosure[i];
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

		public ISessionFactoryImplementor Factory
		{
			get { return factory; }
		}

		protected string Alias(string rootAlias, int tableNumber)
		{
			if (tableNumber == 0)
			{
				return rootAlias;
			}

			StringBuilder buf = new StringBuilder(rootAlias);

			if (!rootAlias.EndsWith("_"))
			{
				buf.Append('_');
			}

			return buf.Append(tableNumber).Append('_').ToString();

			// TODO: this was the former NH implementation, I wonder
			// if quoting/unquoting stuff matters at all.
			//return Dialect.QuoteForAliasName(
			//	Dialect.UnQuote( rootAlias )
			//	+ StringHelper.Underscore
			//	+ tableNumber
			//	+ StringHelper.Underscore );
		}

		protected virtual void AddDiscriminatorToInsert(SqlInsertBuilder insert) { }

		protected virtual void AddDiscriminatorToSelect(SelectFragment select, string name, string suffix) { }

		public string[] GetPropertyColumnNames(string propertyName)
		{
			return propertyMapping.GetColumnNames(propertyName);
		}

		public int GetPropertyIndex(string propertyName)
		{
			return entityMetamodel.GetPropertyIndex(propertyName);
		}

		public abstract string GetPropertyTableName(string propertyName);

		protected EntityMetamodel EntityMetamodel
		{
			get { return entityMetamodel; }
		}

		private JoinFragment CreateJoin(string name, bool innerjoin, bool includeSubclasses)
		{
			string[] idCols = StringHelper.Qualify(name, IdentifierColumnNames); //all joins join to the pk of the driving table
			JoinFragment join = Factory.Dialect.CreateOuterJoinFragment();
			int tableSpan = SubclassTableSpan;
			for (int j = 1; j < tableSpan; j++) //notice that we skip the first table; it is the driving table!
			{
				bool joinIsIncluded = IsClassOrSuperclassTable(j) ||
					(includeSubclasses && !IsSubclassTableSequentialSelect(j));
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
			string[] keyCols = StringHelper.Qualify( drivingAlias, GetSubclassTableKeyColumns( tableNumbers[0] ) );
			JoinFragment jf = Factory.Dialect.CreateOuterJoinFragment();
			for ( int i = 1; i < tableNumbers.Length; i++ ) { //skip the driving table
				int j = tableNumbers[i];
				jf.AddJoin( GetSubclassTableName( j ),
						GenerateTableAlias( RootAlias, j ),
						keyCols,
						GetSubclassTableKeyColumns( j ),
						IsInverseSubclassTable( j ) || IsNullableSubclassTable( j ) 
							? JoinType.LeftOuterJoin
							: JoinType.InnerJoin );
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
					String subalias = GenerateTableAlias(RootAlias, columnTableNumbers[columnNumber]);
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
			SqlStringBuilder builder = new SqlStringBuilder();
			string[] subclauses = StringHelper.Qualify(alias, GetSubclassTableKeyColumns(tableNumber));
			for (int i = 0; i < subclauses.Length; i++ )
			{
				string subclause = subclauses[i];
				if (i > 0)
				{
					builder.Add(" and ");
				}
				builder.Add(subclause + "=").AddParameter();
			}
			return builder.ToSqlString();
		}

		protected SqlString RenderSelect(			
			int[] tableNumbers,
	        int[] columnNumbers,
	        int[] formulaNumbers) 
		{
			Array.Sort(tableNumbers); //get 'em in the right order (not that it really matters)

			//render the where and from parts
			int drivingTable = tableNumbers[0];
			string drivingAlias = GenerateTableAlias( RootAlias, drivingTable ); //we *could* regerate this inside each called method!
			SqlString where = CreateWhereByKey( drivingTable, drivingAlias );
			string from = CreateFrom( drivingTable, drivingAlias );

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
			// TODO: H3
			//if (Factory.Settings.IsCommentsEnabled)
			//{
			//    select.SetComment("sequential select " + MappedClass.Name);
			//}
			return select.ToSqlString();
		}

		private string RootAlias
		{
			get { return StringHelper.GenerateAlias(ClassName); }
		}

		protected void PostConstruct(IMapping mapping)
		{
			InitPropertyPaths(mapping);
			idAndVersionSqlTypes = IsVersioned
			                       	? ArrayHelper.Join(IdentifierType.SqlTypes(mapping), VersionType.SqlTypes(mapping))
			                       	: IdentifierType.SqlTypes(mapping);

			idSqlTypes = IdentifierType.SqlTypes(mapping);
		}

		protected abstract int[] PropertyTableNumbersInSelect { get; }

		protected string ConcretePropertySelectFragment(string alias, bool[] includeProperty)
		{
			int propertyCount = entityMetamodel.PropertySpan;
			int[] propertyTableNumbers = PropertyTableNumbersInSelect;
			SelectFragment frag = new SelectFragment(Factory.Dialect);
			for (int i = 0; i < propertyCount; i++)
			{
				if (includeProperty[i])
				{
					//ie. updateable, not a formula
					frag.AddColumns(
						GenerateTableAlias(alias, propertyTableNumbers[i]),
						propertyColumnNames[i],
						propertyColumnAliases[i]
						);
					frag.AddFormulas(
						GenerateTableAlias(alias, propertyTableNumbers[i]),
						propertyColumnFormulaTemplates[i],
						propertyColumnAliases[i]
						);
				}
			}
			return frag.ToSqlStringFragment();
		}

		// TODO NH: should remove duplication between this and the other overload,
		// probably using H3 approach (adding indirection through IInclusionChecker interface).
		protected string ConcretePropertySelectFragment(string alias, ValueInclusion[] inclusions)
		{
			int propertyCount = entityMetamodel.PropertySpan;
			int[] propertyTableNumbers = PropertyTableNumbersInSelect;
			SelectFragment frag = new SelectFragment(Factory.Dialect);
			for (int i = 0; i < propertyCount; i++)
			{
				if (inclusions[i] != ValueInclusion.None)
				{
					//ie. updateable, not a formula
					frag.AddColumns(
						GenerateTableAlias(alias, propertyTableNumbers[i]),
						propertyColumnNames[i],
						propertyColumnAliases[i]
						);
					frag.AddFormulas(
						GenerateTableAlias(alias, propertyTableNumbers[i]),
						propertyColumnFormulaTemplates[i],
						propertyColumnAliases[i]
						);
				}
			}
			return frag.ToSqlStringFragment();
		}

		protected virtual SqlString GenerateSnapshotSelectString()
		{
			//TODO: should we use SELECT .. FOR UPDATE?

			SqlSelectBuilder select = new SqlSelectBuilder(Factory);

			//if (Factory.Settings.IsCommentsEnabled)
			//{
			//    select.SetComment("get current state " + ClassName);
			//}

			string[] aliasedIdColumns = StringHelper.Qualify(RootAlias, IdentifierColumnNames);
			string selectClause = StringHelper.Join(", ", aliasedIdColumns) +
			                      ConcretePropertySelectFragment(RootAlias, PropertyUpdateability);

			SqlString fromClause = new SqlString(FromTableFragment(RootAlias)) +
			                       FromJoinFragment(RootAlias, true, false);

			SqlString joiner = new SqlString("=", Parameter.Placeholder, " and ");
			SqlStringBuilder whereClauseBuilder = new SqlStringBuilder()
				.Add(StringHelper.Join(joiner, aliasedIdColumns))
				.Add("=")
				.AddParameter()
				.Add(WhereJoinFragment(RootAlias, true, false));

			// DONE H3: this is commented out in H3.2
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
				.SetWhereClause(whereClauseBuilder.ToSqlString())
				.ToSqlString();
		}

		public virtual void PostInstantiate()
		{
			int tableSpan = TableSpan;

			sqlInsertStrings = new SqlCommandInfo[tableSpan];
			sqlDeleteStrings = new SqlCommandInfo[tableSpan];
			sqlUpdateStrings = new SqlCommandInfo[tableSpan];

			for (int j = 0; j < tableSpan; j++)
			{
				SqlCommandInfo defaultInsert = GenerateInsertString(false, PropertyInsertability, j);
				sqlInsertStrings[j] = customSQLInsert[j] == null
				                      	? defaultInsert
				                      	: new SqlCommandInfo(customSQLInsert[j], defaultInsert.ParameterTypes);
				SqlCommandInfo defaultUpdate = GenerateUpdateString(PropertyUpdateability, j, null);
				sqlUpdateStrings[j] = customSQLUpdate[j] == null
				                      	? defaultUpdate
				                      	: new SqlCommandInfo(customSQLUpdate[j], defaultUpdate.ParameterTypes);
				SqlCommandInfo defaultDelete = GenerateDeleteString(j);
				sqlDeleteStrings[j] = customSQLDelete[j] == null
				                      	? defaultDelete
				                      	: new SqlCommandInfo(customSQLDelete[j], defaultDelete.ParameterTypes);
			}

			sqlIdentityInsertString = IsIdentifierAssignedByInsert
			                          	? GenerateIdentityInsertString(PropertyInsertability)
			                          	: null;

			sqlSnapshotSelectString = GenerateSnapshotSelectString();
			sqlVersionSelectString = GenerateSelectVersionString();
			if (HasInsertGeneratedProperties)
			{
				sqlInsertGeneratedValuesSelectString = GenerateInsertGeneratedValuesSelectString();
			}
			if (HasUpdateGeneratedProperties)
			{
				sqlUpdateGeneratedValuesSelectString = GenerateUpdateGeneratedValuesSelectString();
			}

			// This is used to determine updates for objects that came in via update()
			tableHasColumns = new bool[sqlUpdateStrings.Length];
			for (int j = 0; j < sqlUpdateStrings.Length; j++)
			{
				tableHasColumns[j] = sqlUpdateStrings[j] != null;
			}

			CreateLoaders();
			CreateUniqueKeyLoaders();
			CreateQueryLoader();
			InitLockers();
		}

		public Cascades.CascadeStyle GetCascadeStyle(int i)
		{
			return subclassPropertyCascadeStyleClosure[i];
		}

		private bool IsAllNull(object[] array, int tableNumber)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (IsPropertyOfTable(i, tableNumber) && array[i] != null)
				{
					return false;
				}
			}
			return true;
		}

		public bool IsSubclassPropertyNullable(int i)
		{
			return subclassPropertyNullabilityClosure[i];
		}

		public EntityType EntityType
		{
			get { return entityMetamodel.EntityType; }
		}

		public abstract string FilterFragment(string alias);

		public string FilterFragment(string alias, IDictionary<string, IFilter> enabledFilters)
		{
			StringBuilder sessionFilterFragment = new StringBuilder();
			filterHelper.Render(sessionFilterFragment, GenerateFilterConditionAlias(alias), enabledFilters);

			return sessionFilterFragment.Append(FilterFragment(alias)).ToString();
		}

		public string GenerateTableAlias(string rootAlias, int tableNumber)
		{
			if (tableNumber == 0)
			{
				return rootAlias;
			}

			StringBuilder buf = new StringBuilder().Append(rootAlias);
			if (!rootAlias.EndsWith("_"))
			{
				buf.Append('_');
			}

			return buf.Append(tableNumber).Append('_').ToString();
		}

		public virtual string GenerateFilterConditionAlias(string rootAlias)
		{
			return rootAlias;
		}

		/// <summary>
		/// Load an instance using the appropriate loader (as determined by <see cref="GetAppropriateLoader" />
		/// </summary>
		public object Load(object id, object optionalObject, LockMode lockMode, ISessionImplementor session)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug(
					"Fetching entity: " +
					MessageHelper.InfoString(this, id, Factory)
					);
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
				//				if( session.FetchProfile != null && LockMode.Upgrade.GreaterThan( lockMode ) )
				//				{
				//					return ( IUniqueEntityLoader ) loaders[ session.FetchProfile ];
				//				}
				//				else
				//				{
				return (IUniqueEntityLoader) loaders[lockMode];
				//				}
			}
			else
			{
				return CreateEntityLoader(lockMode, enabledFilters);
			}
		}

		private void CreateLoaders()
		{
			loaders.Add(LockMode.None, CreateEntityLoader(LockMode.None));
			loaders.Add(LockMode.Read, CreateEntityLoader(LockMode.Read));
			loaders.Add(LockMode.Upgrade, CreateEntityLoader(LockMode.Upgrade));
			loaders.Add(LockMode.UpgradeNoWait, CreateEntityLoader(LockMode.UpgradeNoWait));
		}

		public virtual object[] QuerySpaces
		{
			get { return PropertySpaces; }
		}

		public virtual string OneToManyFilterFragment(string alias)
		{
			return string.Empty;
		}

		protected abstract bool IsClassOrSuperclassTable(int j);

		protected abstract int SubclassTableSpan { get; }

		protected abstract int TableSpan { get; }

		protected SqlCommandInfo GenerateInsertString(bool[] includeProperty, int j)
		{
			return GenerateInsertString(false, includeProperty, j);
		}

		protected SqlCommandInfo GenerateInsertString(bool identityInsert, bool[] includeProperty)
		{
			return GenerateInsertString(identityInsert, includeProperty, 0);
		}

		protected virtual SqlCommandInfo GenerateIdentityInsertString(bool[] includeProperty)
		{
			return GenerateInsertString(true, includeProperty);
		}

		protected virtual SqlCommandInfo GenerateInsertString(bool identityInsert, bool[] includeProperty, int j)
		{
			SqlInsertBuilder builder = new SqlInsertBuilder(Factory)
				.SetTableName(GetTableName(j));

			// add normal properties
			for (int i = 0; i < entityMetamodel.PropertySpan; i++)
			{
				if (includeProperty[i] && IsPropertyOfTable(i, j))
				{
					builder.AddColumns(GetPropertyColumnNames(i), PropertyColumnInsertable[i], PropertyTypes[i]);
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
				// make sure the Dialect has an identity insert string because we don't want
				// to add the column when there is no value to supply the SqlBuilder
				if (Dialect.IdentityInsertString != null)
				{
					// only 1 column if there is IdentityInsert enabled.
					builder.AddColumn(IdentifierColumnNames[0], Dialect.IdentityInsertString);
				}
			}
			else
			{
				builder.AddColumns(GetKeyColumns(j), null, IdentifierType);
			}

			return builder.ToSqlCommandInfo();
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
				id = Insert(fields, PropertyInsertability, SqlIdentityInsertString, obj, session);
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
				for (int j = 0; j < span; j++)
				{
					Insert(id, fields, PropertyInsertability, j, SqlInsertStrings[j], obj, session);
				}
			}
		}

		//Access cached SQL

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
		/// The query that insert a row into the root table, letting the database generate an id
		/// </summary>
		protected SqlCommandInfo SqlIdentityInsertString
		{
			get { return sqlIdentityInsertString; }
		}

		/// <summary>
		/// The queries that update rows by id (and version)
		/// </summary>
		protected SqlCommandInfo[] SqlUpdateStrings
		{
			get { return sqlUpdateStrings; }
		}

		protected virtual bool IsSubclassPropertyDeferred(string propertyName, System.Type entityName)
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

		protected abstract string GetTableName(int table);
		protected abstract string[] GetKeyColumns(int table);
		protected abstract bool IsPropertyOfTable(int property, int table);

		protected virtual SqlCommandInfo GenerateUpdateString(bool[] includeProperty, int j, object[] oldFields)
		{
			SqlUpdateBuilder updateBuilder = new SqlUpdateBuilder(Factory.Dialect, Factory)
				.SetTableName(GetTableName(j))
				.SetIdentityColumn(GetKeyColumns(j), IdentifierType);

			bool hasColumns = false;
			for (int i = 0; i < entityMetamodel.PropertySpan; i++)
			{
				if (includeProperty[i] && IsPropertyOfTable(i, j))
				{
					updateBuilder.AddColumns(GetPropertyColumnNames(i), propertyColumnUpdateable[i], PropertyTypes[i]);
					hasColumns = hasColumns || GetPropertyColumnSpan(i) > 0;
				}
			}

			if (j == 0 && IsVersioned && entityMetamodel.OptimisticLockMode == OptimisticLockMode.Version)
			{
				updateBuilder.SetVersionColumn(new string[] {VersionColumnName}, VersionType);
				hasColumns = true;
			}
			else if (entityMetamodel.OptimisticLockMode > OptimisticLockMode.Version && oldFields != null)
			{
				bool[] versionability = PropertyVersionability;
				bool[] includeInWhere =
					OptimisticLockMode == OptimisticLockMode.All
						? PropertyUpdateability
						: includeProperty;

				for (int i = 0; i < entityMetamodel.PropertySpan; i++)
				{
					bool include = includeInWhere[i] &&
					               IsPropertyOfTable(i, j) &&
					               versionability[i];
					if (include)
					{
						string[] propertyColumnNames = GetPropertyColumnNames(i);
						if (PropertyTypes[i].IsDatabaseNull(oldFields[i]))
						{
							foreach (string column in propertyColumnNames)
							{
								updateBuilder.AddWhereFragment(column + " is null");
							}
						}
						else
						{
							updateBuilder.AddWhereFragment(propertyColumnNames, PropertyTypes[i], "=");
						}
					}
				}
			}

			return hasColumns ? updateBuilder.ToSqlCommandInfo() : null;
		}

		public void Delete(object id, object version, object obj, ISessionImplementor session)
		{
			int span = TableSpan;
			bool isImpliedOptimisticLocking = !entityMetamodel.IsVersioned &&
			                                  entityMetamodel.OptimisticLockMode > OptimisticLockMode.Version;
			object[] loadedState = null;
			if (isImpliedOptimisticLocking)
			{
				// need to treat this as if it where optimistic-lock="all" (dirty does *not* make sense);
				// first we need to locate the "loaded" state
				//
				// Note, it potentially could be a proxy, so perform the location the safe way...
				EntityKey key = new EntityKey(id, this);
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
				deleteStrings = GenerateDeleteStrings(loadedState);
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

		/// <summary>
		/// Delete an object.
		/// </summary>
		public void Delete(object id, object version, int j, object obj, SqlCommandInfo sql, ISessionImplementor session,
		                   object[] loadedState)
		{
			if (IsInverseTable(j))
			{
				return;
			}

			bool useVersion = j == 0 && IsVersioned;
			IExpectation expectation = Expectations.AppropriateExpectation(deleteResultCheckStyles[j]);
			bool useBatch = j == 0 && expectation.CanBeBatched && IsBatchable;

			if (log.IsDebugEnabled)
			{
				log.Debug("Deleting entity: " + MessageHelper.InfoString(this, id));
				if (useVersion)
				{
					log.Debug("Version: " + version);
				}
			}

			try
			{
				IDbCommand statement;
				if (useBatch)
				{
					statement = session.Batcher.PrepareBatchCommand(sql.CommandType, sql.Text, sql.ParameterTypes);
				}
				else
				{
					statement = session.Batcher.PrepareCommand(sql.CommandType, sql.Text, sql.ParameterTypes);
				}

				try
				{
					int index = 0;

					//index += expectation.Prepare(statement, factory.ConnectionProvider.Driver);

					// Do the key. The key is immutable so we can use the _current_ object state
					IdentifierType.NullSafeSet(statement, id, index, session);
					index += IdentifierColumnSpan;

					if (useVersion)
					{
						VersionType.NullSafeSet(statement, version, index, session);
					}
					else if (entityMetamodel.OptimisticLockMode > OptimisticLockMode.Version && loadedState != null)
					{
						IType[] types = PropertyTypes;
						bool[] versionability = PropertyVersionability;
						for (int i = 0; i < entityMetamodel.PropertySpan; i++)
						{
							if (IsPropertyOfTable(i, j) && versionability[i] && !types[i].IsDatabaseNull(loadedState[i]))
							{
								types[i].NullSafeSet(statement, loadedState[i], index, session);
								index += GetPropertyColumnSpan(i);
							}
						}
					}

					if (useBatch)
					{
						session.Batcher.AddToBatch(expectation);
					}
					else
					{
						Check(session.Batcher.ExecuteNonQuery(statement), id, j, expectation, statement);
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
			catch (HibernateException)
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch (Exception sqle)
			{
				throw Convert(sqle, "could not delete: " + MessageHelper.InfoString(this, id), sql.Text);
			}
		}

		protected SqlCommandInfo[] GenerateDeleteStrings(object[] loadedState)
		{
			int span = TableSpan;
			SqlCommandInfo[] deleteStrings = new SqlCommandInfo[span];

			for (int j = span - 1; j >= 0; j--)
			{
				SqlDeleteBuilder delete = new SqlDeleteBuilder(Factory.Dialect, Factory)
					.SetTableName(GetTableName(j))
					.SetIdentityColumn(GetKeyColumns(j), IdentifierType);

				IType[] types = PropertyTypes;

				bool[] versionability = PropertyVersionability;
				//bool[] includeInWhere = PropertyUpdateability;

				for (int i = 0; i < entityMetamodel.PropertySpan; i++)
				{
					bool include = versionability[i] &&
					               IsPropertyOfTable(i, j);

					if (include)
					{
						// this property belongs to the table and it is not specifically
						// excluded from optimistic locking by optimistic-lock="false"
						string[] propertyColumnNames = GetPropertyColumnNames(i);
						if (types[i].IsDatabaseNull(loadedState[i]))
						{
							for (int k = 0; k < propertyColumnNames.Length; k++)
							{
								delete.AddWhereFragment(propertyColumnNames[k] + " is null");
							}
						}
						else
						{
							delete.AddWhereFragment(propertyColumnNames, PropertyTypes[i], " = ");
						}
					}
				}
				deleteStrings[j] = delete.ToSqlCommandInfo();
			}

			return deleteStrings;
		}

		protected int IdentifierColumnSpan
		{
			get { return identifierColumnSpan; }
		}

		/// <summary>
		/// Persist an object
		/// </summary>
		protected void Insert(
			object id,
			object[] fields,
			bool[] notNull,
			int j,
			SqlCommandInfo sql,
			object obj,
			ISessionImplementor session)
		{
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
				log.Debug("Inserting entity: " + MessageHelper.InfoString(this, id));
				if (IsVersioned)
				{
					log.Debug("Version: " + Versioning.GetVersion(fields, this));
				}
			}


			IExpectation expectation = Expectations.AppropriateExpectation(insertResultCheckStyles[j]);
			bool useBatch = j == 0 && expectation.CanBeBatched;

			try
			{
				// Render the SQL query
				IDbCommand insertCmd = useBatch
				                       	? session.Batcher.PrepareBatchCommand(sql.CommandType, sql.Text, sql.ParameterTypes)
				                       	: session.Batcher.PrepareCommand(sql.CommandType, sql.Text, sql.ParameterTypes);

				try
				{
					// Write the values of the field onto the prepared statement - we MUST use the
					// state at the time the insert was issued (cos of foreign key constraints)
					// not necessarily the obect's current state
					int index = 0;
					//index += expectation.Prepare(insertCmd, factory.ConnectionProvider.Driver);

					Dehydrate(id, fields, notNull, propertyColumnInsertable, j, insertCmd, session, index);

					if (useBatch)
					{
						session.Batcher.AddToBatch(expectation);
					}
					else
					{
						Check(session.Batcher.ExecuteNonQuery(insertCmd), id, j, expectation, insertCmd);
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
			catch (HibernateException)
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch (Exception sqle)
			{
				throw Convert(sqle, "could not insert: " + MessageHelper.InfoString(this, id), sql.Text);
			}
		}

		protected SqlString VersionSelectString
		{
			get { return sqlVersionSelectString; }
		}

		protected SqlString SQLSnapshotSelectString
		{
			get { return sqlSnapshotSelectString; }
		}

		protected SqlCommandInfo GenerateDeleteString(int j)
		{
			SqlDeleteBuilder deleteBuilder = new SqlDeleteBuilder(Factory.Dialect, Factory);

			deleteBuilder
				.SetTableName(GetTableName(j))
				.SetIdentityColumn(GetKeyColumns(j), IdentifierType);

			if (j == 0 && IsVersioned)
			{
				deleteBuilder.SetVersionColumn(new string[] {VersionColumnName}, VersionType);
			}

			return deleteBuilder.ToSqlCommandInfo();
		}

		protected bool[] TableHasColumns
		{
			get { return tableHasColumns; }
		}

		protected abstract int[] PropertyTableNumbers { get; }

		protected void CreateQueryLoader()
		{
			if (loaderName != null)
			{
				queryLoader = new NamedQueryLoader(loaderName, this);
			}
		}

		public bool HasSubselectLoadableCollections
		{
			get { return hasSubselectLoadableCollections; }
		}

		protected virtual string DiscriminatorFormulaTemplate
		{
			get { return null; }
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
			return mappedClass.IsInstanceOfType(entity);
		}

		public ValueInclusion[] PropertyInsertGenerationInclusions
		{
			get { return entityMetamodel.PropertyInsertGenerationInclusions; }
		}

		public ValueInclusion[] PropertyUpdateGenerationInclusions
		{
			get { return entityMetamodel.PropertyUpdateGenerationInclusions; }
		}

		public bool IsVersionPropertyGenerated
		{
			get { return IsVersioned && PropertyUpdateGenerationInclusions[VersionProperty] != ValueInclusion.None; }
		}

		public bool IsInstrumented(EntityMode entityMode)
		{
			IEntityTuplizer tuplizer = entityMetamodel.GetTuplizerOrNull(entityMode);
			return tuplizer != null && tuplizer.IsInstrumented;
		}

		public bool HasInsertGeneratedProperties
		{
			get { return entityMetamodel.HasInsertGeneratedValues; }
		}

		public bool HasUpdateGeneratedProperties
		{
			get { return entityMetamodel.HasUpdateGeneratedValues; }
		}

		public bool IsSelectBeforeUpdateRequired
		{
			get { return HasSelectBeforeUpdate; }
		}

		public void ProcessInsertGeneratedProperties(object id, object entity, object[] state, ISessionImplementor session)
		{
			if (!HasInsertGeneratedProperties)
			{
				throw new AssertionFailure("no insert-generated properties");
			}
			ProcessGeneratedProperties(id, entity, state, session, sqlInsertGeneratedValuesSelectString, PropertyInsertGenerationInclusions);
		}

		public void ProcessUpdateGeneratedProperties(object id, object entity, object[] state, ISessionImplementor session)
		{
			if (!HasUpdateGeneratedProperties)
			{
				throw new AssertionFailure("no update-generated properties");
			}
			ProcessGeneratedProperties(id, entity, state, session, sqlUpdateGeneratedValuesSelectString, PropertyUpdateGenerationInclusions);
		}

		private void ProcessGeneratedProperties(
				object id,
				object entity,
				object[] state,
				ISessionImplementor session,
				SqlString selectionSQL,
				ValueInclusion[] includeds)
		{
			session.Batcher.ExecuteBatch(); //force immediate execution of the insert

			try
			{
				IDbCommand cmd =
					session.Batcher.PrepareQueryCommand(CommandType.Text, selectionSQL, IdentifierType.SqlTypes(Factory));
				IDataReader rs = null;
				try
				{
					IdentifierType.NullSafeSet(cmd, id, 0, session);
					rs = session.Batcher.ExecuteReader(cmd);
					if (!rs.Read())
					{
						throw new HibernateException(
							"Unable to locate row for retrieval of generated properties: " +
							MessageHelper.InfoString(this, id, Factory)
							);
					}
					for (int i = 0; i < entityMetamodel.PropertySpan; i++)
					{
						if (includeds[i] != ValueInclusion.None)
						{
							object hydratedState = PropertyTypes[i].Hydrate(rs, GetPropertyAliases("", i), session, entity);
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
			catch (HibernateException)
			{
				// Do not call Convert on HibernateException
				throw;
			}
			catch (Exception sqle)
			{
				throw ADOExceptionHelper.Convert(sqle, "unable to select generated column values", selectionSQL);
			}
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

			//if (getFactory().getSettings().isCommentsEnabled())
			//{
			//    select.setComment("get generated state " + getEntityName());
			//}

			string[] aliasedIdColumns = StringHelper.Qualify(RootAlias, IdentifierColumnNames);

			// Here we render the select column list based on the properties defined as being generated.
			// For partial component generation, we currently just re-select the whole component
			// rather than trying to handle the individual generated portions.
			string selectClause = ConcretePropertySelectFragment(RootAlias, inclusions);
			selectClause = selectClause.Substring(2);

			string fromClause = FromTableFragment(RootAlias) +
			                    FromJoinFragment(RootAlias, true, false);

			SqlString whereClause = new SqlStringBuilder()
				.Add(StringHelper.Join(new SqlString("=", Parameter.Placeholder, " and "), aliasedIdColumns))
				.Add("=").AddParameter()
				.Add(WhereJoinFragment(RootAlias, true, false))
				.ToSqlString();

			return select.SetSelectClause(selectClause)
				.SetFromClause(fromClause)
				.SetOuterJoins(SqlString.Empty, SqlString.Empty)
				.SetWhereClause(whereClause)
				.ToSqlString();
		}

		public virtual bool IsMultiTable
		{
			get { return false; }
		}

		protected int PropertySpan
		{
			get { return EntityMetamodel.PropertySpan; }
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

		protected internal virtual ILockingStrategy GenerateLocker(LockMode lockMode)
		{
			return factory.Dialect.GetLockingStrategy(this, lockMode);
		}

		protected internal IEntityTuplizer GetTuplizer(ISessionImplementor session)
		{
			return GetTuplizer(session.EntityMode);
		}

		protected internal IEntityTuplizer GetTuplizer(EntityMode entityMode)
		{
			return entityMetamodel.GetTuplizer(entityMode);
		}

		public void AfterInitialize(object entity, bool lazyPropertiesAreUnfetched, ISessionImplementor session)
		{
			GetTuplizer(session).AfterInitialize(entity, lazyPropertiesAreUnfetched, session);
		}

		public System.Type GetConcreteProxyClass(EntityMode entityMode)
		{
			return GetTuplizer(entityMode).ConcreteProxyClass;
		}

		#region IOptimisticCacheSource Members

		bool IOptimisticCacheSource.IsVersioned
		{
			get { return IsVersioned; }
		}

		public IComparer VersionComparator
		{
			get { return IsVersioned ? VersionType.Comparator : null; }
		}

		#endregion

	}
}
