using NHibernate.Cache;

namespace NHibernate.Cfg.Loquacious
{
	internal class CacheConfigurationProperties : ICacheConfigurationProperties
	{
		private readonly Configuration cfg;

		public CacheConfigurationProperties(Configuration cfg)
		{
			this.cfg = cfg;
		}

		#region Implementation of ICacheConfigurationProperties

		public bool UseMinimalPuts
		{
			set { cfg.SetProperty(Environment.UseMinimalPuts, value.ToString().ToLowerInvariant()); }
		}

		public bool UseQueryCache
		{
			set { cfg.SetProperty(Environment.UseQueryCache, value.ToString().ToLowerInvariant()); }
		}

		public string RegionsPrefix
		{
			set { cfg.SetProperty(Environment.CacheRegionPrefix, value); }
		}

		public int DefaultExpiration
		{
			set { cfg.SetProperty(Environment.CacheDefaultExpiration, value.ToString()); }
		}

		public void Provider<TProvider>() where TProvider : ICacheProvider
		{
			UseSecondLevelCache = true;
			cfg.SetProperty(Environment.CacheProvider, typeof(TProvider).AssemblyQualifiedName);
		}

		public void QueryCache<TFactory>() where TFactory : IQueryCache
		{
			UseSecondLevelCache = true;
			UseQueryCache = true;
			cfg.SetProperty(Environment.QueryCacheFactory, typeof(TFactory).AssemblyQualifiedName);
		}

		private bool UseSecondLevelCache
		{
			set { cfg.SetProperty(Environment.UseSecondLevelCache, value.ToString().ToLowerInvariant()); }
		}
		#endregion
	}

	internal class CacheConfiguration : ICacheConfiguration
	{
		private readonly FluentSessionFactoryConfiguration fc;

		public CacheConfiguration(FluentSessionFactoryConfiguration parent)
		{
			fc = parent;
			Queries = new QueryCacheConfiguration(this);
		}

		internal Configuration Configuration
		{
			get { return fc.Configuration; }
		}

		#region Implementation of ICacheConfiguration

		public ICacheConfiguration Through<TProvider>() where TProvider : ICacheProvider
		{
			fc.Configuration.SetProperty(Environment.UseSecondLevelCache, "true");
			fc.Configuration.SetProperty(Environment.CacheProvider, typeof(TProvider).AssemblyQualifiedName);
			return this;
		}

		public ICacheConfiguration PrefixingRegionsWith(string regionPrefix)
		{
			fc.Configuration.SetProperty(Environment.CacheRegionPrefix, regionPrefix);
			return this;
		}

		public ICacheConfiguration UsingMinimalPuts()
		{
			fc.Configuration.SetProperty(Environment.UseMinimalPuts, true.ToString().ToLowerInvariant());
			return this;
		}

		public IFluentSessionFactoryConfiguration WithDefaultExpiration(int seconds)
		{
			fc.Configuration.SetProperty(Environment.CacheDefaultExpiration, seconds.ToString());
			return fc;
		}

		public IQueryCacheConfiguration Queries { get; private set; }

		#endregion
	}

	internal class QueryCacheConfiguration : IQueryCacheConfiguration
	{
		private readonly CacheConfiguration cc;

		public QueryCacheConfiguration(CacheConfiguration cc)
		{
			this.cc = cc;
		}

		#region Implementation of IQueryCacheConfiguration

		public ICacheConfiguration Through<TFactory>() where TFactory : IQueryCache
		{
			cc.Configuration.SetProperty(Environment.UseSecondLevelCache, "true");
			cc.Configuration.SetProperty(Environment.UseQueryCache, "true");
			cc.Configuration.SetProperty(Environment.QueryCacheFactory, typeof(TFactory).AssemblyQualifiedName);
			return cc;
		}

		#endregion
	}
}