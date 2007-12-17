using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Lucene.Net.Analysis;
using Lucene.Net.QueryParsers;
using NHibernate.Cfg;
using NHibernate.Search.Impl;
using NHibernate.Search.Storage;
using NUnit.Framework;
using TestCase=NHibernate.Test.TestCase;

namespace NHibernate.Search.Tests.DirectoryProvider
{
	[TestFixture]
	public class FSSlaveAndMasterDPTest : MultiplySessionFactoriesTestCase
	{
		public override void TestInitialize()
		{
			if(Directory.Exists("./lucenedirs/"))
				Directory.Delete("./lucenedirs/",true);
			Directory.CreateDirectory("./lucenedirs/master/copy");
			Directory.CreateDirectory("./lucenedirs/master/main");
			Directory.CreateDirectory("./lucenedirs/slave");
			base.TestInitialize();
		}

		[Test]
		public void ProperCopy()
		{
			ISession s1 = CreateSession(0);
			SnowStorm sn = new SnowStorm();
			sn.DateTime = DateTime.Now;	
			sn.Location = ("Dallas, TX, USA");

			IFullTextSession fts2 = Search.CreateFullTextSession(CreateSession(1));
			QueryParser parser = new QueryParser("id", new StopAnalyzer());
			IList result = fts2.CreateFullTextQuery(parser.Parse("Location:texas")).List();
			Assert.AreEqual(0, result.Count, "No copy yet, fresh index expected");

			s1.Save(sn);
			s1.Flush(); //we don' commit so we need to flush manually

			fts2.Close();
			s1.Close();

			int waitPeroid = 2*1*1000 + 10; //wait a bit more than 2 refresh (one master / one slave)
			Thread.Sleep(waitPeroid);

			//temp test original
			fts2 = Search.CreateFullTextSession(CreateSession(0));
			result = fts2.CreateFullTextQuery(parser.Parse("Location:dallas")).List();
			Assert.AreEqual(1, result.Count, "Original should get one");

			fts2 = Search.CreateFullTextSession(CreateSession(1));
			result = fts2.CreateFullTextQuery(parser.Parse("Location:dallas")).List();
			Assert.AreEqual(1, result.Count, "First copy did not work out");

			s1 = CreateSession(0);
			sn = new SnowStorm();
			sn.DateTime = DateTime.Now;
			sn.Location = ("Chennai, India");

			s1.Save(sn);
			s1.Flush(); //we don' commit so we need to flush manually

			fts2.Close();
			s1.Close();

			Thread.Sleep(waitPeroid); //wait a bit more than 2 refresh (one master / one slave)

			fts2 = Search.CreateFullTextSession(CreateSession(1));
			result = fts2.CreateFullTextQuery(parser.Parse("Location:chennai")).List();
			Assert.AreEqual(1, result.Count, "Second copy did not work out");

			s1 = CreateSession(0);
			sn = new SnowStorm();
			sn.DateTime = DateTime.Now;
			sn.Location = ("Melbourne, Australia");

			s1.Save(sn);
			s1.Flush(); //we don' commit so we need to flush manually

			fts2.Close();
			s1.Close();

			Thread.Sleep(waitPeroid); //wait a bit more than 2 refresh (one master / one slave)

			fts2 = Search.CreateFullTextSession(CreateSession(1));
			result = fts2.CreateFullTextQuery(parser.Parse("Location:melbourne")).List();
			Assert.AreEqual(1, result.Count, "Third copy did not work out");

			fts2.Close();
		}

		private ISession CreateSession(int sessionFactoryNumber)
		{
			return SessionFactories[sessionFactoryNumber].OpenSession(new SearchInterceptor());
		}

		[TearDown]
		protected void TearDown()
		{
			Directory.Delete("./lucenedirs/",true);
		}

		protected override void Configure(IList<Configuration> cfg)
		{
			//master
			cfg[0].SetProperty("hibernate.search.default.sourceBase", "./lucenedirs/master/copy");
			cfg[0].SetProperty("hibernate.search.default.indexBase", "./lucenedirs/master/main");
			cfg[0].SetProperty("hibernate.search.default.refresh", "1"); //every minute
			cfg[0].SetProperty("hibernate.search.default.directory_provider", typeof(FSMasterDirectoryProvider).AssemblyQualifiedName);

			//slave(s)
			cfg[1].SetProperty("hibernate.search.default.sourceBase", "./lucenedirs/master/copy");
			cfg[1].SetProperty("hibernate.search.default.indexBase", "./lucenedirs/slave");
			cfg[1].SetProperty("hibernate.search.default.refresh", "1"); //every minute
			cfg[1].SetProperty("hibernate.search.default.directory_provider", typeof(FSSlaveDirectoryProvider).AssemblyQualifiedName);
		}


		protected override IList Mappings
		{
			get { return new string[] {"DirectoryProvider.SnowStorm.hbm.xml"}; }
		}

		protected override int NumberOfSessionFactories
		{
			get { return 2; }
		}
	}
}