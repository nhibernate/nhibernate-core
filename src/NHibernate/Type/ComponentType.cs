using System;
using System.Collections;
using System.Data;
using NHibernate.Engine;
using NHibernate.Loader;
using NHibernate.Property;
using NHibernate.SqlTypes;
using NHibernate.Util;
using NHibernate.Persister;

namespace NHibernate.Type
{
	/// <summary></summary>
	public class ComponentType : AbstractType, IAbstractComponentType
	{
		private readonly System.Type componentClass;
		private readonly IType[ ] propertyTypes;
		private readonly IGetter[ ] getters;
		private readonly ISetter[ ] setters;
		private readonly string[ ] propertyNames;
		private readonly int propertySpan;
		private readonly Cascades.CascadeStyle[ ] cascade;
		private readonly FetchMode[ ] joinedFetch;
		private ISetter parentSetter;
		private IGetter parentGetter;
        private IGetSetHelper getset = null;


		/// <summary>
		/// 
		/// </summary>
		/// <param name="mapping"></param>
		/// <returns></returns>
		public override SqlType[ ] SqlTypes( IMapping mapping )
		{
			//not called at runtime so doesn't matter if its slow :)
			SqlType[ ] sqlTypes = new SqlType[ GetColumnSpan( mapping ) ];
			int n = 0;
			for( int i = 0; i < propertySpan; i++ )
			{
				SqlType[ ] subtypes = propertyTypes[ i ].SqlTypes( mapping );
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
				span += propertyTypes[ i ].GetColumnSpan( mapping );
			}
			return span;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="componentClass"></param>
		/// <param name="propertyNames"></param>
		/// <param name="propertyGetters"></param>
		/// <param name="propertySetters"></param>
		/// <param name="foundCustomAcessor"></param>
		/// <param name="propertyTypes"></param>
		/// <param name="joinedFetch"></param>
		/// <param name="cascade"></param>
		/// <param name="parentProperty"></param>
		public ComponentType( System.Type componentClass,
		                      string[ ] propertyNames,
		                      IGetter[ ] propertyGetters,
		                      ISetter[ ] propertySetters,
		                      // currently not used, see the comment near the end of the method body
							  bool foundCustomAcessor,
		                      IType[ ] propertyTypes,
		                      FetchMode[ ] joinedFetch,
		                      Cascades.CascadeStyle[ ] cascade,
		                      string parentProperty)
		{
			this.componentClass = componentClass;
			this.propertyTypes = propertyTypes;
			propertySpan = propertyNames.Length;
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
			this.propertyNames = propertyNames;
			this.cascade = cascade;
			this.joinedFetch = joinedFetch;

			// NH: reflection optimizer works with custom accessors
			if( /*!foundCustomAcessor &&*/ Cfg.Environment.UseReflectionOptimizer )
			{
				this.getset = GetSetHelperFactory.Create(componentClass, setters, getters);
			}
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
				if( !propertyTypes[ i ].Equals( getters[ i ].Get( x ), getters[ i ].Get( y ) ) )
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
				if( propertyTypes[ i ].IsDirty( getters[ i ].Get( x ), getters[ i ].Get( y ), session ) )
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
			return ResolveIdentifier( Hydrate( rs, names, session, owner ), session, owner );

			/*
			int begin = 0;
			bool notNull = false;
			object[ ] values = new object[propertySpan];
			for( int i = 0; i < propertySpan; i++ )
			{
				int length = propertyTypes[ i ].GetColumnSpan( session.Factory );
				string[ ] range = ArrayHelper.Slice( names, begin, length );
				object val = propertyTypes[ i ].NullSafeGet( rs, range, session, owner );
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
			*/
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
				propertyTypes[ i ].NullSafeSet( st, subvalues[ i ], begin, session );
				begin += propertyTypes[ i ].GetColumnSpan( session.Factory );
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
        /// <remarks>
        /// Use the IGetSetHelper if available
        /// </remarks>
		/// <param name="component"></param>
		/// <returns></returns>
		public object[ ] GetPropertyValues( object component )
		{
            if (getset == null)
            {
                object[] values = new object[propertySpan];
                for (int i = 0; i < propertySpan; i++)
                {
                    values[i] = GetPropertyValue(component, i);
                }
                return values;
            }
            else
            {
                return getset.GetPropertyValues(component);
            }
		}

		/// <summary>
		/// 
		/// </summary>
        /// <remarks>
        /// Use the IGetSetHelper if available
        /// </remarks>
        /// <param name="component"></param>
		/// <param name="values"></param>
		public void SetPropertyValues( object component, object[ ] values )
		{
            if (this.getset == null)
            {
                for (int i = 0; i < propertySpan; i++)
                {
                    setters[i].Set(component, values[i]);
                }
            }
            else
            {
                this.getset.SetPropertyValues(component, values);
            }
		}

		/// <summary></summary>
		public IType[ ] Subtypes
		{
			get { return propertyTypes; }
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
		public override string ToString( object value, ISessionFactoryImplementor factory )
		{
			if( value==null ) return "null";
			IDictionary result = new Hashtable();
			object[ ] values = GetPropertyValues( value );
			
			for( int i = 0; i < propertyTypes.Length; i++ )
			{
				result[ propertyNames[ i ] ] = propertyTypes[ i ].ToString( values[ i ], factory );
			}

			return StringHelper.Unqualify( Name ) + CollectionPrinter.ToString( result );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public override object FromString(string xml)
		{
			throw new NotSupportedException();
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
				values[ i ] = propertyTypes[ i ].DeepCopy( values[ i ] );
			}

			object result = Instantiate();
			SetPropertyValues( result, values );

			// TODO NH: The code below is present in H2.1, but it breaks some
			// tests in NH because FooComponent.Parent setter throws
			// an exception if setting parent to null.
			//
			// not absolutely necessary, but helps for some
			// Equals/GetHashCode implementations
			//
			if( parentGetter != null )
			{
				parentSetter.Set( result, parentGetter.Get( component ) );
			}

			return result;
		}

		public override object Copy( object original, object target, ISessionImplementor session, object owner, IDictionary copiedAlready )
		{
			if( original == null )
			{
				return null;
			}

			if( original == target )
			{
				return target;
			}
		
			object result = target == null
				? Instantiate( owner, session )
				: target;
		
			object[] values = TypeFactory.Copy(
				GetPropertyValues( original ), GetPropertyValues( result ),
				propertyTypes, session, owner, copiedAlready );
		
			SetPropertyValues( result, values );
			return result;
		}

		/// <remarks>
		/// This method does not populate the component parent
		/// </remarks>
		public object Instantiate()
		{
			try
			{
				return Activator.CreateInstance( componentClass, true );
			}
			catch( Exception e )
			{
				throw new InstantiationException( "Could not instantiate component: ", e, componentClass );
			}
		}

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
				for( int i = 0; i < propertyTypes.Length; i++ )
				{
					values[ i ] = propertyTypes[ i ].Disassemble( values[ i ], session );
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
				for( int i = 0; i < propertyTypes.Length; i++ )
				{
					assembled[ i ] = propertyTypes[ i ].Assemble( values[ i ], session, owner );
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
		public FetchMode GetFetchMode( int i )
		{
			return joinedFetch[ i ];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="names"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object Hydrate( IDataReader rs, string[] names, ISessionImplementor session, object owner )
		{
			int begin = 0;
			bool notNull = false;
			object[] values = new object[ propertySpan ];
			for ( int i = 0; i < propertySpan; i++ )
			{
				int length = propertyTypes[ i ].GetColumnSpan( session.Factory );
				string[] range = ArrayHelper.Slice( names, begin, length ); //cache this
				object val = propertyTypes[ i ].Hydrate( rs, range, session, owner );
				if ( val != null )
				{
					notNull = true;
				}
				values[ i ] = val;
				begin += length;
			}

			return notNull ? values : null;
		}

		public override object ResolveIdentifier( object value, ISessionImplementor session, object owner )
		{
			if ( value != null )
			{
				object result = Instantiate( owner, session );
				object[] values = ( object[] ) value;
				for ( int i = 0; i < values.Length; i++ )
				{
					values[ i ] = propertyTypes[ i ].ResolveIdentifier( values[ i ], session, owner );
				}
				SetPropertyValues( result, values );
				return result;
			}
			else
			{
				return null;
			}
		}

		public override bool IsModified( object old, object current, ISessionImplementor session )
		{
			if ( current == null )
			{
				return old != null;
			}
			if ( old == null )
			{
				return current != null;
			}
			object[] currentValues = GetPropertyValues( current, session );
			object[] oldValues = ( object[] ) old;
			for ( int i = 0; i < currentValues.Length; i++ )
			{
				if ( propertyTypes[ i ].IsModified( oldValues[ i ], currentValues[ i ], session ) )
				{
					return true;
				}
			}
			return false;
		}
	}
}