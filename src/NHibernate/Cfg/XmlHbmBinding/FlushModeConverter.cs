using System;

using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Cfg.XmlHbmBinding
{
	internal static class FlushModeConverter
	{
		public static FlushMode GetFlushMode(HbmQuery querySchema)
		{
			return GetFlushMode(querySchema.flushmodeSpecified, querySchema.flushmode);
		}

		public static FlushMode GetFlushMode(HbmSqlQuery querySchema)
		{
			return GetFlushMode(querySchema.flushmodeSpecified, querySchema.flushmode);
		}

		private static FlushMode GetFlushMode(bool flushModeSpecified, HbmFlushMode flushMode)
		{
			if (!flushModeSpecified)
				return FlushMode.Unspecified;

			switch (flushMode)
			{
				case HbmFlushMode.Auto:
					return FlushMode.Auto;

				case HbmFlushMode.Manual:
				case HbmFlushMode.Never:
					return FlushMode.Manual;

				case HbmFlushMode.Always:
					return FlushMode.Always;

				default:
					throw new ArgumentOutOfRangeException("flushMode");
			}
		}
	}
}