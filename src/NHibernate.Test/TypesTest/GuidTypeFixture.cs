using System;

using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// The Unit Tests for the GuidType.
	/// </summary>
	[TestFixture]
	public class GuidTypeFixture : TestCase
	{
		#region NUnit.Framework.TestFixture Members

		[TestFixtureSetUp]
		public void TestFixtureSetUp() 
		{
			ExportSchema( new string[] { "TypesTest.GuidClass.hbm.xml"}, true, "NHibernate.Test" );
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
		/// Verify Equals will correctly determine when the property
		/// is dirty.
		/// </summary>
		[Test]
		public void Equals() 
		{
			GuidType type = (GuidType)NHibernate.Guid;
			
			Guid lhs = new Guid("{01234567-abcd-abcd-abcd-0123456789ab}");
			Guid rhs = new Guid("{01234567-abcd-abcd-abcd-0123456789ab}");
			Assert.IsTrue( type.Equals( lhs, rhs ) );

			rhs = new Guid("{11234567-abcd-abcd-abcd-0123456789ab}");

			Assert.IsFalse( type.Equals( lhs, rhs ) );
			
		}

		[Test]
		public void ReadWrite() 
		{
			Guid val = new Guid("{01234567-abcd-abcd-abcd-0123456789ab}");
			
			GuidClass basic = new GuidClass();
			basic.Id = 1;
			basic.GuidValue = val;

			ISession s = sessions.OpenSession();
			s.Save(basic);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			basic = (GuidClass)s.Load( typeof(GuidClass), 1 );

			Assert.AreEqual( val, basic.GuidValue );

			s.Delete( basic );
			s.Flush();
			s.Close();
		}
	}
}