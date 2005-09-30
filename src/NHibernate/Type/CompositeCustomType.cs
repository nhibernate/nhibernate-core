using System;
using System.Data;
using System.Reflection;
using log4net;
using NHibernate.Engine;
using NHibernate.Loader;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Summary description for CompositeCustomType.
	/// </summary>
	public class CompositeCustomType : AbstractType, IAbstractComponentType
	{
		private readonly ICompositeUserType userType;
		private readonly string name;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="userTypeClass"></param>
		public CompositeCustomType( System.Type userTypeClass )
		{
			name = userTypeClass.Name;

			try
			{
				userType = ( ICompositeUserType ) Activator.CreateInstance( userTypeClass );
			}
			catch( MethodAccessException mae )
			{
				throw new MappingException( "MethodAccessException trying to instantiate custom type: " + name, mae );
			}
			catch( TargetInvocationException tie )
			{
				throw new MappingException( "TargetInvocationException trying to instantiate custom type: " + name, tie );
			}
			catch( ArgumentException ae )
			{
				throw new MappingException( "ArgumentException trying to instantiate custom type: " + name, ae );
			}
			catch( InvalidCastException ice )
			{
				throw new MappingException( name + " must implement NHibernate.ICompositeUserType", ice );
			}
			if( !userType.ReturnedClass.IsSerializable )
			{
				LogManager.GetLogger( typeof( CustomType ) ).Warn( "custom type is not Serializable: " + userTypeClass );
			}
		}

		/// <summary></summary>
		public virtual IType[ ] Subtypes
		{
			get { return userType.PropertyTypes; }
		}

		/// <summary></summary>
		public virtual string[ ] PropertyNames
		{
			get { return userType.PropertyNames; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public virtual object[ ] GetPropertyValues( object component, ISessionImplementor session )
		{
			return GetPropertyValues(  component );
		}

		public object[] GetPropertyValues(object component)
		{
			int len = Subtypes.Length;
			object[ ] result = new object[len];
			for( int i = 0; i < len; i++ )
			{
				result[ i ] = GetPropertyValue( component, i );
			}
			return result;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <param name="values"></param>
		public virtual void SetPropertyValues( Object component, Object[ ] values )
		{
			for( int i = 0; i < values.Length; i++ )
			{
				userType.SetPropertyValue( component, i, values[ i ] );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <param name="i"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public virtual object GetPropertyValue( object component, int i, ISessionImplementor session )
		{
			return GetPropertyValue( component, i );
		}

		public object GetPropertyValue( object component, int i )
		{
			return userType.GetPropertyValue( component, i );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public virtual Cascades.CascadeStyle Cascade( int i )
		{
			return Cascades.CascadeStyle.StyleNone;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public virtual OuterJoinFetchStrategy EnableJoinedFetch( int i )
		{
			return OuterJoinFetchStrategy.Auto;
		}

		/// <summary></summary>
		public override bool IsComponentType
		{
			get { return true; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cached"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object Assemble( object cached, ISessionImplementor session,
		                                 object owner )
		{
			return userType.Assemble( cached, session, owner );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override object DeepCopy( object value )
		{
			return userType.DeepCopy( value );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public override object Disassemble( object value, ISessionImplementor session )
		{
			return userType.Disassemble( value, session );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public override bool Equals( object x, object y )
		{
			return userType.Equals( x, y );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mapping"></param>
		/// <returns></returns>
		public override int GetColumnSpan( IMapping mapping )
		{
			IType[ ] types = userType.PropertyTypes;
			int n = 0;
			for( int i = 0; i < types.Length; i++ )
			{
				n += types[ i ].GetColumnSpan( mapping ); //Can nested type cause recursion???
			}
			return n;
		}

		/// <summary></summary>
		public override string Name
		{
			get { return name; }
		}

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return userType.ReturnedClass; }
		}

		/// <summary></summary>
		public override bool HasNiceEquals
		{
			get { return false; }
		}

		/// <summary></summary>
		public override bool IsMutable
		{
			get { return userType.IsMutable; }
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
			return userType.NullSafeGet( rs, new string[ ] {name}, session, owner );
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
			return userType.NullSafeGet( rs, names, session, owner );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="value"></param>
		/// <param name="index"></param>
		/// <param name="session"></param>
		public override void NullSafeSet(
			IDbCommand cmd,
			object value,
			int index,
			ISessionImplementor session )
		{
			userType.NullSafeSet( cmd, value, index, session );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mapping"></param>
		/// <returns></returns>
		public override SqlType[ ] SqlTypes( IMapping mapping )
		{
			IType[ ] types = userType.PropertyTypes;
			SqlType[ ] result = new SqlType[GetColumnSpan( mapping )];
			int n = 0;
			for( int i = 0; i < types.Length; i++ )
			{
				SqlType[ ] sqlTypes = types[ i ].SqlTypes( mapping );
				for( int k = 0; k < sqlTypes.Length; k++ )
				{
					result[ n++ ] = sqlTypes[ k ];
				}
			}
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		public override string ToString( object value, ISessionFactoryImplementor factory )
		{
			return value == null ? "null" : value.ToString();
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

		public override bool Equals(object obj)
		{
			if( !base.Equals( obj ) )
			{
				return false;
			}

			return ( ( CompositeCustomType ) obj ).userType.GetType() == userType.GetType();
		}

		public override int GetHashCode()
		{
			return userType.GetHashCode();
		}
	}
}