using System;

namespace NHibernate.Type
{
	[Serializable]
	public class UtcDateTimeType : DateTimeType
	{
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
