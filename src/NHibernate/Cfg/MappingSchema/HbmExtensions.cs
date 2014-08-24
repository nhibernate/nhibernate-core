using System;
using NHibernate.Engine;

namespace NHibernate.Cfg.MappingSchema
{
	public static class HbmExtensions
	{
		public static Versioning.OptimisticLock ToOptimisticLock(this HbmOptimisticLockMode hbmOptimisticLockMode)
		{
			switch (hbmOptimisticLockMode)
			{
				case HbmOptimisticLockMode.None:
					return Versioning.OptimisticLock.None;
				case HbmOptimisticLockMode.Version:
					return Versioning.OptimisticLock.Version;
				case HbmOptimisticLockMode.Dirty:
					return Versioning.OptimisticLock.Dirty;
				case HbmOptimisticLockMode.All:
					return Versioning.OptimisticLock.All;
				default:
					return Versioning.OptimisticLock.Version;
			}
		}

		public static string ToNullValue(this HbmUnsavedValueType unsavedValueType)
		{
			switch (unsavedValueType)
			{
				case HbmUnsavedValueType.Undefined:
					return "undefined";
				case HbmUnsavedValueType.Any:
					return "any";
				case HbmUnsavedValueType.None:
					return "none";
				default:
					throw new ArgumentOutOfRangeException("unsavedValueType");
			}
		}

		public static string ToCacheConcurrencyStrategy(this HbmCacheUsage cacheUsage)
		{
			switch (cacheUsage)
			{
				case HbmCacheUsage.ReadOnly:
					return "read-only";
				case HbmCacheUsage.ReadWrite:
					return "read-write";
				case HbmCacheUsage.NonstrictReadWrite:
					return "nonstrict-read-write";
				case HbmCacheUsage.Transactional:
					return "transactional";
				default:
					throw new ArgumentOutOfRangeException("cacheUsage");
			}
		}

		public static CacheMode? ToCacheMode(this HbmCacheMode cacheMode)
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

		public static string JoinString(this string[] source)
		{
			if (source != null)
			{
				string result = string.Join(System.Environment.NewLine, source).Trim();
				return result.Length == 0 ? null : result;
			}
			return null;
		}
	}
}