using System.Linq;
using System;
using System.Data;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace NHibernate.Test.NHSpecificTest.NH3237
{
	public class DateTimeOffsetUserType : IUserType, IParameterizedType
	{
		public TimeSpan Offset { get; private set; }

		public DateTimeOffsetUserType()
		{

		}
		public DateTimeOffsetUserType(TimeSpan offset)
		{
			Offset = offset;
		}

		public System.Type ReturnedType
		{
			get { return typeof(DateTimeOffset); }
		}

		public SqlType[] SqlTypes
		{
			get { return new[] { new SqlType(DbType.DateTime) }; }
		}

		public object NullSafeGet(IDataReader dr, string[] names, object owner)
		{
			object r = dr[names[0]];
			if (r == DBNull.Value)
			{
				return null;
			}

			DateTime storedTime = (DateTime)r;
			return new DateTimeOffset(storedTime, Offset);
		}

		public void NullSafeSet(IDbCommand cmd, object value, int index)
		{
			if (value == null)
			{
				NHibernateUtil.DateTime.NullSafeSet(cmd, null, index);
			}
			else
			{
				DateTimeOffset dateTimeOffset = (DateTimeOffset)value;
				DateTime paramVal = dateTimeOffset.ToOffset(Offset).DateTime;

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

		public void SetParameterValues(System.Collections.Generic.IDictionary<string, string> parameters)
		{
			string offset;
			if (parameters.TryGetValue("Offset", out offset))
			{
				Offset = TimeSpan.Parse(offset);
			}
		}
	}
}
