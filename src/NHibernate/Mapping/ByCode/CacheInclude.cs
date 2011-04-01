using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode
{
	public abstract class CacheInclude
	{
		public static CacheInclude All = new AllCacheInclude();
		public static CacheInclude NonLazy = new NonLazyCacheInclude();

		public abstract HbmCacheInclude ToHbm();

		#region Nested type: AllCacheInclude

		public class AllCacheInclude : CacheInclude
		{
			public override HbmCacheInclude ToHbm()
			{
				return HbmCacheInclude.All;
			}
		}

		#endregion

		#region Nested type: NonLazyCacheInclude

		public class NonLazyCacheInclude : CacheInclude
		{
			public override HbmCacheInclude ToHbm()
			{
				return HbmCacheInclude.NonLazy;
			}
		}

		#endregion
	}
}