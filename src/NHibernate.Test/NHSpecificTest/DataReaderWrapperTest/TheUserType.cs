using System;
using System.Data;
using System.Data.Common;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace NHibernate.Test.NHSpecificTest.DataReaderWrapperTest
{
	public class TheUserType : IUserType
	{
		public SqlType[] SqlTypes
		{
			get { return new[] {new SqlType(DbType.String)}; }
		}

		public System.Type ReturnedType
		{
			get { return typeof(string); }
		}

		public bool Equals(object x, object y)
		{
			return x.Equals(y);
		}

		public int GetHashCode(object x)
		{
			return x.GetHashCode();
		}

		public object NullSafeGet(DbDataReader rs, string[] names, object owner)
		{
			return rs.GetValue(rs.GetOrdinal(names[0]));
		}

		public void NullSafeSet(DbCommand cmd, object value, int index)
		{
			NHibernateUtil.String.NullSafeSet(cmd, value, index);
		}

		public object DeepCopy(object value)
		{
			return value;
		}

		public bool IsMutable
		{
			get { return NHibernateUtil.String.IsMutable; }
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
