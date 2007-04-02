using Lucene.Net.Analysis;
using Lucene.Net.Store;
using NHibernate.Cfg;
using NHibernate.Search.Impl;
using NHibernate.Search.Storage;
using NUnit.Framework;

namespace NHibernate.Search.Tests
{
	[TestFixture]
	public abstract class SearchTestCase : NHibernate.Test.TestCase
	{
		protected Directory GetDirectory(System.Type clazz)
		{
			return SearchFactory.GetSearchFactory(sessions).GetDirectoryProvider(clazz).Directory;
		}

		protected override void Configure(Configuration configuration)
		{
			cfg.SetProperty("hibernate.search.default.directory_provider", typeof (RAMDirectoryProvider).AssemblyQualifiedName);
			cfg.SetProperty(Environment.AnalyzerClass, typeof (StopAnalyzer).AssemblyQualifiedName);
		}


		protected override ISession OpenSession()
		{
			lastOpenedSession = sessions.OpenSession(new SearchInterceptor());
			return lastOpenedSession;
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Search.Tests"; }
		}

		protected override void BuildSessionFactory()
		{
			base.BuildSessionFactory();
			SearchFactory.Initialize(cfg, sessions);
		}

	}
}