using System;
using System.Data;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace NHibernate.Test.NHSpecificTest.EntityWithUserTypeCanHaveLinqGenerators
{
	public class DoubleStringUserType : IUserType
	{
		public SqlType[] SqlTypes
		{
			get { return new[] { SqlTypeFactory.GetString(20) }; }
		}

		public System.Type ReturnedType
		{
			get { return typeof(string); }
		}

		public bool IsMutable
		{
			get { return false; }
		}

		public int GetHashCode(object x)
		{
			if (x == null)
			{
				return 0;
			}
			return x.GetHashCode();
		}

		public object NullSafeGet(IDataReader rs, string[] names, object owner)
		{
			object obj = NHibernateUtil.String.NullSafeGet(rs, names[0]);
			if (obj == null)
			{
				return null;
			}
			return Convert.ToDouble((string)obj);
		}

		public void NullSafeSet(IDbCommand cmd, object value, int index)
		{
			if (value == null)
			{
				((IDataParameter)cmd.Parameters[index]).Value = DBNull.Value;
			}
			else
			{
				var doubleValue = (double)value;
				((IDataParameter)cmd.Parameters[index]).Value = doubleValue.ToString();
			}
		}

		public object DeepCopy(object value)
		{
			return value;
		}

		public object Replace(object original, object target, object owner)
		{
			return original;
		}

		public object Assemble(object cached, object owner)
		{
			return cached;
		}

		public object Disassemble(object value)
		{
			return value;
		}

		bool IUserType.Equals(object x, object y)
		{
			return object.Equals(x, y);
		}
	}
}
