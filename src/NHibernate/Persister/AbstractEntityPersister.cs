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

		protected static readonly System.Type[] NoClasses = new System.Type[0];

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

		private readonly System.Type[] proxyInterfaces; //not array?
		private System.Type concreteProxyClass;
		private bool hasProxy;
		protected bool hasEmbeddedIdentifier;

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
		private readonly Hashtable gettersByPropertyName = new Hashtable();
		private readonly Hashtable settersByPropertyName = new Hashtable();
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

		public virtual string IdentifierSelectFragment(string name, string suffix) {
			//TODO: fix this once the interface is changed from a string to SqlString
			// this works now because there are no parameters in the select fragment string
			return new SqlCommand.SelectFragment(dialect)
				.SetSuffix(suffix)
				.AddColumns( name, IdentifierColumnNames )
				.ToSqlStringFragment(false)
				.ToString();
				//.Substring(2); //strip leading ", " - commented out because of the "false" parameter now does that
		}

		public abstract string[] ToColumns(string name, string path) ;

		public virtual IType GetPropertyType(string path) {
			return (IType) typesByPropertyPath[path];
		}

		public virtual Cascades.CascadeStyle[] PropertyCascadeStyles {
			get { return cascadeStyles; }
		}

		public virtual void SetPropertyValues(object obj, object[] values) {
			//TODO: optimizer implementation
			for (int j=0; j<hydrateSpan; j++)
				Setters[j].Set(obj, values[j]);
		}

		public virtual object[] GetPropertyValues(object obj) {
			//TODO: optimizer implementation
			object[] result = new object[hydrateSpan];
			for (int j=0;j<hydrateSpan; j++)
				result[j] = Getters[j].Get(obj);
			return result;
		}

		public virtual object GetPropertyValue(object obj, int i) {
			return Getters[i].Get(obj);
		}

		public virtual void SetPropertyValue(object obj, int i, object value) {
			Setters[i].Set(obj, value);
		}

		public virtual int[] FindDirty(object[] x, object[] y, object obj, ISessionImplementor session) {
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

		public virtual object GetIdentifier(object obj) {
			object id;
			if (hasEmbeddedIdentifier) {
				id = obj;
			} else {
				if (identifierGetter==null) throw new HibernateException( "The class has no identifier property: " + className );
				id = identifierGetter.Get(obj);
			}
			return id;
		}

		public virtual object GetVersion(object obj) {
			if (!versioned) return null;
			return versionGetter.Get(obj);
		}

		public virtual void SetIdentifier(object obj, object id) {
			if(hasEmbeddedIdentifier) {
				ComponentType copier = (ComponentType) identifierType;
				copier.SetPropertyValues(obj, copier.GetPropertyValues(id));
			}
			else if (identifierSetter!=null) {
				identifierSetter.Set(obj, id);
			}
		}

		public virtual object Instantiate(object id) {
			if (hasEmbeddedIdentifier && id.GetType() == mappedClass) {
				return id;
			} else {
				if (abstractClass) throw new HibernateException("Cannot instantiate abstract class or interface: " + className);
				//TODO: optimizer implementation
				try {
					return constructor.Invoke(null);
				} catch (Exception e) {
					throw new InstantiationException("Could not instantiate entity: ", mappedClass, e);
				}
			}
		}

		protected virtual ReflectHelper.Setter[] Setters {
			get { return setters; }
		}

		protected virtual ReflectHelper.Getter[] Getters {
			get { return getters; }
		}

		public virtual IType[] PropertyTypes {
			get { return propertyTypes; }
		}

		public virtual IType IdentifierType {
			get { return identifierType; }
		}

		public virtual string[] IdentifierColumnNames {
			get { return identifierColumnNames; }
		}

		public virtual bool IsPolymorphic {
			get { return polymorphic; }
		}

		public virtual bool IsInherited {
			get { return inherited; }
		}

		public virtual bool HasCompositeKey {
			get { return identifierColumnNames.Length > 1; }
		}

		public virtual bool HasCascades {
			get { return hasCascades; }
		}

		public virtual ICacheConcurrencyStrategy Cache {
			get { return cache; }
		}

		public virtual bool HasIdentifierProperty {
			get { return identifierGetter!=null; }
		}

		public virtual PropertyInfo ProxyIdentifierProperty {
			get { return proxyIdentifierProperty; }
		}

		public virtual IVersionType VersionType {
			get { return versionType; }
		}

		public virtual int VersionProperty {
			get { return versionProperty; }
		}

		public virtual bool IsVersioned {
			get { return versioned; }
		}

		public virtual bool IsIdentifierAssignedByInsert {
			get { return useIdentityColumn; }
		}

		public virtual bool IsUnsaved(object id) {
			return unsavedIdentifierValue.IsUnsaved(id);
		}

		public virtual string[] PropertyNames {
			get { return propertyNames; }
		}

		public virtual string IdentifierPropertyName {
			get { return identifierPropertyName; }
		}

		public virtual string VersionColumnName {
			get { return versionColumnName; }
		}

		public string[] GetPropertyColumnNames(string path) {
			return (string[]) columnNamesByPropertyPath[path];
		}

		public virtual bool ImplementsLifecycle {
			get { return implementsLifecycle; }
		}

		public virtual bool ImplementsValidatable {
			get { return implementsValidatable; }
		}

		public virtual bool HasCollections {
			get { return hasCollections; }
		}

		public virtual bool IsMutable {
			get { return mutable; }
		}

		public virtual bool HasCache {
			get { return cache!=null; }
		}

		public virtual bool HasSubclasses {
			get { return hasSubclasses; }
		}

		public virtual System.Type[] ProxyInterfaces {
			get { return proxyInterfaces; }
		}

		public virtual bool HasProxy {
			get { return hasProxy; }
		}

		/// <summary>
		/// Returns the SQL used to get the Identity value from the last insert.
		/// </summary>
		/// <remarks>This is not a NHibernate Command because there are no parameters.</remarks>
		public string SqlIdentitySelect {
			get { return identitySelectString; }
		}

		public virtual IIdentifierGenerator IdentifierGenerator {
			get {
				if (idgen==null)
					throw new HibernateException("No ID SchemaExport is configured for class " + className + " (Try using Insert() with an assigned ID)");
				return idgen;
			}
		}

		protected virtual void Check(int rows, object id) {
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
					System.Type proxy = model.ProxyInterface;
					if(proxy != null) proxyGetter = ReflectHelper.GetGetter( proxy, identifierPropertyName ).Property;
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
				identifierColumnNames[i] = col.GetQuotedName(dialect);
				i++;
			}

			// GENERATOR
			idgen = model.Identifier.CreateIdentifierGenerator(dialect);
			useIdentityColumn = idgen is IdentityGenerator;
			identitySelectString = useIdentityColumn ? dialect.IdentitySelectString : null;

			// UNSAVED-VALUE:

			string unsavedValue = model.Identifier.NullValue;
			if (unsavedValue==null || "any".Equals(unsavedValue) ) {
				unsavedIdentifierValue = Cascades.IdentifierValue.SaveAny;
			} else if ( "none".Equals(unsavedValue) ) {
				unsavedIdentifierValue = Cascades.IdentifierValue.SaveNone;
			} else if ( "null".Equals(unsavedValue) ) {
				unsavedIdentifierValue = Cascades.IdentifierValue.SaveNull;
			} else {
				IType idType = model.Identifier.Type;
				try {
					unsavedIdentifierValue = new Cascades.IdentifierValue( ((IIdentifierType)idType ).StringToObject(unsavedValue) );
				}
				catch (InvalidCastException) {
					throw new MappingException("Bad identifier type: " + idType.GetType().Name );
				}
				catch (Exception) {
					throw new MappingException("Could not parse unsaved-value: " + unsavedValue);
				}
			}

			// VERSION:

			if (model.IsVersioned) {
				foreach(Column col in model.Version.ColumnCollection) {
					versionColumnName = col.GetQuotedName(dialect); //only hapens once
				}
			} else {
				versionColumnName = null;
			}

			if (model.IsVersioned) {
				versionPropertyName = model.Version.Name;
				versioned = true;
				versionGetter = ReflectHelper.GetGetter(mappedClass, versionPropertyName);
				if (!(model.Version.Type is IVersionType))
				{
					log.Warn(model.Name + " has version column " + model.Version.Name + ", but the column type " + model.Version.Type.Name + " is not versionable");
					versionPropertyName = null;
					versioned = false;
					versionType = null;
					versionGetter = null;
				}
				else
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
				propertyTypes[i] = prop.Type;
				propertyUpdateability[i] = prop.IsUpdateable;
				propertyInsertability[i] = prop.IsInsertable;

				gettersByPropertyName.Add( propertyNames[i], getters[i] );
				settersByPropertyName.Add( propertyNames[i], setters[i] );

				cascadeStyles[i] = prop.CascadeStyle;
				if ( cascadeStyles[i] != Cascades.CascadeStyle.StyleNone ) foundCascade = true;

				i++;
			}

			//TODO: optimizer implementation

			hasCascades = foundCascade;
			versionProperty = tempVersionProperty;

			// CALLBACK INTERFACES
			implementsLifecycle = typeof(ILifecycle).IsAssignableFrom(mappedClass);
			implementsValidatable = typeof(IValidatable).IsAssignableFrom(mappedClass);

			cache = model.Cache;

			hasCollections = InitHasCollections();

			// PROXIES
			System.Type pi = model.ProxyInterface;
			hasProxy = pi!=null;  //TODO: && Environment.jvmSupportsProxies();
			ArrayList pis = new ArrayList();
			pis.Add(typeof(HibernateProxy));
			if (!mappedClass.Equals(pi) && pi!=null ) pis.Add(pi); // != null because we use arraylist instead of hashset
			concreteProxyClass = pi;

			if (hasProxy) {
				foreach(Subclass sc in model.SubclassCollection) {
					pi = sc.ProxyInterface;
					if (pi==null) throw new MappingException( "All subclasses must also have proxies: " + mappedClass.Name);
					if ( !sc.PersistentClazz.Equals(pi) ) pis.Add(pi);
				}
			}

			proxyInterfaces = (System.Type[]) pis.ToArray( typeof(System.Type) );
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

		public virtual IClassMetadata ClassMetadata {
			get { return (IClassMetadata) this; }
		}

		public virtual System.Type ConcreteProxyClass {
			get { return concreteProxyClass; }
		}

		public virtual System.Type MappedSuperclass {
			get { return superclass; }
		}

		public virtual bool IsExplicitPolymorphism {
			get { return explicitPolymorphism; }
		}

		public virtual bool[] PropertyUpdateability {
			get { return propertyUpdateability; }
		}

		protected virtual bool UseDynamicUpdate {
			get { return dynamicUpdate; }
		}

		public virtual bool[] PropertyInsertability {
			get { return propertyInsertability; }
		}

		public virtual object GetPropertyValue(object obj, string propertyName) {
			return ( (ReflectHelper.Getter) gettersByPropertyName[propertyName] ).Get(obj);
		}

		public virtual void SetPropertyValue(object obj, string propertyName, object value) {
			( (ReflectHelper.Setter) settersByPropertyName[propertyName] ).Set(obj, value);
		}
	
		protected virtual bool HasEmbeddedIdentifier {
			get {
				return hasEmbeddedIdentifier;
			}
		}

		public virtual bool HasIdentifierPropertyOrEmbeddedCompositeIdentifier 
		{
			get { return HasIdentifierProperty || hasEmbeddedIdentifier;}
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
		public abstract string GetSubclassPropertyTableName(int i) ;
		public abstract string GetSubclassPropertyName(int i);
	}
}
