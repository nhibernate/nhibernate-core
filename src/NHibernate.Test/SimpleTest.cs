using System;
using System.Collections;

using NHibernate.DomainModel;
using NUnit.Framework;

namespace NHibernate.Test 
{
	[TestFixture]
	public class SimpleTest : TestCase 
	{
		private DateTime testDateTime = new DateTime(2003, 8, 16);
		private DateTime updateDateTime = new DateTime(2003, 8, 17);

		#region NUnit.Framework.TestFixture Members

		[TestFixtureSetUp]
		public void TestFixtureSetUp() 
		{
			ExportSchema( new string[] { "Simple.hbm.xml"} );
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
		public void TestCRUD() 
		{
			long key = 10;
			long otherKey = 9;

			ISession s1 = sessions.OpenSession();
			ITransaction t1 = s1.BeginTransaction();
			
			// create a new
			Simple simple1 = new Simple();
			Simple otherSimple1 = new Simple();

			simple1.Name = "Simple 1";
			simple1.Address = "Street 12";
			simple1.Date = testDateTime;
			simple1.Count = 99;

			otherSimple1.Name = "Other Simple 1";
			otherSimple1.Address = "Other Street 12";
			otherSimple1.Date = testDateTime;
			otherSimple1.Count = 98;

			simple1.Other = otherSimple1;
			
			s1.Save(otherSimple1, otherKey);
			s1.Save(simple1, key);
			
			t1.Commit();
			s1.Close();

			// try to Load the object to make sure the save worked
			ISession s2 = sessions.OpenSession();
			ITransaction t2 = s2.BeginTransaction();
			
			Simple simple2 = (Simple)s2.Load(typeof(Simple), key);
			Simple otherSimple2 = (Simple)s2.Load(typeof(Simple), otherKey);

			// verify each property was saved as expected
			Assert.IsNotNull(simple2, "Unable to load object");
			Assert.IsNotNull(otherSimple2);
			Assert.AreSame(simple2.Other, otherSimple2);

			// update
			simple2.Count = 999;
			simple2.Name = "Simple 1 (Update)";
			simple2.Address = "Street 123";
			simple2.Date = updateDateTime;
			
			s2.Update(simple2, key);
			
			t2.Commit();
			s2.Close();

			// lets verify that the update worked 
			ISession s3 = sessions.OpenSession();
			ITransaction t3 = s3.BeginTransaction();

//			Simple simple3 = (Simple)s3.Load(typeof(Simple), key);
			Simple simple3 = (Simple)s3.Find( "from Simple as s where s.id = ? and '?'='?'", key, NHibernateUtil.Int64 )[0];
			Simple otherSimple3;

			Assert.AreEqual(simple2.Count, simple3.Count);
			Assert.AreEqual(simple2.Name, simple3.Name);
			Assert.AreEqual(simple2.Address, simple3.Address);
			Assert.AreEqual(simple2.Date, simple3.Date);
			
			// note that the Other will not be the same object because
			// they were loaded in 2 different sessions
			otherSimple3 = simple3.Other;

			// the update worked - lets clear out the table
			s3.Delete(simple3);
			s3.Delete(otherSimple3);

			t3.Commit();
			s3.Close();

			// verify there is no other Simple objects in the db
			ISession s4 = sessions.OpenSession();
			Assert.AreEqual(0, s4.CreateCriteria(typeof(Simple)).List().Count);
			s4.Close();

		}

		
		[Test]
		public void SetPropertiesOnQuery()
		{
			DateTime now = DateTime.Now;
			
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			
			// create a new
			long key = 10;
			Simple simple = new Simple();
			simple.Name = "Simple 1";
			simple.Address = "Street 12";
			simple.Date = now;
			simple.Count = 99;
			
			s.Save(simple, key);
			
			t.Commit();
			
			t = s.BeginTransaction();

			IQuery q = s.CreateQuery("from s in class Simple where s.Name=:Name and s.Count=:Count");
			q.SetProperties(simple);
			
			Simple loadedSimple = (Simple)q.List()[0];
			Assert.AreEqual(99, loadedSimple.Count);
			Assert.AreEqual("Simple 1", loadedSimple.Name);
			Assert.AreEqual("Street 12", loadedSimple.Address);
			Assert.AreEqual(now.ToString(), loadedSimple.Date.ToString());

			s.Delete(simple);
			
			t.Commit();
			s.Close();
			
		}

		
	}
}
