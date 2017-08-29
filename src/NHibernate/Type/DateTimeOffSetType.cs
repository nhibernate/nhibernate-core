using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.DateTimeOffset" /> Property to a <see cref="DbType.DateTimeOffset"/>
	/// </summary>
	[Serializable]
	public partial class DateTimeOffsetType : PrimitiveType, IIdentifierType, ILiteralType, IVersionType
	{
		static readonly DateTimeOffset BaseDateValue = DateTimeOffset.MinValue;

		/// <summary></summary>
		public DateTimeOffsetType()
			: base(SqlTypeFactory.DateTimeOffSet)
		{
		}

		public override string Name
		{
			get { return "DateTimeOffset"; }
		}

		public override System.Type ReturnedClass
		{
			get { return typeof (DateTimeOffset); }
		}

		public override System.Type PrimitiveClass
		{
			get { return typeof (DateTimeOffset); }
		}

		public override object DefaultValue
		{
			get { return BaseDateValue; }
		}

		public IComparer Comparator
		{
			get { return Comparer<DateTimeOffset>.Default; }
		}

		public override void Set(DbCommand st, object value, int index, ISessionImplementor session)
		{
			var dateValue = (DateTimeOffset) value;
			st.Parameters[index].Value =
				new DateTimeOffset(dateValue.Ticks, dateValue.Offset);
		}

		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			try
			{
				var dbValue = (DateTimeOffset) rs[index];
				return new DateTimeOffset(dbValue.Ticks, dbValue.Offset);
			}
			catch (Exception ex)
			{
				throw new FormatException(string.Format("Input string '{0}' was not in the correct format.", rs[index]), ex);
			}
		}

		public override object Get(DbDataReader rs, string name, ISessionImplementor session)
		{
			return Get(rs, rs.GetOrdinal(name), session);
		}

		public object Next(object current, ISessionImplementor session)
		{
			return Seed(session);
		}

		public object Seed(ISessionImplementor session)
		{
			return DateTimeOffset.Now;
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

			var date1 = (DateTimeOffset) x;
			var date2 = (DateTimeOffset) y;

			return date1.Equals(date2);
		}

		public object StringToObject(string xml)
		{
			return string.IsNullOrEmpty(xml) ? null : FromStringValue(xml);
		}

		public override string ToString(object val)
		{
			return ((DateTimeOffset) val).ToString();
		}

		public override object FromStringValue(string xml)
		{
			return DateTimeOffset.Parse(xml);
		}

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return "'" + ((DateTimeOffset) value) + "'";
		}
	}
}
