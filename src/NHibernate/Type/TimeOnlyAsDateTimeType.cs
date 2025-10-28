#if NET6_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.TimeOnly" /> Property to a DateTime column that only stores the
	/// time part of the DateTime as significant.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This defaults the Date to "1753-01-01" - which should not matter because
	/// using this Type indicates that you don't care about the Date portion of the DateTime.
	/// However, if you need to adjust this base value, you can specify the parameter 'BaseDateValue' on the type mapping,
	/// using the date format 'yyyy-MM-dd'
	/// </para>
	/// </remarks>
	[Serializable]
	public class TimeOnlyAsDateTimeType : AbstractTimeOnlyType<DateTime>, IParameterizedType
	{
		private DateTime _baseDateValue = new(1753, 01, 01);
		private readonly string _sqlFormat;
		/// <summary>
		/// Default constructor. Sets the fractional seconds precision (scale) to 0
		/// </summary>
		public TimeOnlyAsDateTimeType() : this(0)
		{
		}

		/// <summary>
		/// Constructor for specifying a time with a scale.
		/// </summary>
		/// <param name="fractionalSecondsPrecision">The sql type to use for the type.</param>
		public TimeOnlyAsDateTimeType(byte fractionalSecondsPrecision) : base(fractionalSecondsPrecision, SqlTypeFactory.GetDateTime(fractionalSecondsPrecision))
		{
			_sqlFormat = "yyyy-MM-dd HH:mm:ss" + (fractionalSecondsPrecision > 0 ? "." + new string('F', Math.Min(7, (int) fractionalSecondsPrecision)) : "");
		}

		protected override TimeOnly GetTimeOnlyFromReader(DbDataReader rs, int index, ISessionImplementor session) => 
			TimeOnly.FromTimeSpan(rs.GetDateTime(index).TimeOfDay);

		protected override DateTime GetParameterValueToSet(TimeOnly timeOnly, ISessionImplementor session) => 
			GetDateTimeFromTimeOnly(timeOnly);

		private DateTime GetDateTimeFromTimeOnly(TimeOnly timeOnly) => 
			_baseDateValue + timeOnly.ToTimeSpan();

		void IParameterizedType.SetParameterValues(IDictionary<string, string> parameters)
		{
			if (parameters?.TryGetValue("BaseDateValue", out var stringVal) == true
				&& !string.IsNullOrEmpty(stringVal))
			{
				_baseDateValue = DateTime.ParseExact(stringVal, "yyyy-MM-dd", CultureInfo.InvariantCulture).Date;
			}
		}

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect) =>
			"'" + GetDateTimeFromTimeOnly(AdjustTimeOnly((TimeOnly) value)).ToString(_sqlFormat) + "'";
	}
}
#endif
