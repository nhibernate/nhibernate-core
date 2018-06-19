using System;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace NHibernate.Test.NHSpecificTest.GH1759
{
	public class DateSpanType : ICompositeUserType
	{
		public new bool Equals(object x, object y)
		{
			if (ReferenceEquals(x, y))
			{
				return true;
			}

			if (x == null || y == null)
			{
				return false;
			}

			return x.Equals(y);
		}

		public string[] PropertyNames => new[] { "Date1", "Date2" };

		public IType[] PropertyTypes => new IType[] { NHibernateUtil.Date, NHibernateUtil.Date };

		public object GetPropertyValue(object component, int property)
		{
			var p = (DateSpan) component;
			return property == 0 ? p.Date1 : p.Date2;
		}

		public void SetPropertyValue(object component, int property, object value)
		{
			throw new NotImplementedException();
		}

		public System.Type ReturnedClass => typeof(DateSpan);

		public int GetHashCode(object x)
		{
			return x.GetHashCode();
		}

		public object NullSafeGet(DbDataReader dr, string[] names, ISessionImplementor session, object owner)
		{
			var data = new DateSpan
			{
				Date1 = (DateTime?) NHibernateUtil.Date.NullSafeGet(dr, new[] { names[0] }, session, owner),
				Date2 = (DateTime?) NHibernateUtil.Date.NullSafeGet(dr, new[] { names[1] }, session, owner)
			};

			return data;
		}

		public void NullSafeSet(DbCommand cmd, object value, int index, bool[] settable, ISessionImplementor session)
		{
			if (value == null)
			{
				if (settable[0])
					cmd.Parameters[index++].Value = DBNull.Value;
				if (settable[1])
					cmd.Parameters[index].Value = DBNull.Value;
			}
			else
			{
				var data = (DateSpan) value;
				if (settable[0])
					NHibernateUtil.Date.NullSafeSet(cmd, data.Date1, index++, session);
				if (settable[1])
					NHibernateUtil.Date.NullSafeSet(cmd, data.Date2, index, session);
			}
		}

		public object DeepCopy(object value)
		{
			return value;
		}

		public bool IsMutable => true;

		public object Disassemble(object value, ISessionImplementor session)
		{
			return DeepCopy(value);
		}

		public object Assemble(object cached, ISessionImplementor session, object owner)
		{
			return DeepCopy(cached);
		}

		public object Replace(object original, object target, ISessionImplementor session, object owner)
		{
			return original;
		}
	}
}
