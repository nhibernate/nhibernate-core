using System;
using System.Data;

using NHibernate.DomainModel.NHSpecific;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest 
{
	
	[TestFixture]
	public class SimpleCompenentFixture : TestCase {

		private DateTime testDateTime = new DateTime(2003, 8, 16);
		private DateTime updateDateTime = new DateTime(2003, 8, 17);


		[SetUp]
		public void SetUp() {
			ExportSchema( new string[] { "NHSpecific.SimpleComponent.hbm.xml"} );
		}

		[Test]
		public void TestLoad() {
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			TestInsert();

			SimpleComponent simpleComp = (SimpleComponent)s.Load(typeof(SimpleComponent), 10);

			Assert.AreEqual(10, simpleComp.Key);
			Assert.AreEqual("TestCreated", simpleComp.Audit.CreatedUserId);
			Assert.AreEqual("TestUpdated", simpleComp.Audit.UpdatedUserId);

			t.Commit();
			s.Close();

		}
		/// <summary>
		/// Test the ability to insert a new row with a User Assigned Key
		/// Right now - the only way to verify this is to watch SQL Profiler
		/// </summary>
		[Test]
		public void TestInsert() {
			
			ISession s = OpenSession();
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
