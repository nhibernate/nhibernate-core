using System;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Text;
using Iesi.Collections;

using log4net;

using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Hql;
using NHibernate.Id;
using NHibernate.Impl;
using NHibernate.Loader;
using NHibernate.Mapping;
using NHibernate.Metadata;
using NHibernate.Property;
using NHibernate.Proxy;
using NHibernate.SqlCommand;
using NHibernate.Tuple;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Persister.Entity
{
	/// <summary>
	/// Superclass for built-in mapping strategies. Implements functionalty common to both mapping
	/// strategies
	/// </summary>
	/// <remarks>
	/// May be considred an immutable view of the mapping object
	/// </remarks>
	public abstract class AbstractEntityPersister : AbstractPropertyMapping, IOuterJoinLoadable, IQueryable, IClassMetadata, IUniqueKeyLoadable, ISqlLoadable
	{
		private readonly ISessionFactoryImplementor factory;

		private static readonly ILog log = LogManager.GetLogger( typeof( AbstractEntityPersister ) );

		public const string EntityClass = "class";

		private readonly Dialect.Dialect dialect;
		//private readonly SQLExceptionConverter sqlExceptionConverter;

		// The class itself
		private readonly System.Type mappedClass;
		private readonly bool implementsLifecycle;
		private readonly bool implementsValidatable;
		private readonly int batchSize;
		private readonly ConstructorInfo constructor;
		//Not ported:
		//private final BulkBean optimizer;
		//private final FastClass fastClass;

		// The optional SQL string defined in the where attribute
		private readonly string sqlWhereString;
		private readonly string sqlWhereStringTemplate;

		// proxies (if the proxies are interfaces, we use an array of interfaces of all subclasses)
		private readonly System.Type concreteProxyClass;
		private readonly IProxyFactory proxyFactory;

		// the identifier property
		private readonly bool hasEmbeddedIdentifier;

		private readonly string[ ] rootTableKeyColumnNames;
		private readonly string[ ] identifierAliases;
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

		private readonly IGetter[ ] getters;
		private readonly ISetter[ ] setters;

		private readonly Hashtable gettersByPropertyName = new Hashtable();
		private readonly Hashtable settersByPropertyName = new Hashtable();
		//private readonly Hashtable typesByPropertyName = new Hashtable();

		// the cache
		private readonly ICacheConcurrencyStrategy cache;

		private readonly Hashtable uniqueKeyLoaders = new Hashtable();
		//private readonly Hashtable uniqueKeyColumns = new Hashtable();

		private readonly Hashtable subclassPropertyAliases = new Hashtable();
		private readonly Hashtable subclassPropertyColumnNames = new Hashtable();

		private readonly Hashtable lockers = new Hashtable();

        private readonly IGetSetHelper getset = null;

		private readonly EntityMetamodel entityMetamodel;


		//information about properties of this class,
		//including inherited properties
		//(only really needed for updatable/insertable properties)

		// the number of columns that the property spans
		// the array is indexed as propertyColumnSpans[propertyIndex] = ##
		private readonly int[ ] propertyColumnSpans;
		// the names of the columns for the property
		// the array is indexed as propertyColumnNames[propertyIndex][columnIndex] = "columnName"
		private readonly string[ ][ ] propertyColumnNames;
		// the alias names for the columns of the property.  This is used in the AS portion for 
		// selecting a column.  It is indexed the same as propertyColumnNames
		private readonly string[ ][ ] propertyColumnAliases;
		//private readonly string[ ] propertyFormulaTemplates;
		private readonly string[ ][ ] propertyColumnFormulaTemplates;

		private readonly bool[ ][ ] propertyColumnInsertable;
		private readonly bool[ ][ ] propertyColumnUpdateable;
		private readonly bool[ ] propertyUniqueness;

		private readonly bool hasFormulaProperties;

		// the closure of all columns used by the entire hierarchy including
		// subclasses and superclasses of this class
		private readonly string[ ] subclassColumnClosure;
		private readonly string[ ] subclassFormulaTemplateClosure;
		private readonly string[ ] subclassFormulaClosure;
		private readonly string[ ] subclassColumnAliasClosure;
		private readonly string[ ] subclassFormulaAliasClosure;

		// the closure of all properties in the entire hierarchy including
		// subclasses and superclasses of this class
		private readonly string[ ][ ] subclassPropertyColumnNameClosure;
		private readonly string[ ][ ] subclassPropertyFormulaTemplateClosure;
		private readonly string[ ] subclassPropertyNameClosure;
		private readonly IType[ ] subclassPropertyTypeClosure;
		private readonly FetchMode[ ] subclassPropertyFetchModeClosure;

		// temporarily 'protected' instead of 'private readonly'

		protected bool[ ] propertyDefinedOnSubclass;

		protected SqlString GetLockString( LockMode lockMode )
		{
			return ( SqlString ) lockers[ lockMode ];
		}

		public System.Type MappedClass
		{
			get { return mappedClass; }
		}

		public override string ClassName
		{
			get { return entityMetamodel.Type.FullName; }
		}

		public virtual object IdentifierSpace
		{
			get { return entityMetamodel.RootType.AssemblyQualifiedName; }
		}

		public virtual SqlString IdentifierSelectFragment( string name, string suffix )
		{
			return new SelectFragment( dialect )
				.SetSuffix( suffix )
				.AddColumns( name, IdentifierColumnNames, IdentifierAliases )
				.ToSqlStringFragment( false );
		}

		public virtual Cascades.CascadeStyle[ ] PropertyCascadeStyles
		{
			get { return entityMetamodel.CascadeStyles; }
		}

		/// <summary>
		/// Set the given values to the mapped properties of the given object
		/// </summary>
        /// <remarks>
        /// Use the IGetSetHelper if available
        /// </remarks>
		/// <param name="obj"></param>
		/// <param name="values"></param>
		public virtual void SetPropertyValues( object obj, object[ ] values )
		{
            if (getset == null)
            {
                //TODO: optimizer implementation
                for (int j = 0; j < HydrateSpan; j++)
                {
                    Setters[j].Set(obj, values[j]);
                }
            }
            else
            {
                getset.SetPropertyValues(obj, values);
            }
		}

		/// <summary>
		/// Return the values of the mapped properties of the object
		/// </summary>
        /// <remarks>
        /// Use the IGetSetHelper if available
        /// </remarks>
        /// <param name="obj"></param>
		/// <returns></returns>
		public virtual object[ ] GetPropertyValues( object obj )
		{
            if (getset == null)
            {
                int span = HydrateSpan;
                //TODO: optimizer implementation
                object[] result = new object[span];
                for (int j = 0; j < span; j++)
                {
                    result[j] = Getters[j].Get(obj);
                }
                return result;
            }
            else
            {
                return getset.GetPropertyValues(obj);
            }
		}

		/// <summary>
		/// Get the value of the numbered property
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		public virtual object GetPropertyValue( object obj, int i )
		{
			return Getters[ i ].Get( obj );
		}

		/// <summary>
		/// Set the value of the numbered property
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="i"></param>
		/// <param name="value"></param>
		public virtual void SetPropertyValue( object obj, int i, object value )
		{
			Setters[ i ].Set( obj, value );
		}


		private void LogDirtyProperties( int[] props )
		{
			if( log.IsDebugEnabled )
			{
				for( int i = 0; i < props.Length; i++ )
				{
					string propertyName = entityMetamodel.Properties[ props[ i ] ].Name;
					log.Debug( StringHelper.Qualify( ClassName, propertyName ) + " is dirty" );
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
		public virtual int[ ] FindDirty( object[ ] x, object[ ] y, object obj, ISessionImplementor session )
		{
			int[ ] props = TypeFactory.FindDirty(
				entityMetamodel.Properties, x, y, propertyColumnUpdateable, false, session );

			if( props == null )
			{
				return null;
			}
			else
			{
				LogDirtyProperties( props );
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
		public virtual int[ ] FindModified( object[ ] old, object[ ] current, object obj, ISessionImplementor session )
		{
			int[ ] props = TypeFactory.FindModified(
				entityMetamodel.Properties, old, current, propertyColumnUpdateable, false, session );
			if( props == null )
			{
				return null;
			}
			else
			{
				LogDirtyProperties( props );
				return props;
			}
		}

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
					throw new HibernateException( "The class has no identifier property: " + ClassName );
				}
				id = identifierGetter.Get( obj );
			}
			return id;
		}

		public virtual object GetVersion( object obj )
		{
			if( !IsVersioned )
			{
				return null;
			}
			return versionGetter.Get( obj );
		}

		public virtual void SetIdentifier( object obj, object id )
		{
			if( HasEmbeddedIdentifier )
			{
				ComponentType copier = ( ComponentType ) entityMetamodel.IdentifierProperty.Type;
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
				if( mappedClass.IsAbstract )
				{
					throw new HibernateException( "Cannot instantiate abstract class or interface: " + ClassName );
				}

				object result;
				//TODO: optimizer implementation
				try
				{
					result = constructor.Invoke( null );
				}
				catch( Exception e )
				{
					throw new InstantiationException( "Could not instantiate entity: ", e, mappedClass );
				}

				SetIdentifier( result, id );
				return result;
			}
		}

		protected virtual ISetter[ ] Setters
		{
			get { return setters; }
		}

		protected virtual IGetter[ ] Getters
		{
			get { return getters; }
		}

		public virtual IType[ ] PropertyTypes
		{
			get { return entityMetamodel.PropertyTypes; }
		}

		public virtual IType IdentifierType
		{
			get { return entityMetamodel.IdentifierProperty.Type; }
		}

		public override string[ ] IdentifierColumnNames
		{
			get { return rootTableKeyColumnNames; }
		}

		public string[ ] IdentifierAliases
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

		public bool IsUnsaved( object obj )
		{
			object id;
			if( HasIdentifierPropertyOrEmbeddedCompositeIdentifier )
			{
				id = GetIdentifier( obj );
			}
			else
			{
				id = null;
			}
			// we always assume a transient instance with a null
			// identifier or no identifier property is unsaved!
			if( id == null )
			{
				return true;
			}

			if( IsVersioned )
			{
				Cascades.VersionValue unsavedVersionValue = entityMetamodel.VersionProperty.UnsavedValue;
				// let this take precedence if defined, since it works for
				// assigned identifiers
				object result = unsavedVersionValue.IsUnsaved( GetVersion( obj ) );
				if( result != null )
				{
					return ( bool ) result;
				}
			}
			return entityMetamodel.IdentifierProperty.UnsavedValue.IsUnsaved( id );
		}

		/// <summary></summary>
		public virtual string[ ] PropertyNames
		{
			get { return entityMetamodel.PropertyNames; }
		}

		/// <summary></summary>
		public virtual string IdentifierPropertyName
		{
			get { return entityMetamodel.IdentifierProperty.Name; }
		}

		/// <summary></summary>
		public virtual string VersionColumnName
		{
			get { return versionColumnName; }
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
			get { return entityMetamodel.HasCollections; }
		}

		/// <summary></summary>
		public virtual bool IsMutable
		{
			get { return entityMetamodel.IsMutable; }
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
		/// <remarks>This is not a NHibernate Command because there are no parameters.</remarks>
		public string SqlIdentitySelect
		{
			get { return factory.Dialect.IdentitySelectString; }
		}

		public virtual IIdentifierGenerator IdentifierGenerator
		{
			get { return entityMetamodel.IdentifierProperty.IdentifierGenerator; }
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

		private void InitPropertyPaths( IMapping factory )
		{
			for( int i = 0; i < SubclassPropertyNameClosure.Length; i++ )
			{
				InitPropertyPaths(
					SubclassPropertyNameClosure[ i ],
					SubclassPropertyTypeClosure[ i ],
					SubclassPropertyColumnNameClosure[ i ],
					SubclassPropertyFormulaTemplateClosure[ i ],
					factory );
			}

			string idProp = IdentifierPropertyName;
			if( idProp != null )
			{
				InitPropertyPaths( idProp, IdentifierType, IdentifierColumnNames, null, factory );
			}
			if( HasEmbeddedIdentifier )
			{
				InitPropertyPaths( null, IdentifierType, IdentifierColumnNames, null, factory );
			}
			InitPropertyPaths( PathExpressionParser.EntityID, IdentifierType, IdentifierColumnNames, null, factory );

			if( IsPolymorphic )
			{
				AddPropertyPath( PathExpressionParser.EntityClass, DiscriminatorType, new string[ ] { DiscriminatorColumnName },
					null
					// TODO H3: new string[ ] { DiscriminatorFormulaTemplate }
					);
			}
		}

		protected AbstractEntityPersister( PersistentClass persistentClass, ISessionFactoryImplementor factory )
		{
			this.factory = factory;
			dialect = factory.Dialect;
			//sqlExceptionConverter = factory.SQLExceptionConverter;

			this.entityMetamodel = new EntityMetamodel( persistentClass, factory );

			// CLASS
			mappedClass = persistentClass.MappedClass;

			sqlWhereString = persistentClass.Where;
			sqlWhereStringTemplate = sqlWhereString == null ?
				null :
				Template.RenderWhereStringTemplate( sqlWhereString, Dialect );

			batchSize = persistentClass.BatchSize;
			constructor = ReflectHelper.GetDefaultConstructor( mappedClass );

			// verify that the class has a default constructor if it is not abstract - it is considered
			// a mapping exception if the default ctor is missing.
			if( !entityMetamodel.IsAbstract && constructor == null )
			{
				throw new MappingException( "The mapped class " + mappedClass.FullName + " must declare a default (no-arg) constructor." );
			}

			// IDENTIFIER
			hasEmbeddedIdentifier = persistentClass.HasEmbeddedIdentifier;
			IValue idValue = persistentClass.Identifier;

			if( persistentClass.HasIdentifierProperty )
			{
				Mapping.Property idProperty = persistentClass.IdentifierProperty;
				identifierSetter = idProperty.GetSetter( mappedClass );
				identifierGetter = idProperty.GetGetter( mappedClass );
			}
			else
			{
				identifierGetter = null;
				identifierSetter = null;
			}

			System.Type prox = persistentClass.ProxyInterface;
			MethodInfo proxySetIdentifierMethod = null;
			MethodInfo proxyGetIdentifierMethod = null;

			if( persistentClass.HasIdentifierProperty && prox != null )
			{
				Mapping.Property idProperty = persistentClass.IdentifierProperty;

				PropertyInfo getIdPropertyInfo = idProperty.GetGetter( prox ).Property;

				if( getIdPropertyInfo != null )
				{
					proxyGetIdentifierMethod = getIdPropertyInfo.GetGetMethod( true );
				}

				PropertyInfo setIdPropertyInfo = idProperty.GetSetter( prox ).Property;

				if( setIdPropertyInfo != null )
				{
					proxySetIdentifierMethod = setIdPropertyInfo.GetSetMethod( true );
				}
			}

			// HYDRATE SPAN
			hydrateSpan = persistentClass.PropertyClosureCollection.Count;

			// IDENTIFIER 

			identifierColumnSpan = persistentClass.Identifier.ColumnSpan;
			rootTableKeyColumnNames = new string[ identifierColumnSpan ];
			identifierAliases = new string[ identifierColumnSpan ];

			int i = 0;
			foreach( Column col in idValue.ColumnCollection )
			{
				rootTableKeyColumnNames[ i ] = col.GetQuotedName( factory.Dialect );
				identifierAliases[ i ] = col.GetAlias( Dialect );
				i++;
			}

			// VERSION:

			if( persistentClass.IsVersioned )
			{
				foreach( Column col in persistentClass.Version.ColumnCollection )
				{
					versionColumnName = col.GetQuotedName( Dialect );
					break; //only happens once
				}
			}
			else
			{
				versionColumnName = null;
			}

			if( persistentClass.IsVersioned )
			{
				versionGetter = persistentClass.Version.GetGetter( mappedClass );
				versionType = ( IVersionType ) persistentClass.Version.Type;
			}
			else
			{
				versionGetter = null;
				versionType = null;
			}

			// PROPERTIES 

			getters = new IGetter[hydrateSpan];
			setters = new ISetter[hydrateSpan];
			string[ ] setterNames = new string[hydrateSpan];
			string[ ] getterNames = new string[hydrateSpan];
			System.Type[ ] classes = new System.Type[hydrateSpan];

			i = 0;
			
			// NH: reflection optimizer works with custom accessors
			//bool foundCustomAccessor = false;

			foreach( Mapping.Property prop in persistentClass.PropertyClosureCollection )
			{
				//if( !prop.IsBasicPropertyAccessor )
				//{
				//	foundCustomAccessor = true;
				//}

				getters[ i ] = prop.GetGetter( mappedClass );
				setters[ i ] = prop.GetSetter( mappedClass );
				getterNames[ i ] = getters[ i ].PropertyName;
				setterNames[ i ] = setters[ i ].PropertyName;
				classes[ i ] = getters[ i ].ReturnType;
				
				string propertyName = prop.Name;
				gettersByPropertyName[ propertyName ] = getters[ i ];
				settersByPropertyName[ propertyName ] = setters[ i ];

				i++;
			}


			// PROPERTIES (FROM ABSTRACTENTITYPERSISTER SUBCLASSES)
			propertyColumnNames = new string[ HydrateSpan ][ ];
			propertyColumnAliases = new string[ HydrateSpan ][ ];
			propertyColumnSpans = new int[ HydrateSpan ];
			propertyColumnFormulaTemplates = new string[ HydrateSpan ][ ];
			propertyColumnUpdateable = new bool[ HydrateSpan ][];
			propertyColumnInsertable = new bool[ HydrateSpan ][];
			propertyUniqueness = new bool[ HydrateSpan ];

			HashedSet thisClassProperties = new HashedSet();
			i = 0;
			bool foundFormula = false;

			foreach( Mapping.Property prop in persistentClass.PropertyClosureCollection )
			{
				thisClassProperties.Add( prop );

				int span = prop.ColumnSpan;
				propertyColumnSpans[ i ] = span;
				string[] colNames = new string[span];
				string[] colAliases = new string[span];
				string[] templates = new string[span];

				int k = 0;
				foreach( ISelectable thing in prop.ColumnCollection )
				{
					colAliases[ k ] = thing.GetAlias( factory.Dialect , prop.Value.Table );
					if ( thing.IsFormula ) 
					{
						foundFormula = true;
						templates[ k ] = thing.GetTemplate( factory.Dialect );
					}
					else 
					{
						colNames[ k ] = thing.GetTemplate( factory.Dialect );					
					}
					k++;
				}
				propertyColumnNames[ i ] = colNames;
				propertyColumnFormulaTemplates[ i ] = templates;
				propertyColumnAliases[ i ] = colAliases;
				propertyColumnInsertable[ i ] = prop.Value.ColumnInsertability;
				propertyColumnUpdateable[ i ] = prop.Value.ColumnUpdateability;
				propertyUniqueness[ i ] = prop.Value.IsUnique;

				i++;
			}

			hasFormulaProperties = foundFormula;

			// NH: reflection optimizer works with custom accessors
			if( /*!foundCustomAccessor &&*/ Cfg.Environment.UseReflectionOptimizer )
			{
				getset = GetSetHelperFactory.Create( MappedClass, Setters, Getters );
			}

			// SUBCLASS PROPERTY CLOSURE
			ArrayList columns = new ArrayList(); //this.subclassColumnClosure
			ArrayList aliases = new ArrayList();
			ArrayList formulaAliases = new ArrayList();
			ArrayList formulaTemplates = new ArrayList();
			ArrayList types = new ArrayList(); //this.subclassPropertyTypeClosure
			ArrayList names = new ArrayList(); //this.subclassPropertyNameClosure
			ArrayList subclassTemplates = new ArrayList();
			ArrayList propColumns = new ArrayList(); //this.subclassPropertyColumnNameClosure
			ArrayList joinedFetchesList = new ArrayList(); //this.subclassPropertyEnableJoinedFetch
			ArrayList definedBySubclass = new ArrayList(); // this.propertyDefinedOnSubclass
			ArrayList formulas = new ArrayList();

			foreach( Mapping.Property prop in persistentClass.SubclassPropertyClosureCollection )
			{
				names.Add( prop.Name );
				types.Add( prop.Type );
				definedBySubclass.Add( !thisClassProperties.Contains( prop ) );

				string[] cols = new string[prop.ColumnSpan];
				string[] forms = new string[prop.ColumnSpan];
				int[] colnos = new int[prop.ColumnSpan];
				int[] formnos = new int[prop.ColumnSpan];
				int l = 0;

				foreach( ISelectable thing in prop.ColumnCollection )
				{
					if ( thing.IsFormula ) 
					{
						string template = thing.GetTemplate( factory.Dialect );
						formnos[l] = formulaTemplates.Count;
						colnos[l] = -1;
						formulaTemplates.Add( template );
						forms[l] = template;
						formulas.Add( thing.GetText( factory.Dialect ) );
						formulaAliases.Add( thing.GetAlias( factory.Dialect ) );
						// TODO H3: formulasLazy.add( lazy );
					}
					else 
					{
						String colName = thing.GetTemplate( factory.Dialect );
						colnos[l] = columns.Count; //before add :-)
						formnos[l] = -1;
						columns.Add( colName );
						cols[l] = colName;
						aliases.Add( thing.GetAlias( factory.Dialect, prop.Value.Table ) );
						// TODO H3: columnsLazy.add( lazy );
						// TODO H3: columnSelectables.add( new Boolean( prop.isSelectable() ) );
					}
					l++;
				}

				propColumns.Add( cols );
				subclassTemplates.Add( forms );
				//propColumnNumbers.Add( colnos );
				//propFormulaNumbers.Add( formnos );

				joinedFetchesList.Add( prop.Value.FetchMode );
				// TODO H3: cascades.Add( prop.CascadeStyle );
			}

			subclassColumnClosure = ( string[ ] ) columns.ToArray( typeof( string ) );
			subclassFormulaClosure = ( string[ ] ) formulas.ToArray( typeof( string ) );
			subclassFormulaTemplateClosure = ( string[ ] ) formulaTemplates.ToArray( typeof( string ) );
			subclassPropertyTypeClosure = ( IType[ ] ) types.ToArray( typeof( IType ) );
			subclassColumnAliasClosure = ( string[ ] ) aliases.ToArray( typeof( string ) );
			subclassFormulaAliasClosure = ( string[ ] ) formulaAliases.ToArray( typeof( string ) );
			subclassPropertyNameClosure = ( string[ ] ) names.ToArray( typeof( string ) );
			subclassPropertyFormulaTemplateClosure = ArrayHelper.To2DStringArray( subclassTemplates );
			subclassPropertyColumnNameClosure = ( string[ ][ ] ) propColumns.ToArray( typeof( string[ ] ) );

			subclassPropertyFetchModeClosure = new FetchMode[ joinedFetchesList.Count ];
			int m = 0;
			foreach( FetchMode qq in joinedFetchesList )
			{
				subclassPropertyFetchModeClosure[ m++ ] = qq;
			}

			propertyDefinedOnSubclass = new bool[ definedBySubclass.Count ];
			m = 0;
			foreach( bool val in definedBySubclass )
			{
				propertyDefinedOnSubclass[ m++ ] = val;
			}

			// CALLBACK INTERFACES
			implementsLifecycle = typeof( ILifecycle ).IsAssignableFrom( mappedClass );
			implementsValidatable = typeof( IValidatable ).IsAssignableFrom( mappedClass );

			cache = persistentClass.Cache;

			// PROXIES
			concreteProxyClass = persistentClass.ProxyInterface;
			bool hasProxy = concreteProxyClass != null;

			if( hasProxy )
			{
				HashedSet proxyInterfaces = new HashedSet();
				proxyInterfaces.Add( typeof( INHibernateProxy ) );

				if( !mappedClass.Equals( concreteProxyClass ) )
				{
					if( !concreteProxyClass.IsInterface )
					{
						throw new MappingException(
							"proxy must be either an interface, or the class itself: " +
								mappedClass.FullName );
					}

					proxyInterfaces.Add( concreteProxyClass );
				}

				if( mappedClass.IsInterface )
				{
					proxyInterfaces.Add( mappedClass );
				}

				if( HasProxy )
				{
					foreach( Subclass subclass in persistentClass.SubclassCollection )
					{
						System.Type subclassProxy = subclass.ProxyInterface;
						if( subclassProxy == null )
						{
							throw new MappingException( "All subclasses must also have proxies: "
								+ mappedClass.Name );
						}

						if( !subclass.MappedClass.Equals( subclassProxy ) )
						{
							proxyInterfaces.Add( subclassProxy );
						}
					}
				}

				if( HasProxy )
				{
					proxyFactory = CreateProxyFactory();
					proxyFactory.PostInstantiate( mappedClass, proxyInterfaces, proxyGetIdentifierMethod, proxySetIdentifierMethod );
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
		}

		protected virtual IProxyFactory CreateProxyFactory()
		{
			return new CastleProxyFactory();
		}

		/// <summary>
		/// Must be called by subclasses, at the end of their constructors
		/// </summary>
		/// <param name="model"></param>
		protected void InitSubclassPropertyAliasesMap( PersistentClass model )
		{
			// ALIASES
			InternalInitSubclassPropertyAliasesMap( null, model.SubclassPropertyClosureCollection );

			// aliases for identifier
			if( HasIdentifierProperty )
			{
				subclassPropertyAliases[ IdentifierPropertyName ] = identifierAliases;
				subclassPropertyAliases[ PathExpressionParser.EntityID ] = identifierAliases;
			}

			if( HasEmbeddedIdentifier )
			{
				// Fetch embedded identifier property names from the "virtual" identifier component
				IAbstractComponentType componentId = ( IAbstractComponentType ) IdentifierType;
				string[ ] idPropertyNames = componentId.PropertyNames;
				string[ ] idAliases = IdentifierAliases;

				for( int i = 0; i < idPropertyNames.Length; i++ )
				{
					subclassPropertyAliases[ idPropertyNames[ i ] ] = new string[ ] {idAliases[ i ]};
				}
			}

			if( IsPolymorphic )
			{
				subclassPropertyAliases[ PathExpressionParser.EntityClass ] = new string[ ] {DiscriminatorAlias};
			}
		}

		private void InternalInitSubclassPropertyAliasesMap( string path, ICollection col )
		{
			foreach( Mapping.Property prop in col )
			{
				string propName = path == null ? prop.Name : path + "." + prop.Name;
				if( prop.IsComposite )
				{
					Component component = ( Component ) prop.Value;
					InternalInitSubclassPropertyAliasesMap( propName, component.PropertyCollection );
				}
				else
				{
					string[ ] aliases = new string[prop.ColumnSpan];
					string[ ] cols = new string[prop.ColumnSpan];
					int l = 0;
					foreach( ISelectable thing in prop.ColumnCollection )
					{
						aliases[ l ] = thing.GetAlias( dialect, prop.Value.Table );
						cols[ l ] = thing.GetText( dialect );
						l++;
					}

					subclassPropertyAliases[ propName ] = aliases;
					subclassPropertyColumnNames[ propName ] = cols;
				}
			}
		}

		protected void InitLockers()
		{
			SqlString lockString = GenerateLockString( null, null );
			SqlString lockExclusiveString = Dialect.SupportsForUpdate ?
				GenerateLockString( lockString, " for update" ) :
				GenerateLockString( lockString, null );
			SqlString lockExclusiveNowaitString = Dialect.SupportsForUpdateNoWait ?
				GenerateLockString( lockString, " for update nowait" ) :
				GenerateLockString( lockExclusiveString, null );

			lockers.Add( LockMode.Read, lockString );
			lockers.Add( LockMode.Upgrade, lockExclusiveString );
			lockers.Add( LockMode.UpgradeNoWait, lockExclusiveNowaitString );
		}

		protected abstract SqlString GenerateLockString( SqlString sqlString, string forUpdateFragment );

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

		public virtual bool[ ] PropertyUpdateability
		{
			get { return entityMetamodel.PropertyUpdateability; }
		}

		public virtual bool[ ] PropertyCheckability
		{
			get { return entityMetamodel.PropertyCheckability; }
		}

		public virtual bool[ ] PropertyNullability
		{
			get { return entityMetamodel.PropertyNullability; }
		}

		protected virtual bool UseDynamicUpdate
		{
			get { return entityMetamodel.IsDynamicUpdate; }
		}

		protected virtual bool UseDynamicInsert
		{
			get { return entityMetamodel.IsDynamicInsert; }
		}

		public virtual bool[ ] PropertyInsertability
		{
			get { return entityMetamodel.PropertyInsertability; }
		}

		public virtual object GetPropertyValue( object obj, string propertyName )
		{
			IGetter getter = ( IGetter ) gettersByPropertyName[ propertyName ];
			if( getter == null )
			{
				throw new HibernateException( "unmapped property: " + propertyName );
			}
			return getter.Get( obj );
		}

		public virtual void SetPropertyValue( object obj, string propertyName, object value )
		{
			ISetter setter = ( ISetter ) settersByPropertyName[ propertyName ];
			if( setter == null )
			{
				throw new HibernateException( "unmapped property: " + propertyName );
			}
			setter.Set( obj, value );
		}

		protected virtual bool HasEmbeddedIdentifier
		{
			get { return hasEmbeddedIdentifier; }
		}

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

		protected string GetSQLWhereString( string alias )
		{
			return StringHelper.Replace( sqlWhereStringTemplate, Template.Placeholder, alias );
		}

		protected bool HasWhere
		{
			get { return ( sqlWhereString != null && sqlWhereString.Length > 0 ); }
		}

		public virtual bool HasIdentifierPropertyOrEmbeddedCompositeIdentifier
		{
			get { return HasIdentifierProperty || HasEmbeddedIdentifier; }
		}

		protected void CheckColumnDuplication( ISet distinctColumns, ICollection columns )
		{
			foreach( Column col in columns )
			{
				if( !distinctColumns.Add( col.Name ) )
				{
					throw new MappingException(
						"Repeated column in mapping for class " +
							ClassName +
							" should be mapped with insert=\"false\" update=\"false\": " +
							col.Name );
				}
			}
		}

		protected IUniqueEntityLoader CreateEntityLoader( ISessionFactoryImplementor factory )
		{
			Loader.Loader nonBatchLoader = new EntityLoader( this, 1, factory );
			if( batchSize > 1 )
			{
				Loader.Loader batchLoader = new EntityLoader( this, batchSize, factory );
				int smallBatchSize = ( int ) Math.Round( Math.Sqrt( batchSize ) );
				Loader.Loader smallBatchLoader = new EntityLoader( this, smallBatchSize, factory );
				return new BatchingEntityLoader( this, batchSize, batchLoader, smallBatchSize, smallBatchLoader, nonBatchLoader );
			}
			else
			{
				return ( IUniqueEntityLoader ) nonBatchLoader;
			}
		}

		protected void CreateUniqueKeyLoaders( ISessionFactoryImplementor factory )
		{
			IType[] propertyTypes = PropertyTypes;
			string[] propertyNames = PropertyNames;

			// TODO: Does not handle components, or properties of a joined subclass
			for( int i = 0; i < entityMetamodel.PropertySpan; i++ )
			{
				if( propertyUniqueness[ i ] )
				{
					uniqueKeyLoaders[ propertyNames[ i ] ] =
						CreateUniqueKeyLoader( 
								propertyTypes[ i ],
								GetPropertyColumnNames( i ) // TODO H3: , new Hashtable()
							);
				}
			}
		}

		private EntityLoader CreateUniqueKeyLoader( IType uniqueKeyType, string[ ] columns ) // TODO H3: add enabledFilters
		{
			if( uniqueKeyType.IsEntityType )
			{
				System.Type type = ( ( EntityType ) uniqueKeyType ).AssociatedClass;
				uniqueKeyType = Factory.GetPersister( type ).IdentifierType;
			}

			return new EntityLoader( this, columns, uniqueKeyType, 1, Factory );
		}

		public override IType Type
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

		public string[ ] GetSubclassPropertyColumnAliases( string propertyName, string suffix )
		{
			string[ ] rawAliases = ( string[ ] ) subclassPropertyAliases[ propertyName ];

			if( rawAliases == null )
			{
				return null;
			}

			string[ ] result = new string[rawAliases.Length];
			for( int i = 0; i < rawAliases.Length; i++ )
			{
				result[ i ] = new Alias( suffix ).ToUnquotedAliasString( rawAliases[ i ], Dialect );
			}
			return result;
		}

		public string[ ] KeyColumnNames
		{
			get { return IdentifierColumnNames; }
		}

		public string Name
		{
			get { return ClassName; }
		}

		public SqlString SelectFragment( string alias, string suffix )
		{
			return IdentifierSelectFragment( alias, suffix ).Append( PropertySelectFragment( alias, suffix ) );
		}

		public string[ ] GetIdentifierAliases( string suffix )
		{
			// NOTE: this assumes something about how PropertySelectFragment is implemented by the subclass!
			// was toUnqotedAliasStrings( getIdentiferColumnNames() ) before - now tried
			// to remove that unquoting and missing aliases..
			return new Alias( suffix ).ToAliasStrings( IdentifierAliases, dialect );
		}

		public string[ ] GetPropertyAliases( string suffix, int i )
		{
			// NOTE: this assumes something about how pPropertySelectFragment is implemented by the subclass!
			return new Alias( suffix ).ToUnquotedAliasStrings( propertyColumnAliases[ i ], dialect );
		}

		public string GetDiscriminatorAlias( string suffix )
		{
			// NOTE: this assumes something about how PropertySelectFragment is implemented by the subclass!
			// was toUnqotedAliasStrings( getdiscriminatorColumnName() ) before - now tried
			// to remove that unquoting and missing aliases..		
			return HasSubclasses ?
				new Alias( suffix ).ToAliasString( DiscriminatorAlias, dialect ) :
				null;
		}

		protected abstract string DiscriminatorAlias { get; }

		public object LoadByUniqueKey( string propertyName, object uniqueKey, ISessionImplementor session )
		{
			return ( ( EntityLoader ) uniqueKeyLoaders[ propertyName ] ).LoadByUniqueKey( session, uniqueKey );
		}

		public bool IsCollection
		{
			get { return false; }
		}

		public bool ConsumesAlias()
		{
			return true;
		}

		public virtual IType GetPropertyType( string path )
		{
			return ToType( path );
		}

		protected bool HasSelectBeforeUpdate
		{
			get { return entityMetamodel.IsSelectBeforeUpdate; }
		}

		protected abstract SqlString VersionSelectString { get; }

		/// <summary>
		/// Retrieve the version number
		/// </summary>
		/// <param name="id"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public object GetCurrentVersion( object id, ISessionImplementor session )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Getting version: " + MessageHelper.InfoString( this, id ) );
			}

			try
			{
				IDbCommand st = session.Batcher.PrepareQueryCommand( VersionSelectString, false );
				IDataReader rs = null;
				try
				{
					IdentifierType.NullSafeSet( st, id, 0, session );
					rs = session.Batcher.ExecuteReader( st );
					if( !rs.Read() )
					{
						return null;
					}
					if( !IsVersioned )
					{
						return this;
					}
					return VersionType.NullSafeGet( rs, VersionColumnName, session, null );
				}
				finally
				{
					session.Batcher.CloseQueryCommand( st, rs );
				}
			}
			catch( HibernateException )
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch( Exception sqle )
			{
				throw Convert( sqle, "could not retrieve version: " + MessageHelper.InfoString( this, id ) );
			}
		}

		/// <summary>
		/// Do a version check
		/// </summary>
		/// <param name="id"></param>
		/// <param name="version"></param>
		/// <param name="obj"></param>
		/// <param name="lockMode"></param>
		/// <param name="session"></param>
		public virtual void Lock( object id, object version, object obj, LockMode lockMode, ISessionImplementor session )
		{
			if( lockMode != LockMode.None )
			{
				if( log.IsDebugEnabled )
				{
					log.Debug( "Locking entity: " + MessageHelper.InfoString( this, id ) );
					if( IsVersioned )
					{
						log.Debug( "Version: " + version );
					}
				}

				try
				{
					IDbCommand st = session.Batcher.PrepareCommand( GetLockString( lockMode ) );
					IDataReader rs = null;

					try
					{
						IdentifierType.NullSafeSet( st, id, 0, session );
						if( IsVersioned )
						{
							VersionType.NullSafeSet( st, version, IdentifierColumnNames.Length, session );
						}

						rs = session.Batcher.ExecuteReader( st );
						if( !rs.Read() )
						{
							throw new StaleObjectStateException( MappedClass, id );
						}
					}
					finally
					{
						session.Batcher.CloseCommand( st, rs );
					}
				}
				catch( HibernateException )
				{
					// Do not call Convert on HibernateExceptions
					throw;
				}
				catch( Exception sqle )
				{
					throw Convert( sqle, "could not lock: " + MessageHelper.InfoString( this, id ) );
				}
			}
		}

		protected object GetGeneratedIdentity( object obj, ISessionImplementor session, IDataReader rs )
		{
			object id;

			try
			{
				if( !rs.Read() )
				{
					throw new HibernateException( "The database returned no natively generated identity value" );
				}
				id = IdentifierGeneratorFactory.Get( rs, IdentifierType.ReturnedClass );
			}
			finally
			{
				rs.Close();
			}

			if( log.IsDebugEnabled )
			{
				log.Debug( "Natively generated identity: " + id );
			}

			return id;
		}

		public object[ ] GetCurrentPersistentState( object id, object version, ISessionImplementor session )
		{
			if( !HasSelectBeforeUpdate )
			{
				return null;
			}

			if( log.IsDebugEnabled )
			{
				log.Debug( "Getting current persistent state for: " + MessageHelper.InfoString( this, id ) );
			}

			IType[ ] types = PropertyTypes;
			object[ ] values = new object[types.Length];
			bool[ ] includeProperty = PropertyUpdateability;
			try
			{
				IDbCommand st = session.Batcher.PrepareCommand( ConcreteSelectString );
				IDataReader rs = null;
				try
				{
					IdentifierType.NullSafeSet( st, id, 0, session );
					if( IsVersioned )
					{
						VersionType.NullSafeSet( st, version, IdentifierColumnNames.Length, session );
					}
					rs = session.Batcher.ExecuteReader( st );
					if( !rs.Read() )
					{
						throw new StaleObjectStateException( MappedClass, id );
					}
					for( int i = 0; i < types.Length; i++ )
					{
						if( includeProperty[ i ] )
						{
							values[ i ] = types[ i ].Hydrate( rs, GetPropertyAliases( string.Empty, i ), session, null ); //null owner ok??
						}
					}
				}
				finally
				{
					session.Batcher.CloseCommand( st, rs );
				}
			}
			catch( HibernateException )
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch( Exception sqle )
			{
				throw Convert( sqle, "error retrieving current persistent state" );
			}

			return values;
		}

		protected abstract string VersionedTableName { get; }

		/// <summary>
		/// Generate the SQL that selects the version number by id
		/// </summary>
		/// <returns></returns>
		protected SqlString GenerateSelectVersionString( ISessionFactoryImplementor factory )
		{
			SqlSimpleSelectBuilder builder = new SqlSimpleSelectBuilder( factory );
			builder.SetTableName( VersionedTableName );

			if( IsVersioned )
			{
				builder.AddColumn( VersionColumnName );
			}
			else
			{
				builder.AddColumns( IdentifierColumnNames );
			}

			builder.AddWhereFragment( IdentifierColumnNames, IdentifierType, " = " );

			return builder.ToSqlString();
		}

		protected abstract SqlString ConcreteSelectString { get; }

		protected OptimisticLockMode OptimisticLockMode
		{
			get { return entityMetamodel.OptimisticLockMode; }
		}

		public bool IsManyToMany
		{
			get { return false; }
		}

		public object CreateProxy( object id, ISessionImplementor session )
		{
			return proxyFactory.GetProxy( id, session );
		}

		/// <summary>
		/// Transform the array of property indexes to an array of booleans
		/// </summary>
		/// <param name="dirtyProperties"></param>
		/// <returns></returns>
		protected bool[ ] GetPropertiesToUpdate( int[ ] dirtyProperties )
		{
			bool[ ] propsToUpdate = new bool[HydrateSpan];
			for( int j = 0; j < dirtyProperties.Length; j++ )
			{
				propsToUpdate[ dirtyProperties[ j ] ] = true;
			}
			if( IsVersioned )
			{
				propsToUpdate[ VersionProperty ] = true;
			}

			return propsToUpdate;
		}

		public override string ToString()
		{
			return StringHelper.Root( GetType().FullName ) + '(' + ClassName + ')';
		}

		/// <summary>
		/// Get the column names for the numbered property of <em>this</em> class
		/// </summary>
		public string[ ] GetPropertyColumnNames( int i )
		{
			return propertyColumnNames[ i ];
		}

		protected int GetPropertyColumnSpan( int i )
		{
			return propertyColumnSpans[ i ];
		}

		public SqlString SelectFragment( string alias, string suffix, bool includeCollectionColumns )
		{
			return SelectFragment( alias, suffix );
		}

		public SqlString SelectFragment(
			IJoinable rhs,
			string rhsAlias,
			string lhsAlias,
			string entitySuffix,
			string collectionSuffix,
			bool includeCollectionColumns)
		{
			return SelectFragment( lhsAlias, entitySuffix );
		}

		protected ADOException Convert( Exception sqlException, string message )
		{
			return ADOExceptionHelper.Convert( /* sqlExceptionConverter, */ sqlException, message );
		}


		public abstract SqlString QueryWhereFragment( string alias, bool innerJoin, bool includeSubclasses );

		public abstract object DiscriminatorSQLValue { get; }

		public abstract void Delete( object id, object version, object obj, ISessionImplementor session );

		public abstract object[ ] PropertySpaces { get; }

		public abstract void Insert( object id, object[ ] fields, object obj, ISessionImplementor session );

		public abstract object Insert( object[ ] fields, object obj, ISessionImplementor session );

		public abstract object Load( object id, object optionalObject, LockMode lockMode, ISessionImplementor session );

		public abstract void PostInstantiate( ISessionFactoryImplementor factory );

		public abstract void Update( object id, object[ ] fields, int[ ] dirtyFields, object[ ] oldFields, object oldVersion, object obj, ISessionImplementor session );

		public abstract IType DiscriminatorType { get; }

		public abstract SqlString FromJoinFragment( string alias, bool innerJoin, bool includeSubclasses );

		public abstract SqlString FromTableFragment( string alias );

		public abstract System.Type GetSubclassForDiscriminatorValue( object value );

		public bool IsDefinedOnSubclass( int i )
		{
			return propertyDefinedOnSubclass[ i ];
		}

		public SqlString PropertySelectFragment( string name, string suffix )
		{
			SelectFragment select = new SelectFragment( Factory.Dialect )
				.SetSuffix( suffix )
				.SetUsedAliases( IdentifierAliases );

			int[] columnTableNumbers = SubclassColumnTableNumberClosure;
			string[] columnAliases = SubclassColumnAliasClosure;
			string[] columns = SubclassColumnClosure;

			for( int i = 0; i < columns.Length; i++ )
			{
				string subalias = Alias( name, columnTableNumbers[ i ] );
				select.AddColumn( subalias, columns[ i ], columnAliases[ i ]	);
			}

			int[] formulaTableNumbers = SubclassFormulaTableNumberClosure;
			string[] formulaTemplates = SubclassFormulaTemplateClosure;
			string[] formulaAliases = SubclassFormulaAliasClosure;

			for( int i = 0; i < formulaTemplates.Length; i++ )
			{
				string subalias = Alias( name, formulaTableNumbers[ i ] );
				select.AddFormula( subalias, formulaTemplates[ i ], formulaAliases[ i ] );
			}

			if( HasSubclasses )
			{
				AddDiscriminatorToSelect( select, name, suffix );
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

		public abstract string TableName { get; }

		public string[ ] ToColumns( string name, int i )
		{
			string alias = Alias( name, GetSubclassPropertyTableNumber( i ) );
			string[] cols = GetSubclassPropertyColumnNames( i );
			string[] templates = SubclassPropertyFormulaTemplateClosure[ i ];
			string[] result = new string[ cols.Length ];

			for( int j = 0; j < cols.Length; j++ )
			{
				if( cols[ j ] == null ) 
				{
					result[ j ] = StringHelper.Replace( templates[ j ], Template.Placeholder, alias );
				}
				else 
				{
					result[ j ] = StringHelper.Qualify( alias, cols[ j ] );
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
			IType type = ToType( rootPropertyName );
			if ( type.IsAssociationType && ( (IAssociationType) type ).UseLHSPrimaryKey ) 
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
			int index = System.Array.IndexOf( SubclassPropertyNameClosure, rootPropertyName ); //TODO: optimize this better!
			return index == -1 ? 0 : GetSubclassPropertyTableNumber( index );
		}

		public override string[] ToColumns(string alias, string propertyName)
		{
			return base.ToColumns( Alias( alias, GetSubclassPropertyTableNumber( propertyName ) ), propertyName );
		}

		public abstract SqlString WhereJoinFragment( string alias, bool innerJoin, bool includeSubclasses );

		public abstract string DiscriminatorColumnName { get; }

		public abstract string GetSubclassPropertyTableName( int i );

		public abstract bool IsCacheInvalidationRequired { get; }

		protected abstract int GetSubclassPropertyTableNumber( int i );

		public bool IsUnsavedVersion( object[ ] values )
		{
			if( !IsVersioned )
			{
				return false;
			}

			object result = entityMetamodel.VersionProperty.UnsavedValue.IsUnsaved( values[ VersionProperty ] );
			if( result != null )
			{
				return ( bool ) result;
			}

			return true;
		}

		protected bool HasFormulaProperties
		{
			get { return hasFormulaProperties; }
		}

		protected string[] GetPropertyColumnAliases( int i )
		{
			return propertyColumnAliases[ i ];
		}

		public FetchMode GetFetchMode( int i )
		{
			return subclassPropertyFetchModeClosure[ i ];
		}

		public IType GetSubclassPropertyType( int i )
		{
			return subclassPropertyTypeClosure[ i ];
		}

		public string GetSubclassPropertyName( int i )
		{
			return this.subclassPropertyNameClosure[ i ];
		}

		public int CountSubclassProperties()
		{
			return subclassPropertyTypeClosure.Length;
		}

		public string[ ] GetSubclassPropertyColumnNames( int i )
		{
			return subclassPropertyColumnNameClosure[ i ];
		}

		public ISessionFactoryImplementor Factory
		{
			get { return factory; }
		}

		protected string Alias( string rootAlias, int tableNumber )
		{
			if( tableNumber == 0 )
			{
				return rootAlias;
			}

			StringBuilder buf = new StringBuilder( rootAlias );

			if( !rootAlias.EndsWith( "_" ) )
			{
				buf.Append( '_' );
			}

			return buf.Append( tableNumber ).Append( '_' ).ToString();

			// TODO: this was the former NH implementation, I wonder
			// if quoting/unquoting stuff matters at all.
			//return Dialect.QuoteForAliasName(
			//	Dialect.UnQuote( rootAlias )
			//	+ StringHelper.Underscore
			//	+ tableNumber
			//	+ StringHelper.Underscore );
		}

		protected virtual void AddDiscriminatorToSelect( SelectFragment select, string name, string suffix )
		{
		}

		public string[] GetPropertyColumnNames( string propertyName )
		{
			return GetColumnNames( propertyName );
		}

		public int GetPropertyIndex( string propertyName )
		{
			return entityMetamodel.GetPropertyIndex( propertyName );
		}

		public abstract string GetPropertyTableName( string propertyName );

		protected EntityMetamodel EntityMetamodel
		{
			get { return entityMetamodel; }
		}

		protected void PostConstruct( IMapping mapping )
		{
			InitPropertyPaths( mapping );
		}
	}
}