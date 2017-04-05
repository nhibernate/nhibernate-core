using System;
using System.Text;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Summary description for BinaryBlobTypeFixture.
	/// </summary>
	[TestFixture]
	public class BinaryBlobTypeFixture : TypeFixtureBase
	{
		protected override string TypeName
		{
			get { return "BinaryBlob"; }
		}

		[Test]
		public void ReadWrite()
		{
			ISession s = OpenSession();
			BinaryBlobClass b = new BinaryBlobClass();
			b.BinaryBlob = UnicodeEncoding.UTF8.GetBytes("foo/bar/baz");
			s.Save(b);
			s.Flush();
			s.Close();

			s = OpenSession();
			b = (BinaryBlobClass) s.Load(typeof(BinaryBlobClass), b.Id);
			ObjectAssert.AreEqual(UnicodeEncoding.UTF8.GetBytes("foo/bar/baz"), b.BinaryBlob);
			s.Delete(b);
			s.Flush();
			s.Close();
		}

		[Test]
		public void ReadWriteLargeBlob()
		{
			ISession s = OpenSession();
			BinaryBlobClass b = new BinaryBlobClass();
			b.BinaryBlob = UnicodeEncoding.UTF8.GetBytes(new string('T', 10000));
			s.Save(b);
			s.Flush();
			s.Close();

			s = OpenSession();
			b = (BinaryBlobClass) s.Load(typeof(BinaryBlobClass), b.Id);
			ObjectAssert.AreEqual(UnicodeEncoding.UTF8.GetBytes(new string('T', 10000)), b.BinaryBlob);
			s.Delete(b);
			s.Flush();
			s.Close();
		}

		[Test]
		public void ReadWriteZeroLen()
		{
			object savedId;
			using (ISession s = OpenSession())
			{
				BinaryBlobClass b = new BinaryBlobClass();
				b.BinaryBlob = new byte[0];
				savedId = s.Save(b);
				s.Flush();
			}

			using (var s = OpenSession())
			{
				var b = s.Get<BinaryBlobClass>(savedId);
				Assert.That(b.BinaryBlob, Is.Not.Null.And.Length.EqualTo(0));
				s.Delete(b);
				s.Flush();
			}
		}
	}
}