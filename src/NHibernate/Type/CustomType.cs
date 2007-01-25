using System;
using System.Collections;
using System.Data;
using System.Reflection;
using log4net;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Adapts IUserType to the generic IType interface.
	/// <seealso cref="IUserType"/>
	/// </summary>
	[Serializable]
	public class CustomType : AbstractType, IIdentifierType, IDiscriminatorType, IVersionType
	{
		private readonly IUserType userType;
		private readonly INullableUserType userTypeAsNullable;
		private readonly string name;
		private readonly SqlType[ ] sqlTypes;

		/// <summary></summary>
		protected IUserType UserType
		{
			get { return userType; }
		}

		public CustomType( System.Type userTypeClass, IDictionary parameters )
		{
			name = userTypeClass.Name;

			try
			{
				userType = ( IUserType ) Activator.CreateInstance( userTypeClass );
				userTypeAsNullable = userType as INullableUserType;
			}
			catch( ArgumentNullException ane )
			{
				throw new MappingException( "Argument is a null reference.", ane );
			}
			catch( ArgumentException ae )
			{
				throw new MappingException( "Argument " + userTypeClass.Name + " is not a RuntimeType", ae );
			}
			catch( TargetInvocationException tie )
			{
				throw new MappingException( "The constructor being called throws an exception.", tie );
			}
			catch( MethodAccessException mae )
			{
				throw new MappingException( "The caller does not have permission to call this constructor.", mae );
			}
			catch( MissingMethodException mme )
			{
				throw new MappingException( "No matching constructor was found.", mme );
			}
			catch( InvalidCastException ice )
			{
				throw new MappingException( userTypeClass.Name + " must implement NHibernate.UserTypes.IUserType", ice );
			}
		    TypeFactory.InjectParameters(userType,parameters);
			sqlTypes = userType.SqlTypes;
			if( !userType.ReturnedType.IsSerializable )
			{
				LogManager.GetLogger( typeof( CustomType ) ).Warn( "custom type is not Serializable: " + userTypeClass );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mapping"></param>
		/// <returns></returns>
		public override SqlType[ ] SqlTypes( IMapping mapping )
		{
			return sqlTypes;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		public override int GetColumnSpan( IMapping session )
		{
			return sqlTypes.Length;
		}

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return userType.ReturnedType; }
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

		public override int GetHashCode(object x, ISessionFactoryImplementor factory)
		{
			return userType.GetHashCode(x);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="names"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object NullSafeGet(
			IDataReader rs,
			string[ ] names,
			ISessionImplementor session,
			object owner
			)
		{
			return userType.NullSafeGet( rs, names, owner );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="name"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object NullSafeGet(
			IDataReader rs,
			string name,
			ISessionImplementor session,
			object owner
			)
		{
			return NullSafeGet( rs, new string[ ] {name}, session, owner );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="st"></param>
		/// <param name="value"></param>
		/// <param name="index"></param>
		/// <param name="settable"></param>
		/// <param name="session"></param>
		public override void NullSafeSet( IDbCommand st, object value, int index, bool[] settable, ISessionImplementor session )
		{
			if (settable[0]) userType.NullSafeSet( st, value, index );
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
			ISessionImplementor session
			)
		{
			userType.NullSafeSet( cmd, value, index );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		public override string ToLoggableString( object value, ISessionFactoryImplementor factory )
		{
			return value == null ? "null" : value.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public override object FromString( string xml )
		{
			throw new NotSupportedException( "not yet implemented!" ); //TODO: look for constructor
		}

		/// <summary></summary>
		public override string Name
		{
			get { return name; }
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

		/// <summary></summary>
		public override bool IsMutable
		{
			get { return userType.IsMutable; }
		}

		/// <summary></summary>
		public override bool HasNiceEquals
		{
			get { return false; }
		}

		public override bool Equals(object obj)
		{
			if( !base.Equals( obj ) )
			{
				return false;
			}

			return ( ( CustomType ) obj ).userType.GetType() == userType.GetType();
		}

		public override int GetHashCode()
		{
			return userType.GetHashCode();
		}

		public override bool IsDirty(object old, object current, bool[] checkable, ISessionImplementor session)
		{
			return checkable[ 0 ] && IsDirty(old, current, session);
		}

		public object StringToObject(string xml)
		{
			return ((IEnhancedUserType) userType).FromXMLString(xml);
		}

		public string ObjectToSQLString(object value)
		{
			return ((IEnhancedUserType) userType).ObjectToSQLString(value);
		}

		public object Next(object current, ISessionImplementor session)
		{
			return ((IUserVersionType) userType).Next(current, session);
		}

		public object Seed(ISessionImplementor session)
		{
			return ((IUserVersionType) userType).Seed(session);
		}

		public IComparer Comparator
		{
			get { return (IComparer) userType; }
		}

		public override bool IsDatabaseNull(object value)
		{
			if (userTypeAsNullable != null)
			{
				return userTypeAsNullable.IsDatabaseNull(value);
			}
			else
			{
				return base.IsDatabaseNull(value);
			}
		}
	}
}
