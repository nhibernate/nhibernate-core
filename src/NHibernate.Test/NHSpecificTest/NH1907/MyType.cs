using System;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace NHibernate.Test.NHSpecificTest.NH1907
{
	public class MyType
	{
		public int ToPersist { get; set; }

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			var other = (MyType)obj;
			return ToPersist == other.ToPersist;
		}

		public override int GetHashCode()
		{
			return ToPersist.GetHashCode();
		}
	}

	public class SimpleCustomType : IUserType
	{
		private static readonly SqlType[] ReturnSqlTypes = { SqlTypeFactory.Int32 };


		#region IUserType Members

		public new bool Equals(object x, object y)
		{
			if (ReferenceEquals(x, y))
			{
				return true;
			}
			if (ReferenceEquals(null, x) || ReferenceEquals(null, y))
			{
				return false;
			}

			return x.Equals(y);
		}

		public int GetHashCode(object x)
		{
			return (x == null) ? 0 : x.GetHashCode();
		}

		public SqlType[] SqlTypes
		{
			get { return ReturnSqlTypes; }
		}

		public object DeepCopy(object value)
		{
			return value;
		}

		public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			if (value == null)
			{
				cmd.Parameters[index].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters[index].Value = ((MyType)value).ToPersist;
			}
		}

		public System.Type ReturnedType
		{
			get { return typeof(Int32); }
		}

		public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			int index0 = rs.GetOrdinal(names[0]);
			if (rs.IsDBNull(index0))
			{
				return null;
			}
			int value = rs.GetInt32(index0);
			return new MyType { ToPersist = value};
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

		#endregion
	}

}
