using System;
using System.Data;
using System.Reflection;
using log4net;
using NHibernate.Engine;
using NHibernate.Loader;
using NHibernate.Property;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Type
{
	/// <summary></summary>
	public class ComponentType : AbstractType, IAbstractComponentType
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( ComponentType ) );

		private System.Type componentClass;
		private ConstructorInfo contructor;
		private IType[ ] types;
		private IGetter[ ] getters;
		private ISetter[ ] setters;
		private string[ ] propertyNames;
		private int propertySpan;
		private Cascades.CascadeStyle[ ] cascade;
		private OuterJoinFetchStrategy[ ] joinedFetch;
		private string parentProperty; // not used !?!
		private ISetter parentSetter;
		//TODO: This is new...
		private IGetter parentGetter; // not used !?!

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mapping"></param>
		/// <returns></returns>
		public override SqlType[ ] SqlTypes( IMapping mapping )
		{
			//not called at runtime so doesn't matter if its slow :)
			SqlType[ ] sqlTypes = new SqlType[GetColumnSpan( mapping )];
			int n = 0;
			for( int i = 0; i < propertySpan; i++ )
			{
				SqlType[ ] subtypes = types[ i ].SqlTypes( mapping );
				for( int j = 0; j < subtypes.Length; j++ )
				{
					sqlTypes[ n++ ] = subtypes[ j ];
				}
			}
			return sqlTypes;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mapping"></param>
		/// <returns></returns>
		public override int GetColumnSpan( IMapping mapping )
		{
			int span = 0;
			for( int i = 0; i < propertySpan; i++ )
			{
				span += types[ i ].GetColumnSpan( mapping );
			}
			return span;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="componentClass"></param>
		/// <param name="properties"></param>
		/// <param name="propertyGetters"></param>
		/// <param name="propertySetters"></param>
		/// <param name="foundCustomAcessor"></param>
		/// <param name="types"></param>
		/// <param name="joinedFetch"></param>
		/// <param name="cascade"></param>
		/// <param name="parentProperty"></param>
		/// <param name="embedded"></param>
		public ComponentType( System.Type componentClass,
		                      string[ ] properties,
		                      IGetter[ ] propertyGetters,
		                      ISetter[ ] propertySetters,
		                      bool foundCustomAcessor, // not used !?!
		                      IType[ ] types,
		                      OuterJoinFetchStrategy[ ] joinedFetch,
		                      Cascades.CascadeStyle[ ] cascade,
		                      string parentProperty,
		                      bool embedded ) // not used !?!
		{
			this.componentClass = componentClass;
			this.types = types;
			propertySpan = properties.Length;
			getters = propertyGetters;
			setters = propertySetters;
			string[ ] getterNames = new string[propertySpan];
			string[ ] setterNames = new string[propertySpan];
			System.Type[ ] propTypes = new System.Type[propertySpan];
			for( int i = 0; i < propertySpan; i++ )
			{
				getterNames[ i ] = getters[ i ].PropertyName;
				setterNames[ i ] = setters[ i ].PropertyName;
				propTypes[ i ] = getters[ i ].ReturnType;
			}

			if( parentProperty == null )
			{
				parentSetter = null;
				parentGetter = null;
			}
			else
			{
				IPropertyAccessor pa = PropertyAccessorFactory.GetPropertyAccessor( null );
				parentSetter = pa.GetSetter( componentClass, parentProperty );
				parentGetter = pa.GetGetter( componentClass, parentProperty );
			}
			this.parentProperty = parentProperty;
			this.propertyNames = properties;
			this.cascade = cascade;
			this.joinedFetch = joinedFetch;
			contructor = ReflectHelper.GetDefaultConstructor( componentClass );

		}

		/// <summary></summary>
		public override bool IsPersistentCollectionType
		{
			get { return false; }
		}

		/// <summary></summary>
		public override bool IsComponentType
		{
			get { return true; }
		}

		/// <summary></summary>
		public override bool IsEntityType
		{
			get { return false; }
		}

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return componentClass; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public override bool Equals( object x, object y )
		{
			if( x == y )
			{
				return true;
			}
			if( x == null || y == null )
			{
				return false;
			}
			for( int i = 0; i < propertySpan; i++ )
			{
				if( !types[ i ].Equals( getters[ i ].Get( x ), getters[ i ].Get( y ) ) )
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public override bool IsDirty( object x, object y, ISessionImplementor session )
		{
			if( x == y )
			{
				return false;
			}
			if( x == null || y == null )
			{
				return true;
			}
			for( int i = 0; i < getters.Length; i++ )
			{
				if( types[ i ].IsDirty( getters[ i ].Get( x ), getters[ i ].Get( y ), session ) )
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="names"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object NullSafeGet( IDataReader rs, string[ ] names, ISessionImplementor session, object owner )
		{
			int begin = 0;
			bool notNull = false;
			object[ ] values = new object[propertySpan];
			for( int i = 0; i < propertySpan; i++ )
			{
				int length = types[ i ].GetColumnSpan( session.Factory );
				string[ ] range = ArrayHelper.Slice( names, begin, length );
				object val = types[ i ].NullSafeGet( rs, range, session, owner );
				if( val != null )
				{
					notNull = true;
				}
				values[ i ] = val;
				begin += length;
			}

			if( notNull )
			{
				object result = Instantiate( owner, session );
				for( int i = 0; i < propertySpan; i++ )
				{
					setters[ i ].Set( result, values[ i ] );
				}
				return result;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="st"></param>
		/// <param name="value"></param>
		/// <param name="begin"></param>
		/// <param name="session"></param>
		public override void NullSafeSet( IDbCommand st, object value, int begin, ISessionImplementor session )
		{
			object[ ] subvalues = NullSafeGetValues( value );

			for( int i = 0; i < propertySpan; i++ )
			{
				types[ i ].NullSafeSet( st, subvalues[ i ], begin, session );
				begin += types[ i ].GetColumnSpan( session.Factory );
			}
		}

		private object[ ] NullSafeGetValues( object value )
		{
			if( value == null )
			{
				return new object[propertySpan];
			}
			else
			{
				return GetPropertyValues( value );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="name"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object NullSafeGet( IDataReader rs, string name, ISessionImplementor session, object owner )
		{
			return NullSafeGet( rs, new string[ ] {name}, session, owner );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <param name="i"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public object GetPropertyValue( object component, int i, ISessionImplementor session )
		{
			return GetPropertyValue( component, i );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		public object GetPropertyValue( object component, int i )
		{
			return getters[ i ].Get( component );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public object[ ] GetPropertyValues( object component, ISessionImplementor session )
		{
			return GetPropertyValues( component );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <returns></returns>
		public object[ ] GetPropertyValues( object component )
		{
			object[ ] values = new object[propertySpan];
			for( int i = 0; i < propertySpan; i++ )
			{
				values[ i ] = GetPropertyValue( component, i );
			}
			return values;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <param name="values"></param>
		public void SetPropertyValues( object component, object[ ] values )
		{
			for( int i = 0; i < propertySpan; i++ )
			{
				setters[ i ].Set( component, values[ i ] );
			}
		}

		/// <summary></summary>
		public IType[ ] Subtypes
		{
			get { return types; }
		}

		/// <summary></summary>
		public override string Name
		{
			get { return componentClass.Name; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		public override string ToXML( object value, ISessionFactoryImplementor factory )
		{
			return ( value == null ) ? null : value.ToString();
		}

		/// <summary></summary>
		public string[ ] PropertyNames
		{
			get { return propertyNames; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <returns></returns>
		public override object DeepCopy( object component )
		{
			if( component == null )
			{
				return null;
			}

			object[ ] values = GetPropertyValues( component );
			for( int i = 0; i < propertySpan; i++ )
			{
				values[ i ] = types[ i ].DeepCopy( values[ i ] );
			}

			object result = Instantiate();
			SetPropertyValues( result, values );
			return result;
		}

		/// <summary></summary>
		public object Instantiate()
		{
			try
			{
				return contructor.Invoke( null );
			}
			catch( Exception e )
			{
				throw new InstantiationException( "Could not instantiate component: ", e, componentClass );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public object Instantiate( object parent, ISessionImplementor session )
		{
			object result = Instantiate();
			try
			{
				if( parentSetter != null && parent != null )
				{
					parentSetter.Set( result, session.ProxyFor( parent ) );
				}
				return result;
			}
			catch( Exception e )
			{
				throw new InstantiationException( "Could not set component parent for: ", e, componentClass );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public Cascades.CascadeStyle Cascade( int i )
		{
			return cascade[ i ];
		}

		/// <summary></summary>
		public override bool IsMutable
		{
			get { return true; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public override object Disassemble( object value, ISessionImplementor session )
		{
			if( value == null )
			{
				return null;
			}
			else
			{
				object[ ] values = GetPropertyValues( value );
				for( int i = 0; i < types.Length; i++ )
				{
					values[ i ] = types[ i ].Disassemble( values[ i ], session );
				}
				return values;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object Assemble( object obj, ISessionImplementor session, object owner )
		{
			if( obj == null )
			{
				return null;
			}
			else
			{
				object[ ] values = ( object[ ] ) obj;
				object[ ] assembled = new object[values.Length];
				for( int i = 0; i < types.Length; i++ )
				{
					assembled[ i ] = types[ i ].Assemble( values[ i ], session, owner );
				}
				object result = Instantiate();
				SetPropertyValues( result, assembled );
				return result;
			}
		}

		/// <summary></summary>
		public override bool HasNiceEquals
		{
			get { return false; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public OuterJoinFetchStrategy EnableJoinedFetch( int i )
		{
			return joinedFetch[ i ];
		}


	}
}