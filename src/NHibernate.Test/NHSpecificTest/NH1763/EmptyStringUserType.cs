using System;
using System.Data;
using NHibernate.Engine;
using NHibernate.UserTypes;

namespace NHibernate.Test.NHSpecificTest.NH1763
{
	[Serializable]
	public class EmptyStringUserType : ICompositeUserType
	{
		public EmptyStringUserType()
		{
		}

		public System.Type ReturnedClass
		{
			get
			{
				return typeof(string);
			}
		}
		public bool IsMutable
		{
			get { return true; }
		}
		public String[] PropertyNames
		{
			get
			{
				return new String[] { "string" };
			}
		}

		public NHibernate.Type.IType[] PropertyTypes
		{
			get
			{
				return new NHibernate.Type.IType[] { NHibernateUtil.String };
			}
		}
		public object Assemble(object cached, NHibernate.Engine.ISessionImplementor session, object owner)
		{
			return DeepCopy(cached);
		}

		public object Disassemble(Object value, NHibernate.Engine.ISessionImplementor session)
		{
			return DeepCopy(value);
		}
		public Object DeepCopy(Object a)
		{
			if (a == null) return null;
			return a;
		}

		public new bool Equals(object x, object y)
		{
			return (x == y) || (x != null && y != null && (x.Equals(y)));
		}

		public object NullSafeGet(System.Data.IDataReader rs, String[] names, NHibernate.Engine.ISessionImplementor session, Object owner)
		{
			return NHibernateUtil.String.NullSafeGet(rs, names[0], session, owner);
		}

		public void NullSafeSet(System.Data.IDbCommand st, Object value, int index, bool[] settable, NHibernate.Engine.ISessionImplementor session)
		{
			if (settable[0])
			{
				string str = null;
				if (value != null) str = value.ToString().Trim();
				if (str == String.Empty)
				{
					str = null;
					NHibernateUtil.String.NullSafeSet(st, str, index, session);
				}
				else
					NHibernateUtil.String.NullSafeSet(st, value, index, session);
			}
		}

		public Object GetPropertyValue(Object component, int property)
		{
			return null;
		}

		public void SetPropertyValue(Object object1, int i, Object object2)
		{
		}

		#region ICompositeUserType Members

		public int GetHashCode(object x)
		{
			return x == null ? typeof(string).GetHashCode() : x.GetHashCode();
		}

		public object Replace(object original, object target, ISessionImplementor session, object owner)
		{
			return DeepCopy(original);
		}

		#endregion
	}
}