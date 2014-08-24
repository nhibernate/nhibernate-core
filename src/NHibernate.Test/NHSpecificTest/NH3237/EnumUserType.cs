using System.Linq;
using System;
using System.Data;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using NHibernate.Type;

namespace NHibernate.Test.NHSpecificTest.NH3237
{
	public class EnumUserType : IUserType
	{
		public System.Type ReturnedType
		{
			get { return typeof(TestEnum); }
		}

		public SqlType[] SqlTypes
		{
			get { return new[] { new SqlType(DbType.Int32) }; }
		}

		public object NullSafeGet(IDataReader dr, string[] names, object owner)
		{
			var name = names[0];
			int index = dr.GetOrdinal(name);

			if (dr.IsDBNull(index))
			{
				return null;
			}

			try
			{
				return Enum.Parse(typeof(TestEnum), dr.GetValue(index).ToString());
			}
			catch (InvalidCastException ice)
			{
				throw new ADOException(
					string.Format(
						"Could not cast the value in field {0} of type {1} to the Type {2}.  Please check to make sure that the mapping is correct and that your DataProvider supports this Data Type.",
						names[0], dr[index].GetType().Name, GetType().Name), ice);
			}
		}

		public void NullSafeSet(IDbCommand cmd, object value, int index)
		{
			if (value == null)
			{
				NHibernateUtil.DateTime.NullSafeSet(cmd, null, index);
			}
			else
			{
				var paramVal = (int)value;

				IDataParameter parameter = (IDataParameter)cmd.Parameters[index];
				parameter.Value = paramVal;
			}
		}

		public object Assemble(object cached, object owner)
		{
			return cached;
		}

		public object DeepCopy(object value)
		{
			return value;
		}

		public object Disassemble(object value)
		{
			return value;
		}

		public new bool Equals(object x, object y)
		{
			if (ReferenceEquals(x, null))
			{
				return ReferenceEquals(y, null);
			}
			return x.Equals(y);
		}

		public int GetHashCode(object x)
		{
			if (ReferenceEquals(x, null))
			{
				return 0;
			}
			return x.GetHashCode();
		}

		public bool IsMutable
		{
			get { return false; }
		}

		public object Replace(object original, object target, object owner)
		{
			return original;
		}

		public int Compare(object x, object y)
		{
			return ((DateTimeOffset)x).CompareTo((DateTimeOffset)y);
		}
	}
}
