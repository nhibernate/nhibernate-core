using System;

using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public abstract class QueryBinder : Binder
	{
		public QueryBinder(Mappings mappings)
			: base(mappings)
		{
		}

		public QueryBinder(Binder parent)
			: base(parent)
		{
		}

		protected static FlushMode GetFlushMode(bool flushModeSpecified, HbmFlushMode flushMode)
		{
			if (!flushModeSpecified)
				return FlushMode.Unspecified;

			switch (flushMode)
			{
				case HbmFlushMode.Auto:
					return FlushMode.Auto;

				case HbmFlushMode.Never:
					return FlushMode.Never;

				default:
					throw new ArgumentOutOfRangeException("flushMode");
			}
		}
	}
}