using System;
using System.Data;

using NHibernate.DomainModel;
using NUnit.Framework;

namespace NHibernate.Test {
	
	[TestFixture]
	public class SimpleCompenentTest : TestCase {

		private DateTime testDateTime = new DateTime(2003, 8, 16);
		private DateTime updateDateTime = new DateTime(2003, 8, 17);


		[SetUp]
		public void SetUp() {
			//log4net.Config.DOMConfigurator.Configure();
			ExportSchema( new string[] { "SimpleComponent.hbm.xml"} );
		}

		[Test]
		public void TestLoad() {
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();

			TestInsert();

			SimpleComponent simpleComp = (SimpleComponent)s.Load(typeof(SimpleComponent), 10);

			Assertion.AssertEquals(10, simpleComp.Key);
			Assertion.AssertEquals("TestCreated", simpleComp.Audit.CreatedUserId);
			Assertion.AssertEquals("TestUpdated", simpleComp.Audit.UpdatedUserId);

			t.Commit();
			s.Close();

		}
		/// <summary>
		/// Test the ability to insert a new row with a User Assigned Key
		/// Right now - the only way to verify this is to watch SQL Profiler
		/// </summary>
		[Test]
		public void TestInsert() {
			
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			
			// create a new
			SimpleComponent simpleComp = new SimpleComponent();
			simpleComp.Name = "Simple 1";
			simpleComp.Address = "Street 12";
			simpleComp.Date = testDateTime;
			simpleComp.Count = 99;
			simpleComp.Audit.CreatedDate = System.DateTime.Now;
			simpleComp.Audit.CreatedUserId = "TestCreated";
			simpleComp.Audit.UpdatedDate = System.DateTime.Now;
			simpleComp.Audit.UpdatedUserId = "TestUpdated";
			
			
			s.Save(simpleComp, 10);
			
			t.Commit();
			s.Close();

			
		}
	}
}
