using System;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Xml;

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
	public class CustomType : AbstractType, IDiscriminatorType, IVersionType
	{
		private readonly IUserType userType;
		private readonly string name;
		private readonly SqlType[] sqlTypes;

		/// <summary></summary>
		protected IUserType UserType
		{
			get { return userType; }
		}

		public CustomType(System.Type userTypeClass, IDictionary<string, string> parameters)
		{
			name = userTypeClass.Name;

			try
			{
				userType = (IUserType) Cfg.Environment.BytecodeProvider.ObjectsFactory.CreateInstance(userTypeClass);
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
			if (!userType.ReturnedType.IsSerializable)
			{
				LoggerProvider.LoggerFor(typeof(CustomType)).Warn("custom type is not Serializable: " + userTypeClass);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mapping"></param>
		/// <returns></returns>
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

		public override object NullSafeGet(IDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			return userType.NullSafeGet(rs, names, owner);
		}

		public override object NullSafeGet(IDataReader rs, string name, ISessionImplementor session, object owner)
		{
			return NullSafeGet(rs, new string[] {name}, session, owner);
		}

		public override void NullSafeSet(IDbCommand st, object value, int index, bool[] settable, ISessionImplementor session)
		{
			if (settable[0]) 
				userType.NullSafeSet(st, value, index);
		}

		public override void NullSafeSet(IDbCommand cmd, object value, int index, ISessionImplementor session)
		{
			userType.NullSafeSet(cmd, value, index);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		public override string ToLoggableString(object value, ISessionFactoryImplementor factory)
		{
			if (value == null)
			{
				return "null";
			}
			else
			{
				return ToXMLString(value, factory);
			}
		}

		/// <summary></summary>
		public override string Name
		{
			get { return name; }
		}

		public override object DeepCopy(object value, EntityMode entityMode, ISessionFactoryImplementor factory)
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

		public override int GetHashCode(object x, EntityMode entityMode)
		{
			return userType.GetHashCode(x);
		}

		public override bool IsDirty(object old, object current, bool[] checkable, ISessionImplementor session)
		{
			return checkable[0] && IsDirty(old, current, session);
		}

		public object StringToObject(string xml)
		{
			return ((IEnhancedUserType) userType).FromXMLString(xml);
		}
		
		public object FromStringValue(string xml)
		{
			return ((IEnhancedUserType)userType).FromXMLString(xml);
		}

		public virtual string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return ((IEnhancedUserType)userType).ObjectToSQLString(value);
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

		public override object FromXMLNode(XmlNode xml, IMapping factory)
		{
			return FromXMLString(xml.Value, factory);
		}

		public virtual object FromXMLString(string xml, IMapping factory)
		{
			return ((IEnhancedUserType)userType).FromXMLString(xml);
		}

		public virtual bool IsEqual(object x, object y)
		{
			return userType.Equals(x, y);
		}

		public override bool IsEqual(object x, object y, EntityMode entityMode)
		{
			return IsEqual(x, y);
		}

		public override void SetToXMLNode(XmlNode node, object value, ISessionFactoryImplementor factory)
		{
			node.Value= ToXMLString(value, factory);
		}

		public override bool[] ToColumnNullness(object value, IMapping mapping)
		{
			bool[] result = new bool[GetColumnSpan(mapping)];
			if (value != null)
				ArrayHelper.Fill(result, true);
			return result;
		}

		public virtual string ToXMLString(object value, ISessionFactoryImplementor factory)
		{
			if (value == null)
				return null;
			IEnhancedUserType eut = userType as IEnhancedUserType;
			if (eut != null)
			{
				return eut.ToXMLString(value);
			}
			else
			{
				return value.ToString();
			}
		}
	}
}