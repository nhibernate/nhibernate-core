using System;

using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// The Unit Tests for the BooleanType.
	/// </summary>
	[TestFixture]
	public class BooleanTypeFixture : TestCase 
	{
		#region NUnit.Framework.TestFixture Members

		[TestFixtureSetUp]
		public void TestFixtureSetUp() 
		{
			ExportSchema( new string[] { "TypesTest.BooleanClass.hbm.xml"}, true, "NHibernate.Test" );
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
			BooleanType type = (BooleanType)NHibernate.Boolean;

			Assert.IsTrue( type.Equals( true, true ) );
			Assert.IsTrue( type.Equals( false, false ) );
			Assert.IsFalse( type.Equals( true, false ) );
		}

		[Test]
		public void ReadWrite() 
		{
			BooleanClass basic = new BooleanClass();
			basic.Id = 1;
			basic.BooleanValue = true;

			ISession s = sessions.OpenSession();
			s.Save(basic);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			basic = (BooleanClass)s.Load( typeof(BooleanClass), 1 );

			Assert.AreEqual( true, basic.BooleanValue );

			s.Delete( basic );
			s.Flush();
			s.Close();
		}
	}
}
