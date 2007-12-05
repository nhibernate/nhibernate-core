using System;
using System.Collections;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Search.Tests.Bridge
{
	[TestFixture]
	public class BridgeTest : SearchTestCase
	{
		[Test]
		public void DefaultAndNullBridges()
		{
			Cloud cloud = new Cloud();
			cloud.DateTime = null;
			cloud.Double1 = (null);
			cloud.Double2 = (2.1d);
			cloud.Int1 = (null);
			cloud.Int2 = (2);
			cloud.Float1 = (null);
			cloud.Float2 = (2.1f);
			cloud.Long1 = (null);
			cloud.Long2 = (2L);
			cloud.String1 = (null);
			cloud.Type = (CloudType.Dog);
			cloud.Storm = (false);
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			s.Save(cloud);
			s.Flush();
			tx.Commit();

			tx = s.BeginTransaction();
			IFullTextSession session = Search.CreateFullTextSession(s);
			QueryParser parser = new QueryParser("id", new StandardAnalyzer());
            Lucene.Net.Search.Query query;
			IList result;

			query = parser.Parse("Double2:[2 TO 2.1] AND Float2:[2  TO 2.1] " +
								 "AND Int2:[2 TO 2.1] AND Long2:[2 TO 2.1] AND Type:\"Dog\" AND Storm:false");

			result = session.CreateFullTextQuery(query).List();
			Assert.AreEqual(1, result.Count, "find primitives and do not fail on null");

			query = parser.Parse("Double1:[2 TO 2.1] OR Float1:[2 TO 2.1] OR Int1:[2 TO 2.1] OR Long1:[2 TO 2.1]");
			result = session.CreateFullTextQuery(query).List();
			Assert.AreEqual(0, result.Count, "null elements should not be stored"); //the query is dumb because restrictive

			s.Delete(s.Get(typeof(Cloud), cloud.Id));
			tx.Commit();
			s.Close();
		}

		[Test]
		public void CustomBridges()
		{
			Cloud cloud = new Cloud();
			cloud.CustomFieldBridge = ("This is divided by 2");
			cloud.CustomStringBridge = ("This is div by 4");
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			s.Save(cloud);
			s.Flush();
			tx.Commit();

			tx = s.BeginTransaction();
			IFullTextSession session = Search.CreateFullTextSession(s);
			QueryParser parser = new QueryParser("id", new SimpleAnalyzer());
            Lucene.Net.Search.Query query;
			IList result;

			query = parser.Parse("CustomFieldBridge:This AND CustomStringBridge:This");
			result = session.CreateFullTextQuery(query).List();
			Assert.AreEqual(1, result.Count, "Properties not mapped");

			query = parser.Parse("CustomFieldBridge:by AND CustomStringBridge:is");
			result = session.CreateFullTextQuery(query).List();
			Assert.AreEqual(0, result.Count, "Custom types not taken into account");

			s.Delete(s.Get(typeof(Cloud), cloud.Id));
			tx.Commit();
			s.Close();
		}

		[Test]
		public void DateTimeBridge()
		{
			Cloud cloud = new Cloud();


			DateTime date = new DateTime(2000, 12, 15, 3, 43, 2);
			cloud.DateTime = (date); //5 millisecond
			cloud.DateTimeDay = (date);
			cloud.DateTimeHour = (date);
			cloud.DateTimeMillisecond = (date);
			cloud.DateTimeMinute = (date);
			cloud.DateTimeMonth = (date);
			cloud.DateTimeSecond = (date);
			cloud.DateTimeYear = (date);
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			s.Save(cloud);
			s.Flush();
			tx.Commit();

			tx = s.BeginTransaction();
			IFullTextSession session = Search.CreateFullTextSession(s);
			QueryParser parser = new QueryParser("id", new StandardAnalyzer());
            Lucene.Net.Search.Query query;
			IList result;

			query = parser.Parse("DateTime:[19900101 TO 20060101]"
								 + " AND DateTimeDay:[20001214 TO 2000121501]"
								 + " AND DateTimeMonth:[200012 TO 20001201]"
								 + " AND DateTimeYear:[2000 TO 200001]"
								 + " AND DateTimeHour:[20001214 TO 2000121503]"
								 + " AND DateTimeMinute:[20001214 TO 200012150343]"
								 + " AND DateTimeSecond:[20001214 TO 20001215034302]"
								 + " AND DateTimeMillisecond:[20001214 TO 20001215034302005]"
				);
			result = session.CreateFullTextQuery(query).List();
			Assert.AreEqual(1, result.Count, "DateTime not found or not property truncated");

			s.Delete(s.Get(typeof(Cloud), cloud.Id));
			tx.Commit();
			s.Close();
		}

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			configuration.SetProperty(Environment.AnalyzerClass, typeof(SimpleAnalyzer).AssemblyQualifiedName);
		}

		protected override IList Mappings
		{
			get { return new string[] { "Bridge.Cloud.hbm.xml" }; }
		}
	}
}