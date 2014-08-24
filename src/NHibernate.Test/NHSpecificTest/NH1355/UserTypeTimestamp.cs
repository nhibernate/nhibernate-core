using System.Data;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace NHibernate.Test.NHSpecificTest.NH1355
{
	/// <summary>
	/// UserTypeTimestamp implements the Nhibernate BinaryType
	/// that is used to handle Nhibernate version.
	/// </summary>
	public class UserTypeTimestamp : IUserVersionType
	{
		#region IUserVersionType Members

		public object Next(object current, ISessionImplementor session)
		{
			return current;
		}

		public object Seed(ISessionImplementor session)
		{
			return new byte[8];
		}

		public object Assemble(object cached, object owner)
		{
			return DeepCopy(cached);
		}

		public object DeepCopy(object value)
		{
			return value;
		}

		public object Disassemble(object value)
		{
			return DeepCopy(value);
		}

		public int GetHashCode(object x)
		{
			return x.GetHashCode();
		}

		public bool IsMutable
		{
			get { return false; }
		}

		public object NullSafeGet(IDataReader rs, string[] names, object owner)
		{
			return rs.GetValue(rs.GetOrdinal(names[0]));
		}

		public void NullSafeSet(IDbCommand cmd, object value, int index)
		{
			NHibernateUtil.Binary.NullSafeSet(cmd, value, index);
		}

		public object Replace(object original, object target, object owner)
		{
			return original;
		}

		public System.Type ReturnedType
		{
			get { return typeof (byte[]); }
		}

		public SqlType[] SqlTypes
		{
			get { return new SqlType[] {new SqlType(DbType.Binary)}; }
		}

		public int Compare(object x, object y)
		{
			byte[] xbytes = (byte[]) x;
			byte[] ybytes = (byte[]) y;
			if (xbytes.Length < ybytes.Length)
			{
				return -1;
			}
			if (xbytes.Length > ybytes.Length)
			{
				return 1;
			}
			for (int i = 0; i < xbytes.Length; i++)
			{
				if (xbytes[i] < ybytes[i])
				{
					return -1;
				}
				if (xbytes[i] > ybytes[i])
				{
					return 1;
				}
			}
			return 0;
		}

		bool IUserType.Equals(object x, object y)
		{
			return (x == y);
		}

		#endregion
	}
}