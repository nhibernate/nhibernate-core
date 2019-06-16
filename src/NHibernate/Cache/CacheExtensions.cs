using System;

namespace NHibernate.Cache
{
	//6.0 TODO: Remove
	internal static class CacheExtensions
	{
#pragma warning disable 618
		public static CacheBase AsCacheBase(this ICache cache)
#pragma warning restore 618
		{
			if (cache == null) throw new ArgumentNullException(nameof(cache));
			return cache as CacheBase ?? new ObsoleteCacheWrapper(cache);
		}
	}
}