using System;
using System.Data;
using NUnit.Framework;

namespace NHibernate.Test {
	
	[TestFixture]
	public class SimpleTest : TestCase {

		[SetUp]
		public void SetUp() {
			ExportSchema( new string[] { "Simple.hbm.xml" } );
		}

		[Test]
		public void TestSetPropertes() {
			DateTime now = DateTime.Now;
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			Simple simple = new Simple();
			simple.Name = "Simple 1";
			simple.Address = "Street 12";
			simple.Date = now;
			simple.Count = 99;
			s.Save(simple, 10);
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

			s.Delete(simple);
			//
			// note: this line fails!!!!
			//
			t.Commit();
			s.Close();
		}
		
	}
}
