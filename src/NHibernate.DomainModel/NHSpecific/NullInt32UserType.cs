using System;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace NHibernate.DomainModel.NHSpecific
{
	/// <summary>
	/// Converts a value of 0 to a DbNull
	/// </summary>
	[Serializable]
	public class NullInt32UserType : IUserType
	{
		private static NullableType _int32Type = NHibernateUtil.Int32;

		public NullInt32UserType()
		{
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
			if (value.Equals(0))
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
			return _int32Type.NullSafeGet(rs, names, session);
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