using System;

using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Summary description for StringClobTypeFixture.
	/// </summary>
	[TestFixture]
	public class StringClobTypeFixture : TestCase
	{
		#region NUnit.Framework.TestFixture Members

		[TestFixtureSetUp]
		public void TestFixtureSetUp() 
		{
			ExportSchema( new string[] { "TypesTest.StringClobClass.hbm.xml"}, true, "NHibernate.Test" );
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
			ISession s = sessions.OpenSession();
			StringClobClass b = new StringClobClass();
			b.StringClob = "foo/bar/baz";
			s.Save(b);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			b = (StringClobClass)s.Load( typeof(StringClobClass), b.Id );
			Assert.AreEqual( "foo/bar/baz", b.StringClob );
			s.Delete( b );
			s.Flush();
			s.Close();
		}
	}
}
