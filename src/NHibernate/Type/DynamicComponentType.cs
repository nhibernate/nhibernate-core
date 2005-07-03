using System.Collections;
using NHibernate;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Loader;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Type
{
	/// <summary>
	/// Handles "dynamic" components, represented as <tt>Map</tt>s
	/// </summary>
	public class DynamicComponentType : AbstractType, IAbstractComponentType
	{
		private string[] propertyNames;
		private IType[] propertyTypes;
		private int propertySpan;
		private readonly Cascades.CascadeStyle[] cascade;
		private readonly OuterJoinFetchStrategy[] joinedFetch;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyNames"></param>
		/// <param name="propertyTypes"></param>
		/// <param name="joinedFetch"></param>
		/// <param name="cascade"></param>
		public DynamicComponentType(
			string[] propertyNames,
			IType[] propertyTypes,
			OuterJoinFetchStrategy[] joinedFetch,
			Cascades.CascadeStyle[] cascade
			)
		{
			this.propertyNames = propertyNames;
			this.propertyTypes = propertyTypes;
			this.cascade = cascade;
			this.joinedFetch = joinedFetch;
			propertySpan = propertyTypes.Length;
		}

		public override object Copy( object original, object target, ISessionImplementor session, object owner, IDictionary copiedAlready )
		{
			if( original == null ) return null;
			if( original == target ) return target;
		
			object[] values = TypeFactory.Copy(
				GetPropertyValues( original ),
				GetPropertyValues( target ),
				propertyTypes, session, owner, copiedAlready );
		
			object result = target == null ? Instantiate() : target;
			SetPropertyValues( result, values );
			return result;
		}

		#region IAbstractComponentType Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public Cascades.CascadeStyle Cascade( int i )
		{
			return cascade[ i ];
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

		/// <summary>
		/// 
		/// </summary>
		public string[] PropertyNames
		{
			get
			{
				return propertyNames;
			}
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
		/// <param name="session"></param>
		/// <returns></returns>
		public object[] GetPropertyValues( object component, ISessionImplementor session )
		{
			return GetPropertyValues( component );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		public object GetPropertyValue( object component, int i )
		{
			return ( (Map) component)[ propertyNames[ i ] ];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <returns></returns>
		public object[] GetPropertyValues( object component )
		{
			Map bean = (Map) component;
			object[] result = new object[ propertySpan ];
			for ( int i = 0; i < propertySpan; i++ )
			{
				result[ i ] = bean[ propertyNames[ i ] ];
			}
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		public IType[] Subtypes
		{
			get	{ return propertyTypes;	} 
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public object Instantiate()
		{
			return new Map();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <param name="values"></param>
		public void SetPropertyValues( object component, object[] values )
		{
			Map map = component as Map;
			for ( int i = 0; i < propertySpan; i++ )
			{
				map[ propertyNames[ i ] ] = values[ i ];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <returns></returns>
		public override object DeepCopy( object component )
		{
			if ( component == null )
			{
				return null;
			}

			object[] values = GetPropertyValues( component );
			for ( int i = 0; i < propertySpan; i++ )
			{
				values[ i ] = propertyTypes[ i ].DeepCopy( values[ i ] );
			}
			object result = Instantiate();
			SetPropertyValues( result, values );

			return result;
		}

		#endregion

		#region IType Members

		/// <summary></summary>
		public override bool IsAssociationType
		{
			// TODO:  Add DynamicComponentType.IsAssociationType getter implementation
			get	{ return false;	}
		}

		/// <summary></summary>
		public override bool IsPersistentCollectionType
		{
			// TODO:  Add DynamicComponentType.IsPersistentCollectionType getter implementation
			get	{ return false;	}
		}

		/// <summary></summary>
		public override bool IsComponentType
		{
			// TODO:  Add DynamicComponentType.IsComponentType getter implementation
			get	{ return false;	}
		}

		/// <summary></summary>
		public override bool IsEntityType
		{
			// TODO:  Add DynamicComponentType.IsEntityType getter implementation
			get	{ return false;	}
		}

		/// <summary></summary>
		public override bool IsObjectType
		{
			// TODO:  Add DynamicComponentType.IsObjectType getter implementation
			get	{ return false;	}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mapping"></param>
		/// <returns></returns>
		public override SqlType[] SqlTypes(IMapping mapping)
		{
			// Not called at runtime so it doesn't matter if it's slow :-)
			SqlType[] sqlTypes = new SqlType[ GetColumnSpan( mapping ) ];
			int n = 0;
			for ( int i = 0; i < propertySpan; i++ )
			{
				SqlType[] subtypes = propertyTypes[ i ].SqlTypes( mapping );
				for ( int j = 0; j < subtypes.Length; j++ )
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
		public override int GetColumnSpan(IMapping mapping)
		{
			// TODO:  Add DynamicComponentType.GetColumnSpan implementation
			return 0;
		}

		/// <summary>
		/// 
		/// </summary>
		public override System.Type ReturnedClass
		{
			// TODO:  Add DynamicComponentType.ReturnedClass getter implementation
			get	{ return null; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public override bool Equals( object x, object y )
		{
			if ( x == y ) 
			{
				return true;
			}
			if ( x == null || y == null )
			{
				return false;
			}
			Map xbean = x as Map;
			Map ybean = y as Map;
			for ( int i = 0; i < propertySpan; i++ )
			{
				if ( !propertyTypes[ i ].Equals( xbean[ propertyNames[ i ] ], ybean[ propertyNames[ i ] ] ) )
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="old"></param>
		/// <param name="current"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public override bool IsDirty( object old, object current, ISessionImplementor session )
		{
			if ( old == current ) 
			{
				return false;
			}
			if ( old == null || current == null )
			{
				return true;
			}
			Map xbean = old as Map;
			Map ybean = current as Map;
			for ( int i = 0; i < propertySpan; i++ )
			{
				if ( propertyTypes[ i ].IsDirty( xbean[ propertyNames[ i ] ], ybean[ propertyNames[ i ] ], session ) )
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
		public override object NullSafeGet(System.Data.IDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			// TODO:  Add DynamicComponentType.NullSafeGet implementation
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="name"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object NullSafeGet(System.Data.IDataReader rs, string name, ISessionImplementor session, System.Object owner)
		{
			// TODO:  Add DynamicComponentType.NHibernate.Type.IType.NullSafeGet implementation
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="st"></param>
		/// <param name="value"></param>
		/// <param name="index"></param>
		/// <param name="session"></param>
		public override void NullSafeSet(System.Data.IDbCommand st, object value, int index, ISessionImplementor session)
		{
			// TODO:  Add DynamicComponentType.NullSafeSet implementation
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		public override string ToXML(object value, ISessionFactoryImplementor factory)
		{
			// TODO:  Add DynamicComponentType.ToXML implementation
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		public override string Name
		{
			// TODO: 
			get	{ return typeof(Map).Name; }
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool IsMutable
		{
			get { return true; }
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool HasNiceEquals
		{
			get	{ return false; }
		}

		#endregion
	}
}
