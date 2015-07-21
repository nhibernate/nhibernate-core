using System;
using System.Data;
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

		public object NullSafeGet(IDataReader resultSet,
		                          string[] names,
		                          object owner)
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

		public void NullSafeSet(IDbCommand statement,
		                        object value,
		                        int index)
		{
			if (value == null)
			{
				((IDbDataParameter) statement.Parameters[index]).Value = DBNull.Value;
				((IDbDataParameter) statement.Parameters[index + 1]).Value = DBNull.Value;
			}
			else
			{
				MonetaryAmount currency = (MonetaryAmount) value;
				((IDbDataParameter) statement.Parameters[index]).Value = currency.Value;
				((IDbDataParameter) statement.Parameters[index + 1]).Value = currency.Currency;
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