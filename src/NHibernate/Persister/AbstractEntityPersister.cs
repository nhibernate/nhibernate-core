using System;
using System.Collections;
using System.Reflection;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Util;
using NHibernate.Id;
using NHibernate.Mapping;
using NHibernate.Metadata;
using NHibernate.Proxy;
using NHibernate.Hql;
using NHibernate.Dialect;
using NHibernate.Sql;
using NHibernate.Type;
using NHibernate.Loader;

namespace NHibernate.Persister {
	/// <summary>
	/// Superclass for built-in mapping strategies. Implements functionalty common to both mapping
	/// strategies
	/// </summary>
	/// <remarks>
	/// May be considred an immutable view of the mapping object
	/// </remarks>
	public abstract class AbstractEntityPersister : IQueryable, IClassMetadata {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(AbstractEntityPersister));

		protected System.Type[] NoClasses = new System.Type[0];

		private System.Type mappedClass;
		protected Dialect.Dialect dialect;
		private ConstructorInfo constructor;

		private IIdentifierGenerator idgen;
		private bool polymorphic;
		private bool explicitPolymorphism;
		private bool inherited;
		private bool hasSubclasses;
		private bool versioned;
		private bool abstractClass;
		private bool implementsLifecycle;
		private bool implementsValidatable;
		private bool hasCollections;
		private bool hasCascades;
		private bool mutable;
		private bool useIdentityColumn;
		private System.Type superclass;
		private bool dynamicUpdate;

		private string identitySelectString;

		private System.Type[] proxyInterfaces; //not array?
		private System.Type concreteProxyClass;
		private bool hasProxy;
		private bool hasEmbeddedIdentifier;

		private string[] identifierColumnNames;
		private Cascades.IdentifierValue unsavedIdentifierValue;

		private Hashtable idTypesByPropertyPath = new Hashtable();
		private Hashtable idColumnNamesByPropertyPath = new Hashtable();
		protected Hashtable columnNamesByPropertyPath = new Hashtable();
		protected Hashtable typesByPropertyPath = new Hashtable();

		private string identifierPropertyName;
		private IType identifierType;
		private ReflectHelper.Setter identifierSetter;
		private ReflectHelper.Getter identifierGetter;
		private PropertyInfo proxyIdentifierProperty;

		private string[] propertyNames;
		private IType[] propertyTypes;
		private bool[] propertyUpdateability;
		private bool[] propertyInsertability;

		private string versionPropertyName;
		private string versionColumnName;
		private IVersionType versionType;
		private ReflectHelper.Getter versionGetter;
		private int versionProperty;

		private ReflectHelper.Getter[] getters;
		private ReflectHelper.Setter[] setters;
		protected int hydrateSpan;

		private string className;

		private Cascades.CascadeStyle[] cascadeStyles;
		private ICacheConcurrencyStrategy cache;

		public System.Type MappedClass {
			get { return mappedClass; }
		}
		
		public string ClassName {
			get { return className; }
		}

		public string IdentifierSelectFragment(string name, string suffix) {
			return new SelectFragment()
				.SetSuffix(suffix)
				.AddColumns( name, IdentifierColumnNames )
				.ToFragmentString()
				.Substring(2); //string leading ", "
		}

		public virtual string[] ToColumns(string name, string path) {
			string[] cols = null;

			if ( path.Equals(PathExpressionParser.EntityClass) ) {
				cols = new string[] { DiscriminatorColumnName };
			} else {
				string idprop = IdentifierPropertyName;
				if (PathExpressionParser.EntityID.Equals(path) ||
								( idprop!=null && path.Equals(idprop) ) ) {
					cols = IdentifierColumnNames;
				} else if (path.StartsWith(PathExpressionParser.EntityID + StringHelper.Dot) ||
								( idprop!=null && path.StartsWith(idprop + StringHelper.Dot) ) ) {
					int loc = path.IndexOf(".");
					string componentPath = path.Substring(loc+1);

					if (IdentifierType.IsComponentType) {
						cols = GetIdentifierPropertyColumnNames(componentPath);
						if (cols==null) throw new QueryException("composite id path not found (or dereferenced a <key-many-to-one>)");
					} else {
						throw new QueryException("unresolved id property: " + path);
					}
				} else {
					return null;
				}
			}
			return StringHelper.Prefix(cols, name + StringHelper.Dot);
		}

		public IType GetPropertyType(string path) {
			if (path.Equals(PathExpressionParser.EntityClass)) {
				return DiscriminatorType;
			} else {
				string idprop = IdentifierPropertyName;

				if ( PathExpressionParser.EntityID.Equals(path) ||
					(idprop!=null && path.Equals(idprop)) ) {
					return IdentifierType;
				} else if ( path.StartsWith(PathExpressionParser.EntityID + StringHelper.Dot) ||
					(idprop != null && path.StartsWith(idprop + StringHelper.Dot) ) ) {
					return GetIdentifierPropertyType( path.Substring(PathExpressionParser.EntityID.Length+1) );
				} else {
					return (IType) typesByPropertyPath[path];
				}
			}
		}

		public Cascades.CascadeStyle[] PropertyCascadeStyles {
			get { return cascadeStyles; }
		}

		public void SetPropertyValues(object obj, object[] values) {
			for (int j=0; j<hydrateSpan; j++)
				Setters[j].Set(obj, values[j]);
		}

		public object[] GetPropertyValues(object obj) {
			object[] result = new object[hydrateSpan];
			for (int j=0;j<hydrateSpan; j++)
				result[j] = Getters[j].Get(obj);
			return result;
		}

		public object GetPropertyValue(object obj, int i) {
			return Getters[i].Get(obj);
		}

		public void SetPropertyValue(object obj, int i, object value) {
			Setters[i].Set(obj, value);
		}

		public int[] FindDirty(object[] x, object[] y, object obj, ISessionImplementor session) {
			int[] props = TypeFactory.FindDirty(propertyTypes, x, y, propertyUpdateability, session);
			if (props==null) {
				return null;
			} else {
				if ( log.IsDebugEnabled ) {
					for (int i=0; i<props.Length; i++) {
						log.Debug( className + StringHelper.Dot + propertyNames[ props[i] ] + " is dirty");
					}
				}
				return props;
			}
		}

		public object GetIdentifier(object obj) {
			object id;
			if (hasEmbeddedIdentifier) {
				id = obj;
			} else {
				if (identifierGetter==null) throw new HibernateException( "The class has no identifier property: " + className );
				id = identifierGetter.Get(obj);
			}
			return id;
		}

		public object GetVersion(object obj) {
			if (!versioned) return null;
			return versionGetter.Get(obj);
		}

		public void SetIdentifier(object obj, object id) {
			if (identifierSetter!=null) {
				identifierSetter.Set(obj, id);
			}
		}

		public object Instantiate(object id) {
			if (hasEmbeddedIdentifier) {
				return id;
			} else {
				if (abstractClass) throw new HibernateException("Cannot instantiate abstract class or interface: " + className);
				try {
					return constructor.Invoke(null);
				} catch (Exception e) {
					throw new InstantiationException("Could not instantiate entity: ", mappedClass, e);
				}
			}
		}

		protected ReflectHelper.Setter[] Setters {
			get { return setters; }
		}

		protected ReflectHelper.Getter[] Getters {
			get { return getters; }
		}

		public IType[] PropertyTypes {
			get { return propertyTypes; }
		}

		public IType IdentifierType {
			get { return identifierType; }
		}

		public string[] IdentifierColumnNames {
			get { return identifierColumnNames; }
		}

		public bool IsPolymorphic {
			get { return polymorphic; }
		}

		public bool IsInherited {
			get { return inherited; }
		}

		public bool HasCompositeKey {
			get { return identifierColumnNames.Length > 1; }
		}

		public bool HasCascades {
			get { return hasCascades; }
		}

		public ICacheConcurrencyStrategy Cache {
			get { return cache; }
		}

		public bool HasIdentifierProperty {
			get { return identifierGetter!=null; }
		}

		public PropertyInfo ProxyIdentifierProperty {
			get { return proxyIdentifierProperty; }
		}

		public IVersionType VersionType {
			get { return versionType; }
		}

		public int VersionProperty {
			get { return versionProperty; }
		}

		public bool IsVersioned {
			get { return versioned; }
		}

		public bool IsIdentifierAssignedByInsert {
			get { return useIdentityColumn; }
		}

		public bool IsUnsaved(object id) {
			return unsavedIdentifierValue.IsUnsaved(id);
		}

		public string[] PropertyNames {
			get { return propertyNames; }
		}

		public string IdentifierPropertyName {
			get { return identifierPropertyName; }
		}

		public string VersionColumnName {
			get { return versionColumnName; }
		}

		public string[] GetIdentifierPropertyColumnNames(string path) {
			return (string[]) idColumnNamesByPropertyPath[path];
		}

		public IType GetIdentifierPropertyType(string path) {
			return (IType) idTypesByPropertyPath[path];
		}

		public string[] GetPropertyColumnNames(string path) {
			return (string[]) columnNamesByPropertyPath[path];
		}

		public bool ImplementsLifecycle {
			get { return implementsLifecycle; }
		}

		public bool ImplementsValidatable {
			get { return implementsValidatable; }
		}

		public bool HasCollections {
			get { return hasCollections; }
		}

		public bool IsMutable {
			get { return mutable; }
		}

		public bool HasCache {
			get { return cache!=null; }
		}

		public bool HasSubclasses {
			get { return hasSubclasses; }
		}

		public System.Type[] ProxyInterfaces {
			get { return proxyInterfaces; }
		}

		public bool HasProxy {
			get { return hasProxy; }
		}

		public string SqlIdentitySelect {
			get { return identitySelectString; }
		}

		public IIdentifierGenerator IdentifierGenerator {
			get {
				if (idgen==null)
					throw new HibernateException("No ID SchemaExport is configured for class " + className + " (Try using Insert() with an assigned ID)");
				return idgen;
			}
		}

		protected void Check(int rows, object id) {
			if (rows<1) {
				throw new StaleObjectStateException( MappedClass, id );
			} else if (rows>1) {
				throw new HibernateException("Duplicate identifier in table for " + ClassName + ": " + id);
			}
		}

		protected AbstractEntityPersister(PersistentClass model, ISessionFactoryImplementor factory) {
			this.dialect = factory.Dialect;

			// CLASS

			className = model.PersistentClazz.Name;
			mappedClass = model.PersistentClazz;

			mutable = model.IsMutable;
			dynamicUpdate = model.DynamicUpdate;

			polymorphic = model.IsPolymorphic;
			explicitPolymorphism = model.IsExplicitPolymorphism;
			inherited = model.IsInherited;
			superclass = inherited ? model.Superclass.PersistentClazz : null;
			hasSubclasses = model.HasSubclasses;

			constructor = ReflectHelper.GetDefaultConstructor(mappedClass);
			abstractClass = ReflectHelper.IsAbstractClass(mappedClass);

			// IDENTIFIER

			hasEmbeddedIdentifier = model.HasEmbeddedIdentifier;
			identifierPropertyName = model.HasIdentifierProperty ? model.IdentifierProperty.Name : null;
			Value idValue = model.Identifier;
			identifierType = idValue.Type;

			if (identifierPropertyName!=null) {
				identifierSetter = ReflectHelper.GetSetter(mappedClass, identifierPropertyName);
				identifierGetter = ReflectHelper.GetGetter(mappedClass, identifierPropertyName);

				PropertyInfo proxyGetter = identifierGetter.Property;
				try {
					proxyGetter = ReflectHelper.GetGetter( model.ProxyInterface, identifierPropertyName ).Property;
				} catch (Exception) {}
				proxyIdentifierProperty = proxyGetter;
			} else {
				identifierGetter = null;
				identifierSetter = null;
				proxyIdentifierProperty = null;
			}

			// HYDRATE SPAN

			hydrateSpan = model.PropertyClosureCollection.Count;

			// IDENTIFIER 
			
			int idColumnSpan = model.Identifier.ColumnSpan;
			identifierColumnNames = new string[idColumnSpan];

			int i=0;
			foreach(Column col in idValue.ColumnCollection) {
				identifierColumnNames[i] = col.Name;
				i++;
			}

			if (idValue.IsComposite) {
				foreach(Property prop in ((Component)idValue).PropertyCollection) {
					idTypesByPropertyPath[prop.Name] = prop.Type;
					
					string[] cols = new string[prop.ColumnSapn];
					int j=0;
					foreach(Column col in prop.ColumnCollection) {
						cols[j++] = col.Name;
					}

					idColumnNamesByPropertyPath[ prop.Name ] = cols;
					if (model.HasEmbeddedIdentifier) {
						columnNamesByPropertyPath[ prop.Name ] = cols;
						typesByPropertyPath[ prop.Name ] = prop.Type;
					}
				}
			}

			// GENERATOR
			idgen = model.Identifier.CreateIdentifierGenerator(dialect);
			useIdentityColumn = idgen is IdentityGenerator;
			identitySelectString = (useIdentityColumn) ? dialect.IdentitySelectString : null;

			// UNSAVED-VALUE:

			string cts = model.Identifier.NullValue;
			if (cts==null || "any".Equals(cts) ) {
				unsavedIdentifierValue = Cascades.IdentifierValue.SaveAny;
			} else if ( "none".Equals(cts) ) {
				unsavedIdentifierValue = Cascades.IdentifierValue.SaveNone;
			} else if ( "null".Equals(cts) ) {
				unsavedIdentifierValue = Cascades.IdentifierValue.SaveNull;
			} else {
				try {
					unsavedIdentifierValue = new Cascades.IdentifierValue( ((IIdentifierType)model.Identifier.Type ).StringToObject(cts) );
				} catch (Exception) {
					throw new MappingException("Could not parse unsaved-value: " + cts);
				}
			}

			// VERSION:

			if (model.IsVersioned) {
				foreach(Column col in model.Version.ColumnCollection) {
					versionColumnName = col.Name; //only hapens once
				}
			} else {
				versionColumnName = null;
			}

			if (model.IsVersioned) {
				versionPropertyName = model.Version.Name;
				versioned = true;
				versionGetter = ReflectHelper.GetGetter(mappedClass, versionPropertyName);
				versionType = (IVersionType) model.Version.Type;
			} else {
				versionPropertyName = null;
				versioned = false;
				versionType = null;
				versionGetter = null;
			}

			// PROPERTIES 

			propertyTypes = new IType[hydrateSpan];
			propertyNames = new string[hydrateSpan];
			propertyUpdateability = new bool[hydrateSpan];
			propertyInsertability = new bool[hydrateSpan];
			getters = new ReflectHelper.Getter[hydrateSpan];
			setters = new ReflectHelper.Setter[hydrateSpan];
			cascadeStyles = new Cascades.CascadeStyle[hydrateSpan];
			string[] setterNames = new string[hydrateSpan];
			string[] getterNames = new string[hydrateSpan];
			System.Type[] types = new System.Type[hydrateSpan];

			i=0;
			int tempVersionProperty=-66;
			bool foundCascade = false;

			foreach(Property prop in model.PropertyClosureCollection) {
				if (prop==model.Version) tempVersionProperty = i;
				propertyNames[i] = prop.Name;
				getters[i] = ReflectHelper.GetGetter( mappedClass, propertyNames[i] );
				setters[i] = ReflectHelper.GetSetter( mappedClass, propertyNames[i] );
				getterNames[i] = getters[i].Property.Name;
				setterNames[i] = setters[i].Property.Name;
				types[i] = getters[i].Property.PropertyType;
				propertyUpdateability[i] = prop.IsUpdateable;
				propertyInsertability[i] = prop.IsInsertable;

				cascadeStyles[i] = prop.CascadeStyle;
				if ( cascadeStyles[i] != Cascades.CascadeStyle.StyleNone ) foundCascade = true;

				i++;
			}

			hasCascades = foundCascade;
			versionProperty = tempVersionProperty;

			// CALLBACK INTERFACES
			implementsLifecycle = typeof(ILifecycle).IsAssignableFrom(mappedClass);
			implementsValidatable = typeof(IValidatable).IsAssignableFrom(mappedClass);

			cache = model.Cache;

			hasCollections = InitHasCollections();

			// PROXIES
			System.Type pi = model.ProxyInterface;
			hasProxy = pi!=null;
			Hashtable pis = new Hashtable();
			pis.Add(typeof(HibernateProxy), null);
			if (!mappedClass.Equals(pi) ) pis.Add(pi, null);
			concreteProxyClass = pi;

			if (hasProxy) {
				foreach(Subclass sc in model.SubclassCollection) {
					pi = sc.ProxyInterface;
					if (pi==null) throw new MappingException( "All subclasses must also have proxies: " + mappedClass.Name);
					if ( !sc.PersistentClazz.Equals(pi) ) pis.Add(pi, null);
				}
			}

			proxyInterfaces = new System.Type[pis.Count];
			i=0;
			foreach(System.Type type in pis.Keys) {
				proxyInterfaces[i++] = type;
			}

		}

		private bool InitHasCollections() {
			return InitHasCollections(propertyTypes);
		}

		private bool InitHasCollections(IType[] types) {
			for (int i=0; i<types.Length; i++) {
				if (types[i].IsPersistentCollectionType) {
					return true;
				} else if (types[i].IsComponentType) {
					if ( InitHasCollections( ((IAbstractComponentType) types[i]).Subtypes) )
						return true;
				}
			}
			return false;
		}

		public IClassMetadata ClassMetadata {
			get { return (IClassMetadata) this; }
		}

		public System.Type ConcreteProxyClass {
			get { return concreteProxyClass; }
		}

		public System.Type MappedSuperclass {
			get { return superclass; }
		}

		public bool IsExplicitPolymorphism {
			get { return explicitPolymorphism; }
		}

		public bool[] PropertyUpdateability {
			get { return propertyUpdateability; }
		}

		protected bool UseDynamicUpdate {
			get { return dynamicUpdate; }
		}

		public bool[] PropertyInsertability {
			get { return propertyInsertability; }
		}


		public abstract string QueryWhereFragment(string alias, bool innerJoin, bool includeSublcasses);
		public abstract string DiscriminatorSQLString { get; }
		public abstract void Delete(object id, object version, object obj, ISessionImplementor session);
		public abstract object[] PropertySpaces { get; }
		public abstract object IdentifierSpace { get; }
		public abstract void Insert(object id, object[] fields, object obj, ISessionImplementor session);
		public abstract object Insert(object[] fields, object obj, ISessionImplementor session);
		public abstract object Load(object id, object optionalObject, LockMode lockMode, ISessionImplementor session);
		public abstract void Lock(object id, object version, object obj, LockMode lockMode, ISessionImplementor session);
		public abstract void PostInstantiate(ISessionFactoryImplementor factory);
		public abstract void Update(object id, object[] fields, int[] dirtyFields, object oldVersion, object obj, ISessionImplementor session);
		public abstract int CountSubclassProperties();
		public abstract IDiscriminatorType DiscriminatorType { get; }
		public abstract OuterJoinLoaderType EnableJoinedFetch(int i);
		public abstract string FromJoinFragment(string alias, bool innerJoin, bool includeSubclasses);
		public abstract string FromTableFragment(string alias);
		public abstract string GetConcreteClassAlias(string alias);
		public abstract string[] GetPropertyColumnNames(int i);
		public abstract System.Type GetSubclassForDiscriminatorValue(object value);
		public abstract IType GetSubclassPropertyType(int i);
		public abstract bool IsDefinedOnSubclass(int i);
		public abstract string PropertySelectFragment(string alias, string suffix);
		public abstract string TableName { get; }
		public abstract string[] ToColumns(string name, int i);
		public abstract string WhereJoinFragment(string alias, bool innerJoin, bool includeSubclasses);
		public abstract string DiscriminatorColumnName { get; }
		public abstract string[] GetSubclassPropertyColumnNames(int i);
	}
}
