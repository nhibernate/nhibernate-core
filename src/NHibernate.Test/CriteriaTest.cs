using System;
using System.Collections;

using NHibernate.DomainModel;
using NUnit.Framework;

namespace NHibernate.Test
{
	
	[TestFixture]
	public class CriteriaTest : TestCase 
	{
		
		#region NUnit.Framework.TestFixture Members

		[TestFixtureSetUp]
		public void TestFixtureSetUp() 
		{
			ExportSchema( new string[] { "Simple.hbm.xml"});
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
		

		[Test]
		public void SimpleSelectTest() 
		{			
			// create the objects to search on
			ISession s1 = OpenSession();
			ITransaction t1 = s1.BeginTransaction();
			
			long simple1Key = 15;
			Simple simple1 = new Simple();
			simple1.Address = "Street 12";
			simple1.Date = DateTime.Now;
			simple1.Name = "For Criteria Test";
			simple1.Count = 16;

			long notSimple1Key = 17;
			Simple notSimple1 = new Simple();
			notSimple1.Address = "Street 123";
			notSimple1.Date = DateTime.Now;
			notSimple1.Name = "Don't be found";
			notSimple1.Count = 18;

			s1.Save(notSimple1, notSimple1Key);
			s1.Save(simple1, simple1Key);

			t1.Commit();
			s1.Close();

			ISession s2 = OpenSession();
			ITransaction t2 = s2.BeginTransaction();

			IList results2 = s2.CreateCriteria(typeof(Simple))
				.Add(Expression.Expression.Eq("Address","Street 12"))
				.List();
			
			Assert.AreEqual(1, results2.Count);

			Simple simple2 = (Simple)results2[0];

			Assert.IsNotNull(simple2, "Unable to load object");
			Assert.AreEqual(simple1.Count, simple2.Count, "Load failed");
			Assert.AreEqual(simple1.Name, simple2.Name, "Load failed");
			Assert.AreEqual(simple1.Address, simple2.Address, "Load failed");
			Assert.AreEqual(simple1.Date.ToString(), simple2.Date.ToString(), "Load failed");

			s2.Delete("from Simple");

			t2.Commit();
			s2.Close();
		}

		[Test]
		public void SimpleDateCriteria() 
		{
			Simple s1 = new Simple();
			s1.Address = "blah";
			s1.Count = 1;
			s1.Date = new DateTime( 2004, 01, 01 );
			
			Simple s2 = new Simple();
			s2.Address = "blah";
			s2.Count = 2;
			s2.Date = new DateTime( 2006, 01, 01 );

			ISession s = OpenSession();
			s.Save( s1, 1 );
			s.Save( s2, 2 );
			s.Flush();
			s.Close();

			s = OpenSession();
			IList results = s.CreateCriteria( typeof(Simple) )
				.Add( Expression.Expression.Gt( "Date", new DateTime( 2005, 01, 01 ) ) )
				.AddOrder( Expression.Order.Asc( "Date" ) )
				.List();

			Assert.AreEqual( 1, results.Count, "one gt from 2005" );
			Simple simple = (Simple)results[0];
			Assert.IsTrue( simple.Date > new DateTime( 2005, 01, 01), "should have returned dates after 2005" );
		
			results = s.CreateCriteria( typeof(Simple) )
				.Add( Expression.Expression.Lt( "Date", new DateTime( 2005, 01, 01 ) ) )
				.AddOrder( Expression.Order.Asc( "Date" ) )
				.List();
			
			Assert.AreEqual( 1, results.Count, "one lt than 2005" );
			simple = (Simple)results[0];
			Assert.IsTrue( simple.Date < new DateTime( 2005, 01, 01 ), "should be less than 2005" );

			s.Delete( "from Simple" );
			s.Close();
		}
	}
}
