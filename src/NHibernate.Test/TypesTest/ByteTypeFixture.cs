using System;

using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// The Unit Tests for the ByteType.
	/// </summary>
	[TestFixture]
	public class ByteTypeFixture : TestCase
	{
		#region NUnit.Framework.TestFixture Members

		[TestFixtureSetUp]
		public void TestFixtureSetUp() 
		{
			ExportSchema( new string[] { "TypesTest.ByteClass.hbm.xml"}, true, "NHibernate.Test" );
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
			ByteType type = (ByteType)NHibernate.Byte;
			
			Assert.IsTrue( type.Equals( (byte)5, (byte)5 ) );
			Assert.IsFalse( type.Equals( (byte)5, (byte)6 ) );
			
		}

		[Test]
		public void ReadWrite() 
		{
			
			ByteClass basic = new ByteClass();
			basic.Id = 1;
			basic.ByteValue = (byte)43;

			ISession s = sessions.OpenSession();
			s.Save(basic);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			basic = (ByteClass)s.Load( typeof(ByteClass), 1 );

			Assert.AreEqual( (byte)43, basic.ByteValue );

			s.Delete( basic );
			s.Flush();
			s.Close();
		}
	}
}

	