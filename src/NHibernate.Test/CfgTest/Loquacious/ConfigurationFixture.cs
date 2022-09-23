using System.Data;
using System.Data.SqlClient;
using System.IO;
using NHibernate.AdoNet;
using NHibernate.Bytecode;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Exceptions;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.CfgTest.Loquacious
{
	[TestFixture]
	public class ConfigurationFixture
	{
		[Test]
		public void CompleteConfiguration()
		{
			const string connectionString = "The connection string";
			// Here I'm configuring near all properties outside the scope of Configuration class
			// Using the Configuration class the user can add mappings and configure listeners
			var cfg = new Configuration();
			cfg.SessionFactory().Named("SomeName")
				.Caching
					.Through<HashtableCacheProvider>()
					.PrefixingRegionsWith("xyz")
					.Queries
						.Through<StandardQueryCacheFactory>()
					.UsingMinimalPuts()
					.WithDefaultExpiration(15)
				.GeneratingCollections
					.Through<DefaultCollectionTypeFactory>()
				.Proxy
					.DisableValidation()
					.Through<StaticProxyFactoryFactory>()
				.ParsingHqlThrough<ASTQueryTranslatorFactory>()
				.Mapping
					.UsingDefaultCatalog("MyCatalog")
					.UsingDefaultSchema("MySche")
				.Integrate
					.Using<MsSql2000Dialect>()
					.AutoQuoteKeywords()
					.EnableLogFormattedSql()
#if NETFX
					.BatchingQueries
						.Through<SqlClientBatchingBatcherFactory>()
						.Each(15)
#endif
					.Connected
						.Through<DebugConnectionProvider>()
						.By<SqlClientDriver>()
						.Releasing(ConnectionReleaseMode.AfterTransaction)
						.With(IsolationLevel.ReadCommitted)
						.Using(connectionString)
					.CreateCommands
						.AutoCommentingSql()
						.ConvertingExceptionsThrough<SQLStateConverter>()
						.Preparing()
						.WithTimeout(10)
						.WithMaximumDepthOfOuterJoinFetching(11)
						.WithHqlToSqlSubstitutions("true 1, false 0, yes 'Y', no 'N'")
					.Schema.Validating()
					.Schema.ThrowOnSchemaUpdate(true);

			Assert.That(cfg.Properties[Environment.SessionFactoryName], Is.EqualTo("SomeName"));
			Assert.That(cfg.Properties[Environment.CacheProvider], Is.EqualTo(typeof(HashtableCacheProvider).AssemblyQualifiedName));
			Assert.That(cfg.Properties[Environment.CacheRegionPrefix], Is.EqualTo("xyz"));
			Assert.That(cfg.Properties[Environment.QueryCacheFactory], Is.EqualTo(typeof(StandardQueryCacheFactory).AssemblyQualifiedName));
			Assert.That(cfg.Properties[Environment.UseMinimalPuts], Is.EqualTo("true"));
			Assert.That(cfg.Properties[Environment.CacheDefaultExpiration], Is.EqualTo("15"));
			Assert.That(cfg.Properties[Environment.CollectionTypeFactoryClass], Is.EqualTo(typeof(DefaultCollectionTypeFactory).AssemblyQualifiedName));
			Assert.That(cfg.Properties[Environment.UseProxyValidator], Is.EqualTo("false"));
			Assert.That(cfg.Properties[Environment.ProxyFactoryFactoryClass], Is.EqualTo(typeof(StaticProxyFactoryFactory).AssemblyQualifiedName));
			Assert.That(cfg.Properties[Environment.QueryTranslator], Is.EqualTo(typeof(ASTQueryTranslatorFactory).AssemblyQualifiedName));
			Assert.That(cfg.Properties[Environment.DefaultCatalog], Is.EqualTo("MyCatalog"));
			Assert.That(cfg.Properties[Environment.DefaultSchema], Is.EqualTo("MySche"));
			Assert.That(cfg.Properties[Environment.Dialect], Is.EqualTo(typeof(MsSql2000Dialect).AssemblyQualifiedName));
			Assert.That(cfg.Properties[Environment.Hbm2ddlKeyWords], Is.EqualTo("auto-quote"));
			Assert.That(cfg.Properties[Environment.FormatSql], Is.EqualTo("true"));
#if NETFX
			Assert.That(cfg.Properties[Environment.BatchStrategy], Is.EqualTo(typeof(SqlClientBatchingBatcherFactory).AssemblyQualifiedName));
			Assert.That(cfg.Properties[Environment.BatchSize], Is.EqualTo("15"));
#endif
			Assert.That(cfg.Properties[Environment.ConnectionProvider], Is.EqualTo(typeof(DebugConnectionProvider).AssemblyQualifiedName));
			Assert.That(cfg.Properties[Environment.ConnectionDriver], Is.EqualTo(typeof(SqlClientDriver).AssemblyQualifiedName));
			Assert.That(cfg.Properties[Environment.ReleaseConnections], Is.EqualTo(ConnectionReleaseModeParser.ToString(ConnectionReleaseMode.AfterTransaction)));
			Assert.That(cfg.Properties[Environment.Isolation], Is.EqualTo("ReadCommitted"));
			Assert.That(cfg.Properties[Environment.ConnectionString], Is.EqualTo(connectionString));
			Assert.That(cfg.Properties[Environment.UseSqlComments], Is.EqualTo("true"));
			Assert.That(cfg.Properties[Environment.SqlExceptionConverter], Is.EqualTo(typeof(SQLStateConverter).AssemblyQualifiedName));
			Assert.That(cfg.Properties[Environment.PrepareSql], Is.EqualTo("true"));
			Assert.That(cfg.Properties[Environment.CommandTimeout], Is.EqualTo("10"));
			Assert.That(cfg.Properties[Environment.MaxFetchDepth], Is.EqualTo("11"));
			Assert.That(cfg.Properties[Environment.QuerySubstitutions], Is.EqualTo("true 1, false 0, yes 'Y', no 'N'"));
			Assert.That(cfg.Properties[Environment.Hbm2ddlAuto], Is.EqualTo("validate"));
			Assert.That(cfg.Properties[Environment.Hbm2ddlThrowOnUpdate], Is.EqualTo("true"));

			// Keywords import and auto-validation require a valid connection string, disable them before checking
			// the session factory can be built.
			cfg.SetProperty(Environment.Hbm2ddlKeyWords, "none");
			cfg.SetProperty(Environment.Hbm2ddlAuto, null);
			Assert.That(() => cfg.BuildSessionFactory().Dispose(), Throws.Nothing);
		}

		[Test]
		public void UseDbConfigurationStringBuilder()
		{
			// This is a possible minimal configuration
			// in this case we must define best default properties for each dialect
			// The place where put default properties values is the Dialect itself.
			var cfg = new Configuration();
			cfg.SessionFactory()
				.Proxy.Through<StaticProxyFactoryFactory>()
				.Integrate
					.Using<MsSql2005Dialect>()
					.Connected
						.Using(new SqlConnectionStringBuilder { DataSource = "(local)", InitialCatalog = "nhibernate", IntegratedSecurity = true });

			Assert.That(cfg.Properties[Environment.ProxyFactoryFactoryClass], Is.EqualTo(typeof(StaticProxyFactoryFactory).AssemblyQualifiedName));
			Assert.That(cfg.Properties[Environment.Dialect], Is.EqualTo(typeof(MsSql2005Dialect).AssemblyQualifiedName));
			Assert.That(cfg.Properties[Environment.ConnectionString], Is.EqualTo("Data Source=(local);Initial Catalog=nhibernate;Integrated Security=True"));
		}

		[Test]
		public void UseConnectionStringName()
		{
			var cfg = new Configuration();
			cfg.SessionFactory()
				.Integrate
					.Connected
						.ByAppConfing("MyName");

			Assert.That(cfg.Properties[Environment.ConnectionStringName], Is.EqualTo("MyName"));
		}

		[Test]
		public void NH2890Loquacious()
		{
			var cfg = new Configuration();
			cfg.Configure(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestEmbeddedConfig.cfg.xml"))
				.SetDefaultAssembly("NHibernate.DomainModel")
				.SetDefaultNamespace("NHibernate.DomainModel")
				.SessionFactory()
				.ParsingLinqThrough<NHibernate.Test.CfgTest.ConfigurationFixture.SampleQueryProvider>();

			using (var sessionFactory = cfg.BuildSessionFactory())
			{
				using (var session = sessionFactory.OpenSession())
				{
					var query = session.Query<NHibernate.DomainModel.A>();
					Assert.IsInstanceOf(typeof(NHibernate.Test.CfgTest.ConfigurationFixture.SampleQueryProvider), query.Provider);
				}
			}
		}
	}
}
