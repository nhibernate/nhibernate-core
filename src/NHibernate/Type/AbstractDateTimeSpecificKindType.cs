using System;
using System.Data.Common;
using NHibernate.Engine;

namespace NHibernate.Type
{
	[Serializable]
	public abstract class AbstractDateTimeSpecificKindType : DateTimeType
	{
		protected abstract DateTimeKind DateTimeKind { get; }

		protected virtual DateTime CreateDateTime(DateTime dateValue)
		{
			return new DateTime(dateValue.Year, dateValue.Month, dateValue.Day, dateValue.Hour, dateValue.Minute, dateValue.Second, DateTimeKind);
		}

		public override object FromStringValue(string xml)
		{
			return DateTime.SpecifyKind(DateTime.Parse(xml), DateTimeKind);
		}

		public override int GetHashCode(object x)
		{
			int hashCode = base.GetHashCode(x);
			unchecked
			{
				hashCode = 31*hashCode + ((DateTime) x).Kind.GetHashCode();
			}
			return hashCode;
		}

		public override bool IsEqual(object x, object y)
		{
			if (x == y)
			{
				return true;
			}

			if (x == null || y == null)
			{
				return false;
			}

			return base.IsEqual(x, y) && ((DateTime) x).Kind == ((DateTime) y).Kind;
		}

		public override void Set(DbCommand st, object value, int index, ISessionImplementor session)
		{
			var dateValue = (DateTime) value;
			st.Parameters[index].Value = CreateDateTime(dateValue);
		}

		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			try
			{
				DateTime dbValue = Convert.ToDateTime(rs[index]);
				return CreateDateTime(dbValue);
			}
			catch (Exception ex)
			{
				throw new FormatException(string.Format("Input string '{0}' was not in the correct format.", rs[index]), ex);
			}
		}
	}
}