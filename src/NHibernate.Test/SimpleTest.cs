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


		[SetUp]
		public void SetUp() 
		{
			//log4net.Config.DOMConfigurator.Configure();
			ExportSchema( new string[] { "Simple.hbm.xml"} );
		}

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
			Assertion.AssertNotNull("Unable to load object", simple2);
			Assertion.AssertNotNull(otherSimple2);
			Assertion.AssertSame(simple2.Other, otherSimple2);

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

			Simple simple3 = (Simple)s3.Load(typeof(Simple), simple2.Key);
			Simple otherSimple3;

			Assertion.AssertEquals(simple2.Count, simple3.Count);
			Assertion.AssertEquals(simple2.Name, simple3.Name);
			Assertion.AssertEquals(simple2.Address, simple3.Address);
			Assertion.AssertEquals(simple2.Date, simple3.Date);
			
			// note that the Other will not be the same object because
			// they were loaded in 2 different sessions
			otherSimple3 = simple3.Other;
			Assertion.AssertEquals(simple2.Other.Key, otherSimple3.Key);

			// the update worked - lets clear out the table
			s3.Delete(simple3);
			s3.Delete(otherSimple3);

			t3.Commit();
			s3.Close();

			// verify there is no other Simple objects in the db
			ISession s4 = sessions.OpenSession();
			Assertion.AssertEquals(0, s4.CreateCriteria(typeof(Simple)).List().Count);
			s4.Close();

		}

		
		private void OldTestCase()
		{
			DateTime now = DateTime.Now;
			
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			
			// create a new
			Simple simple = new Simple();
			simple.Name = "Simple 1";
			simple.Address = "Street 12";
			simple.Date = now;
			simple.Count = 99;
			
			s.Save(simple, 10);
			
			t.Commit();
			s.Close();

			
			// BUG: It looks like the problem is coming here because we now have the sample Entity loaded
			// twice in the entries field for this Session.  I don't understand why that would happen
			// because a Dictionary should not allow a key to be in there twice...
			IQuery q = s.CreateQuery("from s in class Simple where s.Name=:Name and s.Count=:Count");
			q.SetProperties(simple);
			
			Simple loadedSimple = (Simple)q.List()[0];
			// Check if save failed
			Assertion.AssertEquals("Save failed", 99,             simple.Count);
			Assertion.AssertEquals("Save failed", "Simple 1",     simple.Name);
			Assertion.AssertEquals("Save failed", "Street 12",    simple.Address);
			Assertion.AssertEquals("Save failed", now.ToString(), simple.Date.ToString());
			// Check if load failed
			Assertion.AssertNotNull("Unable to load object",              loadedSimple);
			Assertion.AssertEquals("Load failed", simple.Count,           loadedSimple.Count);
			Assertion.AssertEquals("Load failed", simple.Name,            loadedSimple.Name);
			Assertion.AssertEquals("Load failed", simple.Address,         loadedSimple.Address);
			Assertion.AssertEquals("Load failed", simple.Date.ToString(), loadedSimple.Date.ToString());

			// The INSERT, UPDATE amd SELECT are performed due to some hacks, 
			// see Impl\SessionImpl.cs class AdoHack
			// Btw: when something goes wrong, the transaction remains opened ;-)

			// When you set a breakpoint at this line, you'll see the record in the database
			((Transaction.Transaction)t).AdoTransaction.Commit();

			//s.Delete(simple);
			//
			// note: this line fails!!!!
			//
			t.Commit();
			s.Close();
			
		}

		
	}
}
