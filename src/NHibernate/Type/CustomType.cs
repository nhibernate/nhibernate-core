using System;
using System.Data;
using System.Reflection;
using log4net;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Adapts IUserType to the generic IType interface.
	/// <seealso cref="IUserType"/>
	/// </summary>
	public class CustomType : AbstractType
	{
		private readonly IUserType userType;
		private readonly string name;
		private readonly SqlType[ ] sqlTypes;

		/// <summary></summary>
		protected IUserType UserType
		{
			get { return userType; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="userTypeClass"></param>
		public CustomType( System.Type userTypeClass )
		{
			name = userTypeClass.Name;

			try
			{
				userType = ( IUserType ) Activator.CreateInstance( userTypeClass );
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
				throw new MappingException( userTypeClass.Name + " must implement NHibernate.IUserType", ice );
			}
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
		public override string ToXML( object value, ISessionFactoryImplementor factory )
		{
			return value.ToString();
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
	}
}