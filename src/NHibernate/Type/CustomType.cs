using System;
using System.Collections;
using System.Data.Common;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using NHibernate.Util;
using System.Collections.Generic;

namespace NHibernate.Type
{
	/// <summary>
	/// Adapts IUserType to the generic IType interface.
	/// <seealso cref="IUserType"/>
	/// </summary>
	[Serializable]
	public partial class CustomType : AbstractType, IDiscriminatorType, IVersionType
	{
		private readonly IUserType userType;
		private readonly string name;
		private readonly SqlType[] sqlTypes;

		/// <summary></summary>
		public IUserType UserType
		{
			// needed as public by ours Contrib projects
			get { return userType; }
		}

		public CustomType(System.Type userTypeClass, IDictionary<string, string> parameters)
		{
			name = userTypeClass.Name;

			try
			{
				userType = (IUserType) Cfg.Environment.ObjectsFactory.CreateInstance(userTypeClass);
			}
			catch (ArgumentNullException ane)
			{
				throw new MappingException("Argument is a null reference.", ane);
			}
			catch (ArgumentException ae)
			{
				throw new MappingException("Argument " + userTypeClass.Name + " is not a RuntimeType", ae);
			}
			catch (TargetInvocationException tie)
			{
				throw new MappingException("The constructor being called throws an exception.", tie);
			}
			catch (MethodAccessException mae)
			{
				throw new MappingException("The caller does not have permission to call this constructor.", mae);
			}
			catch (MissingMethodException mme)
			{
				throw new MappingException("No matching constructor was found.", mme);
			}
			catch (InvalidCastException ice)
			{
				throw new MappingException(userTypeClass.Name + " must implement NHibernate.UserTypes.IUserType", ice);
			}
			TypeFactory.InjectParameters(userType, parameters);
			sqlTypes = userType.SqlTypes;
		}

		/// <inheritdoc />
		public override SqlType[] SqlTypes(IMapping mapping)
		{
			return sqlTypes;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		public override int GetColumnSpan(IMapping session)
		{
			return sqlTypes.Length;
		}

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return userType.ReturnedType; }
		}

		public override object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			return userType.NullSafeGet(rs, names, session, owner);
		}

		public override object NullSafeGet(DbDataReader rs, string name, ISessionImplementor session, object owner)
		{
			return NullSafeGet(rs, new[] { name }, session, owner);
		}

		public override void NullSafeSet(DbCommand st, object value, int index, bool[] settable, ISessionImplementor session)
		{
			if (settable[0])
				userType.NullSafeSet(st, value, index, session);
		}

		public override void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			userType.NullSafeSet(cmd, value, index, session);
		}

		/// <inheritdoc />
		public override string ToLoggableString(object value, ISessionFactoryImplementor factory)
		{
			if (value == null)
			{
				return "null";
			}

			if (userType is IEnhancedUserType enhancedUserType)
			{
				// 6.0 TODO: remove warning disable/restore
#pragma warning disable 618
				return enhancedUserType.ToXMLString(value);
#pragma warning restore 618
			}
			return value.ToString();
		}

		/// <summary></summary>
		public override string Name
		{
			get { return name; }
		}

		public override object DeepCopy(object value, ISessionFactoryImplementor factory)
		{
			return userType.DeepCopy(value);
		}

		/// <summary></summary>
		public override bool IsMutable
		{
			get { return userType.IsMutable; }
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}

			return ((CustomType) obj).userType.GetType() == userType.GetType();
		}

		public override int GetHashCode()
		{
			return userType.GetType().GetHashCode();
		}

		public override int GetHashCode(object x)
		{
			return userType.GetHashCode(x);
		}

		public override bool IsDirty(object old, object current, bool[] checkable, ISessionImplementor session)
		{
			return checkable[0] && IsDirty(old, current, session);
		}

		// 6.0 TODO: rename "xml" parameter as "value": it is not a xml string. The fact it generally comes from a xml
		// attribute value is irrelevant to the method behavior.
		/// <inheritdoc />
		public object StringToObject(string xml)
		{
			if (!(userType is IEnhancedUserType enhancedUserType))
				throw new InvalidOperationException(
					$"User type {userType} does not implement {nameof(IEnhancedUserType)}, Either implement it, or " +
					$"avoid using this user type as an identifier or a discriminator.");
			// 6.0 TODO: remove warning disable/restore
#pragma warning disable 618
			return enhancedUserType.FromXMLString(xml);
#pragma warning restore 618
		}

		// 6.0 TODO: rename "xml" parameter as "value": it is not a xml string. The fact it generally comes from a xml
		// attribute value is irrelevant to the method behavior.
		/// <inheritdoc cref="IVersionType.FromStringValue"/>
		public object FromStringValue(string xml)
		{
			if (!(userType is IEnhancedUserType enhancedUserType))
				throw new InvalidOperationException(
					$"User type {userType} does not implement {nameof(IEnhancedUserType)}, Either implement it, or " +
					$"avoid using this user type as an identifier or a discriminator.");
			// 6.0 TODO: remove warning disable/restore
#pragma warning disable 618
			return enhancedUserType.FromXMLString(xml);
#pragma warning restore 618
		}

		public virtual string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			if (!(userType is IEnhancedUserType enhancedUserType))
				throw new InvalidOperationException(
					$"User type {userType} does not implement {nameof(IEnhancedUserType)}, its SQL literal value " +
					$"cannot be resolved. Either implement it, or avoid using this user type as an identifier, a " +
					$"discriminator, or with queries requiring its literal value.");
			return enhancedUserType.ObjectToSQLString(value);
		}

		public object Next(object current, ISessionImplementor session)
		{
			if (!(userType is IUserVersionType userVersionType))
				throw new InvalidOperationException(
					$"User type {userType} does not implement {nameof(IUserVersionType)}, Either implement it, or " +
					$"avoid using this user type as a version type.");
			return userVersionType.Next(current, session);
		}

		public object Seed(ISessionImplementor session)
		{
			if (!(userType is IUserVersionType userVersionType))
				throw new InvalidOperationException(
					$"User type {userType} does not implement {nameof(IUserVersionType)}, Either implement it, or " +
					$"avoid using this user type as a version type.");
			return userVersionType.Seed(session);
		}

		public IComparer Comparator
		{
			get
			{
				return userType as IComparer ?? throw new InvalidOperationException(
					$"User type {userType} does not implement {nameof(IUserVersionType)}, Either implement it, or " +
					$"avoid using this user type as a version type.");
			}
		}

		public override object Replace(object original, object current, ISessionImplementor session, object owner,
									   IDictionary copiedAlready)
		{
			return userType.Replace(original, current, owner);
		}

		public override object Assemble(object cached, ISessionImplementor session, object owner)
		{
			return userType.Assemble(cached, owner);
		}

		public override object Disassemble(object value, ISessionImplementor session, object owner)
		{
			return userType.Disassemble(value);
		}

		public override bool IsEqual(object x, object y)
		{
			return userType.Equals(x, y);
		}

		public override bool[] ToColumnNullness(object value, IMapping mapping)
		{
			bool[] result = new bool[GetColumnSpan(mapping)];
			if (value != null)
				ArrayHelper.Fill(result, true);
			return result;
		}
	}
}
