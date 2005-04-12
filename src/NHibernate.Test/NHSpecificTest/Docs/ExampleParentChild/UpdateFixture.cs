using System;

using NHibernate;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.Docs.ExampleParentChild
{
	[TestFixture]
	public class UpdateFixture : TestCase
	{
		#region NUnit.Framework.TestFixture Members

		[TestFixtureSetUp]
		public void TestFixtureSetUp() 
		{
			ExportSchema( new string[] { "NHSpecificTest.Docs.ExampleParentChild.Mappings.hbm.xml"}, true, "NHibernate.Test" );
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
		public void Update()
		{
			ISession session1 = sessions.OpenSession();

			Parent parent1 =  new Parent();
			Child child1 = new Child();
			parent1.AddChild( child1 );

			long pId = (long)session1.Save( parent1 );
			long cId = (long)session1.Save( child1 );
			session1.Flush();
			session1.Close();

			ISession session2 = sessions.OpenSession();
			Parent parent = session2.Load( typeof( Parent ), pId ) as Parent;
			Child child = session2.Load( typeof( Child ), cId ) as Child;
			session2.Close();

			parent.AddChild( child );
			Child newChild = new Child();
			parent.AddChild( newChild );

			ISession session = sessions.OpenSession();
			session.Update( parent );
			session.Flush();
			session.Close();
		}
	}
}
