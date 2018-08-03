using System;
using System.Collections.Generic;
using NHibernate.AdoNet;
using NHibernate.Bytecode;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Exceptions;
using NHibernate.Hql;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using NHibernate.Transaction;
using NSubstitute;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.CfgTest
{
	[TestFixture]
	public class SettingsFactoryFixture
	{
		[Test]
		public void DefaultValueForKeyWords()
		{
			var properties = new Dictionary<string, string>
			                 	{
			                 		{"dialect", typeof (Dialect.MsSql2005Dialect).FullName}
			                 	};
			var settings = new SettingsFactory().BuildSettings(properties);
			Assert.That(settings.IsKeywordsImportEnabled);
			Assert.That(!settings.IsAutoQuoteEnabled);
		}

		[Test]
		public void DefaultServices()
		{
			var properties = new Dictionary<string, string>
			{
				{Environment.Dialect, typeof(Dialect.PostgreSQL83Dialect).FullName},
				{Environment.UseQueryCache, "true"}
			};
			var settings = new SettingsFactory().BuildSettings(properties);
			Assert.That(settings.BatcherFactory, Is.TypeOf<NonBatchingBatcherFactory>());
			Assert.That(settings.CacheProvider, Is.TypeOf<NoCacheProvider>());
			Assert.That(settings.ConnectionProvider, Is.TypeOf<UserSuppliedConnectionProvider>());
			Assert.That(settings.Dialect, Is.TypeOf<Dialect.PostgreSQL83Dialect>());
			Assert.That(settings.LinqToHqlGeneratorsRegistry, Is.TypeOf<DefaultLinqToHqlGeneratorsRegistry>());
			Assert.That(settings.QueryCacheFactory, Is.TypeOf<StandardQueryCacheFactory>());
			Assert.That(settings.QueryModelRewriterFactory, Is.Null);
			Assert.That(settings.QueryTranslatorFactory, Is.TypeOf<ASTQueryTranslatorFactory>());
			Assert.That(settings.QueryCacheFactory, Is.TypeOf<StandardQueryCacheFactory>());
			Assert.That(settings.SqlExceptionConverter, Is.TypeOf<SQLStateConverter>());
			Assert.That(settings.TransactionFactory, Is.TypeOf<AdoNetWithSystemTransactionFactory>());
		}

		[Test]
		public void RegisteredServices()
		{
			var batcherFactory = Substitute.For<IBatcherFactory>();
			var cacheProvider = Substitute.For<ICacheProvider>();
			var connectionProvider = Substitute.For<IConnectionProvider>();
			var dialect = new Dialect.MsSql2005Dialect();
			var linqToHqlRegistry = Substitute.For<ILinqToHqlGeneratorsRegistry>();
			var queryCacheFactory = Substitute.For<IQueryCacheFactory>();
			var queryModelRewriterFactory = Substitute.For<IQueryModelRewriterFactory>();
			var queryTranslatorFactory = Substitute.For<IQueryTranslatorFactory>();
			var sqlExceptionConverter = Substitute.For<ISQLExceptionConverter>();
			var transactionFactory = Substitute.For<ITransactionFactory>();

			var sp = new SimpleServiceProvider();
			sp.Register(() => batcherFactory);
			sp.Register(() => cacheProvider);
			sp.Register(() => connectionProvider);
			sp.Register<Dialect.Dialect>(() => dialect);
			sp.Register(() => linqToHqlRegistry);
			sp.Register(() => queryCacheFactory);
			sp.Register(() => queryModelRewriterFactory);
			sp.Register(() => queryTranslatorFactory);
			sp.Register(() => sqlExceptionConverter);
			sp.Register(() => transactionFactory);

			Environment.ServiceProvider = sp;

			var properties = new Dictionary<string, string>
			{
				{Environment.UseQueryCache, "true"}
			};
			var settings = new SettingsFactory().BuildSettings(properties);
			Assert.That(settings.BatcherFactory, Is.EqualTo(batcherFactory));
			Assert.That(settings.CacheProvider, Is.EqualTo(cacheProvider));
			Assert.That(settings.ConnectionProvider, Is.EqualTo(connectionProvider));
			Assert.That(settings.Dialect, Is.EqualTo(dialect));
			Assert.That(settings.LinqToHqlGeneratorsRegistry, Is.EqualTo(linqToHqlRegistry));
			Assert.That(settings.QueryCacheFactory, Is.EqualTo(queryCacheFactory));
			Assert.That(settings.QueryModelRewriterFactory, Is.EqualTo(queryModelRewriterFactory));
			Assert.That(settings.QueryTranslatorFactory, Is.EqualTo(queryTranslatorFactory));
			Assert.That(settings.SqlExceptionConverter, Is.EqualTo(sqlExceptionConverter));
			Assert.That(settings.TransactionFactory, Is.EqualTo(transactionFactory));
		}

		[Test]
		public void InvalidRegisteredServices()
		{
			InvalidRegisteredService<IBatcherFactory>();
			InvalidRegisteredService<ICacheProvider>();
			InvalidRegisteredService<IConnectionProvider>();
			InvalidRegisteredService<Dialect.Dialect>();
			InvalidRegisteredService<ILinqToHqlGeneratorsRegistry>();
			InvalidRegisteredService<IQueryCacheFactory>();
			InvalidRegisteredService<IQueryModelRewriterFactory>();
			InvalidRegisteredService<IQueryTranslatorFactory>();
			InvalidRegisteredService<ITransactionFactory>();
		}

		private void InvalidRegisteredService<TService>()
		{
			var sp = new SimpleServiceProvider();
			sp.Register<TService>(() => throw new InvalidOperationException());

			Environment.ServiceProvider = sp;

			var properties = new Dictionary<string, string>
			{
				{Environment.UseQueryCache, "true"}
			};
			if (typeof(TService) != typeof(Dialect.Dialect))
			{
				properties.Add(Environment.Dialect, typeof(Dialect.PostgreSQL83Dialect).FullName);
			}

			Assert.Throws<HibernateException>(
				() => new SettingsFactory().BuildSettings(properties),
				$"HibernateException should be thrown for service {typeof(TService)}");
		}

		private IServiceProvider _originalSp;

		[SetUp]
		public void Setup()
		{
			_originalSp = Environment.ServiceProvider;
		}

		[TearDown]
		public void TearDown()
		{
			Environment.ServiceProvider = _originalSp;
		}
	}
}
