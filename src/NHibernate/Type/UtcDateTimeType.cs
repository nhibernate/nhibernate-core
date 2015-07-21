using System;

namespace NHibernate.Type
{
	[Serializable]
	public class UtcDateTimeType : AbstractDateTimeSpecificKindType
	{
		protected override DateTimeKind DateTimeKind
		{
			get { return DateTimeKind.Utc; }
		}

		public override string Name
		{
			get { return "UtcDateTime"; }
		}
	}
}