using System;

using NHibernate.DomainModel.NHSpecific;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest
{
	/// <summary>
	/// Summary description for BlobberInMemoryFixture.
	/// </summary>
	[TestFixture]
	public class BlobberInMemoryFixture : TestCase
	{
		[SetUp]
		public void SetUp() 
		{
			ExportSchema( new string[] { "NHSpecific.BlobberInMemory.hbm.xml" } );
		}

		[Test]
		public void BlobClobInMemory() 
		{
			ISession s = sessions.OpenSession();
			BlobberInMemory b = new BlobberInMemory();
			b.BinaryBlob = System.Text.UnicodeEncoding.UTF8.GetBytes("foo/bar/baz");
			b.StringClob = "foo/bar/baz";
			s.Save(b);
			
			s.Flush();
			s.Refresh(b);
			b.BinaryBlob[0] = System.Text.UTF8Encoding.UTF8.GetBytes("a")[0];
			b.StringClob.Substring(2,3);
			b.StringClob = "a" + b.StringClob.Substring(1);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			b = (BlobberInMemory)s.Load( typeof(BlobberInMemory), b.Id );
			BlobberInMemory b2 = new BlobberInMemory();
			s.Save(b2);
			b2.BinaryBlob = b.BinaryBlob;
			b.BinaryBlob = null;
			Assert.AreEqual( "aoo", b.StringClob.Substring(0, 3) );
			b.StringClob.Substring(1, 6);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			b = (BlobberInMemory)s.Load( typeof(BlobberInMemory), b.Id );
			b.StringClob = "xcvfxvc xcvbx cvbx cvbx cvbxcvbxcvbxcvb";
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			b = (BlobberInMemory)s.Load( typeof(BlobberInMemory), b.Id );
			Assert.AreEqual( "xcvfxvc", b.StringClob.Substring(0, 7) );
			s.Flush();
			s.Close();
		}
	}
}
