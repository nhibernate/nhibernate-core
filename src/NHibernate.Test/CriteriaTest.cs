using System;
using System.Collections;
using System.Data;

using NHibernate.DomainModel;
using NUnit.Framework;

namespace NHibernate.Test {
	
	[TestFixture]
	public class CriteriaTest : TestCase {

		[SetUp]
		public void SetUp() {
			//log4net.Config.DOMConfigurator.Configure();
			ExportSchema( new string[] { "Simple.hbm.xml"}, true );
		}

		[Test]
		public void SimpleSelectTest() {
			
			// create the objects to search on
			ISession s1 = sessions.OpenSession();
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

			ISession s2 = sessions.OpenSession();
			ITransaction t2 = s2.BeginTransaction();

			IList results2 = s2.CreateCriteria(typeof(Simple))
				.Add(Expression.Expression.Eq("Address","Street 12"))
				.List();
			
			Assertion.AssertEquals(1, results2.Count);

			Simple simple2 = (Simple)results2[0];

			Assertion.AssertNotNull("Unable to load object", simple2);
			Assertion.AssertEquals("Load failed", simple1.Count, simple2.Count);
			Assertion.AssertEquals("Load failed", simple1.Name, simple2.Name);
			Assertion.AssertEquals("Load failed", simple1.Address, simple2.Address);
			Assertion.AssertEquals("Load failed", simple1.Date.ToString(), simple2.Date.ToString());

			s2.Delete("from Simple");

			t2.Commit();
			s2.Close();
		}
	


	}
}
