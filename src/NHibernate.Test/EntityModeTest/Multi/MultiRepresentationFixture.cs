using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using NUnit.Framework;

namespace NHibernate.Test.EntityModeTest.Multi
{
	[TestFixture, Ignore("Not supported yet.")]
	public class MultiRepresentationFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] {"EntityModeTest.Multi.Stock.hbm.xml", "EntityModeTest.Multi.Valuation.hbm.xml"}; }
		}

		private class TestData
		{
			private readonly ISessionFactory sessions;
			public long stockId;

			public TestData(ISessionFactory factory)
			{
				sessions = factory;
			}

			public void Create()
			{
				ISession session = sessions.OpenSession();
				session.BeginTransaction();
				var stock = new Stock {TradeSymbol = "NHForge"};
				var valuation = new Valuation {Stock = stock, ValuationDate = DateTime.Now, Value = 200.0D};
				stock.CurrentValuation = valuation;
				stock.Valuations.Add(valuation);

				session.Save(stock);
				session.Save(valuation);

				session.Transaction.Commit();
				session.Close();

				stockId = stock.Id;
			}

			public void Destroy()
			{
				ISession session = sessions.OpenSession();
				session.BeginTransaction();
				IList<Stock> stocks = session.CreateQuery("from Stock").List<Stock>();
				foreach (Stock stock in stocks)
				{
					stock.CurrentValuation = null;
					session.Flush();
					foreach (Valuation valuation in stock.Valuations)
					{
						session.Delete(valuation);
					}
					session.Delete(stock);
				}
				session.Transaction.Commit();
				session.Close();
			}
		}

		[Test]
		public void PocoRetreival()
		{
			var testData = new TestData(sessions);
			testData.Create();

			ISession session = OpenSession();
			ITransaction txn = session.BeginTransaction();

			var stock = session.Get<Stock>(1);
			Assert.That(stock.Id, Is.EqualTo(1L));

			txn.Commit();
			session.Close();

			testData.Destroy();
		}

		[Test]
		public void XmlHQL()
		{
			var testData = new TestData(sessions);
			testData.Create();

			ISession session = OpenSession();
			ITransaction txn = session.BeginTransaction();
			ISession xml = session.GetSession(EntityMode.Xml);

			IList result = xml.CreateQuery("from Stock").List();

			Assert.That(result.Count, Is.EqualTo(1L));
			var element = (XmlElement) result[0];
			Assert.That(element.Attributes["id"], Is.EqualTo(testData.stockId));

			Console.WriteLine("**** XML: ****************************************************");
			//prettyPrint( element );
			Console.WriteLine("**************************************************************");

			txn.Rollback();
			session.Close();

			testData.Destroy();
		}

		[Test]
		public void XmlRetreival()
		{
			var testData = new TestData(sessions);
			testData.Create();

			ISession session = OpenSession();
			ITransaction txn = session.BeginTransaction();
			ISession xml = session.GetSession(EntityMode.Xml);

			object rtn = xml.Get(typeof (Stock).FullName, testData.stockId);
			var element = (XmlElement) rtn;

			Assert.That(element.Attributes["id"], Is.EqualTo(testData.stockId));

			Console.WriteLine("**** XML: ****************************************************");
			//prettyPrint( element );
			Console.WriteLine("**************************************************************");

			XmlNode currVal = element.GetElementsByTagName("currentValuation")[0];

			Console.WriteLine("**** XML: ****************************************************");
			//prettyPrint( currVal );
			Console.WriteLine("**************************************************************");

			txn.Rollback();
			session.Close();

			testData.Destroy();
		}

		[Test]
		public void XmlSave()
		{
			var testData = new TestData(sessions);
			testData.Create();

			ISession pojos = OpenSession();
			ITransaction txn = pojos.BeginTransaction();
			ISession xml = pojos.GetSession(EntityMode.Xml);

			var domDoc = new XmlDocument();
			XmlElement stock = domDoc.CreateElement("stock");
			domDoc.AppendChild(stock);
			XmlElement tradeSymbol = domDoc.CreateElement("tradeSymbol");
			tradeSymbol.InnerText = "Microsoft";
			stock.AppendChild(tradeSymbol);

			XmlElement cval = domDoc.CreateElement("currentValuation");
			XmlElement val = domDoc.CreateElement("valuation");
			stock.AppendChild(cval);
			//val.appendContent(stock); TODO
			XmlElement valuationDate = domDoc.CreateElement("valuationDate");
			tradeSymbol.InnerText = DateTime.Now.ToString();
			val.AppendChild(valuationDate);

			XmlElement value = domDoc.CreateElement("value");
			tradeSymbol.InnerText = "121.00";
			val.AppendChild(value);

			xml.Save(typeof (Stock).FullName, stock);
			xml.Flush();

			txn.Rollback();
			pojos.Close();

			Assert.That(!pojos.IsOpen);
			Assert.That(!xml.IsOpen);

			//prettyPrint( stock );

			testData.Destroy();
		}
	}
}