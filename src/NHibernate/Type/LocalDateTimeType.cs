using System;

namespace NHibernate.Type
{
	[Serializable]
	public class LocalDateTimeType : DateTimeType
	{
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
