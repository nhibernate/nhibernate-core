using System;
using System.Collections;
using System.Data;

using NHibernate.Engine;
using NHibernate.Loader;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Type
{
	/// <summary>
	/// Handles "dynamic" components, represented as <c>&lt;map&gt;</c>s
	/// </summary>
	public class DynamicComponentType : AbstractType, IAbstractComponentType
	{
		private string[ ] propertyNames;
		private IType[ ] propertyTypes;
		private int propertySpan;
		private readonly Cascades.CascadeStyle[ ] cascade;
		private readonly FetchMode[ ] joinedFetch;

		public DynamicComponentType(
			string[ ] propertyNames,
			IType[ ] propertyTypes,
			FetchMode[ ] joinedFetch,
			Cascades.CascadeStyle[ ] cascade
			)
		{
			this.propertyNames = propertyNames;
			this.propertyTypes = propertyTypes;
			this.joinedFetch = joinedFetch;
			this.cascade = cascade;
			propertySpan = propertyTypes.Length;
		}

		public Cascades.CascadeStyle Cascade( int i )
		{
			return cascade[ i ];
		}

		public FetchMode GetFetchMode( int i )
		{
			return joinedFetch[ i ];
		}

		public string[ ] PropertyNames
		{
			get { return propertyNames; }
		}

		public object GetPropertyValue( object component, int i, ISessionImplementor session )
		{
			return GetPropertyValue( component, i );
		}

		public object[ ] GetPropertyValues( object component, ISessionImplementor session )
		{
			return GetPropertyValues( component );
		}

		public object GetPropertyValue( object component, int i )
		{
			return ( ( IDictionary ) component )[ propertyNames[ i ] ];
		}

		public object[ ] GetPropertyValues( object component )
		{
			IDictionary bean = ( IDictionary ) component;
			object[ ] result = new object[propertySpan];
			for( int i = 0; i < propertySpan; i++ )
			{
				result[ i ] = bean[ propertyNames[ i ] ];
			}
			return result;
		}

		public IType[ ] Subtypes
		{
			get { return propertyTypes; }
		}

		public object Instantiate()
		{
			return new Hashtable();
		}

		public void SetPropertyValues( object component, object[ ] values )
		{
			IDictionary map = ( IDictionary ) component;
			for( int i = 0; i < propertySpan; i++ )
			{
				map[ propertyNames[ i ] ] = values[ i ];
			}
		}

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

			object[ ] values = TypeFactory.Copy(
				GetPropertyValues( original ),
				GetPropertyValues( target ),
				propertyTypes, session, owner, copiedAlready );

			object result = target == null ? Instantiate() : target;
			SetPropertyValues( result, values );
			return result;
		}

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

			IDictionary xbean = x as IDictionary;
			IDictionary ybean = y as IDictionary;

			for( int i = 0; i < propertySpan; i++ )
			{
				if( !propertyTypes[ i ].Equals( xbean[ propertyNames[ i ] ], ybean[ propertyNames[ i ] ] ) )
				{
					return false;
				}
			}

			return true;
		}

		public override bool IsDirty( object old, object current, ISessionImplementor session )
		{
			if( old == current )
			{
				return false;
			}
			if( old == null || current == null )
			{
				return true;
			}

			IDictionary xbean = old as IDictionary;
			IDictionary ybean = current as IDictionary;
			for( int i = 0; i < propertySpan; i++ )
			{
				if( propertyTypes[ i ].IsDirty( xbean[ propertyNames[ i ] ], ybean[ propertyNames[ i ] ], session ) )
				{
					return true;
				}
			}
			return false;
		}

		public override int GetColumnSpan( IMapping mapping )
		{
			int span = 0;
			for( int i = 0; i < propertySpan; i++ )
			{
				span += propertyTypes[ i ].GetColumnSpan( mapping );
			}
			return span;
		}

		public override string Name
		{
			// TODO: 
			get { return typeof( IDictionary ).Name; }
		}

		public override bool HasNiceEquals
		{
			get { return false; }
		}

		public override bool IsMutable
		{
			get { return true; }
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

		public override object NullSafeGet( IDataReader rs, string name, ISessionImplementor session, Object owner )
		{
			return NullSafeGet( rs, new string[ ] { name }, session, owner );
		}

		public override object NullSafeGet( IDataReader rs, string[ ] names, ISessionImplementor session, object owner )
		{
			int begin = 0;
			bool notNull = false;

			object[ ] values = new object[ propertySpan ];
			for( int i = 0; i < propertySpan; i++ )
			{
				int length = propertyTypes[ i ].GetColumnSpan( session.Factory );
				string[ ] range = ArrayHelper.Slice( names, begin, length ); //cache this
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
				IDictionary result = ( IDictionary ) Instantiate();
				for( int i = 0; i < propertySpan; i++ )
				{
					result[ propertyNames[ i ] ] = values[ i ];
				}
				return result;
			}
			else
			{
				return null;
			}
		}

		public override void NullSafeSet( IDbCommand st, object value, int begin, ISessionImplementor session )
		{
			object[ ] subvalues = NullSafeGetValues( value );

			for( int i = 0; i < propertySpan; i++ )
			{
				propertyTypes[ i ].NullSafeSet( st, subvalues[ i ], begin, session );
				begin += propertyTypes[ i ].GetColumnSpan( session.Factory );
			}
		}

		public override System.Type ReturnedClass
		{
			get { return typeof( IDictionary ); }
		}

		public override SqlType[ ] SqlTypes( IMapping mapping )
		{
			// Not called at runtime so it doesn't matter if it's slow :-)
			SqlType[ ] sqlTypes = new SqlType[GetColumnSpan( mapping )];
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

		public override string ToLoggableString( object value, ISessionFactoryImplementor factory )
		{
			return value == null ? "null" : CollectionPrinter.ToString( ( IDictionary ) value );
		}

		public override object FromString( string xml )
		{
			throw new NotSupportedException();
		}

		public override bool IsComponentType
		{
			get { return true; }
		}
	}
}