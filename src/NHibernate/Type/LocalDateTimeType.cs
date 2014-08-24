using System;

namespace NHibernate.Type
{
	[Serializable]
	public class LocalDateTimeType : AbstractDateTimeSpecificKindType
	{
		protected override DateTimeKind DateTimeKind
		{
			get { return DateTimeKind.Local; }
		}

		public override string Name
		{
			get { return "LocalDateTime"; }
		}
	}
}