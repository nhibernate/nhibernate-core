using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using NHibernate;
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
			BinaryType type = (BinaryType)NHibernateUtil.Binary;

			byte[] expected = System.Text.Encoding.UTF8.GetBytes("ghij1`23%$");
			byte[] expectedClone = System.Text.Encoding.UTF8.GetBytes("ghij1`23%$");

			Assert.IsTrue( type.Equals( expected, expected ) );
			Assert.IsTrue( type.Equals( expected, expectedClone ) );
			Assert.IsFalse( type.Equals( expected, GetByteArray( 15 ) ) );
		}

		/// <summary>
		/// Certain drivers (ie - Oralce) don't handle writing and reading null byte[] 
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
			s.Save(bcBinary);
			s.Flush();
			s.Close();

			s = OpenSession();
			BinaryClass bcBinaryLoaded = (BinaryClass)s.Load( typeof(BinaryClass), 1 );

			Assert.IsNotNull(bcBinaryLoaded);
			Assert.AreEqual(null, bcBinaryLoaded.DefaultSize, "A property mapped as type=\"Byte[]\" with a null byte[] value was not saved & loaded as null");
			Assert.AreEqual(null, bcBinaryLoaded.WithSize, "A property mapped as type=\"Byte[](length)\" with null byte[] value was not saved & loaded as null");

			s.Delete(bcBinaryLoaded);
			s.Flush();
			s.Close();
		}

		/// <summary>
		/// Certain drivers (ie - Oralce) don't handle writing and reading byte[0] 
		/// to and from the db consistently.  Verify if this driver does.
		/// </summary>
		[Test]
		public void InsertZeroLength() 
		{
			if (typeof(Dialect.Oracle9Dialect).IsInstanceOfType( dialect)) {
				return;
			}
			BinaryClass bcBinary = new BinaryClass();
			bcBinary.Id = 1;

			bcBinary.DefaultSize = new byte[0];
			bcBinary.WithSize = new byte[0];

			ISession s = OpenSession();
			s.Save(bcBinary);
			s.Flush();
			s.Close();

			s = OpenSession();
			BinaryClass bcBinaryLoaded = (BinaryClass)s.Load( typeof(BinaryClass), 1 );

			Assert.IsNotNull(bcBinaryLoaded);
			Assert.AreEqual(0, bcBinaryLoaded.DefaultSize.Length, "A property mapped as type=\"Byte[]\" with a byte[0] value was not saved & loaded as byte[0]");
			Assert.AreEqual(0, bcBinaryLoaded.WithSize.Length, "A property mapped as type=\"Byte[](length)\" with a byte[0] value was not saved & loaded as byte[0]");

			s.Delete(bcBinaryLoaded);
			s.Flush();
			s.Close();
		}

		/// <summary>
		/// Test the setting of values in Parameters and the reading of the 
		/// values out of the IDataReader.
		/// </summary>
		[Test]
		public void ReadWrite() 
		{
			BinaryClass bcBinary = Create( 1 );
			BinaryClass expected = Create( 1 );
			
			ISession s = OpenSession();
			s.Save( bcBinary );
			s.Flush();
			s.Close();

			s = OpenSession();
			bcBinary = (BinaryClass)s.Load( typeof(BinaryClass), 1 );

			// make sure what was saved was expected
			ObjectAssert.AreEqual( expected.DefaultSize, bcBinary.DefaultSize );
			ObjectAssert.AreEqual( expected.WithSize, bcBinary.WithSize );

			s.Delete( bcBinary );
			s.Flush();
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
