using System;
using System.Collections;
using System.Reflection;
using Iesi.Collections;
using log4net;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Loader;
using NHibernate.Mapping;
using NHibernate.Metadata;
using NHibernate.Property;
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
		private static readonly ILog log = LogManager.GetLogger( typeof( AbstractEntityPersister ) );

		/// <summary></summary>
		protected static readonly System.Type[ ] NoClasses = new System.Type[0];

		private System.Type mappedClass;

		[NonSerialized]
		private Dialect.Dialect dialect;

		[NonSerialized]
		private ConstructorInfo constructor;

		[NonSerialized]
		private IIdentifierGenerator idgen;

		[NonSerialized]
		private bool polymorphic;

		[NonSerialized]
		private bool explicitPolymorphism;

		[NonSerialized]
		private bool inherited;

		[NonSerialized]
		private bool hasSubclasses;

		[NonSerialized]
		private bool versioned;

		[NonSerialized]
		private bool abstractClass;

		[NonSerialized]
		private bool implementsLifecycle;

		[NonSerialized]
		private bool implementsValidatable;

		[NonSerialized]
		private bool hasCollections;

		[NonSerialized]
		private bool hasCascades;

		[NonSerialized]
		private bool mutable;

		[NonSerialized]
		private bool useIdentityColumn;

		[NonSerialized]
		private System.Type superclass;

		[NonSerialized]
		private bool dynamicUpdate;

		[NonSerialized]
		private bool dynamicInsert;

		[NonSerialized]
		private string sqlWhereString;

		[NonSerialized]
		private string sqlWhereStringTemplate;

		[NonSerialized]
		private string identitySelectString;

		[NonSerialized]
		private readonly System.Type[ ] proxyInterfaces;

		[NonSerialized]
		private System.Type concreteProxyClass;

		[NonSerialized]
		private bool hasProxy;

		[NonSerialized]
		private bool hasEmbeddedIdentifier;

		[NonSerialized]
		private string[ ] identifierColumnNames;

		[NonSerialized]
		private Cascades.IdentifierValue unsavedIdentifierValue;

		/// <summary></summary>
		[NonSerialized]
		protected Hashtable columnNamesByPropertyPath = new Hashtable();

		/// <summary></summary>
		[NonSerialized]
		protected Hashtable typesByPropertyPath = new Hashtable();

		[NonSerialized]
		private string identifierPropertyName;

		[NonSerialized]
		private IType identifierType;

		[NonSerialized]
		private ISetter identifierSetter;

		[NonSerialized]
		private IGetter identifierGetter;

		[NonSerialized]
		private PropertyInfo proxyIdentifierProperty;

		[NonSerialized]
		private string[ ] propertyNames;

		[NonSerialized]
		private IType[ ] propertyTypes;

		[NonSerialized]
		private bool[ ] propertyUpdateability;

		[NonSerialized]
		private bool[ ] propertyInsertability;

		[NonSerialized]
		private string versionPropertyName; // not used !?!

		[NonSerialized]
		private string versionColumnName;

		[NonSerialized]
		private IVersionType versionType;

		[NonSerialized]
		private IGetter versionGetter;

		[NonSerialized]
		private int versionProperty;

		[NonSerialized]
		private IGetter[ ] getters;

		[NonSerialized]
		private ISetter[ ] setters;

		[NonSerialized]
		private readonly Hashtable gettersByPropertyName = new Hashtable();

		[NonSerialized]
		private readonly Hashtable settersByPropertyName = new Hashtable();

		/// <summary></summary>
		[NonSerialized]
		protected int hydrateSpan;

		[NonSerialized]
		private string className;

		[NonSerialized]
		private Cascades.CascadeStyle[ ] cascadeStyles;

		[NonSerialized]
		private ICacheConcurrencyStrategy cache;

		// a cglib thing
		//[NonSerialized] private MetaClass optimizer;

		/// <summary></summary>
		public System.Type MappedClass
		{
			get { return mappedClass; }
		}

		/// <summary></summary>
		public string ClassName
		{
			get { return className; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="suffix"></param>
		/// <returns></returns>
		public virtual SqlString IdentifierSelectFragment( string name, string suffix )
		{
			return new SelectFragment( Dialect )
				.SetSuffix( suffix )
				.AddColumns( name, IdentifierColumnNames )
				.ToSqlStringFragment( false );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public virtual IType GetPropertyType( string path )
		{
			return ( IType ) typesByPropertyPath[ path ];
		}

		/// <summary></summary>
		public virtual Cascades.CascadeStyle[ ] PropertyCascadeStyles
		{
			get { return cascadeStyles; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="values"></param>
		public virtual void SetPropertyValues( object obj, object[ ] values )
		{
			//TODO: optimizer implementation
			for( int j = 0; j < hydrateSpan; j++ )
			{
				Setters[ j ].Set( obj, values[ j ] );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public virtual object[ ] GetPropertyValues( object obj )
		{
			//TODO: optimizer implementation
			object[ ] result = new object[hydrateSpan];
			for( int j = 0; j < hydrateSpan; j++ )
			{
				result[ j ] = Getters[ j ].Get( obj );
			}
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		public virtual object GetPropertyValue( object obj, int i )
		{
			return Getters[ i ].Get( obj );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="i"></param>
		/// <param name="value"></param>
		public virtual void SetPropertyValue( object obj, int i, object value )
		{
			Setters[ i ].Set( obj, value );
		}

		/// <summary>
		/// Determine if the given field values are dirty.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="obj"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public virtual int[ ] FindDirty( object[ ] x, object[ ] y, object obj, ISessionImplementor session )
		{
			int[ ] props = TypeFactory.FindDirty( propertyTypes, x, y, propertyUpdateability, session );
			if( props == null )
			{
				return null;
			}
			else
			{
				if( log.IsDebugEnabled )
				{
					for( int i = 0; i < props.Length; i++ )
					{
						log.Debug( className + StringHelper.Dot + propertyNames[ props[ i ] ] + " is dirty" );
					}
				}
				return props;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public virtual object GetIdentifier( object obj )
		{
			object id;
			if( HasEmbeddedIdentifier )
			{
				id = obj;
			}
			else
			{
				if( identifierGetter == null )
				{
					throw new HibernateException( "The class has no identifier property: " + className );
				}
				id = identifierGetter.Get( obj );
			}
			return id;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public virtual object GetVersion( object obj )
		{
			if( !versioned )
			{
				return null;
			}
			return versionGetter.Get( obj );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="id"></param>
		public virtual void SetIdentifier( object obj, object id )
		{
			if( HasEmbeddedIdentifier )
			{
				ComponentType copier = ( ComponentType ) identifierType;
				copier.SetPropertyValues( obj, copier.GetPropertyValues( id ) );
			}
			else if( identifierSetter != null )
			{
				identifierSetter.Set( obj, id );
			}
		}

		/// <summary>
		/// Return a new instance initialized with the given identifier.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public virtual object Instantiate( object id )
		{
			if( HasEmbeddedIdentifier && id.GetType() == mappedClass )
			{
				return id;
			}
			else
			{
				if( abstractClass )
				{
					throw new HibernateException( "Cannot instantiate abstract class or interface: " + className );
				}
				//TODO: optimizer implementation
				try
				{
					return constructor.Invoke( null );
				}
				catch( Exception e )
				{
					throw new InstantiationException( "Could not instantiate entity: ", mappedClass, e );
				}
			}
		}

		/// <summary></summary>
		protected virtual ISetter[ ] Setters
		{
			get { return setters; }
		}

		/// <summary></summary>
		protected virtual IGetter[ ] Getters
		{
			get { return getters; }
		}

		/// <summary></summary>
		public virtual IType[ ] PropertyTypes
		{
			get { return propertyTypes; }
		}

		/// <summary></summary>
		public virtual IType IdentifierType
		{
			get { return identifierType; }
		}

		/// <summary></summary>
		public virtual string[ ] IdentifierColumnNames
		{
			get { return identifierColumnNames; }
		}

		/// <summary></summary>
		public virtual bool IsPolymorphic
		{
			get { return polymorphic; }
		}

		/// <summary></summary>
		public virtual bool IsInherited
		{
			get { return inherited; }
		}

		/// <summary></summary>
		public virtual bool HasCascades
		{
			get { return hasCascades; }
		}

		/// <summary></summary>
		public virtual ICacheConcurrencyStrategy Cache
		{
			get { return cache; }
		}

		/// <summary></summary>
		public virtual bool HasIdentifierProperty
		{
			get { return identifierGetter != null; }
		}

		/// <summary></summary>
		public virtual PropertyInfo ProxyIdentifierProperty
		{
			get { return proxyIdentifierProperty; }
		}

		/// <summary></summary>
		public virtual IVersionType VersionType
		{
			get { return versionType; }
		}

		/// <summary></summary>
		public virtual int VersionProperty
		{
			get { return versionProperty; }
		}

		/// <summary></summary>
		public virtual bool IsVersioned
		{
			get { return versioned; }
		}

		/// <summary></summary>
		public virtual bool IsIdentifierAssignedByInsert
		{
			get { return useIdentityColumn; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public virtual bool IsUnsaved( object id )
		{
			return unsavedIdentifierValue.IsUnsaved( id );
		}

		/// <summary></summary>
		public virtual string[ ] PropertyNames
		{
			get { return propertyNames; }
		}

		/// <summary></summary>
		public virtual string IdentifierPropertyName
		{
			get { return identifierPropertyName; }
		}

		/// <summary></summary>
		public virtual string VersionColumnName
		{
			get { return versionColumnName; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public string[ ] GetPropertyColumnNames( string path )
		{
			return ( string[ ] ) columnNamesByPropertyPath[ path ];
		}

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
			get { return hasCollections; }
		}

		/// <summary></summary>
		public virtual bool IsMutable
		{
			get { return mutable; }
		}

		/// <summary></summary>
		public virtual bool HasCache
		{
			get { return cache != null; }
		}

		/// <summary></summary>
		public virtual bool HasSubclasses
		{
			get { return hasSubclasses; }
		}

		/// <summary></summary>
		public virtual System.Type[ ] ProxyInterfaces
		{
			get { return proxyInterfaces; }
		}

		/// <summary></summary>
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

		/// <summary></summary>
		public virtual IIdentifierGenerator IdentifierGenerator
		{
			get
			{
				if( idgen == null )
				{
					throw new HibernateException( "No ID SchemaExport is configured for class " + className + " (Try using Insert() with an assigned ID)" );
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
		protected virtual void Check( int rows, object id )
		{
			if( rows < 1 )
			{
				throw new StaleObjectStateException( MappedClass, id );
			}
			else if( rows > 1 )
			{
				throw new HibernateException( "Duplicate identifier in table for " + ClassName + ": " + id );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="model"></param>
		/// <param name="factory"></param>
		protected AbstractEntityPersister( PersistentClass model, ISessionFactoryImplementor factory )
		{
			dialect = factory.Dialect;

			// CLASS

			//className = model.PersistentClazz.Name;
			className = model.PersistentClazz.FullName;
			mappedClass = model.PersistentClazz;

			mutable = model.IsMutable;
			dynamicUpdate = model.DynamicUpdate;
			dynamicInsert = model.DynamicInsert;
			sqlWhereString = model.Where;
			sqlWhereStringTemplate = sqlWhereString == null ?
				null :
				Template.RenderWhereStringTemplate( sqlWhereString, Dialect );

			polymorphic = model.IsPolymorphic;
			explicitPolymorphism = model.IsExplicitPolymorphism;
			inherited = model.IsInherited;
			superclass = inherited ? model.Superclass.PersistentClazz : null;
			hasSubclasses = model.HasSubclasses;

			constructor = ReflectHelper.GetDefaultConstructor( mappedClass );
			abstractClass = ReflectHelper.IsAbstractClass( mappedClass );

			// IDENTIFIER

			hasEmbeddedIdentifier = model.HasEmbeddedIdentifier;
			Value idValue = model.Identifier;
			identifierType = idValue.Type;

			if( model.HasIdentifierProperty )
			{
				Mapping.Property idProperty = model.IdentifierProperty;
				identifierPropertyName = idProperty.Name;
				identifierSetter = idProperty.GetSetter( mappedClass );
				identifierGetter = idProperty.GetGetter( mappedClass );
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
			PropertyInfo proxySetIdentifierMethod = null; // not used !?!
			PropertyInfo proxyGetIdentifierMethod = null;

			if( model.HasIdentifierProperty && prox != null )
			{
				Mapping.Property idProperty = model.IdentifierProperty;

				try
				{
					proxyGetIdentifierMethod = idProperty.GetGetter( prox ).Property;
				}
				catch( PropertyNotFoundException )
				{
				}

				try
				{
					proxySetIdentifierMethod = idProperty.GetSetter( prox ).Property;
				}
				catch( PropertyNotFoundException )
				{
				}
			}

			proxyIdentifierProperty = proxyGetIdentifierMethod;

			// HYDRATE SPAN

			hydrateSpan = model.PropertyClosureCollection.Count;

			// IDENTIFIER 

			int idColumnSpan = model.Identifier.ColumnSpan;
			identifierColumnNames = new string[idColumnSpan];

			int i = 0;
			foreach( Column col in idValue.ColumnCollection )
			{
				identifierColumnNames[ i ] = col.GetQuotedName( Dialect );
				i++;
			}

			// GENERATOR
			idgen = model.Identifier.CreateIdentifierGenerator( Dialect );
			useIdentityColumn = idgen is IdentityGenerator;
			identitySelectString = useIdentityColumn ? Dialect.IdentitySelectString : null;

			// UNSAVED-VALUE:

			string unsavedValue = model.Identifier.NullValue;
			if( unsavedValue == null || "null".Equals( unsavedValue ) )
			{
				unsavedIdentifierValue = Cascades.IdentifierValue.SaveNull;
			}
			else if( "none".Equals( unsavedValue ) )
			{
				unsavedIdentifierValue = Cascades.IdentifierValue.SaveNone;
			}
			else if( "any".Equals( unsavedValue ) )
			{
				unsavedIdentifierValue = Cascades.IdentifierValue.SaveAny;
			}
			else
			{
				IType idType = model.Identifier.Type;
				try
				{
					unsavedIdentifierValue = new Cascades.IdentifierValue( ( ( IIdentifierType ) idType ).StringToObject( unsavedValue ) );
				}
				catch( InvalidCastException )
				{
					throw new MappingException( "Bad identifier type: " + idType.GetType().Name );
				}
				catch( Exception )
				{
					throw new MappingException( "Could not parse unsaved-value: " + unsavedValue );
				}
			}

			// VERSION:

			if( model.IsVersioned )
			{
				foreach( Column col in model.Version.ColumnCollection )
				{
					versionColumnName = col.GetQuotedName( Dialect ); //only hapens once
				}
			}
			else
			{
				versionColumnName = null;
			}

			if( model.IsVersioned )
			{
				versionPropertyName = model.Version.Name;
				versioned = true;
				versionGetter = model.Version.GetGetter( mappedClass );
				// TODO: determine if this block is kept
				// this if-else block is extra logic in nhibernate - not sure if I like it - would rather throw
				// an exception if there is a bad mapping
				if( !( model.Version.Type is IVersionType ) )
				{
					log.Warn( model.Name + " has version column " + model.Version.Name + ", but the column type " + model.Version.Type.Name + " is not versionable" );
					versionPropertyName = null;
					versioned = false;
					versionType = null;
					versionGetter = null;
				}
				else
				{
					versionType = ( IVersionType ) model.Version.Type;
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
			getters = new IGetter[hydrateSpan];
			setters = new ISetter[hydrateSpan];
			cascadeStyles = new Cascades.CascadeStyle[hydrateSpan];
			string[ ] setterNames = new string[hydrateSpan];
			string[ ] getterNames = new string[hydrateSpan];
			System.Type[ ] types = new System.Type[hydrateSpan];

			i = 0;
			int tempVersionProperty = -66;
			bool foundCascade = false;

			foreach( Mapping.Property prop in model.PropertyClosureCollection )
			{
				if( prop == model.Version )
				{
					tempVersionProperty = i;
				}
				propertyNames[ i ] = prop.Name;
				getters[ i ] = prop.GetGetter( mappedClass );
				setters[ i ] = prop.GetSetter( mappedClass );
				getterNames[ i ] = getters[ i ].PropertyName;
				setterNames[ i ] = setters[ i ].PropertyName;
				types[ i ] = getters[ i ].ReturnType;
				propertyTypes[ i ] = prop.Type;
				propertyUpdateability[ i ] = prop.IsUpdateable;
				propertyInsertability[ i ] = prop.IsInsertable;

				gettersByPropertyName[ propertyNames[ i ] ] = getters[ i ];
				settersByPropertyName[ propertyNames[ i ] ] = setters[ i ];

				cascadeStyles[ i ] = prop.CascadeStyle;
				if( cascadeStyles[ i ] != Cascades.CascadeStyle.StyleNone )
				{
					foundCascade = true;
				}

				i++;
			}

			//TODO: optimizer implementation

			hasCascades = foundCascade;
			versionProperty = tempVersionProperty;

			// CALLBACK INTERFACES
			implementsLifecycle = typeof( ILifecycle ).IsAssignableFrom( mappedClass );
			implementsValidatable = typeof( IValidatable ).IsAssignableFrom( mappedClass );

			cache = model.Cache;

			hasCollections = InitHasCollections();

			// PROXIES
			System.Type pi = model.ProxyInterface;
			hasProxy = pi != null;
			ArrayList pis = new ArrayList();
			pis.Add( typeof( INHibernateProxy ) );

			// != null because we use arraylist instead of hashset
			// mono does not like a null value passed into Equals()
			if( pi != null && !mappedClass.Equals( pi ) )
			{
				// if the <class> name="type" is not the same type as the proxy="type"
				// then add the proxy's type to the list.  They will
				// be different types when the <class> is a class and the proxy is 
				// an interface, or when a <class> is an interface and the proxy interface
				// is diff (why would you do that??).  They will be the same type
				// when the <class> is an interface and the proxy interface is the same
				// interface.
				pis.Add( pi );
			}
			concreteProxyClass = pi;

			if( hasProxy )
			{
				foreach( Subclass sc in model.SubclassCollection )
				{
					pi = sc.ProxyInterface;
					if( pi == null )
					{
						throw new MappingException( "All subclasses must also have proxies: " + mappedClass.Name );
					}
					if( !sc.PersistentClazz.Equals( pi ) )
					{
						pis.Add( pi );
					}
				}
			}

			proxyInterfaces = ( System.Type[ ] ) pis.ToArray( typeof( System.Type ) );
		}

		/// <summary></summary>
		private bool InitHasCollections()
		{
			return InitHasCollections( propertyTypes );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="types"></param>
		/// <returns></returns>
		private bool InitHasCollections( IType[ ] types )
		{
			for( int i = 0; i < types.Length; i++ )
			{
				if( types[ i ].IsPersistentCollectionType )
				{
					return true;
				}
				else if( types[ i ].IsComponentType )
				{
					if( InitHasCollections( ( ( IAbstractComponentType ) types[ i ] ).Subtypes ) )
					{
						return true;
					}
				}
			}
			return false;
		}

		/// <summary></summary>
		public virtual IClassMetadata ClassMetadata
		{
			get { return this; }
		}

		/// <summary></summary>
		public virtual System.Type ConcreteProxyClass
		{
			get { return concreteProxyClass; }
		}

		/// <summary></summary>
		public virtual System.Type MappedSuperclass
		{
			get { return superclass; }
		}

		/// <summary></summary>
		public virtual bool IsExplicitPolymorphism
		{
			get { return explicitPolymorphism; }
		}

		/// <summary></summary>
		public virtual bool[ ] PropertyUpdateability
		{
			get { return propertyUpdateability; }
		}

		/// <summary></summary>
		protected virtual bool UseDynamicUpdate
		{
			get { return dynamicUpdate; }
		}

		/// <summary></summary>
		protected virtual bool UseDynamicInsert
		{
			get { return dynamicInsert; }
		}

		/// <summary></summary>
		public virtual bool[ ] PropertyInsertability
		{
			get { return propertyInsertability; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public virtual object GetPropertyValue( object obj, string propertyName )
		{
			IGetter getter = ( IGetter ) gettersByPropertyName[ propertyName ];
			if( getter == null )
			{
				throw new HibernateException( "unmapped property: " + propertyName );
			}
			return getter.Get( obj );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		public virtual void SetPropertyValue( object obj, string propertyName, object value )
		{
			ISetter setter = ( ISetter ) settersByPropertyName[ propertyName ];
			if( setter == null )
			{
				throw new HibernateException( "unmapped property: " + propertyName );
			}
			setter.Set( obj, value );
		}

		/// <summary></summary>
		protected virtual bool HasEmbeddedIdentifier
		{
			get { return hasEmbeddedIdentifier; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fields"></param>
		/// <returns></returns>
		public bool[ ] GetNotNullInsertableColumns( object[ ] fields )
		{
			bool[ ] notNull = new bool[fields.Length];
			bool[ ] insertable = PropertyInsertability;

			for( int i = 0; i < fields.Length; i++ )
			{
				notNull[ i ] = insertable[ i ] && fields[ i ] != null;
			}

			return notNull;
		}

		/// <summary></summary>
		protected Dialect.Dialect Dialect
		{
			get { return dialect; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		protected string GetSQLWhereString( string alias )
		{
			return StringHelper.Replace( sqlWhereStringTemplate, Template.PlaceHolder, alias );
		}

		/// <summary></summary>
		protected bool HasWhere
		{
			get { return ( sqlWhereString != null && sqlWhereString.Length > 0 ); }
		}

		/// <summary></summary>
		public virtual bool HasIdentifierPropertyOrEmbeddedCompositeIdentifier
		{
			get { return HasIdentifierProperty || HasEmbeddedIdentifier; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="distinctColumns"></param>
		/// <param name="columns"></param>
		protected void CheckColumnDuplication( ISet distinctColumns, ICollection columns )
		{
			foreach( Column col in columns )
			{
				if( distinctColumns.Contains( col.Name ) )
				{
					throw new MappingException(
						"Repated column in mapping for class " +
							className +
							" should be mapped with insert=\"false\" update=\"false\": " +
							col.Name );
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="innerJoin"></param>
		/// <param name="includeSublcasses"></param>
		/// <returns></returns>
		public abstract SqlString QueryWhereFragment( string alias, bool innerJoin, bool includeSublcasses );
		
		/// <summary></summary>
		public abstract string DiscriminatorSQLString { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="version"></param>
		/// <param name="obj"></param>
		/// <param name="session"></param>
		public abstract void Delete( object id, object version, object obj, ISessionImplementor session );
		
		/// <summary></summary>
		public abstract object[ ] PropertySpaces { get; }
		
		/// <summary></summary>
		public abstract object IdentifierSpace { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="fields"></param>
		/// <param name="obj"></param>
		/// <param name="session"></param>
		public abstract void Insert( object id, object[ ] fields, object obj, ISessionImplementor session );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fields"></param>
		/// <param name="obj"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public abstract object Insert( object[ ] fields, object obj, ISessionImplementor session );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="optionalObject"></param>
		/// <param name="lockMode"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public abstract object Load( object id, object optionalObject, LockMode lockMode, ISessionImplementor session );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="version"></param>
		/// <param name="obj"></param>
		/// <param name="lockMode"></param>
		/// <param name="session"></param>
		public abstract void Lock( object id, object version, object obj, LockMode lockMode, ISessionImplementor session );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		public abstract void PostInstantiate( ISessionFactoryImplementor factory );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="fields"></param>
		/// <param name="dirtyFields"></param>
		/// <param name="oldVersion"></param>
		/// <param name="obj"></param>
		/// <param name="session"></param>
		public abstract void Update( object id, object[ ] fields, int[ ] dirtyFields, object oldVersion, object obj, ISessionImplementor session );
		
		/// <summary></summary>
		public abstract int CountSubclassProperties();
		
		/// <summary></summary>
		public abstract IDiscriminatorType DiscriminatorType { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public abstract OuterJoinLoaderType EnableJoinedFetch( int i );

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
		/// <returns></returns>
		public abstract SqlString FromTableFragment( string alias );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public abstract string[ ] GetPropertyColumnNames( int i );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public abstract System.Type GetSubclassForDiscriminatorValue( object value );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public abstract IType GetSubclassPropertyType( int i );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public abstract bool IsDefinedOnSubclass( int i );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="suffix"></param>
		/// <returns></returns>
		public abstract SqlString PropertySelectFragment( string alias, string suffix );

		/// <summary></summary>
		public abstract string TableName { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		public abstract string[ ] ToColumns( string name, int i );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public abstract string[ ] ToColumns( string name, string path );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="innerJoin"></param>
		/// <param name="includeSubclasses"></param>
		/// <returns></returns>
		public abstract SqlString WhereJoinFragment( string alias, bool innerJoin, bool includeSubclasses );
		
		/// <summary></summary>
		public abstract string DiscriminatorColumnName { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public abstract string[ ] GetSubclassPropertyColumnNames( int i );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public abstract string GetSubclassPropertyTableName( int i );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public abstract string GetSubclassPropertyName( int i );
	}
}