using System;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Cfg.XmlHbmBinding
{
	internal static class CacheModeConverter
	{
		public static CacheMode? GetCacheMode(HbmCacheMode cacheMode)
		{
			switch (cacheMode)
			{
				case HbmCacheMode.Get:
					return CacheMode.Get;
				case HbmCacheMode.Ignore:
					return CacheMode.Ignore;
				case HbmCacheMode.Normal:
					return CacheMode.Normal;
				case HbmCacheMode.Put:
					return CacheMode.Put;
				case HbmCacheMode.Refresh:
					return CacheMode.Refresh;
				default:
					throw new ArgumentOutOfRangeException("cacheMode");
			}
		}
	}
}
