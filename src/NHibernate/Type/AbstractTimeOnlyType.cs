#if NET6_0_OR_GREATER
using System;
using System.Data.Common;
using System.Globalization;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Base class for TimeOnly types.
	/// Fractional seconds precision:
	///   null  => preserve full precision (no truncation)
	///   0..6  => truncate (floor) to that many fractional digits
	///   >=7   => preserve full precision
	/// </summary>
	[Serializable]
	public abstract class AbstractTimeOnlyType<TParameter> : PrimitiveType
	{
		private static readonly int[] TicksPerPrecision = { 10_000_000, 1_000_000, 100_000, 10_000, 1_000, 100, 10 };
		private readonly int _ticksForPrecision;
		private readonly bool _truncate;
		private readonly byte? _fractionalSecondsPrecision;

		protected AbstractTimeOnlyType(byte? fractionalSecondsPrecision, SqlType sqlType) : base(sqlType)
		{
			_fractionalSecondsPrecision = fractionalSecondsPrecision;
			if (fractionalSecondsPrecision is >= 0 and <= 6)
			{
				_ticksForPrecision = TicksPerPrecision[fractionalSecondsPrecision.Value];
				_truncate = true;
			}
			else
			{
				// null or >=7 => full precision, no truncation
				_ticksForPrecision = 1;
				_truncate = false;
			}
		}

		public override string Name => GetType().Name.EndsWith("Type", StringComparison.Ordinal)
			? GetType().Name[..^4]
			: GetType().Name;

		public override System.Type ReturnedClass => typeof(TimeOnly);

		public override System.Type PrimitiveClass => typeof(TimeOnly);

		public override object DefaultValue => TimeOnly.MinValue;

		/// <summary>
		/// Truncate (floor) fractional seconds according to the declared precision.
		/// Override to change behavior (e.g., implement rounding).
		/// </summary>
		protected virtual TimeOnly AdjustTimeOnly(TimeOnly timeOnly)
		{
			if (!_truncate)
				return timeOnly;

			long ticks = timeOnly.Ticks;
			long remainder = ticks % _ticksForPrecision;
			if (remainder == 0)
				return timeOnly;

			return new TimeOnly(ticks - remainder);
		}

		/// <summary>
		/// Convert <paramref name="timeOnly"/> into the <typeparamref name="TParameter"/> which will be set on the parameter
		/// </summary>
		/// <param name="timeOnly"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		protected abstract TParameter GetParameterValueToSet(TimeOnly timeOnly, ISessionImplementor session);

		///<inheritdoc/>
		public override void Set(DbCommand st, object value, int index, ISessionImplementor session)
		{
			st.Parameters[index].Value = GetParameterValueToSet(AdjustTimeOnly((TimeOnly) value), session);
		}

		///<inheritdoc/>
		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			try
			{
				return AdjustTimeOnly(GetTimeOnlyFromReader(rs, index, session));
			}
			catch (Exception ex) when (ex is not FormatException)
			{
				throw new FormatException(string.Format("Input '{0}' was not convertible to TimeOnly.", rs[index]), ex);
			}
		}

		/// <summary>
		/// Get the <see cref="TimeOnly"/> value from the <see cref="DbDataReader"/> at index <paramref name="index"/>
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="index"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		protected abstract TimeOnly GetTimeOnlyFromReader(DbDataReader rs, int index, ISessionImplementor session);

		public override bool IsEqual(object x, object y)
		{
			if (ReferenceEquals(x, y)) return true;
			if (x == null || y == null) return false;

			var t1 = (TimeOnly) x;
			var t2 = (TimeOnly) y;

			// Fast path: compare essential components first
			if (t1.Hour != t2.Hour || t1.Minute != t2.Minute || t1.Second != t2.Second)
				return false;

			// Precision handling
			if (!_fractionalSecondsPrecision.HasValue)
				return t1 == t2; // full precision exact match

			if (_fractionalSecondsPrecision == 0)
				return true; // up to seconds already matched

			return AdjustTimeOnly(t1) == AdjustTimeOnly(t2);
		}

		public override int GetHashCode(object x) => AdjustTimeOnly((TimeOnly) x).GetHashCode();

		/// <inheritdoc />
		public override string ToLoggableString(object value, ISessionFactoryImplementor factory) =>
			value == null ? null : ((TimeOnly) value).ToString("HH:mm:ss.fffffff", CultureInfo.InvariantCulture);
	}
}
#endif
