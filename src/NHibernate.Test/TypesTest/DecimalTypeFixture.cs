using System;

using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest 
{
	/// <summary>
	/// The Unit Tests for the DecimalType
	/// </summary>
	[TestFixture]
	public class DecimalTypeFixture  : TestCase 
	{

		#region NUnit.Framework.TestFixture Members

		[TestFixtureSetUp]
		public void TestFixtureSetUp() 
		{
			ExportSchema( new string[] { "TypesTest.DecimalClass.hbm.xml"}, true, "NHibernate.Test" );
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
		
		/// <summary>
		/// Test that two decimal fields that are exactly equal are returned
		/// as Equal by the DecimalType.
		/// </summary>
		[Test]
		public void Equals() 
		{
			decimal lhs = 5.64351M;
			decimal rhs = 5.64351M;

			DecimalType type = (DecimalType)NHibernateUtil.Decimal;
			Assert.IsTrue( type.Equals(lhs, rhs) );
			
			// Test that two decimal fields that are equal except one has a higher precision than
			// the other one are returned as Equal by the DecimalType.
			rhs = 5.643510M;
			Assert.IsTrue(type.Equals(lhs, rhs));
		}
		
		[Test]
		public void ReadWrite() 
		{
			decimal expected = 5.64351M;

			DecimalClass basic = new DecimalClass();
			basic.Id = 1;
			basic.DecimalValue = expected;

			ISession s = sessions.OpenSession();
			s.Save(basic);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			basic = (DecimalClass)s.Load( typeof(DecimalClass), 1 );

			Assert.AreEqual( expected, basic.DecimalValue );
			Assert.AreEqual( 5.643510M, basic.DecimalValue );

			s.Delete( basic );
			s.Flush();
			s.Close();
		}
	}
}
