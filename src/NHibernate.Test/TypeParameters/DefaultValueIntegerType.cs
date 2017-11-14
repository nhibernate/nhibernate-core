using System;
using System.Data.Common;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.UserTypes;
using System.Collections.Generic;
using NHibernate.Engine;

namespace NHibernate.Test.TypeParameters
{
	public class DefaultValueIntegerType : IUserType, IParameterizedType
	{
		private int defaultValue;
		private static NullableType _int32Type = NHibernateUtil.Int32;

		public void SetParameterValues(IDictionary<string, string> parameters)
		{
			defaultValue = int.Parse(parameters["default"]);
		}

		#region IUserType Members

		public new bool Equals(object x, object y)
		{
			if (x == y) return true;

			int lhs = (x == null) ? 0 : (int) x;
			int rhs = (y == null) ? 0 : (int) y;

			return _int32Type.IsEqual(lhs, rhs);
		}

		public int GetHashCode(object x)
		{
			return (x == null) ? 0 : x.GetHashCode();
		}

		public SqlType[] SqlTypes
		{
			get { return new SqlType[] {_int32Type.SqlType}; }
		}

		public object DeepCopy(object value)
		{
			return value;
		}

		public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			if (value.Equals(defaultValue))
			{
				cmd.Parameters[index].Value = DBNull.Value;
			}
			else
			{
				_int32Type.Set(cmd, value, index, session);
			}
		}

		public System.Type ReturnedType
		{
			get { return typeof(Int32); }
		}

		public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			object value = _int32Type.NullSafeGet(rs, names, session);
			if (value == null)
				return defaultValue;
			return value;
		}

		public bool IsMutable
		{
			get { return _int32Type.IsMutable; }
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

		#endregion
	}
}