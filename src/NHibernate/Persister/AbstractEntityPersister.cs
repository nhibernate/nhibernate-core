using System;
using System.Collections;
using System.Reflection;

using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Hql;
using NHibernate.Id;
using NHibernate.Loader;
using NHibernate.Mapping;
using NHibernate.Metadata;
using NHibernate.Proxy;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Persister 
{
	/// <summary>
	/// Superclass for built-in mapping strategies. Implements functionalty common to both mapping
	/// strategies
	/// </summary>
	/// <remarks>
	/// May be considred an immutable view of the mapping object
	/// </remarks>
	public abstract class AbstractEntityPersister : IQueryable, IClassMetadata 
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(AbstractEntityPersister));

		protected static readonly System.Type[] NoClasses = new System.Type[0];

		private System.Type mappedClass;
		[NonSerialized] protected Dialect.Dialect dialect;
		[NonSerialized] private ConstructorInfo constructor;

		[NonSerialized] private IIdentifierGenerator idgen;
		[NonSerialized] private bool polymorphic;
		[NonSerialized] private bool explicitPolymorphism;
		[NonSerialized] private bool inherited;
		[NonSerialized] private bool hasSubclasses;
		[NonSerialized] private bool versioned;
		[NonSerialized] private bool abstractClass;
		[NonSerialized] private bool implementsLifecycle;
		[NonSerialized] private bool implementsValidatable;
		[NonSerialized] private bool hasCollections;
		[NonSerialized] private bool hasCascades;
		[NonSerialized] private bool mutable;
		[NonSerialized] private bool useIdentityColumn;
		[NonSerialized] private System.Type superclass;
		[NonSerialized] private bool dynamicUpdate;
		[NonSerialized] private bool dynamicInsert;
		[NonSerialized] private string sqlWhereString;
		[NonSerialized] private string sqlWhereStringTemplate;

		[NonSerialized] private string identitySelectString;

		[NonSerialized] private readonly System.Type[] proxyInterfaces;
		[NonSerialized] private System.Type concreteProxyClass;
		[NonSerialized] private bool hasProxy;
		[NonSerialized] protected bool hasEmbeddedIdentifier;

		[NonSerialized] private string[] identifierColumnNames;
		[NonSerialized] private Cascades.IdentifierValue unsavedIdentifierValue;

		[NonSerialized] protected Hashtable columnNamesByPropertyPath = new Hashtable();
		[NonSerialized] protected Hashtable typesByPropertyPath = new Hashtable();

		[NonSerialized] private string identifierPropertyName;
		[NonSerialized] private IType identifierType;
		[NonSerialized] private Property.ISetter identifierSetter;
		[NonSerialized] private Property.IGetter identifierGetter;
		[NonSerialized] private PropertyInfo proxyIdentifierProperty;

		[NonSerialized] private string[] propertyNames;
		[NonSerialized] private IType[] propertyTypes;
		[NonSerialized] private bool[] propertyUpdateability;
		[NonSerialized] private bool[] propertyInsertability;

		[NonSerialized] private string versionPropertyName;
		[NonSerialized] private string versionColumnName;
		[NonSerialized] private IVersionType versionType;
		[NonSerialized] private Property.IGetter versionGetter;
		[NonSerialized] private int versionProperty;

		[NonSerialized] private Property.IGetter[] getters;
		[NonSerialized] private Property.ISetter[] setters;
		[NonSerialized] private readonly Hashtable gettersByPropertyName = new Hashtable();
		[NonSerialized] private readonly Hashtable settersByPropertyName = new Hashtable();
		[NonSerialized] protected int hydrateSpan;

		[NonSerialized] private string className;

		[NonSerialized] private Cascades.CascadeStyle[] cascadeStyles;
		[NonSerialized] private ICacheConcurrencyStrategy cache;

		// a cglib thing
		//[NonSerialized] private MetaClass optimizer;

		public System.Type MappedClass 
		{
			get { return mappedClass; }
		}
		
		public string ClassName 
		{
			get { return className; }
		}

		public virtual SqlString IdentifierSelectFragment(string name, string suffix) 
		{
			return new SelectFragment(dialect)
				.SetSuffix(suffix)
				.AddColumns( name, IdentifierColumnNames )
				.ToSqlStringFragment(false);
		}

		
		public virtual IType GetPropertyType(string path) 
		{
			return (IType) typesByPropertyPath[path];
		}

		public virtual Cascades.CascadeStyle[] PropertyCascadeStyles 
		{
			get { return cascadeStyles; }
		}

		public virtual void SetPropertyValues(object obj, object[] values) 
		{
			//TODO: optimizer implementation
			for (int j=0; j<hydrateSpan; j++) 
			{
				Setters[j].Set(obj, values[j]);
			}
		}

		public virtual object[] GetPropertyValues(object obj) 
		{
			//TODO: optimizer implementation
			object[] result = new object[hydrateSpan];
			for (int j=0;j<hydrateSpan; j++) 
			{
				result[j] = Getters[j].Get(obj);
			}
			return result;
		}

		public virtual object GetPropertyValue(object obj, int i) 
		{
			return Getters[i].Get(obj);
		}

		public virtual void SetPropertyValue(object obj, int i, object value) 
		{
			Setters[i].Set(obj, value);
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
			int[] props = TypeFactory.FindDirty(propertyTypes, x, y, propertyUpdateability, session);
			if (props==null) 
			{
				return null;
			} 
			else 
			{
				if ( log.IsDebugEnabled ) 
				{
					for (int i=0; i<props.Length; i++) 
					{
						log.Debug( className + StringHelper.Dot + propertyNames[ props[i] ] + " is dirty");
					}
				}
				return props;
			}
		}

		public virtual object GetIdentifier(object obj) 
		{
			object id;
			if (hasEmbeddedIdentifier) 
			{
				id = obj;
			} 
			else 
			{
				if (identifierGetter==null) throw new HibernateException( "The class has no identifier property: " + className );
				id = identifierGetter.Get(obj);
			}
			return id;
		}

		public virtual object GetVersion(object obj) 
		{
			if (!versioned) return null;
			return versionGetter.Get(obj);
		}

		public virtual void SetIdentifier(object obj, object id) 
		{
			if(hasEmbeddedIdentifier) 
			{
				ComponentType copier = (ComponentType) identifierType;
				copier.SetPropertyValues(obj, copier.GetPropertyValues(id));
			}
			else if (identifierSetter!=null) 
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
			if (hasEmbeddedIdentifier && id.GetType()==mappedClass) 
			{
				return id;
			} 
			else 
			{
				if (abstractClass) throw new HibernateException("Cannot instantiate abstract class or interface: " + className);
				//TODO: optimizer implementation
				try 
				{
					return constructor.Invoke(null);
				} 
				catch (Exception e) 
				{
					throw new InstantiationException("Could not instantiate entity: ", mappedClass, e);
				}
			}
		}

		protected virtual Property.ISetter[] Setters 
		{
			get { return setters; }
		}

		protected virtual Property.IGetter[] Getters 
		{
			get { return getters; }
		}

		public virtual IType[] PropertyTypes 
		{
			get { return propertyTypes; }
		}

		public virtual IType IdentifierType 
		{
			get { return identifierType; }
		}

		public virtual string[] IdentifierColumnNames 
		{
			get { return identifierColumnNames; }
		}

		public virtual bool IsPolymorphic 
		{
			get { return polymorphic; }
		}

		public virtual bool IsInherited 
		{
			get { return inherited; }
		}

		public virtual bool HasCascades 
		{
			get { return hasCascades; }
		}

		public virtual ICacheConcurrencyStrategy Cache 
		{
			get { return cache; }
		}

		public virtual bool HasIdentifierProperty 
		{
			get { return identifierGetter!=null; }
		}

		public virtual PropertyInfo ProxyIdentifierProperty 
		{
			get { return proxyIdentifierProperty; }
		}

		public virtual IVersionType VersionType 
		{
			get { return versionType; }
		}

		public virtual int VersionProperty 
		{
			get { return versionProperty; }
		}

		public virtual bool IsVersioned 
		{
			get { return versioned; }
		}

		public virtual bool IsIdentifierAssignedByInsert 
		{
			get { return useIdentityColumn; }
		}

		public virtual bool IsUnsaved(object id) 
		{
			return unsavedIdentifierValue.IsUnsaved(id);
		}

		public virtual string[] PropertyNames 
		{
			get { return propertyNames; }
		}

		public virtual string IdentifierPropertyName 
		{
			get { return identifierPropertyName; }
		}

		public virtual string VersionColumnName 
		{
			get { return versionColumnName; }
		}

		public string[] GetPropertyColumnNames(string path) 
		{
			return (string[]) columnNamesByPropertyPath[path];
		}

		public virtual bool ImplementsLifecycle 
		{
			get { return implementsLifecycle; }
		}

		public virtual bool ImplementsValidatable 
		{
			get { return implementsValidatable; }
		}

		public virtual bool HasCollections 
		{
			get { return hasCollections; }
		}

		public virtual bool IsMutable 
		{
			get { return mutable; }
		}

		public virtual bool HasCache 
		{
			get { return cache!=null; }
		}

		public virtual bool HasSubclasses 
		{
			get { return hasSubclasses; }
		}

		public virtual System.Type[] ProxyInterfaces 
		{
			get { return proxyInterfaces; }
		}

		public virtual bool HasProxy 
		{
			get { return hasProxy; }
		}

		/// <summary>
		/// Returns the SQL used to get the Identity value from the last insert.
		/// </summary>
		/// <remarks>This is not a NHibernate Command because there are no parameters.</remarks>
		public string SqlIdentitySelect 
		{
			get { return identitySelectString; }
		}

		public virtual IIdentifierGenerator IdentifierGenerator 
		{
			get 
			{
				if (idgen==null) 
				{
					throw new HibernateException("No ID SchemaExport is configured for class " + className + " (Try using Insert() with an assigned ID)");
				}
				return idgen;
			}
		}

		/// <summary>
		/// Checks to make sure that one and only one row was affected
		/// by the IDbCommand that was run.
		/// </summary>
		/// <param name="rows">The results of IDbCommand..ExecuteNonQuery()</param>
		/// <param name="id">The idenitifer of the Entity.  Use for logging purposes.</param>
		protected virtual void Check(int rows, object id) 
		{
			if (rows<1) 
			{
				throw new StaleObjectStateException( MappedClass, id );
			} 
			else if (rows>1) 
			{
				throw new HibernateException("Duplicate identifier in table for " + ClassName + ": " + id);
			}
		}

		protected AbstractEntityPersister(PersistentClass model, ISessionFactoryImplementor factory) 
		{
			this.dialect = factory.Dialect;

			// CLASS

			//className = model.PersistentClazz.Name;
			className = model.PersistentClazz.FullName;
			mappedClass = model.PersistentClazz;

			mutable = model.IsMutable;
			dynamicUpdate = model.DynamicUpdate;
			dynamicInsert = model.DynamicInsert;
			sqlWhereString = model.Where;
			sqlWhereStringTemplate = sqlWhereString==null ?
				null :
				Template.RenderWhereStringTemplate(sqlWhereString, dialect);

			polymorphic = model.IsPolymorphic;
			explicitPolymorphism = model.IsExplicitPolymorphism;
			inherited = model.IsInherited;
			superclass = inherited ? model.Superclass.PersistentClazz : null;
			hasSubclasses = model.HasSubclasses;

			constructor = ReflectHelper.GetDefaultConstructor(mappedClass);
			abstractClass = ReflectHelper.IsAbstractClass(mappedClass);

			// IDENTIFIER

			hasEmbeddedIdentifier = model.HasEmbeddedIdentifier;
			Value idValue = model.Identifier;
			identifierType = idValue.Type;

			if (model.HasIdentifierProperty) 
			{
				Mapping.Property idProperty = model.IdentifierProperty;
				identifierPropertyName = idProperty.Name;
				identifierSetter = idProperty.GetSetter(mappedClass);
				identifierGetter = idProperty.GetGetter(mappedClass);
			} 
			else 
			{
				identifierGetter = null;
				identifierSetter = null;
				proxyIdentifierProperty = null;
			}

			//this code has been modified to be more like h2.1 because of IGetter
			// and ISetter
			System.Type prox = model.ProxyInterface;
			PropertyInfo proxySetIdentifierMethod = null;
			PropertyInfo proxyGetIdentifierMethod = null; 

			if( model.HasIdentifierProperty && prox!=null) 
			{
				Mapping.Property idProperty = model.IdentifierProperty;

				try 
				{
					proxyGetIdentifierMethod = idProperty.GetGetter(prox).Property;
				} 
				catch (PropertyNotFoundException) 
				{
				}

				try 
				{
					proxySetIdentifierMethod = idProperty.GetSetter(prox).Property;
				} 
				catch (PropertyNotFoundException) 
				{
				}
			}

			proxyIdentifierProperty = proxyGetIdentifierMethod;

			// HYDRATE SPAN

			hydrateSpan = model.PropertyClosureCollection.Count;

			// IDENTIFIER 
			
			int idColumnSpan = model.Identifier.ColumnSpan;
			identifierColumnNames = new string[idColumnSpan];

			int i=0;
			foreach(Column col in idValue.ColumnCollection) 
			{
				identifierColumnNames[i] = col.GetQuotedName(dialect);
				i++;
			}

			// GENERATOR
			idgen = model.Identifier.CreateIdentifierGenerator(dialect);
			useIdentityColumn = idgen is IdentityGenerator;
			identitySelectString = useIdentityColumn ? dialect.IdentitySelectString : null;

			// UNSAVED-VALUE:

			string unsavedValue = model.Identifier.NullValue;
			if (unsavedValue==null || "null".Equals(unsavedValue) ) 
			{
				unsavedIdentifierValue = Cascades.IdentifierValue.SaveNull;
			} 
			else if ( "none".Equals(unsavedValue) ) 
			{
				unsavedIdentifierValue = Cascades.IdentifierValue.SaveNone;
			} 
			else if ( "any".Equals(unsavedValue) ) {
				unsavedIdentifierValue = Cascades.IdentifierValue.SaveAny;
			} 
			else 
			{
				IType idType = model.Identifier.Type;
				try 
				{
					unsavedIdentifierValue = new Cascades.IdentifierValue( ((IIdentifierType)idType ).StringToObject(unsavedValue) );
				}
				catch (InvalidCastException) 
				{
					throw new MappingException("Bad identifier type: " + idType.GetType().Name );
				}
				catch (Exception) 
				{
					throw new MappingException("Could not parse unsaved-value: " + unsavedValue);
				}
			}

			// VERSION:

			if (model.IsVersioned) 
			{
				foreach(Column col in model.Version.ColumnCollection) 
				{
					versionColumnName = col.GetQuotedName(dialect); //only hapens once
				}
			} 
			else 
			{
				versionColumnName = null;
			}

			if (model.IsVersioned) 
			{
				versionPropertyName = model.Version.Name;
				versioned = true;
				versionGetter = model.Version.GetGetter(mappedClass);
				// TODO: determine if this block is kept
				// this if-else block is extra logic in nhibernate - not sure if I like it - would rather throw
				// an exception if there is a bad mapping
				if (!(model.Version.Type is IVersionType))
				{
					log.Warn(model.Name + " has version column " + model.Version.Name + ", but the column type " + model.Version.Type.Name + " is not versionable");
					versionPropertyName = null;
					versioned = false;
					versionType = null;
					versionGetter = null;
				}
				else 
				{
					versionType = (IVersionType) model.Version.Type;
				}
			} 
			else 
			{
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
			getters = new Property.IGetter[hydrateSpan];
			setters = new Property.ISetter[hydrateSpan];
			cascadeStyles = new Cascades.CascadeStyle[hydrateSpan];
			string[] setterNames = new string[hydrateSpan];
			string[] getterNames = new string[hydrateSpan];
			System.Type[] types = new System.Type[hydrateSpan];

			i=0;
			int tempVersionProperty=-66;
			bool foundCascade = false;

			foreach(Mapping.Property prop in model.PropertyClosureCollection) 
			{
				if (prop==model.Version) tempVersionProperty = i;
				propertyNames[i] = prop.Name;
				getters[i] = prop.GetGetter( mappedClass );
				setters[i] = prop.GetSetter( mappedClass );
				getterNames[i] = getters[i].PropertyName;
				setterNames[i] = setters[i].PropertyName;
				types[i] = getters[i].ReturnType;
				propertyTypes[i] = prop.Type;
				propertyUpdateability[i] = prop.IsUpdateable;
				propertyInsertability[i] = prop.IsInsertable;

				gettersByPropertyName[ propertyNames[i] ] = getters[i];
				settersByPropertyName[ propertyNames[i] ] = setters[i];

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
			hasProxy = pi!=null;  
			ArrayList pis = new ArrayList();
			pis.Add(typeof(HibernateProxy));
			//pis.Add( typeof(INHibernateProxy) );
			// != null because we use arraylist instead of hashset
			if (!mappedClass.Equals(pi) && pi!=null ) 
			{
				pis.Add(pi); 
			}
			concreteProxyClass = pi;

			if (hasProxy) 
			{
				foreach(Subclass sc in model.SubclassCollection) 
				{
					pi = sc.ProxyInterface;
					if (pi==null) throw new MappingException( "All subclasses must also have proxies: " + mappedClass.Name);
					if ( !sc.PersistentClazz.Equals(pi) ) pis.Add(pi);
				}
			}

			proxyInterfaces = (System.Type[]) pis.ToArray( typeof(System.Type) );
		}

		private bool InitHasCollections() 
		{
			return InitHasCollections(propertyTypes);
		}

		private bool InitHasCollections(IType[] types) 
		{
			for (int i=0; i<types.Length; i++) 
			{
				if (types[i].IsPersistentCollectionType) 
				{
					return true;
				} 
				else if (types[i].IsComponentType) 
				{
					if ( InitHasCollections( ((IAbstractComponentType) types[i]).Subtypes) )
						return true;
				}
			}
			return false;
		}

		public virtual IClassMetadata ClassMetadata 
		{
			get { return (IClassMetadata) this; }
		}

		public virtual System.Type ConcreteProxyClass 
		{
			get { return concreteProxyClass; }
		}

		public virtual System.Type MappedSuperclass 
		{
			get { return superclass; }
		}

		public virtual bool IsExplicitPolymorphism 
		{
			get { return explicitPolymorphism; }
		}

		public virtual bool[] PropertyUpdateability 
		{
			get { return propertyUpdateability; }
		}

		protected virtual bool UseDynamicUpdate 
		{
			get { return dynamicUpdate; }
		}

		protected virtual bool UseDynamicInsert 
		{
			get { return dynamicInsert; }
		}

		public virtual bool[] PropertyInsertability 
		{
			get { return propertyInsertability; }
		}

		public virtual object GetPropertyValue(object obj, string propertyName) 
		{
			Property.IGetter getter = (Property.IGetter) gettersByPropertyName[propertyName];
			if(getter==null) throw new HibernateException("unmapped property: " + propertyName);
			return getter.Get(obj);
		}

		public virtual void SetPropertyValue(object obj, string propertyName, object value) 
		{
			Property.ISetter setter = (Property.ISetter ) settersByPropertyName[propertyName];
			if(setter==null) throw new HibernateException("unmapped property: " + propertyName);
			setter.Set(obj, value);
		}
	
		protected virtual bool HasEmbeddedIdentifier 
		{
			get { return hasEmbeddedIdentifier; }
		}

		public bool[] GetNotNullInsertableColumns(object[] fields) 
		{
			bool[] notNull = new bool[fields.Length];
			bool[] insertable = PropertyInsertability;
			
			for(int i=0; i < fields.Length; i++) 
			{
				notNull[i] = insertable[i] && fields[i]!=null;
			}

			return notNull;
		}

		protected Dialect.Dialect Dialect 
		{
			get { return dialect; }
		}

		protected string GetSQLWhereString(string alias) 
		{
			return StringHelper.Replace(sqlWhereStringTemplate, Template.PlaceHolder, alias);
		}

		protected bool HasWhere 
		{
			get { return (sqlWhereString!=null && sqlWhereString!=String.Empty); }
		}

		public virtual bool HasIdentifierPropertyOrEmbeddedCompositeIdentifier 
		{
			get { return HasIdentifierProperty || hasEmbeddedIdentifier;}
		}

		// IDictionary was a Set in h2.0.3
		protected void CheckColumnDuplication(IDictionary distinctColumns, ICollection columns) 
		{
			foreach(Column col in columns) 
			{
				if( distinctColumns.Contains(col.Name) ) 
				{
					throw new MappingException(
						"Repated column in mapping for class " +
						className + 
						" should be mapped with insert=\"false\" update=\"false\": " +
						col.Name);
				}
			}
		}

		public abstract SqlString QueryWhereFragment(string alias, bool innerJoin, bool includeSublcasses);
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
		public abstract SqlString FromJoinFragment(string alias, bool innerJoin, bool includeSubclasses);
		public abstract SqlString FromTableFragment(string alias);
		public abstract string[] GetPropertyColumnNames(int i);
		public abstract System.Type GetSubclassForDiscriminatorValue(object value);
		public abstract IType GetSubclassPropertyType(int i);
		public abstract bool IsDefinedOnSubclass(int i);
		public abstract SqlString PropertySelectFragment(string alias, string suffix);
		public abstract string TableName { get; }
		public abstract string[] ToColumns(string name, int i);
		public abstract string[] ToColumns(string name, string path) ;
		public abstract SqlString WhereJoinFragment(string alias, bool innerJoin, bool includeSubclasses);
		public abstract string DiscriminatorColumnName { get; }
		public abstract string[] GetSubclassPropertyColumnNames(int i);
		public abstract string GetSubclassPropertyTableName(int i) ;
		public abstract string GetSubclassPropertyName(int i);
	}
}
