using System;
using NHibernate.Cache;

namespace NHibernate.Cfg.Loquacious
{
	public class CacheConfigurationProperties
#pragma warning disable 618
		: ICacheConfigurationProperties
#pragma warning restore 618
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

		[Obsolete("This method is invalid and should not be used. Use QueryCacheFactory method instead.", true)]
		public void QueryCache<TFactory>() where TFactory : IQueryCache
		{
			throw new InvalidOperationException("This method is invalid and should not be used. Use QueryCacheFactory method instead.");
		}

		#endregion

		public void QueryCacheFactory<TFactory>() where TFactory : IQueryCacheFactory
		{
			UseSecondLevelCache = true;
			UseQueryCache = true;
			cfg.SetProperty(Environment.QueryCacheFactory, typeof(TFactory).AssemblyQualifiedName);
		}

		private bool UseSecondLevelCache
		{
			set { cfg.SetProperty(Environment.UseSecondLevelCache, value.ToString().ToLowerInvariant()); }
		}
	}

	public class CacheConfiguration
#pragma warning disable 618
		: ICacheConfiguration
#pragma warning restore 618
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

		public CacheConfiguration Through<TProvider>() where TProvider : ICacheProvider
		{
			fc.Configuration.SetProperty(Environment.UseSecondLevelCache, "true");
			fc.Configuration.SetProperty(Environment.CacheProvider, typeof(TProvider).AssemblyQualifiedName);
			return this;
		}

		public CacheConfiguration PrefixingRegionsWith(string regionPrefix)
		{
			fc.Configuration.SetProperty(Environment.CacheRegionPrefix, regionPrefix);
			return this;
		}

		public CacheConfiguration UsingMinimalPuts()
		{
			fc.Configuration.SetProperty(Environment.UseMinimalPuts, true.ToString().ToLowerInvariant());
			return this;
		}

		public FluentSessionFactoryConfiguration WithDefaultExpiration(int seconds)
		{
			fc.Configuration.SetProperty(Environment.CacheDefaultExpiration, seconds.ToString());
			return fc;
		}

		public QueryCacheConfiguration Queries { get; }

#pragma warning disable 618
		#region Implementation of ICacheConfiguration

		ICacheConfiguration ICacheConfiguration.Through<TProvider>()
		{
			return Through<TProvider>();
		}

		ICacheConfiguration ICacheConfiguration.PrefixingRegionsWith(string regionPrefix)
		{
			return PrefixingRegionsWith(regionPrefix);
		}

		ICacheConfiguration ICacheConfiguration.UsingMinimalPuts()
		{
			return UsingMinimalPuts();
		}

		IFluentSessionFactoryConfiguration ICacheConfiguration.WithDefaultExpiration(int seconds)
		{
			return WithDefaultExpiration(seconds);
		}

		IQueryCacheConfiguration ICacheConfiguration.Queries => Queries;

		#endregion
#pragma warning restore 618
	}

	public class QueryCacheConfiguration
#pragma warning disable 618
		: IQueryCacheConfiguration
#pragma warning restore 618
	{
		private readonly CacheConfiguration cc;

		public QueryCacheConfiguration(CacheConfiguration cc)
		{
			this.cc = cc;
		}

		// 6.0 TODO: enable constraint and remove runtime type check
		public CacheConfiguration Through<TFactory>() //where TFactory : IQueryCacheFactory
		{
			if (!typeof(IQueryCacheFactory).IsAssignableFrom(typeof(TFactory)))
				throw new ArgumentException($"{nameof(TFactory)} must be an {nameof(IQueryCacheFactory)}", nameof(TFactory));

			cc.Configuration.SetProperty(Environment.UseSecondLevelCache, "true");
			cc.Configuration.SetProperty(Environment.UseQueryCache, "true");
			cc.Configuration.SetProperty(Environment.QueryCacheFactory, typeof(TFactory).AssemblyQualifiedName);
			return cc;
		}

		#region Implementation of IQueryCacheConfiguration
#pragma warning disable 618

		ICacheConfiguration IQueryCacheConfiguration.Through<TFactory>()
		{
			return Through<TFactory>();
		}

#pragma warning restore 618
		#endregion
	}
}
