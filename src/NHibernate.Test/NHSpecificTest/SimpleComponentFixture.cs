using System;
using System.Data;

using NHibernate.DomainModel.NHSpecific;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest 
{
	[TestFixture]
	public class SimpleComponentFixture : TestCase {

		private DateTime testDateTime = new DateTime(2003, 8, 16);
		private DateTime updateDateTime = new DateTime(2003, 8, 17);

		protected override System.Collections.IList Mappings
		{
			get
			{
				return new string[] { "NHSpecific.SimpleComponent.hbm.xml"};
			}
		}

		protected override void OnSetUp()
		{
			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
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
			
			
				s.Save(simpleComp, 10L);
			
				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using( ISession s = OpenSession() )
			{
				s.Delete( s.Load( typeof( SimpleComponent ), 10L ) );
				s.Flush();
			}
		}


		[Test]
		public void TestLoad()
		{
			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{

				SimpleComponent simpleComp = (SimpleComponent)s.Load( typeof( SimpleComponent ), 10L );

				Assert.AreEqual(10L, simpleComp.Key);
				Assert.AreEqual("TestCreated", simpleComp.Audit.CreatedUserId);
				Assert.AreEqual("TestUpdated", simpleComp.Audit.UpdatedUserId);

				t.Commit();
			}
		}
		/// <summary>
		/// Test the ability to insert a new row with a User Assigned Key
		/// Right now - the only way to verify this is to watch SQL Profiler
		/// </summary>
		[Test]
		public void TestInsert()
		{
			// Do nothing, all the action is in OnSetUp.
		}
	}
}
