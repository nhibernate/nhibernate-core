using System;
using System.Data;

using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// The Unit Tests for the ByteType.
	/// </summary>
	[TestFixture]
	public class ByteTypeFixture : BaseTypeFixture
	{
		/// <summary>
		/// Test that Get(IDataReader, index) returns a boxed Byte value that is what
		/// we expect.
		/// </summary>
		[Test]
		public void Get() 
		{
			NullableType type = NHibernate.Byte;

			byte expected = 5;
			
			// move to the first record
			reader.Read();

			byte actual = (byte)type.Get(reader, ByteTypeColumnIndex);
			Assert.AreEqual(expected, actual);

		}
	}
}

	