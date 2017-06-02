using System;
using System.Data;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace NHibernate.Test.NHSpecificTest.EntityWithUserTypeCanHaveLinqGenerators
{
	public class ExampleUserType : IUserType
	{
		public new bool Equals(object a, object b)
		{
			IExample ga = a as IExample;
			IExample gb = b as IExample;
			if (ga == null && gb == null)
			{
				return true;
			}
			if (ga != null && gb != null && (ga.GetType() == gb.GetType()))
			{
				return ga.Value == gb.Value;
			}
			return false;
		}

		public int GetHashCode(object x)
		{
			return ((IExample)x).Value.GetHashCode();
		}

		public object DeepCopy(object value)
		{
			if (value == null) return null;
			if (value.GetType() == typeof(BarExample))
			{
				return new BarExample { Value = ((IExample)value).Value };
			}
			return new FooExample { Value = ((IExample)value).Value };
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

		public SqlType[] SqlTypes { get { return new SqlType[] { SqlTypeFactory.GetString(255) }; } }

		public System.Type ReturnedType { get { return typeof(IExample); } }
		public bool IsMutable { get { return true; } }

		public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			var dataParameter = cmd.Parameters[index];
			var example = (IExample)value;
			dataParameter.DbType = DbType.String;
			if (value == null || example.Value == null)
			{
				dataParameter.Value = DBNull.Value;
			}
			else
			{
				dataParameter.Value = example.ToString();
			}
		}

		public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			var index = rs.GetOrdinal(names[0]);
			if (rs.IsDBNull(index))
			{
				return null;
			}
			var val = rs.GetString(index);

			var parts = val.Split(':');
			if (parts[0] == "Bar")
			{
				return new BarExample { Value = parts[1] };
			}
			return new FooExample { Value = parts[1] };
		}
	}
}
