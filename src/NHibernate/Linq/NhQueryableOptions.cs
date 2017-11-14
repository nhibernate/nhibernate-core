namespace NHibernate.Linq
{
	public class NhQueryableOptions: IQueryableOptions
	{
		protected internal bool? Cacheable { get; private set; }
		protected internal CacheMode? CacheMode{ get; private set; }
		protected internal string CacheRegion { get; private set; }
		protected internal int? Timeout { get; private set; }

		public IQueryableOptions SetCacheable(bool cacheable)
		{
			Cacheable = cacheable;
			return this;
		}

		public IQueryableOptions SetCacheMode(CacheMode cacheMode)
		{
			CacheMode = cacheMode;
			return this;
		}

		public IQueryableOptions SetCacheRegion(string cacheRegion)
		{
			CacheRegion = cacheRegion;
			return this;
		}

		public IQueryableOptions SetTimeout(int timeout)
		{
			Timeout = timeout;
			return this;
		}

		internal NhQueryableOptions Clone()
		{
			return new NhQueryableOptions
			{
				Cacheable = Cacheable,
				CacheMode = CacheMode,
				CacheRegion = CacheRegion,
				Timeout = Timeout
			};
		}
	}
}
