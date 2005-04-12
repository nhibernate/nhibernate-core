using System;

using NHibernate;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.Docs.Associations.BiM21
{
	[TestFixture]
	public class Fixture : TestCase
	{
		#region NUnit.Framework.TestFixture Members

		[TestFixtureSetUp]
		public void TestFixtureSetUp() 
		{
			ExportSchema( new string[] { "NHSpecificTest.Docs.Associations.BiM21.Mappings.hbm.xml"}, true, "NHibernate.Test" );
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
			//base.TearDown ();
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown() 
		{
			// only do this at the end of the test fixture
			base.TearDown();
		}

		#endregion
		

		[Test]
		public void TestCorrectUse()
		{

			ISession session = sessions.OpenSession();

			Person fred = new Person();
			Person wilma = new Person();

			Address flinstoneWay = new Address();

			fred.Address = flinstoneWay;
			wilma.Address = flinstoneWay;

			session.Save( flinstoneWay );
			session.Save( fred );
			session.Save( wilma );

			session.Close();

		}

		[Test]
		[ExpectedException( typeof(ADOException) )]
		public void TestErrorUsage()
		{
			using( ISession session = sessions.OpenSession() ) 
			{
				Person fred = new Person();
				Person wilma = new Person();

				Address flinstoneWay = new Address();

				fred.Address = flinstoneWay;
				wilma.Address = flinstoneWay;

				session.Save( fred );
			}
		}
	}
}
