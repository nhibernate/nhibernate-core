using NHibernate.Hql;
namespace NHibernate.Cfg.Loquacious
{
	public interface IFluentSessionFactoryConfiguration
	{
		/// <summary>
		/// Set the SessionFactory mnemonic name.
		/// </summary>
		/// <param name="sessionFactoryName">The mnemonic name.</param>
		/// <returns>The fluent configuration itself.</returns>
		/// <remarks>
		/// The SessionFactory mnemonic name can be used as a surrogate key in a multi-DB application. 
		/// </remarks>
		IFluentSessionFactoryConfiguration Named(string sessionFactoryName);

		/// <summary>
		/// DataBase integration configuration.
		/// </summary>
		IDbIntegrationConfiguration Integrate { get; }

		/// <summary>
		/// Cache configuration.
		/// </summary>
		ICacheConfiguration Caching { get; }

		IFluentSessionFactoryConfiguration GenerateStatistics();
		IFluentSessionFactoryConfiguration Using(EntityMode entityMode);
		IFluentSessionFactoryConfiguration ParsingHqlThrough<TQueryTranslator>() where TQueryTranslator : IQueryTranslatorFactory;

		IProxyConfiguration Proxy { get; }

		ICollectionFactoryConfiguration GeneratingCollections { get; }

		IMappingsConfiguration Mapping { get; }
	}
}