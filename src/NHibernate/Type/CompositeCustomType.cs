using System;
using System.Collections;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using System.Collections.Generic;

namespace NHibernate.Type
{
	[Serializable]
	public partial class CompositeCustomType : AbstractType, IAbstractComponentType
	{
		private readonly ICompositeUserType userType;
		private readonly string name;

		public ICompositeUserType UserType
		{
			// needed as public by ours Contrib projects
			get { return userType; }
		}

		public CompositeCustomType(System.Type userTypeClass, IDictionary<string, string> parameters)
		{
			name = userTypeClass.FullName;

			try
			{
				userType = (ICompositeUserType) Cfg.Environment.BytecodeProvider.ObjectsFactory.CreateInstance(userTypeClass);
			}
			catch (MethodAccessException mae)
			{
				throw new MappingException("MethodAccessException trying to instantiate custom type: " + name, mae);
			}
			catch (TargetInvocationException tie)
			{
				throw new MappingException("TargetInvocationException trying to instantiate custom type: " + name, tie);
			}
			catch (ArgumentException ae)
			{
				throw new MappingException("ArgumentException trying to instantiate custom type: " + name, ae);
			}
			catch (InvalidCastException ice)
			{
				throw new MappingException(name + " must implement NHibernate.UserTypes.ICompositeUserType", ice);
			}
			TypeFactory.InjectParameters(userType, parameters);
			if (!userType.ReturnedClass.IsSerializable)
			{
				LoggerProvider.LoggerFor(typeof(CustomType)).WarnFormat("the custom composite class '{0}' handled by '{1}' is not Serializable: ", userType.ReturnedClass, userTypeClass);
			}

			// This is to be nice to an application developer.
			if (userType.PropertyTypes == null)
				throw new InvalidOperationException(String.Format("ICompositeUserType {0} returned a null value for 'PropertyTypes'.", userType.GetType()));
			if (userType.PropertyNames == null)
				throw new InvalidOperationException(String.Format("ICompositeUserType {0} returned a null value for 'PropertyNames'.", userType.GetType()));
		}

		public virtual IType[] Subtypes
		{
			get { return userType.PropertyTypes; }
		}

		public virtual string[] PropertyNames
		{
			get { return userType.PropertyNames; }
		}

		public virtual object[] GetPropertyValues(object component, ISessionImplementor session)
		{
			return GetPropertyValues(component);
		}

		public virtual object[] GetPropertyValues(object component)
		{
			int len = Subtypes.Length;
			object[] result = new object[len];
			for (int i = 0; i < len; i++)
			{
				result[i] = GetPropertyValue(component, i);
			}
			return result;
		}

		public virtual void SetPropertyValues(object component, object[] values)
		{
			for (int i = 0; i < values.Length; i++)
				userType.SetPropertyValue(component, i, values[i]);
		}

		public virtual object GetPropertyValue(object component, int i, ISessionImplementor session)
		{
			return GetPropertyValue(component, i);
		}

		public object GetPropertyValue(object component, int i)
		{
			return userType.GetPropertyValue(component, i);
		}

		public virtual CascadeStyle GetCascadeStyle(int i)
		{
			return CascadeStyle.None;
		}

		public virtual FetchMode GetFetchMode(int i)
		{
			return FetchMode.Default;
		}

		public bool IsEmbedded
		{
			get { return false; }
		}

		public override bool IsComponentType
		{
			get { return true; }
		}

		public override object Assemble(object cached, ISessionImplementor session, object owner)
		{
			return userType.Assemble(cached, session, owner);
		}

		public override object DeepCopy(object value, ISessionFactoryImplementor factory)
		{
			return userType.DeepCopy(value);
		}

		public override object Disassemble(object value, ISessionImplementor session, object owner)
		{
			return userType.Disassemble(value, session);
		}

		public override int GetColumnSpan(IMapping mapping)
		{
			IType[] types = userType.PropertyTypes;
			int n = 0;
			for (int i = 0; i < types.Length; i++)
			{
				n += types[i].GetColumnSpan(mapping); //Can nested type cause recursion???
			}
			return n;
		}

		public override string Name
		{
			get { return name; }
		}

		public override System.Type ReturnedClass
		{
			get { return userType.ReturnedClass; }
		}

		public override bool IsMutable
		{
			get { return userType.IsMutable; }
		}

		public override object NullSafeGet(DbDataReader rs, string name, ISessionImplementor session, object owner)
		{
			return userType.NullSafeGet(rs, new string[] {name}, session, owner);
		}

		public override object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			return userType.NullSafeGet(rs, names, session, owner);
		}

		public override void NullSafeSet(DbCommand st, object value, int index, bool[] settable, ISessionImplementor session)
		{
			userType.NullSafeSet(st, value, index, settable, session);
		}

		public override void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			bool[] settable = Enumerable.Repeat(true, GetColumnSpan(session.Factory)).ToArray();
			userType.NullSafeSet(cmd, value, index, settable, session);
		}

		public override SqlType[] SqlTypes(IMapping mapping)
		{
			IType[] types = userType.PropertyTypes;
			SqlType[] result = new SqlType[GetColumnSpan(mapping)];
			int n = 0;
			for (int i = 0; i < types.Length; i++)
			{
				SqlType[] sqlTypes = types[i].SqlTypes(mapping);
				for (int k = 0; k < sqlTypes.Length; k++)
				{
					result[n++] = sqlTypes[k];
				}
			}
			return result;
		}

		public override string ToLoggableString(object value, ISessionFactoryImplementor factory)
		{
			return value == null ? "null" : value.ToString();
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}

			return ((CompositeCustomType) obj).userType.GetType() == userType.GetType();
		}

		public override int GetHashCode()
		{
			return userType.GetHashCode();
		}

		public override bool IsDirty(object old, object current, bool[] checkable, ISessionImplementor session)
		{
			return IsDirty(old, current, session);
		}

		public bool[] PropertyNullability
		{
			get { return null; }
		}

		public override object Replace(object original, object current, ISessionImplementor session, object owner, IDictionary copiedAlready)
		{
			return userType.Replace(original, current, session, owner);
		}

		public override bool IsEqual(object x, object y)
		{
			return userType.Equals(x, y);
		}

		public virtual bool IsMethodOf(MethodBase method)
		{
			return false;
		}

		public override bool[] ToColumnNullness(object value, IMapping mapping)
		{
			bool[] result = new bool[GetColumnSpan(mapping)];
			if (value == null)
				return result;
			object[] values = GetPropertyValues(value);
			int loc = 0;
			IType[] propertyTypes = Subtypes;
			for (int i = 0; i < propertyTypes.Length; i++)
			{
				bool[] propertyNullness = propertyTypes[i].ToColumnNullness(values[i], mapping);
				Array.Copy(propertyNullness, 0, result, loc, propertyNullness.Length);
				loc += propertyNullness.Length;
			}
			return result;
		}

	}
}