using NHibernate.Bytecode;
using NHibernate.Hql;
using NHibernate.Linq;

namespace NHibernate.Cfg.Loquacious
{
	public class FluentSessionFactoryConfiguration 
#pragma warning disable 618
		: IFluentSessionFactoryConfiguration
#pragma warning restore 618
	{
		private readonly Configuration _configuration;

		public FluentSessionFactoryConfiguration(Configuration configuration)
		{
			_configuration = configuration;
			Integrate = new DbIntegrationConfiguration(configuration);
			Caching = new CacheConfiguration(this);
			Proxy = new ProxyConfiguration(this);
			GeneratingCollections = new CollectionFactoryConfiguration(this);
			Mapping = new MappingsConfiguration(this);
		}

		internal Configuration Configuration
		{
			get { return _configuration; }
		}

		/// <summary>
		/// Set the SessionFactory mnemonic name.
		/// </summary>
		/// <param name="sessionFactoryName">The mnemonic name.</param>
		/// <returns>The fluent configuration itself.</returns>
		/// <remarks>
		/// The SessionFactory mnemonic name can be used as a surrogate key in a multi-DB application. 
		/// </remarks>
		public FluentSessionFactoryConfiguration Named(string sessionFactoryName)
		{
			_configuration.SetProperty(Environment.SessionFactoryName, sessionFactoryName);
			return this;
		}

		/// <summary>
		/// DataBase integration configuration.
		/// </summary>
		public DbIntegrationConfiguration Integrate { get; }

		/// <summary>
		/// Cache configuration.
		/// </summary>
		public CacheConfiguration Caching { get; }

		public FluentSessionFactoryConfiguration GenerateStatistics()
		{
			_configuration.SetProperty(Environment.GenerateStatistics, "true");
			return this;
		}

		public FluentSessionFactoryConfiguration DefaultFlushMode(FlushMode flushMode)
		{
			_configuration.SetProperty(Environment.DefaultFlushMode, flushMode.ToString());
			return this;
		}

		public FluentSessionFactoryConfiguration ParsingHqlThrough<TQueryTranslator>()
			where TQueryTranslator : IQueryTranslatorFactory
		{
			_configuration.SetProperty(Environment.QueryTranslator, typeof (TQueryTranslator).AssemblyQualifiedName);
			return this;
		}

		public FluentSessionFactoryConfiguration ParsingLinqThrough<TQueryProvider>()
			where TQueryProvider : INhQueryProvider
		{
			_configuration.SetProperty(Environment.QueryLinqProvider, typeof(TQueryProvider).AssemblyQualifiedName);
			return this;
		}

		public ProxyConfiguration Proxy { get; }
		public CollectionFactoryConfiguration GeneratingCollections { get; }
		public MappingsConfiguration Mapping { get; }
#pragma warning disable 618
		#region Implementation of IFluentSessionFactoryConfiguration

		IFluentSessionFactoryConfiguration IFluentSessionFactoryConfiguration.Named(string sessionFactoryName)

		{
			_configuration.SetProperty(Environment.SessionFactoryName, sessionFactoryName);
			return this;
		}

		IDbIntegrationConfiguration IFluentSessionFactoryConfiguration.Integrate
		{
			get { return Integrate; }
		}

		ICacheConfiguration IFluentSessionFactoryConfiguration.Caching
		{
			get { return Caching; }
		}

		IFluentSessionFactoryConfiguration IFluentSessionFactoryConfiguration.GenerateStatistics()
		{
			_configuration.SetProperty(Environment.GenerateStatistics, "true");
			return this;
		}

		IFluentSessionFactoryConfiguration IFluentSessionFactoryConfiguration.DefaultFlushMode(FlushMode flushMode)
		{
			_configuration.SetProperty(Environment.DefaultFlushMode, flushMode.ToString());
			return this;
		}

		IFluentSessionFactoryConfiguration IFluentSessionFactoryConfiguration.ParsingHqlThrough<TQueryTranslator>()
		{
			_configuration.SetProperty(Environment.QueryTranslator, typeof (TQueryTranslator).AssemblyQualifiedName);
			return this;
		}

		IFluentSessionFactoryConfiguration IFluentSessionFactoryConfiguration.ParsingLinqThrough<TQueryProvider>()
		{
			_configuration.SetProperty(Environment.QueryLinqProvider, typeof(TQueryProvider).AssemblyQualifiedName);
			return this;
		}

		IProxyConfiguration IFluentSessionFactoryConfiguration.Proxy
		{
			get { return Proxy; }
		}

		ICollectionFactoryConfiguration IFluentSessionFactoryConfiguration.GeneratingCollections
		{
			get { return GeneratingCollections; }
		}

		IMappingsConfiguration IFluentSessionFactoryConfiguration.Mapping
		{
			get { return Mapping; }
		}

		#endregion
#pragma warning restore 618

	}

	public class CollectionFactoryConfiguration 
#pragma warning disable 618
		: ICollectionFactoryConfiguration
#pragma warning restore 618
	{
		private readonly FluentSessionFactoryConfiguration fc;

		public CollectionFactoryConfiguration(FluentSessionFactoryConfiguration parent)
		{
			fc = parent;
		}

		public FluentSessionFactoryConfiguration Through<TCollectionsFactory>()
			where TCollectionsFactory : ICollectionTypeFactory
		{
			fc.Configuration.SetProperty(Environment.CollectionTypeFactoryClass,
										typeof (TCollectionsFactory).AssemblyQualifiedName);
			return fc;
		}

#pragma warning disable 618
		#region Implementation of ICollectionFactoryConfiguration

		IFluentSessionFactoryConfiguration ICollectionFactoryConfiguration.Through<TCollecionsFactory>()
		{
			fc.Configuration.SetProperty(Environment.CollectionTypeFactoryClass,
			                             typeof (TCollecionsFactory).AssemblyQualifiedName);
			return fc;
		}

		#endregion
#pragma warning restore 618
	}
}
