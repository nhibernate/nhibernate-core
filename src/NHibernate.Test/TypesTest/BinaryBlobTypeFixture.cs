using System;

using NHibernate;

using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Summary description for BinaryBlobTypeFixture.
	/// </summary>
	[TestFixture]
	public class BinaryBlobTypeFixture : TestCase 
	{
		#region NUnit.Framework.TestFixture Members

		[TestFixtureSetUp]
		public void TestFixtureSetUp() 
		{
			ExportSchema( new string[] { "TypesTest.BinaryBlobClass.hbm.xml"}, true, "NHibernate.Test" );
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
		public void ReadWrite() 
		{
			ISession s = OpenSession();
			BinaryBlobClass b = new BinaryBlobClass();
			b.BinaryBlob = System.Text.UnicodeEncoding.UTF8.GetBytes("foo/bar/baz");
			s.Save(b);
			s.Flush();
			s.Close();

			s = OpenSession();
			b = (BinaryBlobClass)s.Load( typeof(BinaryBlobClass), b.Id );
			ObjectAssert.AreEqual( System.Text.UnicodeEncoding.UTF8.GetBytes("foo/bar/baz") , b.BinaryBlob );
			s.Delete( b );
			s.Flush();
			s.Close();
		}
	}
}
