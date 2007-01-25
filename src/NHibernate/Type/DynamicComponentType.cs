using System;
using System.Collections;
using System.Data;

using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Type
{
	/// <summary>
	/// Handles "dynamic" components, represented as <c>&lt;map&gt;</c>s
	/// </summary>
	[Serializable]
	public class DynamicComponentType : AbstractType, IAbstractComponentType
	{
		private string[ ] propertyNames;
		private IType[ ] propertyTypes;
		private readonly bool[] propertyNullability;
		private int propertySpan;
		private readonly Cascades.CascadeStyle[ ] cascade;
		private readonly FetchMode[ ] joinedFetch;

		public DynamicComponentType(
			string[ ] propertyNames,
			IType[ ] propertyTypes,
			bool[] nullabilities,
			FetchMode[ ] joinedFetch,
			Cascades.CascadeStyle[ ] cascade
			)
		{
			this.propertyNames = propertyNames;
			this.propertyTypes = propertyTypes;
			this.propertyNullability = nullabilities;
			this.joinedFetch = joinedFetch;
			this.cascade = cascade;
			propertySpan = propertyTypes.Length;
		}

		public Cascades.CascadeStyle GetCascadeStyle( int i )
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

		public override object Replace( object original, object target, ISessionImplementor session, object owner, IDictionary copiedAlready )
		{
			if( original == null )
			{
				return null;
			}
			if( original == target )
			{
				return target;
			}

			object[ ] values = TypeFactory.Replace(
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
			object[] xvalues = GetPropertyValues( x );
			object[] yvalues = GetPropertyValues( y );
			for( int i = 0; i < xvalues.Length; i++ )
			{
				if( propertyTypes[ i ].IsDirty( xvalues[ i ], yvalues[ i ], session ) )
				{
					return true;
				}
			}
			return false;
		}

		public override bool IsDirty( object x, object y, bool[] checkable, ISessionImplementor session )
		{
			if( x == y )
			{
				return false;
			}
			if( x == null || y == null )
			{
				return true;
			}

			object[] xvalues = GetPropertyValues( x );
			object[] yvalues = GetPropertyValues( y );
			int loc = 0;
			for( int i = 0; i < xvalues.Length; i++ )
			{
				int len = propertyTypes[ i ].GetColumnSpan( session.Factory );
				if( len <= 1 )
				{
					bool dirty = ( len == 0 || checkable[ loc ] ) &&
						propertyTypes[ i ].IsDirty( xvalues[ i ], yvalues[ i ], session );
					if( dirty )
					{
						return true;
					}
				}
				else
				{
					bool[] subcheckable = new bool[len];
					Array.Copy( checkable, loc, subcheckable, 0, len );
					bool dirty = propertyTypes[ i ].IsDirty( xvalues[ i ], yvalues[ i ], subcheckable, session );
					if( dirty )
					{
						return true;
					}
				}
				loc += len;
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

		public override void NullSafeSet( IDbCommand st, object value, int begin, bool[] settable, ISessionImplementor session )
		{
			object[] subvalues = NullSafeGetValues( value );

			int subvaluesIndex = 0;
			int sqlParamIndex = begin;
			int settableIndex = 0;
			for (int i = 0; i < propertySpan; i++)
			{
				int len = propertyTypes[i].GetColumnSpan( session.Factory );
				if (len == 0)
				{
					// noop
				}
				else if (len == 1)
				{
					if (settable[settableIndex])
					{
						propertyTypes[i].NullSafeSet( st, subvalues[i], sqlParamIndex, session );
						sqlParamIndex++;
						settableIndex++;
					}
				}
				else
				{
					bool[] subsettable = new bool[len];
					Array.Copy( settable, subvaluesIndex, subsettable, 0, len );
					propertyTypes[i].NullSafeSet( st, subvalues[i], sqlParamIndex, subsettable, session );
					int subsettableCount = ArrayHelper.CountTrue( subsettable );
					sqlParamIndex += subsettableCount;
					settableIndex += subsettableCount;
				}
				subvaluesIndex += len;
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

		public bool[] PropertyNullability
		{
			get { return propertyNullability; }
		}
	}
}
