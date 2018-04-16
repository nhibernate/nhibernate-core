using System;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace NHibernate.Test.SqlTest
{
	[Serializable]
	public class MonetaryAmountUserType : IUserType
	{
		private static readonly SqlType[] MySqlTypes = {SqlTypeFactory.Decimal, SqlTypeFactory.GetString(3)};

		public SqlType[] SqlTypes
		{
			get { return MySqlTypes; }
		}

		public System.Type ReturnedType
		{
			get { return typeof(MonetaryAmount); }
		}

		public bool IsMutable
		{
			get { return false; }
		}

		public object DeepCopy(object value)
		{
			return value; // MonetaryAmount is immutable
		}

		public new bool Equals(object x, object y)
		{
			return object.Equals(x, y);
		}

		public object NullSafeGet(DbDataReader resultSet, string[] names, ISessionImplementor session, object owner)
		{
			int index0 = resultSet.GetOrdinal(names[0]);
			int index1 = resultSet.GetOrdinal(names[1]);
			if (resultSet.IsDBNull(index0))
			{
				return null;
			}
			decimal value = resultSet.GetDecimal(index0);
			string cur = resultSet.GetString(index1);
			return new MonetaryAmount(value, cur);
		}

		public void NullSafeSet(DbCommand statement, object value, int index, ISessionImplementor session)
		{
			if (value == null)
			{
				statement.Parameters[index].Value = DBNull.Value;
				statement.Parameters[index + 1].Value = DBNull.Value;
			}
			else
			{
				MonetaryAmount currency = (MonetaryAmount) value;
				statement.Parameters[index].Value = currency.Value;
				statement.Parameters[index + 1].Value = currency.Currency;
			}
		}

		public object Disassemble(object value)
		{
			return value;
		}

		public object Assemble(object cached, Object owner)
		{
			return cached;
		}

		public object Replace(object original, object target, object owner)
		{
			return original;
		}

		public int GetHashCode(Object x)
		{
			return x.GetHashCode();
		}
	}
}