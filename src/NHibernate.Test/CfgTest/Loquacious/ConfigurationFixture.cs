using System.Data;
using System.Data.SqlClient;
using NHibernate.AdoNet;
using NHibernate.ByteCode.LinFu;
using NHibernate.Cache;
using NHibernate.Cfg;
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
			var cfg = new Configuration();
			cfg.SessionFactory().Named("SomeName")
				.Caching
					.Through<HashtableCacheProvider>()
					.PrefixingRegionsWith("xyz")
					.Queries
						.Through<StandardQueryCache>()
					.UsingMinimalPuts()
					.WithDefaultExpiration(15)
				.GeneratingCollections
					.Through<DefaultCollectionTypeFactory>()
				.Proxy
					.DisableValidation()
					.Through<ProxyFactoryFactory>()
				.ParsingHqlThrough<ClassicQueryTranslatorFactory>()
				.Mapping
					.UsingDefaultCatalog("MyCatalog")
					.UsingDefaultSchema("MySche")
				.Integrate
					.Using<MsSql2000Dialect>()
					.AutoQuoteKeywords()
					.BatchingQueries
						.Through<SqlClientBatchingBatcherFactory>()
						.Each(10)
					.Connected
						.Through<DebugConnectionProvider>()
						.By<SqlClientDriver>()
						.Releasing(ConnectionReleaseMode.AfterTransaction)
						.With(IsolationLevel.ReadCommitted)
						.Using("The connection string but it has some overload")
					.CreateCommands
						.AutoCommentingSql()
						.ConvertingExceptionsThrough<SQLStateConverter>()
						.Preparing()
						.WithTimeout(10)
						.WithMaximumDepthOfOuterJoinFetching(10)
						.WithHqlToSqlSubstitutions("true 1, false 0, yes 'Y', no 'N'")
					.Schema
						.Validating()
			;

		}

		public void ProofOfConceptMinimalConfiguration()
		{
			// This is a possible minimal configuration
			// in this case we must define best default properties for each dialect
			// The place where put default properties values is the Dialect itself.
			var cfg = new Configuration();
			cfg.SessionFactory()
				.Proxy.Through<ProxyFactoryFactory>()
				.Integrate
					.Using<MsSql2005Dialect>()
					.Connected
						.Using(new SqlConnectionStringBuilder
						       	{
						       		DataSource = "(local)", 
											InitialCatalog = "nhibernate", 
											IntegratedSecurity = true
						       	});
		}
	}
}