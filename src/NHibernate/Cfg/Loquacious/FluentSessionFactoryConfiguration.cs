using NHibernate.Bytecode;
using NHibernate.Hql;
using NHibernate.Linq;

namespace NHibernate.Cfg.Loquacious
{
	internal class FluentSessionFactoryConfiguration : IFluentSessionFactoryConfiguration
	{
		private readonly Configuration configuration;

		public FluentSessionFactoryConfiguration(Configuration configuration)
		{
			this.configuration = configuration;
			Integrate = new DbIntegrationConfiguration(configuration);
			Caching = new CacheConfiguration(this);
			Proxy = new ProxyConfiguration(this);
			GeneratingCollections = new CollectionFactoryConfiguration(this);
			Mapping = new MappingsConfiguration(this);
		}

		internal Configuration Configuration
		{
			get { return configuration; }
		}

		#region Implementation of IFluentSessionFactoryConfiguration

		public IFluentSessionFactoryConfiguration Named(string sessionFactoryName)
		{
			configuration.SetProperty(Environment.SessionFactoryName, sessionFactoryName);
			return this;
		}

		public IDbIntegrationConfiguration Integrate { get; private set; }

		public ICacheConfiguration Caching { get; private set; }

		public IFluentSessionFactoryConfiguration GenerateStatistics()
		{
			configuration.SetProperty(Environment.GenerateStatistics, "true");
			return this;
		}

		public IFluentSessionFactoryConfiguration DefaultFlushMode(FlushMode flushMode)
		{
			configuration.SetProperty(Environment.DefaultFlushMode, flushMode.ToString());
			return this;
		}

		public IFluentSessionFactoryConfiguration ParsingHqlThrough<TQueryTranslator>()
			where TQueryTranslator : IQueryTranslatorFactory
		{
			configuration.SetProperty(Environment.QueryTranslator, typeof (TQueryTranslator).AssemblyQualifiedName);
			return this;
		}

		public IFluentSessionFactoryConfiguration ParsingLinqThrough<TQueryProvider>()
			where TQueryProvider : INhQueryProvider
		{
			configuration.SetProperty(Environment.QueryLinqProvider, typeof(TQueryProvider).AssemblyQualifiedName);
			return this;
		}

		public IProxyConfiguration Proxy { get; private set; }
		public ICollectionFactoryConfiguration GeneratingCollections { get; private set; }
		public IMappingsConfiguration Mapping { get; private set; }

		#endregion
	}

	internal class CollectionFactoryConfiguration : ICollectionFactoryConfiguration
	{
		private readonly FluentSessionFactoryConfiguration fc;

		public CollectionFactoryConfiguration(FluentSessionFactoryConfiguration parent)
		{
			fc = parent;
		}

		#region Implementation of ICollectionFactoryConfiguration

		public IFluentSessionFactoryConfiguration Through<TCollecionsFactory>()
			where TCollecionsFactory : ICollectionTypeFactory
		{
			fc.Configuration.SetProperty(Environment.CollectionTypeFactoryClass,
			                             typeof (TCollecionsFactory).AssemblyQualifiedName);
			return fc;
		}

		#endregion
	}
}