using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using NHibernate.DomainModel.NHSpecific;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest
{
	/// <summary>
	/// Summary description for BasicBinaryFixture.
	/// </summary>
	[TestFixture]
	public class BasicBinaryFixture : TestCase 
	{

		[SetUp]
		public void SetUp() 
		{
			ExportSchema( new string[] { "NHSpecific.BasicBinary.hbm.xml"}, true );
		}

		[Test]
		public void InsertNull() 
		{
			BasicBinary bcBinary = new BasicBinary();
			bcBinary.Id = 1;

			bcBinary.DefaultSize = null;
			bcBinary.WithSize = null;

			ISession s = sessions.OpenSession();
			s.Save(bcBinary);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			BasicBinary bcBinaryLoaded = (BasicBinary)s.Load(typeof(BasicBinary), 1);

			Assert.IsNotNull(bcBinaryLoaded);
			Assert.AreEqual(null, bcBinary.DefaultSize);
			Assert.AreEqual(null, bcBinary.WithSize);

			s.Delete(bcBinaryLoaded);
			s.Flush();
			s.Close();
		}

		[Test]
		public void InsertZeroLength() 
		{
			BasicBinary bcBinary = new BasicBinary();
			bcBinary.Id = 1;

			bcBinary.DefaultSize = new byte[0];
			bcBinary.WithSize = new byte[0];

			ISession s = sessions.OpenSession();
			s.Save(bcBinary);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			BasicBinary bcBinaryLoaded = (BasicBinary)s.Load(typeof(BasicBinary), 1);

			Assert.IsNotNull(bcBinaryLoaded);
			Assert.AreEqual(0, bcBinary.DefaultSize.Length);
			Assert.AreEqual(0, bcBinary.WithSize.Length);

			s.Delete(bcBinaryLoaded);
			s.Flush();
			s.Close();
		}

		[Test]
		public void Insert() 
		{
			BasicBinary bcBinary = new BasicBinary();
			bcBinary.Id = 1;

			bcBinary.DefaultSize = GetByteArray(5);
			bcBinary.WithSize = GetByteArray(10);

			ISession s = sessions.OpenSession();
			s.Save(bcBinary);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();

			BasicBinary bcBinaryLoaded = (BasicBinary)s.Load(typeof(BasicBinary), 1);

			Assert.IsNotNull(bcBinaryLoaded);
			Assert.IsFalse(bcBinary==bcBinaryLoaded);

			ObjectAssertion.AssertEquals(bcBinary.DefaultSize, bcBinaryLoaded.DefaultSize);
			ObjectAssertion.AssertEquals(bcBinary.WithSize, bcBinaryLoaded.WithSize);

			s.Delete(bcBinaryLoaded);
			s.Flush();
			s.Close();
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
