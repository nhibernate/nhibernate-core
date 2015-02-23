using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using NHibernate.Dialect;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Tests for mapping a byte[] Property to a BinaryType.
	/// </summary>
	[TestFixture]
	public class BinaryTypeFixture : TypeFixtureBase
	{
		protected override string TypeName
		{
			get { return "Binary"; }
		}

		/// <summary>
		/// Verify Equals will correctly determine when the property
		/// is dirty.
		/// </summary>
		[Test]
		public void Equals()
		{
			BinaryType type = (BinaryType) NHibernateUtil.Binary;

			byte[] expected = Encoding.UTF8.GetBytes("ghij1`23%$");
			byte[] expectedClone = Encoding.UTF8.GetBytes("ghij1`23%$");

			Assert.IsTrue(type.IsEqual(expected, expected));
			Assert.IsTrue(type.IsEqual(expected, expectedClone));
			Assert.IsFalse(type.IsEqual(expected, GetByteArray(15)));
		}

		/// <summary>
		/// Certain drivers (ie - Oracle) don't handle writing and reading null byte[] 
		/// to and from the db consistently.  Verify if this driver does.
		/// </summary>
		[Test]
		public void InsertNull()
		{
			BinaryClass bcBinary = new BinaryClass();
			bcBinary.Id = 1;

			bcBinary.DefaultSize = null;
			bcBinary.WithSize = null;

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Save(bcBinary);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			BinaryClass bcBinaryLoaded = (BinaryClass) s.Load(typeof(BinaryClass), 1);

			Assert.IsNotNull(bcBinaryLoaded);
			Assert.AreEqual(null, bcBinaryLoaded.DefaultSize,
			                "A property mapped as type=\"Byte[]\" with a null byte[] value was not saved & loaded as null");
			Assert.AreEqual(null, bcBinaryLoaded.WithSize,
			                "A property mapped as type=\"Byte[](length)\" with null byte[] value was not saved & loaded as null");

			s.Delete(bcBinaryLoaded);
			t.Commit();
			s.Close();
		}

		/// <summary>
		/// Certain drivers (ie - Oracle) don't handle writing and reading byte[0] 
		/// to and from the db consistently.  Verify if this driver does.
		/// </summary>
		[Test]
		public void InsertZeroLength()
		{
			if (Dialect is Oracle8iDialect)
			{
				Assert.Ignore("Certain drivers (ie - Oralce) don't handle writing and reading byte[0]");
			}
			BinaryClass bcBinary = new BinaryClass();
			bcBinary.Id = 1;

			bcBinary.DefaultSize = new byte[0];
			bcBinary.WithSize = new byte[0];

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Save(bcBinary);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			BinaryClass bcBinaryLoaded = (BinaryClass) s.Load(typeof(BinaryClass), 1);

			Assert.IsNotNull(bcBinaryLoaded);
			Assert.AreEqual(0, bcBinaryLoaded.DefaultSize.Length,
			                "A property mapped as type=\"Byte[]\" with a byte[0] value was not saved & loaded as byte[0]");
			Assert.AreEqual(0, bcBinaryLoaded.WithSize.Length,
			                "A property mapped as type=\"Byte[](length)\" with a byte[0] value was not saved & loaded as byte[0]");

			s.Delete(bcBinaryLoaded);
			t.Commit();
			s.Close();
		}

		/// <summary>
		/// Test the setting of values in Parameters and the reading of the 
		/// values out of the IDataReader.
		/// </summary>
		[Test]
		public void ReadWrite()
		{
			BinaryClass bcBinary = Create(1);
			BinaryClass expected = Create(1);

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Save(bcBinary);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			bcBinary = (BinaryClass) s.Load(typeof(BinaryClass), 1);

			// make sure what was saved was expected
			ObjectAssert.AreEqual(expected.DefaultSize, bcBinary.DefaultSize);
			ObjectAssert.AreEqual(expected.WithSize, bcBinary.WithSize);

			Assert.IsFalse(s.IsDirty(), "The session is dirty: an Update will be raised on commit, See NH-1246");

			s.Delete(bcBinary);
			t.Commit();
			s.Close();
		}

		private BinaryClass Create(int id)
		{
			BinaryClass bcBinary = new BinaryClass();
			bcBinary.Id = id;

			bcBinary.DefaultSize = GetByteArray(5);
			bcBinary.WithSize = GetByteArray(10);

			return bcBinary;
		}

		private byte[] GetByteArray(int value)
		{
			BinaryFormatter bf = new BinaryFormatter();
			MemoryStream stream = new MemoryStream();
			bf.Serialize(stream, value);
			return stream.ToArray();
		}
	}
}
