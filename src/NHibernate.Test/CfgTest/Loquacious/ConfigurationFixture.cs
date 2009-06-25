using System.Data;
using NHibernate.AdoNet;
using NHibernate.ByteCode.LinFu;
using NHibernate.Cache;
using NHibernate.Cfg.Loquacious;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Exceptions;
using NHibernate.Hql.Classic;
using NHibernate.Type;

namespace NHibernate.Test.CfgTest.Loquacious
{
	public class ConfigurationFixture
	{
		public void ProofOfConcept()
		{
			// Here I'm configuring near all properties outside the scope of Configuration class
			// Using the Configuration class the user can add mappings and configure listeners
			IFluentSessionFactoryConfiguration sfc= null;
			sfc.Named("SomeName")
				.Integrate
					.Using<MsSql2000Dialect>()
					.AutoQuoteKeywords()
					.BatchingQueries
						.Trough<SqlClientBatchingBatcherFactory>()
						.Each(10)
					.Connected
						.Trough<DebugConnectionProvider>()
						.Through<SqlClientDriver>()
						.Releasing(ConnectionReleaseMode.AfterTransaction)
						.With(IsolationLevel.ReadCommitted)
						.Using("The connection string but it has some overload")
					.CreateCommands
						.AutoCommentingSql()
						.ConvertingExpetionsTrough<SQLStateConverter>()
						.Preparing()
						.WithTimeout(10)
						.WithMaximumDepthOfOuterJoinFetching(10)
						.WithHqlToSqlSubstitutions("true 1, false 0, yes 'Y', no 'N'")
					.Schema
						.Validating()
			;
			sfc.Caching
					.Trough<HashtableCacheProvider>()
					.PrefixingRegionsWith("xyz")
					.Queries
						.Trough<StandardQueryCache>()
					.UsingMinimalPuts()
					.WithDefaultExpiration(15)
				.GeneratingCollections
					.Trough<DefaultCollectionTypeFactory>()
				.Proxy
					.DisableValidation()
					.Trough<ProxyFactoryFactory>()
				.ParsingHqlThrough<ClassicQueryTranslatorFactory>()
				.Mapping
					.UsingDefaultCatalog("MyCatalog")
					.UsingDefaultSchema("MySche")
			;
		}

		public void ProofOfConceptMinimalConfiguration()
		{
			// This is a possible minimal configuration
			// in this case we must define best default properties for each dialect
			// The place where put default properties values is the Dialect itself.
			IFluentSessionFactoryConfiguration sfc = null;
			sfc
				.Proxy.Trough<ProxyFactoryFactory>()
				.Integrate.Using<MsSql2005Dialect>();
		}
	}
}