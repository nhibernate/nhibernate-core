using System;
using System.Collections;

using NHibernate.DomainModel;

using NUnit.Framework;

namespace NHibernate.Test
{
	/// <summary>
	/// Summary description for SQLLoaderTest.
	/// </summary>
	[TestFixture]
	public class SQLLoaderTest : TestCase
	{
		#region NUnit.Framework.TestFixture Members

		[TestFixtureSetUp]
		public void TestFixtureSetUp() 
		{
			ExportSchema( new string[] { "ABC.hbm.xml",
										   "Category.hbm.xml",
										   "Simple.hbm.xml",
										   "Fo.hbm.xml",
										   "SingleSeveral.hbm.xml",
										   "Componentizable.hbm.xml"
									   } );
		}

		[SetUp]
		public void SetUp() 
		{
			// there are test in here where we don't need to resetup the 
			// tables - so only set the tables up once
		}

		[TearDown]
		public override void TearDown() 
		{
			// do nothing except not let the base TearDown get called
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown() 
		{
			base.TearDown();
		}

		#endregion

		static int nextInt = 1;
		static long nextLong = 1;

		[Test]
		public void TestTS()
		{
			/*
			if ( Dialect is NHibernate.Dialect.Oracle9Dialect )
			{
				return;
			}
			*/

			ISession session = sessions.OpenSession();

			Simple sim = new Simple();
			sim.Date = DateTime.Now;
			session.Save( sim, 1 );
			IQuery q = session.CreateSQLQuery( "select {sim.*} from Simple {sim} where {sim}.date_ = ?", "sim", typeof( Simple ) );
			q.SetTimestamp( 0, sim.Date );
			Assert.AreEqual( 1, q.List().Count, "q.List.Count");
			session.Delete( sim );
			session.Flush();
			session.Close();
		}

		[Test]
		[Ignore("Test not written")]
		public void TestFindBySQLStar()
		{
		}

		[Test]
		[Ignore("Test not written")]
		public void TestFindBySQLProperties()
		{
		}

		[Test]
		[Ignore("Test not written")]
		public void TestFindBySQLAssociatedObject()
		{
		}

		[Test]
		[Ignore("Test not written")]
		public void TestFindBySQLMultipleObject()
		{
		}

		[Test]
		[Ignore("Test not written")]
		public void TestFindBySQLParameters()
		{
		}

		[Test]
		[Ignore("Test not written")]
		public void TestEscapedODBC()
		{
		}

		[Test]
		[Ignore("Test not written")]
		public void TestDoubleAliasing()
		{
		}

		[Test]
		[Ignore("Test not written")]
		public void TestEmbeddedCompositeProperties()
		{
		}

		[Test]
		[Ignore("Test not written")]
		public void TestComponentStar()
		{
		}

		[Test]
		[Ignore("Test not written")]
		public void TestComponentNoStar()
		{
		}

		private void ComponentTest()
		{
		}

		[Test]
		[Ignore("Test not written")]
		public void TestFindSimpleBySQL()
		{
		}

		[Test]
		[Ignore("Test not written")]
		public void TestFindBySQLSimpleByDiffSessions()
		{
		}

		[Test]
		[Ignore("Test not written")]
		public void TestFindBySQLDiscriminatorSameSession()
		{
		}

		[Test]
		[Ignore("Test not written")]
		public void TestFindBySQLDiscriminatedDiffSessions()
		{
		}

		[Test]
		[Ignore("Test not written")]
		public void TestNamedSQLQuery()
		{
		}
	}
}
