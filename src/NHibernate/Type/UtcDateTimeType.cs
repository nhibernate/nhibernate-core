using System;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	[Serializable]
	public class UtcDateTimeType : DateTimeType
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public UtcDateTimeType()
		{
		}

		/// <summary>
		/// Constructor for specifying a datetime with a scale. Use <see cref="SqlTypeFactory.GetDateTime"/>.
		/// </summary>
		/// <param name="sqlType">The sql type to use for the type.</param>
		public UtcDateTimeType(DateTimeSqlType sqlType) : base(sqlType)
		{
		}

		/// <inheritdoc />
		protected override DateTimeKind Kind => DateTimeKind.Utc;

		/// <inheritdoc />
		public override string Name => "UtcDateTime";
	}

	[Serializable]
	public class UtcDateTimeNoMsType : DateTimeNoMsType
	{
		/// <inheritdoc />
		protected override DateTimeKind Kind => DateTimeKind.Utc;

		/// <inheritdoc />
		public override string Name => "UtcDateTimeNoMs";
	}
}
