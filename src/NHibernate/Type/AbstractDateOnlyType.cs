#if NET6_0_OR_GREATER
using System;
using System.Data.Common;
using System.Globalization;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Base class for DateOnly types.
	/// </summary>
	[Serializable]
	public abstract class AbstractDateOnlyType<TParameter> : PrimitiveType
	{
		protected AbstractDateOnlyType(SqlType sqlType) : base(sqlType)
		{
		}

		public override string Name =>
			GetType().Name.EndsWith("Type", StringComparison.Ordinal)
				? GetType().Name[..^4]
				: GetType().Name;

		public override System.Type ReturnedClass => typeof(DateOnly);

		public override System.Type PrimitiveClass => typeof(DateOnly);

		public override object DefaultValue => DateOnly.MinValue;

		/// <inheritdoc />
		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			try
			{
				return GetDateOnlyFromReader(rs, index, session);
			}
			catch (Exception ex) when (ex is not FormatException)
			{
				throw new FormatException(string.Format("Input string '{0}' was not in the correct format.", rs[index]), ex);
			}
		}

		///// <inheritdoc />
		//public override object Get(DbDataReader rs, string name, ISessionImplementor session)
		//{
		//	return Get(rs, rs.GetOrdinal(name), session);
		//}

		/// <inheritdoc />
		public override void Set(DbCommand st, object value, int index, ISessionImplementor session)
		{
			st.Parameters[index].Value = GetParameterValueToSet((DateOnly) value,session);
		}

		/// <summary>
		/// Get the DateOnly value from the <see cref="DbDataReader"/> at index <paramref name="index"/>
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="index"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		protected abstract DateOnly GetDateOnlyFromReader(DbDataReader rs, int index, ISessionImplementor session);

		/// <summary>
		/// Convert <paramref name="dateOnly"/> into the <typeparamref name="TParameter"/> which will be set on the parameter
		/// </summary>
		/// <param name="dateOnly"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		protected abstract TParameter GetParameterValueToSet(DateOnly dateOnly, ISessionImplementor session);

		public override bool IsEqual(object x, object y)
		{
			if (ReferenceEquals(x, y)) return true;
			if (x == null || y == null) return false;
			return ((DateOnly) x).Equals((DateOnly) y);
		}

		public override int GetHashCode(object x) => ((DateOnly) x).GetHashCode();

		/// <inheritdoc />
		public override string ToLoggableString(object value, ISessionFactoryImplementor factory) =>
			value == null ? null : ((DateOnly) value).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

	}
}
#endif
