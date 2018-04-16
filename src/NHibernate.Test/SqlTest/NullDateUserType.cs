using System;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace NHibernate.Test.SqlTest
{
	public class NullDateUserType : IUserType
	{
		private SqlType[] sqlTypes = new SqlType[1] {SqlTypeFactory.Date};

		public SqlType[] SqlTypes
		{
			get { return sqlTypes; }
		}

		public System.Type ReturnedType
		{
			get { return typeof(DateTime); }
		}

		public new bool Equals(object x, object y)
		{
			return object.Equals(x, y);
		}

		public int GetHashCode(object x)
		{
			return x.GetHashCode();
		}

		public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			int ordinal = rs.GetOrdinal(names[0]);
			if (rs.IsDBNull(ordinal))
			{
				return DateTime.MinValue;
			}
			else
			{
				return rs.GetDateTime(ordinal);
			}
		}

		public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			object valueToSet = ((DateTime) value == DateTime.MinValue) ? DBNull.Value : value;
			cmd.Parameters[index].Value = valueToSet;
		}

		public object DeepCopy(object value)
		{
			return value;
		}

		public bool IsMutable
		{
			get { return false; }
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
	}
}