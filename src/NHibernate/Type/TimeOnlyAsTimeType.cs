#if NET6_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.TimeOnly" /> Property to a <see cref="DbType.Time"/> column.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Some DB drivers will use a <see cref="TimeSpan"/> value for the <see cref="DbType.Time"/> column, whereas
	/// others will use a full <see cref="System.DateTime" />. In the latter case, the date part defaults to
	/// to "1753-01-01" - which should not matter because using this Type indicates that you don't care about the Date portion of the DateTime.
	/// However, if you need to adjust this base value, you can specify the parameter 'BaseDateValue' on the type mapping,
	/// using the date format 'yyyy-MM-dd'
	/// </para>
	/// </remarks>
	[Serializable]
	public class TimeOnlyAsTimeType : AbstractTimeOnlyType<object>, IParameterizedType
	{
		private DateTime BaseDateValue = new(1753, 01, 01);
		private readonly string _sqlFormat;
		
		/// <summary>
		/// Default constructor. Sets the fractional seconds precision (scale) to 0
		/// </summary>
		public TimeOnlyAsTimeType() : this(0)
		{
		}

		/// <summary>
		/// Constructor for specifying a fractional seconds precision (scale).
		/// </summary>
		/// <param name="fractionalSecondsPrecision">The fractional seconds precision. Any value beyond 7 is pointless, since it's the maximum precision allowed by .NET</param>
		public TimeOnlyAsTimeType(byte fractionalSecondsPrecision) : base(fractionalSecondsPrecision, SqlTypeFactory.GetTime(fractionalSecondsPrecision))
		{
			_sqlFormat = "HH:mm:ss" + (fractionalSecondsPrecision > 0 ? "." + new string('F', Math.Min(7, (int) fractionalSecondsPrecision)) : "");
		}

		protected override TimeOnly GetTimeOnlyFromReader(DbDataReader rs, int index, ISessionImplementor session)
		{
			if (rs.GetFieldType(index) == typeof(TimeSpan)) //For those dialects where DbType.Time means TimeSpan.
			{
				return new TimeOnly(((TimeSpan) rs[index]).Ticks);
			}

			var dbValue = rs.GetDateTime(index);
			return TimeOnly.FromTimeSpan(dbValue.TimeOfDay);
		}

		protected override object GetParameterValueToSet(TimeOnly timeOnly, ISessionImplementor session)
		{
			if (session.Factory.ConnectionProvider.Driver.RequiresTimeSpanForTime)
				return timeOnly.ToTimeSpan();
			else
				return BaseDateValue + timeOnly.ToTimeSpan();
		}

		void IParameterizedType.SetParameterValues(IDictionary<string, string> parameters)
		{
			if (parameters.TryGetValue("BaseDateValue", out var stringVal) && !string.IsNullOrEmpty(stringVal))
			{
				BaseDateValue = DateTime.ParseExact(stringVal, "yyyy-MM-dd", CultureInfo.InvariantCulture).Date;
			}
		}

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect) =>
			dialect.ToStringLiteral(AdjustTimeOnly((TimeOnly) value).ToString(_sqlFormat), SqlTypeFactory.GetAnsiString(50));
	}
}
#endif
