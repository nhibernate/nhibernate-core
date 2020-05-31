using System.Data;
using NHibernate.AdoNet;
using NHibernate.Bytecode;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Cfg.Loquacious;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Linq.Functions;
using NHibernate.Type;
using NUnit.Framework;
using NHibernate.Exceptions;

namespace NHibernate.Test.CfgTest.Loquacious
{
	[TestFixture]
	public class LambdaConfigurationFixture
	{
		[Test]
		public void FullConfiguration()
		{
			var configure = new Configuration();
			configure.SessionFactoryName("SomeName");
			configure.Cache(c =>
												{
													c.UseMinimalPuts = true;
													c.DefaultExpiration = 15;
													c.RegionsPrefix = "xyz";
													c.Provider<HashtableCacheProvider>();
													c.QueryCacheFactory<StandardQueryCacheFactory>();
												});
			configure.CollectionTypeFactory<DefaultCollectionTypeFactory>();
			configure.HqlQueryTranslator<ASTQueryTranslatorFactory>();
			configure.LinqToHqlGeneratorsRegistry<DefaultLinqToHqlGeneratorsRegistry>();
			configure.Proxy(p =>
												{
													p.Validation = false;
													p.ProxyFactoryFactory<StaticProxyFactoryFactory>();
												});
			configure.Mappings(m=>
								{
									m.DefaultCatalog = "MyCatalog";
									m.DefaultSchema = "MySche";
								});
			configure.DataBaseIntegration(db =>
											{
												db.Dialect<MsSql2000Dialect>();
												db.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
#if NETFX
												db.Batcher<SqlClientBatchingBatcherFactory>();
#endif
												db.BatchSize = 15;
												db.ConnectionProvider<DebugConnectionProvider>();
												db.Driver<SqlClientDriver>();
												db.ConnectionReleaseMode = ConnectionReleaseMode.AfterTransaction;
												db.IsolationLevel = IsolationLevel.ReadCommitted;
												db.ConnectionString = "The connection string";
												db.AutoCommentSql = true;
												db.ExceptionConverter<SQLStateConverter>();
												db.PrepareCommands = true;
												db.Timeout = 10;
												db.MaximumDepthOfOuterJoinFetching = 11;
												db.HqlToSqlSubstitutions = "true 1, false 0, yes 'Y', no 'N'";
												db.SchemaAction = SchemaAutoAction.Validate;
												db.ThrowOnSchemaUpdate = true;
											});

			Assert.That(configure.Properties[Environment.SessionFactoryName], Is.EqualTo("SomeName"));
			Assert.That(configure.Properties[Environment.CacheProvider],
									Is.EqualTo(typeof(HashtableCacheProvider).AssemblyQualifiedName));
			Assert.That(configure.Properties[Environment.CacheRegionPrefix], Is.EqualTo("xyz"));
			Assert.That(configure.Properties[Environment.QueryCacheFactory],
									Is.EqualTo(typeof(StandardQueryCacheFactory).AssemblyQualifiedName));
			Assert.That(configure.Properties[Environment.UseMinimalPuts], Is.EqualTo("true"));
			Assert.That(configure.Properties[Environment.CacheDefaultExpiration], Is.EqualTo("15"));
			Assert.That(configure.Properties[Environment.CollectionTypeFactoryClass],
									Is.EqualTo(typeof(DefaultCollectionTypeFactory).AssemblyQualifiedName));
			Assert.That(configure.Properties[Environment.UseProxyValidator], Is.EqualTo("false"));
			Assert.That(configure.Properties[Environment.ProxyFactoryFactoryClass],
						Is.EqualTo(typeof(StaticProxyFactoryFactory).AssemblyQualifiedName));
			Assert.That(configure.Properties[Environment.QueryTranslator],
						Is.EqualTo(typeof(ASTQueryTranslatorFactory).AssemblyQualifiedName));
			Assert.That(configure.Properties[Environment.DefaultCatalog], Is.EqualTo("MyCatalog"));
			Assert.That(configure.Properties[Environment.DefaultSchema], Is.EqualTo("MySche"));
			Assert.That(configure.Properties[Environment.Dialect],
						Is.EqualTo(typeof(MsSql2000Dialect).AssemblyQualifiedName));
			Assert.That(configure.Properties[Environment.Hbm2ddlKeyWords], Is.EqualTo("auto-quote"));
#if NETFX
			Assert.That(configure.Properties[Environment.BatchStrategy],
						Is.EqualTo(typeof(SqlClientBatchingBatcherFactory).AssemblyQualifiedName));
#endif
			Assert.That(configure.Properties[Environment.BatchSize], Is.EqualTo("15"));
			Assert.That(configure.Properties[Environment.ConnectionProvider],
						Is.EqualTo(typeof(DebugConnectionProvider).AssemblyQualifiedName));
			Assert.That(configure.Properties[Environment.ConnectionDriver],
						Is.EqualTo(typeof(SqlClientDriver).AssemblyQualifiedName));
			Assert.That(configure.Properties[Environment.ReleaseConnections],
									Is.EqualTo(ConnectionReleaseModeParser.ToString(ConnectionReleaseMode.AfterTransaction)));
			Assert.That(configure.Properties[Environment.Isolation], Is.EqualTo("ReadCommitted"));
			Assert.That(configure.Properties[Environment.ConnectionString], Is.EqualTo("The connection string"));
			Assert.That(configure.Properties[Environment.UseSqlComments], Is.EqualTo("true"));
			Assert.That(configure.Properties[Environment.SqlExceptionConverter],
									Is.EqualTo(typeof(SQLStateConverter).AssemblyQualifiedName));
			Assert.That(configure.Properties[Environment.PrepareSql], Is.EqualTo("true"));
			Assert.That(configure.Properties[Environment.CommandTimeout], Is.EqualTo("10"));
			Assert.That(configure.Properties[Environment.MaxFetchDepth], Is.EqualTo("11"));
			Assert.That(configure.Properties[Environment.QuerySubstitutions], Is.EqualTo("true 1, false 0, yes 'Y', no 'N'"));
			Assert.That(configure.Properties[Environment.Hbm2ddlAuto], Is.EqualTo("validate"));
			Assert.That(configure.Properties[Environment.Hbm2ddlThrowOnUpdate], Is.EqualTo("true"));
			Assert.That(configure.Properties[Environment.LinqToHqlGeneratorsRegistry], Is.EqualTo(typeof(DefaultLinqToHqlGeneratorsRegistry).AssemblyQualifiedName));
			
			// Keywords import and auto-validation require a valid connection string, disable them before checking
			// the session factory can be built.
			configure.SetProperty(Environment.Hbm2ddlKeyWords, "none");
			configure.SetProperty(Environment.Hbm2ddlAuto, null);
			Assert.That(() => configure.BuildSessionFactory().Dispose(), Throws.Nothing);
		}
	}
}
