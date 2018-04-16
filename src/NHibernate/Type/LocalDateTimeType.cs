using System;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	[Serializable]
	public class LocalDateTimeType : DateTimeType
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public LocalDateTimeType()
		{
		}

		/// <summary>
		/// Constructor for specifying a datetime with a scale. Use <see cref="SqlTypeFactory.GetDateTime"/>.
		/// </summary>
		/// <param name="sqlType">The sql type to use for the type.</param>
		public LocalDateTimeType(DateTimeSqlType sqlType) : base(sqlType)
		{
		}

		/// <inheritdoc />
		protected override DateTimeKind Kind => DateTimeKind.Local;

		/// <inheritdoc />
		public override string Name => "LocalDateTime";
	}

	[Serializable]
	public class LocalDateTimeNoMsType : DateTimeNoMsType
	{
		/// <inheritdoc />
		protected override DateTimeKind Kind => DateTimeKind.Local;

		/// <inheritdoc />
		public override string Name => "LocalDateTimeNoMs";
	}
}
