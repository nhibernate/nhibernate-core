#if NET6_0_OR_GREATER
using System;
using System.Data;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="DateOnly" /> property to a <see cref="DbType.Date"/> column
	/// </summary>
	[Serializable]
	public class DateOnlyAsDateType : AbstractDateOnlyType<DateTime>
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public DateOnlyAsDateType() : base(SqlTypeFactory.Date)
		{
		}

		protected override DateOnly GetDateOnlyFromReader(DbDataReader rs, int index, ISessionImplementor session)
		{
			return DateOnly.FromDateTime(rs.GetDateTime(index));
		}

		protected override DateTime GetParameterValueToSet(DateOnly dateOnly, ISessionImplementor session) =>
			dateOnly.ToDateTime(TimeOnly.MinValue);


		/// <inheritdoc />
		public override string ObjectToSQLString(object value, Dialect.Dialect dialect) =>
			"'" + ((DateOnly) value).ToString("yyyy-MM-dd") + "'";
	}
}
#endif
