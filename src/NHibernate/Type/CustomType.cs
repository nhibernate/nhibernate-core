using System;
using System.Reflection;
using System.Data;

using log4net;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type {

	/// <summary>
	/// Adapts IUserType to the generic IType interface.
	/// <seealso cref="NHibernate.IUserType"/>
	/// </summary>
	public class CustomType	: AbstractType {

		private readonly IUserType userType;
		private readonly string name;
		private readonly SqlType[] sqlTypes;

		protected IUserType UserType {
			get {
				return userType;
			}
		}

		public CustomType(System.Type userTypeClass) {
			name = userTypeClass.Name;

			try {
				userType = (IUserType) Activator.CreateInstance(userTypeClass);
			}
			catch (ArgumentNullException ane) {
				throw new MappingException( "Argument is a null reference.", ane );
			}
			catch (ArgumentException ae) {
				throw new MappingException( "Argument " + userTypeClass.Name + " is not a RuntimeType", ae );
			}
			catch (TargetInvocationException tie) {
				throw new MappingException( "The constructor being called throws an exception.", tie );
			}
			catch (MethodAccessException mae) {
				throw new MappingException( "The caller does not have permission to call this constructor.", mae );
			}
			catch (MissingMethodException mme) {
				throw new MappingException( "No matching constructor was found.", mme );
			}
			catch (InvalidCastException ice) {
				throw new MappingException( userTypeClass.Name + " must implement NHibernate.IUserType", ice );
			}
			sqlTypes = userType.SqlTypes;
			if ( !userType.ReturnedType.IsSerializable ) {
				LogManager.GetLogger(typeof(CustomType)).Warn("custom type is not Serializable: " + userTypeClass);
			}
		}
		
		public override SqlType[] SqlTypes(IMapping mapping) {
			return sqlTypes;
		}

		public override int GetColumnSpan(IMapping session) {
			return sqlTypes.Length;
		}
		
		public override System.Type ReturnedClass {
			get { return userType.ReturnedType; }
		}
		
		public override bool Equals(object x, object y) {
			return userType.Equals(x, y);
		}
		
		public override object NullSafeGet(
			IDataReader rs,
			string[] names,
			ISessionImplementor session,
			object owner
		) {
			return userType.NullSafeGet(rs, names, owner);
		}
		
		public override object NullSafeGet(
			IDataReader rs,
			string name,
			ISessionImplementor session,
			object owner
		) {
			return NullSafeGet(rs, new string[] { name }, session, owner);
		}
		
		public override void NullSafeSet(
			IDbCommand cmd,
			object value,
			int index,
			ISessionImplementor session
		) {
			userType.NullSafeSet(cmd, value, index);
		}
		
		public override string ToXML(object value, ISessionFactoryImplementor factory) {
			return value.ToString();
		}
		
		public override string Name {
			get { return name; }
		}
		
		public override object DeepCopy(object value) {
			return userType.DeepCopy(value);
		}
		
		public override bool IsMutable {
			get { return userType.IsMutable; }
		}
		
		public override bool HasNiceEquals {
			get { return false; }
		}
	}
}