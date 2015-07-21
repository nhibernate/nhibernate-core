using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode
{
	public abstract class LazyRelation
	{
		public static LazyRelation Proxy = new LazyProxy();
		public static LazyRelation NoProxy = new LazyNoProxy();
		public static LazyRelation NoLazy = new NoLazyRelation();

		public abstract HbmLaziness ToHbm();

		#region Nested type: LazyNoProxy

		private class LazyNoProxy : LazyRelation
		{
			public override HbmLaziness ToHbm()
			{
				return HbmLaziness.NoProxy;
			}
		}

		#endregion

		#region Nested type: LazyProxy

		private class LazyProxy : LazyRelation
		{
			public override HbmLaziness ToHbm()
			{
				return HbmLaziness.Proxy;
			}
		}

		#endregion

		#region Nested type: NoLazyRelation

		private class NoLazyRelation : LazyRelation
		{
			public override HbmLaziness ToHbm()
			{
				return HbmLaziness.False;
			}
		}

		#endregion
	}
}